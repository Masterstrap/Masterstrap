using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using Masterstrap.Views;
using Masterstrap.Helpers;

namespace Masterstrap
{
    public partial class MainWindow : Window
    {
        private void TabButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button button = sender as Button;
                if (button != null && int.TryParse(button.Tag?.ToString(), out int tabIndex))
                {

                    if ((tabIndex != 1 && tabIndex != 3) && (this.MainTabControl?.SelectedIndex == 1 || this.MainTabControl?.SelectedIndex == 3))
                    {
                        this._editorCategoryFilter = "All";

                        if (this.SearchFlagsBox != null)
                            this.SearchFlagsBox.Text = "";

                        if (this._editableFlagsList != null && this._allFlagsList != null)
                            this.ApplyEditorFlagsFilter();
                    }


                    if (this.MainTabControl != null && tabIndex >= 0 && tabIndex <= 11)
                    {
                        int viewIndex = this.NavToViewTabIndex(tabIndex);
                        this.MainTabControl.SelectedIndex = viewIndex;
                        this.UpdateTabBorderHighlight(tabIndex);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void OpenFastFlagEditor_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.MainTabControl != null)
                {
                    this.MainTabControl.SelectedIndex = 3;
                    this.UpdateTabBorderHighlight(1);
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void UpdateTabBorderHighlight(int activeTabIndex)
        {
            try
            {
                Brush activeFg = this.TryFindResource("NavItemActiveFgBrush") as Brush ?? Brushes.White;
                Brush inactiveFg = this.TryFindResource("NavItemInactiveFgBrush") as Brush
                    ?? new SolidColorBrush(Color.FromRgb(0xA0, 0xA0, 0xA0));
                Brush activeBg = this.TryFindResource("NavButtonHoverBgBrush") as Brush
                    ?? new SolidColorBrush(Color.FromArgb(26, 255, 255, 255));

                void ApplyNav(Border row, Border accentBar, Button btn, bool isActive)
                {
                    if (row != null)
                    {
                        row.Background = isActive ? activeBg : Brushes.Transparent;
                        row.BorderThickness = new Thickness(0);
                        this.UpdateBorderGlow(row, false, false);
                    }

                    if (accentBar != null)
                    {
                        accentBar.Visibility = isActive ? Visibility.Visible : Visibility.Collapsed;
                    }

                    if (btn != null)
                    {
                        btn.Foreground = isActive ? activeFg : inactiveFg;
                    }
                }

                bool home = activeTabIndex == 0;
                bool fastFlag = activeTabIndex == 1 || activeTabIndex == 3;
                bool global = activeTabIndex == 2;
                bool games = activeTabIndex == 4;
                bool mods = activeTabIndex == 5;
                bool proxy = activeTabIndex == 6;
                bool accountManager = activeTabIndex == 7;
                bool linkPs = activeTabIndex == 8;
                bool settings = activeTabIndex == 9;
                bool about = activeTabIndex == 10;
                bool ffSettings = activeTabIndex == 11;

                ApplyNav(this.HomeNavBorder, this.HomeNavAccentBar, this.LeftTabApplyFlagsBtn, home);
                ApplyNav(this.EditBorder, this.EditNavAccentBar, this.LeftTabEditBtn, fastFlag);
                ApplyNav(this.GlobalBorder, this.GlobalNavAccentBar, this.LeftTabGlobalBtn, global);
                ApplyNav(this.GameFFlagsBorder, this.GameFFlagsNavAccentBar, this.LeftTabGameFFlagsBtn, games);
                ApplyNav(this.ModsBorder, this.ModsNavAccentBar, this.LeftTabModsBtn, mods);
                ApplyNav(this.ProxyBorder, this.ProxyNavAccentBar, this.LeftTabProxyBtn, proxy);
                ApplyNav(this.AccountManagerBorder, this.AccountManagerNavAccentBar, this.LeftTabAccountManagerBtn, accountManager);
                ApplyNav(this.LinkPsBorder, this.LinkPsNavAccentBar, this.LeftTabLinkPsBtn, linkPs);
                ApplyNav(this.SettingsBorder, this.SettingsNavAccentBar, this.LeftTabSettingsBtn, settings);
                ApplyNav(this.FaqBorder, this.FaqNavAccentBar, this.LeftTabFaqBtn, about);
                ApplyNav(this.FFSettingsBorder, this.FFSettingsNavAccentBar, this.LeftTabFFSettingsBtn, ffSettings);
            }
            catch (Exception ex)
            {
                this.Log($"[TabBorder] Error updating tab border highlight: {ex.Message}");
            }
        }

        private int NavToViewTabIndex(int navIndex)
        {
            return navIndex switch
            {
                4 => 9,
                5 => 7,
                6 => 8,
                7 => 5,
                8 => 4,
                9 => 6,
                10 => 10,
                11 => 11,
                _ => navIndex
            };
        }

        private int ViewToNavTabIndex(int viewIndex)
        {
            return viewIndex switch
            {
                4 => 8,
                5 => 7,
                6 => 9,
                7 => 5,
                8 => 6,
                9 => 4,
                10 => 10,
                11 => 11,
                _ => viewIndex
            };
        }

        private void BackFromEditor_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.MainTabControl != null)
                {
                    if (this._editableFlagsList != null && this._allFlagsList != null)
                    {
                        this._editableFlagsList.Clear();
                        foreach (var flag in this._allFlagsList)
                        {
                            this._editableFlagsList.Add(flag);
                        }
                        this.UpdateEditStats();
                    }

                    if (this.SearchFlagsBox != null)
                    {
                        this.SearchFlagsBox.Text = "";
                    }

                    this.MainTabControl.SelectedIndex = 1;
                    this.UpdateTabBorderHighlight(1);
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void UpdateBorderGlow(Border border, bool isActive, bool glassUi = false)
        {
            try
            {
                if (border == null)
                    return;

                if (border.Effect is not DropShadowEffect glow)
                {
                    glow = new DropShadowEffect
                    {
                        ShadowDepth = 0,
                        BlurRadius = 16,
                        Color = Color.FromRgb(58, 58, 58),
                        Opacity = 0
                    };
                    border.Effect = glow;
                }

                if (!isActive)
                {
                    glow.Opacity = 0;
                    return;
                }

                if (glassUi)
                {
                    glow.Color = Color.FromRgb(130, 132, 138);
                    glow.BlurRadius = 18;
                    glow.Opacity = 0.28;
                }
                else
                {
                    glow.Color = Color.FromRgb(58, 58, 58);
                    glow.BlurRadius = 16;
                    glow.Opacity = 0.5;
                }
            }
            catch (Exception ex)
            {
                this.Log($"[TabBorder] Error updating border glow: {ex.Message}");
            }
        }

        private bool TryLaunchRobloxWithProgressWindow()
        {
            string robloxExePath = this._robloxExecutablePath;
            if (string.IsNullOrEmpty(robloxExePath))
            {
                RuntimeDiagLog.Append("[SaveAndLaunch] TryLaunch: aborted — _robloxExecutablePath empty (Information System has no Roblox path).");
                this.Log("[Roblox] Roblox path not found");
                this.UpdateStatus("Roblox Not Found", Colors.Red);
                return false;
            }
            if (!File.Exists(robloxExePath))
            {
                RuntimeDiagLog.Append($"[SaveAndLaunch] TryLaunch: aborted — executable missing: {robloxExePath}");
                this.Log($"[Roblox] File not found: {robloxExePath}");
                this.UpdateStatus("Roblox File Not Found", Colors.Red);
                return false;
            }
            string robloxVersionName = ExtractVersionName(this._robloxVersion);
            string softwareVersionName = ExtractVersionName(this._softwareVersion);
            DateTime? robloxLastUpdate = ExtractLastUpdateTime(this._robloxVersion);
            DateTime? softwareLastUpdate = ExtractLastUpdateTime(this._softwareVersion);
            if (!string.IsNullOrEmpty(robloxVersionName) && !string.IsNullOrEmpty(softwareVersionName) &&
                string.Equals(robloxVersionName, softwareVersionName, StringComparison.OrdinalIgnoreCase))
            {
            }
            else if (robloxLastUpdate.HasValue && softwareLastUpdate.HasValue)
            {
                DateTime robloxDate = robloxLastUpdate.Value.Date;
                DateTime softwareDate = softwareLastUpdate.Value.Date;
                if (DateTime.Compare(robloxDate, softwareDate) > 0)
                {
                    RuntimeDiagLog.Append($"[SaveAndLaunch] TryLaunch: version date gate — Roblox newer than cache cache (roblox={robloxDate:yyyy-MM-dd}, software={softwareDate:yyyy-MM-dd}).");
                    this.Log("[Roblox] Version mismatch - Roblox newer");
                    this.UpdateStatus("Version Mismatch", Colors.Red);
                    return false;
                }
            }
            this.UpdateStatus("Opening Roblox...", Colors.Blue);
            bool allowManageFastFlags = this.AllowManageFastFlagsToggle?.IsChecked ?? true;
            var flagsFromEditor = allowManageFastFlags && this._allFlagsList != null && this._allFlagsList.Count > 0
                ? this._allFlagsList.ToList()
                : null;

            this.TryApplyFastFlagsViaClientSettings(flagsFromEditor, "[SaveAndLaunch]");

            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = robloxExePath,
                    UseShellExecute = true
                });
            }
            catch (Exception launchEx)
            {
                this.Log($"[Roblox] Launch failed: {launchEx.Message}");
                this.UpdateStatus("Launch Failed", Colors.Red);
                return false;
            }
            this._saveAndLaunchMode = false;
            RuntimeDiagLog.Append($"[SaveAndLaunch] TryLaunch: started Roblox exe={robloxExePath}");
            this.Log("[Roblox] Roblox started");
            return true;
        }

