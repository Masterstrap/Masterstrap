using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.Net;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Linq;
using Masterstrap.Services;

namespace Masterstrap
{
    public partial class MainWindow
    {
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern bool SHObjectProperties(IntPtr hwnd, uint shopObjectType, string pszObjectName, string pszPropertyPage);

        private const uint SHOP_FILEPATH = 0x00000002;
        private static readonly HttpClient _modsHttpClient = new HttpClient();
        private bool _isInitializingModsTab;

        internal class ModsUiSettings
        {
            public string CursorType { get; set; } = "Default";
            public bool OldAvatarEditorBackground { get; set; }
            public bool OldCharacterSounds { get; set; }
            public string EmojiType { get; set; } = "Default";
            public string CustomFontPath { get; set; } = "";

            public string SkyboxName { get; set; } = "Default";
            public bool Fullbright { get; set; }
            public bool LightingOverlays { get; set; }
            public bool MotionBlur { get; set; }
            public bool FpsCounter { get; set; }
            public bool ServerDetails { get; set; }
            public double Brightness { get; set; } = 50.0;
            public string ShiftlockName { get; set; } = "Default";
        }

        private string GetModsSettingsPath()
        {
            string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Masterstrap");
            Directory.CreateDirectory(appDataPath);
            return Path.Combine(appDataPath, "mods-settings.json");
        }

        private async void ModsForceApplyBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var settings = this.CaptureModsSettingsFromUi();
                this.SaveModsSettings(settings);

