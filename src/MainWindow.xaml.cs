using Masterstrap.Services;
using Masterstrap.Models;
using Masterstrap.Views;
using Masterstrap.Helpers;
using Masterstrap.Controls;
using Microsoft.Win32;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Navigation;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Documents;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Masterstrap
{
    public partial class MainWindow : Window, IComponentConnector
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int processId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool IsWindow(IntPtr hWnd);

        [DllImport("psapi.dll", SetLastError = true)]
        private static extern bool EmptyWorkingSet(IntPtr hProcess);

        private AppSettingsManager _settingsManager;
        private FFlagService? _fflagService;
        private DispatcherTimer _statusTimer;
        private DispatcherTimer _clockTimer;
        private bool _statusDotState = true;
        private int _flagsCount = 0;
        private int _cacheSlotCount = 0;
        private string _loadedFlagsPath = "";
        private string _fflagsInfoPanelDisplayName = "";
        private ObservableCollection<FlagItem> _editableFlagsList = new ObservableCollection<FlagItem>();
        private List<FlagItem> _allFlagsList = new List<FlagItem>();
        private string _editorCategoryFilter = "All";
        private List<FlagItem> _hiddenFlagsByFastMode = new List<FlagItem>();
        private readonly Dictionary<string, string> _fastModeOriginalValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private string _robloxVersion = "version-unknown";
        private string _softwareVersion = "version-unknown";
        private string _robloxExecutablePath = "";
        private bool _hasLocalRobloxInstall = false;
        private int _lastRobloxProcessCount = 0;
        private bool _hasUnsavedChanges = false;
        private bool _initializationComplete = false;
        private bool _deferVisibleStartupForLaunchShell = false;
        private static readonly HttpClient _httpClient = new HttpClient();

        private bool _closingForSaveAndLaunch = false;
        private bool _skipSaveForNextLaunchOnlyFlow = false;
        private bool _saveAndLaunchMode = false;

        private EventWaitHandle _showWindowEvent = null;

        private bool HasUnsavedChanges
        {
            get { return this._hasUnsavedChanges; }
            set { this._hasUnsavedChanges = value; }
        }

        private bool _isApplyingLanguage;
        private string _currentDisplayLanguage = "English";
        private bool _isApplyingGlobalTheme;
        private string _currentGlobalTheme = "Default";
        private string _currentUiTheme = "Black";
        private string _customBackgroundImagePath = string.Empty;
        private bool _isPresetFlagsExpanded = false;
        private List<FlagItem> _presetFlagsPreviousView = new List<FlagItem>();
        private bool _suppressSaveAndLaunchNoiseLogs;
        private Unlock240FpsMode _unlock240FpsMode = Unlock240FpsMode.FFlag;
        private int _unlock240GlobalFpsRequested = 240;
        private bool _isSyncingUnlock240GlobalFpsUi;
        public MainWindow()
        {
            this.SetupAssemblyResolution();
            this._settingsManager = new AppSettingsManager();
            string savedLanguage = this._settingsManager.GetDisplayLanguage();
            if (string.IsNullOrWhiteSpace(savedLanguage)) savedLanguage = "English";
            this._currentDisplayLanguage = savedLanguage;
            this._currentGlobalTheme = this._settingsManager.GetGlobalTheme();
            this._currentUiTheme = this._settingsManager.GetUiTheme();
            this._customBackgroundImagePath = this._settingsManager.GetCustomBackgroundImagePath();
            LocalizationService.SetLanguage(savedLanguage);
            this.InitializeComponent();

            try
            {
                if (this.FastModeToggle != null)
                    this.FastModeToggle.IsChecked = this._settingsManager.IsFastModeEnabled();
            }
            catch { }

            this.InitializeRemaining();
            this.Closed += MainWindow_Closed;
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(async () =>
            {
                try
                {
                    if (!string.Equals(this._currentDisplayLanguage, "English", StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(this._currentDisplayLanguage, "EnglishCanada", StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(this._currentDisplayLanguage, "SouthAfrica", StringComparison.OrdinalIgnoreCase))
                    {
                        this.TranslateVisualTree(this);
                    }
                }
                catch { }

            }), System.Windows.Threading.DispatcherPriority.ContextIdle);
        }


        private void MainWindow_Closed(object sender, EventArgs e)
        {
            try
            {

                if (this._overlayWindow != null)
                {
                    this._overlayWindow.Close();
                    this._overlayWindow = null;
                }

                if (this._closingForSaveAndLaunch)
                {
                    this._closingForSaveAndLaunch = false;
                    this.Log("[System] MainWindow closed during Save and Launch transition. App remains active.");
                    return;
                }
bool hasOtherWindows = false;
                foreach (Window window in Application.Current.Windows)
                {
                    if (window == this || window is OverlayWindow)
                        continue;
                    if (window.IsVisible)
                    {
                        hasOtherWindows = true;
                        break;
                    }
                }

                if (!hasOtherWindows)
                {
                    this.Log("[System] MainWindow closed - shutting down application.");
                    Application.Current?.Shutdown();
                }
            }
            catch { }
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                var placeholder = this.FindName("SearchPlaceholder") as TextBlock;
                if (placeholder != null)
                    placeholder.Visibility = Visibility.Collapsed;
            }
            catch { }
        }

        private void InitializeRemaining()
        {
            try
            {
                var placeholder = this.FindName("SearchPlaceholder") as TextBlock;
                if (placeholder != null)
                    placeholder.Visibility = string.IsNullOrEmpty(this.SearchFlagsBox?.Text) ? Visibility.Visible : Visibility.Collapsed;
            }
            catch { }
            this._statusTimer = new DispatcherTimer();
            this._clockTimer = new DispatcherTimer();
            this.SetupStatusAnimation();
            this.SetupClockTimer();
            this.UpdateStatus("Initializing...", Colors.Yellow);
            this.UpdateFileInfo("Not loaded", "Not loaded");
            this.UpdateCounts(0, 0, 0);
            this.Log("[System] Master FastFlag v1.0");
            this.Log("[System] Loading service...");

            this.FlagsDataGrid.ItemsSource = this._editableFlagsList;

            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    this.InitializeActivityLog();

                    this.InitializeAccountManager();

                    Task.Run(async () =>
                    {
                        await CommunityModService.EnsureSkyboxPackDownloadedAsync(true, msg => this.Log(msg));
                        await CursorModService.EnsureCursorPackDownloadedAsync(msg => this.Log(msg));

                        this.Dispatcher.Invoke(() => this.InitializeModsTab());
                    });

                    this.InitializeLinkPsTab();
                    this.InitializeService();
                    _selectedAccount = this._settingsManager.GetSelectedAccount();
                    this.PopulateAccountDropdown();

                    this.SizeChanged += MainWindow_SizeChanged;

                }
                catch (Exception ex)
                {
                    this.Log($"[System] Deferred initialization error: {ex.Message}");
                }
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
        }

        protected override async void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            try
            {
                Helpers.AcrylicHelper.UseBlurBehind = string.Equals(this._currentGlobalTheme, "GlassmorphicBlur", StringComparison.OrdinalIgnoreCase);
                Helpers.AcrylicHelper.ApplyAcrylicEffect(this);
            }
            catch { }

            if (!this._deferVisibleStartupForLaunchShell)
            {
                this.WindowState = WindowState.Normal;
                this.Show();
            }

            this.StartListeningForShowEvent();

            this.UpdateTabBorderHighlight(0);

            this._initializationComplete = false;

            this.RestoreToggleStates();

            this.RestoreRenderingSettings();

            this.RestoreFastFlagSettingsState();

            this.RestoreLanguageSettings();

            this.RestoreGlobalThemeSettings();

            this._initializationComplete = true;

            this.AttachToggleEventHandlers();

            this.AttachRenderingEventHandlers();

            this.HasUnsavedChanges = false;
            this.Log("[System] ✓ UI restored from settings - unsaved changes flag reset");

            try
            {
                this.UpdateUnlock240GlobalFpsUiAndVisibility();
            }
            catch { }

            this.AutoLoadFlagsIfEnabled();

            this.EnableUserInteractionControls();
            this.UpdateStatus("Ready", Colors.LimeGreen);
            this.Log("[System] ✓ Ready");


            this.Log("[System] ✓ Initialization complete - change detection enabled");

            if (!this._deferVisibleStartupForLaunchShell)
                App.RegisterShowWindowActivationTarget(this);
        }

        public void PrepareBootstrapHiddenForLaunchShell()
        {
            this._deferVisibleStartupForLaunchShell = true;
            this.ShowInTaskbar = false;
            this.WindowStartupLocation = WindowStartupLocation.Manual;
            this.Left = -16000;
            this.Top = -16000;
            this.Width = 480;
            this.Height = 360;
        }

        public void AbandonLaunchShellHiddenBootstrap()
        {
            if (!this._deferVisibleStartupForLaunchShell)
                return;
            this._deferVisibleStartupForLaunchShell = false;
            try
            {
                this.ShowInTaskbar = true;
                this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                this.WindowState = WindowState.Normal;
                this.Left = double.NaN;
                this.Top = double.NaN;
                this.Width = double.NaN;
                this.Height = double.NaN;
                this.Show();
                this.Activate();
            }
            catch { /* ignore */ }
        }

        public bool IsLaunchShellHiddenBootstrapActive => this._deferVisibleStartupForLaunchShell;


    private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Tab)
        {
            e.Handled = true;
            return;
        }

        if (e.Key == Key.F11)
        {
            this.WindowState = this.WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
            e.Handled = true;
        }
    }

    private void ChromeDragRegion_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton != MouseButton.Left)
            return;

        if (ChromeDragRegion_IsInteractiveControl(e.OriginalSource as DependencyObject))
            return;

        if (e.ClickCount == 2)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
        }
        else
        {
            try
            {
                this.DragMove();
            }
            catch
            {
            }
        }
    }

    private static bool ChromeDragRegion_IsInteractiveControl(DependencyObject? source)
    {
        while (source != null)
        {
            switch (source)
            {
                case ButtonBase:
                case Thumb:
                case TextBoxBase:
                case ComboBox:
                case Slider:
                case PasswordBox:
                    return true;
            }
            source = System.Windows.Media.VisualTreeHelper.GetParent(source);
        }
        return false;
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
        try
        {
            if (this._closingForSaveAndLaunch)
            {
                this._statusTimer?.Stop();
                this._clockTimer?.Stop();
                this.StopProxyStatusTimerIfAny();
                e.Cancel = false;
                this.Log("[System] Closing MainWindow for Save and Launch (progress window continues)");
                return;
            }

            bool hasPendingThemeChange = this.HasPendingGlobalThemeChange();
            this.Log($"[System] Window_Closing called. HasUnsavedChanges={this.HasUnsavedChanges}, PendingThemeChange={hasPendingThemeChange}");

            if ((this.HasUnsavedChanges || hasPendingThemeChange) && this._initializationComplete)
            {
                this.Log("[System] ⚠ Unsaved changes detected - showing dialog");

                var dialog = new Masterstrap.Views.UnsavedChangesDialog(this)
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };

                this.Log("[System] Dialog created, calling ShowDialog()");
                try
                {
                    bool? result = dialog.ShowDialog();
                    this.Log($"[System] Dialog result: {result}");

                    if (result == true)
                    {
                        if (dialog.Result == Masterstrap.Views.UnsavedChangesResult.Save)
                        {
                            this.SaveToggleStates();
                            this.SaveModsSettingsFromUi();
                            this.HasUnsavedChanges = false;
                            this.Log("[System] ✓ Settings saved before closing");
                        }
                        else if (dialog.Result == Masterstrap.Views.UnsavedChangesResult.DontSave)
                        {
                            this.Log("[System] ℹ Discarding unsaved changes");
                        }

                        e.Cancel = false;
                        this._statusTimer?.Stop();
                        this._clockTimer?.Stop();
                        this.StopProxyStatusTimerIfAny();
                        this.Log("[System] ✓ Application closing");
                    }
                    else
                    {
                        e.Cancel = true;
                        this.Log("[System] ℹ Application close cancelled");
                    }
                }
                catch (Exception dialogEx)
                {
                    this.Log($"[System] ✗ Error showing dialog: {dialogEx.Message}");
                    this.Log($"[System] ✗ Stack trace: {dialogEx.StackTrace}");
                    e.Cancel = false;
                    this._statusTimer?.Stop();
                    this._clockTimer?.Stop();
                    this.StopProxyStatusTimerIfAny();
                }
            }
            else
            {
                this._statusTimer?.Stop();
                this._clockTimer?.Stop();
                this.StopProxyStatusTimerIfAny();
                this.Log("[System] ✓ Application closing");
            }
        }
        catch (Exception ex)
        {
            this.Log($"[System] Error during close: {ex.Message}");
        }
    }

    private bool HasPendingGlobalThemeChange()
    {
        try
        {
            string savedEffect = this._settingsManager?.GetGlobalTheme() ?? "Default";
            string savedUi = this._settingsManager?.GetUiTheme() ?? "Black";
            string currentEffect = this.GetSelectedEffectTheme();
            string currentUi = this.GetSelectedUiTheme();
            return !string.Equals(savedEffect, currentEffect, StringComparison.OrdinalIgnoreCase)
                || !string.Equals(savedUi, currentUi, StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            if (e.RemovedItems.Count > 0)
            {
                if (e.RemovedItems[0] is TabItem removedTab && ReferenceEquals(removedTab, this.AboutMainTabItem))
                    this.ResetAboutTabScrollPositions();

                if (e.RemovedItems[0] is TabItem oldTab && oldTab.Content is FrameworkElement oldContent)
                {
                    Storyboard fadeOut = this.FindResource("TabFadeOutAnimation") as Storyboard;
                    if (fadeOut != null)
                        fadeOut.Begin(oldContent, true);
                }

                if (e.AddedItems.Count > 0 && e.AddedItems[0] is TabItem newTab && newTab.Content is FrameworkElement newContent)
                {
                    newContent.Opacity = 0;
                    Storyboard fadeIn = this.FindResource("TabFadeInAnimation") as Storyboard;
                    if (fadeIn != null)
                        fadeIn.Begin(newContent, true);
                }
            }


            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    bool g = string.Equals(this._currentGlobalTheme, "Glassmorphic", StringComparison.OrdinalIgnoreCase)
                             || string.Equals(this._currentGlobalTheme, "GlassmorphicBlur", StringComparison.OrdinalIgnoreCase);
                    bool b = string.Equals(this._currentGlobalTheme, "GlassmorphicBlur", StringComparison.OrdinalIgnoreCase);
                    this.ApplyTabPageGlassCardPanels(g, b);
                }
                catch
                {
                }
            }), DispatcherPriority.Loaded);

            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    int idx = this.MainTabControl?.SelectedIndex ?? -1;
                    this.TranslateVisualTreeMainTabContent(idx);
                }
                catch
                {
                }
            }), DispatcherPriority.Loaded);
        }
        catch (Exception ex)
        {
            this.Log($"[Transition] Error during tab transition: {ex.Message}");
        }
    }

    private void ResetAboutTabScrollPositions()
    {
        try
        {
            if (this.FAQTabContent == null)
                return;
            this.ResetScrollViewersToHome(this.FAQTabContent);
        }
        catch (Exception ex)
        {
            this.Log($"[About] Reset scroll: {ex.Message}");
        }
    }

    private void AboutSubTabsControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            bool g = string.Equals(this._currentGlobalTheme, "Glassmorphic", StringComparison.OrdinalIgnoreCase)
                     || string.Equals(this._currentGlobalTheme, "GlassmorphicBlur", StringComparison.OrdinalIgnoreCase);
            bool b = string.Equals(this._currentGlobalTheme, "GlassmorphicBlur", StringComparison.OrdinalIgnoreCase);
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    this.ApplyTabPageGlassCardPanels(g, b);
                }
                catch
                {
                }
            }), DispatcherPriority.Loaded);
        }
        catch
        {
        }
    }

    private void ResetScrollViewersToHome(DependencyObject root)
    {
        if (root == null)
            return;
        if (root is ScrollViewer sv)
            sv.ScrollToHome();
        int n = VisualTreeHelper.GetChildrenCount(root);
        for (int i = 0; i < n; i++)
            this.ResetScrollViewersToHome(VisualTreeHelper.GetChild(root, i));
    }

    private void InitializeService()
    {
        try
        {
            this.LoadVersionInformation();
            this.EnableAllButtons();
        }
        catch (Exception ex)
        {
            this.Log("[System] Fatal Error: " + ex.Message);
            this.UpdateStatus("Fatal Error", Colors.Red);
            this.DisableButtons();
        }
    }

    private bool TryAutoLoadFlagsFromPersistedSession(string logPrefix)
    {
        return false;
    }


    private void SetupAssemblyResolution()
    {
        AppDomain.CurrentDomain.AssemblyResolve += (ResolveEventHandler)((sender, args) =>
        {
            try
            {
                string assemblyShortName = new AssemblyName(args.Name).Name;
                Assembly assembly = ((IEnumerable<Assembly>)AppDomain.CurrentDomain.GetAssemblies()).FirstOrDefault<Assembly>((Func<Assembly, bool>)(a => a.GetName().Name == assemblyShortName));
                if (assembly != null)
                    return assembly;
                string str1 = assemblyShortName + ".dll";
                string[] strArray = new string[3]
                {
          System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, str1),
          System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Libs", str1),
          System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName) ?? "", str1)
                };
                foreach (string str2 in strArray)
                {
                    if (File.Exists(str2))
                        return Assembly.LoadFrom(str2);
                }
            }
            catch
            {
            }
            return (Assembly)null;
        });
    }


    private void LoadFlagsBtn_Click(object sender, RoutedEventArgs e)
    {
        try
        {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.Title = "Select FFlags JSON File";
                openFileDialog1.Filter = "JSON files (*.json)|*.json|Text files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog1.DefaultExt = ".json";
                OpenFileDialog openFileDialog2 = openFileDialog1;
                if (!openFileDialog2.ShowDialog().GetValueOrDefault())
                    return;
                this._loadedFlagsPath = openFileDialog2.FileName;
                this._fflagsInfoPanelDisplayName = "";
                this.UpdateStatus("Loading...", Colors.Yellow);
                this.Log("[File] Loading: " + System.IO.Path.GetFileName(this._loadedFlagsPath));
                this.AddActivityEntry($" Loading FFlags: {System.IO.Path.GetFileName(this._loadedFlagsPath)}", Colors.Yellow);
                this.UpdateStatus("Loaded", Colors.LimeGreen);
                this.UpdateCounts(this._flagsCount, this._cacheSlotCount, this.GetRobloxProcessCount());
                this.Log($"[Success] Loaded {this._flagsCount} FFlags");
                this.AddActivityEntry($"✓ Loaded {this._flagsCount} FFlags successfully", Colors.LimeGreen);

                FileInfo flagsFileInfo = new FileInfo(this._loadedFlagsPath);
                string flagsUpdateTime = flagsFileInfo.LastWriteTime.ToString("dd/MM/yyyy HH:mm");

                this._settingsManager.SetLastGamePresetTag("");
                this._settingsManager.SetLastFflagsPanelLabel("");
                this.Dispatcher.Invoke(this.UpdateFFlagsCountDisplay);

                this.Log($"[Info] FFlags file: {System.IO.Path.GetFileName(this._loadedFlagsPath)}");
                this.Log($"[Info] Updated: {flagsUpdateTime}");

                this._settingsManager.SetLastFlagJsonPath(this._loadedFlagsPath);
                this.Log("[Cache] ✓ FFlag JSON path saved for auto-load next time");
                this.AddActivityEntry($"✓ Cached FFlags path for auto-load", Colors.LightSeaGreen);

                this.PopulateEditableFlags();

                this.DisplayJsonContentInLog(this._loadedFlagsPath);

                this.Dispatcher.Invoke(() =>
                {
                    this.UpdateFFlagsCountDisplay();
                });
                this.HasUnsavedChanges = false;
        }
        catch (Exception ex)
        {
            this.UpdateStatus("Error", Colors.Red);
            this.Log("[Error] " + ex.Message);
        }
    }

    private async Task<bool> EnsureRobloxInstalledAsync()
    {
        try
        {
            this.Log("[Roblox] === ENSURE ROBLOX INSTALLED ===");

            var (success, robloxExePath) = await RobloxInstallationManager.EnsureRobloxInstalledAsync(
                this.Log,
                status =>
                {
                    try
                    {
                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            Color c = Colors.Yellow;
                            if (!string.IsNullOrWhiteSpace(status) &&
                                status.IndexOf("fail", StringComparison.OrdinalIgnoreCase) >= 0)
                                c = Colors.Red;
                            else if (string.Equals(status, "Roblox installed", StringComparison.OrdinalIgnoreCase))
                                c = Colors.LimeGreen;
                            this.UpdateStatus(status ?? string.Empty, c);
                        }));
                    }
                    catch { /* ignore UI */ }
                });

            if (!success || string.IsNullOrWhiteSpace(robloxExePath) || !System.IO.File.Exists(robloxExePath))
            {
                this.Log("[Roblox] ✗ Roblox installation/verification failed");
                this.Dispatcher.Invoke(() => this.UpdateStatus("Roblox Setup Failed", Colors.Red));
                return false;
            }

            this._robloxExecutablePath = robloxExePath;
            string? versionDir = System.IO.Path.GetDirectoryName(robloxExePath);
            string? versionFolder = string.IsNullOrWhiteSpace(versionDir)
                ? null
                : System.IO.Path.GetFileName(versionDir.TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar));

            if (!string.IsNullOrWhiteSpace(versionFolder) && versionFolder.StartsWith("version-", StringComparison.OrdinalIgnoreCase))
            {
                this._hasLocalRobloxInstall = true;
                DateTime exeTime = System.IO.File.GetLastWriteTimeUtc(robloxExePath);
                string robloxUpdateTime = exeTime.ToLocalTime().ToString("dd/MM/yyyy HH:mm");
                this._robloxVersion = $"{versionFolder} (last update: {robloxUpdateTime})";

                this.Dispatcher.Invoke(() =>
                {
                    this.InfoRobloxVersion.Text = versionFolder;
                    this.InfoRobloxUpdate.Text = $"Last update: {robloxUpdateTime}";
                });

                this.Log($"[Roblox] ✓ {versionFolder} → {robloxExePath}");
            }
            else
            {
                this._robloxVersion = versionFolder ?? "version-unknown";
                this.Log($"[Roblox] ✓ {robloxExePath}");
            }

            this.Dispatcher.Invoke(() => this.UpdateStatus("Roblox ready", Colors.LimeGreen));
            return true;
        }
        catch (Exception ex)
        {
            this.Log($"[Roblox] ✗ Error in EnsureRobloxInstalledAsync: {ex.Message}");
            this.Dispatcher.Invoke(() =>
            {
                this.UpdateStatus("Installation failed", Colors.Red);
                this.AddActivityEntry($"❌ Error: {ex.Message}", Colors.IndianRed);
            });
            return false;
        }
    }

    private class RobloxVersionData
    {
        public string ClientVersionUpload { get; set; }
        public string Version { get; set; }
        public string BootstrapperVersion { get; set; }
    }

    private async Task<RobloxVersionData> FetchRobloxVersionDataAsync()
    {
        try
        {
            using (var client = new HttpClient { Timeout = TimeSpan.FromSeconds(15) })
            {
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");

                string versionUrl = "https://clientsettings.roblox.com/v2/client-version/WindowsPlayer";
                this.Log($"[Roblox] HTTP GET: {versionUrl}");

                string jsonResponse = await client.GetStringAsync(versionUrl);
                this.Log($"[Roblox] ✓ Response received");
                this.Log($"[Roblox] Raw JSON: {jsonResponse}");

                using (var doc = System.Text.Json.JsonDocument.Parse(jsonResponse))
                {
                    var root = doc.RootElement;

                    this.Log($"[Roblox] JSON fields found:");
                    foreach (var property in root.EnumerateObject())
                    {
                        string value = property.Value.ValueKind == System.Text.Json.JsonValueKind.String
                            ? property.Value.GetString()
                            : property.Value.ToString();
                        this.Log($"[Roblox]   - {property.Name}: {value}");
                    }

                    if (root.TryGetProperty("clientVersionUpload", out JsonElement clientVersionUpload))
                    {
                        string hash = clientVersionUpload.GetString();
                        this.Log($"[Roblox] ✓ Extracted clientVersionUpload: {hash}");

                        if (string.IsNullOrWhiteSpace(hash))
                        {
                            throw new Exception("clientVersionUpload value is empty");
                        }

                        string version = "";
                        if (root.TryGetProperty("version", out JsonElement versionProp))
                        {
                            version = versionProp.GetString() ?? "";
                            this.Log($"[Roblox] ✓ Version: {version}");
                        }

                        string bootstrapperVersion = "";
                        if (root.TryGetProperty("bootstrapperVersion", out JsonElement bootstrapProp))
                        {
                            bootstrapperVersion = bootstrapProp.GetString() ?? "";
                            this.Log($"[Roblox] ✓ Bootstrapper Version: {bootstrapperVersion}");
                        }

                        return new RobloxVersionData
                        {
                            ClientVersionUpload = hash,
                            Version = version,
                            BootstrapperVersion = bootstrapperVersion
                        };
                    }
                    else
                    {
                        this.Log($"[Roblox] ✗ Field 'clientVersionUpload' not found in JSON response");
                        throw new Exception("clientVersionUpload field not found in API response");
                    }
                }
            }
        }
        catch (HttpRequestException httpEx)
        {
            this.Log($"[Roblox] ✗ HTTP Error: {httpEx.Message}");
            this.Log($"[Roblox] ✗ Status Code: {httpEx.StatusCode}");
            throw;
        }
        catch (System.Text.Json.JsonException jsonEx)
        {
            this.Log($"[Roblox] ✗ JSON Parse Error: {jsonEx.Message}");
            throw;
        }
        catch (Exception ex)
        {
            this.Log($"[Roblox] ✗ Error fetching version data: {ex.Message}");
            this.Log($"[Roblox] ✗ Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    private async Task DownloadRobloxAsync(string downloadUrl, string savePath)
    {
        try
        {
            this.Log($"[Roblox] Creating HTTP client for download...");

            string[] cdnBaseUrls = new string[]
            {
                "https://setup.rbxcdn.com",
                "https://setup-aws.rbxcdn.com",
                "https://setup-ak.rbxcdn.com",
                "https://roblox-setup.cachefly.net",
                "https://s3.amazonaws.com/setup.roblox.com"
            };

            string fileName = System.IO.Path.GetFileName(downloadUrl);

            Exception lastException = null;

            foreach (var cdnBaseUrl in cdnBaseUrls)
            {
                try
                {
                    string cdnDownloadUrl = $"{cdnBaseUrl}/{fileName}";
                    this.Log($"[Roblox] Attempting CDN: {cdnBaseUrl}");

                    using (var client = new HttpClient { Timeout = TimeSpan.FromSeconds(600) })
                    {
                        client.DefaultRequestHeaders.Add("User-Agent", "Roblox/WinInet");
                        client.DefaultRequestHeaders.Add("Accept", "*/*");
                        client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
                        client.DefaultRequestHeaders.Add("Referer", "https://www.roblox.com/");
                        client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");

                        this.Log($"[Roblox] HTTP GET: {cdnDownloadUrl}");
                        using (var request = new HttpRequestMessage(HttpMethod.Get, cdnDownloadUrl))
                        {
                            using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
                            {
                                this.Log($"[Roblox] Response Status: {(int)response.StatusCode} {response.StatusCode}");

                                if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                                {
                                    this.Log($"[Roblox] ✗ CDN {cdnBaseUrl} returned 403 Forbidden (rate limited), trying next CDN...");
                                    await Task.Delay(1000);
                                    lastException = new Exception($"HTTP 403: Forbidden from {cdnBaseUrl}");
                                    continue;
                                }

                                if (!response.IsSuccessStatusCode)
                                {
                                    this.Log($"[Roblox] ✗ HTTP Error: {(int)response.StatusCode} {response.StatusCode}");
                                    this.Log($"[Roblox] ✗ Reason: {response.ReasonPhrase}");

                                    string content = await response.Content.ReadAsStringAsync();
                                    if (!string.IsNullOrWhiteSpace(content))
                                    {
                                        this.Log($"[Roblox] ✗ Response body: {content.Substring(0, Math.Min(200, content.Length))}");
                                    }

                                    lastException = new Exception($"HTTP {(int)response.StatusCode}: {response.ReasonPhrase}");
                                    continue;
                                }

                                this.Log($"[Roblox] ✓ Successfully connected to CDN: {cdnBaseUrl}");

                                long? totalBytes = response.Content.Headers.ContentLength;
                                double totalMB = totalBytes.HasValue ? (totalBytes.Value / 1024.0 / 1024.0) : 0;
                                this.Log($"[Roblox] Total size: {totalMB:F2} MB");

                                using (var contentStream = await response.Content.ReadAsStreamAsync())
                                using (var fileStream = System.IO.File.Create(savePath))
                                {
                                    var buffer = new byte[131072];
                                    long totalRead = 0;
                                    bool moreToRead = true;
                                    var sw = System.Diagnostics.Stopwatch.StartNew();
                                    var lastLogTime = sw.Elapsed;

                                    while (moreToRead)
                                    {
                                        int read = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                                        if (read == 0)
                                        {
                                            moreToRead = false;
                                            continue;
                                        }

                                        await fileStream.WriteAsync(buffer, 0, read);
                                        totalRead += read;

                                        double elapsedSec = Math.Max(0.0001, sw.Elapsed.TotalSeconds);
                                        double mbPerSec = (totalRead / 1024.0 / 1024.0) / elapsedSec;
                                        double percentComplete = totalBytes.HasValue ? (totalRead * 100.0) / totalBytes.Value : 0;

                                        if ((sw.Elapsed - lastLogTime).TotalSeconds >= 1.0 || (totalBytes.HasValue && percentComplete >= 100.0))
                                        {
                                            lastLogTime = sw.Elapsed;

                                            if (totalBytes.HasValue)
                                            {
                                                this.Log($"[Roblox] {percentComplete:F1}% — {mbPerSec:F2} MB/s ({(totalRead / 1024.0 / 1024.0):F2}/{totalMB:F2} MB)");
                                                this.Dispatcher.Invoke(() =>
                                                {
                                                    this.AddActivityEntry($"⬇️ {percentComplete:F1}% — {mbPerSec:F2} MB/s", Colors.Blue);
                                                });
                                            }
                                            else
                                            {
                                                this.Log($"[Roblox] {(totalRead / 1024.0 / 1024.0):F2} MB — {mbPerSec:F2} MB/s");
                                            }
                                        }
                                    }

                                    sw.Stop();
                                    double avgMbPerSec = (totalRead / 1024.0 / 1024.0) / Math.Max(0.0001, sw.Elapsed.TotalSeconds);
                                    this.Log($"[Roblox] ✓ Download complete: {savePath}");
                                    this.Log($"[Roblox] ✓ File size: {(totalRead / 1024.0 / 1024.0):F2} MB");
                                    this.Log($"[Roblox] ✓ Average speed: {avgMbPerSec:F2} MB/s");
                                    this.Log($"[Roblox] ✓ Total time: {sw.Elapsed:hh\\:mm\\:ss}");
                                    return;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.Log($"[Roblox] ✗ CDN {cdnBaseUrl} error: {ex.Message}");
                    lastException = ex;
                    await Task.Delay(1000);
                }
            }

            this.Log($"[Roblox] ✗ All CDNs failed to download file");
            throw new Exception($"Failed to download from any CDN. Last error: {lastException?.Message ?? "Unknown error"}");
        }
        catch (Exception ex)
        {
            this.Log($"[Roblox] ✗ Download error: {ex.Message}");
            throw;
        }
    }

    private void RestoreFlagsBtn_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            this.EnsureFFlagServiceInitialized();
            int restored = this._fflagService?.RestoreClientSettingsFromBackup(this._robloxExecutablePath) ?? 0;
            this._fflagService?.ClearQueuedFlags();
            this.Log(restored > 0
                ? $"[ClientSettings] Restored {restored} ClientAppSettings backup file(s)."
                : "[ClientSettings] No Masterstrap backup found to restore.");
        }
        catch (Exception ex)
        {
            this.Log($"[ClientSettings] Restore failed: {ex.Message}");
        }
    }

    private static string TryGetGamePresetCardTitle(Button presetButton)
    {
        try
        {
            if (presetButton?.Parent is StackPanel sp)
            {
                int idx = sp.Children.IndexOf(presetButton);
                for (int i = idx - 1; i >= 0; i--)
                {
                    if (sp.Children[i] is TextBlock tb)
                    {
                        string t = tb.Text?.Trim();
                        if (!string.IsNullOrEmpty(t))
                            return t;
                    }
                }
            }
        }
        catch
        {
        }
        return null;
    }

    private static string FormatGamePresetTagFallback(string tag)
    {
        if (string.IsNullOrEmpty(tag))
            return "";
        if (tag.Equals("allgame", StringComparison.OrdinalIgnoreCase))
            return "All Game";
        if (tag.Equals("bloxfruits", StringComparison.OrdinalIgnoreCase))
            return "Blox Fruits";
        if (tag.Equals("volleyball42", StringComparison.OrdinalIgnoreCase))
            return "Volleyball 4.2";
        var sb = new StringBuilder();
        for (int i = 0; i < tag.Length; i++)
        {
            char c = tag[i];
            if (i > 0 && char.IsUpper(c) && char.IsLower(tag[i - 1]))
                sb.Append(' ');
            sb.Append(i == 0 ? char.ToUpperInvariant(c) : c);
        }
        return sb.ToString();
    }

    private const string GamePresetDefaultContentResourceKey = "GamePresetApplyDefaultContent";
    private const string GamePresetDefaultBackgroundResourceKey = "GamePresetApplyDefaultBackground";
    private const string GamePresetFeedbackTimerResourceKey = "GamePresetApplyFeedbackTimer";

    private static void BeginGamePresetApplyButtonFeedback(Button button)
    {
        if (!button.Resources.Contains(GamePresetDefaultContentResourceKey))
        {
            button.Resources[GamePresetDefaultContentResourceKey] = button.Content;
            button.Resources[GamePresetDefaultBackgroundResourceKey] = button.Background;
        }

        if (button.Resources[GamePresetFeedbackTimerResourceKey] is DispatcherTimer existingTimer)
        {
            existingTimer.Stop();
            button.Resources.Remove(GamePresetFeedbackTimerResourceKey);
        }

        button.Background = new SolidColorBrush(Color.FromRgb(76, 230, 86));
        button.Content = "\u2714";
        button.Foreground = new SolidColorBrush(Colors.White);

        var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
        timer.Tick += (_, __) =>
        {
            timer.Stop();
            if (!button.Resources.Contains(GamePresetDefaultContentResourceKey))
                return;
            button.Background = (Brush)button.Resources[GamePresetDefaultBackgroundResourceKey];
            button.Content = button.Resources[GamePresetDefaultContentResourceKey];
            button.Foreground = new SolidColorBrush(Colors.White);
            button.Resources.Remove(GamePresetFeedbackTimerResourceKey);
        };
        button.Resources[GamePresetFeedbackTimerResourceKey] = timer;
        timer.Start();
    }

    private void GamePreset_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Button button = sender as Button;
            if (button == null) return;

            string gamePresetName = button.Tag?.ToString();
            if (string.IsNullOrEmpty(gamePresetName)) return;

            BeginGamePresetApplyButtonFeedback(button);

            this.Log($"[GamePreset] Loading preset: {gamePresetName}");
            this.UpdateStatus("Loading Game Preset...", Colors.Yellow);
            this.AddActivityEntry($"✓ Loading Game Preset: {gamePresetName}", Colors.Yellow);

            try
            {
                this.CommitPendingFastFlagGridEdits();

                this.Log($"[GamePreset] Attempting to load from embedded resources or file system...");
                throw new InvalidOperationException($"Game preset '{gamePresetName}' is unavailable.");
                string gameFFlagsContent = string.Empty;

                this._loadedFlagsPath = "";
                string presetTitle = TryGetGamePresetCardTitle(button);
                if (string.IsNullOrWhiteSpace(presetTitle))
                    presetTitle = FormatGamePresetTagFallback(gamePresetName);
                this._fflagsInfoPanelDisplayName = presetTitle ?? "";
                this.UpdateStatus("Loading...", Colors.Yellow);
                this.Log("[GamePreset] Loading FFlags from in-memory preset (no temp file)");
                this.UpdateStatus("Loaded", Colors.LimeGreen);
                this.UpdateCounts(this._flagsCount, this._cacheSlotCount, this.GetRobloxProcessCount());
                this.Log($"[GamePreset] ✓ Loaded {this._flagsCount} FFlags for {gamePresetName}");
                this.AddActivityEntry($"✓ Loaded {this._flagsCount} FFlags for {gamePresetName}", Colors.LimeGreen);


                this.PopulateEditableFlags(preserveCustomEditedFlags: false);

                string panelLabel = string.IsNullOrWhiteSpace(presetTitle) ? FormatGamePresetTagFallback(gamePresetName) : presetTitle.Trim();
                this._settingsManager.SetLastGamePresetTag(gamePresetName);
                this._settingsManager.SetLastFflagsPanelLabel(panelLabel);
                this._settingsManager.SetLastFlagJsonPath("");

                this.Dispatcher.Invoke(this.UpdateFFlagsCountDisplay);

                if (this._initializationComplete)
                    this.HasUnsavedChanges = true;

                this.Log($"[GamePreset] ✓ Finished loading {gamePresetName}");
                this.ShowToastNotification(LocalizationService.Translate("Configuration saved successfully!"));
            }
            catch (InvalidOperationException ioEx)
            {
                this.Log($"[GamePreset] ✗ Game preset not found: {ioEx.Message}");
                this.UpdateStatus("Game Preset Not Found", Colors.Red);
                this.AddActivityEntry($"❌ Game preset file not found: {gamePresetName}.json (not in embedded resources or file system)", Colors.IndianRed);
            }
        }
        catch (Exception ex)
        {
            this.UpdateStatus("Error", Colors.Red);
            this.Log($"[GamePreset] ✗ Error: {ex.Message}");
            this.AddActivityEntry($"❌ Error loading game preset: {ex.Message}", Colors.IndianRed);
        }
    }

    private void Minimize_Click(object sender, RoutedEventArgs e)
    {
        this.WindowState = WindowState.Minimized;
    }

    private void MaximizeRestore_Click(object sender, RoutedEventArgs e)
    {
        this.WindowState = this.WindowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void SupportBtn_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "https://discord.com/invite/T7ntt9dq3e",
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            this.Log("[Error] Failed to open Discord link: " + ex.Message);
        }
    }

    private void FastModeToggle_Checked(object sender, RoutedEventArgs e)
    {
        try
        {
            {
                this.ApplyFastModeValueReductionInUI();
                this.Log("[FastMode] ✓ Fast Mode ON - crash-prone numeric flags reduced to 50% to improve stability");
            }
            if (this._initializationComplete)
            {
                this.HasUnsavedChanges = true;
                this.Log("[FastMode] ✓ Fast Mode enabled - marked as unsaved");
            }
        }
        catch (Exception ex)
        {
            this.Log($"[FastMode] Error: {ex.Message}");
        }
    }

    private void FastModeToggle_Unchecked(object sender, RoutedEventArgs e)
    {
        try
        {
            {
                this.RestoreFastModeReducedValuesInUI();
                this.Log("[FastMode] ✗ Fast Mode OFF - restored original values for crash-prone flags");
            }
            if (this._initializationComplete)
            {
                this.HasUnsavedChanges = true;
                this.Log("[FastMode] ✗ Fast Mode disabled - marked as unsaved");
            }
        }
        catch (Exception ex)
        {
            this.Log($"[FastMode] Error: {ex.Message}");
        }
    }

    private void ApplyFastModeValueReductionInUI()
    {
        try
        {
                return;

            int changed = 0;

            foreach (var flag in this._allFlagsList)
            {
                if (flag == null || string.IsNullOrWhiteSpace(flag.Name) || !crashFlagNames.Contains(flag.Name))
                    continue;

                if (!this._fastModeOriginalValues.ContainsKey(flag.Name))
                    this._fastModeOriginalValues[flag.Name] = flag.Value ?? string.Empty;

                string raw = (flag.Value ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(raw))
                    continue;

                if (int.TryParse(raw, out int intVal))
                {
                    int reduced = Math.Max(1, (int)Math.Floor(intVal * 0.5));
                    if (!string.Equals(flag.Value, reduced.ToString(), StringComparison.Ordinal))
                    {
                        flag.Value = reduced.ToString();
                        changed++;
                    }
                }
                else if (double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out double doubleVal))
                {
                    double reduced = Math.Max(1.0, Math.Floor(doubleVal * 0.5));
                    string reducedText = reduced.ToString(CultureInfo.InvariantCulture);
                    if (!string.Equals(flag.Value, reducedText, StringComparison.Ordinal))
                    {
                        flag.Value = reducedText;
                        changed++;
                    }
                }
            }

            if (this.FlagsDataGrid != null)
                this.FlagsDataGrid.Items.Refresh();
            this.UpdateEditStats();
            this.UpdateCountsFromEditableList();
            this.Log($"[FastMode] Reduced {changed} crash-prone numeric flag value(s) by 50%");
        }
        catch (Exception ex)
        {
            this.Log($"[FastMode] Error applying 50% reduction: {ex.Message}");
        }
    }

    private void RestoreFastModeReducedValuesInUI()
    {
        try
        {
            if (this._fastModeOriginalValues.Count == 0)
                return;

            int restored = 0;
            foreach (var flag in this._allFlagsList)
            {
                if (flag == null || string.IsNullOrWhiteSpace(flag.Name))
                    continue;

                if (this._fastModeOriginalValues.TryGetValue(flag.Name, out string originalValue))
                {
                    if (!string.Equals(flag.Value, originalValue, StringComparison.Ordinal))
                    {
                        flag.Value = originalValue;
                        restored++;
                    }
                }
            }

            this._fastModeOriginalValues.Clear();
            if (this.FlagsDataGrid != null)
                this.FlagsDataGrid.Items.Refresh();
            this.UpdateEditStats();
            this.UpdateCountsFromEditableList();
            this.Log($"[FastMode] Restored {restored} reduced crash-prone flag value(s)");
        }
        catch (Exception ex)
        {
            this.Log($"[FastMode] Error restoring reduced values: {ex.Message}");
        }
    }

    private void CommitPendingFastFlagGridEdits()
    {
        try
        {
            if (this.FlagsDataGrid == null)
                return;
            this.FlagsDataGrid.CommitEdit(DataGridEditingUnit.Cell, true);
            this.FlagsDataGrid.CommitEdit(DataGridEditingUnit.Row, true);
        }
        catch
        {
        }
    }

    private void PersistFastFlagsFromEditorToJsonFile()
    {
        try
        {
            if (this._allFlagsList == null || this._allFlagsList.Count == 0)
            {
                this.Log("[Settings] ⚠ No flags to save (All flags list is empty)");
                return;
            }

            this.Log("[Settings] ✓ Saving FFlags from DataGrid...");

            var jsonDict = new Dictionary<string, object>();

            foreach (var flag in this._allFlagsList)
            {
                try
                {
                    object value = ParseFlagValue(flag.Value);
                    jsonDict[flag.Name] = value;
                }
                catch
                {
                    jsonDict[flag.Name] = flag.Value;
                }
            }

            string flagsFilePath = this._loadedFlagsPath;

            this.Log($"[Settings] _loadedFlagsPath = '{flagsFilePath}'");

            if (string.IsNullOrEmpty(flagsFilePath))
            {
                string appDataPath = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Masterstrap");
                System.IO.Directory.CreateDirectory(appDataPath);
                flagsFilePath = System.IO.Path.Combine(appDataPath, "SavedFlags.json");
                this.Log($"[Settings] ⚠ No loaded path found, using default: {flagsFilePath}");
            }

            string jsonContent = JsonSerializer.Serialize(jsonDict, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(flagsFilePath, jsonContent);

            this._loadedFlagsPath = flagsFilePath;
            this._settingsManager?.SetLastFlagJsonPath(flagsFilePath);

            this.Log($"[Settings] ✓ FFlags saved successfully!");
            this.Log($"[Settings] File: {System.IO.Path.GetFileName(flagsFilePath)}");
            this.Log($"[Settings] Location: {flagsFilePath}");
            this.Log($"[Settings] Total flags saved: {jsonDict.Count}");
        }
        catch (Exception flagsEx)
        {
            this.Log($"[Settings] ❌ Error saving FFlags: {flagsEx.Message}");
            this.Log($"[Settings] Stack trace: {flagsEx.StackTrace}");
        }
    }

    private void PersistFastFlagTabUiToSettingsSilent()
    {
        try
        {
            if (this._settingsManager == null)
                return;

            string msaaQuality = this.GetSelectedRenderingTag(this.AntiAliasingComboBox, "Automatic");
            string renderingMode = this.GetSelectedRenderingTag(this.RenderingModeComboBox, "Automatic");
            string textureQuality = this.GetSelectedRenderingTag(this.TextureQualityComboBox, "Automatic");
            bool preserveRenderingQuality = this.PreserveQualityToggle?.IsChecked ?? false;
            bool frmQuality = this.FRMQualityToggle?.IsChecked ?? false;
            bool meshDetailEnabled = this.MeshDetailToggle?.IsChecked ?? false;
            const int meshDetailValue = 3;

            this._settingsManager.SaveRenderingSettings(msaaQuality, renderingMode, textureQuality);
            this._settingsManager.SaveRenderingToggles(preserveRenderingQuality, frmQuality, meshDetailEnabled, meshDetailValue);
            bool disableShadows = this.DisablePlayerShadowsToggle?.IsChecked ?? false;
            bool disablePostFx = this.DisablePostProcessingToggle?.IsChecked ?? false;
            bool disableTerrainTextures = this.DisableTerrainTexturesToggle?.IsChecked ?? false;
            string lightingTech = this.GetSelectedRenderingTag(this.PreferredLightingTechnologyComboBox, "Automatic");
            this._settingsManager.SaveRenderingExtras(disableShadows, disablePostFx, disableTerrainTextures, lightingTech);
            if (frmQuality && this.FindName("FRMQualitySlider") is Slider frmSlider)
                this._settingsManager.SaveFRMQualityValue((int)frmSlider.Value);
        }
        catch
        {
        }
    }

    private void SaveBtn_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            this.CommitPendingFastFlagGridEdits();
            this.Log("[Settings] ✓ Saving configuration...");

            this.ApplyShortcutSettings();

            bool fastMode = this.FastModeToggle?.IsChecked ?? true;

            this.Log("[Settings] Saving toggle states:");
            this.Log($"  • Desktop Shortcut: {this.DesktopShortcutToggle?.IsChecked ?? false}");
            this.Log($"  • Start Menu Shortcut: {this.StartMenuShortcutToggle?.IsChecked ?? false}");
            this.Log($"  • Launch Roblox Shortcut: {this.LaunchRobloxShortcutToggle?.IsChecked ?? false}");
            this.Log($"  • Fast Mode: {fastMode}");

            SaveToggleStates();
            this.SaveModsSettingsFromUi();
            _ = this.ApplyCursorAndShiftlockFromUiAsync();

            string msaaQuality = this.GetSelectedRenderingTag(this.AntiAliasingComboBox, "Automatic");
            string renderingMode = this.GetSelectedRenderingTag(this.RenderingModeComboBox, "Automatic");
            string textureQuality = this.GetSelectedRenderingTag(this.TextureQualityComboBox, "Automatic");

            bool preserveRenderingQuality = this.PreserveQualityToggle?.IsChecked ?? false;
            bool frmQuality = this.FRMQualityToggle?.IsChecked ?? false;
            bool meshDetailEnabled = this.MeshDetailToggle?.IsChecked ?? false;
            int meshDetailValue = 3;

            this.Log("[Settings] Saving rendering settings:");
            this.Log($"  • MSAA Quality: {msaaQuality}");
            this.Log($"  • Rendering Mode: {renderingMode}");
            this.Log($"  • Texture Quality: {textureQuality}");
            this.Log($"  • Preserve Rendering Quality: {preserveRenderingQuality}");
            this.Log($"  • FRM Quality: {frmQuality}");

            this._settingsManager.SaveRenderingSettings(msaaQuality, renderingMode, textureQuality);
            this._settingsManager.SaveRenderingToggles(preserveRenderingQuality, frmQuality, meshDetailEnabled, meshDetailValue);
            bool disableShadows = this.DisablePlayerShadowsToggle?.IsChecked ?? false;
            bool disablePostFx = this.DisablePostProcessingToggle?.IsChecked ?? false;
            bool disableTerrainTextures = this.DisableTerrainTexturesToggle?.IsChecked ?? false;
            string lightingTech = this.GetSelectedRenderingTag(this.PreferredLightingTechnologyComboBox, "Automatic");
            this._settingsManager.SaveRenderingExtras(disableShadows, disablePostFx, disableTerrainTextures, lightingTech);

            this.Log("[Settings] " + LocalizationService.Translate("Configuration saved successfully"));

            this.SaveFastFlagSettingsState();
            this.ApplyFastFlagSettingsPresetsToService();

            this.PersistFastFlagsFromEditorToJsonFile();

            this.HasUnsavedChanges = false;


            ShowToastNotification(LocalizationService.Translate("Configuration saved successfully!"), "#404040");
        }
        catch (Exception ex)
        {
            this.Log($"[Error] Failed to save configuration: {ex.Message}");
            ShowToastNotification("Save failed!", "#404040");
        }
    }

    private void CloseBtn_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private int GetRobloxProcessCount()
    {
        try
        {
            int count = 0;

            Process[] playerBetaProcesses = null;
            Process[] robloxProcesses = null;

            try
            {
                playerBetaProcesses = Process.GetProcessesByName("RobloxPlayerBeta");
                foreach (var proc in playerBetaProcesses)
                {
                    try
                    {
                        if (!proc.HasExited)
                        {
                            count++;
                        }
                    }
                    catch
                    {
                    }
                    finally
                    {
                        try { proc?.Dispose(); } catch { }
                    }
                }
            }
            catch { }
            finally
            {
                if (playerBetaProcesses != null)
                {
                    foreach (var p in playerBetaProcesses)
                    {
                        try { p?.Dispose(); } catch { }
                    }
                }
            }

            try
            {
                robloxProcesses = Process.GetProcessesByName("roblox");
                foreach (var proc in robloxProcesses)
                {
                    try
                    {
                        if (!proc.HasExited)
                        {
                            count++;
                        }
                    }
                    catch
                    {
                    }
                    finally
                    {
                        try { proc?.Dispose(); } catch { }
                    }
                }
            }
            catch { }
            finally
            {
                if (robloxProcesses != null)
                {
                    foreach (var p in robloxProcesses)
                    {
                        try { p?.Dispose(); } catch { }
                    }
                }
            }

            if (count != this._lastRobloxProcessCount)
            {
                this.Log($"[ProcessCount] Roblox process count: {this._lastRobloxProcessCount} → {count}");
                this._lastRobloxProcessCount = count;
            }

            return count;
        }
        catch
        {
            return 0;
        }
    }

    private void ShowToastNotification(string message, string colorHex)
    {
        try
        {
            ItemsControl toastPanelEarly = this.FindName("ToastNotificationsPanel") as ItemsControl;
            if (toastPanelEarly != null && toastPanelEarly.Items.Count > 0)
            {
                if (toastPanelEarly.Items[0] is Border existingToast)
                {
                    toastPanelEarly.Items.Remove(existingToast);
                    existingToast.Opacity = 0;
                }
                return;
            }

            Border toastBorder = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1F1F1F")),
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorHex)),
                BorderThickness = new Thickness(1.5),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(16, 12, 16, 12),
                Width = double.NaN,
                Margin = new Thickness(0, 0, 0, 8),
                Effect = new DropShadowEffect
                {
                    ShadowDepth = 0,
                    BlurRadius = 20,
                    Color = (Color)ColorConverter.ConvertFromString(colorHex),
                    Opacity = 0.4
                },
                RenderTransform = new TranslateTransform { Y = 120 },
                Opacity = 0
            };

            TextBlock messageText = new TextBlock
            {
                Text = message,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF")),
                FontSize = 12,
                FontWeight = FontWeights.SemiBold,
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.NoWrap
            };

            toastBorder.Child = messageText;

            ItemsControl toastPanel = this.FindName("ToastNotificationsPanel") as ItemsControl;
            if (toastPanel != null)
            {
                toastPanel.Items.Add(toastBorder);

                var slideUpAnim = FindResource("ToastSlideUpAnimation") as Storyboard;
                if (slideUpAnim != null)
                {
                    slideUpAnim.Begin(toastBorder, true);
                }

                DispatcherTimer hideTimer = new DispatcherTimer();
                hideTimer.Interval = TimeSpan.FromSeconds(3);
                hideTimer.Tick += (s, e) =>
                {
                    hideTimer.Stop();
                    if (!toastPanel.Items.Contains(toastBorder))
                        return;
                    var slideDownAnim = FindResource("ToastSlideDownAnimation") as Storyboard;
                    if (slideDownAnim != null)
                    {
                        slideDownAnim.Begin(toastBorder, true);
                    }

                    DispatcherTimer removeTimer = new DispatcherTimer();
                    removeTimer.Interval = TimeSpan.FromMilliseconds(350);
                    removeTimer.Tick += (s2, e2) =>
                    {
                        removeTimer.Stop();
                        ItemsControl panel = this.FindName("ToastNotificationsPanel") as ItemsControl;
                        if (panel != null && panel.Items.Contains(toastBorder))
                        {
                            panel.Items.Remove(toastBorder);
                        }
                    };
                    removeTimer.Start();
                };
                hideTimer.Start();
            }
        }
        catch (Exception ex)
        {
            this.Log($"[Toast] Error showing notification: {ex.Message}");
        }
    }

    private void UpdateFooterInfo()
    {
        try
        {
            this.Dispatcher.Invoke(() =>
            {
                int processCount = this.GetRobloxProcessCount();
                string processStatus = processCount > 0 ? $"Running ({processCount})" : "Not Running";
            });
        }
        catch (Exception ex)
        {
            this.Log($"[Footer] Error updating footer: {ex.Message}");
        }
    }

    private void RefreshVersionColorsAfterThemeChange()
    {
        try
        {
            DateTime? robloxLastUpdate = ExtractLastUpdateTime(this._robloxVersion);
            DateTime? softwareLastUpdate = ExtractLastUpdateTime(this._softwareVersion);
            string robloxVersionName = ExtractVersionName(this._robloxVersion);
            string softwareVersionName = ExtractVersionName(this._softwareVersion);
            this.UpdateVersionColors(robloxVersionName, softwareVersionName, robloxLastUpdate, softwareLastUpdate);
        }
        catch { }
    }

    private void UpdateVersionColors(string robloxVersionName, string softwareVersionName, DateTime? robloxLastUpdate, DateTime? softwareLastUpdate)
    {
        try
        {
            bool lightUi = string.Equals(this._currentUiTheme, "White", StringComparison.OrdinalIgnoreCase);
            Brush greenBrush = this.TryFindResource("InfoSysGreenBrush") as Brush
                ?? new SolidColorBrush(Color.FromRgb(0xA5, 0xFF, 0x7F));
            Brush mutedBrush = this.TryFindResource("InfoSysLabelBrush") as Brush
                ?? new SolidColorBrush(Color.FromRgb(0xB5, 0xB5, 0xB5));
            var redColor = lightUi
                ? new SolidColorBrush(Color.FromRgb(0xC6, 0x28, 0x28))
                : new SolidColorBrush(Color.FromRgb(255, 120, 130));

            this.Log($"[Version] UpdateVersionColors called - Roblox Name: {robloxVersionName}, Software Name: {softwareVersionName}");

            if (!this._hasLocalRobloxInstall)
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.InfoRobloxVersion.Foreground = mutedBrush;
                    this.InfoSoftwareVersion.Foreground = !string.IsNullOrEmpty(softwareVersionName) ? greenBrush : mutedBrush;
                    this.Log("[Version] Colors: No local Roblox — Roblox row gray, Software from cache if applicable");
                });
                return;
            }

            if (!string.IsNullOrEmpty(robloxVersionName) && !string.IsNullOrEmpty(softwareVersionName) &&
                string.Equals(robloxVersionName, softwareVersionName, StringComparison.OrdinalIgnoreCase))
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.Log("[Version] Colors: Case 1 - Both green (identical version names)");
                    this.InfoRobloxVersion.Foreground = greenBrush;
                    this.InfoSoftwareVersion.Foreground = greenBrush;
                });
                return;
            }

            if (!robloxLastUpdate.HasValue || !softwareLastUpdate.HasValue)
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.InfoRobloxVersion.Foreground = mutedBrush;
                    this.InfoSoftwareVersion.Foreground = mutedBrush;
                    this.Log("[Version] Colors set to gray (unknown)");
                });
                return;
            }

            DateTime robloxDate = robloxLastUpdate.Value.Date;
            DateTime softwareDate = softwareLastUpdate.Value.Date;
            int comparison = DateTime.Compare(robloxDate, softwareDate);
            this.Log($"[Version] Date comparison result: {comparison} (negative=software newer, 0=same, positive=roblox newer)");

            this.Dispatcher.Invoke(() =>
            {
                if (comparison == 0)
                {
                    this.Log("[Version] Colors: Same dates - Both green (compatible)");
                    this.InfoRobloxVersion.Foreground = greenBrush;
                    this.InfoSoftwareVersion.Foreground = greenBrush;
                }
                else if (comparison > 0)
                {
                    this.Log("[Version] Colors: Case 2 - Roblox green, Software red (outdated)");
                    this.InfoRobloxVersion.Foreground = greenBrush;
                    this.InfoSoftwareVersion.Foreground = redColor;
                    this.Log($"[Version] InfoSoftwareVersion Foreground set to RED");
                }
                else
                {
                    this.Log("[Version] Colors: Case 3 - Both green (compatible)");
                    this.InfoRobloxVersion.Foreground = greenBrush;
                    this.InfoSoftwareVersion.Foreground = greenBrush;
                }
            });
        }
        catch (Exception ex)
        {
            this.Log($"[Version] ✗ Error updating colors: {ex.Message}");
            this.Log($"[Version] Stack trace: {ex.StackTrace}");
        }
    }

    private string? ExtractVersionName(string? version, bool logExtraction = true)
    {
        if (string.IsNullOrWhiteSpace(version))
            return null;

        int versionStartIdx = version.IndexOf("version-");
        if (versionStartIdx < 0)
            return null;

        int spaceIdx = version.IndexOf(" ", versionStartIdx);
        int parenIdx = version.IndexOf("(", versionStartIdx);

        int versionEndIdx = -1;
        if (spaceIdx >= 0 && parenIdx >= 0)
            versionEndIdx = Math.Min(spaceIdx, parenIdx);
        else if (spaceIdx >= 0)
            versionEndIdx = spaceIdx;
        else if (parenIdx >= 0)
            versionEndIdx = parenIdx;
        else
            versionEndIdx = version.Length;

        if (versionEndIdx <= versionStartIdx)
            return null;

        string versionName = version.Substring(versionStartIdx, versionEndIdx - versionStartIdx).Trim();
        if (logExtraction)
            this.Log($"[Version] Extracted version name: '{versionName}'");
        return versionName;
    }

    private DateTime? ExtractLastUpdateTime(string version)
    {
        if (string.IsNullOrWhiteSpace(version))
            return null;

        int lastUpdateIdx = version.IndexOf("(last update:");
        if (lastUpdateIdx < 0)
            return null;

        int timeStartIdx = version.IndexOf(":", lastUpdateIdx) + 1;
        int timeEndIdx = version.IndexOf(")", timeStartIdx);

        if (timeStartIdx <= 0 || timeEndIdx <= timeStartIdx)
            return null;

        string timeString = version.Substring(timeStartIdx, timeEndIdx - timeStartIdx).Trim();
        this.Log($"[Version] Extracted time string: '{timeString}'");

        if (DateTime.TryParseExact(timeString, "d/M/yyyy HH:mm",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None, out DateTime result))
        {
            return result;
        }

        string[] formats = { "dd/MM/yyyy HH:mm", "d/M/yyyy H:mm", "dd/MM/yyyy HH:mm:ss", "d/M/yyyy H:mm:ss" };
        foreach (var format in formats)
        {
            if (DateTime.TryParseExact(timeString, format,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out result))
            {
                return result;
            }
        }

        this.Log($"[Version] Could not parse time: '{timeString}'");
        return null;
    }

    public void Log(string message)
    {
        if (string.IsNullOrEmpty(message))
            return;

        bool sensitive =
            message.Contains("[License]", StringComparison.OrdinalIgnoreCase) ||
            message.Contains("[Usage]", StringComparison.OrdinalIgnoreCase);
        if (!sensitive)
        {
            try
            {
                string logPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "Masterstrap_runtime.log");
                System.IO.File.AppendAllText(logPath, $"[{DateTime.Now:G}] {message}\n");
            }
            catch { }
        }

        if (this._suppressSaveAndLaunchNoiseLogs &&
            (message.Contains("[Roblox]", StringComparison.OrdinalIgnoreCase) ||
             message.Contains("[Install]", StringComparison.OrdinalIgnoreCase) ||
             message.Contains("[ProcessCount]", StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        if (sensitive ||
            message.Contains("[Update]", StringComparison.OrdinalIgnoreCase) ||
            message.Contains("[Version]", StringComparison.OrdinalIgnoreCase) ||
             message.Contains("[System]", StringComparison.OrdinalIgnoreCase) ||
             message.Contains("[Settings]", StringComparison.OrdinalIgnoreCase) ||
             message.Contains("[AutoLoad]", StringComparison.OrdinalIgnoreCase) ||
             message.Contains("[FFlagsSearch]", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        this.Dispatcher.BeginInvoke((Action)(() =>
        {
            try
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {message}");
                this.AddActivityEntry(message);
            }
            catch { }
        }));
    }

    public void LogBatch(string message)
    {
        this.AddActivityEntry(message, Colors.LightSeaGreen);
    }

    private void ApplyCachedSoftwareVersionFromDiskToUi(bool quietLogs = false)
    {
    }

    private void LoadVersionInformation()
    {
        try
        {
            this.Log($"[Info] === Loading Version Information ===");

            this._softwareVersion = "not installed";
            this.Dispatcher.Invoke(() =>
            {
                try
                {
                    this.InfoSoftwareVersion.Text = "Not installed";
                    this.InfoSoftwareUpdate.Text = "No cache data found";
                }
                catch { }
            });

            this.SearchRobloxVersion();

            try
            {
                this.ApplyCachedSoftwareVersionFromDiskToUi(quietLogs: false);
            }
            catch (Exception ex)
            {
                this.Log($"[Cache] Cache load exception (software not installed): {ex.Message}");
            }

            this.Dispatcher.Invoke(() =>
            {
                try
                {
                    this.Log($"[Info] Updating colors after loading versions...");
                    DateTime? robloxLastUpdate = ExtractLastUpdateTime(this._robloxVersion);
                    DateTime? softwareLastUpdate = ExtractLastUpdateTime(this._softwareVersion);
                    string robloxVersionName = ExtractVersionName(this._robloxVersion);
                    string softwareVersionName = ExtractVersionName(this._softwareVersion);
                    this.Log($"[Info] Roblox version to parse: {this._robloxVersion}");
                    this.Log($"[Info] Software version to parse: {this._softwareVersion}");
                    this.UpdateVersionColors(robloxVersionName, softwareVersionName, robloxLastUpdate, softwareLastUpdate);
                }
                catch (Exception ex)
                {
                    this.Log($"[Info] Error updating colors after version load: {ex.Message}");
                }
            });
        }
        catch (Exception ex)
        {
            this.Log($"[Info] ✗ Critical error in LoadVersionInformation: {ex.Message}");
        }
    }

    private void SearchRobloxVersion()
    {
        try
        {
            this.Log("[Info] Searching for installed Roblox version in app directory...");

            string foundVersion = "version-unknown";
            DateTime latestModifiedTime = DateTime.MinValue;
            string foundPath = "";

            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string appVersionsPath = System.IO.Path.Combine(appDirectory, "versions");

            this.Log($"[Info] Searching in: {appVersionsPath}");

            if (System.IO.Directory.Exists(appVersionsPath))
            {
                try
                {
                    DirectoryInfo versionsDir = new DirectoryInfo(appVersionsPath);
                    DirectoryInfo[] versionFolders = versionsDir.GetDirectories("version-*");

                    this.Log($"[Info] Found {versionFolders.Length} Roblox version folder(s)");

                    foreach (DirectoryInfo versionFolder in versionFolders)
                    {
                        try
                        {
                            string exePath = System.IO.Path.Combine(versionFolder.FullName, "RobloxPlayerBeta.exe");

                            if (System.IO.File.Exists(exePath))
                            {
                                string versionFromFolder = versionFolder.Name;
                                DateTime versionFolderTime = versionFolder.LastWriteTime;

                                if (versionFolderTime > latestModifiedTime)
                                {
                                    latestModifiedTime = versionFolderTime;
                                    foundVersion = versionFromFolder;
                                    foundPath = exePath;
                                }

                                this.Log($"[Info] Found: {versionFromFolder} (folder modified: {versionFolderTime:dd/MM/yyyy HH:mm})");
                            }
                        }
                        catch (Exception ex)
                        {
                            this.Log($"[Info] Error processing version folder {versionFolder.Name}: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.Log($"[Info] Error searching app versions folder: {ex.Message}");
                }
            }
            else
            {
                this.Log($"[Info] ⚠ Versions folder not found at: {appVersionsPath}");
            }

            if (foundVersion != "version-unknown" && latestModifiedTime != DateTime.MinValue)
            {
                this._hasLocalRobloxInstall = true;
                string robloxUpdateTime = latestModifiedTime.ToString("dd/MM/yyyy HH:mm");
                this._robloxVersion = $"{foundVersion} (last update: {robloxUpdateTime})";
                this._robloxExecutablePath = foundPath;

                this.Dispatcher.Invoke(() =>
                {
                    this.InfoRobloxVersion.Text = foundVersion;
                    this.InfoRobloxUpdate.Text = $"Last update: {robloxUpdateTime}";
                });

                this.Log($"[Info] ✓ Latest Roblox Version: {foundVersion}");
                this.Log($"[Info] ✓ Update time: {robloxUpdateTime}");
                this.Log($"[Info] ✓ Location: {foundPath}");
            }
            else
            {
                this._hasLocalRobloxInstall = false;
                this.Log("[Info] ⚠ No Roblox version found in app versions folder");

                this.Dispatcher.Invoke(() =>
                {
                    this.InfoRobloxVersion.Text = "Not installed";
                    this.InfoRobloxUpdate.Text = "No installation found";
                });

                this._robloxVersion = "version-unknown";
                this._robloxExecutablePath = "";
            }
        }
        catch (Exception ex)
        {
            this.Log($"[Info] Error searching for Roblox version: {ex.Message}");
            this._hasLocalRobloxInstall = false;
            this._robloxVersion = "version-unknown";
        }
    }

    private void UpdateStatus(string status, Color color)
    {
        this.Dispatcher.Invoke((Action)(() =>
        {
            string displayText = LocalizationService.Translate(status);
            this.StatusText.Text = displayText;
            this.StatusDot.Fill = (Brush)new SolidColorBrush(color);
        }));
    }

    private void UpdateFileInfo(string flagsPath, string cachePath)
    {
        this.Dispatcher.Invoke((Action)(() =>
        {
        }));
    }

    private void UpdateCounts(int flagsCount, int cacheSlotCount, int robloxCount)
    {
        this.Dispatcher.Invoke((Action)(() =>
        {
            this._flagsCount = flagsCount;
            this._cacheSlotCount = cacheSlotCount;

            this.UpdateFooterInfo();
        }));
    }

    private void UpdateCountsFromEditableList()
    {
        this.Dispatcher.Invoke((Action)(() =>
        {
            this._flagsCount = this._allFlagsList.Count;

            this.UpdateFooterInfo();

            this.UpdateFFlagsCountDisplay();
        }));
    }

    private void ResetToNoFileLoadedState()
    {
        try
        {
            this._loadedFlagsPath = "";
            this._fflagsInfoPanelDisplayName = "";
            this._settingsManager?.SetLastFlagJsonPath("");
            this._settingsManager?.SetLastGamePresetTag("");
            this._settingsManager?.SetLastFflagsPanelLabel("");
            this.UpdateCountsFromEditableList();
            this.Log("[Edit] Count = 0: reset to “no file loaded” (will not auto-load on next launch).");
        }
        catch (Exception ex)
        {
            this.Log($"[Edit] ResetToNoFileLoadedState: {ex.Message}");
        }
    }

    private void UpdateFFlagsCountDisplay()
    {
        try
        {
            int totalCount = this._allFlagsList.Count;
            string fileName = "None loaded";
            string savedPanelLabel = this._settingsManager?.GetLastFflagsPanelLabel() ?? "";

            if (totalCount == 0)
            {
                fileName = "None loaded";
            }
            else if (!string.IsNullOrWhiteSpace(savedPanelLabel))
            {
                fileName = savedPanelLabel;
            }
            else if (!string.IsNullOrEmpty(this._loadedFlagsPath) && System.IO.File.Exists(this._loadedFlagsPath))
            {
                fileName = System.IO.Path.GetFileName(this._loadedFlagsPath);
            }
            else if (!string.IsNullOrWhiteSpace(this._fflagsInfoPanelDisplayName))
            {
                fileName = this._fflagsInfoPanelDisplayName;
            }


            if (this.InfoFFlagsName != null)
            {
                this.InfoFFlagsName.Text = fileName;
            }

            if (this.InfoFFlagsCount != null)
            {
                this.InfoFFlagsCount.Text = $"Count: {totalCount}";
            }

        }
        catch { }
    }

    private void SetupStatusAnimation()
    {
        this._statusTimer.Interval = TimeSpan.FromSeconds(0.5);
        this._statusTimer.Tick += (EventHandler)((s, e) =>
        {
            this._statusDotState = !this._statusDotState;
            this.StatusDot.Opacity = this._statusDotState ? 1.0 : 0.3;
        });
        this._statusTimer.Start();
    }

    private void DisableButtons()
    {
        if (this.LoadFlagsBtn != null) this.LoadFlagsBtn.IsEnabled = false;
        if (this.LoadCacheBtn != null) this.LoadCacheBtn.IsEnabled = false;
        if (this.ApplyFlagsBtn != null) this.ApplyFlagsBtn.IsEnabled = false;
        if (this.RestoreFlagsBtn != null) this.RestoreFlagsBtn.IsEnabled = false;
    }

    private void EnableAllButtons()
    {
        if (this.LoadFlagsBtn != null) this.LoadFlagsBtn.IsEnabled = true;
        if (this.LoadCacheBtn != null) this.LoadCacheBtn.IsEnabled = true;
        if (this.ApplyFlagsBtn != null) this.ApplyFlagsBtn.IsEnabled = true;
        if (this.RestoreFlagsBtn != null) this.RestoreFlagsBtn.IsEnabled = true;
    }

    private void DisableUserInteractionControls()
    {
        try
        {
            this.Dispatcher.Invoke(() =>
            {
                this.DisableButtons();

                if (this.MainTabControl != null)
                    this.MainTabControl.IsEnabled = false;

                var allControls = FindAllControls(this);
                foreach (var control in allControls)
                {
                    if (control is System.Windows.Controls.Button ||
                        control is System.Windows.Controls.TextBox ||
                        control is System.Windows.Controls.ComboBox ||
                        control is System.Windows.Controls.DataGrid ||
                        control is System.Windows.Controls.CheckBox)
                    {
                        control.IsEnabled = false;
                    }
                }

                this.Log("[System] ✓ User interaction controls disabled (but auto-apply continues)");
            });
        }
        catch (Exception ex)
        {
            this.Log($"[System] Error disabling controls: {ex.Message}");
        }
    }

    private void EnableUserInteractionControls()
    {
        try
        {
            this.Dispatcher.Invoke(() =>
            {
                this.EnableAllButtons();

                if (this.MainTabControl != null)
                    this.MainTabControl.IsEnabled = true;

                var allControls = FindAllControls(this);
                foreach (var control in allControls)
                {
                    if (control is System.Windows.Controls.Button ||
                        control is System.Windows.Controls.TextBox ||
                        control is System.Windows.Controls.ComboBox ||
                        control is System.Windows.Controls.DataGrid ||
                        control is System.Windows.Controls.CheckBox)
                    {
                        control.IsEnabled = true;
                    }
                }

                this.Log("[System] ✓ User interaction controls enabled");
            });
        }
        catch (Exception ex)
        {
            this.Log($"[System] Error enabling controls: {ex.Message}");
        }
    }

    private List<System.Windows.Controls.Control> FindAllControls(DependencyObject parent)
    {
        var controls = new List<System.Windows.Controls.Control>();

        if (parent == null) return controls;

        if (parent is System.Windows.Controls.Control control)
            controls.Add(control);

        int childrenCount = System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < childrenCount; i++)
        {
            var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);
            controls.AddRange(FindAllControls(child));
        }

        return controls;
    }

    private void PopulateEditableFlags(bool preserveCustomEditedFlags = true)
    {
        try
        {
            this.Dispatcher.Invoke(() =>
            {
                Dictionary<string, JsonElement>? loadedFlags = null;
                if (!string.IsNullOrEmpty(this._loadedFlagsPath) && File.Exists(this._loadedFlagsPath))
                {
                    try
                    {
                        string json = File.ReadAllText(this._loadedFlagsPath);
                        loadedFlags = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
                    }
                    catch (Exception jsonEx)
                    {
                        this.Log($"[Edit] Failed to read flags JSON: {jsonEx.Message}");
                    }
                }

                if (loadedFlags == null || loadedFlags.Count == 0)
                {
                    this.Log("[Edit] No flags loaded");
                    return;
                }

                var customFlags = preserveCustomEditedFlags
                    ? this._allFlagsList.Where(f => f.IsEdited).ToList()
                    : new List<FlagItem>();

                this._editableFlagsList.Clear();
                this._allFlagsList.Clear();

                foreach (var flag in loadedFlags)
                {
                    string value = flag.Value.ValueKind switch
                    {
                        JsonValueKind.Number => flag.Value.GetRawText(),
                        JsonValueKind.String => flag.Value.GetString(),
                        JsonValueKind.True => "true",
                        JsonValueKind.False => "false",
                        JsonValueKind.Null => "null",
                        _ => flag.Value.GetRawText()
                    };

                    var flagItem = new FlagItem
                    {
                        Name = flag.Key,
                        Value = value,
                        IsEdited = false,
                        LastModified = DateTime.UtcNow
                    };
                    this._allFlagsList.Add(flagItem);
                }

                foreach (var customFlag in customFlags)
                {
                    var existingFlag = this._allFlagsList.FirstOrDefault(f => f.Name.Equals(customFlag.Name, StringComparison.OrdinalIgnoreCase));
                    if (existingFlag == null)
                    {
                        this._allFlagsList.Add(customFlag);
                        this.Log($"[Edit] ✓ Restored custom flag: {customFlag.Name} = {customFlag.Value}");
                    }
                }

                this.ApplyEditorFlagsFilter();
                this.UpdateCountsFromEditableList();
                string editSourceLabel = string.IsNullOrEmpty(this._loadedFlagsPath)
                    ? "(in-memory preset)"
                    : System.IO.Path.GetFileName(this._loadedFlagsPath);
                this.Log($"[Edit] ✓ Loaded {this._allFlagsList.Count} flags for editing from: {editSourceLabel}");
            });
        }
        catch (Exception ex)
        {
            this.Log($"[Edit] Error populating flags: {ex.Message}");
        }
    }

    private void ClearSearchBtn_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            this.SearchFlagsBox.Clear();
            this.ApplyEditorFlagsFilter();
            this.Log("[Edit] ✓ Search cleared");
        }
        catch (Exception ex)
        {
            this.Log($"[Edit] Error clearing search: {ex.Message}");
        }
    }

    private void SearchFlagsBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            try
            {
                var placeholder = this.FindName("SearchPlaceholder") as TextBlock;
                if (placeholder != null)
                    placeholder.Visibility = string.IsNullOrEmpty(this.SearchFlagsBox.Text) ? Visibility.Visible : Visibility.Collapsed;
            }
            catch { }

            this.ApplyEditorFlagsFilter();
        }
        catch (Exception ex)
        {
            this.Log($"[Edit] Search error: {ex.Message}");
        }
    }

    private void FilterCategory_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is Button button && button.Tag is string categoryTag)
            {
                this._editorCategoryFilter = categoryTag;
                this.ApplyEditorFlagsFilter();

                if (categoryTag == "All")
                    this.Log("[Edit] ✓ Showing all FFlags");
                else
                    this.Log($"[Edit] ✓ Filter: {categoryTag} ({this._editableFlagsList.Count} flags)");
            }
        }
        catch (Exception ex)
        {
            this.Log($"[Edit] Filter error: {ex.Message}");
        }
    }

    private void ApplyEditorFlagsFilter()
    {
        string searchText = (this.SearchFlagsBox?.Text ?? string.Empty).Trim();
        if (string.Equals(searchText, "search fflags by name or value...", StringComparison.OrdinalIgnoreCase))
            searchText = string.Empty;

        this._editableFlagsList.Clear();

        IEnumerable<FlagItem> query = this._allFlagsList;

        if (!string.Equals(this._editorCategoryFilter, "All", StringComparison.OrdinalIgnoreCase) &&
            Enum.TryParse(this._editorCategoryFilter, true, out FlagCategoryType targetCategory))
        {
            query = query.Where(flag => flag.Category == targetCategory);
        }

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            query = query.Where(flag =>
                flag.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                (flag.Value?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        foreach (var flag in query)
            this._editableFlagsList.Add(flag);

        this.UpdateEditStats();
    }

    private void AddNewFlagBtn_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var addFlagDialog = new Masterstrap.Views.AddFlagDialog();
            addFlagDialog.Owner = this;

            if (addFlagDialog.ShowDialog() == true)
            {
                if (addFlagDialog.LoadedFlags != null && addFlagDialog.LoadedFlags.Count > 0)
                {
                    foreach (var loadedFlag in addFlagDialog.LoadedFlags)
                    {
                        string name = loadedFlag.Key;
                        string value = loadedFlag.Value;

                        var existingFlag = this._allFlagsList.FirstOrDefault(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

                        if (existingFlag != null)
                        {
                            existingFlag.Value = value;
                            existingFlag.IsEdited = true;
                            existingFlag.LastModified = DateTime.UtcNow;
                        }
                        else
                        {
                            var newFlag = new FlagItem
                            {
                                Name = name,
                                Value = value,
                                IsEdited = true,
                                LastModified = DateTime.UtcNow
                            };

                            this._allFlagsList.Add(newFlag);
                            if (!this._editableFlagsList.Contains(newFlag))
                            {
                                this._editableFlagsList.Add(newFlag);
                            }
                        }
                    }

                    this.UpdateEditStats();
                    this.AutoSaveFlags();
                    this.UpdateCountsFromEditableList();
                    this.AutoExportFlagsIfEnabled();

                    if (!string.IsNullOrEmpty(addFlagDialog.LoadedJsonPath))
                    {
                        this._loadedFlagsPath = addFlagDialog.LoadedJsonPath;
                        this._fflagsInfoPanelDisplayName = "";
                        this._settingsManager?.SetLastGamePresetTag("");
                        this._settingsManager?.SetLastFflagsPanelLabel("");
                        this._settingsManager?.SetLastFlagJsonPath(this._loadedFlagsPath);
                        this.UpdateFFlagsCountDisplay();
                        this.Log($"[Edit] ✓ FFlags file path saved for display and auto-load: {System.IO.Path.GetFileName(this._loadedFlagsPath)}");
                    }

                    if (this._initializationComplete)
                        this.HasUnsavedChanges = true;
                    this.Log($"[Edit] ✓ Added {addFlagDialog.LoadedFlags.Count} flags from JSON");
                 }
                else
                {
                    string name = addFlagDialog.FlagName;
                    string value = addFlagDialog.FlagValue;

                    var existingFlag = this._allFlagsList.FirstOrDefault(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

                    if (existingFlag != null)
                    {
                        DateTime newTime = DateTime.UtcNow;

                        if (newTime > existingFlag.LastModified)
                        {
                            existingFlag.Value = value;
                            existingFlag.IsEdited = true;
                            existingFlag.LastModified = newTime;

                            this.UpdateEditStats();
                            this.AutoSaveFlags();
                            this.UpdateCountsFromEditableList();
                            this.AutoExportFlagsIfEnabled();
                            if (this._initializationComplete)
                                this.HasUnsavedChanges = true;
                            this.Log($"[Edit] ✓ Flag merged and updated: {name} = {value}");
                        }
                        else
                        {
                            this.Log($"[Edit] ℹ Flag '{name}' already exists with newer value - no change made");
                        }
                    }
                    else
                    {
                        var newFlag = new FlagItem
                        {
                            Name = name,
                            Value = value,
                            IsEdited = true,
                            LastModified = DateTime.UtcNow
                        };

                        this._allFlagsList.Add(newFlag);
                        this._editableFlagsList.Add(newFlag);

                        this.UpdateEditStats();
                        this.AutoSaveFlags();
                        this.UpdateCountsFromEditableList();
                        this.AutoExportFlagsIfEnabled();
                        if (this._initializationComplete)
                            this.HasUnsavedChanges = true;
                        this.Log($"[Edit] ✓ New flag added: {name} = {value}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            this.Log($"[Edit] Error adding flag: {ex.Message}");
        }
    }

    private void BuildFlagBtn_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            string allFlagPath = this.FindAllFlagJsonPath();
            if (string.IsNullOrWhiteSpace(allFlagPath) || !File.Exists(allFlagPath))
            {
                MessageBox.Show("Cannot find FFlags/allflag.json.", "Build Flag", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string rawJson = File.ReadAllText(allFlagPath);
            var allFlagDict = JsonSerializer.Deserialize<Dictionary<string, string>>(rawJson);

            if (allFlagDict == null || allFlagDict.Count == 0)
            {
                MessageBox.Show("allflag.json is empty or invalid.", "Build Flag", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var buildWindow = new BuildFlagWindow(allFlagDict);
            buildWindow.Owner = this;

            if (buildWindow.ShowDialog() != true)
                return;

            int addedCount = 0;
            int updatedCount = 0;

            foreach (var selected in buildWindow.SelectedFlags)
            {
                var existing = this._allFlagsList.FirstOrDefault(f => f.Name.Equals(selected.Name, StringComparison.OrdinalIgnoreCase));
                if (existing != null)
                {
                    if (!string.Equals(existing.Value, selected.Value, StringComparison.Ordinal))
                    {
                        existing.Value = selected.Value;
                        existing.IsEdited = true;
                        existing.LastModified = DateTime.UtcNow;
                        updatedCount++;
                    }
                    continue;
                }

                var newFlag = new FlagItem
                {
                    Name = selected.Name,
                    Value = selected.Value,
                    IsEdited = true,
                    LastModified = DateTime.UtcNow
                };

                this._allFlagsList.Add(newFlag);
                this._editableFlagsList.Add(newFlag);
                addedCount++;
            }

            this.UpdateEditStats();
            this.AutoSaveFlags();
            this.UpdateCountsFromEditableList();
            this.AutoExportFlagsIfEnabled();
            if (this._initializationComplete)
                this.HasUnsavedChanges = true;

            this.Log($"[BuildFlag] ✓ Added {addedCount}, updated {updatedCount} from allflag.json");
        }
        catch (Exception ex)
        {
            this.Log($"[BuildFlag] Error: {ex.Message}");
        }
    }

    private void ShowPresetFlagsBtn_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (!this._isPresetFlagsExpanded)
            {
                this._presetFlagsPreviousView = this._editableFlagsList.ToList();
                var combined = new List<FlagItem>(this._editableFlagsList);
                var existingNames = new HashSet<string>(combined.Select(f => f.Name), StringComparer.OrdinalIgnoreCase);

                foreach (var hiddenGraphicsFlag in this._allFlagsList.Where(f => f.Category == FlagCategoryType.Graphics))
                {
                    if (existingNames.Add(hiddenGraphicsFlag.Name))
                    {
                        combined.Add(hiddenGraphicsFlag);
                    }
                }

                var presetFlags = this.CollectRenderingGraphicsPresetFlagsFromUi();
                foreach (var kv in presetFlags)
                {
                    if (!existingNames.Add(kv.Key))
                        continue;

                    combined.Add(new FlagItem
                    {
                        Name = kv.Key,
                        Value = kv.Value,
                        IsEdited = false,
                        LastModified = DateTime.UtcNow
                    });
                }

                this._editableFlagsList.Clear();
                foreach (var flag in combined)
                    this._editableFlagsList.Add(flag);

                this._isPresetFlagsExpanded = true;
                if (this.ShowPresetFlagsBtn != null)
                    this.ShowPresetFlagsBtn.Content = LocalizationService.Translate("Hide Preset Flags");

                this.UpdateEditStats();
                this.Log($"[Preset] ✓ Showing preset/hidden Rendering and Graphics flags ({combined.Count} rows).");
            }
            else
            {
                this._editableFlagsList.Clear();
                foreach (var flag in this._presetFlagsPreviousView)
                    this._editableFlagsList.Add(flag);

                this._isPresetFlagsExpanded = false;
                if (this.ShowPresetFlagsBtn != null)
                    this.ShowPresetFlagsBtn.Content = LocalizationService.Translate("Show Preset Flags");

                this.UpdateEditStats();
                this.Log("[Preset] ✓ Restored previous flags view.");
            }
        }
        catch (Exception ex)
        {
            this.Log($"[Preset] ✗ Error showing preset flags: {ex.Message}");
        }
    }

    private Dictionary<string, string> CollectRenderingGraphicsPresetFlagsFromUi()
    {
        var renderingFlags = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        if (this.AntiAliasingComboBox?.SelectedItem is ComboBoxItem msaaItem)
        {
            string msaaText = ((msaaItem.Tag as string) ?? msaaItem.Content?.ToString() ?? "Automatic").Trim().ToLowerInvariant();
            if (msaaText == "off") renderingFlags["FIntDebugForceMSAASamples"] = "1";
            else if (msaaText == "2x") renderingFlags["FIntDebugForceMSAASamples"] = "2";
            else if (msaaText == "4x") renderingFlags["FIntDebugForceMSAASamples"] = "4";
            else if (msaaText == "8x") renderingFlags["FIntDebugForceMSAASamples"] = "8";
        }

        if (this.PreserveQualityToggle?.IsChecked == true)
        {
            renderingFlags["DFFlagDisableDPIScale"] = "True";
        }

        if (this.FRMQualityToggle?.IsChecked == true)
        {
            int frmQuality = 21;
            var frmSlider = this.FindName("FRMQualitySlider") as Slider;
            if (frmSlider != null)
                frmQuality = Math.Clamp((int)frmSlider.Value, 1, 21);
            renderingFlags["DFIntDebugFRMQualityLevelOverride"] = frmQuality.ToString();
        }

        if (this.MeshDetailToggle?.IsChecked == true)
        {
            renderingFlags["DFIntCSGLevelOfDetailSwitchingDistanceStatic"] = "3";
            renderingFlags["DFIntCSGLevelOfDetailSwitchingDistance"] = "3";
            renderingFlags["DFIntCSGLevelOfDetailSwitchingDistanceL12"] = "2";
            renderingFlags["DFIntCSGLevelOfDetailSwitchingDistanceL23"] = "1";
            renderingFlags["DFIntCSGLevelOfDetailSwitchingDistanceL34"] = "0";
        }

        if (this.RenderingModeComboBox?.SelectedItem is ComboBoxItem renderItem)
        {
            string renderMode = ((renderItem.Tag as string) ?? renderItem.Content?.ToString() ?? "Automatic").Trim().ToLowerInvariant();
            if (renderMode == "vulkan")
            {
                renderingFlags["FFlagDebugGraphicsPreferVulkan"] = "True";
                renderingFlags["FFlagDebugGraphicsDisableDirect3D11"] = "True";
            }
            else if (renderMode == "opengl")
            {
                renderingFlags["FFlagDebugGraphicsPreferOpenGL"] = "True";
                renderingFlags["FFlagDebugGraphicsDisableDirect3D11"] = "True";
            }
            else if (renderMode == "direct3d11" || renderMode == "direct3d 11" || renderMode == "directx 11")
            {
                renderingFlags["FFlagDebugGraphicsPreferD3D11"] = "True";
            }
        }

        if (this.TextureQualityComboBox?.SelectedItem is ComboBoxItem textureItem)
        {
            string texture = ((textureItem.Tag as string) ?? textureItem.Content?.ToString() ?? "Automatic").Trim().ToLowerInvariant();
            if (texture == "low")
            {
                renderingFlags["DFFlagTextureQualityOverrideEnabled"] = "True";
                renderingFlags["DFIntTextureQualityOverride"] = "0";
            }
            else if (texture == "medium")
            {
                renderingFlags["DFFlagTextureQualityOverrideEnabled"] = "True";
                renderingFlags["DFIntTextureQualityOverride"] = "1";
            }
            else if (texture == "high")
            {
                renderingFlags["DFFlagTextureQualityOverrideEnabled"] = "True";
                renderingFlags["DFIntTextureQualityOverride"] = "2";
            }
        }

        return renderingFlags;
    }

    private string FindAllFlagJsonPath()
    {
        var candidates = new List<string>();

        string baseDir = AppDomain.CurrentDomain.BaseDirectory ?? string.Empty;
        string currentDir = Environment.CurrentDirectory ?? string.Empty;

        if (!string.IsNullOrWhiteSpace(baseDir))
            candidates.AddRange(this.BuildCandidatePaths(baseDir));

        if (!string.IsNullOrWhiteSpace(currentDir))
            candidates.AddRange(this.BuildCandidatePaths(currentDir));

        return candidates.FirstOrDefault(File.Exists) ?? string.Empty;
    }

    private IEnumerable<string> BuildCandidatePaths(string startDirectory)
    {
        var results = new List<string>();
        try
        {
            DirectoryInfo dir = new DirectoryInfo(startDirectory);
            int maxDepth = 8;
            int depth = 0;

            while (dir != null && depth <= maxDepth)
            {
                results.Add(System.IO.Path.Combine(dir.FullName, "FFlags", "allflag.json"));
                dir = dir.Parent;
                depth++;
            }
        }
        catch
        {
        }
        return results;
    }

    private void DeleteSelectedFlagBtn_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var selected = this.FlagsDataGrid.SelectedItems.Cast<FlagItem>().ToList();
            if (selected.Count == 0)
            {
                this.Log("[Edit] No flag selected");
                return;
            }

            foreach (var flag in selected)
            {
                this._allFlagsList.Remove(flag);
                this._editableFlagsList.Remove(flag);
            }

            this.FlagsDataGrid.SelectedItems.Clear();

            if (this._initializationComplete)
                this.HasUnsavedChanges = true;
            this.UpdateEditStats();

            if (this._allFlagsList.Count == 0)
            {
                this.ResetToNoFileLoadedState();
                this.Log($"[Edit] ✓ Deleted {selected.Count} flag(s); list empty (state = no file loaded)");
            }
            else
            {
                this.UpdateCounts(this._allFlagsList.Count, 0, 0);
                this.Log($"[Edit] ✓ Deleted {selected.Count} flag(s) (total: {this._allFlagsList.Count})");
            }
        }
        catch (Exception ex)
        {
            this.Log($"[Edit] Error deleting flag: {ex.Message}");
        }
    }

    private void ClearAllFlagsBtn_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var result = MessageBox.Show("Are you sure you want to clear all flags from the editor?\n\n⚠ This will only remove flags from the display, NOT the original file.",
                "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                this._allFlagsList.Clear();
                this._editableFlagsList.Clear();
                this.UpdateEditStats();
                this.ResetToNoFileLoadedState();
                if (this._initializationComplete)
                    this.HasUnsavedChanges = true;
                this.Log("[Edit] ✓ All flags cleared from editor (state = no file loaded)");
            }
        }
        catch (Exception ex)
        {
            this.Log($"[Edit] Error clearing flags: {ex.Message}");
        }
    }

    private void ExportJsonBtn_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Title = "Export FFlags JSON";
            saveDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
            saveDialog.DefaultExt = ".json";

            if (saveDialog.ShowDialog() == true)
            {
                var jsonDict = new Dictionary<string, object>();

                foreach (var flag in this._allFlagsList)
                {
                    try
                    {
                        object value = ParseFlagValue(flag.Value);
                        jsonDict[flag.Name] = value;
                    }
                    catch
                    {
                        jsonDict[flag.Name] = flag.Value;
                    }
                }

                string json = JsonSerializer.Serialize(jsonDict, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(saveDialog.FileName, json);
                this.Log($"[Edit] ✓ Exported to: {System.IO.Path.GetFileName(saveDialog.FileName)}");
            }
        }
        catch (Exception ex)
        {
            this.Log($"[Edit] Export error: {ex.Message}");
        }
    }

    private object ParseFlagValue(string value)
    {
        value = value.Trim();

        if (bool.TryParse(value, out bool boolValue))
            return boolValue;

        if (int.TryParse(value, out int intValue))
            return intValue;

        if (double.TryParse(value, out double doubleValue))
            return doubleValue;

        if (value.StartsWith("\"") && value.EndsWith("\""))
            return value.Substring(1, value.Length - 2);

        if (value == "null")
            return null;

        return value;
    }

    private void UpdateEditStats()
    {
        try
        {
            this.Dispatcher.Invoke(() =>
            {
                int totalCount = this._allFlagsList.Count;
                int editedCount = this._allFlagsList.Count(f => f.IsEdited);

                if (editedCount > 0)
                {
                    this.Log($"[Edit] Status: {editedCount} flag(s) modified");
                }
            });
        }
        catch { }
    }

    private void ApplyEditedFlags()
    {
        try
        {
            this.AutoSaveFlags();

            var editedFlags = this._allFlagsList.Where(f => f.IsEdited).ToList();
            if (editedFlags.Count > 0)
            {
                this.Log($"[FFlags] ✓ Applied {editedFlags.Count} edited flags");
            }

            foreach (var flag in this._allFlagsList)
            {
                flag.IsEdited = false;
            }
            this.UpdateEditStats();
        }
        catch (Exception ex)
        {
            this.Log($"[FFlags] Error applying edited flags: {ex.Message}");
        }
    }


    private void AutoSaveFlags()
    {
        try
        {
            this.UpdateCountsFromEditableList();
            this.Log($"[AutoSave] ✓ Flags saved in memory ({this._editableFlagsList.Count} items)");
            this.AddActivityEntry($"✓ Auto-saved {this._editableFlagsList.Count} FFlags to memory", Colors.CornflowerBlue);
        }
        catch (Exception ex)
        {
            this.Log($"[AutoSave] Error: {ex.Message}");
            this.AddActivityEntry($"⚠ Auto-save error: {ex.Message}", Colors.Orange);
        }
    }

    private void AllowManageFastFlagsToggle_Checked(object sender, RoutedEventArgs e)
    {
        try
        {
            if (this._initializationComplete)
            {
                this.HasUnsavedChanges = true;
                this.Log("[Settings] ✓ Allow Masterstrap to manage Fast Flags ENABLED - Apply/Auto-apply functionality activated");
            }
        }
        catch (Exception ex)
        {
            this.Log($"[Settings] Error in AllowManageFastFlagsToggle_Checked: {ex.Message}");
        }
    }

    private void AllowManageFastFlagsToggle_Unchecked(object sender, RoutedEventArgs e)
    {
        try
        {
            if (this._initializationComplete)
            {
                this.HasUnsavedChanges = true;
                this.Log("[Settings] ✗ Allow Masterstrap to manage Fast Flags DISABLED - No flag usage allowed");
            }
        }
        catch (Exception ex)
        {
            this.Log($"[Settings] Error in AllowManageFastFlagsToggle_Unchecked: {ex.Message}");
        }
    }

    private void AutoExportFlagsIfEnabled()
    {
        try
        {
            string lastFlagJsonPath = this._settingsManager.GetLastFlagJsonPath();

            if (string.IsNullOrEmpty(lastFlagJsonPath) || !File.Exists(lastFlagJsonPath))
            {
                return;
            }

            var jsonDict = new Dictionary<string, object>();

            foreach (var flag in this._allFlagsList)
            {
                try
                {
                    object value = ParseFlagValue(flag.Value);
                    jsonDict[flag.Name] = value;
                }
                catch
                {
                    jsonDict[flag.Name] = flag.Value;
                }
            }

            string json = JsonSerializer.Serialize(jsonDict, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(lastFlagJsonPath, json);

            this.Log($"[AutoExport] ✓ FFlags auto-exported to: {System.IO.Path.GetFileName(lastFlagJsonPath)}");
            this.AddActivityEntry($"ðŸ’¾ Auto-exported {jsonDict.Count} FFlags to {System.IO.Path.GetFileName(lastFlagJsonPath)}", Colors.DodgerBlue);
        }
        catch (Exception ex)
        {
            this.Log($"[AutoExport] ✗ Error auto-exporting flags: {ex.Message}");
            this.AddActivityEntry($"❌ Auto-export failed: {ex.Message}", Colors.IndianRed);
        }
    }

    private void FlagsDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
    {
        try
        {
            if (e.EditingElement is System.Windows.Controls.TextBox textBox)
            {
                var bindingExpression = textBox.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty);
                bindingExpression?.UpdateSource();

                if (this.FlagsDataGrid.CurrentItem is FlagItem currentFlag)
                {
                    currentFlag.IsEdited = true;
                    this.UpdateEditStats();
                    this.AutoSaveFlags();
                    this.AutoExportFlagsIfEnabled();
                    if (this._initializationComplete)
                        this.HasUnsavedChanges = true;
                }
            }
        }
        catch (Exception ex)
        {
            this.Log($"[Edit] Error saving flag: {ex.Message}");
        }
    }

    private void ApplyRenderingGraphicsFFlags()
    {
        try
        {
            var renderingFlags = new Dictionary<string, string>();
            int appliedCount = 0;

            var renderingFlagNames = new HashSet<string>
            {
                "DebugFRMOptionalMSAALevelOverride",
                "DebugFRMQualityLevelOverride",
                "FixDPIScaling",
                "FRenderingMode",
                "TextureQualityOverride"
            };

            var existingFlagNames = new HashSet<string>(_allFlagsList.Select(f => f.Name), StringComparer.OrdinalIgnoreCase);

            {
                this.Log($"[Rendering] ✓ Applied {appliedCount} Rendering and Graphics FFlags");
                this.AddActivityEntry($"\u2713 Applied {appliedCount} Rendering FFlags to application", Colors.MediumPurple);
            }
            else if (renderingFlags.Count == 0 && existingFlagNames.Intersect(renderingFlagNames).Count() > 0)
            {
                int manualCount = existingFlagNames.Intersect(renderingFlagNames).Count();
                this.Log($"[Rendering] ℹ Rendering FFlags from Edit tab will be used (skipped {manualCount} UI control(s))");
                this.AddActivityEntry($"\u2713 Using {manualCount} manually edited Rendering FFlags from FastFlag Editor", Colors.MediumPurple);
            }
            else if (renderingFlags.Count == 0)
            {
                this.Log($"[Rendering] ℹ No Rendering and Graphics FFlags configured (all set to Automatic)");
            }
        }
        catch (Exception ex)
        {
            this.Log($"[Rendering] ✗ Error applying Rendering and Graphics FFlags: {ex.Message}");
            this.AddActivityEntry($"❌ Rendering FFlags error: {ex.Message}", Colors.OrangeRed);
        }
    }

    private void ApplyRenderingGraphicsFFlagsFromUi()
    {
        try
        {
            var renderingFlags = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            int appliedCount = 0;

            var renderingFlagNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "FIntDebugForceMSAASamples",

                "DFIntDebugFRMQualityLevelOverride",

                "DFFlagDisableDPIScale",

                "FFlagDebugGraphicsDisableDirect3D11",
                "FFlagDebugGraphicsPreferD3D11",
                "FFlagDebugGraphicsPreferVulkan",
                "FFlagDebugGraphicsPreferOpenGL",

                "DFFlagTextureQualityOverrideEnabled",
                "DFIntTextureQualityOverride",

                "DFIntCSGLevelOfDetailSwitchingDistanceStatic",
                "DFIntCSGLevelOfDetailSwitchingDistance",
                "DFIntCSGLevelOfDetailSwitchingDistanceL12",
                "DFIntCSGLevelOfDetailSwitchingDistanceL23",
                "DFIntCSGLevelOfDetailSwitchingDistanceL34",

                "FFlagDebugForceDisableShadows",
                "FFlagDisablePostFx",
                "DisablePostFx",
                "FFlagDebugDisableRenderingPostEffects",
                "FFlagDebugDisableOTAMaterialTexture",
                "DFFlagDebugRenderForceTechnologyVoxel",
                "FFlagDebugForceFutureIsBrightPhase3"
            };

            var existingFlagNames = new HashSet<string>(_allFlagsList.Select(f => f.Name), StringComparer.OrdinalIgnoreCase);

            if (this.AntiAliasingComboBox?.SelectedItem is ComboBoxItem msaaItem)
            {
                if (existingFlagNames.Contains("FIntDebugForceMSAASamples"))
                {
                    this.Log("[Rendering] \u2139 MSAA: using manually edited FIntDebugForceMSAASamples (skipping UI control)");
                }
                else
                {
                    string msaaText = ((msaaItem.Tag as string) ?? msaaItem.Content?.ToString() ?? "Automatic").Trim();
                    string? msaaValue = null;

                    switch (msaaText.ToLowerInvariant())
                    {
                        case "off":
                            msaaValue = "1";
                            break;
                        case "2x":
                            msaaValue = "2";
                            break;
                        case "4x":
                            msaaValue = "4";
                            break;
                        case "8x":
                            msaaValue = "8";
                            break;
                    }

                    if (!string.IsNullOrEmpty(msaaValue))
                    {
                        renderingFlags["FIntDebugForceMSAASamples"] = msaaValue;
                        appliedCount++;
                        this.Log($"[Rendering] ✓ MSAA Quality: {msaaText} (Value: {msaaValue})");
                    }
                    else
                    {
                        this.Log("[Rendering] \u2139 MSAA set to Automatic (no override flag)");
                    }
                }
            }

            if (this.PreserveQualityToggle?.IsChecked == true)
            {
                if (existingFlagNames.Contains("DFFlagDisableDPIScale"))
                {
                    this.Log("[Rendering] \u2139 Display Scaling: using manually edited DFFlagDisableDPIScale (skipping UI control)");
                }
                else
                {
                    renderingFlags["DFFlagDisableDPIScale"] = "True";
                    appliedCount++;
                    this.Log("[Rendering] ✓ Preserve rendering quality with display scaling: Enabled");
                }
            }

            if (this.FRMQualityToggle?.IsChecked == true)
            {
                if (existingFlagNames.Contains("DFIntDebugFRMQualityLevelOverride"))
                {
                    this.Log("[Rendering] \u2139 FRM Quality: using manually edited DFIntDebugFRMQualityLevelOverride (skipping UI control)");
                }
                else
                {
                    int frmQuality = 21;
                    var frmSlider = this.FindName("FRMQualitySlider") as Slider;
                    if (frmSlider != null)
                    {
                        frmQuality = (int)frmSlider.Value;
                    }

                    frmQuality = Math.Clamp(frmQuality, 1, 21);
                    renderingFlags["DFIntDebugFRMQualityLevelOverride"] = frmQuality.ToString();
                    appliedCount++;
                    this.Log($"[Rendering] ✓ FRM Quality Override: {frmQuality}");
                }
            }

            if (this.MeshDetailToggle?.IsChecked == true)
            {
                bool anyMeshManual =
                    existingFlagNames.Contains("DFIntCSGLevelOfDetailSwitchingDistanceStatic") ||
                    existingFlagNames.Contains("DFIntCSGLevelOfDetailSwitchingDistance") ||
                    existingFlagNames.Contains("DFIntCSGLevelOfDetailSwitchingDistanceL12") ||
                    existingFlagNames.Contains("DFIntCSGLevelOfDetailSwitchingDistanceL23") ||
                    existingFlagNames.Contains("DFIntCSGLevelOfDetailSwitchingDistanceL34");

                if (anyMeshManual)
                {
                    this.Log("[Rendering] \u2139 Mesh Detail: using manually edited mesh LOD flags (skipping UI control)");
                }
                else
                {
                    renderingFlags["DFIntCSGLevelOfDetailSwitchingDistanceStatic"] = "3";
                    renderingFlags["DFIntCSGLevelOfDetailSwitchingDistance"] = "3";
                    renderingFlags["DFIntCSGLevelOfDetailSwitchingDistanceL12"] = "2";
                    renderingFlags["DFIntCSGLevelOfDetailSwitchingDistanceL23"] = "1";
                    renderingFlags["DFIntCSGLevelOfDetailSwitchingDistanceL34"] = "0";
                    appliedCount += 5;
                    this.Log("[Rendering] ✓ Mesh Detail: Enabled (Static=3, L0=3, L12=2, L23=1, L34=0)");
                }
            }

            if (this.RenderingModeComboBox?.SelectedItem is ComboBoxItem renderItem)
            {
                string renderMode = ((renderItem.Tag as string) ?? renderItem.Content?.ToString() ?? "Automatic").Trim();

                bool anyManualRenderingModeFlag =
                    existingFlagNames.Contains("FFlagDebugGraphicsDisableDirect3D11") ||
                    existingFlagNames.Contains("FFlagDebugGraphicsPreferD3D11") ||
                    existingFlagNames.Contains("FFlagDebugGraphicsPreferVulkan") ||
                    existingFlagNames.Contains("FFlagDebugGraphicsPreferOpenGL");

                if (anyManualRenderingModeFlag)
                {
                    this.Log("[Rendering] \u2139 Rendering Mode: using manually edited debug graphics flags (skipping UI control)");
                }
                else
                {
                    bool preferD3D11 = false;
                    bool preferVulkan = false;
                    bool preferOpenGL = false;
                    bool disableD3D11 = false;

                    switch (renderMode.ToLowerInvariant())
                    {
                        case "vulkan":
                            preferVulkan = true;
                            disableD3D11 = true;
                            break;
                        case "opengl":
                            preferOpenGL = true;
                            disableD3D11 = true;
                            break;
                        case "direct3d11":
                        case "direct3d 11":
                        case "directx 11":
                            preferD3D11 = true;
                            disableD3D11 = false;
                            break;
                    }

                    if (preferVulkan)
                        renderingFlags["FFlagDebugGraphicsPreferVulkan"] = "True";
                    if (preferOpenGL)
                        renderingFlags["FFlagDebugGraphicsPreferOpenGL"] = "True";
                    if (preferD3D11)
                        renderingFlags["FFlagDebugGraphicsPreferD3D11"] = "True";
                    if (disableD3D11)
                        renderingFlags["FFlagDebugGraphicsDisableDirect3D11"] = "True";

                    renderingFlags["FFlagHandleAltEnterFullscreen"] = "True";
                    renderingFlags["FFlagHandleAltEnterFullscreenManually"] = "True";

                    appliedCount += 2;

                    if (preferVulkan || preferOpenGL || preferD3D11 || disableD3D11)
                    {
                        appliedCount += (preferVulkan ? 1 : 0) + (preferOpenGL ? 1 : 0) + (preferD3D11 ? 1 : 0) + (disableD3D11 ? 1 : 0);
                        this.Log($"[Rendering] ✓ Rendering Mode: {renderMode}");
                    }
                    else
                    {
                        this.Log("[Rendering] ℹ Rendering Mode set to Automatic (Alt+Enter fix applied)");
                    }
                }
            }

            if (this.TextureQualityComboBox?.SelectedItem is ComboBoxItem textureItem)
            {
                string textureQuality = ((textureItem.Tag as string) ?? textureItem.Content?.ToString() ?? "Automatic").Trim();

                bool anyTextureManual =
                    existingFlagNames.Contains("DFIntTextureQualityOverride") ||
                    existingFlagNames.Contains("DFFlagTextureQualityOverrideEnabled");

                if (anyTextureManual)
                {
                    this.Log("[Rendering] ⚠ï¸ Texture Quality: using manually edited texture override flags (skipping UI control)");
                }
                else
                {
                    string? textureValue = null;

                    switch (textureQuality.ToLowerInvariant())
                    {
                        case "low":
                            textureValue = "0";
                            break;
                        case "medium":
                            textureValue = "1";
                            break;
                        case "high":
                            textureValue = "2";
                            break;
                    }

                    if (!string.IsNullOrEmpty(textureValue))
                    {
                        renderingFlags["DFFlagTextureQualityOverrideEnabled"] = "True";
                        renderingFlags["DFIntTextureQualityOverride"] = textureValue;
                        appliedCount += 2;
                        this.Log($"[Rendering] ✓ Texture Quality: {textureQuality} (Level {textureValue})");
                    }
                    else
                    {
                        this.Log("[Rendering] \u2139 Texture Quality set to Automatic (no override flags)");
                    }
                }
            }

            if (this.DisablePlayerShadowsToggle?.IsChecked == true)
            {
                if (existingFlagNames.Contains("FFlagDebugForceDisableShadows"))
                {
                    this.Log("[Rendering] \u2139 Player shadows: using manually edited FFlagDebugForceDisableShadows (skipping UI control)");
                }
                else
                {
                    renderingFlags["FFlagDebugForceDisableShadows"] = "True";
                    appliedCount++;
                    this.Log("[Rendering] ✓ Disable player shadows: Enabled");
                }
            }

            if (this.DisablePostProcessingToggle?.IsChecked == true)
            {
                bool anyManualPostFx =
                    existingFlagNames.Contains("FFlagDisablePostFx") ||
                    existingFlagNames.Contains("DisablePostFx") ||
                    existingFlagNames.Contains("FFlagDebugDisableRenderingPostEffects");

                if (anyManualPostFx)
                {
                    this.Log("[Rendering] \u2139 Post-processing: using manually edited postFX flags (skipping UI control)");
                }
                else
                {
                    renderingFlags["FFlagDisablePostFx"] = "True";
                    renderingFlags["DisablePostFx"] = "True";
                    renderingFlags["FFlagDebugDisableRenderingPostEffects"] = "True";
                    appliedCount += 3;
                    this.Log("[Rendering] ✓ Disable post-processing effects: Enabled");
                }
            }

            if (this.DisableTerrainTexturesToggle?.IsChecked == true)
            {
                if (existingFlagNames.Contains("FFlagDebugDisableOTAMaterialTexture"))
                {
                    this.Log("[Rendering] \u2139 Terrain textures: using manually edited FFlagDebugDisableOTAMaterialTexture (skipping UI control)");
                }
                else
                {
                    renderingFlags["FFlagDebugDisableOTAMaterialTexture"] = "True";
                    appliedCount++;
                    this.Log("[Rendering] ✓ Disable terrain textures: Enabled");
                }
            }

            if (this.PreferredLightingTechnologyComboBox?.SelectedItem is ComboBoxItem techItem)
            {
                string tech = ((techItem.Tag as string) ?? techItem.Content?.ToString() ?? "Automatic").Trim();
                bool anyManualTech =
                    existingFlagNames.Contains("DFFlagDebugRenderForceTechnologyVoxel") ||
                    existingFlagNames.Contains("FFlagDebugForceFutureIsBrightPhase3");

                if (anyManualTech)
                {
                    this.Log("[Rendering] \u2139 Lighting technology: using manually edited lighting tech flags (skipping UI control)");
                }
                else
                {
                    switch (tech.ToLowerInvariant())
                    {
                        case "voxel":
                            renderingFlags["DFFlagDebugRenderForceTechnologyVoxel"] = "True";
                            renderingFlags["FFlagDebugForceFutureIsBrightPhase3"] = "False";
                            appliedCount += 2;
                            this.Log("[Rendering] ✓ Preferred lighting technology: Voxel");
                            break;
                        case "future":
                            renderingFlags["FFlagDebugForceFutureIsBrightPhase3"] = "True";
                            renderingFlags["DFFlagDebugRenderForceTechnologyVoxel"] = "False";
                            appliedCount += 2;
                            this.Log("[Rendering] ✓ Preferred lighting technology: Future");
                            break;
                        case "shadowmap":
                            renderingFlags["DFFlagDebugRenderForceTechnologyVoxel"] = "False";
                            renderingFlags["FFlagDebugForceFutureIsBrightPhase3"] = "False";
                            appliedCount += 2;
                            this.Log("[Rendering] ✓ Preferred lighting technology: ShadowMap");
                            break;
                        default:
                            break;
                    }
                }
            }

            {
                this.Log($"[Rendering] \u2713 Applied {appliedCount} Rendering / Geometry FFlags");
                this.AddActivityEntry($"\u2713 Applied {appliedCount} Rendering & Geometry FFlags to application", Colors.MediumPurple);
            }
            else if (renderingFlags.Count == 0 && existingFlagNames.Intersect(renderingFlagNames).Any())
            {
                int manualCount = existingFlagNames.Intersect(renderingFlagNames).Count();
                this.Log($"[Rendering] \u2139 Rendering / Geometry FFlags from FastFlag Editor will be used (skipped {manualCount} UI control(s))");
                this.AddActivityEntry($"\u2713 Using {manualCount} manually edited Rendering / Geometry FFlags from FastFlag Editor", Colors.MediumPurple);
            }
            else if (renderingFlags.Count == 0)
            {
                this.Log("[Rendering] \u2139 No Rendering / Geometry FFlags configured from UI (all Automatic / disabled)");
            }
        }
        catch (Exception ex)
        {
            this.Log($"[Rendering] \u2717 Error applying Rendering and Graphics FFlags: {ex.Message}");
            this.AddActivityEntry($"❌ Rendering FFlags error: {ex.Message}", Colors.OrangeRed);
        }
    }



    private void SetupClockTimer()
    {
        this._clockTimer.Interval = TimeSpan.FromMilliseconds(500);
            this._clockTimer.Tick += (s, e) =>
        {
            this.UpdateRobloxProcessDisplay();
        };
        this._clockTimer.Start();
    }

    private void UpdateRobloxProcessDisplay()
    {
        try
        {
            int robloxCount = this.GetRobloxProcessCount();
            this.Dispatcher.Invoke(() =>
            {
                this.UpdateFooterInfo();
            });
        }
        catch
        {
        }
    }

    private void LeftTabApplyFlagsBtn_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            this.MainTabControl.SelectedIndex = 0;

            this.HighlightActiveTab(this.LeftTabApplyFlagsBtn);

            this.Log("[UI] Switched to Apply tab");
        }
        catch (Exception ex)
        {
            this.Log("[Error] Failed to switch tab: " + ex.Message);
        }
    }

    private void LeftTabEditBtn_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            this.MainTabControl.SelectedIndex = 1;

            this.HighlightActiveTab(this.LeftTabEditBtn);

            this.Log("[UI] Switched to FastFlag tab");
        }
        catch (Exception ex)
        {
            this.Log("[Error] Failed to switch tab: " + ex.Message);
        }
    }

    private void LeftTabSettingsBtn_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            this.MainTabControl.SelectedIndex = 6;

            this.HighlightActiveTab(this.LeftTabSettingsBtn);

            this.Log("[UI] Switched to Settings tab");
        }
        catch (Exception ex)
        {
            this.Log("[Error] Failed to switch tab: " + ex.Message);
        }
    }

    private void LeftTabFaqBtn_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            this.MainTabControl.SelectedIndex = 10;

            this.HighlightActiveTab(this.LeftTabFaqBtn);

            this.Log("[UI] Switched to About tab");
        }
        catch (Exception ex)
        {
            this.Log("[Error] Failed to switch tab: " + ex.Message);
        }
    }

    private void HighlightActiveTab(Button activeTab)
    {
        try
        {
            this.Dispatcher.Invoke(() =>
            {
                int idx = 0;
                if (activeTab == this.LeftTabApplyFlagsBtn)
                {
                    idx = 0;
                }
                else if (activeTab == this.LeftTabEditBtn)
                {
                    idx = 1;
                }
                else if (activeTab == this.LeftTabSettingsBtn)
                {
                    idx = 9;
                }
                else if (activeTab == this.LeftTabFaqBtn)
                {
                    idx = 10;
                }

                this.UpdateTabBorderHighlight(idx);
            });
        }
        catch (Exception ex)
        {
            this.Log("[Error] Error highlighting tab: " + ex.Message);
        }
    }

                      private void AutoLoadCacheToggle_Checked(object sender, RoutedEventArgs e)
                      {
                          try
                          {
                              if (this._initializationComplete)
                              {
                                  this.HasUnsavedChanges = true;
                                  this.Log("[Settings] ✓ Auto-load Cache enabled");
                              }
                          }
                          catch (Exception ex)
                          {
                              this.Log($"[Settings] Error: {ex.Message}");
                          }
                      }

                      private void AutoLoadCacheToggle_Unchecked(object sender, RoutedEventArgs e)
                      {
                          try
                          {
                              if (this._initializationComplete)
                              {
                                  this.HasUnsavedChanges = true;
                                  this.Log("[Settings] ✗ Auto-load Cache disabled");
                              }
                          }
                          catch (Exception ex)
                          {
                              this.Log($"[Settings] Error: {ex.Message}");
                          }
                      }

                      private void RestoreToggleStates()
                      {
                          try
                          {
                              var (desktopShortcut, _, _, _) = this._settingsManager.GetToggleStates();
                                      bool fastMode = this._settingsManager.IsFastModeEnabled();

                                      bool allowManageFastFlags = this._settingsManager.IsAllowManageFastFlagsEnabled();

                              Action restore = () =>
                              {
                                  if (this.DesktopShortcutToggle != null)
                                      this.DesktopShortcutToggle.IsChecked = desktopShortcut;
                                  if (this.StartMenuShortcutToggle != null)
                                      this.StartMenuShortcutToggle.IsChecked = this._settingsManager.IsStartMenuShortcutEnabled();
                                  if (this.LaunchRobloxShortcutToggle != null)
                                      this.LaunchRobloxShortcutToggle.IsChecked = this._settingsManager.IsLaunchRobloxShortcutEnabled();
                                  if (this.FastModeToggle != null)
                                      this.FastModeToggle.IsChecked = fastMode;
                                  if (this.AllowManageFastFlagsToggle != null)
                                      this.AllowManageFastFlagsToggle.IsChecked = allowManageFastFlags;
                                   if (this.ProtocolInterceptionToggle != null)
                                       this.ProtocolInterceptionToggle.IsChecked = this._settingsManager.IsProtocolInterceptionEnabled();
                                  this._unlock240FpsMode = this._settingsManager.GetUnlock240FpsMode();
                                  this._unlock240GlobalFpsRequested = this._settingsManager.GetUnlock240GlobalFpsRequested();
                                  this.UpdateUnlock240FpsModeButtons();
                                  this.SyncUnlock240FpsModeToService();
                                  this.UpdateUnlock240GlobalFpsUiAndVisibility();
                                  this.Log("[Settings] ✓ Toggle states restored from settings");
                              };
                              if (this.Dispatcher.CheckAccess())
                                  restore();
                              else
                                  this.Dispatcher.Invoke(restore);
                          }
                          catch (Exception ex)
                          {
                              this.Log($"[Settings] ✗ Error restoring toggle states: {ex.Message}");
                          }
                      }

                      private async void AutoLoadFlagsIfEnabled()
                      {
                          try
                          {
                              var (_, _, autoLoadCache, _) = this._settingsManager.GetToggleStates();
                              autoLoadCache = true;

                              {
                                  if (this.TryAutoLoadFlagsFromPersistedSession("[AutoLoad]"))
                                  {
                                      this.AddActivityEntry($"✓ Auto-loaded {this._flagsCount} FFlags", Colors.LimeGreen);
                                      string pathAgain = this._settingsManager.GetLastFlagJsonPath();
                                      if (!string.IsNullOrEmpty(pathAgain) && System.IO.File.Exists(pathAgain))
                                          this.DisplayJsonContentInLog(pathAgain);
                                  }
                                  else
                                      this.Log("[AutoLoad] ⚠ï¸ No saved FFlags file or game preset to restore");
                              }

                          }
                          catch (Exception ex)
                          {
                              this.Log($"[AutoLoad] ✗ Error during auto-load: {ex.Message}");
                          }
                      }

                      private string GetSelectedRenderingTag(ComboBox comboBox, string fallback)
                      {
                          if (comboBox?.SelectedItem is ComboBoxItem item)
                          {
                              string tag = item.Tag as string;
                              if (!string.IsNullOrWhiteSpace(tag))
                                  return tag.Trim();
                              string content = item.Content?.ToString();
                              if (!string.IsNullOrWhiteSpace(content))
                                  return content.Trim();
                          }
                          return fallback;
                      }

                      private void SelectRenderingComboByTag(ComboBox comboBox, string savedValue, string fallback)
                      {
                          if (comboBox == null)
                              return;

                          string desired = string.IsNullOrWhiteSpace(savedValue) ? fallback : savedValue.Trim();

                          var taggedMatch = comboBox.Items
                              .OfType<ComboBoxItem>()
                              .FirstOrDefault(item => string.Equals(item.Tag as string, desired, StringComparison.OrdinalIgnoreCase));

                          if (taggedMatch != null)
                          {
                              comboBox.SelectedItem = taggedMatch;
                              return;
                          }

                          var contentMatch = comboBox.Items
                              .OfType<ComboBoxItem>()
                              .FirstOrDefault(item => string.Equals(item.Content?.ToString(), desired, StringComparison.OrdinalIgnoreCase));

                          if (contentMatch != null)
                          {
                              comboBox.SelectedItem = contentMatch;
                              return;
                          }

                          var fallbackMatch = comboBox.Items
                              .OfType<ComboBoxItem>()
                              .FirstOrDefault(item => string.Equals(item.Tag as string, fallback, StringComparison.OrdinalIgnoreCase));

                          if (fallbackMatch != null)
                          {
                              comboBox.SelectedItem = fallbackMatch;
                              return;
                          }

                          comboBox.SelectedIndex = 0;
                      }

                      private void ApplyFirstRunRenderingDefaultsToUI()
                      {
                          if (this.AntiAliasingComboBox != null)
                              this.SelectRenderingComboByTag(this.AntiAliasingComboBox, "Automatic", "Automatic");

                          if (this.RenderingModeComboBox != null)
                              this.SelectRenderingComboByTag(this.RenderingModeComboBox, "Automatic", "Automatic");

                          if (this.TextureQualityComboBox != null)
                              this.SelectRenderingComboByTag(this.TextureQualityComboBox, "Automatic", "Automatic");

                          if (this.PreferredLightingTechnologyComboBox != null)
                              this.SelectRenderingComboByTag(this.PreferredLightingTechnologyComboBox, "Automatic", "Automatic");

                          if (this.PreserveQualityToggle != null)
                              this.PreserveQualityToggle.IsChecked = false;

                          if (this.FRMQualityToggle != null)
                              this.FRMQualityToggle.IsChecked = false;

                          if (this.MeshDetailToggle != null)
                              this.MeshDetailToggle.IsChecked = false;

                          if (this.DisablePlayerShadowsToggle != null)
                              this.DisablePlayerShadowsToggle.IsChecked = false;

                          if (this.DisablePostProcessingToggle != null)
                              this.DisablePostProcessingToggle.IsChecked = false;

                          if (this.DisableTerrainTexturesToggle != null)
                              this.DisableTerrainTexturesToggle.IsChecked = false;

                          var frmSlider = this.FindName("FRMQualitySlider") as Slider;
                          var frmContainer = this.FindName("FRMQualitySliderContainer") as Grid;
                          if (frmSlider != null)
                              frmSlider.Value = 21;
                          if (frmContainer != null)
                          {
                              frmContainer.RenderTransform ??= new TranslateTransform();
                              AnimateSlideContainer(frmContainer, show: false, expandedHeight: 44);
                          }
                      }

                      private void RestoreRenderingSettings()
                      {
                          try
                          {
                              if (this._settingsManager != null &&
                                  this._settingsManager.WasFirstRun &&
                                  !this._settingsManager.GetFirstRunRenderingDefaultsApplied())
                              {
                                  this.Dispatcher.Invoke(() =>
                                  {
                                      ApplyFirstRunRenderingDefaultsToUI();
                                      this.Log("[Settings] ✓ Applied first-run Rendering & Graphics defaults (Automatic + OFF)");
                                  });

                                  this._settingsManager.SaveRenderingSettings("Automatic", "Automatic", "Automatic");
                                  this._settingsManager.SaveRenderingToggles(false, false, meshDetailEnabled: false, meshDetailValue: 3);
                                  this._settingsManager.SaveFRMQualityValue(21);
                                  this._settingsManager.SaveRenderingExtras(false, false, false, "Automatic");
                                  this._settingsManager.SetFirstRunRenderingDefaultsApplied(true);
                                  return;
                              }

                              var renderingSettings = this._settingsManager.GetRenderingSettings();

                              this.Dispatcher.Invoke(() =>
                              {
                                  if (this.AntiAliasingComboBox != null)
                                  {
                                      this.SelectRenderingComboByTag(this.AntiAliasingComboBox, renderingSettings.MSAAQuality, "Automatic");
                                  }

                                  if (this.RenderingModeComboBox != null)
                                  {
                                      this.SelectRenderingComboByTag(this.RenderingModeComboBox, renderingSettings.RenderingMode, "Automatic");
                                  }

                                  if (this.TextureQualityComboBox != null)
                                  {
                                      this.SelectRenderingComboByTag(this.TextureQualityComboBox, renderingSettings.TextureQuality, "Automatic");
                                  }

                                  if (this.PreserveQualityToggle != null)
                                  {
                                      this.PreserveQualityToggle.IsChecked = renderingSettings.PreserveRenderingQuality;
                                      this.Log($"[Settings] ✓ Restored Preserve Rendering Quality: {renderingSettings.PreserveRenderingQuality}");
                                  }

                                  if (this.FRMQualityToggle != null)
                                  {
                                      this.FRMQualityToggle.IsChecked = renderingSettings.FRMQuality;
                                      this.Log($"[Settings] ✓ Restored FRM Quality: {renderingSettings.FRMQuality}");
                                  }

                                  if (this.MeshDetailToggle != null)
                                  {
                                      this.MeshDetailToggle.IsChecked = renderingSettings.MeshDetailEnabled;
                                      this.Log($"[Settings] ✓ Restored Mesh Detail: {renderingSettings.MeshDetailEnabled}");
                                  }

                if (this.DisablePlayerShadowsToggle != null)
                {
                    this.DisablePlayerShadowsToggle.IsChecked = renderingSettings.DisablePlayerShadows;
                    this.Log($"[Settings] ✓ Restored Disable Player Shadows: {renderingSettings.DisablePlayerShadows}");
                }

                if (this.DisablePostProcessingToggle != null)
                {
                    this.DisablePostProcessingToggle.IsChecked = renderingSettings.DisablePostProcessingEffects;
                    this.Log($"[Settings] ✓ Restored Disable Post-Processing: {renderingSettings.DisablePostProcessingEffects}");
                }

                if (this.DisableTerrainTexturesToggle != null)
                {
                    this.DisableTerrainTexturesToggle.IsChecked = renderingSettings.DisableTerrainTextures;
                    this.Log($"[Settings] ✓ Restored Disable Terrain Textures: {renderingSettings.DisableTerrainTextures}");
                }

                if (this.PreferredLightingTechnologyComboBox != null)
                {
                    this.SelectRenderingComboByTag(this.PreferredLightingTechnologyComboBox, renderingSettings.PreferredLightingTechnology, "Automatic");
                }

                                  var frmSlider = this.FindName("FRMQualitySlider") as Slider;
                                  var frmContainer = this.FindName("FRMQualitySliderContainer") as Grid;

                                  if (frmSlider != null && frmContainer != null)
                                  {
                                      int frmQualityValue = renderingSettings.FRMQualityValue;
                                      frmSlider.Value = frmQualityValue > 0 ? frmQualityValue : 21;

                                      if (renderingSettings.FRMQuality)
                                      {
                                          frmContainer.RenderTransform ??= new TranslateTransform();
                                          AnimateSlideContainer(frmContainer, show: true, expandedHeight: 44);
                                      }
                                      else
                                      {
                                          frmContainer.RenderTransform ??= new TranslateTransform();
                                          AnimateSlideContainer(frmContainer, show: false, expandedHeight: 44);
                                      }

                                      this.Log($"[Settings] ✓ Restored FRM Quality Slider: {frmQualityValue}");
                                  }

                                  this.Log("[Settings] ✓ Rendering settings restored from settings");
                              });
                          }
                          catch (Exception ex)
                          {
                              this.Log($"[Settings] ✗ Error restoring rendering settings: {ex.Message}");
                          }
                      }

                      private void AttachToggleEventHandlers()
                      {
                          try
                          {
                              this.Log("[Settings] ✓ General toggle event handlers attached after initialization");
                          }
                          catch (Exception ex)
                          {
                              this.Log($"[Settings] ✗ Error attaching toggle event handlers: {ex.Message}");
                          }
                      }

                      private void AttachRenderingEventHandlers()
                      {
                          try
                          {
                              if (this.AntiAliasingComboBox != null)
                              {
                                  this.AntiAliasingComboBox.SelectionChanged += RenderingComboBox_SelectionChanged;
                              }

                              if (this.RenderingModeComboBox != null)
                              {
                                  this.RenderingModeComboBox.SelectionChanged += RenderingComboBox_SelectionChanged;
                              }

                              if (this.TextureQualityComboBox != null)
                              {
                                  this.TextureQualityComboBox.SelectionChanged += RenderingComboBox_SelectionChanged;
                              }

            if (this.PreferredLightingTechnologyComboBox != null)
            {
                this.PreferredLightingTechnologyComboBox.SelectionChanged += RenderingComboBox_SelectionChanged;
            }

                              if (this.DesktopShortcutToggle != null)
                              {
                                  this.DesktopShortcutToggle.Checked += DesktopShortcutToggle_Checked;
                                  this.DesktopShortcutToggle.Unchecked += DesktopShortcutToggle_Unchecked;
                              }
                              if (this.StartMenuShortcutToggle != null)
                              {
                                  this.StartMenuShortcutToggle.Checked += StartMenuShortcutToggle_Checked;
                                  this.StartMenuShortcutToggle.Unchecked += StartMenuShortcutToggle_Unchecked;
                              }
                              if (this.LaunchRobloxShortcutToggle != null)
                              {
                                  this.LaunchRobloxShortcutToggle.Checked += LaunchRobloxShortcutToggle_Checked;
                                  this.LaunchRobloxShortcutToggle.Unchecked += LaunchRobloxShortcutToggle_Unchecked;
                              }

                              if (this.PreserveQualityToggle != null)
                              {
                                  this.PreserveQualityToggle.Checked += (s, e) =>
                                  {
                                      if (this._initializationComplete)
                                      {
                                          this.HasUnsavedChanges = true;
                                          this.Log("[Settings] ✓ Preserve Rendering Quality enabled");
                                      }
                                  };

                                  this.PreserveQualityToggle.Unchecked += (s, e) =>
                                  {
                                      if (this._initializationComplete)
                                      {
                                          this.HasUnsavedChanges = true;
                                          this.Log("[Settings] ✓ Preserve Rendering Quality disabled");
                                      }
                                  };
                              }

                              if (this.MeshDetailToggle != null)
                              {
                                  this.MeshDetailToggle.Checked += (s, e) =>
                                  {
                                      if (this._initializationComplete)
                                      {
                                          this.HasUnsavedChanges = true;
                                          this.Log("[Settings] ✓ Mesh Detail enabled");
                                      }
                                  };

                                  this.MeshDetailToggle.Unchecked += (s, e) =>
                                  {
                                      if (this._initializationComplete)
                                      {
                                          this.HasUnsavedChanges = true;
                                          this.Log("[Settings] ✓ Mesh Detail disabled");
                                      }
                                  };
                              }

            void AttachSimpleUnsavedToggle(ToggleButton? t, string label)
            {
                if (t == null)
                    return;

                t.Checked += (s, e) =>
                {
                    if (this._initializationComplete)
                    {
                        this.HasUnsavedChanges = true;
                        this.Log($"[Settings] ✓ {label}: enabled");
                    }
                };

                t.Unchecked += (s, e) =>
                {
                    if (this._initializationComplete)
                    {
                        this.HasUnsavedChanges = true;
                        this.Log($"[Settings] ✓ {label}: disabled");
                    }
                };
            }

            AttachSimpleUnsavedToggle(this.DisablePlayerShadowsToggle, "Disable Player Shadows");
            AttachSimpleUnsavedToggle(this.DisablePostProcessingToggle, "Disable Post-Processing Effects");
            AttachSimpleUnsavedToggle(this.DisableTerrainTexturesToggle, "Disable Terrain Textures");

                              var frmSlider = this.FindName("FRMQualitySlider") as Slider;
                              if (frmSlider != null)
                              {
                                  frmSlider.PreviewMouseLeftButtonUp += FRMQualitySlider_ValueChanged;
                                  frmSlider.KeyUp += FRMQualitySlider_KeyUp;
                              }

                              this.Log("[Settings] ✓ Rendering event handlers attached after initialization");
                          }
                          catch (Exception ex)
                          {
                              this.Log($"[Settings] ✗ Error attaching rendering event handlers: {ex.Message}");
                          }
                      }

                      private void RenderingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
                      {
                          try
                          {
                              if (this._initializationComplete)
                              {
                                  this.HasUnsavedChanges = true;
                                  this.Log("[Settings] Rendering setting changed - marked as unsaved");
                              }
                          }
                          catch (Exception ex)
                          {
                              this.Log($"[Settings] ✗ Error in RenderingComboBox_SelectionChanged: {ex.Message}");
                          }
                      }

                      private void AntiAliasingCard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
                      {
                          try
                          {
                              if (this.AntiAliasingComboBox != null)
                              {
                                  if (e.OriginalSource is DependencyObject source &&
                                      IsSourceInsideElement(source, this.AntiAliasingComboBox))
                                  {
                                      return;
                                  }
                                  this.AntiAliasingComboBox.Focus();
                                  this.AntiAliasingComboBox.IsDropDownOpen = true;
                              }
                          }
                          catch (Exception ex)
                          {
                              this.Log($"[UI] ✗ Error opening AntiAliasingComboBox from card click: {ex.Message}");
                          }
                      }

                      private void RenderingModeCard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
                      {
                          try
                          {
                              if (this.RenderingModeComboBox != null)
                              {
                                  if (e.OriginalSource is DependencyObject source &&
                                      IsSourceInsideElement(source, this.RenderingModeComboBox))
                                  {
                                      return;
                                  }
                                  this.RenderingModeComboBox.Focus();
                                  this.RenderingModeComboBox.IsDropDownOpen = true;
                              }
                          }
                          catch (Exception ex)
                          {
                              this.Log($"[UI] ✗ Error opening RenderingModeComboBox from card click: {ex.Message}");
                          }
                      }

                      private void TextureQualityCard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
                      {
                          try
                          {
                              if (this.TextureQualityComboBox != null)
                              {
                                  if (e.OriginalSource is DependencyObject source &&
                                      IsSourceInsideElement(source, this.TextureQualityComboBox))
                                  {
                                      return;
                                  }
                                  this.TextureQualityComboBox.Focus();
                                  this.TextureQualityComboBox.IsDropDownOpen = true;
                              }
                          }
                          catch (Exception ex)
                          {
                              this.Log($"[UI] ✗ Error opening TextureQualityComboBox from card click: {ex.Message}");
                          }
                      }

    private void PreferredLightingTechnologyCard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        try
        {
            if (this.PreferredLightingTechnologyComboBox != null)
            {
                if (e.OriginalSource is DependencyObject source &&
                    IsSourceInsideElement(source, this.PreferredLightingTechnologyComboBox))
                {
                    return;
                }
                this.PreferredLightingTechnologyComboBox.Focus();
                this.PreferredLightingTechnologyComboBox.IsDropDownOpen = true;
            }
        }
        catch (Exception ex)
        {
            this.Log($"[UI] ✗ Error opening PreferredLightingTechnologyComboBox from card click: {ex.Message}");
        }
    }

                      private static bool IsSourceInsideElement(DependencyObject source, DependencyObject target)
                      {
                          DependencyObject current = source;
                          while (current != null)
                          {
                              if (ReferenceEquals(current, target))
                                  return true;

                              DependencyObject visualParent = null;
                              if (current is Visual || current is System.Windows.Media.Media3D.Visual3D)
                              {
                                  visualParent = VisualTreeHelper.GetParent(current);
                              }

                              current = visualParent ?? LogicalTreeHelper.GetParent(current);
                          }

                          return false;
                      }

                      private void FRMQualityToggle_Checked(object sender, RoutedEventArgs e)
                      {
                          try
                          {
                              var frmContainer = this.FindName("FRMQualitySliderContainer") as Grid;
                              if (frmContainer != null)
                              {
                                  frmContainer.RenderTransform ??= new TranslateTransform();
                                  AnimateSlideContainer(frmContainer, show: true, expandedHeight: 44);
                                  this.Log("[Settings] ✓ FRM Quality Slider shown");
                              }

                              if (this._initializationComplete)
                              {
                                  this.HasUnsavedChanges = true;
                              }
                          }
                          catch (Exception ex)
                          {
                              this.Log($"[Settings] ✗ Error in FRM Quality Toggle Checked: {ex.Message}");
                          }
                      }

                      private void FRMQualityToggle_Unchecked(object sender, RoutedEventArgs e)
                      {
                          try
                          {
                              var frmContainer = this.FindName("FRMQualitySliderContainer") as Grid;
                              if (frmContainer != null)
                              {
                                  frmContainer.RenderTransform ??= new TranslateTransform();
                                  AnimateSlideContainer(frmContainer, show: false, expandedHeight: 44);
                                  this.Log("[Settings] ✓ FRM Quality Slider hidden");
                              }

                              if (this._initializationComplete)
                              {
                                  this.HasUnsavedChanges = true;
                              }
                                             }
                                             catch (Exception ex)
                                             {
                                                 this.Log($"[Settings] ✗ Error in FRM Quality Toggle Unchecked: {ex.Message}");
                                             }
                                         }

                          private void ShowToastNotification(string message)
                          {
                              try
                              {
            var toastPanelEarly = this.FindName("ToastNotificationsPanel") as ItemsControl;
            if (toastPanelEarly != null && toastPanelEarly.Items.Count > 0)
            {
                if (toastPanelEarly.Items[0] is Border existingToast)
                {
                    toastPanelEarly.Items.Remove(existingToast);
                    existingToast.Opacity = 0;
                }
                return;
            }

                                  Border toastBorder = new Border
                                  {
                                      Style = this.FindResource("ToastNotificationStyle") as Style
                                  };

                                  TextBlock toastText = new TextBlock
                                  {
                                      Text = message,
                                      Foreground = new SolidColorBrush(Colors.White),
                                      FontSize = 13,
                                      FontWeight = FontWeights.Normal,
                                      VerticalAlignment = VerticalAlignment.Center,
                                      HorizontalAlignment = HorizontalAlignment.Center,
                                      TextAlignment = TextAlignment.Center
                                  };

                                  toastBorder.Child = toastText;
                                  toastBorder.RenderTransform = new TranslateTransform();

                                  var toastPanel = this.FindName("ToastNotificationsPanel") as ItemsControl;
                                  if (toastPanel != null)
                                  {
                                      toastPanel.Items.Add(toastBorder);

                                      Storyboard slideUpAnimation = this.FindResource("ToastSlideUpAnimation") as Storyboard;
                                      if (slideUpAnimation != null)
                                      {
                                          Storyboard.SetTarget(slideUpAnimation, toastBorder);
                                          slideUpAnimation.Begin();
                                      }

                                      DispatcherTimer removeTimer = new DispatcherTimer();
                                      removeTimer.Interval = TimeSpan.FromSeconds(3);
                                      removeTimer.Tick += (s, e) =>
                                      {
                                          removeTimer.Stop();
                                  if (toastPanel != null && !toastPanel.Items.Contains(toastBorder))
                                      return;

                                          Storyboard slideDownAnimation = this.FindResource("ToastSlideDownAnimation") as Storyboard;
                                          if (slideDownAnimation != null)
                                          {
                                              slideDownAnimation.Completed += (sender, args) =>
                                              {
                                                  toastPanel.Items.Remove(toastBorder);
                                              };
                                              Storyboard.SetTarget(slideDownAnimation, toastBorder);
                                              slideDownAnimation.Begin();
                                          }
                                          else
                                          {
                                              toastPanel.Items.Remove(toastBorder);
                                          }
                                      };
                                      removeTimer.Start();
                                  }
                              }
                              catch (Exception ex)
                              {
                                  this.Log($"[Toast] Error showing toast notification: {ex.Message}");
                              }
                          }

                      private void FRMQualitySlider_ValueChanged(object sender, MouseButtonEventArgs e)
                      {
                          try
                          {
                              var frmSlider = this.FindName("FRMQualitySlider") as Slider;
                              if (frmSlider != null)
                              {
                                  int sliderValue = (int)frmSlider.Value;
                                  this._settingsManager.SaveFRMQualityValue(sliderValue);
                                  this.Log($"[Settings] ✓ FRM Quality Slider value saved: {sliderValue}");

                                  if (this._initializationComplete)
                                  {
                                      this.HasUnsavedChanges = true;
                                      this.Log("[Settings] Rendering setting changed - marked as unsaved");
                                  }
                              }
                          }
                          catch (Exception ex)
                          {
                              this.Log($"[Settings] ✗ Error saving FRM Quality Slider value: {ex.Message}");
                          }
                      }

                      private void FRMQualitySlider_KeyUp(object sender, KeyEventArgs e)
                      {
                          try
                          {
                              var frmSlider = this.FindName("FRMQualitySlider") as Slider;
                              if (frmSlider != null)
                              {
                                  int sliderValue = (int)frmSlider.Value;
                                  this._settingsManager.SaveFRMQualityValue(sliderValue);
                                  this.Log($"[Settings] ✓ FRM Quality Slider value saved: {sliderValue}");

                                  if (this._initializationComplete)
                                  {
                                      this.HasUnsavedChanges = true;
                                      this.Log("[Settings] Rendering setting changed - marked as unsaved");
                                  }
                              }
                          }
                          catch (Exception ex)
                          {
                              this.Log($"[Settings] ✗ Error saving FRM Quality Slider value: {ex.Message}");
                          }
                      }
                          private void ApplyShortcutSettings()
                          {
                              try
                              {
                                  string exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName ?? "";
                                  if (string.IsNullOrWhiteSpace(exePath) || !System.IO.File.Exists(exePath))
                                      return;

                                  string desktopShortcutPath = System.IO.Path.Combine(
                                      Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                                      "Masterstrap.lnk");
                                  string startMenuDir = System.IO.Path.Combine(
                                      Environment.GetFolderPath(Environment.SpecialFolder.StartMenu),
                                      "Programs");
                                  string startMenuShortcutPath = System.IO.Path.Combine(startMenuDir, "Masterstrap.lnk");
                                  string launchRobloxShortcutPath = System.IO.Path.Combine(
                                      Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                                      "Launch Roblox.lnk");

                                  bool desktopEnabled = this.DesktopShortcutToggle?.IsChecked ?? false;
                                  bool startMenuEnabled = this.StartMenuShortcutToggle?.IsChecked ?? false;
                                  bool launchRobloxEnabled = this.LaunchRobloxShortcutToggle?.IsChecked ?? false;

                                  this.CreateOrDeleteShortcut(desktopEnabled, desktopShortcutPath, exePath, "",
                                      "Masterstrap", exePath);
                                  this.CreateOrDeleteShortcut(startMenuEnabled, startMenuShortcutPath, exePath, "",
                                      "Masterstrap", exePath);

                                  string iconCandidate = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(exePath) ?? "", "masterstrap.ico");
                                  string launchIcon = System.IO.File.Exists(iconCandidate) ? iconCandidate : exePath;
                                  this.CreateOrDeleteShortcut(launchRobloxEnabled, launchRobloxShortcutPath, exePath, "--launch-and-apply-flags --main",
                                      "Launch Roblox", launchIcon);
                              }
                              catch (Exception ex)
                              {
                                  this.Log($"[Settings] Shortcut apply error: {ex.Message}");
                              }
                          }

                          private void CreateOrDeleteShortcut(bool enabled, string shortcutPath, string targetPath, string arguments, string description, string iconPath)
                          {
                              try
                              {
                                  if (enabled)
                                  {
                                      string? dir = System.IO.Path.GetDirectoryName(shortcutPath);
                                      if (!string.IsNullOrWhiteSpace(dir))
                                          Directory.CreateDirectory(dir);

                                      dynamic shell = Activator.CreateInstance(Type.GetTypeFromProgID("WScript.Shell"));
                                      dynamic shortcut = shell.CreateShortcut(shortcutPath);
                                      shortcut.TargetPath = targetPath;
                                      shortcut.Arguments = arguments ?? "";
                                      shortcut.WorkingDirectory = System.IO.Path.GetDirectoryName(targetPath);
                                      shortcut.Description = description ?? "Masterstrap";
                                      shortcut.IconLocation = string.IsNullOrWhiteSpace(iconPath) ? targetPath : iconPath;
                                      shortcut.Save();
                                      this.Log($"[Settings] ✓ Shortcut ready: {shortcutPath}");
                                  }
                                  else if (System.IO.File.Exists(shortcutPath))
                                  {
                                      System.IO.File.Delete(shortcutPath);
                                      this.Log($"[Settings] ✓ Shortcut removed: {shortcutPath}");
                                  }
                              }
                              catch (Exception ex)
                              {
                                  this.Log($"[Settings] ✗ Shortcut operation failed ({shortcutPath}): {ex.Message}");
                              }
                          }

                          private void DesktopShortcutToggle_Checked(object sender, RoutedEventArgs e)
                          {
                              try
                              {
                                  if (this._initializationComplete)
                                  {
                                      this.HasUnsavedChanges = true;
                                      this.Log("[Settings] ✓ Desktop Shortcut enabled");
                                  }
                              }
                              catch (Exception ex)
                              {
                                  this.Log($"[Settings] Error: {ex.Message}");
                              }
                          }

                          private void StartMenuShortcutToggle_Checked(object sender, RoutedEventArgs e)
                          {
                              if (this._initializationComplete)
                                  this.HasUnsavedChanges = true;
                          }

                          private void StartMenuShortcutToggle_Unchecked(object sender, RoutedEventArgs e)
                          {
                              if (this._initializationComplete)
                                  this.HasUnsavedChanges = true;
                          }

                          private void LaunchRobloxShortcutToggle_Checked(object sender, RoutedEventArgs e)
                          {
                              if (this._initializationComplete)
                                  this.HasUnsavedChanges = true;
                          }

                          private void LaunchRobloxShortcutToggle_Unchecked(object sender, RoutedEventArgs e)
                          {
                              if (this._initializationComplete)
                                  this.HasUnsavedChanges = true;
                          }

                          private void DesktopShortcutToggle_Unchecked(object sender, RoutedEventArgs e)
                          {
                              try
                              {
                                  if (this._initializationComplete)
                                  {
                                      this.HasUnsavedChanges = true;
                                      this.Log("[Settings] ✗ Desktop Shortcut disabled");
                                  }
                              }
                              catch (Exception ex)
                              {
                                  this.Log($"[Settings] Error: {ex.Message}");
                              }
                          }

                          private void PreserveRenderingQualityToggle_Checked(object sender, RoutedEventArgs e)
                          {
                              try
                              {
                                  if (this._initializationComplete)
                                  {
                                      this.HasUnsavedChanges = true;
                                      this.Log("[Settings] ✓ Preserve Rendering Quality enabled - marked as unsaved");
                                  }
                              }
                              catch (Exception ex)
                              {
                                  this.Log($"[Settings] Error: {ex.Message}");
                              }
                          }

                          private void PreserveRenderingQualityToggle_Unchecked(object sender, RoutedEventArgs e)
                          {
                              try
                              {
                                  if (this._initializationComplete)
                                  {
                                      this.HasUnsavedChanges = true;
                                      this.Log("[Settings] ✗ Preserve Rendering Quality disabled - marked as unsaved");
                                  }
                              }
                              catch (Exception ex)
                              {
                                  this.Log($"[Settings] Error: {ex.Message}");
                              }
                          }

                                  private static readonly string[] EmbeddedLanguageOptions = SortLanguageKeysByNativeDisplayOrder(
                                      new[]
                                      {
                                          "Brazil", "Chile", "Chinese", "Colombia", "Dutch", "English", "EnglishCanada", "Filipino", "French", "German", "Hebrew", "Indonesian",
                                          "Italy", "Japanese", "Khmer", "Korean", "Lao", "Malay", "Polish", "Portuguese", "Romanian", "Russian", "SouthAfrica", "Spain",
                                          "SpanishArgentina", "Swedish", "Taiwan", "Thai", "Turkiye", "Ukrainian", "UnitedArabEmirates", "Vietnamese"
                                      });

                                  private static string[] SortLanguageKeysByNativeDisplayOrder(string[] keys)
                                  {
                                      var copy = new string[keys.Length];
                                      Array.Copy(keys, copy, keys.Length);
                                      var cmp = CultureInfo.InvariantCulture.CompareInfo;
                                      Array.Sort(copy, (a, b) => cmp.Compare(
                                          GetNativeLanguageName(a),
                                          GetNativeLanguageName(b),
                                          CompareOptions.IgnoreCase));
                                      return copy;
                                  }

                                  private static string MapDisplayLanguageToCanonicalTag(string? language)
                                  {
                                      if (string.IsNullOrWhiteSpace(language)) return "English";
                                      string l = language.Trim();
                                      foreach (string key in EmbeddedLanguageOptions)
                                      {
                                          if (string.Equals(l, key, StringComparison.OrdinalIgnoreCase))
                                              return key;
                                          if (string.Equals(l, GetNativeLanguageName(key), StringComparison.OrdinalIgnoreCase))
                                              return key;
                                      }
                                      return "English";
                                  }

                                  private static string GetFlagForLanguage(string language)
                                  {
                                      if (string.IsNullOrWhiteSpace(language)) return "";
                                      var trimmed = language.Trim();
                                      if (string.Equals(trimmed, "Brazil", StringComparison.OrdinalIgnoreCase)) return "\uD83C\uDDE7\uD83C\uDDF7 ";
                                      if (string.Equals(trimmed, "Chile", StringComparison.OrdinalIgnoreCase)) return "\uD83C\uDDE8\uD83C\uDDF1 ";
                                      if (string.Equals(trimmed, "Chinese", StringComparison.OrdinalIgnoreCase)) return "\uD83C\uDDE8\uD83C\uDDF3 ";
                                      if (string.Equals(trimmed, "Colombia", StringComparison.OrdinalIgnoreCase)) return "\uD83C\uDDE8\uD83C\uDDF4 ";
                                      if (string.Equals(trimmed, "English", StringComparison.OrdinalIgnoreCase)) return "\uD83C\uDDFA\uD83C\uDDF8 ";
                                      if (string.Equals(trimmed, "EnglishCanada", StringComparison.OrdinalIgnoreCase)) return "\uD83C\uDDE8\uD83C\uDDE6 ";
                                      if (string.Equals(trimmed, "Filipino", StringComparison.OrdinalIgnoreCase)) return "\uD83C\uDDF5\uD83C\uDDED ";
                                      if (string.Equals(trimmed, "French", StringComparison.OrdinalIgnoreCase)) return "\uD83C\uDDEB\uD83C\uDDF7 ";
                                      if (string.Equals(trimmed, "Hebrew", StringComparison.OrdinalIgnoreCase)) return "\uD83C\uDDEE\uD83C\uDDF1 ";
                                      if (string.Equals(trimmed, "Indonesian", StringComparison.OrdinalIgnoreCase)) return "\uD83C\uDDEE\uD83C\uDDE9 ";
                                      if (string.Equals(trimmed, "Italy", StringComparison.OrdinalIgnoreCase)) return "\uD83C\uDDEE\uD83C\uDDF9 ";
                                      if (string.Equals(trimmed, "Japanese", StringComparison.OrdinalIgnoreCase)) return "\uD83C\uDDEF\uD83C\uDDF5 ";
                                      if (string.Equals(trimmed, "Korean", StringComparison.OrdinalIgnoreCase)) return "\uD83C\uDDF0\uD83C\uDDF7 ";
                                      if (string.Equals(trimmed, "Khmer", StringComparison.OrdinalIgnoreCase)) return "\uD83C\uDDF0\uD83C\uDDED ";
                                      if (string.Equals(trimmed, "Malay", StringComparison.OrdinalIgnoreCase)) return "\uD83C\uDDF2\uD83C\uDDFE ";
                                      if (string.Equals(trimmed, "Portuguese", StringComparison.OrdinalIgnoreCase)) return "\uD83C\uDDF5\uD83C\uDDF9 ";
                                      if (string.Equals(trimmed, "Russian", StringComparison.OrdinalIgnoreCase)) return "\uD83C\uDDF7\uD83C\uDDFA ";
                                      if (string.Equals(trimmed, "SouthAfrica", StringComparison.OrdinalIgnoreCase)) return "\uD83C\uDDFF\uD83C\uDDE6 ";
                                      if (string.Equals(trimmed, "Spain", StringComparison.OrdinalIgnoreCase)) return "\uD83C\uDDEA\uD83C\uDDF8 ";
                                      if (string.Equals(trimmed, "SpanishArgentina", StringComparison.OrdinalIgnoreCase)) return "\uD83C\uDDE6\uD83C\uDDF7 ";
                                      if (string.Equals(trimmed, "Thai", StringComparison.OrdinalIgnoreCase)) return "\uD83C\uDDF9\uD83C\uDDED ";
                                      if (string.Equals(trimmed, "Taiwan", StringComparison.OrdinalIgnoreCase)) return "\uD83C\uDDF9\uD83C\uDDFC ";
                                      if (string.Equals(trimmed, "Turkiye", StringComparison.OrdinalIgnoreCase)) return "\uD83C\uDDF9\uD83C\uDDF7 ";
                                      if (string.Equals(trimmed, "Ukrainian", StringComparison.OrdinalIgnoreCase)) return "\uD83C\uDDFA\uD83C\uDDE6 ";
                                      if (string.Equals(trimmed, "UnitedArabEmirates", StringComparison.OrdinalIgnoreCase)) return "\uD83C\uDDE6\uD83C\uDDEA ";
                                      if (string.Equals(trimmed, "Vietnamese", StringComparison.OrdinalIgnoreCase)) return "\uD83C\uDDFB\uD83C\uDDF3 ";
                                      if (string.Equals(trimmed, "Lao", StringComparison.OrdinalIgnoreCase)) return "\uD83C\uDDF1\uD83C\uDDE6 ";
                                      if (string.Equals(trimmed, "German", StringComparison.OrdinalIgnoreCase)) return "\uD83C\uDDE9\uD83C\uDDEA ";
                                      if (string.Equals(trimmed, "Romanian", StringComparison.OrdinalIgnoreCase)) return "\uD83C\uDDF7\uD83C\uDDF4 ";
                                      if (string.Equals(trimmed, "Swedish", StringComparison.OrdinalIgnoreCase)) return "\uD83C\uDDF8\uD83C\uDDEA ";
                                      if (string.Equals(trimmed, "Dutch", StringComparison.OrdinalIgnoreCase)) return "\uD83C\uDDF3\uD83C\uDDF1 ";
                                      if (string.Equals(trimmed, "Polish", StringComparison.OrdinalIgnoreCase)) return "\uD83C\uDDF5\uD83C\uDDF1 ";
                                      return "";
                                  }

                                  private static string GetNativeLanguageName(string language)
                                  {
                                      if (string.IsNullOrWhiteSpace(language)) return language ?? "";
                                      var trimmed = language.Trim();
                                      if (string.Equals(trimmed, "Brazil", StringComparison.OrdinalIgnoreCase)) return "Portugues (Brazil)";
                                      if (string.Equals(trimmed, "Chile", StringComparison.OrdinalIgnoreCase)) return "Spanish (Chile)";
                                      if (string.Equals(trimmed, "Chinese", StringComparison.OrdinalIgnoreCase)) return "Chinese (Simplified)";
                                      if (string.Equals(trimmed, "Colombia", StringComparison.OrdinalIgnoreCase)) return "Spanish (Colombia)";
                                      if (string.Equals(trimmed, "English", StringComparison.OrdinalIgnoreCase)) return "English";
                                      if (string.Equals(trimmed, "EnglishCanada", StringComparison.OrdinalIgnoreCase)) return "English (Canada)";
                                      if (string.Equals(trimmed, "Filipino", StringComparison.OrdinalIgnoreCase)) return "Filipino";
                                      if (string.Equals(trimmed, "French", StringComparison.OrdinalIgnoreCase)) return "French";
                                      if (string.Equals(trimmed, "Hebrew", StringComparison.OrdinalIgnoreCase)) return "Hebrew";
                                      if (string.Equals(trimmed, "Indonesian", StringComparison.OrdinalIgnoreCase)) return "Bahasa Indonesia";
                                      if (string.Equals(trimmed, "Italy", StringComparison.OrdinalIgnoreCase)) return "Italian";
                                      if (string.Equals(trimmed, "Japanese", StringComparison.OrdinalIgnoreCase)) return "Japanese";
                                      if (string.Equals(trimmed, "Korean", StringComparison.OrdinalIgnoreCase)) return "Korean";
                                      if (string.Equals(trimmed, "Khmer", StringComparison.OrdinalIgnoreCase)) return "Khmer";
                                      if (string.Equals(trimmed, "Lao", StringComparison.OrdinalIgnoreCase)) return "Lao";
                                      if (string.Equals(trimmed, "Malay", StringComparison.OrdinalIgnoreCase)) return "Bahasa Melayu";
                                      if (string.Equals(trimmed, "Portuguese", StringComparison.OrdinalIgnoreCase)) return "Portugues";
                                      if (string.Equals(trimmed, "Russian", StringComparison.OrdinalIgnoreCase)) return "Russian";
                                      if (string.Equals(trimmed, "SouthAfrica", StringComparison.OrdinalIgnoreCase)) return "English (South Africa)";
                                      if (string.Equals(trimmed, "Spain", StringComparison.OrdinalIgnoreCase)) return "Spanish (Spain)";
                                      if (string.Equals(trimmed, "SpanishArgentina", StringComparison.OrdinalIgnoreCase)) return "Spanish (Latin America)";
                                      if (string.Equals(trimmed, "Thai", StringComparison.OrdinalIgnoreCase)) return "Thai";
                                      if (string.Equals(trimmed, "Taiwan", StringComparison.OrdinalIgnoreCase)) return "Chinese (Traditional Taiwan)";
                                      if (string.Equals(trimmed, "Turkiye", StringComparison.OrdinalIgnoreCase)) return "Turkish";
                                      if (string.Equals(trimmed, "Ukrainian", StringComparison.OrdinalIgnoreCase)) return "Ukrainian";
                                      if (string.Equals(trimmed, "UnitedArabEmirates", StringComparison.OrdinalIgnoreCase)) return "Arabic (UAE)";
                                      if (string.Equals(trimmed, "Vietnamese", StringComparison.OrdinalIgnoreCase)) return "Vietnamese";
                                      if (string.Equals(trimmed, "German", StringComparison.OrdinalIgnoreCase)) return "German";
                                      if (string.Equals(trimmed, "Romanian", StringComparison.OrdinalIgnoreCase)) return "Romanian";
                                      if (string.Equals(trimmed, "Swedish", StringComparison.OrdinalIgnoreCase)) return "Swedish";
                                      if (string.Equals(trimmed, "Dutch", StringComparison.OrdinalIgnoreCase)) return "Dutch";
                                      if (string.Equals(trimmed, "Polish", StringComparison.OrdinalIgnoreCase)) return "Polish";
                                      return trimmed;
                                  }

                                  private void EnsureLanguageComboEmbedded()
                                  {
                                      if (this.LanguageCombo == null) return;
                                      try
                                      {
                                          this._isApplyingLanguage = true;
                                          try
                                          {
                                              if (this.LanguageCombo.Items.Count == 0)
                                              {
                                                  var itemStyle = this.TryFindResource("GlassmorphicComboBoxItemStyle") as System.Windows.Style;
                                                  foreach (string lang in EmbeddedLanguageOptions)
                                                  {
                                                      string displayName = GetNativeLanguageName(lang);
                                                      string displayText = GetFlagForLanguage(lang) + displayName;
                                                      var item = new ComboBoxItem { Content = displayText, Tag = lang };
                                                      if (itemStyle != null) item.Style = itemStyle;
                                                      this.LanguageCombo.Items.Add(item);
                                                  }
                                              }
                                              else
                                              {
                                                  for (int i = 0; i < this.LanguageCombo.Items.Count && i < EmbeddedLanguageOptions.Length; i++)
                                                  {
                                                      if (this.LanguageCombo.Items[i] is ComboBoxItem cbi && cbi.Tag is string lang)
                                                      {
                                                          string displayName = GetNativeLanguageName(lang);
                                                          cbi.Content = GetFlagForLanguage(lang) + displayName;
                                                      }
                                                  }
                                              }
                                              int idx = this.GetDisplayLanguageIndex(this._currentDisplayLanguage);
                                              if (idx >= 0 && idx < this.LanguageCombo.Items.Count)
                                                  this.LanguageCombo.SelectedIndex = idx;
                                          }
                                          finally { this._isApplyingLanguage = false; }
                                      }
                                      catch (Exception ex) { this.Log($"[Language] ✗ Error ensuring language combobox: {ex.Message}"); }
                                  }

                                  private void SettingsLanguageCard_Loaded(object sender, RoutedEventArgs e)
                                  {
                                      this.Dispatcher.BeginInvoke(new Action(() =>
                                      {
                                          try
                                          {
                                              this.EnsureLanguageComboEmbedded();
                                              this.EnsureUiThemeComboEmbedded();
                                              this.EnsureEffectThemeComboEmbedded();
                                          }
                                          catch (Exception ex)
                                          {
                                              this.Log($"[Settings] Language card deferred init: {ex.Message}");
                                          }
                                      }), DispatcherPriority.Loaded);
                                  }

                                  private void EnsureUiThemeComboEmbedded()
                                  {
                                      if (this.UiThemeCombo == null) return;
                                      try
                                      {
                                          this._isApplyingGlobalTheme = true;
                                          try
                                          {
                                              var itemStyle = this.TryFindResource("GlassmorphicComboBoxItemStyle") as System.Windows.Style;
                                              this.UiThemeCombo.Items.Clear();
                                              var lightItem = new ComboBoxItem { Content = "Light", Tag = "White" };
                                              var darkItem = new ComboBoxItem { Content = "Dark", Tag = "Black" };
                                              if (itemStyle != null)
                                              {
                                                  lightItem.Style = itemStyle;
                                                  darkItem.Style = itemStyle;
                                              }
                                              this.UiThemeCombo.Items.Add(lightItem);
                                              this.UiThemeCombo.Items.Add(darkItem);
                                              int idx = string.Equals(this._currentUiTheme, "White", StringComparison.OrdinalIgnoreCase) ? 0 : 1;
                                              if (idx >= 0 && idx < this.UiThemeCombo.Items.Count)
                                                  this.UiThemeCombo.SelectedIndex = idx;
                                          }
                                          finally { this._isApplyingGlobalTheme = false; }
                                      }
                                      catch (Exception ex)
                                      {
                                          this.Log($"[Theme] ✗ Error ensuring UI theme combobox: {ex.Message}");
                                      }
                                  }

                                  private void EnsureEffectThemeComboEmbedded()
                                  {
                                      if (this.EffectThemeCombo == null) return;
                                      try
                                      {
                                          this._isApplyingGlobalTheme = true;
                                          try
                                          {
                                              if (this.EffectThemeCombo.Items.Count == 0)
                                              {
                                                  var itemStyle = this.TryFindResource("GlassmorphicComboBoxItemStyle") as System.Windows.Style;
                                                  var defaultItem = new ComboBoxItem { Content = "Default", Tag = "Default" };
                                                  var glassItem = new ComboBoxItem { Content = "glassmorphic", Tag = "Glassmorphic" };
                                                  var glassBlurItem = new ComboBoxItem { Content = "glassmorphic + blur", Tag = "GlassmorphicBlur" };
                                                  if (itemStyle != null)
                                                  {
                                                      defaultItem.Style = itemStyle;
                                                      glassItem.Style = itemStyle;
                                                      glassBlurItem.Style = itemStyle;
                                                  }
                                                  this.EffectThemeCombo.Items.Add(defaultItem);
                                                  this.EffectThemeCombo.Items.Add(glassItem);
                                                  this.EffectThemeCombo.Items.Add(glassBlurItem);
                                              }

                                              int idx = 0;
                                              if (string.Equals(this._currentGlobalTheme, "Glassmorphic", StringComparison.OrdinalIgnoreCase)) idx = 1;
                                              else if (string.Equals(this._currentGlobalTheme, "GlassmorphicBlur", StringComparison.OrdinalIgnoreCase)) idx = 2;
                                              if (idx >= 0 && idx < this.EffectThemeCombo.Items.Count)
                                                  this.EffectThemeCombo.SelectedIndex = idx;
                                          }
                                          finally { this._isApplyingGlobalTheme = false; }
                                      }
                                      catch (Exception ex)
                                      {
                                          this.Log($"[Theme] ✗ Error ensuring effect theme combobox: {ex.Message}");
                                      }
                                  }

                                  private string GetSelectedUiTheme()
                                  {
                                      if (this.UiThemeCombo?.SelectedItem is ComboBoxItem item)
                                      {
                                          if (item.Tag is string tag && !string.IsNullOrWhiteSpace(tag))
                                          {
                                              if (string.Equals(tag, "White", StringComparison.OrdinalIgnoreCase))
                                                  return "White";
                                              if (string.Equals(tag, "Black", StringComparison.OrdinalIgnoreCase))
                                                  return "Black";
                                          }
                                          string text = item.Content?.ToString()?.Trim() ?? string.Empty;
                                          if (string.Equals(text, "Light", StringComparison.OrdinalIgnoreCase) || string.Equals(text, "White", StringComparison.OrdinalIgnoreCase))
                                              return "White";
                                          if (string.Equals(text, "Dark", StringComparison.OrdinalIgnoreCase) || string.Equals(text, "Black", StringComparison.OrdinalIgnoreCase))
                                              return "Black";
                                      }
                                      return string.Equals(this._currentUiTheme, "White", StringComparison.OrdinalIgnoreCase) ? "White" : "Black";
                                  }

                                  private string GetSelectedEffectTheme()
                                  {
                                      if (this.EffectThemeCombo?.SelectedItem is ComboBoxItem item)
                                      {
                                          if (item.Tag is string tag && !string.IsNullOrWhiteSpace(tag))
                                              return tag;
                                          string text = item.Content?.ToString()?.Trim() ?? "Default";
                                          if (string.Equals(text, "glassmorphic + blur", StringComparison.OrdinalIgnoreCase))
                                              return "GlassmorphicBlur";
                                          if (string.Equals(text, "glassmorphic", StringComparison.OrdinalIgnoreCase))
                                              return "Glassmorphic";
                                      }
                                      return this._currentGlobalTheme ?? "Default";
                                  }

                                  private void RestoreGlobalThemeSettings()
                                  {
                                      try
                                      {
                                          this._currentGlobalTheme = this._settingsManager.GetGlobalTheme();
                                          this._currentUiTheme = this._settingsManager.GetUiTheme();
                                          this.EnsureUiThemeComboEmbedded();
                                          this.EnsureEffectThemeComboEmbedded();
                                          this.ApplyGlobalThemeToUi(this._currentGlobalTheme, this._currentUiTheme);
                                      }
                                      catch (Exception ex)
                                      {
                                          this.Log($"[Theme] ✗ Error restoring theme: {ex.Message}");
                                      }
                                  }

                                  private void SetWindowResourceSolidBrushOpacity(string key, double opacity)
                                  {
                                      if (!(this.TryFindResource(key) is SolidColorBrush scb))
                                          return;
                                      if (scb.IsFrozen)
                                      {
                                          SolidColorBrush clone = scb.Clone();
                                          clone.Opacity = opacity;
                                          this.Resources[key] = clone;
                                      }
                                      else
                                      {
                                          scb.Opacity = opacity;
                                      }
                                  }

                                  private void SetWindowResourceLinearGradientOpacity(string key, double opacity)
                                  {
                                      if (!(this.TryFindResource(key) is LinearGradientBrush lg))
                                          return;
                                      if (lg.IsFrozen)
                                      {
                                          LinearGradientBrush clone = lg.Clone();
                                          clone.Opacity = opacity;
                                          this.Resources[key] = clone;
                                      }
                                      else
                                      {
                                          lg.Opacity = opacity;
                                      }
                                  }

                                  private void ApplyGlobalThemeToUi(string effectTheme, string uiTheme)
                                  {
                                      bool isWhite = false;
                                      bool glassOnly = string.Equals(effectTheme, "Glassmorphic", StringComparison.OrdinalIgnoreCase);
                                      bool glassWithBlur = string.Equals(effectTheme, "GlassmorphicBlur", StringComparison.OrdinalIgnoreCase);
                                      bool glass = glassOnly || glassWithBlur;
                                      bool blur = glassWithBlur;
                                      this._currentGlobalTheme = glassWithBlur ? "GlassmorphicBlur" : (glassOnly ? "Glassmorphic" : "Default");
                                      this._currentUiTheme = isWhite ? "White" : "Black";

                                      this.ApplyUiThemeResourcePalette(isWhite);
                                      this.ApplyCustomBackgroundForTheme(glass, blur);

                                      Color shellGray = Color.FromRgb(0x1C, 0x1C, 0x1C);
                                      Color shellGrayBackdrop = Color.FromRgb(0x2E, 0x2E, 0x2E);
                                      Color winBg = isWhite ? Color.FromRgb(0xF9, 0xF9, 0xF9) : shellGrayBackdrop;
                                      Color mainInner = isWhite ? Color.FromRgb(0xFF, 0xFF, 0xFF) : shellGrayBackdrop;
                                      Color titleBar = isWhite ? Color.FromRgb(0xF9, 0xF9, 0xF9) : shellGray;
                                      Color rightPanel = isWhite ? Color.FromRgb(0xFF, 0xFF, 0xFF) : shellGray;
                                      Color footer = isWhite ? Color.FromRgb(0xF9, 0xF9, 0xF9) : shellGray;

                                      this.Background = glass
                                          ? Brushes.Transparent
                                          : new SolidColorBrush(winBg);

                                      this.SetWindowResourceSolidBrushOpacity("GlassPanelBrush", glass ? 0.45 : 1.0);
                                      this.SetWindowResourceSolidBrushOpacity("GlassButtonBrush", glass ? 0.81 : 1.0);
                                      this.SetWindowResourceSolidBrushOpacity("BgPanel", glass ? 0.85 : 1.0);
                                      this.SetWindowResourceSolidBrushOpacity("BgSidebar", glass ? 0.90 : 1.0);
                                      this.SetWindowResourceSolidBrushOpacity("SidebarSurfaceBrush", glass ? 0.40 : 1.0);
                                      this.SetWindowResourceSolidBrushOpacity("NavRailBgBrush", glass ? 0.40 : 1.0);
                                      this.SetWindowResourceSolidBrushOpacity("GameCardGlassBorderBrush", glass ? 0.85 : 1.0);
                                      this.SetWindowResourceSolidBrushOpacity("AllGameCardGlassBorderBrush", glass ? 0.9 : 1.0);
                                      this.SetWindowResourceSolidBrushOpacity("TabContentSurfaceBrush", glass ? 0.86 : 1.0);

                                      this.SetWindowResourceLinearGradientOpacity("GameCardGlassOverlayBrush", glass ? 0.70 : 0.0);
                                      this.SetWindowResourceLinearGradientOpacity("GameCardDarkBaseBrush", glass ? 0.55 : 1.0);
                                      this.SetWindowResourceLinearGradientOpacity("AllGameCardBaseBrush", glass ? 0.55 : 1.0);
                                      this.SetWindowResourceLinearGradientOpacity("AllGameCardGlassHighlightBrush", glass ? 1.0 : 0.0);

                                      if (this.MainGlassBorder != null)
                                      {
                                          this.MainGlassBorder.Margin = glass ? new Thickness(2) : new Thickness(0);
                                          this.MainGlassBorder.Background = new SolidColorBrush(titleBar) { Opacity = glass ? 0.52 : 1.0 };
                                      }

                                      if (this.TitleBarGlassBorder != null)
                                          this.TitleBarGlassBorder.Background = Brushes.Transparent;

                                      if (this.RightContentGlassBorder != null)
                                          this.RightContentGlassBorder.Background = Brushes.Transparent;

                                      if (this.FooterGlassBorder != null)
                                          this.FooterGlassBorder.Background = Brushes.Transparent;

                                      if (this.LeftSidebarPanelBorder != null)
                                          this.LeftSidebarPanelBorder.Background = Brushes.Transparent;

                                      try
                                      {
                                          Helpers.AcrylicHelper.UseBlurBehind = blur;
                                          Helpers.AcrylicHelper.ApplyAcrylicEffect(this);
                                      }
                                      catch { }

                                      this.RefreshVersionColorsAfterThemeChange();

                                      try
                                      {
                                          this.ApplyFlagsAreaGlassChrome(glass, blur);
                                          int si = this.MainTabControl?.SelectedIndex ?? 0;
                                          int nav = this.ViewToNavTabIndex(si);
                                          this.UpdateTabBorderHighlight(nav == 3 ? 1 : nav);
                                      }
                                      catch
                                      {
                                      }

                                      this.Dispatcher.BeginInvoke(new Action(() =>
                                      {
                                          try
                                          {
                                              this.ApplyCustomBackgroundForTheme(glass, blur);
                                              this.ApplyFlagsAreaGlassChrome(glass, blur);
                                              int si = this.MainTabControl?.SelectedIndex ?? 0;
                                              int nav = this.ViewToNavTabIndex(si);
                                              this.UpdateTabBorderHighlight(nav == 3 ? 1 : nav);
                                          }
                                          catch
                                          {
                                          }
                                      }), DispatcherPriority.Render);
                                  }

                                  private void ApplyCustomBackgroundForTheme(bool glassMode, bool blurMode)
                                  {
                                      if (this.UserCustomBackgroundImage == null)
                                          return;

                                      bool showImage = glassMode
                                                       && !string.IsNullOrWhiteSpace(this._customBackgroundImagePath)
                                                       && File.Exists(this._customBackgroundImagePath);

                                      if (!showImage)
                                      {
                                          this.UserCustomBackgroundImage.Source = null;
                                          this.UserCustomBackgroundImage.Visibility = Visibility.Collapsed;
                                          return;
                                      }

                                      try
                                      {
                                          BitmapImage bitmap = new BitmapImage();
                                          bitmap.BeginInit();
                                          bitmap.CacheOption = BitmapCacheOption.OnLoad;
                                          bitmap.UriSource = new Uri(this._customBackgroundImagePath, UriKind.Absolute);
                                          bitmap.EndInit();
                                          bitmap.Freeze();

                                          this.UserCustomBackgroundImage.Source = bitmap;
                                          this.UserCustomBackgroundImage.Effect = blurMode
                                              ? new BlurEffect { Radius = 18 }
                                              : null;
                                          this.UserCustomBackgroundImage.Visibility = Visibility.Visible;
                                      }
                                      catch
                                      {
                                          this.UserCustomBackgroundImage.Source = null;
                                          this.UserCustomBackgroundImage.Effect = null;
                                          this.UserCustomBackgroundImage.Visibility = Visibility.Collapsed;
                                      }
                                  }

                                  private static List<T> FindVisualChildren<T>(DependencyObject root) where T : DependencyObject
                                  {
                                      var results = new List<T>();
                                      if (root == null)
                                          return results;
                                      var stack = new Stack<DependencyObject>();
                                      stack.Push(root);
                                      while (stack.Count > 0)
                                      {
                                          DependencyObject current = stack.Pop();
                                          int n = VisualTreeHelper.GetChildrenCount(current);
                                          for (int i = n - 1; i >= 0; i--)
                                          {
                                              var child = VisualTreeHelper.GetChild(current, i);
                                              if (child is T match)
                                                  results.Add(match);
                                              stack.Push(child);
                                          }
                                      }
                                      return results;
                                  }

                                  private static bool IsUnderMainTabControlSubtree(DependencyObject leaf, TabControl mainTc)
                                  {
                                      if (mainTc == null || leaf == null)
                                          return false;
                                      for (DependencyObject d = leaf; d != null; d = VisualTreeHelper.GetParent(d))
                                      {
                                          if (ReferenceEquals(d, mainTc))
                                              return true;
                                      }
                                      return false;
                                  }

                                  private TabItem FindOwningMainTabItem(DependencyObject leaf)
                                  {
                                      if (this.MainTabControl == null)
                                          return null;
                                      for (DependencyObject d = leaf; d != null; d = VisualTreeHelper.GetParent(d))
                                      {
                                          if (d is TabItem ti)
                                          {
                                              DependencyObject p = VisualTreeHelper.GetParent(ti);
                                              if (p is TabControl tc && ReferenceEquals(tc, this.MainTabControl))
                                                  return ti;
                                          }
                                      }

                                      if (IsUnderMainTabControlSubtree(leaf, this.MainTabControl))
                                          return this.MainTabControl.SelectedItem as TabItem;
                                      return null;
                                  }

                                  private static bool IsMainTabIndexEligibleForContentCardGlassmorphic(int index) =>
                                      index == 1 || index == 2 || index == 4 || index == 6;

                                  private static void ApplyFlatGlassCardFace(Border face, bool enable)
                                  {
                                      if (face == null)
                                          return;
                                      if (!enable)
                                      {
                                          face.Opacity = 1.0;
                                          face.ClearValue(Border.BorderBrushProperty);
                                          face.ClearValue(Border.BorderThicknessProperty);
                                          return;
                                      }

                                      face.Opacity = 0.86;
                                      face.BorderBrush = new SolidColorBrush(Color.FromArgb(0x52, 0xF0, 0xF4, 0xFF));
                                      face.BorderThickness = new Thickness(1);
                                  }

                                  private void ApplyFastFlagEditorLaunchCardChrome(bool useCardsGlass)
                                  {
                                      if (this.OpenFastFlagEditorBtn == null || this.MainTabControl == null)
                                          return;
                                      TabItem host = this.FindOwningMainTabItem(this.OpenFastFlagEditorBtn);
                                      int idx = host != null ? this.MainTabControl.Items.IndexOf(host) : -1;
                                      bool want = useCardsGlass && idx == 1;
                                      this.OpenFastFlagEditorBtn.ApplyTemplate();
                                      if (this.OpenFastFlagEditorBtn.Template?.FindName("ContentBorder", this.OpenFastFlagEditorBtn) is Border face)
                                          ApplyFlatGlassCardFace(face, want);
                                  }

                                  private void ApplyTabPageGlassCardPanels(bool glass, bool blur)
                                  {
                                      _ = blur;
                                      bool useCardsGlass = glass;
                                      if (this.MainTabControl == null)
                                          return;

                                      const int fastFlagEditorTabIndex = 3;
                                      int count = this.MainTabControl.Items.Count;
                                      for (int tabIdx = 0; tabIdx < count; tabIdx++)
                                      {
                                          if (tabIdx == fastFlagEditorTabIndex)
                                              continue;

                                          if (this.MainTabControl.Items[tabIdx] is not TabItem ti)
                                              continue;
                                          if (ti.Content is not DependencyObject contentRoot)
                                              continue;

                                          bool eligibleTab = IsMainTabIndexEligibleForContentCardGlassmorphic(tabIdx);
                                          foreach (GlassCardPanel panel in FindVisualChildren<GlassCardPanel>(contentRoot))
                                              panel.ApplyTabCardGlassmorphic(useCardsGlass && eligibleTab);
                                      }

                                      this.ApplyFastFlagEditorLaunchCardChrome(useCardsGlass);
                                  }

                                  private void ApplyFlagsAreaGlassChrome(bool glass, bool blur)
                                  {
                                      if (this.LeftSidebarPanelBorder != null)
                                      {
                                          this.LeftSidebarPanelBorder.BorderThickness = new Thickness(0);
                                          this.LeftSidebarPanelBorder.BorderBrush = null;
                                      }

                                      if (this.RightContentGlassBorder != null)
                                      {
                                          this.RightContentGlassBorder.BorderThickness = new Thickness(0);
                                          this.RightContentGlassBorder.BorderBrush = null;
                                      }

                                      if (this.ActivityLogPanelBorder != null)
                                      {
                                          this.ActivityLogPanelBorder.BorderThickness = new Thickness(0);
                                          this.ActivityLogPanelBorder.BorderBrush = null;
                                      }

                                      bool flagsGlassmorphic = glass && !blur;

                                      void ApplyFlagsButtonChrome(Button btn)
                                      {
                                          if (btn == null)
                                              return;
                                          btn.ApplyTemplate();
                                          if (btn.Template?.FindName("FaceBd", btn) is not Border face)
                                              return;
                                          var blurBg = btn.Template.FindName("BlurBg", btn) as Border;
                                          var glowBd = btn.Template.FindName("GlowBd", btn) as Border;

                                          if (blurBg != null)
                                              blurBg.Visibility = Visibility.Collapsed;
                                          if (glowBd != null)
                                              glowBd.Visibility = Visibility.Collapsed;
                                          if (blurBg?.Effect is BlurEffect beOff)
                                              beOff.Radius = 0;

                                          face.Opacity = 1.0;
                                      }

                                      ApplyFlagsButtonChrome(this.LoadFlagsBtn);
                                      ApplyFlagsButtonChrome(this.LoadCacheBtn);
                                      ApplyFlagsButtonChrome(this.ApplyFlagsBtn);
                                      ApplyFlagsButtonChrome(this.RestoreFlagsBtn);

                                      if (this.ActivityLogGlassFace != null)
                                          this.ActivityLogGlassFace.Opacity = glass ? 0.9 : 1.0;

                                      this.ApplyTabPageGlassCardPanels(glass, blur);
                                  }

                                  private void ApplyUiThemeResourcePalette(bool isWhite)
                                  {
                                      void PutSolid(string key, Color c, double opacity)
                                      {
                                          if (!(this.TryFindResource(key) is SolidColorBrush scb))
                                              return;
                                          SolidColorBrush brush = scb.IsFrozen ? scb.Clone() : scb;
                                          brush.Color = c;
                                          brush.Opacity = opacity;
                                          if (ReferenceEquals(brush, scb))
                                              return;
                                          this.Resources[key] = brush;
                                      }

                                      if (isWhite)
                                      {
                                          PutSolid("TextMain", Color.FromRgb(0x0A, 0x0A, 0x0A), 1);
                                          PutSolid("TextMuted", Color.FromRgb(0x3D, 0x3D, 0x3D), 1);
                                          PutSolid("TextBody", Color.FromRgb(0x1A, 0x1A, 0x1A), 1);
                                          PutSolid("TitleBarVersionBrush", Color.FromRgb(0x6E, 0x6E, 0x6E), 1);
                                          PutSolid("TitleBarSubtextBrush", Color.FromRgb(0x15, 0x4A, 0x9E), 1);
                                          PutSolid("BorderColor", Color.FromRgb(0xC5, 0xC5, 0xC5), 1);
                                          PutSolid("PanelFrameBorderBrush", Color.FromRgb(0x00, 0x00, 0x00), 1);
                                          PutSolid("GlassPanelBrush", Color.FromRgb(0xFF, 0xFF, 0xFF), 1);
                                          PutSolid("SidebarSurfaceBrush", Color.FromRgb(0xF9, 0xF9, 0xF9), 1);
                                          PutSolid("NavRailBgBrush", Color.FromRgb(0xF9, 0xF9, 0xF9), 1);
                                          PutSolid("NavAccentBarBrush", Color.FromRgb(0x00, 0x78, 0xD4), 1);
                                          PutSolid("NavSectionLabelBrush", Color.FromRgb(0x6E, 0x6E, 0x6E), 1);
                                          PutSolid("NavItemActiveFgBrush", Color.FromRgb(0x0A, 0x0A, 0x0A), 1);
                                          PutSolid("NavItemInactiveFgBrush", Color.FromRgb(0x6E, 0x6E, 0x6E), 1);
                                          PutSolid("ContentWellBgBrush", Color.FromRgb(0xF5, 0xF5, 0xF5), 1);
                                          PutSolid("TabContentSurfaceBrush", Color.FromRgb(0xF5, 0xF5, 0xF5), 1);
                                          PutSolid("FastFlagSectionBgBrush", Color.FromRgb(0xE4, 0xE6, 0xEA), 1);
                                          PutSolid("InteractiveButtonBgBrush", Color.FromRgb(0xE8, 0xE8, 0xE8), 1);
                                          PutSolid("InteractiveButtonHoverBrush", Color.FromRgb(0xDD, 0xDD, 0xDD), 1);
                                          PutSolid("InteractiveButtonPressedBrush", Color.FromRgb(0xD0, 0xD0, 0xD0), 1);
                                          PutSolid("InteractiveButtonBorderBrush", Color.FromRgb(0xC8, 0xC8, 0xC8), 1);
                                          PutSolid("InteractivePrimaryBgBrush", Color.FromRgb(0xD8, 0xD8, 0xD8), 1);
                                          PutSolid("InteractivePrimaryHoverBrush", Color.FromRgb(0xCC, 0xCC, 0xCC), 1);
                                          PutSolid("NavButtonHoverBgBrush", Color.FromRgb(0x00, 0x00, 0x00), 0.06);
                                          PutSolid("SidebarInfoPanelSolidBrush", Color.FromRgb(0xFF, 0xFF, 0xFF), 1);
                                          PutSolid("InfoSysCyanBrush", Color.FromRgb(0x00, 0x6E, 0xB8), 1);
                                          PutSolid("InfoSysGreenBrush", Color.FromRgb(0x1E, 0x7A, 0x3A), 1);
                                          PutSolid("InfoSysLabelBrush", Color.FromRgb(0x55, 0x55, 0x55), 1);
                                          PutSolid("InfoSysTimestampBrush", Color.FromRgb(0x76, 0x76, 0x76), 1);
                                          PutSolid("TabNavItemBgBrush", Color.FromRgb(0xF3, 0xF3, 0xF3), 1);
                                          PutSolid("TabNavRailBorderBrush", Color.FromRgb(0xA8, 0xA8, 0xA8), 1);
                                          PutSolid("TabNavRailBorderBrushActive", Color.FromRgb(0x74, 0x74, 0x74), 1);
                                          PutSolid("TabNavRailHoverBgBrush", Color.FromRgb(0xE8, 0xE8, 0xE8), 1);
                                          PutSolid("GlassButtonBrush", Color.FromRgb(0xF0, 0xF0, 0xF0), 1);
                                          PutSolid("BgMain", Color.FromRgb(0xFE, 0xFE, 0xFE), 1);
                                          PutSolid("BgPanel", Color.FromRgb(0xFF, 0xFF, 0xFF), 1);
                                          PutSolid("BgSidebar", Color.FromRgb(0xF9, 0xF9, 0xF9), 1);
                                          PutSolid("GameCardGlassBorderBrush", Color.FromRgb(0xAE, 0xAE, 0xAE), 0.95);
                                          PutSolid("AllGameCardGlassBorderBrush", Color.FromRgb(0xA0, 0xA0, 0xA0), 0.95);
                                          PutSolid("ComboFaceBrush", Color.FromRgb(0xFF, 0xFF, 0xFF), 1);
                                          PutSolid("ComboPopupBrush", Color.FromRgb(0xFF, 0xFF, 0xFF), 1);
                                          PutSolid("ComboFaceHoverBrush", Color.FromRgb(0xE8, 0xF1, 0xFF), 1);
                                          PutSolid("ComboItemHoverBrush", Color.FromRgb(0xE0, 0xE8, 0xF8), 1);
                                          PutSolid("ComboShadowBrush", Color.FromRgb(0xD9, 0xD9, 0xD9), 1);
                                          PutSolid("InputFieldBgBrush", Color.FromRgb(0xFF, 0xFF, 0xFF), 1);
                                          PutSolid("AppearanceEyeButtonBgBrush", Color.FromRgb(0xF3, 0xF4, 0xF6), 1);
                                          PutSolid("AppearanceEyeButtonBorderBrush", Color.FromRgb(0x4B, 0x55, 0x63), 1);
                                          PutSolid("AppearanceEyeButtonHoverBgBrush", Color.FromRgb(0xE5, 0xE7, 0xEB), 1);
                                          PutSolid("AppearanceEyeButtonHoverBorderBrush", Color.FromRgb(0x25, 0x63, 0xEB), 1);
                                          PutSolid("AppearanceEyeButtonPressedBgBrush", Color.FromRgb(0xD1, 0xD5, 0xDB), 1);
                                          PutSolid("AppearanceEyeIconBrush", Color.FromRgb(0x11, 0x18, 0x27), 1);
                                          PutSolid("ActivityLogHoverBgBrush", Color.FromRgb(0xE8, 0xF1, 0xFF), 1);
                                          PutSolid("ActivityLogSelectedBgBrush", Color.FromRgb(0xD9, 0xE8, 0xF5), 1);
                                          PutSolid("UiAccentHeading", Color.FromRgb(0x1A, 0x37, 0x7E), 1);
                                          PutSolid("UiAccentLabel", Color.FromRgb(0x1E, 0x40, 0x8C), 1);
                                          PutSolid("InfoPanelBorderBrush", Color.FromRgb(0x2E, 0x5C, 0xAE), 0.6);
                                          PutSolid("BrightBlue", Color.FromRgb(0x00, 0x47, 0xAB), 1);
                                          PutSolid("Success", Color.FromRgb(0x26, 0xA2, 0x4E), 1);
                                          PutSolid("FaqSubTabHoverBgBrush", Color.FromRgb(0xE8, 0xF2, 0xFC), 1);
                                          PutSolid("FaqCreditsBannerGlassBrush", Color.FromRgb(0xFF, 0xFF, 0xFF), 0.72);
                                          PutSolid("FaqCreditsBannerBorderBrush", Color.FromRgb(0xB8, 0xC5, 0xD6), 0.88);
                                          PutSolid("FaqCoreFlowBadgeBgBrush", Color.FromRgb(0x2A, 0x58, 0x8A), 1);
                                          PutSolid("FaqCoreFlowBadgeFgBrush", Color.FromRgb(0xFF, 0xFF, 0xFF), 1);
                                          PutSolid("PanelCardGlassBlurTintBrush", Color.FromRgb(0xC4, 0xD0, 0xE0), 0.52);
                                          PutSolid("DonorTierTitleFgBrush", Color.FromRgb(0x0A, 0x0A, 0x0A), 1);
                                          PutSolid("DonorColumnDividerBrush", Color.FromRgb(0xB0, 0xB6, 0xC2), 1);
                                          this.Resources["GameCardDarkBaseBrush"] = BuildLightGameCardBaseGradient();
                                          this.Resources["AllGameCardBaseBrush"] = BuildLightAllGameCardBaseGradient();
                                          this.Resources["GlassBackgroundGradient"] = new LinearGradientBrush
                                          {
                                              StartPoint = new System.Windows.Point(0.5, 0),
                                              EndPoint = new System.Windows.Point(0.5, 1),
                                              GradientStops = new GradientStopCollection
                                              {
                                                  new GradientStop(Color.FromRgb(0xF8, 0xF8, 0xF8), 0),
                                                  new GradientStop(Color.FromRgb(0xF2, 0xF2, 0xF2), 1)
                                              }
                                          };
                                      }
                                      else
                                      {
                                          PutSolid("TextMain", Color.FromRgb(0xFF, 0xFF, 0xFF), 1);
                                          PutSolid("TextMuted", Color.FromRgb(0xD6, 0xD6, 0xD6), 1);
                                          PutSolid("TextBody", Color.FromRgb(0xEE, 0xEE, 0xEE), 1);
                                          PutSolid("TitleBarVersionBrush", Color.FromRgb(0x9E, 0x9E, 0x9E), 1);
                                          PutSolid("TitleBarSubtextBrush", Color.FromRgb(0xB6, 0xF0, 0xFF), 1);
                                          PutSolid("BorderColor", Color.FromRgb(0x3A, 0x3A, 0x3A), 1);
                                          PutSolid("PanelFrameBorderBrush", Color.FromRgb(0x3A, 0x3A, 0x3A), 1);
                                          PutSolid("TabNavItemBgBrush", Color.FromRgb(0x25, 0x25, 0x25), 0.65);
                                          PutSolid("TabNavRailBorderBrush", Color.FromRgb(0x58, 0x58, 0x58), 0.65);
                                          PutSolid("TabNavRailBorderBrushActive", Color.FromRgb(0x72, 0x72, 0x72), 0.95);
                                          PutSolid("TabNavRailHoverBgBrush", Color.FromRgb(0x4A, 0x4A, 0x4A), 0.85);
                                          PutSolid("GlassPanelBrush", Color.FromRgb(0x1C, 0x1C, 0x1C), 1);
                                          PutSolid("FastFlagSectionBgBrush", Color.FromRgb(0x13, 0x13, 0x13), 1);
                                          PutSolid("SidebarSurfaceBrush", Color.FromRgb(0x1C, 0x1C, 0x1C), 1);
                                          PutSolid("NavRailBgBrush", Color.FromRgb(0x1C, 0x1C, 0x1C), 1);
                                          PutSolid("NavAccentBarBrush", Color.FromRgb(0x7E, 0xC7, 0xFF), 1);
                                          PutSolid("NavSectionLabelBrush", Color.FromRgb(0xA0, 0xA0, 0xA0), 1);
                                          PutSolid("NavItemActiveFgBrush", Color.FromRgb(0xFF, 0xFF, 0xFF), 1);
                                          PutSolid("NavItemInactiveFgBrush", Color.FromRgb(0xE6, 0xE6, 0xE6), 1);
                                          PutSolid("ContentWellBgBrush", Color.FromRgb(0x1C, 0x1C, 0x1C), 1);
                                          PutSolid("TabContentSurfaceBrush", Color.FromRgb(0x1C, 0x1C, 0x1C), 1);
                                          PutSolid("InteractiveButtonBgBrush", Color.FromRgb(0x2E, 0x2E, 0x2E), 1);
                                          PutSolid("InteractiveButtonHoverBrush", Color.FromRgb(0x3C, 0x3C, 0x3C), 1);
                                          PutSolid("InteractiveButtonPressedBrush", Color.FromRgb(0x25, 0x25, 0x25), 1);
                                          PutSolid("InteractiveButtonBorderBrush", Color.FromRgb(0x40, 0x40, 0x40), 1);
                                          PutSolid("InteractivePrimaryBgBrush", Color.FromRgb(0x38, 0x38, 0x38), 1);
                                          PutSolid("InteractivePrimaryHoverBrush", Color.FromRgb(0x46, 0x46, 0x46), 1);
                                          PutSolid("NavButtonHoverBgBrush", Color.FromRgb(0xFF, 0xFF, 0xFF), 0.09);
                                          PutSolid("SidebarInfoPanelSolidBrush", Color.FromRgb(0x1C, 0x1C, 0x1C), 1);
                                          PutSolid("InfoSysCyanBrush", Color.FromRgb(0x00, 0xE5, 0xFF), 1);
                                          PutSolid("InfoSysGreenBrush", Color.FromRgb(0xA5, 0xFF, 0x7F), 1);
                                          PutSolid("InfoSysLabelBrush", Color.FromRgb(0xB5, 0xB5, 0xB5), 1);
                                          PutSolid("InfoSysTimestampBrush", Color.FromRgb(0x8A, 0x8A, 0x8A), 1);
                                          PutSolid("GlassButtonBrush", Color.FromRgb(0x2E, 0x2E, 0x2E), 1);
                                          PutSolid("BgMain", Color.FromRgb(0x1C, 0x1C, 0x1C), 0.95);
                                          PutSolid("BgPanel", Color.FromRgb(0x1C, 0x1C, 0x1C), 0.90);
                                          PutSolid("BgSidebar", Color.FromRgb(0x1C, 0x1C, 0x1C), 0.95);
                                          PutSolid("GameCardGlassBorderBrush", Color.FromRgb(0x70, 0x70, 0x70), 0.85);
                                          PutSolid("AllGameCardGlassBorderBrush", Color.FromRgb(0xAA, 0xAA, 0xAA), 0.9);
                                          PutSolid("ComboFaceBrush", Color.FromRgb(0x3D, 0x3D, 0x3D), 1);
                                          PutSolid("ComboPopupBrush", Color.FromRgb(0x48, 0x48, 0x48), 1);
                                          PutSolid("ComboFaceHoverBrush", Color.FromRgb(0x4A, 0x4A, 0x4A), 1);
                                          PutSolid("ComboItemHoverBrush", Color.FromRgb(0x56, 0x56, 0x56), 1);
                                          PutSolid("ComboShadowBrush", Color.FromRgb(0x1D, 0x1D, 0x1D), 1);
                                          PutSolid("InputFieldBgBrush", Color.FromRgb(0x2E, 0x2E, 0x2E), 1);
                                          PutSolid("AppearanceEyeButtonBgBrush", Color.FromRgb(0x1F, 0x2A, 0x3A), 1);
                                          PutSolid("AppearanceEyeButtonBorderBrush", Color.FromRgb(0x4B, 0x55, 0x63), 1);
                                          PutSolid("AppearanceEyeButtonHoverBgBrush", Color.FromRgb(0x2B, 0x3A, 0x50), 1);
                                          PutSolid("AppearanceEyeButtonHoverBorderBrush", Color.FromRgb(0x60, 0xA5, 0xFA), 1);
                                          PutSolid("AppearanceEyeButtonPressedBgBrush", Color.FromRgb(0x11, 0x18, 0x27), 1);
                                          PutSolid("AppearanceEyeIconBrush", Color.FromRgb(0xF9, 0xFA, 0xFB), 1);
                                          PutSolid("ActivityLogHoverBgBrush", Color.FromRgb(0x2E, 0x2E, 0x2E), 1);
                                          PutSolid("ActivityLogSelectedBgBrush", Color.FromRgb(0x34, 0x3E, 0x4D), 1);
                                          PutSolid("UiAccentHeading", Color.FromRgb(0xB8, 0xDB, 0xFF), 1);
                                          PutSolid("UiAccentLabel", Color.FromRgb(0x7A, 0xE8, 0xFF), 1);
                                          PutSolid("InfoPanelBorderBrush", Color.FromRgb(0x10, 0x84, 0xD7), 0.8);
                                          PutSolid("BrightBlue", Color.FromRgb(0x8E, 0xF0, 0xFF), 1);
                                          PutSolid("Success", Color.FromRgb(0x62, 0xF0, 0x9A), 1);
                                          PutSolid("FaqSubTabHoverBgBrush", Color.FromRgb(0x4A, 0x55, 0x66), 1);
                                          PutSolid("FaqCreditsBannerGlassBrush", Color.FromRgb(0xF5, 0xF7, 0xFA), 0.34);
                                          PutSolid("FaqCreditsBannerBorderBrush", Color.FromRgb(0xFF, 0xFF, 0xFF), 0.38);
                                          PutSolid("FaqCoreFlowBadgeBgBrush", Color.FromRgb(0x58, 0x78, 0x9E), 1);
                                          PutSolid("FaqCoreFlowBadgeFgBrush", Color.FromRgb(0xFF, 0xFF, 0xFF), 1);
                                          PutSolid("PanelCardGlassBlurTintBrush", Color.FromRgb(0x88, 0x96, 0xA8), 85.0 / 255.0);
                                          PutSolid("DonorTierTitleFgBrush", Color.FromRgb(0xF2, 0xF2, 0xF3), 1);
                                          PutSolid("DonorColumnDividerBrush", Color.FromRgb(0x3E, 0x3E, 0x42), 1);
                                          this.Resources["GameCardDarkBaseBrush"] = BuildDarkGameCardBaseGradient();
                                          this.Resources["AllGameCardBaseBrush"] = BuildDarkAllGameCardBaseGradient();
                                          this.Resources["GlassBackgroundGradient"] = new LinearGradientBrush
                                          {
                                              StartPoint = new System.Windows.Point(0.5, 0),
                                              EndPoint = new System.Windows.Point(0.5, 1),
                                              GradientStops = new GradientStopCollection
                                              {
                                                  new GradientStop(Color.FromRgb(0x1C, 0x1C, 0x1C), 0),
                                                  new GradientStop(Color.FromRgb(0x25, 0x25, 0x25), 1)
                                              }
                                          };
                                      }
                                  }

                                  private void FaqSocialHyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
                                  {
                                      try
                                      {
                                          Process.Start(new ProcessStartInfo
                                          {
                                              FileName = e.Uri.AbsoluteUri,
                                              UseShellExecute = true
                                          });
                                          e.Handled = true;
                                      }
                                      catch (Exception ex)
                                      {
                                          try { this.Log("[FAQ] Open link failed: " + ex.Message); } catch { }
                                      }
                                  }

                                  private void FaqSocialButton_Click(object sender, RoutedEventArgs e)
                                  {
                                      try
                                      {
                                          if (sender is not Button button)
                                              return;

                                          string url = button.Tag?.ToString() ?? string.Empty;
                                          if (string.IsNullOrWhiteSpace(url))
                                              return;

                                          Process.Start(new ProcessStartInfo
                                          {
                                              FileName = url,
                                              UseShellExecute = true
                                          });
                                      }
                                      catch (Exception ex)
                                      {
                                          try { this.Log("[FAQ] Open social button link failed: " + ex.Message); } catch { }
                                      }
                                  }

                                  private static LinearGradientBrush BuildDarkGameCardBaseGradient()
                                  {
                                      var g = new LinearGradientBrush { StartPoint = new System.Windows.Point(0, 0), EndPoint = new System.Windows.Point(0, 1) };
                                      g.GradientStops.Add(new GradientStop(Color.FromRgb(0x35, 0x35, 0x35), 0));
                                      g.GradientStops.Add(new GradientStop(Color.FromRgb(0x2E, 0x2E, 0x2E), 0.3));
                                      g.GradientStops.Add(new GradientStop(Color.FromRgb(0x28, 0x28, 0x28), 0.6));
                                      g.GradientStops.Add(new GradientStop(Color.FromRgb(0x26, 0x26, 0x26), 1));
                                      return g;
                                  }

                                  private static LinearGradientBrush BuildLightGameCardBaseGradient()
                                  {
                                      var g = new LinearGradientBrush { StartPoint = new System.Windows.Point(0, 0), EndPoint = new System.Windows.Point(0, 1) };
                                      g.GradientStops.Add(new GradientStop(Color.FromRgb(0xFF, 0xFF, 0xFF), 0));
                                      g.GradientStops.Add(new GradientStop(Color.FromRgb(0xFF, 0xFF, 0xFF), 0.3));
                                      g.GradientStops.Add(new GradientStop(Color.FromRgb(0xF8, 0xF8, 0xF8), 0.6));
                                      g.GradientStops.Add(new GradientStop(Color.FromRgb(0xF5, 0xF5, 0xF5), 1));
                                      return g;
                                  }

                                  private static LinearGradientBrush BuildDarkAllGameCardBaseGradient()
                                  {
                                      var g = new LinearGradientBrush { StartPoint = new System.Windows.Point(0, 0), EndPoint = new System.Windows.Point(0, 1), Opacity = 0.88 };
                                      g.GradientStops.Add(new GradientStop(Color.FromArgb(0xDD, 0x30, 0x30, 0x30), 0));
                                      g.GradientStops.Add(new GradientStop(Color.FromArgb(0xCC, 0x2A, 0x2A, 0x2A), 0.3));
                                      g.GradientStops.Add(new GradientStop(Color.FromArgb(0xBB, 0x24, 0x24, 0x24), 0.7));
                                      g.GradientStops.Add(new GradientStop(Color.FromArgb(0xAA, 0x1D, 0x1D, 0x1D), 1));
                                      return g;
                                  }

                                  private static LinearGradientBrush BuildLightAllGameCardBaseGradient()
                                  {
                                      var g = new LinearGradientBrush { StartPoint = new System.Windows.Point(0, 0), EndPoint = new System.Windows.Point(0, 1), Opacity = 0.88 };
                                      g.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF), 0));
                                      g.GradientStops.Add(new GradientStop(Color.FromArgb(0xF5, 0xFF, 0xFF, 0xFF), 0.3));
                                      g.GradientStops.Add(new GradientStop(Color.FromArgb(0xEA, 0xF8, 0xF8, 0xF8), 0.7));
                                      g.GradientStops.Add(new GradientStop(Color.FromArgb(0xE0, 0xF5, 0xF5, 0xF5), 1));
                                      return g;
                                  }

                                  private void RestoreLanguageSettings()
                                  {
                                      try
                                      {
                                          string saved = this._settingsManager.GetDisplayLanguage();
                                          if (string.IsNullOrWhiteSpace(saved)) saved = "English";
                                          LocalizationService.SetLanguage(saved);
                                          this._currentDisplayLanguage = saved;

                                          this.EnsureLanguageComboEmbedded();
                                          this.EnsureUiThemeComboEmbedded();
                                          this.EnsureEffectThemeComboEmbedded();
                                          if (!string.Equals(this._currentDisplayLanguage, "English", StringComparison.OrdinalIgnoreCase) &&
                                              !string.Equals(this._currentDisplayLanguage, "EnglishCanada", StringComparison.OrdinalIgnoreCase) &&
                                              !string.Equals(this._currentDisplayLanguage, "SouthAfrica", StringComparison.OrdinalIgnoreCase))
                                          {
                                              void TranslateOnce()
                                              {
                                                  try { this.TranslateVisualTree(this); }
                                                  catch { }
                                              }
                                              TranslateOnce();
                                              this.Dispatcher.BeginInvoke(new Action(TranslateOnce), DispatcherPriority.Loaded);
                                              this.Dispatcher.BeginInvoke(new Action(TranslateOnce), DispatcherPriority.ApplicationIdle);
                                              var delayTimer = new DispatcherTimer(DispatcherPriority.ApplicationIdle, this.Dispatcher) { Interval = TimeSpan.FromMilliseconds(450) };
                                              delayTimer.Tick += (s, _) => { delayTimer.Stop(); TranslateOnce(); };
                                              delayTimer.Start();
                                              var lateTimer = new DispatcherTimer(DispatcherPriority.ApplicationIdle, this.Dispatcher) { Interval = TimeSpan.FromMilliseconds(1200) };
                                              lateTimer.Tick += (s, _) => { lateTimer.Stop(); try { this.TranslateVisualTree(this); } catch { } };
                                              lateTimer.Start();
                                          }
                                      }
                                      catch (Exception ex) { this.Log($"[Language] ✗ Error restoring language: {ex.Message}"); }
                                  }

                                  private void TranslateVisualTree(System.Windows.DependencyObject root)
                                  {
                                      const int MaxVisualTreeDepth = 512;
                                      if (root == null)
                                          return;
                                      var stack = new Stack<(System.Windows.DependencyObject node, int depth)>();
                                      stack.Push((root, 0));
                                      while (stack.Count > 0)
                                      {
                                          (System.Windows.DependencyObject current, int depth) = stack.Pop();
                                          if (depth > MaxVisualTreeDepth)
                                              continue;
                                          if (current == this.LanguageCombo)
                                              continue;
                                          if (this.FlagsDataGrid != null && ReferenceEquals(current, this.FlagsDataGrid))
                                              continue;

                                          if (current is System.Windows.Controls.TextBlock tb)
                                          {
                                              if (this.ProxyStatusText != null && ReferenceEquals(tb, this.ProxyStatusText))
                                                  continue;
                                              LocalizationService.ApplyTranslationToTextBlock(tb);
                                          }
                                          else if (current is System.Windows.Controls.Button btn && btn.Content is string btnStr && !string.IsNullOrWhiteSpace(btnStr))
                                              btn.Content = LocalizationService.Translate(btnStr);
                                          else if (current is System.Windows.Controls.TabItem ti && ti.Header is string headerStr && !string.IsNullOrWhiteSpace(headerStr))
                                              ti.Header = LocalizationService.Translate(headerStr);
                                          else if (current is System.Windows.Controls.ComboBoxItem cbi && cbi.Content is string cbiStr && !string.IsNullOrWhiteSpace(cbiStr))
                                              cbi.Content = LocalizationService.Translate(cbiStr);
                                          else if (current is System.Windows.Controls.Label lbl && lbl.Content is string lblStr && !string.IsNullOrWhiteSpace(lblStr))
                                              lbl.Content = LocalizationService.Translate(lblStr);
                                          else if (current is System.Windows.Controls.CheckBox chk && chk.Content is string chkStr && !string.IsNullOrWhiteSpace(chkStr))
                                              chk.Content = LocalizationService.Translate(chkStr);
                                          else if (current is System.Windows.Controls.HeaderedContentControl hcc && hcc.Header is string hdrStr && !string.IsNullOrWhiteSpace(hdrStr))
                                              hcc.Header = LocalizationService.Translate(hdrStr);
                                          else if (current is System.Windows.Controls.ToolTip tt && tt.Content is string ttStr && !string.IsNullOrWhiteSpace(ttStr))
                                              tt.Content = LocalizationService.Translate(ttStr);
                                          else if (current is System.Windows.Controls.Expander exp && exp.Header is string expStr && !string.IsNullOrWhiteSpace(expStr))
                                              exp.Header = LocalizationService.Translate(expStr);
                                          else if (current is System.Windows.Controls.RadioButton rb && rb.Content is string rbStr && !string.IsNullOrWhiteSpace(rbStr))
                                              rb.Content = LocalizationService.Translate(rbStr);
                                          else if (current is System.Windows.Controls.ComboBox cb)
                                          {
                                              if (cb != this.LanguageCombo)
                                              {
                                                  foreach (var item in cb.Items)
                                                  {
                                                      if (item is System.Windows.Controls.ComboBoxItem itemCbi && itemCbi.Content is string itemCbiStr && !string.IsNullOrWhiteSpace(itemCbiStr))
                                                      {
                                                          itemCbi.Content = LocalizationService.Translate(itemCbiStr);
                                                      }
                                                  }
                                              }
                                          }

                                          int n = System.Windows.Media.VisualTreeHelper.GetChildrenCount(current);
                                          for (int i = n - 1; i >= 0; i--)
                                          {
                                              var child = System.Windows.Media.VisualTreeHelper.GetChild(current, i);
                                              stack.Push((child, depth + 1));
                                          }
                                      }
                                  }

                                  private void TranslateVisualTreeMainTabContent(int tabIndex)
                                  {
                                      if (string.Equals(LocalizationService.CurrentLanguage, LocalizationService.English, StringComparison.OrdinalIgnoreCase))
                                          return;
                                      if (this.MainTabControl == null || tabIndex < 0 || tabIndex >= this.MainTabControl.Items.Count)
                                          return;
                                      if (this.MainTabControl.Items[tabIndex] is not System.Windows.Controls.TabItem ti)
                                          return;
                                      if (ti.Content is not System.Windows.DependencyObject root)
                                          return;
                                      try
                                      {
                                          this.TranslateVisualTree(root);
                                      }
                                      catch (Exception ex)
                                      {
                                          this.Log($"[Language] Tab content translate failed (index {tabIndex}): {ex.Message}");
                                      }
                                  }

                                  private int GetDisplayLanguageIndex(string language)
                                  {
                                      string tag = MapDisplayLanguageToCanonicalTag(language);
                                      int idx = Array.IndexOf(EmbeddedLanguageOptions, tag);
                                      return idx >= 0 ? idx : Array.IndexOf(EmbeddedLanguageOptions, "English");
                                  }

                                  private string GetSelectedDisplayLanguage()
                                  {
                                      if (this.LanguageCombo?.SelectedItem is ComboBoxItem languageItem)
                                      {
                                          if (languageItem.Tag is string tagLang && !string.IsNullOrWhiteSpace(tagLang))
                                              return tagLang;
                                          string selected = languageItem.Content?.ToString()?.Trim() ?? "English";
                                          if (string.Equals(selected, "Vietnamese", StringComparison.OrdinalIgnoreCase) ||
                                              string.Equals(selected, "Tiáº¿ng Viá»‡t", StringComparison.OrdinalIgnoreCase))
                                              return "Vietnamese";
                                          if (string.Equals(selected, "Filipino", StringComparison.OrdinalIgnoreCase))
                                              return "Filipino";
                                          if (string.Equals(selected, "Indonesian", StringComparison.OrdinalIgnoreCase) ||
                                              string.Equals(selected, "Bahasa Indonesia", StringComparison.OrdinalIgnoreCase))
                                              return "Indonesian";
                                          if (string.Equals(selected, "Portuguese", StringComparison.OrdinalIgnoreCase) ||
                                              string.Equals(selected, "PortuguÃªs", StringComparison.OrdinalIgnoreCase))
                                              return "Portuguese";
                                          if (string.Equals(selected, "Malay", StringComparison.OrdinalIgnoreCase) ||
                                              string.Equals(selected, "Bahasa Melayu", StringComparison.OrdinalIgnoreCase))
                                              return "Malay";
                                          if (string.Equals(selected, "Japanese", StringComparison.OrdinalIgnoreCase) ||
                                              string.Equals(selected, "æ—¥æœ¬èªž", StringComparison.OrdinalIgnoreCase))
                                              return "Japanese";
                                          if (string.Equals(selected, "Korean", StringComparison.OrdinalIgnoreCase) ||
                                              (selected != null && selected.Contains("í•œêµ­ì–´")))
                                              return "Korean";
                                          if (string.Equals(selected, "Khmer", StringComparison.OrdinalIgnoreCase) ||
                                              (selected != null && selected.Contains("áž€áž˜áŸ’áž–áž»áž‡àº²")))
                                              return "Khmer";
                                          if (string.Equals(selected, "Lao", StringComparison.OrdinalIgnoreCase) ||
                                              (selected != null && selected.Contains("àºžàº²àºªàº²àº¥àº²àº§")))
                                              return "Lao";
                                          if (string.Equals(selected, "Chinese", StringComparison.OrdinalIgnoreCase) ||
                                              string.Equals(selected, "ä¸­å›½", StringComparison.OrdinalIgnoreCase))
                                              return "Chinese";
                                          if (string.Equals(selected, "Thai", StringComparison.OrdinalIgnoreCase) ||
                                              string.Equals(selected, "à¸ à¸²à¸©à¸²à¹„à¸—à¸¢", StringComparison.OrdinalIgnoreCase))
                                              return "Thai";
                                          if (string.Equals(selected, "Russian", StringComparison.OrdinalIgnoreCase) ||
                                              string.Equals(selected, "Ð ÑƒÑÑÐºÐ¸Ð¹", StringComparison.OrdinalIgnoreCase))
                                              return "Russian";
                                          if (string.Equals(selected, "Ukrainian", StringComparison.OrdinalIgnoreCase) ||
                                              string.Equals(selected, "Ð£ÐºÑ€Ð°Ñ—Ð½ÑÑŒÐºÐ°", StringComparison.OrdinalIgnoreCase))
                                              return "Ukrainian";
                                          if (string.Equals(selected, "SpanishArgentina", StringComparison.OrdinalIgnoreCase) ||
                                              string.Equals(selected, "EspaÃ±ol (latinoamericano)", StringComparison.OrdinalIgnoreCase))
                                              return "SpanishArgentina";
                                          if (string.Equals(selected, "EnglishCanada", StringComparison.OrdinalIgnoreCase) ||
                                              string.Equals(selected, "English (Canada)", StringComparison.OrdinalIgnoreCase))
                                              return "EnglishCanada";
                                          if (string.Equals(selected, "French", StringComparison.OrdinalIgnoreCase) ||
                                              string.Equals(selected, "FranÃ§ais", StringComparison.OrdinalIgnoreCase))
                                              return "French";
                                          if (string.Equals(selected, "Hebrew", StringComparison.OrdinalIgnoreCase) ||
                                              string.Equals(selected, "×¢×‘×¨×™×ª", StringComparison.OrdinalIgnoreCase))
                                              return "Hebrew";
                                          if (string.Equals(selected, "Colombia", StringComparison.OrdinalIgnoreCase) ||
                                              (selected != null && selected.Contains("EspaÃ±ol (Colombia)", StringComparison.Ordinal)))
                                              return "Colombia";
                                          if (string.Equals(selected, "Taiwan", StringComparison.OrdinalIgnoreCase) ||
                                              (selected != null && selected.Contains("ç¹é«”ä¸­æ–‡ï¼ˆå°ç£ï¼‰", StringComparison.Ordinal)))
                                              return "Taiwan";
                                          if (string.Equals(selected, "Turkiye", StringComparison.OrdinalIgnoreCase) ||
                                              (selected != null && selected.Contains("TÃ¼rkÃ§e", StringComparison.Ordinal)))
                                              return "Turkiye";
                                          if (string.Equals(selected, "Brazil", StringComparison.OrdinalIgnoreCase) ||
                                              (selected != null && selected.Contains("PortuguÃªs (Brasil)", StringComparison.Ordinal)))
                                              return "Brazil";
                                          if (string.Equals(selected, "Italy", StringComparison.OrdinalIgnoreCase) ||
                                              string.Equals(selected, "Italiano", StringComparison.OrdinalIgnoreCase))
                                              return "Italy";
                                          if (string.Equals(selected, "Spain", StringComparison.OrdinalIgnoreCase) ||
                                              (selected != null && selected.Contains("EspaÃ±ol (EspaÃ±a)", StringComparison.Ordinal)))
                                              return "Spain";
                                          if (string.Equals(selected, "Chile", StringComparison.OrdinalIgnoreCase) ||
                                              (selected != null && selected.Contains("EspaÃ±ol (Chile)", StringComparison.Ordinal)))
                                              return "Chile";
                                          if (string.Equals(selected, "UnitedArabEmirates", StringComparison.OrdinalIgnoreCase) ||
                                              (selected != null && selected.Contains("Ø§Ù„Ø¹Ø±Ø¨ÙŠØ© (Ø§Ù„Ø¥Ù…Ø§Ø±Ø§Øª)", StringComparison.Ordinal)))
                                              return "UnitedArabEmirates";
                                          if (string.Equals(selected, "SouthAfrica", StringComparison.OrdinalIgnoreCase) ||
                                              string.Equals(selected, "English (South Africa)", StringComparison.OrdinalIgnoreCase))
                                              return "SouthAfrica";
                                          if (string.Equals(selected, "English", StringComparison.OrdinalIgnoreCase))
                                              return "English";
                                      }
                                      return "English";
                                  }

                                  private void SaveToggleStates()
                              {
                                  try
                                  {
                                      bool desktopShortcut = this.DesktopShortcutToggle?.IsChecked ?? false;
                                      bool startMenuShortcut = this.StartMenuShortcutToggle?.IsChecked ?? false;
                                      bool launchRobloxShortcut = this.LaunchRobloxShortcutToggle?.IsChecked ?? false;
                                      bool autoLoadFlags = true;
                                      bool autoLoadCache = true;
                                      bool minimizeToTray = false;
                                      bool preserveRenderingQuality = this.PreserveQualityToggle?.IsChecked ?? false;
                                      bool frmQuality = this.FRMQualityToggle?.IsChecked ?? false;
                                      bool allowManageFastFlags = this.AllowManageFastFlagsToggle?.IsChecked ?? true;
                                      int unlock240GlobalRequestedFromUi = this.ReadUnlock240GlobalRequestedFromUi();
                                      Unlock240FpsMode unlock240Mode = this._unlock240FpsMode;
                                      int unlock240GlobalRequested = unlock240GlobalRequestedFromUi;
                                      string displayLanguage = this.GetSelectedDisplayLanguage();
                                      string effectTheme = this.GetSelectedEffectTheme();
                                      string uiTheme = this.GetSelectedUiTheme();
                                      bool meshDetailEnabled = this.MeshDetailToggle?.IsChecked ?? false;
                                      int meshDetailValue = 3;
                                      bool fastMode = this.FastModeToggle?.IsChecked ?? this._settingsManager.IsFastModeEnabled();

                                      this._settingsManager.SaveToggleStates(desktopShortcut, autoLoadFlags, autoLoadCache, minimizeToTray);
                                      this._settingsManager.SetStartMenuShortcutEnabled(startMenuShortcut);
                                      this._settingsManager.SetLaunchRobloxShortcutEnabled(launchRobloxShortcut);
                                      this._settingsManager.SetDisplayLanguage(displayLanguage);
                                      this._settingsManager.SetGlobalTheme(effectTheme);
                                      this._settingsManager.SetUiTheme(uiTheme);
                                      this._currentDisplayLanguage = displayLanguage;
                                      this._currentGlobalTheme = effectTheme;
                                      this._currentUiTheme = uiTheme;
                                      this.ApplyGlobalThemeToUi(effectTheme, uiTheme);
                                      this._settingsManager.SetFastModeEnabled(fastMode);
                                      this._settingsManager.SetAutoCleanupTempEnabled(false);
                                      this._settingsManager.SetMemoryOptimizationEnabled(false);

                                      this._settingsManager.SetAllowManageFastFlagsEnabled(allowManageFastFlags);
                                      this._settingsManager.SetUnlock240FpsMode(unlock240Mode);
                                      this._settingsManager.SetUnlock240GlobalFpsRequested(unlock240GlobalRequested);
                                      this._settingsManager.SetUnlock240GlobalFpsExplicitlySaved(true);
                                      this._unlock240GlobalFpsRequested = unlock240GlobalRequested;
                                      this.Log($"[Settings] Unlock240 Global FPS saved = {unlock240GlobalRequested}");

                                      this.Log("[GlobalSettings] Saving Global Settings to file...");
                                      if (this._gbsEditor != null && this._gbsEditor.Loaded)
                                      {
                                          int quality = (int)this.GraphicsQualitySlider.Value;
                                          this._gbsEditor.SetValue(this._gbsEditor.PresetPaths["Rendering.SavedQualityLevel"], quality);

                                          bool maxQualityEnabled = this.MaxQualityEnabledToggle.IsChecked ?? false;
                                          this._gbsEditor.SetValue(this._gbsEditor.PresetPaths["Rendering.MaxQualityEnabled"], maxQualityEnabled);

                                          int graphicsQualityLevel = (int)this.GraphicsQualityLevelSlider.Value;
                                          this._gbsEditor.SetValue(this._gbsEditor.PresetPaths["Rendering.GraphicsQualityLevel"], graphicsQualityLevel);


                                          double transparency = this.TransparencySlider.Value;
                                          this._gbsEditor.SetValue(this._gbsEditor.PresetPaths["UI.Transparency"], transparency);

                                          bool reducedMotion = this.ReducedMotionToggle.IsChecked ?? false;
                                          this._gbsEditor.SetValue(this._gbsEditor.PresetPaths["UI.ReducedMotion"], reducedMotion);

                                          int fontSize = this.FontSizeCombo.SelectedIndex + 1;
                                          this._gbsEditor.SetValue(this._gbsEditor.PresetPaths["UI.FontSize"], fontSize);

                                          if (NumericInputHelper.TryParseUserDouble(this.MouseSensitivityInput.Text, out double mouseSens))
                                          {
                                              this._gbsEditor.SetValue(this._gbsEditor.PresetPaths["User.MouseSensitivity"], mouseSens);
                                          }

                                          bool vrEnabled = this.VREnabledToggle.IsChecked ?? false;
                                          this._gbsEditor.SetValue(this._gbsEditor.PresetPaths["User.VREnabled"], vrEnabled);

                                          this._gbsEditor.SetValue(this._gbsEditor.PresetPaths["User.PlayerNamesEnabled"], true);
                                          this._gbsEditor.Save();

                                          bool shouldBeReadOnly = this.GlobalReadOnlyToggle.IsChecked ?? false;
                                          this._gbsEditor.SetReadOnly(shouldBeReadOnly);

                                          this.Log("[GlobalSettings] ✓ Global Settings saved to GlobalBasicSettings_13.xml");
                                      }
                                      else
                                      {
                                          this.Log("[GlobalSettings] ⚠ GBSEditor not initialized, skipping Global Settings save");
                                      }

                                  bool showPlayerNames = this.PlayerNamesVisibilityToggle?.IsChecked ?? true;
                                  this._settingsManager.SetPlayerNamesVisible(showPlayerNames);
                                  this.Log("[GlobalSettings] ✓ Player Name Visibility saved (show=" + showPlayerNames + ")");

                                  this._settingsManager.SaveRenderingToggles(preserveRenderingQuality, frmQuality, meshDetailEnabled, meshDetailValue);
                                  this._settingsManager.SetFastModeEnabled(fastMode);

                                  this.HasUnsavedChanges = false;

                                  this.Log("[Settings] ✓ Toggle states saved");
                              }
                              catch (Exception ex)
                              {
                                             this.Log($"[Settings] ✗ Error saving toggle states: {ex.Message}");
                                         }
                                     }

                                  public void SaveToggleStatesPublic()
                                  {
                                      this.SaveToggleStates();
                                      this.SaveModsSettingsFromUi();
                                  }

                                  public void SaveFlagsPublic()
                                  {
                                      this.CommitPendingFastFlagGridEdits();
                                      this.PersistFastFlagsFromEditorToJsonFile();
                                  }

                                  private string _selectedAccount = "";

                                  private void AccountDropdownBtn_Checked(object sender, RoutedEventArgs e)
                                  {
                                      PopulateAccountDropdown();
                                  }

                                  private void PopulateAccountDropdown()
                                   {
                                       this.Log("[Account] Populating dropdown...");
                                       AccountListPanel.Children.Clear();

                                      var accounts = Masterstrap.Services.AccountSwitcherManager.GetSavedAccounts();

                                      foreach (var acc in accounts)
                                      {
                                          var accSp = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 2, 0, 2) };

                                          var accBtn = new Button
                                          {
                                              Background = Brushes.Transparent,
                                              BorderThickness = new Thickness(0),
                                              HorizontalContentAlignment = HorizontalAlignment.Left,
                                              Cursor = Cursors.Hand,
                                              MinWidth = 120
                                          };
                                          accBtn.Click += (s, ev) => {
                                              _selectedAccount = acc;
                                              this._settingsManager.SetSelectedAccount(_selectedAccount);
                                              this.Log($"[Account] Selected account: {acc}");
                                              try { this.UpdateActiveAccountCard(); } catch { }
                                              _ = this.LoadAccountsAsync();
                                              PopulateAccountDropdown();
                                          };
                                          var innerSp = new StackPanel { Orientation = Orientation.Horizontal };
                                          innerSp.Children.Add(new TextBlock { Text = _selectedAccount == acc ? "✓" : "", Width = 15, Foreground = (Brush)FindResource("Success"), FontWeight = FontWeights.Bold });
                                          innerSp.Children.Add(new TextBlock { Text = acc, Foreground = (Brush)FindResource("TextMain"), Margin = new Thickness(5, 0, 0, 0) });
                                          accBtn.Content = innerSp;
                                          accSp.Children.Add(accBtn);


                                          var delBtn = new Button
                                          {
                                              Background = Brushes.Transparent,
                                              BorderThickness = new Thickness(0),
                                              Cursor = Cursors.Hand,
                                              Margin = new Thickness(2,0,0,0),
                                              Padding = new Thickness(4,2,4,2),
                                              ToolTip = "Delete"
                                          };
                                          delBtn.Content = new TextBlock { Text = "\u2716", Foreground = new SolidColorBrush(Color.FromRgb(255, 100, 100)), FontSize = 12, FontWeight = FontWeights.Bold };
                                          delBtn.Click += (s, ev) => {
                                              if (System.Windows.MessageBox.Show($"Delete account '{acc}'?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                                              {
                                                  Masterstrap.Services.AccountSwitcherManager.DeleteAccount(acc);
                                                  if (_selectedAccount == acc) _selectedAccount = "";
                                                  this._settingsManager.SetSelectedAccount(_selectedAccount);
                                                  PopulateAccountDropdown();
                                              }
                                          };
                                          accSp.Children.Add(delBtn);

                                          AccountListPanel.Children.Add(accSp);
                                      }

                                      AccountListPanel.Children.Add(new Border { Height = 1, Background = new SolidColorBrush(Color.FromArgb(50, 255, 255, 255)), Margin = new Thickness(0, 4, 0, 4) });

                                      var addBtn = new Button
                                      {
                                          Background = Brushes.Transparent,
                                          BorderThickness = new Thickness(0),
                                          HorizontalContentAlignment = HorizontalAlignment.Left,
                                          Margin = new Thickness(0, 2, 0, 2),
                                          Cursor = Cursors.Hand
                                      };
                                      addBtn.Click += (s, ev) => {
                                          AccountDropdownBtn.IsChecked = false;
                                          var loginWin = new Masterstrap.Views.LoginWindow { Owner = this };
                                          if (loginWin.ShowDialog() == true && !string.IsNullOrEmpty(loginWin.ExtractedCookie))
                                          {
                                              string accountName = loginWin.DetectedUsername;
                                              if (!string.IsNullOrWhiteSpace(accountName))
                                              {
                                                  var existing = Masterstrap.Services.AccountSwitcherManager.GetSavedAccounts();
                                                  string baseName = accountName;
                                                  int counter = 2;
                                                  while (existing.Any(a => a.Equals(accountName, StringComparison.OrdinalIgnoreCase)))
                                                  {
                                                      accountName = $"{baseName} ({counter++})";
                                                  }
                                                  Masterstrap.Services.AccountSwitcherManager.SaveSessionCookie(accountName, loginWin.ExtractedCookie);
                                                  _selectedAccount = accountName;
                                                  this._settingsManager.SetSelectedAccount(_selectedAccount);
                                                  this.Log($"[Account] Added: {accountName}");
                                                  try { this.UpdateActiveAccountCard(); } catch { }
                                                  _ = this.LoadAccountsAsync();
                                              }
                                              else
                                              {
                                                  var inputWindow = new Window
                                                  {
                                                      Title = "Save New Account", Width = 300, Height = 150,
                                                      WindowStartupLocation = WindowStartupLocation.CenterOwner, Owner = this,
                                                      Background = (Brush)FindResource("AppBackgroundBrush"), Foreground = (Brush)FindResource("TextMain"),
                                                      Topmost = true
                                                  };
                                                  var stackPanel = new StackPanel { Margin = new Thickness(10) };
                                                  stackPanel.Children.Add(new TextBlock { Text = "Enter a name for this account:" });
                                                  var tb = new System.Windows.Controls.TextBox { Text = "New Account", Margin = new Thickness(0,10,0,10), Background = (Brush)FindResource("InteractiveButtonBgBrush"), Foreground = (Brush)FindResource("TextMain") };
                                                  stackPanel.Children.Add(tb);
                                                  var okBtn = new Button { Content = "Save", Style = (Style)FindResource("InteractiveButtonStyle"), Width = 80, HorizontalAlignment = HorizontalAlignment.Right };
                                                  okBtn.Click += (s2, e2) => {
                                                      if (!string.IsNullOrWhiteSpace(tb.Text))
                                                      {
                                                          Masterstrap.Services.AccountSwitcherManager.SaveSessionCookie(tb.Text, loginWin.ExtractedCookie);
                                                          _selectedAccount = tb.Text;
                                                          this._settingsManager.SetSelectedAccount(_selectedAccount);
                                                          try { this.UpdateActiveAccountCard(); } catch { }
                                                          _ = this.LoadAccountsAsync();
                                                      }
                                                      inputWindow.DialogResult = true;
                                                  };
                                                  stackPanel.Children.Add(okBtn);
                                                  inputWindow.Content = stackPanel;
                                                  inputWindow.ShowDialog();
                                              }
                                              PopulateAccountDropdown();
                                          }
                                      };
                                      var addSp = new StackPanel { Orientation = Orientation.Horizontal };
                                      addSp.Children.Add(new TextBlock { Text = "", Width = 15 });
                                      addSp.Children.Add(new TextBlock { Text = "Add account...", Foreground = (Brush)FindResource("UiAccentLabel"), FontWeight = FontWeights.SemiBold, Margin = new Thickness(5, 0, 0, 0) });
                                      addBtn.Content = addSp;
                                      AccountListPanel.Children.Add(addBtn);
                                  }

                                  private async void OpenRobloxBtn_Click(object sender, RoutedEventArgs e)
                                  {
                                      await this.RunSaveAndLaunchRobloxFlowAsync();
                                  }

                                  public async System.Threading.Tasks.Task<bool> RunSaveAndLaunchRobloxFlowAsync()
                                  {
                                      try
                                      {
                                          this._suppressSaveAndLaunchNoiseLogs = true;

                                          this.CommitPendingFastFlagGridEdits();

                                          bool skipSave = this._skipSaveForNextLaunchOnlyFlow;
                                          if (!skipSave)
                                              this.PersistFastFlagTabUiToSettingsSilent();

                                          if (Masterstrap.App.IsUpdateDialogBlockingFlagApply)
                                          {
                                              this.Log("[Roblox] Launch blocked: update dialog active");
                                              this.UpdateStatus("Update required", Colors.Orange);
                                              return false;
                                          }

                                          this._skipSaveForNextLaunchOnlyFlow = false;

                                          if (!skipSave)
                                          {
                                              this.Log("[Settings] Saving configuration before opening Roblox...");
                                              this.SaveFastFlagSettingsState();
                                              this.ApplyFastFlagSettingsPresetsToService();
                                              this.PersistFastFlagsFromEditorToJsonFile();

                                              this.ApplyShortcutSettings();

                                              bool fastMode = this.FastModeToggle?.IsChecked ?? true;

                                              this.Log("[Settings] Saving toggle states:");
                                              this.Log($"  - Desktop Shortcut: {this.DesktopShortcutToggle?.IsChecked ?? false}");
                                              this.Log($"  - Fast Mode: {fastMode}");

                                              SaveToggleStates();
                                              this.SaveModsSettingsFromUi();
                                              this.Log("[Settings] " + LocalizationService.Translate("Configuration saved successfully"));
                                          }
                                          else
                                          {
                                              this.Log("[Settings] Launch Roblox shortcut mode: skip saving, apply ClientSettings + launch only.");
                                          }

                                          this.Log("[Roblox] Checking Roblox installation...");
                                          this.UpdateStatus("Checking Roblox...", Colors.Yellow);
                                          bool isRobloxReady = await this.EnsureRobloxInstalledAsync();
                                          if (!isRobloxReady)
                                          {
                                              this.Log("[Roblox] Roblox installation/verification failed.");
                                              this.UpdateStatus("Roblox Setup Failed", Colors.Red);
                                              MessageBox.Show(
                                                  "Roblox could not be verified or installed.\n\n" +
                                                  "Check your internet connection (try again), or briefly turn off Proxy and retry.\n" +
                                                  "Details: %TEMP%\\Masterstrap_runtime.log",
                                                  "Roblox Setup Failed",
                                                  MessageBoxButton.OK,
                                                  MessageBoxImage.Error);
                                              return false;
                                          }

                                          this.Log("[Roblox] Opening Roblox...");

                                          string robloxExePath = this._robloxExecutablePath;

                                          if (string.IsNullOrEmpty(robloxExePath))
                                          {
                                              this.Log("[Roblox] ✗ Roblox version not found in Information System");
                                              this.UpdateStatus("Roblox Not Found", Colors.Red);
                                              MessageBox.Show(
                                                  "Roblox was not found.\n\n" +
                                                  "Install from https://www.roblox.com/download then restart Masterstrap.",
                                                  "Roblox Not Installed",
                                                  MessageBoxButton.OK,
                                                  MessageBoxImage.Warning);
                                              return false;
                                          }

                                          if (!System.IO.File.Exists(robloxExePath))
                                          {
                                              this.Log($"[Roblox] ✗ File khÃ´ng tá»“n táº¡i: {robloxExePath}");
                                              this.UpdateStatus("Roblox File Not Found", Colors.Red);
                                              MessageBox.Show(
                                                  "Roblox executable was not found at:\n\n" +
                                                  robloxExePath,
                                                  "File Not Found",
                                                  MessageBoxButton.OK,
                                                  MessageBoxImage.Error);
                                              return false;
                                          }

                                          this.Log($"[Roblox] ✓ Launching Roblox from: {robloxExePath}");
                                          this.Log($"[Roblox] Version: {this._robloxVersion}");
                                          this.UpdateStatus("Opening Roblox...", Colors.Blue);
                                          this.HasUnsavedChanges = false;

                                          bool allowManageFastFlags = this.AllowManageFastFlagsToggle?.IsChecked ?? true;
                                          var flagsFromEditor = allowManageFastFlags && this._allFlagsList != null && this._allFlagsList.Count > 0
                                              ? this._allFlagsList.ToList()
                                              : null;

                                          this.TryApplyFastFlagsViaClientSettings(flagsFromEditor, "[Roblox]");

                                          string protocolUrl = null;
                                          if (!string.IsNullOrEmpty(_selectedAccount))
                                          {
                                              this.UpdateStatus("Preparing account...", Colors.Yellow);
                                              var (usedCookiesDat, authTicket, accountDetail) =
                                                  await Masterstrap.Services.AccountSwitcherManager.PrepareLaunchForAccountAsync(_selectedAccount);
                                              if (!string.IsNullOrWhiteSpace(accountDetail))
                                                  this.Log($"[Account] {_selectedAccount}: {accountDetail}");
                                              if (usedCookiesDat)
                                              {
                                                  protocolUrl = null;
                                              }
                                              else if (!string.IsNullOrEmpty(authTicket))
                                              {
                                                  protocolUrl = $"roblox-player:1+launchmode:app+gameinfo:{authTicket}";
                                                  this.Log($"[Account] Launching Roblox with auth ticket for: {_selectedAccount}");
                                              }
                                              else
                                              {
                                                  this.Log($"[Account] No auto-login for {_selectedAccount}. Continuing — sign in inside Roblox if needed.");
                                              }
                                          }

                                          try
                                          {
                                              var startInfo = !string.IsNullOrEmpty(protocolUrl)
                                                  ? new System.Diagnostics.ProcessStartInfo { FileName = protocolUrl, UseShellExecute = true }
                                                  : new System.Diagnostics.ProcessStartInfo { FileName = robloxExePath, UseShellExecute = true };
                                              System.Diagnostics.Process.Start(startInfo);
                                          }
                                          catch (Exception launchEx)
                                          {
                                              this.Log($"[Roblox] Launch failed: {launchEx.Message}");
                                              this.UpdateStatus("Launch Failed", Colors.Red);
                                              return;
                                          }
                                          this._saveAndLaunchMode = false;
                                          this.Log("[Roblox] Roblox launch started");
                                          this.Dispatcher.BeginInvoke(new Action(() =>
                                          {
                                              try
                                              {
                                                  this.Close();
                                              }
                                              catch (Exception ex)
                                              {
                                                  this.Log($"[Roblox] Error closing MainWindow: {ex.Message}");
                                              }
                                          }), System.Windows.Threading.DispatcherPriority.Send);

                                          this.Log("[Roblox] Save and Launch completed");
                                          return true;
                                      }
                                      catch (Exception ex)
                                      {
                                          this.Log($"[Roblox] ✗ Error launching Roblox: {ex.Message}");
                                          this.UpdateStatus("❌ Error", Colors.Red);
                                          ShowToastNotification("Launch failed!", "#404040");
                                          MessageBox.Show(
                                              $"❌ Failed to launch Roblox\n\n" +
                                              $"Error: {ex.Message}",
                                              "Launch Failed",
                                              MessageBoxButton.OK,
                                              MessageBoxImage.Error);
                                          return false;
                                      }
                                      finally
                                      {
                                          this._suppressSaveAndLaunchNoiseLogs = false;
                                      }
                                  }

                                  public System.Threading.Tasks.Task<bool> RunLaunchAndApplyFlagsWithoutSavingAsync()
                                  {
                                      this._skipSaveForNextLaunchOnlyFlow = true;
                                      return this.RunSaveAndLaunchRobloxFlowAsync();
                                  }

                                  private void ShowUnsavedChangesDialog()
                                  {
                                      try
                                      {
                                          var dialog = new UnsavedChangesDialog();
                                          bool? result = dialog.ShowDialog();

                                          if (result == true)
                                          {
                                              if (dialog.Result == UnsavedChangesResult.Save)
                                              {
                                                  this.Log("[Dialog] User chose: SAVE");
                                                  this.SaveBtn_Click(null, null);
                                              }
                                              else if (dialog.Result == UnsavedChangesResult.DontSave)
                                              {
                                                  this.Log("[Dialog] User chose: DON'T SAVE");
                                                  this.HasUnsavedChanges = false;
                                              }
                                          }
                                          else
                                          {
                                              this.Log("[Dialog] User chose: CANCEL");
                                              var fastToggle = this.FastModeToggle;
                                              if (fastToggle != null)
                                              {
                                                  fastToggle.IsChecked = !(fastToggle.IsChecked ?? false);
                                              }
                                              this.HasUnsavedChanges = false;
                                          }
                                      }
                                      catch (Exception ex)
                                      {
                                          this.Log($"[Dialog] ✗ Error showing UnsavedChangesDialog: {ex.Message}");
                                      }
                                  }

                                  private void UpdateXmlSetting(XmlDocument xmlDoc, string settingName, string value, string filePath)
                                  {
                                      try
                                      {
                                          XmlNode setting = xmlDoc.SelectSingleNode($"//item[@name='{settingName}']");
                                          if (setting != null)
                                          {
                                              setting.InnerText = value;
                                              this.Log($"[QuickTuner] → Updated {settingName} = {value}");
                                          }
                                          else
                                          {
                                              this.Log($"[QuickTuner] ⚠ Setting '{settingName}' not found in config");
                                          }
                                          xmlDoc.Save(filePath);
                                      }
                                      catch (Exception ex)
                                      {
                                          this.Log($"[QuickTuner] Error updating {settingName}: {ex.Message}");
                                      }
                                  }

        private void StartListeningForShowEvent()
        {
            try
            {
                App.EnsureSecondInstanceShowWindowListener();
                this.Log("[System] ✓ Single instance activation uses App listener");
            }
            catch (Exception ex)
            {
                this.Log($"[System] Error starting listener: {ex.Message}");
            }
        }

        private void GraphicsQualitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                var slider = sender as Slider;
                if (slider != null)
                {
                    int snapValue = (int)Math.Round(slider.Value);

                    if (Math.Abs(slider.Value - snapValue) > 0.01)
                    {
                        slider.Value = snapValue;
                    }

                    var valueDisplay = this.FindName("GraphicsQualityValue") as TextBlock;
                    if (valueDisplay != null)
                    {
                        valueDisplay.Text = snapValue.ToString();
                    }

                    if (this._initializationComplete)
                    {
                        this.HasUnsavedChanges = true;
                        this.Log("[GlobalSettings] ✓ Graphics Quality changed - marked as unsaved");
                    }
                }
            }
            catch (Exception ex)
            {
                this.Log($"[GraphicsQualitySlider] Error: {ex.Message}");
            }
        }

        private void TransparencySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                var slider = sender as Slider;
                if (slider != null)
                {
                    double snapValue = Math.Round(slider.Value * 10) / 10.0;

                    if (Math.Abs(slider.Value - snapValue) > 0.01)
                    {
                        slider.Value = snapValue;
                    }

                    var valueDisplay = this.FindName("TransparencyValue") as TextBlock;
                    if (valueDisplay != null)
                    {
                        valueDisplay.Text = snapValue.ToString("F1");
                    }

                    if (this._initializationComplete)
                    {
                        this.HasUnsavedChanges = true;
                        this.Log("[GlobalSettings] ✓ Transparency changed - marked as unsaved");
                    }
                }
            }
            catch (Exception ex)
            {
                this.Log($"[TransparencySlider] Error: {ex.Message}");
            }
        }


        private void Unlock240FpsModeButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender == null)
                return;

            if (ReferenceEquals(sender, this.Unlock240FpsOffButton))
                this._unlock240FpsMode = Unlock240FpsMode.Off;
            else if (ReferenceEquals(sender, this.Unlock240FpsGlobalButton))
                this._unlock240FpsMode = Unlock240FpsMode.Global;
            else
                this._unlock240FpsMode = Unlock240FpsMode.FFlag;

            this.UpdateUnlock240FpsModeButtons();
            this.SyncUnlock240FpsModeToService();
            this.UpdateUnlock240GlobalFpsUiAndVisibility();

            if (this._initializationComplete)
            {
                this.HasUnsavedChanges = true;
                this.Log($"[Settings] Unlock 240FPS mode changed to {_unlock240FpsMode}");
            }
        }

        private void UpdateUnlock240FpsModeButtons()
        {
            this.ApplyUnlock240ModeButtonStyle(this.Unlock240FpsOffButton, this._unlock240FpsMode == Unlock240FpsMode.Off);
            this.ApplyUnlock240ModeButtonStyle(this.Unlock240FpsGlobalButton, this._unlock240FpsMode == Unlock240FpsMode.Global);
            this.ApplyUnlock240ModeButtonStyle(this.Unlock240FpsFflagButton, this._unlock240FpsMode == Unlock240FpsMode.FFlag);
        }

        private void ApplyUnlock240ModeButtonStyle(Button button, bool isActive)
        {
            if (button == null)
                return;

            Brush activeBorder = this.TryFindResource("UiAccentLabel") as Brush
                ?? new SolidColorBrush(Color.FromRgb(0x00, 0xFF, 0x7F));
            Brush activeBackground = this.TryFindResource("InteractiveButtonHoverBrush") as Brush
                ?? new SolidColorBrush(Color.FromArgb(0x33, 0x00, 0xFF, 0x7F));
            Brush inactiveBorder = this.TryFindResource("InteractiveButtonBorderBrush") as Brush
                ?? new SolidColorBrush(Color.FromRgb(0x3A, 0x3A, 0x3A));
            Brush inactiveBackground = this.TryFindResource("InteractiveButtonBgBrush") as Brush
                ?? new SolidColorBrush(Color.FromArgb(0x12, 0xFF, 0xFF, 0xFF));
            Brush textBrush = this.TryFindResource("TextMain") as Brush
                ?? Brushes.White;

            button.BorderThickness = new Thickness(isActive ? 1.5 : 1);
            button.BorderBrush = isActive ? activeBorder : inactiveBorder;
            button.Background = isActive ? activeBackground : inactiveBackground;
            button.Foreground = textBrush;
            button.FontWeight = isActive ? FontWeights.SemiBold : FontWeights.Normal;
        }

        private void SyncUnlock240FpsModeToService()
        {
            try
            {
                this.EnsureFFlagServiceInitialized();
                this._fflagService?.SetUnlock240FpsMode(this._unlock240FpsMode);
            }
            catch (Exception ex)
            {
                this.Log($"[ClientSettings] Unlock240FPS sync failed: {ex.Message}");
            }
        }

        private void ApplyUnlock240FpsGlobalPolicyBeforeApply()
        {
            try
            {
                if (_unlock240FpsMode == Unlock240FpsMode.FFlag)
                {
                    return;
                }

                int requestedGlobal = Math.Max(1, _unlock240GlobalFpsRequested);
                int targetFps = ResolveGlobalModeTargetFps(_unlock240FpsMode, requestedGlobal);

                    _unlock240FpsMode == Unlock240FpsMode.Global ? targetFps : null);

                bool wroteGbs = false;
                if (_gbsEditor != null && _gbsEditor.Loaded)
                {
                    _gbsEditor.SetValue(_gbsEditor.PresetPaths["Rendering.FramerateCap"], targetFps);
                    _gbsEditor.Save();
                    wroteGbs = true;
                }
                else
                {
                    var gbs = new GBSEditor();
                    gbs.Load();
                    if (gbs.Loaded)
                    {
                        gbs.SetValue(gbs.PresetPaths["Rendering.FramerateCap"], targetFps);
                        gbs.Save();
                        wroteGbs = true;
                    }
                }

                this.Log(
                    wroteGbs
                        ? $"[GlobalSettings] Applied Unlock240FPS mode {_unlock240FpsMode}: FramerateCap={targetFps}"
                        : $"[GlobalSettings] Unlock240FPS mode {_unlock240FpsMode}: FramerateCap pin for apply={targetFps} (GBS file not loaded — apply still uses pinned FPS)");
            }
            catch (Exception ex)
            {
                this.Log($"[GlobalSettings] Failed to apply Unlock240FPS mode {_unlock240FpsMode}: {ex.Message}");
            }
        }

        private int GetUnlock240GlobalMaxForCurrentPackage() => 9999;

        private int GetUnlock240GlobalDefaultForCurrentPackage() => 3000;

        private void UpdateUnlock240GlobalFpsUiAndVisibility()
        {
            try
            {
                if (this._settingsManager != null)
                    _unlock240GlobalFpsRequested = this._settingsManager.GetUnlock240GlobalFpsRequested();

                int max = GetUnlock240GlobalMaxForCurrentPackage();
                int pkgDefault = GetUnlock240GlobalDefaultForCurrentPackage();
                bool explicitlySaved = this._settingsManager?.IsUnlock240GlobalFpsExplicitlySaved() ?? false;

                if (_unlock240GlobalFpsRequested <= 0)
                    _unlock240GlobalFpsRequested = pkgDefault;
                else if (!explicitlySaved && _unlock240GlobalFpsRequested <= 240 && pkgDefault > 240)
                    _unlock240GlobalFpsRequested = pkgDefault;

                int clamped = Math.Clamp(_unlock240GlobalFpsRequested, 1, max);

                if (this.Unlock240GlobalFpsSlider != null)
                    this.Unlock240GlobalFpsSlider.Maximum = max;

                _isSyncingUnlock240GlobalFpsUi = true;
                try
                {
                    if (this.Unlock240GlobalFpsSlider != null)
                        this.Unlock240GlobalFpsSlider.Value = clamped;
                    if (this.Unlock240GlobalFpsValueInput != null)
                        this.Unlock240GlobalFpsValueInput.Text = clamped.ToString();
                }
                finally
                {
                    _isSyncingUnlock240GlobalFpsUi = false;
                }

                bool show = _unlock240FpsMode == Unlock240FpsMode.Global;
                AnimateSlideContainer(this.Unlock240GlobalSliderContainer, show, expandedHeight: 38);
            }
            catch { }
        }

        private void Unlock240GlobalFpsSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isSyncingUnlock240GlobalFpsUi)
                return;

            int v = (int)Math.Round(e.NewValue);
            _unlock240GlobalFpsRequested = v;
            if (this.Unlock240GlobalFpsValueInput != null)
            {
                _isSyncingUnlock240GlobalFpsUi = true;
                try { this.Unlock240GlobalFpsValueInput.Text = v.ToString(); }
                finally { _isSyncingUnlock240GlobalFpsUi = false; }
            }

            if (this._initializationComplete)
                this.HasUnsavedChanges = true;
        }

        private void Unlock240GlobalFpsValueInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isSyncingUnlock240GlobalFpsUi)
                return;

            if (this.Unlock240GlobalFpsValueInput == null)
                return;

            if (!int.TryParse(this.Unlock240GlobalFpsValueInput.Text?.Trim(), out int v))
                return;

            int max = GetUnlock240GlobalMaxForCurrentPackage();
            v = Math.Clamp(v, 1, max);
            _unlock240GlobalFpsRequested = v;

            if (this.Unlock240GlobalFpsSlider != null)
            {
                _isSyncingUnlock240GlobalFpsUi = true;
                try
                {
                    this.Unlock240GlobalFpsSlider.Maximum = max;
                    this.Unlock240GlobalFpsSlider.Value = v;
                }
                finally { _isSyncingUnlock240GlobalFpsUi = false; }
            }

            if (this._initializationComplete)
                this.HasUnsavedChanges = true;
        }

        private int ReadUnlock240GlobalRequestedFromUi()
        {
            int max = GetUnlock240GlobalMaxForCurrentPackage();

            if (this.Unlock240GlobalFpsValueInput != null &&
                int.TryParse(this.Unlock240GlobalFpsValueInput.Text?.Trim(), out int fromInput))
            {
                return Math.Clamp(fromInput, 1, max);
            }

            if (this.Unlock240GlobalFpsSlider != null)
            {
                int fromSlider = (int)Math.Round(this.Unlock240GlobalFpsSlider.Value);
                return Math.Clamp(fromSlider, 1, max);
            }

            return Math.Clamp(_unlock240GlobalFpsRequested, 1, max);
        }

        private void AnimateSlideContainer(FrameworkElement container, bool show, double expandedHeight)
        {
            if (container == null)
                return;

            var duration = TimeSpan.FromMilliseconds(500);
            var sb = new Storyboard();

            if (show)
            {
                container.Visibility = Visibility.Visible;
                container.Height = 0;
                container.Opacity = 0;
            }

            var h = new DoubleAnimation
            {
                To = show ? expandedHeight : 0,
                Duration = new Duration(duration),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            Storyboard.SetTarget(h, container);
            Storyboard.SetTargetProperty(h, new PropertyPath(FrameworkElement.HeightProperty));
            sb.Children.Add(h);

            var o = new DoubleAnimation
            {
                To = show ? 1 : 0,
                Duration = new Duration(duration),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            Storyboard.SetTarget(o, container);
            Storyboard.SetTargetProperty(o, new PropertyPath(UIElement.OpacityProperty));
            sb.Children.Add(o);

            var tt = container.RenderTransform as TranslateTransform;
            if (tt != null)
            {
                var y = new DoubleAnimation
                {
                    To = show ? 0 : -6,
                    Duration = new Duration(duration),
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                };
                Storyboard.SetTarget(y, container);
                Storyboard.SetTargetProperty(y, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
                sb.Children.Add(y);
            }

            if (!show)
            {
                sb.Completed += (_, __) => container.Visibility = Visibility.Collapsed;
            }

            sb.Begin();
        }

        private int ResolveGlobalModeTargetFps(Unlock240FpsMode mode, int requestedFps)
        {
            if (mode == Unlock240FpsMode.Off)
                return 240;
            if (mode == Unlock240FpsMode.FFlag)
                return Math.Max(1, requestedFps);

            return Math.Max(1, requestedFps);
        }

        private void ReducedMotionToggle_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this._initializationComplete)
                {
                    this.HasUnsavedChanges = true;
                    this.Log("[GlobalSettings] ✓ Reduced Motion enabled - marked as unsaved");
                }
            }
            catch (Exception ex)
            {
                this.Log($"[ReducedMotionToggle] Error in Checked: {ex.Message}");
            }
        }

        private void ReducedMotionToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this._initializationComplete)
                {
                    this.HasUnsavedChanges = true;
                    this.Log("[GlobalSettings] ✓ Reduced Motion disabled - marked as unsaved");
                }
            }
            catch (Exception ex)
            {
                this.Log($"[ReducedMotionToggle] Error in Unchecked: {ex.Message}");
            }
        }

        private void FontSizeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (this._initializationComplete)
                {
                    this.HasUnsavedChanges = true;
                    this.Log("[GlobalSettings] ✓ Font Size changed - marked as unsaved");
                }
            }
            catch (Exception ex)
            {
                this.Log($"[FontSizeCombo] Error: {ex.Message}");
            }
        }

        private void ProtocolInterceptionToggle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool isEnabled = this.ProtocolInterceptionToggle?.IsChecked ?? true;
                this._settingsManager.SetProtocolInterceptionEnabled(isEnabled);
                this.Log($"[Settings] Roblox Launch Interception set to: {(isEnabled ? "ENABLED" : "DISABLED")}");

                Task.Run(() =>
                {
                    try
                    {
                        AppInitializer.ReregisterProtocolHandler();
                        this.Log("[System] Protocol handler updated.");
                    }
                    catch (Exception ex)
                    {
                        this.Log($"[Error] Failed to update protocol handler: {ex.Message}");
                    }
                });
            }
            catch (Exception ex)
            {
                this.Log($"[Error] Protocol toggle error: {ex.Message}");
            }
        }

        private void LanguageCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (this._isApplyingLanguage) return;
                string selectedLanguage = this.GetSelectedDisplayLanguage();
                if (string.IsNullOrWhiteSpace(selectedLanguage)) selectedLanguage = "English";
                this._currentDisplayLanguage = selectedLanguage;
                if (this._initializationComplete)
                {
                    this.HasUnsavedChanges = true;
                    this.Log("[Language] ✓ Language changed. Save and restart the app to apply.");
                }
            }
            catch (Exception ex)
            {
                this.Log($"[Language] ✗ Error: {ex.Message}");
            }
        }

        private void UiThemeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (this._isApplyingGlobalTheme) return;
                this._currentUiTheme = this.GetSelectedUiTheme();
                if (this._initializationComplete)
                {
                    this.HasUnsavedChanges = true;
                    this.ApplyGlobalThemeToUi(this.GetSelectedEffectTheme(), this._currentUiTheme);
                    this.Log($"[Theme] ✓ Global Theme (UI) changed to: {this._currentUiTheme} (applied; Save to persist)");
                }
            }
            catch (Exception ex)
            {
                this.Log($"[Theme] ✗ Error: {ex.Message}");
            }
        }

        private void EffectThemeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (this._isApplyingGlobalTheme) return;
                this._currentGlobalTheme = this.GetSelectedEffectTheme();
                if (this._initializationComplete)
                {
                    this.HasUnsavedChanges = true;
                    this.ApplyGlobalThemeToUi(this._currentGlobalTheme, this.GetSelectedUiTheme());
                    this.Log($"[Theme] ✓ Effect Theme changed to: {this._currentGlobalTheme} (applied; Save to persist)");
                }
            }
            catch (Exception ex)
            {
                this.Log($"[Theme] ✗ Error: {ex.Message}");
            }
        }

        private void UploadBackgroundBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog
                {
                    Title = "Select background image",
                    Filter = "Image Files|*.png;*.jpg;*.jpeg;*.webp;*.bmp",
                    CheckFileExists = true,
                    Multiselect = false
                };

                if (dialog.ShowDialog() != true)
                    return;

                this._customBackgroundImagePath = dialog.FileName;
                this._settingsManager.SetCustomBackgroundImagePath(this._customBackgroundImagePath);
                this.HasUnsavedChanges = true;
                this.ApplyGlobalThemeToUi(this._currentGlobalTheme, this.GetSelectedUiTheme());
                this.Log($"[Theme] ✓ Background image selected: {System.IO.Path.GetFileName(dialog.FileName)}");
            }
            catch (Exception ex)
            {
                this.Log($"[Theme] ✗ Error uploading background: {ex.Message}");
            }
        }

        private void ResetBackgroundBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this._customBackgroundImagePath = string.Empty;
                this._settingsManager.SetCustomBackgroundImagePath(string.Empty);
                this.HasUnsavedChanges = true;
                this.ApplyGlobalThemeToUi(this._currentGlobalTheme, this.GetSelectedUiTheme());
                this.Log("[Theme] ✓ Background image reset");
            }
            catch (Exception ex)
            {
                this.Log($"[Theme] ✗ Error resetting background: {ex.Message}");
            }
        }

        private void MouseSensitivityInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (this._initializationComplete)
                {
                    this.HasUnsavedChanges = true;
                    this.Log("[GlobalSettings] ✓ Mouse Sensitivity changed - marked as unsaved");
                }
            }
            catch (Exception ex)
            {
                this.Log($"[MouseSensitivityInput] Error: {ex.Message}");
            }
        }

        private void VREnabledToggle_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this._initializationComplete)
                {
                    this.HasUnsavedChanges = true;
                    this.Log("[GlobalSettings] ✓ VR Enabled toggled - marked as unsaved");
                }
            }
            catch (Exception ex)
            {
                this.Log($"[VREnabledToggle] Error in Checked: {ex.Message}");
            }
        }

        private void VREnabledToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this._initializationComplete)
                {
                    this.HasUnsavedChanges = true;
                    this.Log("[GlobalSettings] ✓ VR Enabled toggled - marked as unsaved");
                }
            }
            catch (Exception ex)
            {
                this.Log($"[VREnabledToggle] Error in Unchecked: {ex.Message}");
            }
        }

        private void GlobalReadOnlyToggle_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this._initializationComplete)
                {
                    this.HasUnsavedChanges = true;
                    this.Log("[GlobalSettings] ✓ Global Read-Only enabled - marked as unsaved");
                }
            }
            catch (Exception ex)
            {
                this.Log($"[GlobalReadOnlyToggle] Error in Checked: {ex.Message}");
            }
        }

        private void GlobalReadOnlyToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this._initializationComplete)
                {
                    this.HasUnsavedChanges = true;
                    this.Log("[GlobalSettings] ✓ Global Read-Only disabled - marked as unsaved");
                }
            }
            catch (Exception ex)
            {
                this.Log($"[GlobalReadOnlyToggle] Error in Unchecked: {ex.Message}");
            }
        }

        private void PlayerNamesVisibilityToggle_Checked(object sender, RoutedEventArgs e)
        {
            _ = ApplyPlayerNamesVisibilityFromToggleAsync(false);
        }

        private void PlayerNamesVisibilityToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            _ = ApplyPlayerNamesVisibilityFromToggleAsync(true);
        }

        private void OnPlayerNamesHiddenChanged(bool _)
        {
            try
            {
                var d = this.Dispatcher;
                Action sync = () =>
                {
                    try
                    {
                        if (this.PlayerNamesVisibilityToggle == null)
                            return;

                        this._syncingPlayerNamesToggle = true;
                        this.PlayerNamesVisibilityToggle.IsChecked = this._settingsManager?.GetPlayerNamesVisible() ?? true;
                    }
                    finally
                    {
                        this._syncingPlayerNamesToggle = false;
                    }
                };

                if (d == null || d.CheckAccess())
                    sync();
                else
                    d.BeginInvoke(sync);
            }
            catch
            {
            }
        }


        private bool _syncingPlayerNamesToggle;

        private void SyncPlayerNamesVisibilityToggleFromSettings()
        {
            try
            {
                if (this.PlayerNamesVisibilityToggle == null)
                    return;

                this._syncingPlayerNamesToggle = true;
                this.PlayerNamesVisibilityToggle.IsChecked = this._settingsManager?.GetPlayerNamesVisible() ?? true;
            }
            catch (Exception ex)
            {
                this.Log("[PlayerNames] Sync toggle from settings: " + ex.Message);
            }
            finally
            {
                this._syncingPlayerNamesToggle = false;
            }
        }

        private async Task ApplyPlayerNamesVisibilityFromToggleAsync(bool hideNames)
        {
            try
            {
                if (this._syncingPlayerNamesToggle)
                    return;

                if (!this._initializationComplete)
                    return;

                this.HasUnsavedChanges = true;
                this._settingsManager?.SetPlayerNamesVisible(!hideNames);
                this.Log($"[GlobalSettings] ✓ Player Name Visibility saved (show={!hideNames})");
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                this.Log($"[PlayerNamesVisibilityToggle] Error: {ex.Message}");
            }
        }

        private void SyncPlayerNamesHotkeyFlagsBeforeApply() { }

        private void EnsurePlayerNamesHotkeyFlagsInEditorPayload(bool includeGuiHideShortcuts)
        {
            try
            {
                if (this._allFlagsList == null)
                    return;

                this.UpsertFlagForPlayerNamesHotkeys(
                    flagName: "FFlagEnablePlayerNamesEnabledSetting",
                    flagValue: "true",
                    forceEditedState: false);

                if (includeGuiHideShortcuts)
                {
                    this.UpsertFlagForPlayerNamesHotkeys(
                        flagName: "DFIntCanHideGuiGroupId",
                        flagValue: "32380007",
                        forceEditedState: false);
                }

                this.ApplyEditorFlagsFilter();
                this.UpdateEditStats();
            }
            catch (Exception ex)
            {
                this.Log("[PlayerNames] Ensure hotkey flags in editor failed: " + ex.Message);
            }
        }

        private void UpsertFlagForPlayerNamesHotkeys(string flagName, string flagValue, bool forceEditedState)
        {
            var existing = this._allFlagsList.FirstOrDefault(
                f => f.Name.Equals(flagName, StringComparison.OrdinalIgnoreCase));

            if (existing != null)
            {
                existing.Value = flagValue;
                existing.IsEdited = forceEditedState;
                existing.LastModified = DateTime.UtcNow;
                return;
            }

            var flagItem = new FlagItem
            {
                Name = flagName,
                Value = flagValue,
                IsEdited = forceEditedState,
                LastModified = DateTime.UtcNow
            };

            this._allFlagsList.Add(flagItem);
        }

        private void MaxQualityEnabledToggle_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this._initializationComplete)
                {
                    this.HasUnsavedChanges = true;
                    this.Log("[GlobalSettings] ✓ Max Quality Enabled changed - marked as unsaved");
                }
            }
            catch (Exception ex)
            {
                this.Log($"[MaxQualityEnabledToggle] Error in Checked: {ex.Message}");
            }
        }

        private void MaxQualityEnabledToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this._initializationComplete)
                {
                    this.HasUnsavedChanges = true;
                    this.Log("[GlobalSettings] ✓ Max Quality Enabled changed - marked as unsaved");
                }
            }
            catch (Exception ex)
            {
                this.Log($"[MaxQualityEnabledToggle] Error in Unchecked: {ex.Message}");
            }
        }

        private void GraphicsQualityLevelSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                if (this._initializationComplete && this.GraphicsQualityLevelValue != null)
                {
                    int value = (int)e.NewValue;
                    this.GraphicsQualityLevelValue.Text = value.ToString();
                    this.HasUnsavedChanges = true;
                    this.Log($"[GlobalSettings] ✓ Graphics Quality Level changed to {value} - marked as unsaved");
                }
            }
            catch (Exception ex)
            {
                this.Log($"[GraphicsQualityLevelSlider] Error: {ex.Message}");
            }
        }

        private void InitializeActivityLog()
        {
        }

        public void AddActivityEntry(string message, Color? color = null, bool isClickable = false, string url = "")
        {
        }

        public void LogFlagApplyResult(bool success, int successCount = 0, int failedCount = 0, int totalFlagCount = 0)
        {
        }

        public void ClearActivityLog()
        {
            if (this.ActivityLogListBox != null)
            {
                this.Dispatcher.Invoke(() => this.ActivityLogListBox.Items.Clear());
            }
        }

        public void ClearActivityLogSilent()
        {
            if (this.ActivityLogListBox != null)
            {
                this.Dispatcher.Invoke(() => this.ActivityLogListBox.Items.Clear());
            }
        }

        public void ClearLogBtn_Click(object sender, RoutedEventArgs e)
        {
            this.ClearActivityLog();
        }

        public void DisplayJsonContentInLog(string jsonPath)
        {
        }

    }
}