        private void GlobalSaveAndLaunch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Log($"[GlobalSettings] Save and Launch clicked");
                this.SaveToggleStates();
                this.SaveModsSettingsFromUi();
                this.SaveGlobalSettings();
                this.CommitPendingFastFlagGridEdits();
                this.PersistFastFlagsFromEditorToJsonFile();
                this.PersistFastFlagTabUiToSettingsSilent();
                this.HasUnsavedChanges = false;
                this.Log($"[GlobalSettings] Global Settings saved successfully");

                if (this.TryLaunchRobloxWithProgressWindow())
                    ;
                else
                    this.ShowToastNotification("Settings saved.", "#404040");
            }
            catch (Exception ex)
            {
                this.Log($"[GlobalSettings] Error during save and launch: {ex.Message}");
            }
        }

        private void GlobalSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Log($"[GlobalSettings] Save clicked");
                this.SaveToggleStates();
                this.SaveModsSettingsFromUi();
                this.SaveGlobalSettings();
                this.HasUnsavedChanges = false;
                this.Log($"[GlobalSettings] Global Settings saved successfully");
            }
            catch (Exception ex)
            {
                this.Log($"[GlobalSettings] Error during save: {ex.Message}");
            }
        }

        private void SaveGlobalSettings()
        {
            if (this._gbsEditor == null || !this._gbsEditor.Loaded)
            {
                this.Log("[GlobalSettings] ✗ GBSEditor not initialized");
                return;
            }

            try
            {
                if (this.FindName("GraphicsQualitySlider") is Slider graphicsSlider)
                {
                    int qualityLevel = (int)graphicsSlider.Value;
                    this._gbsEditor.SetValue(this._gbsEditor.PresetPaths["Rendering.SavedQualityLevel"], qualityLevel);
                    this.Log($"[GlobalSettings] ✓ Graphics Quality set to: {qualityLevel}");
                }

                if (this.FindName("FramerateLimitInput") is TextBox frameRateBox && int.TryParse(frameRateBox.Text, out int frameRate))
                {
                    this._gbsEditor.SetValue(this._gbsEditor.PresetPaths["Rendering.FramerateCap"], frameRate);
                    this.Log($"[GlobalSettings] ✓ Framerate Cap set to: {frameRate}");
                }

                if (this.FindName("TransparencySlider") is Slider transparencySlider)
                {
                    double transparency = transparencySlider.Value;
                    this._gbsEditor.SetValue(this._gbsEditor.PresetPaths["UI.Transparency"], transparency);
                    this.Log($"[GlobalSettings] ✓ Transparency set to: {transparency}");
                }

                if (this.FindName("ReducedMotionToggle") is ToggleButton reducedMotionToggle)
                {
                    bool reducedMotion = reducedMotionToggle.IsChecked ?? false;
                    this._gbsEditor.SetValue(this._gbsEditor.PresetPaths["UI.ReducedMotion"], reducedMotion);
                    this.Log($"[GlobalSettings] ✓ Reduced Motion set to: {reducedMotion}");
                }

                if (this.FindName("FontSizeCombo") is ComboBox fontSizeCombo)
                {
                    int fontSize = fontSizeCombo.SelectedIndex + 1;
                    this._gbsEditor.SetValue(this._gbsEditor.PresetPaths["UI.FontSize"], fontSize);
                    this.Log($"[GlobalSettings] ✓ Font Size set to: {fontSize}");
                }

                if (this.FindName("MouseSensitivityInput") is TextBox mouseSensitivityBox && NumericInputHelper.TryParseUserDouble(mouseSensitivityBox.Text, out double mouseSensitivity))
                {
                    this._gbsEditor.SetValue(this._gbsEditor.PresetPaths["User.MouseSensitivity"], mouseSensitivity);
                    this.Log($"[GlobalSettings] ✓ Mouse Sensitivity set to: {mouseSensitivity}");
                }

                if (this.FindName("VREnabledToggle") is ToggleButton vrToggle)
                {
                    bool vrEnabled = vrToggle.IsChecked ?? false;
                    this._gbsEditor.SetValue(this._gbsEditor.PresetPaths["User.VREnabled"], vrEnabled);
                    this.Log($"[GlobalSettings] ✓ VR Enabled set to: {vrEnabled}");
                }

                this._gbsEditor.Save();

                if (this.FindName("GlobalReadOnlyToggle") is ToggleButton readOnlyToggle)
                {
                    bool readOnly = readOnlyToggle.IsChecked ?? false;
                    this._gbsEditor.SetReadOnly(readOnly);
                    this.Log($"[GlobalSettings] ✓ Read-Only Protection set to: {readOnly}");
                }

                this.Log($"[GlobalSettings] All settings saved to XML file");
            }
            catch (Exception ex)
            {
                this.Log($"[GlobalSettings] Error saving settings: {ex.Message}");
            }
        }

        private void GlobalClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Log($"[GlobalSettings] Close clicked");
                if (this.MainTabControl != null)
                {
                    this.MainTabControl.SelectedIndex = 0;
                    this.UpdateTabBorderHighlight(0);
                }
            }
            catch (Exception ex)
            {
                this.Log($"[GlobalSettings] Error during close: {ex.Message}");
            }
        }

        private void GraphicsQualitySlider_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.FindName("GraphicsQualitySlider") is Slider graphicsSlider && this.FindName("GraphicsQualityValue") is TextBlock graphicsValue)
                {
                    int qualityLevel = (int)graphicsSlider.Value;
                    graphicsValue.Text = qualityLevel.ToString();
                }
            }
            catch (Exception ex)
            {
                this.Log($"[GlobalSettings] Error updating graphics quality display: {ex.Message}");
            }
        }

        private void TransparencySlider_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.FindName("TransparencySlider") is Slider transparencySlider && this.FindName("TransparencyValue") is TextBlock transparencyValue)
                {
                    double transparency = Math.Round(transparencySlider.Value, 1);
                    transparencyValue.Text = transparency.ToString("F1");
                }
            }
            catch (Exception ex)
            {
                this.Log($"[GlobalSettings] Error updating transparency display: {ex.Message}");
            }
        }
    }
}