                string root = this.GetRobloxVersionRoot();
                if (string.IsNullOrWhiteSpace(root))
                {
                    string errorMsg = "Could not find Roblox installation directory in 'versions/' folder. Please ensure you have launched Roblox at least once through Masterstrap.";
                    this.Log($"[Mods] Error: {errorMsg}");
                    MessageBox.Show(errorMsg, "Masterstrap Mods", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                this.Log("[Mods] Manually applying modifications...");

                if (!string.IsNullOrWhiteSpace(settings.CustomFontPath) && File.Exists(settings.CustomFontPath))
                {
                    await RobloxFontModService.ApplyCustomFontAsync(root, settings.CustomFontPath, msg => this.Log(msg));
                }

                await CommunityModService.ApplySkyboxPresetAsync(settings.SkyboxName, root, msg => this.Log(msg));

                await CommunityModService.ApplyFullbrightAsync(settings.Fullbright, root, msg => this.Log(msg));

                await CursorModService.ApplyCursorPresetAsync(settings.CursorType, root, msg => this.Log(msg));
                await CursorModService.ApplyShiftlockPresetAsync(settings.ShiftlockName, root, msg => this.Log(msg));

                this.Log("[Mods] Manual application complete.");
                MessageBox.Show("Modifications have been applied to your Roblox installation successfully.\n\nNote: If Roblox is currently open, please restart it for some changes to take effect.",
                                "Masterstrap Mods", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                this.Log($"[Mods] Manual apply failed: {ex.Message}");
                MessageBox.Show($"Failed to apply modifications: {ex.Message}", "Masterstrap Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private ModsUiSettings LoadModsSettings()
        {
            try
            {
                string path = this.GetModsSettingsPath();
                if (!File.Exists(path))
                    return new ModsUiSettings();
                string json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<ModsUiSettings>(json) ?? new ModsUiSettings();
            }
            catch
            {
                return new ModsUiSettings();
            }
        }

        private void SaveModsSettings(ModsUiSettings settings)
        {
            try
            {
                string path = this.GetModsSettingsPath();
                string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(path, json);
            }
            catch (Exception ex)
            {
                this.Log($"[Mods] Failed to save settings: {ex.Message}");
            }
        }

        private void MarkModsDirty()
        {
            if (this._isInitializingModsTab || !this._initializationComplete)
                return;
            this.HasUnsavedChanges = true;
        }

        public void SaveModsSettingsFromUi()
        {
            try
            {
                this.SaveModsSettings(this.CaptureModsSettingsFromUi());
            }
            catch (Exception ex)
            {
                this.Log($"[Mods] Save from UI failed: {ex.Message}");
            }
        }

        public async Task ApplyCursorAndShiftlockFromUiAsync()
        {
            string root = this.GetRobloxVersionRoot();
            if (string.IsNullOrWhiteSpace(root))
            {
                this.Log("[Mods] Cursor/shiftlock apply skipped: Roblox install path not found.");
                return;
            }

            var settings = this.CaptureModsSettingsFromUi();
            await CursorModService.ApplyCursorPresetAsync(settings.CursorType, root, msg => this.Log(msg));
            await CursorModService.ApplyShiftlockPresetAsync(settings.ShiftlockName, root, msg => this.Log(msg));
            this.UpdateShiftlockPreview(settings.ShiftlockName);
        }

        private ModsUiSettings CaptureModsSettingsFromUi()
        {
            return new ModsUiSettings
            {
                CursorType = this.GetCanonicalCursorType(),
                OldAvatarEditorBackground = this.ModsOldAvatarToggle?.IsChecked ?? false,
                OldCharacterSounds = this.ModsOldSoundsToggle?.IsChecked ?? false,
                EmojiType = this.GetCanonicalEmojiType(),
                CustomFontPath = this.ModsChooseFontBtn?.Tag as string ?? "",

                SkyboxName = (this.ModsSkyboxComboBox?.SelectedItem as ComboBoxItem)?.Tag as string ?? "Default",
                Fullbright = this.ModsFullbrightToggle?.IsChecked ?? false,
                LightingOverlays = this.ModsLightingOverlaysToggle?.IsChecked ?? false,
                MotionBlur = this.ModsMotionBlurToggle?.IsChecked ?? false,
                FpsCounter = this.ModsFpsCounterToggle?.IsChecked ?? false,
                ServerDetails = this.ModsServerDetailsToggle?.IsChecked ?? false,
                Brightness = this.ModsBrightnessSlider?.Value ?? 50.0,
                ShiftlockName = (this.ModsShiftlockComboBox?.SelectedItem as ComboBoxItem)?.Tag as string ?? "Default"
            };
        }

        private void InitializeModsTab()
        {
            this._isInitializingModsTab = true;
            try
            {
                var settings = this.LoadModsSettings();
                this.SetCursorComboByCanonical(settings.CursorType);
                if (this.ModsOldAvatarToggle != null) this.ModsOldAvatarToggle.IsChecked = settings.OldAvatarEditorBackground;
                if (this.ModsOldSoundsToggle != null) this.ModsOldSoundsToggle.IsChecked = settings.OldCharacterSounds;
                this.SetEmojiComboByCanonical(settings.EmojiType);
                if (this.ModsChooseFontBtn != null) this.ModsChooseFontBtn.Tag = settings.CustomFontPath ?? "";

                if (this.ModsSkyboxComboBox != null)
                {
                    bool found = false;
                    string targetSkybox = string.IsNullOrWhiteSpace(settings.SkyboxName) ? "Default" : settings.SkyboxName;

                    foreach (ComboBoxItem item in this.ModsSkyboxComboBox.Items)
                    {
                        if (string.Equals(item.Tag as string, targetSkybox, StringComparison.OrdinalIgnoreCase))
                        {
                            this.ModsSkyboxComboBox.SelectedItem = item;
                            found = true;
                            break;
                        }
                    }

                    if (!found && this.ModsSkyboxComboBox.Items.Count > 0)
                    {
                        var defaultItem = this.ModsSkyboxComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(i => string.Equals(i.Tag as string, "Default", StringComparison.OrdinalIgnoreCase));
                        if (defaultItem != null)
                            this.ModsSkyboxComboBox.SelectedItem = defaultItem;
                        else
                            this.ModsSkyboxComboBox.SelectedIndex = 0;
                    }
                }
                if (this.ModsFullbrightToggle != null) this.ModsFullbrightToggle.IsChecked = settings.Fullbright;
                if (this.ModsLightingOverlaysToggle != null) this.ModsLightingOverlaysToggle.IsChecked = settings.LightingOverlays;
                if (this.ModsMotionBlurToggle != null) this.ModsMotionBlurToggle.IsChecked = settings.MotionBlur;
                if (this.ModsFpsCounterToggle != null) this.ModsFpsCounterToggle.IsChecked = settings.FpsCounter;
                if (this.ModsServerDetailsToggle != null) this.ModsServerDetailsToggle.IsChecked = settings.ServerDetails;
                if (this.ModsBrightnessSlider != null) this.ModsBrightnessSlider.Value = settings.Brightness;

                this.PopulateCursorAndShiftlockCombos(settings);
                this.UpdateShiftlockPreview(settings.ShiftlockName);

                this.UpdateCustomFontButtons();
                this.UpdateOverlayState();
            }
            finally
            {
                this._isInitializingModsTab = false;
            }
        }

        private void PopulateCursorAndShiftlockCombos(ModsUiSettings settings)
        {
            if (this.ModsCursorTypeComboBox != null)
            {
                this.ModsCursorTypeComboBox.Items.Clear();
                foreach (var preset in CursorModService.GetAvailableCursorPresets())
                {
                    this.ModsCursorTypeComboBox.Items.Add(new ComboBoxItem { Content = preset, Tag = preset });
                }
                this.SetCursorComboByCanonical(settings.CursorType);
            }

            if (this.ModsShiftlockComboBox != null)
            {
                this.RefreshShiftlockComboItems();
                this.SetShiftlockComboByCanonical(settings.ShiftlockName);
            }
        }

        private void RefreshShiftlockComboItems()
        {
            if (this.ModsShiftlockComboBox == null)
                return;

            this.ModsShiftlockComboBox.Items.Clear();
            foreach (var preset in CursorModService.GetAvailableShiftlockPresets())
            {
                this.ModsShiftlockComboBox.Items.Add(new ComboBoxItem { Content = preset, Tag = preset });
            }

            _ = CursorModService.EnsureMasterstrapShiftlockPngAsync(msg => this.Log(msg));
        }

        private void SetShiftlockComboByCanonical(string value)
        {
            if (this.ModsShiftlockComboBox == null) return;
            string canonical = value ?? "Default";
            foreach (ComboBoxItem item in this.ModsShiftlockComboBox.Items)
            {
                if (string.Equals(item.Tag as string, canonical, StringComparison.OrdinalIgnoreCase))
                {
                    this.ModsShiftlockComboBox.SelectedItem = item;
                    return;
                }
            }
            this.ModsShiftlockComboBox.SelectedIndex = 0;
        }

        private string GetComboItemText(ComboBox comboBox)
        {
            if (comboBox?.SelectedItem is ComboBoxItem cbi)
                return cbi.Content?.ToString() ?? "Default";
            return "Default";
        }

        private void SetComboByText(ComboBox comboBox, string value)
        {
            if (comboBox == null)
                return;
            value ??= "Default";
            foreach (var item in comboBox.Items)
            {
                if (item is ComboBoxItem cbi &&
                    string.Equals(cbi.Content?.ToString(), value, StringComparison.OrdinalIgnoreCase))
                {
                    comboBox.SelectedItem = cbi;
                    return;
                }
            }
            comboBox.SelectedIndex = 0;
        }

        private void UpdateCustomFontButtons()
        {
            if (this.ModsChooseFontBtn == null || this.ModsRemoveFontBtn == null)
                return;
            string sourcePath = this.ModsChooseFontBtn.Tag as string ?? "";
            bool hasFont = !string.IsNullOrWhiteSpace(sourcePath);
            this.ModsRemoveFontBtn.IsEnabled = hasFont;
            this.ModsRemoveFontBtn.Opacity = hasFont ? 1.0 : 0.55;
        }

        private string GetRobloxVersionRoot()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(this._robloxExecutablePath) && File.Exists(this._robloxExecutablePath))
                    return Path.GetDirectoryName(this._robloxExecutablePath) ?? "";

                string appVersionsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "versions");
                if (Directory.Exists(appVersionsPath))
                {
                    var latestVersionDir = Directory.GetDirectories(appVersionsPath, "version-*")
                        .OrderByDescending(d => Directory.GetLastWriteTime(d))
                        .FirstOrDefault();

                    if (latestVersionDir != null)
                    {
                        string exePath = Path.Combine(latestVersionDir, "RobloxPlayerBeta.exe");
                        if (File.Exists(exePath))
                        {
                            this._robloxExecutablePath = exePath;
                            return latestVersionDir;
                        }
                    }
                }
            }
            catch { }
            return "";
        }

