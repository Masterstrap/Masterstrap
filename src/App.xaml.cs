using System;
using System.Windows;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.IO;
using System.Linq;
using Masterstrap.Services;
using System.Threading.Tasks;
using Masterstrap.Views;
using Masterstrap.Helpers;
using Microsoft.Win32;
namespace Masterstrap
{
    public partial class App : Application
    {
        private static Mutex _mutex = null;
        private const string AppId = "Masterstrap_SingleInstance_Mutex";
        private const string ShowWindowEventName = "Masterstrap_ShowWindow_Event";
        private EventWaitHandle _showWindowEvent = null;
        private static readonly HttpClient _httpClient = new HttpClient();
        public static volatile bool IsUpdateDialogBlockingFlagApply = false;
        private static int _secondInstanceShowListenerStarted;
        private static Window _activationTargetWindow;

        internal static void RegisterShowWindowActivationTarget(Window window)
        {
            _activationTargetWindow = window;
        }

        internal static void EnsureSecondInstanceShowWindowListener()
        {
            if (Interlocked.CompareExchange(ref _secondInstanceShowListenerStarted, 1, 0) != 0)
                return;
            var dispatcher = Current?.Dispatcher;
            if (dispatcher == null)
                return;
            _ = Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        using (var showEvent = EventWaitHandle.OpenExisting(ShowWindowEventName))
                        {
                            if (showEvent.WaitOne(1000))
                            {
                                try { showEvent.Reset(); } catch { }
                                _ = dispatcher.BeginInvoke(new Action(() =>
                                {
                                    try
                                    {
                                        var w = _activationTargetWindow;
                                        bool isClosed = true;
                                        if (w != null)
                                        {
                                            try
                                            {
                                                if (w.IsLoaded)
                                                {
                                                    isClosed = false;
                                                }
                                                else
                                                {
                                                    foreach (Window win in Current.Windows)
                                                    {
                                                        if (win == w)
                                                        {
                                                            isClosed = false;
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                            catch
                                            {
                                                isClosed = true;
                                            }
                                        }

                                        if (w == null || isClosed)
                                        {
                                            bool skipLaunchShell = false;
                                            try
                                            {
                                                skipLaunchShell = new AppSettingsManager().IsSkipLaunchShellEnabled();
                                            }
                                            catch { }

                                            if (skipLaunchShell)
                                            {
                                                var mainWindow = new MainWindow();
                                                Current.MainWindow = mainWindow;
                                                RegisterShowWindowActivationTarget(mainWindow);
                                                w = mainWindow;
                                            }
                                            else
                                            {
                                                var launcher = new QuickLaunchWindow();
                                                Current.MainWindow = launcher;
                                                RegisterShowWindowActivationTarget(launcher);
                                                w = launcher;
                                            }
                                        }

                                        w.WindowState = WindowState.Normal;
                                        w.Show();
                                        w.Activate();
                                        try { w.Focus(); } catch { }
                                    }
                                    catch { /* ignore */ }
                                }));
                            }
                        }
                    }
                    catch
                    {
                        Thread.Sleep(500);
                    }
                }
            });
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            string logPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "Masterstrap_runtime.log");
            System.IO.File.AppendAllText(logPath, $"\n\n{'='*60}\n[{DateTime.Now}] === APP STARTUP ===\n{'='*60}\n");
            LogRunningProcesses(logPath);

            System.IO.File.AppendAllText(logPath, $"Args count: {e.Args.Length}\n");
            foreach (var arg in e.Args)
            {
                System.IO.File.AppendAllText(logPath, $"Arg: {arg}\n");
                Console.WriteLine($"[App] Arg: {arg}");
            }
            string protocolUrl = e.Args.FirstOrDefault(a => ProtocolHandler.IsValidRobloxProtocol(a));
            bool isProtocolLaunch = !string.IsNullOrEmpty(protocolUrl);

