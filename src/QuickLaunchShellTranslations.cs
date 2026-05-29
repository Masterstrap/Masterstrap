using System;
using System.Collections.Generic;

namespace Masterstrap.Services
{
    internal static class QuickLaunchShellTranslations
    {
        private static readonly HashSet<string> Keys = new(StringComparer.OrdinalIgnoreCase)
        {
            "Launch Roblox",
            "Configure settings",
            "About Masterstrap",
            "Version {0}.{1}.{2}",
            "Close",
        };

        private static readonly object _lock = new object();
        private static Dictionary<string, string>? _reverseMap;

        private static string ResolveEnglishKey(string key)
        {
            if (_reverseMap == null)
            {
                lock (_lock)
                {
                    if (_reverseMap == null)
                    {
                        var rev = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        var maps = new[] { Vi, Fil, Id, Ms, Ja, Zh, Th, Km, Lo, Ko, Ru, Uk, Fr, He, Tw, Tr, It, ArAe, Pt, Es };
                        foreach (var map in maps)
                        {
                            if (map == null) continue;
                            foreach (var pair in map)
                            {
                                if (!string.IsNullOrEmpty(pair.Value) && !rev.ContainsKey(pair.Value))
                                {
                                    rev[pair.Value] = pair.Key;
                                }
                            }
                        }
                        _reverseMap = rev;
                    }
                }
            }

            if (_reverseMap.TryGetValue(key, out string englishKey))
            {
                return englishKey;
            }
            return key;
        }

        public static bool IsLauncherShellKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return false;
            string englishKey = ResolveEnglishKey(key.Trim());
            return Keys.Contains(englishKey);
        }

        public static bool TryTranslate(string currentLanguage, string key, out string value)
        {
            value = key;
            if (string.IsNullOrWhiteSpace(key)) return false;
            string normalizedKey = key.Trim();

            string englishKey = ResolveEnglishKey(normalizedKey);

            if (!Keys.Contains(englishKey)) return false;

            if (IsEnglishLocale(currentLanguage))
            {
                value = englishKey;
                return true;
            }

            var map = ResolveMap(currentLanguage);
            if (map != null && map.TryGetValue(englishKey, out var t) && !string.IsNullOrEmpty(t))
            {
                value = t;
                return true;
            }

            value = englishKey;
            return true;
        }

        private static bool IsEnglishLocale(string lang) =>
            lang.Equals(LocalizationService.English, StringComparison.OrdinalIgnoreCase)
            || lang.Equals(LocalizationService.EnglishCanada, StringComparison.OrdinalIgnoreCase)
            || lang.Equals(LocalizationService.SouthAfrica, StringComparison.OrdinalIgnoreCase);