        private async Task DownloadToFileAsync(string url, string destinationPath)
        {
            using var response = await _modsHttpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            byte[] payload = await response.Content.ReadAsByteArrayAsync();
            string dir = Path.GetDirectoryName(destinationPath) ?? "";
            if (!string.IsNullOrWhiteSpace(dir))
                Directory.CreateDirectory(dir);
            File.WriteAllBytes(destinationPath, payload);
        }

        private static void DeleteIfExists(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }

        private static string GetBackupPath(string targetPath) => targetPath + ".masterstrap.bak";

        private static void BackupIfNeeded(string targetPath)
        {
            try
            {
                string backupPath = GetBackupPath(targetPath);
                if (!File.Exists(targetPath) || File.Exists(backupPath))
                    return;
                Directory.CreateDirectory(Path.GetDirectoryName(backupPath) ?? "");
                File.Copy(targetPath, backupPath, true);
            }
            catch
            {
            }
        }

        private static void RestoreFromBackupOrDelete(string targetPath)
        {
            string backupPath = GetBackupPath(targetPath);
            if (File.Exists(backupPath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(targetPath) ?? "");
                File.Copy(backupPath, targetPath, true);
                File.Delete(backupPath);
                return;
            }

            DeleteIfExists(targetPath);
        }

        private async Task ApplyCursorPresetAsync(string cursorType)
        {
            string root = this.GetRobloxVersionRoot();
            await CursorModService.ApplyCursorPresetAsync(cursorType, root, msg => this.Log(msg));
        }