            if (isProtocolLaunch)
            {
                bool interceptionEnabled = true;
                try
                {
                    var settings = new AppSettingsManager();
                    interceptionEnabled = settings.IsProtocolInterceptionEnabled();
                }
                catch { }

                if (!interceptionEnabled)
                {
                    System.IO.File.AppendAllText(logPath, $"[App] [IMMEDIATE BYPASS] Interception disabled. URL: {protocolUrl}\n");
                    LaunchAsync(protocolUrl, logPath));
                    this.Shutdown();
                    return;
                }
            }
            if (e.Args.Length > 0 && e.Args[0].Contains("debug"))
            {
                System.IO.File.AppendAllText(logPath, "DEBUG MODE: Protocol detected\n");
                if (e.Args.Length > 0)
                {
                    System.IO.File.AppendAllText(logPath, $"Full Protocol URL: {e.Args[0]}\n");
                    MessageBox.Show(
                        "=== PROTOCOL RECEIVED ===\n\n" +
                        $"Full URL:\n{e.Args[0]}\n\n" +
                        "Check temp log:\n" +
                        "C:\\Users\\%USERNAME%\\AppData\\Local\\Temp\\Masterstrap_runtime.log",
                        "Masterstrap Debug",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                this.Shutdown();
                return;
            }

            System.IO.File.AppendAllText(logPath, "Registering protocol handlers...\n");
            Console.WriteLine("[App] Registering protocol handlers...");

            System.Security.Principal.WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            System.Security.Principal.WindowsPrincipal principal = new System.Security.Principal.WindowsPrincipal(identity);
            bool isAdmin = principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
            System.IO.File.AppendAllText(logPath, $"Is Admin: {isAdmin}\n");
            Console.WriteLine($"[App] Is running as Admin: {isAdmin}");

            if (!isAdmin)
            {
                System.IO.File.AppendAllText(logPath, "WARNING: Not running as admin. Protocol registration may fail!\n");
                Console.WriteLine("[App] WARNING: Not running as admin. Protocol registration may fail!");
            }

            AppInitializer.ReregisterProtocolHandler();

            bool isNewInstance = false;
            _mutex = new Mutex(false, AppId, out isNewInstance);
            if (isNewInstance)
            {
                try { _mutex.WaitOne(); } catch { }
            }

            protocolUrl = e.Args.FirstOrDefault(a => ProtocolHandler.IsValidRobloxProtocol(a));
            isProtocolLaunch = !string.IsNullOrEmpty(protocolUrl);
            System.IO.File.AppendAllText(logPath, $"Is protocol launch: {isProtocolLaunch}, URL: {protocolUrl}\n");

            if (!isNewInstance)
            {
                System.IO.File.AppendAllText(logPath, "Another instance running - protocol launch\n");

                if (isProtocolLaunch)
                {
                    bool interceptionEnabled = true;
                    try { interceptionEnabled = new AppSettingsManager().IsProtocolInterceptionEnabled(); } catch { }

                    if (!interceptionEnabled)
                    {
                        System.IO.File.AppendAllText(logPath, $"[App] Interception disabled (Secondary). Bypassing. URL: {protocolUrl}\n");
                        _ = HandleDirectRobloxLaunchAsync(protocolUrl, logPath);
                        try { _mutex?.Dispose(); } catch { }
                        this.Shutdown();
                        return;
                    }

                    try
                    {
                        System.IO.File.AppendAllText(logPath, $"Secondary instance protocol launch: {e.Args[0]}\n");
                        Console.WriteLine($"[App] Secondary instance protocol launch: {e.Args[0]}");

                        string protocolFile = System.IO.Path.Combine(
                            System.IO.Path.GetTempPath(),
                            "Masterstrap_Protocol.txt");
                        System.IO.File.WriteAllText(protocolFile, e.Args[0]);
                        System.IO.File.AppendAllText(logPath, $"Wrote protocol to {protocolFile}\n");

                        try
                        {
                            EventWaitHandle signalEvent = EventWaitHandle.OpenExisting("Masterstrap_ProtocolReceived");
                            signalEvent.Set();
                            signalEvent.Dispose();
                            System.IO.File.AppendAllText(logPath, "Signaled primary instance via event\n");
                        }
                        catch (Exception ex)
                        {
                            System.IO.File.AppendAllText(logPath, $"Could not signal primary: {ex.Message}\n");
                        }
                    }
                    catch (Exception ex)
                    {
                        System.IO.File.AppendAllText(logPath, $"Error handling secondary protocol: {ex.Message}\n");
                        Console.WriteLine($"[App] Error: {ex.Message}");
                    }
                }
                else
                {
                    try
                    {
                        EventWaitHandle showEvent = EventWaitHandle.OpenExisting(ShowWindowEventName);
                        showEvent.Set();
                        showEvent.Dispose();
                        System.Threading.Thread.Sleep(200);
                    }
                    catch { }
                }

                System.IO.File.AppendAllText(logPath, "Shutting down secondary instance\n");

                try
                {
                    _mutex?.Dispose();
                    _mutex = null;
                }
                catch { }

                this.Shutdown();
                return;
            }

            System.IO.File.AppendAllText(logPath, "Primary instance\n");

            try
            {
                _showWindowEvent = new EventWaitHandle(false, EventResetMode.ManualReset, ShowWindowEventName);
            }
            catch { }

            if (isProtocolLaunch)
            {
                if (Application.Current != null)
                {
                    Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                }

                System.IO.File.AppendAllText(logPath, $"Primary instance protocol launch: {e.Args[0]}\n");
                Console.WriteLine($"[App] Primary instance protocol launch: {e.Args[0]}");


                System.IO.File.AppendAllText(logPath, "Calling base.OnStartup() to initialize Dispatcher\n");
                base.OnStartup(e);

                this.Dispatcher.BeginInvoke(async () =>
                {
                    ShutdownMode? originalShutdownMode = null;
                    try
                    {
                        string normalizedProtocolUrl = ProtocolHandler.ExtractLaunchUrl(e.Args[0]);
                        if (string.IsNullOrWhiteSpace(normalizedProtocolUrl))
                            normalizedProtocolUrl = e.Args[0];
                        string protocolUrlForLaunch = e.Args[0];

                        originalShutdownMode = ShutdownMode.OnExplicitShutdown;
                        if (Application.Current != null)
                            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

                        System.IO.File.AppendAllText(logPath, "[App] Using unified Roblox installation manager...\n");
                        var (success, robloxExePath) = await RobloxInstallationManager.EnsureRobloxInstalledAsync(
                            logCallback: (msg) =>
                            {
                                System.IO.File.AppendAllText(logPath, $"{msg}\n");
                                Console.WriteLine(msg);
                            },
                            updateStatusCallback: (status) =>
                            {
                                System.IO.File.AppendAllText(logPath, $"[Status] {status}\n");
                            }
                        );

                        if (!success || string.IsNullOrEmpty(robloxExePath))
                        {
                            System.IO.File.AppendAllText(logPath, "[App] ERROR: Roblox installation failed\n");
                            this.Dispatcher.BeginInvoke(() =>
                            {
                                MessageBox.Show(
                                    "❌ Failed to check or install Roblox\n\n" +
                                    "Please check your internet connection and try again.",
                                    "Roblox Setup Failed",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                                try
                                {
                                    if (Application.Current != null && originalShutdownMode.HasValue)
                                        Application.Current.ShutdownMode = originalShutdownMode.Value;
                                    Application.Current?.Shutdown();
                                }
                                catch { }
                            });
                            return;
                        }

                        System.IO.File.AppendAllText(logPath, $"[App] Roblox ready at: {robloxExePath}\n");
                        bool launchOnlyNoApply = false;
                        try { launchOnlyNoApply = !(new Masterstrap.Services.AppSettingsManager().IsAllowManageFastFlagsEnabled()); } catch { }

                        await this.Dispatcher.InvokeAsync(() => { }, System.Windows.Threading.DispatcherPriority.ApplicationIdle);
                        await Task.Delay(200);

                        this.Dispatcher.BeginInvoke(() =>
                        {
                            bool allowManageFastFlags = !launchOnlyNoApply;
                            Func<Task<bool>> applyFlagsWhenDetected = null;
                            try
                            {
                                applyFlagsWhenDetected = null;
                            }
                            catch { }

                            try
                            {
                                System.IO.File.AppendAllText(logPath, $"[App] Launch protocolUrl: {normalizedProtocolUrl}\n");
                            }
                            catch { }

                            var launchTarget = !string.IsNullOrEmpty(protocolUrl) ? protocolUrl : robloxExePath;
                            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                            {
                                FileName = launchTarget,
                                UseShellExecute = true
                            });
                            System.IO.File.AppendAllText(logPath, "[App] Roblox launch started\n");
                        });
                    }
                    catch (Exception ex)
                    {
                        System.IO.File.AppendAllText(logPath, $"[App] Error in protocol launch: {ex.Message}\n");
                        System.IO.File.AppendAllText(logPath, $"[App] Stack: {ex.StackTrace}\n");
                        try
                        {
                            if (Application.Current != null)
                            {
                                if (originalShutdownMode.HasValue)
                                    Application.Current.ShutdownMode = originalShutdownMode.Value;
                                Application.Current.Shutdown();
                            }
                        }
                        catch { }
                    }
                });

                try
                {
                    EventWaitHandle protocolEvent = new EventWaitHandle(false, EventResetMode.ManualReset, "Masterstrap_ProtocolReceived");
                    System.IO.File.AppendAllText(logPath, "[App] Created protocol event listener in protocol flow\n");
                    Task.Run(() => ListenForProtocolEvents(logPath));
                }
                catch (Exception ex)
                {
                    System.IO.File.AppendAllText(logPath, $"[App] Failed to create protocol listener in protocol flow: {ex.Message}\n");
                }

                EnsureSecondInstanceShowWindowListener();

                return;
            }

            System.IO.File.AppendAllText(logPath, "No protocol - normal startup\n");

            base.OnStartup(e);


            bool skipLaunchShell = e.Args.Any(a =>
                string.Equals(a, "--main", StringComparison.OrdinalIgnoreCase)
                || string.Equals(a, "/main", StringComparison.OrdinalIgnoreCase));
            bool launchAndApplyFlagsShortcut = e.Args.Any(a =>
                string.Equals(a, "--launch-and-apply-flags", StringComparison.OrdinalIgnoreCase)
                || string.Equals(a, "/launch-and-apply-flags", StringComparison.OrdinalIgnoreCase)
                || string.Equals(a, "--save-and-launch", StringComparison.OrdinalIgnoreCase)
                || string.Equals(a, "/save-and-launch", StringComparison.OrdinalIgnoreCase));
            if (launchAndApplyFlagsShortcut)
                skipLaunchShell = true;
            if (!skipLaunchShell)
            {
                try
                {
                    skipLaunchShell = new AppSettingsManager().IsSkipLaunchShellEnabled();
                }
                catch { /* ignore */ }
            }

            if (launchAndApplyFlagsShortcut)
            {
                if (Application.Current != null)
                    Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

                EnsureSecondInstanceShowWindowListener();

                Application.Current?.Dispatcher.BeginInvoke(new Action(async () =>
                {
                    bool started = await SavedSettingsLaunchService.LaunchFromDesktopShortcutAsync();
                    if (!started)
                    {
                        try { Application.Current?.Shutdown(); }
                        catch { /* ignore */ }
                    }
                }));

                try
                {
                    EventWaitHandle protocolEvent = new EventWaitHandle(false, EventResetMode.ManualReset, "Masterstrap_ProtocolReceived");
                    System.IO.File.AppendAllText(logPath, "Created protocol event listener (shortcut launch)\n");
                    Task.Run(() => ListenForProtocolEvents(logPath));
                }
                catch (Exception ex)
                {
                    System.IO.File.AppendAllText(logPath, $"Failed to create protocol listener: {ex.Message}\n");
                }

                System.IO.File.AppendAllText(logPath, "Desktop shortcut launch flow started\n");
                return;
            }

            if (skipLaunchShell)
            {
                var mainWindow = new MainWindow();
                Application.Current.MainWindow = mainWindow;
                mainWindow.Show();
            }
            else
            {
                var launcher = new QuickLaunchWindow();
                Application.Current.MainWindow = launcher;
                RegisterShowWindowActivationTarget(launcher);
                EnsureSecondInstanceShowWindowListener();
                launcher.Show();
            }

            try
            {
                EventWaitHandle protocolEvent = new EventWaitHandle(false, EventResetMode.ManualReset, "Masterstrap_ProtocolReceived");
                System.IO.File.AppendAllText(logPath, "Created protocol event listener\n");

                Task.Run(() => ListenForProtocolEvents(logPath));
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(logPath, $"Failed to create protocol listener: {ex.Message}\n");
            }

            System.IO.File.AppendAllText(logPath, "Normal startup complete\n");
        }

        private void ListenForProtocolEvents(string logPath)
        {
            try
            {
                while (true)
                {
                    using (EventWaitHandle protocolEvent = EventWaitHandle.OpenExisting("Masterstrap_ProtocolReceived"))
                    {
                        protocolEvent.WaitOne();

                        System.IO.File.AppendAllText(logPath, "Protocol event received from secondary instance\n");
                        Console.WriteLine("[App] Protocol event received!");

                        string protocolFile = System.IO.Path.Combine(
                            System.IO.Path.GetTempPath(),
                            "Masterstrap_Protocol.txt");

                        if (System.IO.File.Exists(protocolFile))
                        {
                            try
                            {
                                string protocolUrl = System.IO.File.ReadAllText(protocolFile).Trim();
                                System.IO.File.AppendAllText(logPath, $"Read protocol from file: {protocolUrl}\n");

                                try { protocolEvent.Reset(); } catch { }

                                Task.Run(async () =>
                            {
                                try
                                {
                                    bool interceptionEnabled = true;
                                    try { interceptionEnabled = new AppSettingsManager().IsProtocolInterceptionEnabled(); } catch { }

                                    if (!interceptionEnabled)
                                    {
                                        System.IO.File.AppendAllText(logPath, "[App] Interception disabled in listener. Bypassing...\n");
                                        await HandleDirectRobloxLaunchAsync(protocolUrl, logPath);
                                        return;
                                    }

                                    string normalizedProtocolUrl = ProtocolHandler.ExtractLaunchUrl(protocolUrl);
                                    if (string.IsNullOrWhiteSpace(normalizedProtocolUrl))
                                        normalizedProtocolUrl = protocolUrl;
                                    string protocolUrlForLaunch = protocolUrl;

                                    bool launchOnlyNoApply = false;
                                    try { launchOnlyNoApply = !(new Masterstrap.Services.AppSettingsManager().IsAllowManageFastFlagsEnabled()); } catch { }

                                    ShutdownMode? originalShutdownMode = null;

                                    var installTask = await this.Dispatcher.InvokeAsync(() =>
                                    {
                                        if (Application.Current != null)
                                        {
                                            originalShutdownMode = Application.Current.ShutdownMode;
                                            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                                        }

                                        return RobloxInstallationManager.EnsureRobloxInstalledAsync(
                                            logCallback: (msg) =>
                                            {
                                                try { System.IO.File.AppendAllText(logPath, $"{msg}\n"); } catch { }
                                            },
                                            updateStatusCallback: (status) =>
                                            {
                                                try { System.IO.File.AppendAllText(logPath, $"[Status] {status}\n"); } catch { }
                                            });
                                    });

                                    var (success, robloxExePath) = await installTask;
                                    if (!success || string.IsNullOrEmpty(robloxExePath))
                                        return;

                                    this.Dispatcher.Invoke(() =>
                                    {
                                        bool allowManageFastFlags = !launchOnlyNoApply;
                                        Func<Task<bool>> applyFlagsWhenDetected = null;
                                        try
                                        {
                                            applyFlagsWhenDetected = null;
                                        }
                                        catch { }

                                        var launchTarget = !string.IsNullOrEmpty(protocolUrlForLaunch) ? protocolUrlForLaunch : robloxExePath;
                                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                                        {
                                            FileName = launchTarget,
                                            UseShellExecute = true
                                        });
                                        try { System.IO.File.AppendAllText(logPath, "Roblox launch started from listener\n"); } catch { }
                                        try { System.IO.File.AppendAllText(logPath, $"[App] Listener Launch protocolUrl: {normalizedProtocolUrl}\n"); } catch { }
                                    });
                                }
                                catch
                                {
                                }
                            });
                            }
                            catch (Exception ex)
                            {
                                System.IO.File.AppendAllText(logPath, $"Error reading protocol file: {ex.Message}\n");
                                try { protocolEvent.Reset(); } catch { }
                            }
                        }
                        else
                        {
                            try { protocolEvent.Reset(); } catch { }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(logPath, $"Protocol listener error: {ex.Message}\n");
            }
        }

        private async Task HandleDirectRobloxLaunchAsync(string protocolUrl, string logPath)
        {
            try
            {
                var (success, robloxExePath) = await RobloxInstallationManager.EnsureRobloxInstalledAsync(
                    logCallback: (msg) => { try { System.IO.File.AppendAllText(logPath, msg + "\n"); } catch { } },
                    updateStatusCallback: (status) => { },
                    silent: true
                );

                if (success && !string.IsNullOrEmpty(robloxExePath))
                {
                    Process.Start(new ProcessStartInfo(robloxExePath, protocolUrl) { UseShellExecute = true });
                    try { System.IO.File.AppendAllText(logPath, "[App] Direct launch successful.\n"); } catch { }
                }
            }
            catch (Exception ex)
            {
                try { System.IO.File.AppendAllText(logPath, $"[App] Direct launch failed: {ex.Message}\n"); } catch { }
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            string logPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "Masterstrap_runtime.log");
            System.IO.File.AppendAllText(logPath, $"\n[{DateTime.Now}] === APP EXIT ===\n");

            LogRunningProcesses(logPath);

            if (_mutex != null)
            {
                try { _mutex.ReleaseMutex(); } catch { }
                _mutex.Dispose();
            }
            if (_showWindowEvent != null)
            {
                _showWindowEvent.Dispose();
            }
            base.OnExit(e);
        }

        private void LogRunningProcesses(string logPath)
        {
            try
            {
                System.IO.File.AppendAllText(logPath, "\n--- Running Processes ---\n");

                Process[] mffProcesses = Process.GetProcessesByName("Masterstrap");
                System.IO.File.AppendAllText(logPath, $"Masterstrap processes: {mffProcesses.Length}\n");
                foreach (var proc in mffProcesses)
                {
                    try
                    {
                        System.IO.File.AppendAllText(logPath,
                            $"  - PID: {proc.Id}, Name: {proc.ProcessName}, Started: {proc.StartTime}, Memory: {proc.WorkingSet64 / 1024 / 1024}MB\n");
                    }
                    catch (Exception lineEx)
                    {
                        System.IO.File.AppendAllText(logPath,
                            $"  - PID: {proc.Id}, Name: {proc.ProcessName}, (details skipped: {lineEx.Message})\n");
                    }
                }

                Process[] robloxProcesses = Process.GetProcessesByName("RobloxPlayerBeta");
                System.IO.File.AppendAllText(logPath, $"RobloxPlayerBeta processes: {robloxProcesses.Length}\n");
                foreach (var proc in robloxProcesses)
                {
                    try
                    {
                        System.IO.File.AppendAllText(logPath,
                            $"  - PID: {proc.Id}, Name: {proc.ProcessName}, Started: {proc.StartTime}, Memory: {proc.WorkingSet64 / 1024 / 1024}MB\n");
                    }
                    catch (Exception lineEx)
                    {
                        System.IO.File.AppendAllText(logPath,
                            $"  - PID: {proc.Id}, Name: {proc.ProcessName}, (details skipped: {lineEx.Message})\n");
                    }
                }

                System.IO.File.AppendAllText(logPath, "\n--- All Masterstrap Windows ---\n");
                foreach (var proc in mffProcesses)
                {
                    try
                    {
                        System.IO.File.AppendAllText(logPath, $"PID {proc.Id} MainWindowHandle: {proc.MainWindowHandle}, MainWindowTitle: '{proc.MainWindowTitle}'\n");
                    }
                    catch (Exception lineEx)
                    {
                        System.IO.File.AppendAllText(logPath, $"PID {proc.Id} (window info skipped: {lineEx.Message})\n");
                    }
                }
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(logPath, $"Error logging processes: {ex.Message}\n");
            }
        }

        private async Task<string> EnsureRobloxInstalledForProtocolAsync(string logPath)
        {
            try
            {
                System.IO.File.AppendAllText(logPath, "[App] === ENSURE ROBLOX INSTALLED FOR PROTOCOL ===\n");
                string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string versionsDir = System.IO.Path.Combine(appDirectory, "versions");

                System.IO.File.AppendAllText(logPath, "[App] Fetching latest Roblox version from API...\n");
                var versionData = await FetchRobloxVersionDataAsync();
                string latestVersionHash = versionData?.ClientVersionUpload;

                if (string.IsNullOrWhiteSpace(latestVersionHash))
                {
                    System.IO.File.AppendAllText(logPath, "[App] ⚠ API fetch failed, trying to find existing Roblox installation...\n");
                    string existingRoblox = FindExistingRobloxPlayerBeta(versionsDir);
                    if (!string.IsNullOrEmpty(existingRoblox))
                    {
                        System.IO.File.AppendAllText(logPath, $"[App] ✓ Found existing Roblox at: {existingRoblox}\n");
                        return existingRoblox;
                    }
                    System.IO.File.AppendAllText(logPath, "[App] ✗ No existing Roblox found and API fetch failed\n");
                    return null;
                }

                System.IO.File.AppendAllText(logPath, $"[App] ✓ Latest version hash: {latestVersionHash}\n");

                string versionPath = System.IO.Path.Combine(versionsDir, latestVersionHash);
                string robloxExePath = System.IO.Path.Combine(versionPath, "RobloxPlayerBeta.exe");

                if (System.IO.File.Exists(robloxExePath))
                {
                    System.IO.File.AppendAllText(logPath, $"[App] ✓ CASE 2: Roblox {latestVersionHash} already installed\n");
                    System.IO.File.AppendAllText(logPath, $"[App] Skipping install, using existing version\n");
                    return robloxExePath;
                }

                System.IO.File.AppendAllText(logPath, $"[App] ⚠️ CASE 1: Roblox {latestVersionHash} not found - auto-installing...\n");

                System.IO.Directory.CreateDirectory(versionPath);
                System.IO.File.AppendAllText(logPath, $"[App] Install directory: {versionPath}\n");

                System.IO.File.AppendAllText(logPath, $"[App] Starting manifest-based download...\n");
                var downloader = new Masterstrap.Services.RobloxManifestDownloader(
                    latestVersionHash,
                    versionPath,
                    log: (msg) =>
                    {
                        System.IO.File.AppendAllText(logPath, $"[App] {msg}\n");
                        Console.WriteLine($"[App] {msg}");
                    },
                    addActivity: (msg) => { },
                    updateInstallStatus: (status, progress) => { }
                );

                System.IO.File.AppendAllText(logPath, "[App] Starting download and assembly...\n");
                try
                {
                    await downloader.DownloadAndAssembleRobloxAsync();
                    System.IO.File.AppendAllText(logPath, "[App] ✓ Download and assembly completed successfully!\n");
                }
                catch (Exception downloadEx)
                {
                    System.IO.File.AppendAllText(logPath, $"[App] ✗ Download/Assembly error: {downloadEx.Message}\n");
                    System.IO.File.AppendAllText(logPath, $"[App] ✗ Stack: {downloadEx.StackTrace}\n");
                    throw;
                }

                System.IO.File.AppendAllText(logPath, "[App] ✓ Roblox installation complete!\n");

                if (!System.IO.File.Exists(robloxExePath))
                {
                    System.IO.File.AppendAllText(logPath, "[App] ✗ RobloxPlayerBeta.exe not found after installation\n");
                    System.IO.File.AppendAllText(logPath, $"[App] Checking if file exists at: {robloxExePath}\n");
                    System.IO.File.AppendAllText(logPath, $"[App] Version directory exists: {System.IO.Directory.Exists(versionPath)}\n");
                    if (System.IO.Directory.Exists(versionPath))
                    {
                        System.IO.File.AppendAllText(logPath, $"[App] Files in version directory:\n");
                        foreach (var file in System.IO.Directory.GetFiles(versionPath, "*", System.IO.SearchOption.TopDirectoryOnly))
                        {
                            System.IO.File.AppendAllText(logPath, $"[App]   - {System.IO.Path.GetFileName(file)}\n");
                        }
                    }
                    return null;
                }

                System.IO.File.AppendAllText(logPath, $"[App] ✓ Installation verified: {robloxExePath}\n");
                return robloxExePath;
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(logPath, $"[App] ✗ Error in EnsureRobloxInstalledForProtocolAsync: {ex.Message}\n");
                System.IO.File.AppendAllText(logPath, $"[App] Stack: {ex.StackTrace}\n");
                Console.WriteLine($"[App] ✗ Error: {ex.Message}");
                return null;
            }
        }

        private string FindExistingRobloxPlayerBeta(string versionsDir)
        {
            try
            {
                if (!Directory.Exists(versionsDir))
                    return null;

                var versionDirs = Directory.GetDirectories(versionsDir);
                if (versionDirs.Length == 0)
                    return null;

                Array.Sort(versionDirs, (a, b) => string.Compare(b, a));

                foreach (var versionDir in versionDirs)
                {
                    string exePath = Path.Combine(versionDir, "RobloxPlayerBeta.exe");
                    if (File.Exists(exePath))
                    {
                        Console.WriteLine($"[App] Found RobloxPlayerBeta.exe at: {exePath}");
                        return exePath;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[App] Error searching for existing Roblox: {ex.Message}");
                return null;
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
            string logPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "Masterstrap_runtime.log");
            try
            {
                using (var client = new HttpClient { Timeout = TimeSpan.FromSeconds(15) })
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");

                    string versionUrl = "https://clientsettings.roblox.com/v2/client-version/WindowsPlayer";
                    System.IO.File.AppendAllText(logPath, $"[App] HTTP GET: {versionUrl}\n");
                    Console.WriteLine($"[App] HTTP GET: {versionUrl}");

                    string jsonResponse = await client.GetStringAsync(versionUrl);
                    System.IO.File.AppendAllText(logPath, $"[App] ✓ Response received\n");
                    System.IO.File.AppendAllText(logPath, $"[App] Raw JSON: {jsonResponse}\n");
                    Console.WriteLine($"[App] ✓ Response received");

                    using (var doc = JsonDocument.Parse(jsonResponse))
                    {
                        var root = doc.RootElement;

                        System.IO.File.AppendAllText(logPath, $"[App] JSON fields found:\n");
                        foreach (var property in root.EnumerateObject())
                        {
                            string value = property.Value.ValueKind == JsonValueKind.String
                                ? property.Value.GetString()
                                : property.Value.ToString();
                            System.IO.File.AppendAllText(logPath, $"[App]   - {property.Name}: {value}\n");
                        }

                        if (root.TryGetProperty("clientVersionUpload", out JsonElement clientVersionUpload))
                        {
                            string hash = clientVersionUpload.GetString();
                            System.IO.File.AppendAllText(logPath, $"[App] ✓ Extracted clientVersionUpload: {hash}\n");
                            Console.WriteLine($"[App] ✓ Extracted clientVersionUpload: {hash}");

                            if (string.IsNullOrWhiteSpace(hash))
                            {
                                throw new Exception("clientVersionUpload value is empty");
                            }

                            string version = "";
                            if (root.TryGetProperty("version", out JsonElement versionProp))
                            {
                                version = versionProp.GetString() ?? "";
                            }

                            string bootstrapperVersion = "";
                            if (root.TryGetProperty("bootstrapperVersion", out JsonElement bootstrapProp))
                            {
                                bootstrapperVersion = bootstrapProp.GetString() ?? "";
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
                            System.IO.File.AppendAllText(logPath, $"[App] ✗ Field 'clientVersionUpload' not found in JSON response\n");
                            throw new Exception("clientVersionUpload field not found in API response");
                        }
                    }
                }
            }
            catch (HttpRequestException httpEx)
            {
                System.IO.File.AppendAllText(logPath, $"[App] ✗ HTTP Error: {httpEx.Message}\n");
                System.IO.File.AppendAllText(logPath, $"[App] ✗ Status Code: {httpEx.StatusCode}\n");
                Console.WriteLine($"[App] ✗ HTTP Error: {httpEx.Message}");
                throw;
            }
            catch (JsonException jsonEx)
            {
                System.IO.File.AppendAllText(logPath, $"[App] ✗ JSON Parse Error: {jsonEx.Message}\n");
                Console.WriteLine($"[App] ✗ JSON Parse Error: {jsonEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(logPath, $"[App] ✗ Error fetching version: {ex.Message}\n");
                System.IO.File.AppendAllText(logPath, $"[App] ✗ Stack: {ex.StackTrace}\n");
                Console.WriteLine($"[App] ✗ Error fetching version: {ex.Message}");
                throw;
            }
        }

        public static void KillAllOtherInstances()
        {
            try
            {
                int currentPid = Process.GetCurrentProcess().Id;
                Process[] mffProcesses = Process.GetProcessesByName("Masterstrap");

                foreach (var proc in mffProcesses)
                {
                    if (proc.Id != currentPid)
                    {
                        proc.Kill();
                        Console.WriteLine($"[App] Killed Masterstrap process {proc.Id}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[App] Error killing processes: {ex.Message}");
            }
        }
    }
}