        private static Dictionary<string, string>? ResolveMap(string lang)
        {
            if (lang.Equals(LocalizationService.Vietnamese, StringComparison.OrdinalIgnoreCase)) return Vi;
            if (lang.Equals(LocalizationService.Filipino, StringComparison.OrdinalIgnoreCase)) return Fil;
            if (lang.Equals(LocalizationService.Indonesian, StringComparison.OrdinalIgnoreCase)) return Id;
            if (lang.Equals(LocalizationService.Malay, StringComparison.OrdinalIgnoreCase)) return Ms;
            if (lang.Equals(LocalizationService.Japanese, StringComparison.OrdinalIgnoreCase)) return Ja;
            if (lang.Equals(LocalizationService.Chinese, StringComparison.OrdinalIgnoreCase)) return Zh;
            if (lang.Equals(LocalizationService.Thai, StringComparison.OrdinalIgnoreCase)) return Th;
            if (lang.Equals(LocalizationService.Khmer, StringComparison.OrdinalIgnoreCase)) return Km;
            if (lang.Equals(LocalizationService.Lao, StringComparison.OrdinalIgnoreCase)) return Lo;
            if (lang.Equals(LocalizationService.Korean, StringComparison.OrdinalIgnoreCase)) return Ko;
            if (lang.Equals(LocalizationService.Russian, StringComparison.OrdinalIgnoreCase)) return Ru;
            if (lang.Equals(LocalizationService.Ukrainian, StringComparison.OrdinalIgnoreCase)) return Uk;
            if (lang.Equals(LocalizationService.French, StringComparison.OrdinalIgnoreCase)) return Fr;
            if (lang.Equals(LocalizationService.Hebrew, StringComparison.OrdinalIgnoreCase)) return He;
            if (lang.Equals(LocalizationService.Taiwan, StringComparison.OrdinalIgnoreCase)) return Tw;
            if (lang.Equals(LocalizationService.Turkiye, StringComparison.OrdinalIgnoreCase)) return Tr;
            if (lang.Equals(LocalizationService.Italy, StringComparison.OrdinalIgnoreCase)) return It;
            if (lang.Equals(LocalizationService.UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return ArAe;
            if (lang.Equals(LocalizationService.Portuguese, StringComparison.OrdinalIgnoreCase)
                || lang.Equals(LocalizationService.Brazil, StringComparison.OrdinalIgnoreCase)) return Pt;
            if (lang.Equals(LocalizationService.SpanishLatin, StringComparison.OrdinalIgnoreCase)
                || lang.Equals(LocalizationService.SpanishArgentina, StringComparison.OrdinalIgnoreCase)
                || lang.Equals(LocalizationService.Colombia, StringComparison.OrdinalIgnoreCase)
                || lang.Equals(LocalizationService.Chile, StringComparison.OrdinalIgnoreCase)
                || lang.Equals(LocalizationService.Spain, StringComparison.OrdinalIgnoreCase)) return Es;
            if (lang.Equals(LocalizationService.German, StringComparison.OrdinalIgnoreCase)) return De;
            if (lang.Equals(LocalizationService.Romanian, StringComparison.OrdinalIgnoreCase)) return Ro;
            if (lang.Equals(LocalizationService.Swedish, StringComparison.OrdinalIgnoreCase)) return Sv;
            if (lang.Equals(LocalizationService.Dutch, StringComparison.OrdinalIgnoreCase)) return Nl;
            if (lang.Equals(LocalizationService.Polish, StringComparison.OrdinalIgnoreCase)) return Pl;
            return null;
        }

        private static Dictionary<string, string> D(params (string en, string loc)[] pairs)
        {
            var d = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var (en, loc) in pairs)
                d[en] = loc;
            return d;
        }

        private static readonly Dictionary<string, string> Vi = D(
            ("Launch Roblox", "Khởi chạy Roblox"),
            ("Configure settings", "Cấu hình cài đặt"),
            ("About Masterstrap", "Giới thiệu Masterstrap"),
            ("Version {0}.{1}.{2}", "Phiên bản {0}.{1}.{2}"),
            ("Close", "Đóng"));

        private static readonly Dictionary<string, string> Fil = D(
            ("Launch Roblox", "Ilunsad ang Roblox"),
            ("Configure settings", "I-configure ang mga setting"),
            ("About Masterstrap", "Tungkol sa Masterstrap"),
            ("Version {0}.{1}.{2}", "Bersyon {0}.{1}.{2}"),
            ("Close", "Isara"));

        private static readonly Dictionary<string, string> Id = D(
            ("Launch Roblox", "Luncurkan Roblox"),
            ("Configure settings", "Konfigurasi pengaturan"),
            ("About Masterstrap", "Tentang Masterstrap"),
            ("Version {0}.{1}.{2}", "Versi {0}.{1}.{2}"),
            ("Close", "Tutup"));

        private static readonly Dictionary<string, string> Pt = D(
            ("Launch Roblox", "Iniciar Roblox"),
            ("Configure settings", "Configurar definições"),
            ("About Masterstrap", "Sobre o Masterstrap"),
            ("Version {0}.{1}.{2}", "Versão {0}.{1}.{2}"),
            ("Close", "Fechar"));

        private static readonly Dictionary<string, string> Ms = D(
            ("Launch Roblox", "Lancarkan Roblox"),
            ("Configure settings", "Konfigurasi tetapan"),
            ("About Masterstrap", "Perihal Masterstrap"),
            ("Version {0}.{1}.{2}", "Versi {0}.{1}.{2}"),
            ("Close", "Tutup"));

        private static readonly Dictionary<string, string> Ja = D(
            ("Launch Roblox", "Robloxを起動"),
            ("Configure settings", "設定を構成"),
            ("About Masterstrap", "Masterstrapについて"),
            ("Version {0}.{1}.{2}", "バージョン {0}.{1}.{2}"),
            ("Close", "閉じる"));

        private static readonly Dictionary<string, string> Zh = D(
            ("Launch Roblox", "启动 Roblox"),
            ("Configure settings", "配置设置"),
            ("About Masterstrap", "关于 Masterstrap"),
            ("Version {0}.{1}.{2}", "版本 {0}.{1}.{2}"),
            ("Close", "关闭"));

        private static readonly Dictionary<string, string> Tw = D(
            ("Launch Roblox", "啟動 Roblox"),
            ("Configure settings", "設定"),
            ("About Masterstrap", "關於 Masterstrap"),
            ("Version {0}.{1}.{2}", "版本 {0}.{1}.{2}"),
            ("Close", "關閉"));

        private static readonly Dictionary<string, string> Th = D(
            ("Launch Roblox", "เปิด Roblox"),
            ("Configure settings", "ตั้งค่า"),
            ("About Masterstrap", "เกี่ยวกับ Masterstrap"),
            ("Version {0}.{1}.{2}", "เวอร์ชัน {0}.{1}.{2}"),
            ("Close", "ปิด"));

        private static readonly Dictionary<string, string> Km = D(
            ("Launch Roblox", "បើក Roblox"),
            ("Configure settings", "កំណត់រចនាសម្ព័ន្ធ"),
            ("About Masterstrap", "អំពី Masterstrap"),
            ("Version {0}.{1}.{2}", "កំណែ {0}.{1}.{2}"),
            ("Close", "បិទ"));

        private static readonly Dictionary<string, string> Lo = D(
            ("Launch Roblox", "ເປີດ Roblox"),
            ("Configure settings", "ຕັ້ງຄ່າ"),
            ("About Masterstrap", "ກ່ຽວກັບ Masterstrap"),
            ("Version {0}.{1}.{2}", "ເວີຊັນ {0}.{1}.{2}"),
            ("Close", "ປິດ"));

        private static readonly Dictionary<string, string> Ko = D(
            ("Launch Roblox", "Roblox 실행"),
            ("Configure settings", "설정 구성"),
            ("About Masterstrap", "Masterstrap 정보"),
            ("Version {0}.{1}.{2}", "버전 {0}.{1}.{2}"),
            ("Close", "닫기"));

        private static readonly Dictionary<string, string> Ru = D(
            ("Launch Roblox", "Запустить Roblox"),
            ("Configure settings", "Настроить параметры"),
            ("About Masterstrap", "О Masterstrap"),
            ("Version {0}.{1}.{2}", "Версия {0}.{1}.{2}"),
            ("Close", "Закрыть"));

        private static readonly Dictionary<string, string> Uk = D(
            ("Launch Roblox", "Запустити Roblox"),
            ("Configure settings", "Налаштування"),
            ("About Masterstrap", "Про Masterstrap"),
            ("Version {0}.{1}.{2}", "Версія {0}.{1}.{2}"),
            ("Close", "Закрити"));

        private static readonly Dictionary<string, string> Es = D(
            ("Launch Roblox", "Iniciar Roblox"),
            ("Configure settings", "Configurar ajustes"),
            ("About Masterstrap", "Acerca de Masterstrap"),
            ("Version {0}.{1}.{2}", "Versión {0}.{1}.{2}"),
            ("Close", "Cerrar"));

        private static readonly Dictionary<string, string> Fr = D(
            ("Launch Roblox", "Lancer Roblox"),
            ("Configure settings", "Configurer les paramètres"),
            ("About Masterstrap", "À propos de Masterstrap"),
            ("Version {0}.{1}.{2}", "Version {0}.{1}.{2}"),
            ("Close", "Fermer"));

        private static readonly Dictionary<string, string> He = D(
            ("Launch Roblox", "הפעל Roblox"),
            ("Configure settings", "הגדרות תצורה"),
            ("About Masterstrap", "אודות Masterstrap"),
            ("Version {0}.{1}.{2}", "גרסה {0}.{1}.{2}"),
            ("Close", "סגור"));

        private static readonly Dictionary<string, string> Tr = D(
            ("Launch Roblox", "Roblox'u Başlat"),
            ("Configure settings", "Ayarları yapılandır"),
            ("About Masterstrap", "Masterstrap Hakkında"),
            ("Version {0}.{1}.{2}", "Sürüm {0}.{1}.{2}"),
            ("Close", "Kapat"));

        private static readonly Dictionary<string, string> It = D(
            ("Launch Roblox", "Avvia Roblox"),
            ("Configure settings", "Configura impostazioni"),
            ("About Masterstrap", "Informazioni su Masterstrap"),
            ("Version {0}.{1}.{2}", "Versione {0}.{1}.{2}"),
            ("Close", "Chiudi"));

        private static readonly Dictionary<string, string> ArAe = D(
            ("Launch Roblox", "تشغيل Roblox"),
            ("Configure settings", "تهيئة الإعدادات"),
            ("About Masterstrap", "حول Masterstrap"),
            ("Version {0}.{1}.{2}", "الإصدار {0}.{1}.{2}"),
            ("Close", "إغلاق"));

        private static readonly Dictionary<string, string> De = D(
            ("Launch Roblox", "Roblox starten"),
            ("Configure settings", "Einstellungen konfigurieren"),
            ("About Masterstrap", "Ueber Masterstrap"),
            ("Version {0}.{1}.{2}", "Version {0}.{1}.{2}"),
            ("Close", "Schliessen"));

        private static readonly Dictionary<string, string> Ro = D(
            ("Launch Roblox", "Lansati Roblox"),
            ("Configure settings", "Configurati setarile"),
            ("About Masterstrap", "Despre Masterstrap"),
            ("Version {0}.{1}.{2}", "Versiunea {0}.{1}.{2}"),
            ("Close", "Inchide"));

        private static readonly Dictionary<string, string> Sv = D(
            ("Launch Roblox", "Starta Roblox"),
            ("Configure settings", "Konfigurera installningar"),
            ("About Masterstrap", "Om Masterstrap"),
            ("Version {0}.{1}.{2}", "Version {0}.{1}.{2}"),
            ("Close", "Stang"));

        private static readonly Dictionary<string, string> Nl = D(
            ("Launch Roblox", "Roblox starten"),
            ("Configure settings", "Instellingen configureren"),
            ("About Masterstrap", "Over Masterstrap"),
            ("Version {0}.{1}.{2}", "Versie {0}.{1}.{2}"),
            ("Close", "Sluiten"));

        private static readonly Dictionary<string, string> Pl = D(
            ("Launch Roblox", "Uruchom Roblox"),
            ("Configure settings", "Konfiguruj ustawienia"),
            ("About Masterstrap", "O Masterstrap"),
            ("Version {0}.{1}.{2}", "Wersja {0}.{1}.{2}"),
            ("Close", "Zamknij"));
    }
}