        private async Task ApplyShiftlockPresetAsync(string shiftlock)
        {
            string root = this.GetRobloxVersionRoot();
            await CursorModService.ApplyShiftlockPresetAsync(shiftlock, root, msg => this.Log(msg));
            this.UpdateShiftlockPreview(shiftlock);
        }

        private void UpdateShiftlockPreview(string shiftlock)
        {
            if (this.ModsShiftlockPreview == null) return;
            try
            {
                if (shiftlock == "Default")
                {
                    this.ModsShiftlockPreview.Source = null;
                    if (this.ModsShiftlockPlusIcon != null) this.ModsShiftlockPlusIcon.Visibility = Visibility.Visible;
                    if (this.ModsDeleteShiftlockBtn != null) this.ModsDeleteShiftlockBtn.Visibility = Visibility.Collapsed;
                    return;
                }

                string path = CursorModService.GetShiftlockImagePath(shiftlock);

                if (File.Exists(path))
                {
                    var bitmap = new System.Windows.Media.Imaging.BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(path);
                    bitmap.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                    bitmap.CreateOptions = System.Windows.Media.Imaging.BitmapCreateOptions.IgnoreImageCache;
                    bitmap.EndInit();
                    this.ModsShiftlockPreview.Source = bitmap;
                    if (this.ModsShiftlockPlusIcon != null) this.ModsShiftlockPlusIcon.Visibility = Visibility.Collapsed;
                    if (this.ModsDeleteShiftlockBtn != null) this.ModsDeleteShiftlockBtn.Visibility = shiftlock == "Custom" ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                {
                    this.ModsShiftlockPreview.Source = null;
                    if (this.ModsShiftlockPlusIcon != null) this.ModsShiftlockPlusIcon.Visibility = Visibility.Visible;
                    if (this.ModsDeleteShiftlockBtn != null) this.ModsDeleteShiftlockBtn.Visibility = Visibility.Collapsed;
                }
            }
            catch
            {
                this.ModsShiftlockPreview.Source = null;
                if (this.ModsShiftlockPlusIcon != null) this.ModsShiftlockPlusIcon.Visibility = Visibility.Visible;
            }
        }

        private string GetCanonicalCursorType()
        {
            if (this.ModsCursorTypeComboBox?.SelectedItem is ComboBoxItem item)
                return (item.Tag as string) ?? "Default";
            return "Default";
        }

        private void SetCursorComboByCanonical(string value)
        {
            if (this.ModsCursorTypeComboBox == null)
                return;

            string canonical = (value ?? "Default").Trim();
            foreach (var rawItem in this.ModsCursorTypeComboBox.Items)
            {
                if (rawItem is ComboBoxItem cbi && string.Equals(cbi.Tag as string, canonical, StringComparison.OrdinalIgnoreCase))
                {
                    this.ModsCursorTypeComboBox.SelectedItem = cbi;
                    return;
                }
            }
            this.ModsCursorTypeComboBox.SelectedIndex = 0;
        }

        private string GetCanonicalEmojiType()
        {
            if (this.ModsEmojiTypeComboBox?.SelectedItem is ComboBoxItem item)
                return (item.Tag as string) ?? "Default";
            return "Default";
        }

        private void SetEmojiComboByCanonical(string value)
        {
            if (this.ModsEmojiTypeComboBox == null)
                return;

            string canonical = (value ?? "Default").Trim();
            foreach (var rawItem in this.ModsEmojiTypeComboBox.Items)
            {
                if (rawItem is ComboBoxItem cbi && string.Equals(cbi.Tag as string, canonical, StringComparison.OrdinalIgnoreCase))
                {
                    this.ModsEmojiTypeComboBox.SelectedItem = cbi;
                    return;
                }
            }
            this.ModsEmojiTypeComboBox.SelectedIndex = 0;
        }

        private async Task ApplyOldAvatarEditorAsync(bool enabled)
        {
            string root = this.GetRobloxVersionRoot();
            if (string.IsNullOrWhiteSpace(root))
                return;
            string target = Path.Combine(root, "ExtraContent", "places", "Mobile.rbxl");
            if (enabled)
            {
                BackupIfNeeded(target);
                await this.DownloadToFileAsync("https://raw.githubusercontent.com/bloxstraplabs/bloxstrap/main/Bloxstrap/Resources/Mods/OldAvatarBackground.rbxl", target);
                return;
            }
            RestoreFromBackupOrDelete(target);
        }

        private async Task ApplyOldCharacterSoundsAsync(bool enabled)
        {
            string root = this.GetRobloxVersionRoot();
            if (string.IsNullOrWhiteSpace(root))
                return;

            string soundsRoot = Path.Combine(root, "content", "sounds");
            var map = new (string Target, string Source)[]
            {
                ("action_footsteps_plastic.mp3", "Sounds.OldWalk.mp3"),
                ("action_jump.mp3", "Sounds.OldJump.mp3"),
                ("action_get_up.mp3", "Sounds.OldGetUp.mp3"),
                ("action_falling.mp3", "Sounds.Empty.mp3"),
                ("action_jump_land.mp3", "Sounds.Empty.mp3"),
                ("action_swim.mp3", "Sounds.Empty.mp3"),
                ("impact_water.mp3", "Sounds.Empty.mp3")
            };

            foreach (var item in map)
            {
                string targetPath = Path.Combine(soundsRoot, item.Target);
                if (enabled)
                {
                    string url = $"https://raw.githubusercontent.com/bloxstraplabs/bloxstrap/main/Bloxstrap/Resources/Mods/{item.Source}";
                    BackupIfNeeded(targetPath);
                    await this.DownloadToFileAsync(url, targetPath);
                }
                else
                {
                    RestoreFromBackupOrDelete(targetPath);
                }
            }
        }

        private async Task ApplyEmojiAsync(string emojiType)
        {
            string root = this.GetRobloxVersionRoot();
            if (string.IsNullOrWhiteSpace(root))
                return;

            string target = Path.Combine(root, "content", "fonts", "TwemojiMozilla.ttf");
            string url = emojiType switch
            {
                "Catmoji" => "https://github.com/bloxstraplabs/rbxcustom-fontemojis/releases/download/my-phone-is-78-percent/Catmoji.ttf",
                "Windows 11" => "https://github.com/bloxstraplabs/rbxcustom-fontemojis/releases/download/my-phone-is-78-percent/Win1122H2SegoeUIEmoji.ttf",
                "Windows 10" => "https://github.com/bloxstraplabs/rbxcustom-fontemojis/releases/download/my-phone-is-78-percent/Win10April2018SegoeUIEmoji.ttf",
                "Windows 8.1" => "https://github.com/bloxstraplabs/rbxcustom-fontemojis/releases/download/my-phone-is-78-percent/Win8.1SegoeUIEmoji.ttf",
                _ => ""
            };

            if (string.IsNullOrWhiteSpace(url))
            {
                RestoreFromBackupOrDelete(target);
                return;
            }

            BackupIfNeeded(target);
            await this.DownloadToFileAsync(url, target);
        }

        private void OpenModsFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string root = this.GetRobloxVersionRoot();
                string modsRoot = string.IsNullOrWhiteSpace(root) ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Masterstrap", "Mods") : root;
                Directory.CreateDirectory(modsRoot);
                Process.Start(new ProcessStartInfo("explorer.exe", $"\"{modsRoot}\"") { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                this.Log($"[Mods] Open folder error: {ex.Message}");
            }
        }

        private void ModsHelpBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://github.com/Masterstrap",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                this.Log($"[Mods] Open help error: {ex.Message}");
            }
        }

