using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace Masterstrap.Services
{
    public class LanguageService
    {
        private static LanguageService _instance;
        private static readonly object _lock = new object();

        public static LanguageService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new LanguageService();
                    }
                }
                return _instance;
            }
        }

        private string _currentLanguage = "English";
        private readonly Dictionary<string, Dictionary<string, string>> _translations = new();

        public string CurrentLanguage
        {
            get => _currentLanguage;
            set
            {
                if (_currentLanguage != value)
                {
                    _currentLanguage = value;
                    LanguageChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler LanguageChanged;

        public LanguageService()
        {
            InitializeTranslations();
        }

        private void InitializeTranslations()
        {
            _translations["English"] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Settings"] = "Settings",
                ["SettingsAndOptions"] = "Settings and Options",
                ["LanguageSettings"] = "Language Settings",
                ["LanguageSettingsDesc"] = "Select your preferred display language for the application interface.",
                ["Save"] = "Save",
                ["SaveAndLaunch"] = "Save and Launch",
                ["Cancel"] = "Cancel",
                ["DontSave"] = "Don't Save",
                ["UnsavedChanges"] = "Unsaved Changes",
                ["UnsavedChangesMessage"] = "You have unsaved changes. Do you want to save before exiting?",
                ["ConfigurationSaved"] = "Configuration saved successfully!",
                ["SaveFailed"] = "Save failed!",
                ["DesktopShortcut"] = "Desktop Shortcut",
                ["GeneralSettings"] = "General Settings",
                ["English"] = "English",
                ["Vietnamese"] = "Vietnamese",
            };

            _translations["Vietnamese"] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Settings"] = "Cài đặt",
                ["SettingsAndOptions"] = "Cài đặt và tùy chọn",
                ["LanguageSettings"] = "Cài đặt ngôn ngữ",
                ["LanguageSettingsDesc"] = "Chọn ngôn ngữ hiển thị ưa thích cho giao diện ứng dụng.",
                ["Save"] = "Lưu",
                ["SaveAndLaunch"] = "Lưu và khởi chạy",
                ["Cancel"] = "Hủy",
                ["DontSave"] = "Không lưu",
                ["UnsavedChanges"] = "Thay đổi chưa lưu",
                ["UnsavedChangesMessage"] = "Bạn có thay đổi chưa lưu. Bạn có muốn lưu trước khi thoát không?",
                ["ConfigurationSaved"] = "Đã lưu cấu hình thành công!",
                ["SaveFailed"] = "Lưu thất bại!",
                ["DesktopShortcut"] = "Phím tắt màn hình",
                ["GeneralSettings"] = "Cài đặt chung",
                ["English"] = "Tiếng Anh",
                ["Vietnamese"] = "Tiếng Việt",
            };
        }

        public string GetString(string key)
        {
            if (string.IsNullOrEmpty(key)) return string.Empty;

            if (_translations.TryGetValue(_currentLanguage, out var langDict) &&
                langDict.TryGetValue(key, out var value))
            {
                return value;
            }

            if (_currentLanguage != "English" &&
                _translations.TryGetValue("English", out var enDict) &&
                enDict.TryGetValue(key, out var enValue))
            {
                return enValue;
            }

            return key;
        }

        public void InitializeFromSettings(AppSettingsManager settingsManager)
        {
            if (settingsManager == null) return;

            var lang = settingsManager.GetDisplayLanguage() ?? "English";
            var normalized = LocalizationService.NormalizeLanguage(lang);
            CurrentLanguage = normalized;

            string cultureName = normalized switch
            {
                LocalizationService.Vietnamese => "vi-VN",
                LocalizationService.German => "de-DE",
                LocalizationService.Romanian => "ro-RO",
                LocalizationService.Swedish => "sv-SE",
                LocalizationService.Dutch => "nl-NL",
                LocalizationService.Polish => "pl-PL",
                _ => "en-US"
            };

            try
            {
                var culture = new CultureInfo(cultureName);
                Thread.CurrentThread.CurrentUICulture = culture;
                Thread.CurrentThread.CurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
                CultureInfo.DefaultThreadCurrentCulture = culture;
            }
            catch
            {
            }
        }

        public bool IsVietnamese => string.Equals(_currentLanguage, "Vietnamese", StringComparison.OrdinalIgnoreCase);
    }
}
