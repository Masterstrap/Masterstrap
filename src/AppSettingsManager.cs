using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Masterstrap.Services
{
    public class AppSettingsManager
    {
        private readonly string _settingsPath;
        private AppSettings _settings;
        private readonly bool _wasFirstRun;

        public AppSettingsManager()
        {
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Masterstrap"
            );

            Directory.CreateDirectory(appDataPath);
            _settingsPath = Path.Combine(appDataPath, "settings.json");
            _wasFirstRun = !File.Exists(_settingsPath);
            LoadSettings();
        }

        public bool WasFirstRun => _wasFirstRun;

        private void LoadSettings()
        {
            try
            {
                if (File.Exists(_settingsPath))
                {
                    string json = File.ReadAllText(_settingsPath);
                    _settings = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
                    if (_settings.RenderingSettings == null)
                        _settings.RenderingSettings = new RenderingSettings();
                    if (_settings.PrivateServerLinks == null)
                        _settings.PrivateServerLinks = new List<PrivateServerLinkEntry>();
                    if (!_settings.PlayerNamesVisible.HasValue)
                        _settings.PlayerNamesVisible = !_settings.PlayerNamesHidden;
                    Console.WriteLine("[Settings] ✓ Settings loaded from cache");
                }
                else
                {
                    _settings = new AppSettings();
                    Console.WriteLine("[Settings] No cache found, using defaults");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Settings] ✗ Error loading settings: {ex.Message}");
                _settings = new AppSettings();
            }
        }

        private void SaveSettings()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(_settings, options);
                File.WriteAllText(_settingsPath, json);
                Console.WriteLine("[Settings] ✓ Settings saved to cache");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Settings] ✗ Error saving settings: {ex.Message}");
            }
        }

        public string GetLastFlagJsonPath()
        {
            return _settings?.LastFlagJsonPath ?? "";
        }

        public void SetLastFlagJsonPath(string path)
        {
            if (_settings != null)
            {
                _settings.LastFlagJsonPath = path;
                _settings.LastFlagJsonLoadTime = DateTime.Now;
                SaveSettings();
                Console.WriteLine($"[Settings] ✓ Saved FFlag JSON path: {Path.GetFileName(path)}");
            }
        }

        public bool ShouldAutoLoadFlagJson()
        {
            return !string.IsNullOrEmpty(_settings?.LastFlagJsonPath) &&
                   File.Exists(_settings.LastFlagJsonPath);
        }

        public string GetLastGamePresetTag()
        {
            return _settings?.LastGamePresetTag ?? "";
        }

        public void SetLastGamePresetTag(string tag)
        {
            if (_settings == null)
                return;
            _settings.LastGamePresetTag = tag ?? "";
            SaveSettings();
        }

        public string GetLastFflagsPanelLabel()
        {
            return _settings?.LastFflagsPanelLabel ?? "";
        }

        public void SetLastFflagsPanelLabel(string label)
        {
            if (_settings == null)
                return;
            _settings.LastFflagsPanelLabel = label ?? "";
            SaveSettings();
        }

        public long GetLastAccountManagerPlaceId()
        {
            return _settings?.LastAccountManagerPlaceId ?? 0;
        }

        public string GetLastAccountManagerPlaceTitle()
        {
            return _settings?.LastAccountManagerPlaceTitle ?? "";
        }

        public void SetLastAccountManagerGame(long placeId, string title)
        {
            if (_settings == null) return;
            _settings.LastAccountManagerPlaceId = placeId;
            _settings.LastAccountManagerPlaceTitle = title ?? "";
            SaveSettings();
        }

        public bool HasLoadedCache()
        {
            return _settings?.CacheLoaded ?? false;
        }

        public void SetCacheLoaded()
        {
            if (_settings != null)
            {
                _settings.CacheLoaded = true;
                _settings.CacheLoadTime = DateTime.Now;
                SaveSettings();
                Console.WriteLine("[Settings] ✓ Marked cache as loaded");
            }
        }

        public string GetLastFlagJsonContent()
        {
            try
            {
                string path = GetLastFlagJsonPath();
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    return File.ReadAllText(path);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Settings] ✗ Error reading FFlag JSON: {ex.Message}");
            }
            return "";
        }

        public void ClearCache()
        {
            try
            {
                _settings = new AppSettings();
                if (File.Exists(_settingsPath))
                {
                    File.Delete(_settingsPath);
                }
                Console.WriteLine("[Settings] ✓ Cache cleared");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Settings] ✗ Error clearing cache: {ex.Message}");
            }
        }

        public bool GetPlayerNamesVisible()
        {
            if (_settings == null)
                return true;
            if (_settings.PlayerNamesVisible.HasValue)
                return _settings.PlayerNamesVisible.Value;
            return !_settings.PlayerNamesHidden;
        }

        public void SetPlayerNamesVisible(bool visible)
        {
            if (_settings == null)
                return;
            _settings.PlayerNamesVisible = visible;
            _settings.PlayerNamesHidden = !visible;
            SaveSettings();
        }

        public bool GetPlayerNamesHidden() => !GetPlayerNamesVisible();

        public void SetPlayerNamesHidden(bool hidden) => SetPlayerNamesVisible(!hidden);

        public void SaveToggleStates(bool desktopShortcut, bool autoLoadFlags, bool autoLoadCache, bool minimizeToTray)
        {
            if (_settings != null)
            {
                _settings.DesktopShortcutEnabled = desktopShortcut;
                _settings.AutoLoadFlagsEnabled = true;
                _settings.AutoLoadCacheEnabled = autoLoadCache;
                _settings.MinimizeToTrayEnabled = false;
                SaveSettings();
            }
        }

        public (bool desktopShortcut, bool autoLoadFlags, bool autoLoadCache, bool minimizeToTray) GetToggleStates()
        {
            return (_settings?.DesktopShortcutEnabled ?? false,
                    true,
                    _settings?.AutoLoadCacheEnabled ?? false,
                    false);
        }

        public bool IsStartMenuShortcutEnabled()
        {
            return _settings?.StartMenuShortcutEnabled ?? false;
        }

        public void SetStartMenuShortcutEnabled(bool enabled)
        {
            if (_settings == null)
                return;
            _settings.StartMenuShortcutEnabled = enabled;
            SaveSettings();
        }

        public bool IsLaunchRobloxShortcutEnabled()
        {
            return _settings?.LaunchRobloxShortcutEnabled ?? false;
        }

        public void SetLaunchRobloxShortcutEnabled(bool enabled)
        {
            if (_settings == null)
                return;
            _settings.LaunchRobloxShortcutEnabled = enabled;
            SaveSettings();
        }

        public string GetLaunchDataMirrorBaseUrls()
        {
            return _settings?.LaunchDataMirrorBaseUrls ?? string.Empty;
        }

        public void SetLaunchDataMirrorBaseUrls(string value)
        {
            if (_settings == null)
                return;
            _settings.LaunchDataMirrorBaseUrls = value ?? string.Empty;
            SaveSettings();
        }

        public List<PrivateServerLinkEntry> GetPrivateServerLinks()
        {
            if (_settings?.PrivateServerLinks == null)
                return new List<PrivateServerLinkEntry>();

            var result = new List<PrivateServerLinkEntry>();
            foreach (var entry in _settings.PrivateServerLinks)
            {
                if (entry == null || string.IsNullOrWhiteSpace(entry.Url))
                    continue;

                result.Add(new PrivateServerLinkEntry
                {
                    Name = string.IsNullOrWhiteSpace(entry.Name) ? "Private Server" : entry.Name.Trim(),
                    Url = entry.Url.Trim()
                });
            }

            return result;
        }

        public void SetPrivateServerLinks(IEnumerable<PrivateServerLinkEntry> links)
        {
            if (_settings == null)
                return;

            var normalized = new List<PrivateServerLinkEntry>();
            if (links != null)
            {
                foreach (var entry in links)
                {
                    if (entry == null || string.IsNullOrWhiteSpace(entry.Url))
                        continue;

                    normalized.Add(new PrivateServerLinkEntry
                    {
                        Name = string.IsNullOrWhiteSpace(entry.Name) ? "Private Server" : entry.Name.Trim(),
                        Url = entry.Url.Trim()
                    });
                }
            }

            _settings.PrivateServerLinks = normalized;
            SaveSettings();
        }

        public List<Models.RobloxGame> GetPlayedGames()
        {
            return _settings?.PlayedGames ?? new List<Models.RobloxGame>();
        }

        public void AddToPlayedGames(Models.RobloxGame game)
        {
            if (_settings == null || game == null) return;

            _settings.PlayedGames.RemoveAll(g => g.PlaceId == game.PlaceId);
            _settings.PlayedGames.Insert(0, game);

            if (_settings.PlayedGames.Count > 20)
                _settings.PlayedGames = _settings.PlayedGames.GetRange(0, 20);

            SaveSettings();
        }

        public void SavePlayedGames(List<Models.RobloxGame> games)
        {
            if (_settings == null || games == null) return;
            _settings.PlayedGames = games;
            SaveSettings();
        }

        public bool IsAutoApplyFlagsEnabled()
        {
            return _settings?.AutoApplyFlagsEnabled ?? false;
        }

        public void SetAutoApplyFlagsEnabled(bool enabled)
        {
            if (_settings != null)
            {
                _settings.AutoApplyFlagsEnabled = enabled;
                SaveSettings();
            }
        }

        public bool IsAutoCheckUpdateEnabled()
        {
            return _settings?.AutoCheckUpdateEnabled ?? true;
        }

        public void SetAutoCheckUpdateEnabled(bool enabled)
        {
            if (_settings != null)
            {
                _settings.AutoCheckUpdateEnabled = enabled;
                SaveSettings();
            }
        }

        public RenderingSettings GetRenderingSettings()
        {
            return _settings?.RenderingSettings ?? new RenderingSettings();
        }

        public void SaveRenderingSettings(string msaaQuality, string renderingMode, string textureQuality)
        {
            if (_settings != null)
            {
                _settings.RenderingSettings.MSAAQuality = msaaQuality;
                _settings.RenderingSettings.RenderingMode = renderingMode;
                _settings.RenderingSettings.TextureQuality = textureQuality;
                SaveSettings();
                Console.WriteLine($"[Settings] ✓ Rendering settings saved: MSAA={msaaQuality}, Mode={renderingMode}, Texture={textureQuality}");
            }
        }

        public void SaveRenderingToggles(bool preserveRenderingQuality, bool frmQuality, bool meshDetailEnabled = true, int meshDetailValue = 3)
        {
            if (_settings != null)
            {
                _settings.RenderingSettings.PreserveRenderingQuality = preserveRenderingQuality;
                _settings.RenderingSettings.FRMQuality = frmQuality;
                _settings.RenderingSettings.MeshDetailEnabled = meshDetailEnabled;
                _settings.RenderingSettings.MeshDetailValue = Math.Clamp(meshDetailValue, 0, 3);
                SaveSettings();
                Console.WriteLine($"[Settings] ✓ Rendering toggles saved: PreserveQuality={preserveRenderingQuality}, FRM={frmQuality}, MeshDetail={meshDetailEnabled} ({meshDetailValue})");
            }
        }

        public void SaveRenderingExtras(
            bool disablePlayerShadows,
            bool disablePostProcessingEffects,
            bool disableTerrainTextures,
            string preferredLightingTechnology)
        {
            if (_settings == null)
                return;

            _settings.RenderingSettings.DisablePlayerShadows = disablePlayerShadows;
            _settings.RenderingSettings.DisablePostProcessingEffects = disablePostProcessingEffects;
            _settings.RenderingSettings.DisableTerrainTextures = disableTerrainTextures;
            _settings.RenderingSettings.PreferredLightingTechnology =
                string.IsNullOrWhiteSpace(preferredLightingTechnology) ? "Automatic" : preferredLightingTechnology.Trim();

            SaveSettings();
            Console.WriteLine(
                $"[Settings] ✓ Rendering extras saved: Shadows={disablePlayerShadows}, PostFX={disablePostProcessingEffects}, TerrainTextures={disableTerrainTextures}, Lighting={_settings.RenderingSettings.PreferredLightingTechnology}");
        }

        public void SaveFRMQualityValue(int value)
        {
            if (_settings != null)
            {
                _settings.RenderingSettings.FRMQualityValue = value;
                SaveSettings();
                Console.WriteLine($"[Settings] ✓ FRM Quality value saved: {value}");
            }
        }

        public void SaveAllRenderingFlags(bool manualFullscreen, bool disableScaling, bool disableD3D11,
                                          int meshLodStatic, int meshLodL0, int meshLodL12, int meshLodL23, int meshLodL34,
                                          bool textureQualityOverrideEnabled)
        {
            if (_settings != null)
            {
                _settings.RenderingSettings.ManualFullscreen = manualFullscreen;
                _settings.RenderingSettings.DisableScaling = disableScaling;
                _settings.RenderingSettings.DisableD3D11 = disableD3D11;
                _settings.RenderingSettings.MeshLodStatic = meshLodStatic;
                _settings.RenderingSettings.MeshLodL0 = meshLodL0;
                _settings.RenderingSettings.MeshLodL12 = meshLodL12;
                _settings.RenderingSettings.MeshLodL23 = meshLodL23;
                _settings.RenderingSettings.MeshLodL34 = meshLodL34;
                _settings.RenderingSettings.TextureQualityOverrideEnabled = textureQualityOverrideEnabled;
                SaveSettings();
                Console.WriteLine($"[Settings] ✓ All rendering flags saved (Fishstrap sync)");
            }
        }

        public object GetRenderingFlagValue(string flagName)
        {
            if (_settings?.RenderingSettings == null)
                return null;

            return flagName switch
            {
                "ManualFullscreen" => _settings.RenderingSettings.ManualFullscreen,
                "DisableScaling" => _settings.RenderingSettings.DisableScaling,
                "DisableD3D11" => _settings.RenderingSettings.DisableD3D11,
                "MeshLodStatic" => _settings.RenderingSettings.MeshLodStatic,
                "MeshLodL0" => _settings.RenderingSettings.MeshLodL0,
                "MeshLodL12" => _settings.RenderingSettings.MeshLodL12,
                "MeshLodL23" => _settings.RenderingSettings.MeshLodL23,
                "MeshLodL34" => _settings.RenderingSettings.MeshLodL34,
                "TextureQualityOverrideEnabled" => _settings.RenderingSettings.TextureQualityOverrideEnabled,
                _ => null
            };
        }

        public bool IsAutoCleanupTempEnabled()
        {
            return _settings?.AutoCleanupTempEnabled ?? false;
        }

        public void SetAutoCleanupTempEnabled(bool enabled)
        {
            if (_settings != null)
            {
                _settings.AutoCleanupTempEnabled = enabled;
                SaveSettings();
            }
        }

        public bool IsMemoryOptimizationEnabled()
        {
            return _settings?.MemoryOptimizationEnabled ?? false;
        }

        public void SetMemoryOptimizationEnabled(bool enabled)
        {
            if (_settings != null)
            {
                _settings.MemoryOptimizationEnabled = enabled;
                SaveSettings();
            }
        }

        public bool IsFastModeEnabled()
        {
            return _settings?.FastModeEnabled ?? false;
        }

        public void SetFastModeEnabled(bool enabled)
        {
            if (_settings != null)
            {
                _settings.FastModeEnabled = enabled;
                SaveSettings();
            }
        }

        public bool IsAllowManageFastFlagsEnabled()
        {
            return _settings?.AllowManageFastFlags ?? true;
        }

        public void SetAllowManageFastFlagsEnabled(bool enabled)
        {
            if (_settings != null)
            {
                _settings.AllowManageFastFlags = enabled;
                SaveSettings();
            }
        }

        public Unlock240FpsMode GetUnlock240FpsMode()
        {
            if (_settings == null || string.IsNullOrWhiteSpace(_settings.Unlock240FpsMode))
                return Unlock240FpsMode.FFlag;

            string mode = _settings.Unlock240FpsMode.Trim();
            if (mode.Equals("off", StringComparison.OrdinalIgnoreCase))
                return Unlock240FpsMode.Off;
            if (mode.Equals("global", StringComparison.OrdinalIgnoreCase))
                return Unlock240FpsMode.Global;
            return Unlock240FpsMode.FFlag;
        }

        public void SetUnlock240FpsMode(Unlock240FpsMode mode)
        {
            if (_settings == null)
                return;

            _settings.Unlock240FpsMode = mode switch
            {
                Unlock240FpsMode.Off => "Off",
                Unlock240FpsMode.Global => "Global",
                _ => "FFlag"
            };
            SaveSettings();
        }

        public int GetUnlock240GlobalFpsRequested()
        {
            if (_settings == null)
                return 240;
            return Math.Max(1, _settings.Unlock240GlobalFpsRequested);
        }

        public void SetUnlock240GlobalFpsRequested(int value)
        {
            if (_settings == null)
                return;
            _settings.Unlock240GlobalFpsRequested = Math.Max(1, value);
            SaveSettings();
        }

        public bool IsUnlock240GlobalFpsExplicitlySaved()
        {
            return _settings?.Unlock240GlobalFpsExplicitlySaved ?? false;
        }

        public void SetUnlock240GlobalFpsExplicitlySaved(bool value)
        {
            if (_settings == null)
                return;
            _settings.Unlock240GlobalFpsExplicitlySaved = value;
            SaveSettings();
        }

        public bool GetFirstRunGlobalDefaultsApplied()
        {
            return _settings?.FirstRunGlobalDefaultsApplied ?? false;
        }

        public void SetFirstRunGlobalDefaultsApplied(bool applied)
        {
            if (_settings != null)
            {
                _settings.FirstRunGlobalDefaultsApplied = applied;
                SaveSettings();
            }
        }

        public bool GetFirstRunRenderingDefaultsApplied()
        {
            return _settings?.FirstRunRenderingDefaultsApplied ?? false;
        }

        public void SetFirstRunRenderingDefaultsApplied(bool applied)
        {
            if (_settings == null)
                return;
            _settings.FirstRunRenderingDefaultsApplied = applied;
            SaveSettings();
        }

        public string GetDisplayLanguage()
        {
            string language = _settings?.DisplayLanguage ?? "English";
            return string.IsNullOrWhiteSpace(language) ? "English" : language;
        }

        public string GetLocale()
        {
            string locale = _settings?.Locale?.Trim();
            if (!string.IsNullOrEmpty(locale))
            {
                if (string.Equals(locale, "vi", StringComparison.OrdinalIgnoreCase)) return "vi";
                if (string.Equals(locale, "ph", StringComparison.OrdinalIgnoreCase)) return "ph";
                if (string.Equals(locale, "id", StringComparison.OrdinalIgnoreCase)) return "id";
                if (string.Equals(locale, "pt", StringComparison.OrdinalIgnoreCase)) return "pt";
                if (string.Equals(locale, "ms", StringComparison.OrdinalIgnoreCase)) return "ms";
                if (string.Equals(locale, "ja", StringComparison.OrdinalIgnoreCase)) return "ja";
                if (string.Equals(locale, "zh", StringComparison.OrdinalIgnoreCase)) return "zh";
                if (string.Equals(locale, "th", StringComparison.OrdinalIgnoreCase)) return "th";
                if (string.Equals(locale, "km", StringComparison.OrdinalIgnoreCase)) return "km";
                if (string.Equals(locale, "ko", StringComparison.OrdinalIgnoreCase)) return "ko";
                if (string.Equals(locale, "lo", StringComparison.OrdinalIgnoreCase)) return "lo";
                if (string.Equals(locale, "ru", StringComparison.OrdinalIgnoreCase)) return "ru";
                if (string.Equals(locale, "uk", StringComparison.OrdinalIgnoreCase)) return "uk";
                if (string.Equals(locale, "es-419", StringComparison.OrdinalIgnoreCase)) return "es-419";
                if (string.Equals(locale, "fr", StringComparison.OrdinalIgnoreCase)) return "fr";
                if (string.Equals(locale, "he", StringComparison.OrdinalIgnoreCase)) return "he";
                if (string.Equals(locale, "en-CA", StringComparison.OrdinalIgnoreCase)) return "en-CA";
                if (string.Equals(locale, "zh-TW", StringComparison.OrdinalIgnoreCase)) return "zh-TW";
                if (string.Equals(locale, "es-CO", StringComparison.OrdinalIgnoreCase)) return "es-CO";
                if (string.Equals(locale, "tr", StringComparison.OrdinalIgnoreCase)) return "tr";
                if (string.Equals(locale, "es-ES", StringComparison.OrdinalIgnoreCase)) return "es-ES";
                if (string.Equals(locale, "it", StringComparison.OrdinalIgnoreCase)) return "it";
                if (string.Equals(locale, "es-CL", StringComparison.OrdinalIgnoreCase)) return "es-CL";
                if (string.Equals(locale, "ar-AE", StringComparison.OrdinalIgnoreCase)) return "ar-AE";
                if (string.Equals(locale, "en-ZA", StringComparison.OrdinalIgnoreCase)) return "en-ZA";
                if (string.Equals(locale, "de", StringComparison.OrdinalIgnoreCase)) return "de";
                if (string.Equals(locale, "ro", StringComparison.OrdinalIgnoreCase)) return "ro";
                if (string.Equals(locale, "sv", StringComparison.OrdinalIgnoreCase)) return "sv";
                if (string.Equals(locale, "nl", StringComparison.OrdinalIgnoreCase)) return "nl";
                if (string.Equals(locale, "pl", StringComparison.OrdinalIgnoreCase)) return "pl";
                if (string.Equals(locale, "en", StringComparison.OrdinalIgnoreCase)) return "en";
            }
            string lang = _settings?.DisplayLanguage ?? "English";
            if (string.Equals(lang, "Vietnamese", StringComparison.OrdinalIgnoreCase)) return "vi";
            if (string.Equals(lang, "Filipino", StringComparison.OrdinalIgnoreCase)) return "ph";
            if (string.Equals(lang, "Indonesian", StringComparison.OrdinalIgnoreCase)) return "id";
            if (string.Equals(lang, "Portuguese", StringComparison.OrdinalIgnoreCase)) return "pt";
            if (string.Equals(lang, "Malay", StringComparison.OrdinalIgnoreCase)) return "ms";
            if (string.Equals(lang, "Japanese", StringComparison.OrdinalIgnoreCase)) return "ja";
            if (string.Equals(lang, "Chinese", StringComparison.OrdinalIgnoreCase)) return "zh";
            if (string.Equals(lang, "Thai", StringComparison.OrdinalIgnoreCase)) return "th";
            if (string.Equals(lang, "Khmer", StringComparison.OrdinalIgnoreCase)) return "km";
            if (string.Equals(lang, "Korean", StringComparison.OrdinalIgnoreCase)) return "ko";
            if (string.Equals(lang, "Lao", StringComparison.OrdinalIgnoreCase)) return "lo";
            if (string.Equals(lang, "Russian", StringComparison.OrdinalIgnoreCase)) return "ru";
            if (string.Equals(lang, "Ukrainian", StringComparison.OrdinalIgnoreCase)) return "uk";
            if (string.Equals(lang, "SpanishArgentina", StringComparison.OrdinalIgnoreCase)) return "es-419";
            if (string.Equals(lang, "French", StringComparison.OrdinalIgnoreCase)) return "fr";
            if (string.Equals(lang, "Hebrew", StringComparison.OrdinalIgnoreCase)) return "he";
            if (string.Equals(lang, "EnglishCanada", StringComparison.OrdinalIgnoreCase)) return "en-CA";
            if (string.Equals(lang, "Taiwan", StringComparison.OrdinalIgnoreCase)) return "zh-TW";
            if (string.Equals(lang, "Colombia", StringComparison.OrdinalIgnoreCase)) return "es-CO";
            if (string.Equals(lang, "Turkiye", StringComparison.OrdinalIgnoreCase)) return "tr";
            if (string.Equals(lang, "Spain", StringComparison.OrdinalIgnoreCase)) return "es-ES";
            if (string.Equals(lang, "Italy", StringComparison.OrdinalIgnoreCase)) return "it";
            if (string.Equals(lang, "Chile", StringComparison.OrdinalIgnoreCase)) return "es-CL";
            if (string.Equals(lang, "UnitedArabEmirates", StringComparison.OrdinalIgnoreCase)) return "ar-AE";
            if (string.Equals(lang, "Brazil", StringComparison.OrdinalIgnoreCase)) return "pt-BR";
            if (string.Equals(lang, "SouthAfrica", StringComparison.OrdinalIgnoreCase)) return "en-ZA";
            if (string.Equals(lang, "German", StringComparison.OrdinalIgnoreCase)) return "de";
            if (string.Equals(lang, "Romanian", StringComparison.OrdinalIgnoreCase)) return "ro";
            if (string.Equals(lang, "Swedish", StringComparison.OrdinalIgnoreCase)) return "sv";
            if (string.Equals(lang, "Dutch", StringComparison.OrdinalIgnoreCase)) return "nl";
            if (string.Equals(lang, "Polish", StringComparison.OrdinalIgnoreCase)) return "pl";
            return "en";
        }

        public void SetDisplayLanguage(string language)
        {
            if (_settings == null) return;
            if (string.IsNullOrWhiteSpace(language)) language = "English";
            if (string.Equals(language, "Vietnamese", StringComparison.OrdinalIgnoreCase))
            {
                _settings.DisplayLanguage = "Vietnamese";
                _settings.Locale = "vi";
            }
            else if (string.Equals(language, "Filipino", StringComparison.OrdinalIgnoreCase))
            {
                _settings.DisplayLanguage = "Filipino";
                _settings.Locale = "ph";
            }
            else if (string.Equals(language, "Indonesian", StringComparison.OrdinalIgnoreCase))
            {
                _settings.DisplayLanguage = "Indonesian";
                _settings.Locale = "id";
            }
            else if (string.Equals(language, "Portuguese", StringComparison.OrdinalIgnoreCase))
            {
                _settings.DisplayLanguage = "Portuguese";
                _settings.Locale = "pt";
            }
            else if (string.Equals(language, "Malay", StringComparison.OrdinalIgnoreCase))
            {
                _settings.DisplayLanguage = "Malay";
                _settings.Locale = "ms";
            }
            else if (string.Equals(language, "Japanese", StringComparison.OrdinalIgnoreCase))
            {
                _settings.DisplayLanguage = "Japanese";
                _settings.Locale = "ja";
            }
            else if (string.Equals(language, "Chinese", StringComparison.OrdinalIgnoreCase))
            {
                _settings.DisplayLanguage = "Chinese";
                _settings.Locale = "zh";
            }
            else if (string.Equals(language, "Thai", StringComparison.OrdinalIgnoreCase))
            {
                _settings.DisplayLanguage = "Thai";
                _settings.Locale = "th";
            }
            else if (string.Equals(language, "Khmer", StringComparison.OrdinalIgnoreCase))
            {
                _settings.DisplayLanguage = "Khmer";
                _settings.Locale = "km";
            }
            else if (string.Equals(language, "Lao", StringComparison.OrdinalIgnoreCase))
            {
                _settings.DisplayLanguage = "Lao";
                _settings.Locale = "lo";
            }
            else if (string.Equals(language, "Korean", StringComparison.OrdinalIgnoreCase))
            {
                _settings.DisplayLanguage = "Korean";
                _settings.Locale = "ko";
            }
            else if (string.Equals(language, "Russian", StringComparison.OrdinalIgnoreCase))
            {
                _settings.DisplayLanguage = "Russian";
                _settings.Locale = "ru";
            }
            else if (string.Equals(language, "Ukrainian", StringComparison.OrdinalIgnoreCase))
            {
                _settings.DisplayLanguage = "Ukrainian";
                _settings.Locale = "uk";
            }
            else if (string.Equals(language, "SpanishArgentina", StringComparison.OrdinalIgnoreCase))
            {
                _settings.DisplayLanguage = "SpanishArgentina";
                _settings.Locale = "es-419";
            }
            else if (string.Equals(language, "French", StringComparison.OrdinalIgnoreCase))
            {
                _settings.DisplayLanguage = "French";
                _settings.Locale = "fr";
            }
            else if (string.Equals(language, "Hebrew", StringComparison.OrdinalIgnoreCase))
            {
                _settings.DisplayLanguage = "Hebrew";
                _settings.Locale = "he";
            }
            else if (string.Equals(language, "EnglishCanada", StringComparison.OrdinalIgnoreCase))
            {
                _settings.DisplayLanguage = "EnglishCanada";
                _settings.Locale = "en-CA";
            }
            else if (string.Equals(language, "Taiwan", StringComparison.OrdinalIgnoreCase))
            {
                _settings.DisplayLanguage = "Taiwan";
                _settings.Locale = "zh-TW";
            }
            else if (string.Equals(language, "Colombia", StringComparison.OrdinalIgnoreCase))
            {
                _settings.DisplayLanguage = "Colombia";
                _settings.Locale = "es-CO";
            }
            else if (string.Equals(language, "Turkiye", StringComparison.OrdinalIgnoreCase))
            {
                _settings.DisplayLanguage = "Turkiye";
                _settings.Locale = "tr";
            }
            else if (string.Equals(language, "Spain", StringComparison.OrdinalIgnoreCase))
            {
                _settings.DisplayLanguage = "Spain";
                _settings.Locale = "es-ES";
            }
            else if (string.Equals(language, "Italy", StringComparison.OrdinalIgnoreCase))
            {
                _settings.DisplayLanguage = "Italy";
                _settings.Locale = "it";
            }
            else if (string.Equals(language, "Chile", StringComparison.OrdinalIgnoreCase))
            {
                _settings.DisplayLanguage = "Chile";
                _settings.Locale = "es-CL";
            }
            else if (string.Equals(language, "UnitedArabEmirates", StringComparison.OrdinalIgnoreCase))
            {
                _settings.DisplayLanguage = "UnitedArabEmirates";
                _settings.Locale = "ar-AE";
            }
            else if (string.Equals(language, "Brazil", StringComparison.OrdinalIgnoreCase))
            {
                _settings.DisplayLanguage = "Brazil";
                _settings.Locale = "pt-BR";
            }
            else if (string.Equals(language, "SouthAfrica", StringComparison.OrdinalIgnoreCase))
            {
                _settings.DisplayLanguage = "SouthAfrica";
                _settings.Locale = "en-ZA";
            }
            else if (string.Equals(language, "German", StringComparison.OrdinalIgnoreCase))
            {
                _settings.DisplayLanguage = "German";
                _settings.Locale = "de";
            }
            else if (string.Equals(language, "Romanian", StringComparison.OrdinalIgnoreCase))
            {
                _settings.DisplayLanguage = "Romanian";
                _settings.Locale = "ro";
            }
            else if (string.Equals(language, "Swedish", StringComparison.OrdinalIgnoreCase))
            {
                _settings.DisplayLanguage = "Swedish";
                _settings.Locale = "sv";
            }
            else if (string.Equals(language, "Dutch", StringComparison.OrdinalIgnoreCase))
            {
                _settings.DisplayLanguage = "Dutch";
                _settings.Locale = "nl";
            }
            else if (string.Equals(language, "Polish", StringComparison.OrdinalIgnoreCase))
            {
                _settings.DisplayLanguage = "Polish";
                _settings.Locale = "pl";
            }
            else
            {
                _settings.DisplayLanguage = "English";
                _settings.Locale = "en";
            }
            SaveSettings();
        }

        public string GetGlobalTheme()
        {
            string theme = _settings?.GlobalTheme?.Trim();
            if (!string.IsNullOrWhiteSpace(theme))
            {
                if (string.Equals(theme, "GlassmorphicBlur", StringComparison.OrdinalIgnoreCase)) return "GlassmorphicBlur";
                if (string.Equals(theme, "Glassmorphic", StringComparison.OrdinalIgnoreCase)) return "Glassmorphic";
                return "Default";
            }

            return _wasFirstRun ? "Default" : "Glassmorphic";
        }

        public void SetGlobalTheme(string theme)
        {
            if (_settings == null) return;
            if (string.Equals(theme, "GlassmorphicBlur", StringComparison.OrdinalIgnoreCase))
                _settings.GlobalTheme = "GlassmorphicBlur";
            else if (string.Equals(theme, "Glassmorphic", StringComparison.OrdinalIgnoreCase))
                _settings.GlobalTheme = "Glassmorphic";
            else
                _settings.GlobalTheme = "Default";
            SaveSettings();
        }

        public string GetCustomBackgroundImagePath()
        {
            return _settings?.CustomBackgroundImagePath ?? string.Empty;
        }

        public void SetCustomBackgroundImagePath(string path)
        {
            if (_settings == null) return;
            _settings.CustomBackgroundImagePath = path ?? string.Empty;
            SaveSettings();
        }

        public string GetUiTheme()
        {
            string t = _settings?.UiTheme?.Trim();
            if (string.Equals(t, "White", StringComparison.OrdinalIgnoreCase)
                || string.Equals(t, "Light", StringComparison.OrdinalIgnoreCase))
                return "White";
            return "Black";
        }

        public void SetUiTheme(string uiTheme)
        {
            if (_settings == null) return;
            bool light = string.Equals(uiTheme, "White", StringComparison.OrdinalIgnoreCase)
                         || string.Equals(uiTheme, "Light", StringComparison.OrdinalIgnoreCase);
            _settings.UiTheme = light ? "White" : "Black";
            SaveSettings();
        }

        public bool IsSkipLaunchShellEnabled()
        {
            return _settings?.SkipLaunchShell ?? false;
        }

        public void SetSkipLaunchShell(bool skip)
        {
            if (_settings == null)
                return;
            _settings.SkipLaunchShell = skip;
            SaveSettings();
        }

        public string GetSelectedAccount()
        {
            return _settings?.SelectedAccount ?? string.Empty;
        }

        public void SetSelectedAccount(string accountName)
        {
            if (_settings == null)
                return;
            _settings.SelectedAccount = accountName ?? string.Empty;
            SaveSettings();
        }

        public bool IsProtocolInterceptionEnabled()
        {
            return _settings?.IsProtocolInterceptionEnabled ?? true;
        }

        public void SetProtocolInterceptionEnabled(bool enabled)
        {
            if (_settings != null)
            {
                _settings.IsProtocolInterceptionEnabled = enabled;
                SaveSettings();
            }
        }

        public string GetFastFlagSettingsState()
        {
            return _settings?.FastFlagSettingsState ?? string.Empty;
        }

        public void SetFastFlagSettingsState(string json)
        {
            if (_settings == null) return;
            _settings.FastFlagSettingsState = json ?? string.Empty;
            SaveSettings();
        }

        public string GetFastFlagSettingsPayload()
        {
            return _settings?.FastFlagSettingsPayload ?? string.Empty;
        }

        public void SetFastFlagSettingsPayload(string json)
        {
            if (_settings == null) return;
            _settings.FastFlagSettingsPayload = json ?? string.Empty;
            SaveSettings();
        }
    }

    public class AppSettings
    {
        [JsonPropertyName("skipLaunchShell")]
        public bool SkipLaunchShell { get; set; } = false;

        [JsonPropertyName("isProtocolInterceptionEnabled")]
        public bool IsProtocolInterceptionEnabled { get; set; } = true;

        [JsonPropertyName("lastFlagJsonPath")]
        public string LastFlagJsonPath { get; set; } = "";

        [JsonPropertyName("lastGamePresetTag")]
        public string LastGamePresetTag { get; set; } = "";

        [JsonPropertyName("lastFflagsPanelLabel")]
        public string LastFflagsPanelLabel { get; set; } = "";

        [JsonPropertyName("lastFlagJsonLoadTime")]
        public DateTime LastFlagJsonLoadTime { get; set; }

        [JsonPropertyName("cacheLoaded")]
        public bool CacheLoaded { get; set; } = false;

        [JsonPropertyName("cacheLoadTime")]
        public DateTime CacheLoadTime { get; set; }

        [JsonPropertyName("desktopShortcutEnabled")]
        public bool DesktopShortcutEnabled { get; set; } = false;

        [JsonPropertyName("startMenuShortcutEnabled")]
        public bool StartMenuShortcutEnabled { get; set; } = false;

        [JsonPropertyName("launchRobloxShortcutEnabled")]
        public bool LaunchRobloxShortcutEnabled { get; set; } = false;

        [JsonPropertyName("launchDataMirrorBaseUrls")]
        public string LaunchDataMirrorBaseUrls { get; set; } = "";

        [JsonPropertyName("autoLoadFlagsEnabled")]
        public bool AutoLoadFlagsEnabled { get; set; } = true;

        [JsonPropertyName("playerNamesVisible")]
        public bool? PlayerNamesVisible { get; set; }

        [JsonPropertyName("playerNamesHidden")]
        public bool PlayerNamesHidden { get; set; }

        [JsonPropertyName("autoLoadCacheEnabled")]
        public bool AutoLoadCacheEnabled { get; set; } = true;

        [JsonPropertyName("minimizeToTrayEnabled")]
        public bool MinimizeToTrayEnabled { get; set; } = false;

        [JsonPropertyName("autoApplyFlagsEnabled")]
        public bool AutoApplyFlagsEnabled { get; set; } = true;

        [JsonPropertyName("autoCheckUpdateEnabled")]
        public bool AutoCheckUpdateEnabled { get; set; } = false;

        [JsonPropertyName("autoCleanupTempEnabled")]
        public bool AutoCleanupTempEnabled { get; set; } = false;

        [JsonPropertyName("memoryOptimizationEnabled")]
        public bool MemoryOptimizationEnabled { get; set; } = false;

        [JsonPropertyName("fastModeEnabled")]
        public bool FastModeEnabled { get; set; } = false;

        [JsonPropertyName("allowManageFastFlags")]
        public bool AllowManageFastFlags { get; set; } = true;

        [JsonPropertyName("unlock240FpsMode")]
        public string Unlock240FpsMode { get; set; } = "FFlag";

        [JsonPropertyName("unlock240GlobalFpsRequested")]
        public int Unlock240GlobalFpsRequested { get; set; } = 240;

        [JsonPropertyName("unlock240GlobalFpsExplicitlySaved")]
        public bool Unlock240GlobalFpsExplicitlySaved { get; set; } = false;

        [JsonPropertyName("displayLanguage")]
        public string DisplayLanguage { get; set; } = "English";

        [JsonPropertyName("globalTheme")]
        public string GlobalTheme { get; set; } = "Default";

        [JsonPropertyName("customBackgroundImagePath")]
        public string CustomBackgroundImagePath { get; set; } = "";

        [JsonPropertyName("uiTheme")]
        public string UiTheme { get; set; } = "Black";

        [JsonPropertyName("firstRunGlobalDefaultsApplied")]
        public bool FirstRunGlobalDefaultsApplied { get; set; } = false;

        [JsonPropertyName("firstRunRenderingDefaultsApplied")]
        public bool FirstRunRenderingDefaultsApplied { get; set; } = false;

        [JsonPropertyName("locale")]
        public string Locale { get; set; } = "en";

        [JsonPropertyName("version")]
        public string Version { get; set; } = "1.0";

        [JsonPropertyName("renderingSettings")]
        public RenderingSettings RenderingSettings { get; set; } = new();

        [JsonPropertyName("privateServerLinks")]
        public List<PrivateServerLinkEntry> PrivateServerLinks { get; set; } = new();

        [JsonPropertyName("selectedAccount")]
        public string SelectedAccount { get; set; } = string.Empty;

        [JsonPropertyName("playedGames")]
        public List<Models.RobloxGame> PlayedGames { get; set; } = new();

        [JsonPropertyName("lastAccountManagerPlaceId")]
        public long LastAccountManagerPlaceId { get; set; } = 0;

        [JsonPropertyName("lastAccountManagerPlaceTitle")]
        public string LastAccountManagerPlaceTitle { get; set; } = string.Empty;

        [JsonPropertyName("fastFlagSettingsState")]
        public string FastFlagSettingsState { get; set; } = "";

        [JsonPropertyName("fastFlagSettingsPayload")]
        public string FastFlagSettingsPayload { get; set; } = "";
    }

    public class PrivateServerLinkEntry : System.ComponentModel.INotifyPropertyChanged
    {
        private string _name = "Private Server";
        private string _url = "";
        private string _imageUrl = "";

        [JsonPropertyName("name")]
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(nameof(Name)); }
        }

        [JsonPropertyName("url")]
        public string Url
        {
            get => _url;
            set { _url = value; OnPropertyChanged(nameof(Url)); }
        }

        [JsonPropertyName("imageUrl")]
        public string ImageUrl
        {
            get => _imageUrl;
            set { _imageUrl = value; OnPropertyChanged(nameof(ImageUrl)); }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
    }

    public class RenderingSettings
    {
        [JsonPropertyName("msaaQuality")]
        public string MSAAQuality { get; set; } = "Automatic";

        [JsonPropertyName("renderingMode")]
        public string RenderingMode { get; set; } = "Automatic";

        [JsonPropertyName("textureQuality")]
        public string TextureQuality { get; set; } = "Automatic";

        [JsonPropertyName("preserveRenderingQuality")]
        public bool PreserveRenderingQuality { get; set; } = false;

        [JsonPropertyName("frmQuality")]
        public bool FRMQuality { get; set; } = false;

        [JsonPropertyName("frmQualityValue")]
        public int FRMQualityValue { get; set; } = 21;

        [JsonPropertyName("meshDetailEnabled")]
        public bool MeshDetailEnabled { get; set; } = false;

        [JsonPropertyName("meshDetailValue")]
        public int MeshDetailValue { get; set; } = 3;

        [JsonPropertyName("manualFullscreen")]
        public bool ManualFullscreen { get; set; } = false;

        [JsonPropertyName("disableScaling")]
        public bool DisableScaling { get; set; } = false;

        [JsonPropertyName("disableD3D11")]
        public bool DisableD3D11 { get; set; } = false;

        [JsonPropertyName("meshLodStatic")]
        public int MeshLodStatic { get; set; } = 0;

        [JsonPropertyName("meshLodL0")]
        public int MeshLodL0 { get; set; } = 0;

        [JsonPropertyName("meshLodL12")]
        public int MeshLodL12 { get; set; } = 0;

        [JsonPropertyName("meshLodL23")]
        public int MeshLodL23 { get; set; } = 0;

        [JsonPropertyName("meshLodL34")]
        public int MeshLodL34 { get; set; } = 0;

        [JsonPropertyName("textureQualityOverrideEnabled")]
        public bool TextureQualityOverrideEnabled { get; set; } = false;

        [JsonPropertyName("disablePlayerShadows")]
        public bool DisablePlayerShadows { get; set; } = false;

        [JsonPropertyName("disablePostProcessingEffects")]
        public bool DisablePostProcessingEffects { get; set; } = false;

        [JsonPropertyName("disableTerrainTextures")]
        public bool DisableTerrainTextures { get; set; } = false;

        [JsonPropertyName("preferredLightingTechnology")]
        public string PreferredLightingTechnology { get; set; } = "Automatic";
    }
}