        private void ModsFullscreenOptimizationsLink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://devblogs.microsoft.com/directx/demystifying-full-screen-optimizations/",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                this.Log($"[Mods] Open fullscreen optimizations link error: {ex.Message}");
            }
        }

        private void ModsCompatibilityBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string exePath = this._robloxExecutablePath;
                if (string.IsNullOrWhiteSpace(exePath) || !File.Exists(exePath))
                {
                    MessageBox.Show("Roblox is not installed or not detected.", "Compatibility", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                _ = SHObjectProperties(IntPtr.Zero, SHOP_FILEPATH, exePath, "Compatibility");
            }
            catch (Exception ex)
            {
                this.Log($"[Mods] Open compatibility error: {ex.Message}");
            }
        }

        private async void ModsCursorTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this._isInitializingModsTab)
                return;
            try
            {
                await this.ApplyCursorPresetAsync(this.GetCanonicalCursorType());
                this.MarkModsDirty();
            }
            catch (Exception ex)
            {
                this.Log($"[Mods] Cursor preset failed: {ex.Message}");
            }
        }

        private async void ModsOldAvatarToggle_Checked(object sender, RoutedEventArgs e)
        {
            if (this._isInitializingModsTab)
                return;
            try
            {
                await this.ApplyOldAvatarEditorAsync(true);
                this.MarkModsDirty();
            }
            catch (Exception ex)
            {
                this.Log($"[Mods] Old avatar preset failed: {ex.Message}");
            }
        }

        private async void ModsOldAvatarToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            if (this._isInitializingModsTab)
                return;
            try
            {
                await this.ApplyOldAvatarEditorAsync(false);
                this.MarkModsDirty();
            }
            catch (Exception ex)
            {
                this.Log($"[Mods] Old avatar preset failed: {ex.Message}");
            }
        }

        private async void ModsOldSoundsToggle_Checked(object sender, RoutedEventArgs e)
        {
            if (this._isInitializingModsTab)
                return;
            try
            {
                await this.ApplyOldCharacterSoundsAsync(true);
                this.MarkModsDirty();
            }
            catch (Exception ex)
            {
                this.Log($"[Mods] Old sounds preset failed: {ex.Message}");
            }
        }

        private async void ModsOldSoundsToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            if (this._isInitializingModsTab)
                return;
            try
            {
                await this.ApplyOldCharacterSoundsAsync(false);
                this.MarkModsDirty();
            }
            catch (Exception ex)
            {
                this.Log($"[Mods] Old sounds preset failed: {ex.Message}");
            }
        }

        private async void ModsEmojiTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this._isInitializingModsTab)
                return;
            try
            {
                await this.ApplyEmojiAsync(this.GetCanonicalEmojiType());
                this.MarkModsDirty();
            }
            catch (Exception ex)
            {
                this.Log($"[Mods] Emoji preset failed: {ex.Message}");
            }
        }

        private async void ModsChooseFontBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    Filter = "Font files|*.ttf;*.otf;*.ttc"
                };
                if (dialog.ShowDialog() != true)
                    return;

                string extension = Path.GetExtension(dialog.FileName).ToLowerInvariant();
                bool valid = extension is ".ttf" or ".otf" or ".ttc";
                if (!valid)
                {
                    MessageBox.Show("Invalid font file.", "Mods", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string root = this.GetRobloxVersionRoot();
                if (!string.IsNullOrWhiteSpace(root))
                {
                    await Masterstrap.Services.RobloxFontModService.ApplyCustomFontAsync(root, dialog.FileName, this.Log);
                }

                this.ModsChooseFontBtn.Tag = dialog.FileName;
                this.UpdateCustomFontButtons();
                this.MarkModsDirty();
            }
            catch (Exception ex)
            {
                this.Log($"[Mods] Custom font apply failed: {ex.Message}");
            }
        }

        private async void ModsRemoveFontBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string root = this.GetRobloxVersionRoot();
                if (!string.IsNullOrWhiteSpace(root))
                {
                    await Masterstrap.Services.RobloxFontModService.RestoreDefaultFontsAsync(root, this.Log);
                }

                this.ModsChooseFontBtn.Tag = "";
                this.UpdateCustomFontButtons();
                this.MarkModsDirty();
            }
            catch (Exception ex)
            {
                this.Log($"[Mods] Remove custom font failed: {ex.Message}");
            }
        }
        private async void ModsShiftlockComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this._isInitializingModsTab)
                return;
            try
            {
                string shiftlock = (this.ModsShiftlockComboBox.SelectedItem as ComboBoxItem)?.Tag as string ?? "Default";
                await this.ApplyShiftlockPresetAsync(shiftlock);
                this.MarkModsDirty();
            }
            catch (Exception ex)
            {
                this.Log($"[Mods] Shiftlock preset failed: {ex.Message}");
            }
        }

        private async void ModsImportShiftlockBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = "Image Files (*.png;*.jpg;*.jpeg;*.webp;*.bmp)|*.png;*.jpg;*.jpeg;*.webp;*.bmp",
                    Title = "Select Shiftlock Icon (any size — auto-resized to 128x128)"
                };

                if (dialog.ShowDialog() == true)
                {
                    await CursorModService.ImportCustomShiftlockAsync(dialog.FileName, msg => this.Log(msg));

                    this.RefreshShiftlockComboItems();
                    this.SetShiftlockComboByCanonical("Custom");
                    await this.ApplyShiftlockPresetAsync("Custom");
                    this.MarkModsDirty();
                    this.Log("[Mods] Custom Shiftlock icon imported successfully.");
                }
            }
            catch (Exception ex)
            {
                this.Log($"[Mods] Import Shiftlock failed: {ex.Message}");
            }
        }

        private async void ModsDeleteShiftlockBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string customPath = CursorModService.GetShiftlockImagePath("Custom");
                if (File.Exists(customPath))
                    File.Delete(customPath);

                this.RefreshShiftlockComboItems();
                this.SetShiftlockComboByCanonical("Default");
                await this.ApplyShiftlockPresetAsync("Default");
                this.MarkModsDirty();
                if (this.ModsDeleteShiftlockBtn != null) this.ModsDeleteShiftlockBtn.Visibility = Visibility.Collapsed;
                this.Log("[Mods] Custom Shiftlock image removed.");
            }
            catch (Exception ex)
            {
                this.Log($"[Mods] Delete Shiftlock failed: {ex.Message}");
            }
        }

        private async void ModsSkyboxComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this._isInitializingModsTab)
                return;
            try
            {
                string skybox = (this.ModsSkyboxComboBox.SelectedItem as ComboBoxItem)?.Tag as string ?? "Default";
                await this.ApplySkyboxPresetAsync(skybox);
                this.MarkModsDirty();
            }
            catch (Exception ex)
            {
                this.Log($"[Mods] Skybox preset failed: {ex.Message}");
            }
        }

        private async void ModsFullbrightToggle_Checked(object sender, RoutedEventArgs e)
        {
            if (this._isInitializingModsTab)
                return;
            try
            {
                await this.ApplyFullbrightAsync(true);
                this.MarkModsDirty();
            }
            catch (Exception ex)
            {
                this.Log($"[Mods] Fullbright enable failed: {ex.Message}");
            }
        }

        private async void ModsFullbrightToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            if (this._isInitializingModsTab)
                return;
            try
            {
                await this.ApplyFullbrightAsync(false);
                this.MarkModsDirty();
            }
            catch (Exception ex)
            {
                this.Log($"[Mods] Fullbright disable failed: {ex.Message}");
            }
        }

        private void ModsOverlay_Changed(object sender, RoutedEventArgs e)
        {
            if (this._isInitializingModsTab)
                return;
            this.UpdateOverlayState();
            this.MarkModsDirty();
        }

        private void ModsBrightnessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this._isInitializingModsTab)
                return;
            if (this.ModsBrightnessValueText != null)
                this.ModsBrightnessValueText.Text = $"{(int)e.NewValue}%";

            this.UpdateOverlayBrightness(e.NewValue);
            this.MarkModsDirty();
        }

        private async Task ApplySkyboxPresetAsync(string skybox)
        {
            string root = this.GetRobloxVersionRoot();
            await CommunityModService.ApplySkyboxPresetAsync(skybox, root, msg => this.Log(msg));
        }

        private async Task ApplyFullbrightAsync(bool enabled)
        {
            string root = this.GetRobloxVersionRoot();
            await CommunityModService.ApplyFullbrightAsync(enabled, root, msg => this.Log(msg));
        }

        private OverlayWindow _overlayWindow;

        private void UpdateOverlayState()
        {
            var settings = this.CaptureModsSettingsFromUi();
            bool shouldBeVisible = settings.LightingOverlays || settings.MotionBlur || settings.FpsCounter || settings.ServerDetails || Math.Abs(settings.Brightness - 50.0) > 0.01;

            if (shouldBeVisible)
            {
                if (this._overlayWindow == null)
                {
                    this._overlayWindow = new OverlayWindow();
                    this._overlayWindow.Show();
                }
                this._overlayWindow.UpdateFromSettings(settings);
            }
            else if (this._overlayWindow != null)
            {
                this._overlayWindow.Close();
                this._overlayWindow = null;
            }
        }

        private void UpdateOverlayBrightness(double brightness)
        {
            if (this._overlayWindow != null)
            {
                this._overlayWindow.Brightness = brightness;
            }
            else if (brightness != 50.0)
            {
                this.UpdateOverlayState();
            }
        }
    }
}
