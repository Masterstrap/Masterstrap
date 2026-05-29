using System;
using System.Collections.Generic;

namespace Masterstrap.Services
{
    internal static class ModsTabUiTranslations
    {
        private static readonly HashSet<string> Keys = new(StringComparer.OrdinalIgnoreCase)
        {
            "Mods",
            "Manage and apply file mods to the Roblox client.",
            "Open Mods Folder",
            "Manage custom Roblox mods here.",
            "Help",
            "See info about managing and creating mods.",
            "Manage compatibility settings",
            "Configure application parameters such as DPI scaling behaviour and",
            "fullscreen optimizations.",
            "Presets",
            "Mouse cursor",
            "Choose between using classic Roblox cursor styles.",
            "Use old avatar editor background",
            "Bring back the old avatar editor background used in legacy app versions.",
            "Emulate old character sounds",
            "Attempt to restore old footsteps and character movement sounds before 2014.",
            "Preferred emoji type",
            "Choose what type of emoji should Roblox use.",
            "Miscellaneous",
            "Use custom font",
            "Font size can still be adjusted in the Global tab.",
            "Choose font...",
            "choose font...",
            "Remove",
            "Default",
            "From 2006",
            "From 2013",
            "Windows 11",
            "Windows 10",
            "Windows 8.1",
            "Custom Shiftlock",
            "Change the icon when Shift Lock is enabled. (Froststrap/Voidstrap Collection)",
            "Advanced Modifications",
            "Skybox Preset",
            "Change the in game sky texture. Some presets require a restart.",
            "Fullbright (No Shadows)",
            "Makes everything bright and removes shadows. (Experimental)",
            "Visual Overlays & Stats",
            "Lighting Overlays",
            "Motion Blur Effect",
            "FPS Counter",
            "Server Location & Stats",
            "Screen Brightness",
            "Adjust screen brightness via overlay (50% is default)."
        };

        private static readonly Lazy<Dictionary<string, string>> Reverse = new(BuildReverse);

        private static Dictionary<string, string> D(params (string en, string loc)[] rows)
        {
            var d = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var (en, loc) in rows)
                d[en] = loc;
            return d;
        }

        public static bool TryTranslate(string currentLanguage, string key, out string value)
        {
            value = null!;
            if (string.IsNullOrWhiteSpace(key))
                return false;

            string normalizedKey = key.Trim();
            bool isEnglishLocale =
                currentLanguage.Equals(LocalizationService.English, StringComparison.OrdinalIgnoreCase)
                || currentLanguage.Equals(LocalizationService.EnglishCanada, StringComparison.OrdinalIgnoreCase)
                || currentLanguage.Equals(LocalizationService.SouthAfrica, StringComparison.OrdinalIgnoreCase);

            string englishKey = ResolveEnglishKey(normalizedKey);
            if (!Keys.Contains(englishKey))
                return false;

            if (isEnglishLocale)
            {
                value = englishKey;
                return true;
            }

            Dictionary<string, string>? map = ResolveMap(currentLanguage);
            if (map != null && map.TryGetValue(englishKey, out string translated) && !string.IsNullOrWhiteSpace(translated))
            {
                value = translated;
                return true;
            }

            value = englishKey;
            return true;
        }

        private static string ResolveEnglishKey(string key)
        {
            if (Keys.Contains(key))
                return key;
            return Reverse.Value.TryGetValue(key, out string en) ? en : key;
        }

        private static Dictionary<string, string> BuildReverse()
        {
            var reverse = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            void AddMap(Dictionary<string, string> map)
            {
                foreach (var pair in map)
                {
                    if (!string.IsNullOrWhiteSpace(pair.Value) && !reverse.ContainsKey(pair.Value))
                        reverse[pair.Value] = pair.Key;
                }
            }

            AddMap(Vi); AddMap(Fil); AddMap(Id); AddMap(Pt); AddMap(Ms);
            AddMap(Ja); AddMap(Zh); AddMap(Th); AddMap(Km); AddMap(Lo);
            AddMap(Ko); AddMap(Ru); AddMap(Uk); AddMap(Es); AddMap(Fr);
            AddMap(He); AddMap(Tw); AddMap(Tr); AddMap(It); AddMap(ArAe);
            AddMap(De); AddMap(Ro); AddMap(Sv); AddMap(Nl); AddMap(Pl);
            return reverse;
        }

        private static Dictionary<string, string>? ResolveMap(string lang)
        {
            if (lang.Equals(LocalizationService.Vietnamese, StringComparison.OrdinalIgnoreCase)) return Vi;
            if (lang.Equals(LocalizationService.Filipino, StringComparison.OrdinalIgnoreCase)) return Fil;
            if (lang.Equals(LocalizationService.Indonesian, StringComparison.OrdinalIgnoreCase)) return Id;
            if (lang.Equals(LocalizationService.Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(LocalizationService.Brazil, StringComparison.OrdinalIgnoreCase)) return Pt;
            if (lang.Equals(LocalizationService.Malay, StringComparison.OrdinalIgnoreCase)) return Ms;
            if (lang.Equals(LocalizationService.Japanese, StringComparison.OrdinalIgnoreCase)) return Ja;
            if (lang.Equals(LocalizationService.Chinese, StringComparison.OrdinalIgnoreCase)) return Zh;
            if (lang.Equals(LocalizationService.Thai, StringComparison.OrdinalIgnoreCase)) return Th;
            if (lang.Equals(LocalizationService.Khmer, StringComparison.OrdinalIgnoreCase)) return Km;
            if (lang.Equals(LocalizationService.Lao, StringComparison.OrdinalIgnoreCase)) return Lo;
            if (lang.Equals(LocalizationService.Korean, StringComparison.OrdinalIgnoreCase)) return Ko;
            if (lang.Equals(LocalizationService.Russian, StringComparison.OrdinalIgnoreCase)) return Ru;
            if (lang.Equals(LocalizationService.Ukrainian, StringComparison.OrdinalIgnoreCase)) return Uk;
            if (lang.Equals(LocalizationService.SpanishLatin, StringComparison.OrdinalIgnoreCase)
                || lang.Equals(LocalizationService.SpanishArgentina, StringComparison.OrdinalIgnoreCase)
                || lang.Equals(LocalizationService.Colombia, StringComparison.OrdinalIgnoreCase)
                || lang.Equals(LocalizationService.Spain, StringComparison.OrdinalIgnoreCase)
                || lang.Equals(LocalizationService.Chile, StringComparison.OrdinalIgnoreCase)) return Es;
            if (lang.Equals(LocalizationService.French, StringComparison.OrdinalIgnoreCase)) return Fr;
            if (lang.Equals(LocalizationService.Hebrew, StringComparison.OrdinalIgnoreCase)) return He;
            if (lang.Equals(LocalizationService.Taiwan, StringComparison.OrdinalIgnoreCase)) return Tw;
            if (lang.Equals(LocalizationService.Turkiye, StringComparison.OrdinalIgnoreCase)) return Tr;
            if (lang.Equals(LocalizationService.Italy, StringComparison.OrdinalIgnoreCase)) return It;
            if (lang.Equals(LocalizationService.UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return ArAe;
            if (lang.Equals(LocalizationService.German, StringComparison.OrdinalIgnoreCase)) return De;
            if (lang.Equals(LocalizationService.Romanian, StringComparison.OrdinalIgnoreCase)) return Ro;
            if (lang.Equals(LocalizationService.Swedish, StringComparison.OrdinalIgnoreCase)) return Sv;
            if (lang.Equals(LocalizationService.Dutch, StringComparison.OrdinalIgnoreCase)) return Nl;
            if (lang.Equals(LocalizationService.Polish, StringComparison.OrdinalIgnoreCase)) return Pl;
            return null;
        }

        private static readonly Dictionary<string, string> Vi = D(
            ("Mods", "Mods"),
            ("Manage and apply file mods to the Roblox client.", "Quản lý và áp dụng file mod cho client Roblox."),
            ("Open Mods Folder", "Mở thư mục Mods"),
            ("Manage custom Roblox mods here.", "Quản lý các mod Roblox tùy chỉnh tại đây."),
            ("Help", "Trợ giúp"),
            ("See info about managing and creating mods.", "Xem thông tin về cách quản lý và tạo mods."),
            ("Manage compatibility settings", "Quản lý cài đặt tương thích"),
            ("Configure application parameters such as DPI scaling behaviour and", "Cấu hình các tham số ứng dụng như hành vi DPI scaling và"),
            ("fullscreen optimizations.", "tối ưu toàn màn hình."),
            ("Presets", "Preset"),
            ("Mouse cursor", "Con trỏ chuột"),
            ("Choose between using classic Roblox cursor styles.", "Chọn giữa các kiểu con trỏ Roblox cổ điển."),
            ("Use old avatar editor background", "Dùng nền trình sửa avatar cũ"),
            ("Bring back the old avatar editor background used in legacy app versions.", "Khôi phục nền trình sửa avatar cũ của các phiên bản trước."),
            ("Emulate old character sounds", "Mô phỏng âm thanh nhân vật cũ"),
            ("Attempt to restore old footsteps and character movement sounds before 2014.", "Cố gắng khôi phục âm bước chân và di chuyển nhân vật trước 2014."),
            ("Preferred emoji type", "Loại emoji ưu tiên"),
            ("Choose what type of emoji should Roblox use.", "Chọn loại emoji Roblox nên sử dụng."),
            ("Miscellaneous", "Khác"),
            ("Use custom font", "Dùng font tùy chỉnh"),
            ("Font size can still be adjusted in the Global tab.", "Kích thước font vẫn có thể chỉnh ở tab Global."),
            ("Choose font...", "Chọn font..."),
            ("choose font...", "chọn font..."),
            ("Remove", "Xóa"),
            ("Default", "Mặc định"),
            ("From 2006", "Từ 2006"),
            ("From 2013", "Từ 2013"),
            ("Windows 11", "Windows 11"),
            ("Windows 10", "Windows 10"),
            ("Windows 8.1", "Windows 8.1"),
            ("Custom Shiftlock", "Shiftlock tùy chỉnh"),
            ("Change the icon when Shift Lock is enabled. (Froststrap/Voidstrap Collection)", "Thay đổi biểu tượng khi bật Shift Lock. (Bộ sưu tập Froststrap/Voidstrap)"),
            ("Advanced Modifications", "Tùy chỉnh nâng cao"),
            ("Skybox Preset", "Preset bầu trời (skybox)"),
            ("Change the in game sky texture. Some presets require a restart.", "Đổi kết cấu bầu trời trong game. Một số preset cần khởi động lại."),
            ("Fullbright (No Shadows)", "Sáng hoàn toàn (không bóng)"),
            ("Makes everything bright and removes shadows. (Experimental)", "Làm mọi thứ sáng hơn và loại bỏ bóng. (Thử nghiệm)"),
            ("Visual Overlays & Stats", "Lớp phủ & thống kê hiển thị"),
            ("Lighting Overlays", "Lớp phủ ánh sáng"),
            ("Motion Blur Effect", "Hiệu ứng mờ chuyển động"),
            ("FPS Counter", "Bộ đếm FPS"),
            ("Server Location & Stats", "Vị trí máy chủ & thống kê"),
            ("Screen Brightness", "Độ sáng màn hình"),
            ("Adjust screen brightness via overlay (50% is default).", "Chỉnh độ sáng qua lớp phủ (50% là mặc định)."));

        private static readonly Dictionary<string, string> Fil = D(
            ("Mods", "Mods"), ("Manage and apply file mods to the Roblox client.", "Pamahalaan at ilapat ang file mods sa Roblox client."),
            ("Open Mods Folder", "Buksan ang Mods Folder"), ("Manage custom Roblox mods here.", "Pamahalaan dito ang custom Roblox mods."),
            ("Help", "Tulong"), ("See info about managing and creating mods.", "Tingnan ang impormasyon tungkol sa pag-manage at paggawa ng mods."),
            ("Manage compatibility settings", "Pamahalaan ang compatibility settings"), ("Configure application parameters such as DPI scaling behaviour and", "I-configure ang parameters tulad ng DPI scaling behaviour at"),
            ("fullscreen optimizations.", "fullscreen optimizations."), ("Presets", "Presets"), ("Mouse cursor", "Mouse cursor"),
            ("Choose between using classic Roblox cursor styles.", "Pumili sa classic Roblox cursor styles."), ("Use old avatar editor background", "Gamitin ang lumang avatar editor background"),
            ("Bring back the old avatar editor background used in legacy app versions.", "Ibalik ang lumang avatar editor background na ginamit sa legacy app versions."),
            ("Emulate old character sounds", "I-emulate ang lumang character sounds"), ("Attempt to restore old footsteps and character movement sounds before 2014.", "Subukang ibalik ang lumang footsteps at movement sounds bago 2014."),
            ("Preferred emoji type", "Preferred emoji type"), ("Choose what type of emoji should Roblox use.", "Piliin kung anong emoji type ang gagamitin ng Roblox."),
            ("Miscellaneous", "Iba pa"), ("Use custom font", "Gumamit ng custom font"), ("Font size can still be adjusted in the Global tab.", "Maari pa ring i-adjust ang font size sa Global tab."),
            ("Choose font...", "Pumili ng font..."), ("Remove", "Alisin"), ("Default", "Default"), ("From 2006", "Mula 2006"), ("From 2013", "Mula 2013"),
            ("Windows 11", "Windows 11"), ("Windows 10", "Windows 10"), ("Windows 8.1", "Windows 8.1"),
            ("Custom Shiftlock", "Custom Shiftlock"),
            ("Change the icon when Shift Lock is enabled. (Froststrap/Voidstrap Collection)", "Palitan ang icon kapag naka-on ang Shift Lock. (Koleksyon ng Froststrap/Voidstrap)"),
            ("Advanced Modifications", "Advanced na mga pagbabago"),
            ("Skybox Preset", "Skybox preset"),
            ("Change the in game sky texture. Some presets require a restart.", "Baguhin ang sky texture sa laro. Ang ilang preset ay kailangan i-restart."),
            ("Fullbright (No Shadows)", "Fullbright (Walang shadow)"),
            ("Makes everything bright and removes shadows. (Experimental)", "Pinapaliwanag ang lahat at tinatanggal ang mga shadow. (Eksperimental)"),
            ("Visual Overlays & Stats", "Visual overlays at stats"),
            ("Lighting Overlays", "Lighting overlays"),
            ("Motion Blur Effect", "Motion blur effect"),
            ("FPS Counter", "FPS counter"),
            ("Server Location & Stats", "Lokasyon ng server at stats"),
            ("Screen Brightness", "Liwanag ng screen"),
            ("Adjust screen brightness via overlay (50% is default).", "Ayusin ang liwanag gamit ang overlay (50% ang default)."));

        private static readonly Dictionary<string, string> Id = D(
            ("Mods", "Mods"), ("Manage and apply file mods to the Roblox client.", "Kelola dan terapkan file mod ke klien Roblox."),
            ("Open Mods Folder", "Buka Folder Mods"), ("Manage custom Roblox mods here.", "Kelola mod Roblox kustom di sini."),
            ("Help", "Bantuan"), ("See info about managing and creating mods.", "Lihat info tentang mengelola dan membuat mod."),
            ("Manage compatibility settings", "Kelola pengaturan kompatibilitas"), ("Configure application parameters such as DPI scaling behaviour and", "Atur parameter aplikasi seperti perilaku DPI scaling dan"),
            ("fullscreen optimizations.", "fullscreen optimizations."), ("Presets", "Preset"), ("Mouse cursor", "Kursor mouse"),
            ("Choose between using classic Roblox cursor styles.", "Pilih antara gaya kursor Roblox klasik."),
            ("Use old avatar editor background", "Gunakan latar editor avatar lama"), ("Bring back the old avatar editor background used in legacy app versions.", "Kembalikan latar editor avatar lama dari versi aplikasi sebelumnya."),
            ("Emulate old character sounds", "Emulasi suara karakter lama"), ("Attempt to restore old footsteps and character movement sounds before 2014.", "Coba pulihkan suara langkah kaki dan gerak karakter lama sebelum 2014."),
            ("Preferred emoji type", "Tipe emoji pilihan"), ("Choose what type of emoji should Roblox use.", "Pilih tipe emoji yang harus digunakan Roblox."),
            ("Miscellaneous", "Lainnya"), ("Use custom font", "Gunakan font kustom"), ("Font size can still be adjusted in the Global tab.", "Ukuran font masih bisa diatur di tab Global."),
            ("Choose font...", "Pilih font..."), ("Remove", "Hapus"), ("Default", "Default"), ("From 2006", "Dari 2006"), ("From 2013", "Dari 2013"),
            ("Windows 11", "Windows 11"), ("Windows 10", "Windows 10"), ("Windows 8.1", "Windows 8.1"),
            ("Custom Shiftlock", "Shiftlock kustom"),
            ("Change the icon when Shift Lock is enabled. (Froststrap/Voidstrap Collection)", "Ubah ikon saat Shift Lock aktif. (Koleksi Froststrap/Voidstrap)"),
            ("Advanced Modifications", "Modifikasi lanjutan"),
            ("Skybox Preset", "Preset skybox"),
            ("Change the in game sky texture. Some presets require a restart.", "Ubah tekstur langit dalam game. Beberapa preset memerlukan restart."),
            ("Fullbright (No Shadows)", "Fullbright (tanpa bayangan)"),
            ("Makes everything bright and removes shadows. (Experimental)", "Membuat semuanya terang dan menghilangkan bayangan. (Eksperimental)"),
            ("Visual Overlays & Stats", "Overlay visual & statistik"),
            ("Lighting Overlays", "Overlay pencahayaan"),
            ("Motion Blur Effect", "Efek motion blur"),
            ("FPS Counter", "Penghitung FPS"),
            ("Server Location & Stats", "Lokasi server & statistik"),
            ("Screen Brightness", "Kecerahan layar"),
            ("Adjust screen brightness via overlay (50% is default).", "Sesuaikan kecerahan lewat overlay (50% default)."));

        private static readonly Dictionary<string, string> Pt = D(
            ("Mods", "Mods"), ("Manage and apply file mods to the Roblox client.", "Gerencie e aplique mods de arquivo ao cliente Roblox."),
            ("Open Mods Folder", "Abrir pasta de Mods"), ("Manage custom Roblox mods here.", "Gerencie mods Roblox personalizados aqui."),
            ("Help", "Ajuda"), ("See info about managing and creating mods.", "Veja informações sobre gerenciar e criar mods."),
            ("Manage compatibility settings", "Gerenciar configurações de compatibilidade"), ("Configure application parameters such as DPI scaling behaviour and", "Configure parâmetros do aplicativo como comportamento de DPI scaling e"),
            ("fullscreen optimizations.", "fullscreen optimizations."), ("Presets", "Presets"), ("Mouse cursor", "Cursor do mouse"),
            ("Choose between using classic Roblox cursor styles.", "Escolha entre estilos clássicos de cursor do Roblox."),
            ("Use old avatar editor background", "Usar fundo antigo do editor de avatar"), ("Bring back the old avatar editor background used in legacy app versions.", "Restaure o fundo antigo do editor de avatar usado em versões antigas."),
            ("Emulate old character sounds", "Emular sons antigos do personagem"), ("Attempt to restore old footsteps and character movement sounds before 2014.", "Tente restaurar sons antigos de passos e movimento antes de 2014."),
            ("Preferred emoji type", "Tipo de emoji preferido"), ("Choose what type of emoji should Roblox use.", "Escolha qual tipo de emoji o Roblox deve usar."),
            ("Miscellaneous", "Diversos"), ("Use custom font", "Usar fonte personalizada"), ("Font size can still be adjusted in the Global tab.", "O tamanho da fonte ainda pode ser ajustado na aba Global."),
            ("Choose font...", "Escolher fonte..."), ("Remove", "Remover"), ("Default", "Padrão"), ("From 2006", "De 2006"), ("From 2013", "De 2013"),
            ("Windows 11", "Windows 11"), ("Windows 10", "Windows 10"), ("Windows 8.1", "Windows 8.1"),
            ("Custom Shiftlock", "Shiftlock personalizado"),
            ("Change the icon when Shift Lock is enabled. (Froststrap/Voidstrap Collection)", "Altere o ícone quando o Shift Lock estiver ativo. (Coleção Froststrap/Voidstrap)"),
            ("Advanced Modifications", "Modificações avançadas"),
            ("Skybox Preset", "Predefinição de skybox"),
            ("Change the in game sky texture. Some presets require a restart.", "Altere a textura do céu no jogo. Alguns predefinições exigem reinício."),
            ("Fullbright (No Shadows)", "Fullbright (sem sombras)"),
            ("Makes everything bright and removes shadows. (Experimental)", "Deixa tudo mais claro e remove sombras. (Experimental)"),
            ("Visual Overlays & Stats", "Sobreposições visuais e estatísticas"),
            ("Lighting Overlays", "Sobreposições de iluminação"),
            ("Motion Blur Effect", "Efeito de desfoque de movimento"),
            ("FPS Counter", "Contador de FPS"),
            ("Server Location & Stats", "Localização do servidor e estatísticas"),
            ("Screen Brightness", "Brilho da tela"),
            ("Adjust screen brightness via overlay (50% is default).", "Ajuste o brilho pela sobreposição (50% é o padrão)."));

        private static readonly Dictionary<string, string> Ms = D(
            ("Mods", "Mods"), ("Manage and apply file mods to the Roblox client.", "Urus dan gunakan mod fail pada klien Roblox."),
            ("Open Mods Folder", "Buka Folder Mods"), ("Manage custom Roblox mods here.", "Urus mod Roblox tersuai di sini."),
            ("Help", "Bantuan"), ("See info about managing and creating mods.", "Lihat info tentang mengurus dan mencipta mods."),
            ("Manage compatibility settings", "Urus tetapan keserasian"), ("Configure application parameters such as DPI scaling behaviour and", "Konfigurasi parameter aplikasi seperti tingkah laku DPI scaling dan"),
            ("fullscreen optimizations.", "fullscreen optimizations."), ("Presets", "Preset"), ("Mouse cursor", "Kursor tetikus"),
            ("Choose between using classic Roblox cursor styles.", "Pilih antara gaya kursor Roblox klasik."),
            ("Use old avatar editor background", "Guna latar editor avatar lama"), ("Bring back the old avatar editor background used in legacy app versions.", "Bawa balik latar editor avatar lama yang digunakan versi terdahulu."),
            ("Emulate old character sounds", "Emulasi bunyi watak lama"), ("Attempt to restore old footsteps and character movement sounds before 2014.", "Cuba pulihkan bunyi langkah kaki dan pergerakan watak lama sebelum 2014."),
            ("Preferred emoji type", "Jenis emoji pilihan"), ("Choose what type of emoji should Roblox use.", "Pilih jenis emoji yang Roblox patut guna."),
            ("Miscellaneous", "Lain-lain"), ("Use custom font", "Guna fon tersuai"), ("Font size can still be adjusted in the Global tab.", "Saiz fon masih boleh dilaras di tab Global."),
            ("Choose font...", "Pilih fon..."), ("Remove", "Buang"), ("Default", "Lalai"), ("From 2006", "Dari 2006"), ("From 2013", "Dari 2013"),
            ("Windows 11", "Windows 11"), ("Windows 10", "Windows 10"), ("Windows 8.1", "Windows 8.1"),
            ("Custom Shiftlock", "Shiftlock tersuai"),
            ("Change the icon when Shift Lock is enabled. (Froststrap/Voidstrap Collection)", "Tukar ikon apabila Shift Lock dihidupkan. (Koleksi Froststrap/Voidstrap)"),
            ("Advanced Modifications", "Pengubahsuaian lanjutan"),
            ("Skybox Preset", "Pratetap skybox"),
            ("Change the in game sky texture. Some presets require a restart.", "Tukar tekstur langit dalam permainan. Sesetengah pratetap memerlukan mula semula."),
            ("Fullbright (No Shadows)", "Fullbright (tiada bayang)"),
            ("Makes everything bright and removes shadows. (Experimental)", "Jadikan semuanya terang dan buang bayang. (Percubaan)"),
            ("Visual Overlays & Stats", "Overlay visual & statistik"),
            ("Lighting Overlays", "Overlay pencahayaan"),
            ("Motion Blur Effect", "Kesan motion blur"),
            ("FPS Counter", "Pembilang FPS"),
            ("Server Location & Stats", "Lokasi pelayan & statistik"),
            ("Screen Brightness", "Kecerahan skrin"),
            ("Adjust screen brightness via overlay (50% is default).", "Laras kecerahan melalui overlay (50% ialah lalai)."));

        private static readonly Dictionary<string, string> Ja = D(
            ("Mods", "Mods"), ("Manage and apply file mods to the Roblox client.", "Roblox クライアントにファイル mod を管理・適用します。"),
            ("Open Mods Folder", "Mods フォルダーを開く"), ("Manage custom Roblox mods here.", "ここでカスタム Roblox mod を管理します。"),
            ("Help", "ヘルプ"), ("See info about managing and creating mods.", "mod の管理と作成に関する情報を表示します。"),
            ("Manage compatibility settings", "互換性設定を管理"), ("Configure application parameters such as DPI scaling behaviour and", "DPI スケーリング動作などのアプリ設定と"),
            ("fullscreen optimizations.", "fullscreen optimizations."), ("Presets", "プリセット"), ("Mouse cursor", "マウスカーソル"),
            ("Choose between using classic Roblox cursor styles.", "クラシック Roblox カーソルスタイルを選択します。"),
            ("Use old avatar editor background", "旧アバターエディタ背景を使用"), ("Bring back the old avatar editor background used in legacy app versions.", "旧バージョンで使われていたアバターエディタ背景を復元します。"),
            ("Emulate old character sounds", "旧キャラクター音を再現"), ("Attempt to restore old footsteps and character movement sounds before 2014.", "2014年以前の足音・移動音を復元します。"),
            ("Preferred emoji type", "優先絵文字タイプ"), ("Choose what type of emoji should Roblox use.", "Roblox が使用する絵文字タイプを選択します。"),
            ("Miscellaneous", "その他"), ("Use custom font", "カスタムフォントを使用"), ("Font size can still be adjusted in the Global tab.", "フォントサイズは Global タブで調整できます。"),
            ("Choose font...", "フォントを選択..."), ("Remove", "削除"), ("Default", "既定"), ("From 2006", "2006年版"), ("From 2013", "2013年版"),
            ("Windows 11", "Windows 11"), ("Windows 10", "Windows 10"), ("Windows 8.1", "Windows 8.1"),
            ("Custom Shiftlock", "カスタム Shiftlock"),
            ("Change the icon when Shift Lock is enabled. (Froststrap/Voidstrap Collection)", "Shift Lock 有効時のアイコンを変更します。（Froststrap/Voidstrap コレクション）"),
            ("Advanced Modifications", "高度な変更"),
            ("Skybox Preset", "スカイボックスのプリセット"),
            ("Change the in game sky texture. Some presets require a restart.", "ゲーム内の空のテクスチャを変更します。一部のプリセットは再起動が必要です。"),
            ("Fullbright (No Shadows)", "フルブライト（影なし）"),
            ("Makes everything bright and removes shadows. (Experimental)", "全体を明るくし影を無くします。（実験的）"),
            ("Visual Overlays & Stats", "ビジュアルオーバーレイと統計"),
            ("Lighting Overlays", "ライティングオーバーレイ"),
            ("Motion Blur Effect", "モーションブラー効果"),
            ("FPS Counter", "FPS カウンター"),
            ("Server Location & Stats", "サーバー所在地と統計"),
            ("Screen Brightness", "画面の明るさ"),
            ("Adjust screen brightness via overlay (50% is default).", "オーバーレイで明るさを調整（50% が既定）。"));

        private static readonly Dictionary<string, string> Zh = D(
            ("Mods", "Mods"), ("Manage and apply file mods to the Roblox client.", "管理并将文件模组应用到 Roblox 客户端。"),
            ("Open Mods Folder", "打开 Mods 文件夹"), ("Manage custom Roblox mods here.", "在这里管理自定义 Roblox 模组。"),
            ("Help", "帮助"), ("See info about managing and creating mods.", "查看管理和创建模组的信息。"),
            ("Manage compatibility settings", "管理兼容性设置"), ("Configure application parameters such as DPI scaling behaviour and", "配置应用参数，例如 DPI 缩放行为和"),
            ("fullscreen optimizations.", "fullscreen optimizations."), ("Presets", "预设"), ("Mouse cursor", "鼠标指针"),
            ("Choose between using classic Roblox cursor styles.", "在经典 Roblox 指针样式中进行选择。"),
            ("Use old avatar editor background", "使用旧版头像编辑器背景"), ("Bring back the old avatar editor background used in legacy app versions.", "恢复旧版应用中使用的头像编辑器背景。"),
            ("Emulate old character sounds", "模拟旧角色音效"), ("Attempt to restore old footsteps and character movement sounds before 2014.", "尝试恢复 2014 年前的脚步和角色移动音效。"),
            ("Preferred emoji type", "首选 emoji 类型"), ("Choose what type of emoji should Roblox use.", "选择 Roblox 应使用的 emoji 类型。"),
            ("Miscellaneous", "其他"), ("Use custom font", "使用自定义字体"), ("Font size can still be adjusted in the Global tab.", "字体大小仍可在 Global 标签调整。"),
            ("Choose font...", "选择字体..."), ("Remove", "移除"), ("Default", "默认"), ("From 2006", "2006 版"), ("From 2013", "2013 版"),
            ("Windows 11", "Windows 11"), ("Windows 10", "Windows 10"), ("Windows 8.1", "Windows 8.1"),
            ("Custom Shiftlock", "自定义 Shiftlock"),
            ("Change the icon when Shift Lock is enabled. (Froststrap/Voidstrap Collection)", "启用 Shift Lock 时更改图标。（Froststrap/Voidstrap 合集）"),
            ("Advanced Modifications", "高级修改"),
            ("Skybox Preset", "天空盒预设"),
            ("Change the in game sky texture. Some presets require a restart.", "更改游戏内天空纹理。部分预设需要重启。"),
            ("Fullbright (No Shadows)", "全亮（无阴影）"),
            ("Makes everything bright and removes shadows. (Experimental)", "提高整体亮度并移除阴影。（实验性）"),
            ("Visual Overlays & Stats", "视觉叠加与统计"),
            ("Lighting Overlays", "光照叠加"),
            ("Motion Blur Effect", "动态模糊效果"),
            ("FPS Counter", "FPS 计数器"),
            ("Server Location & Stats", "服务器位置与统计"),
            ("Screen Brightness", "屏幕亮度"),
            ("Adjust screen brightness via overlay (50% is default).", "通过叠加层调节亮度（默认 50%）。"));

        private static readonly Dictionary<string, string> Th = D(
            ("Mods", "Mods"), ("Manage and apply file mods to the Roblox client.", "จัดการและใช้งานไฟล์ม็อดกับไคลเอนต์ Roblox."),
            ("Open Mods Folder", "เปิดโฟลเดอร์ Mods"), ("Manage custom Roblox mods here.", "จัดการม็อด Roblox แบบกำหนดเองที่นี่."),
            ("Help", "ช่วยเหลือ"), ("See info about managing and creating mods.", "ดูข้อมูลเกี่ยวกับการจัดการและสร้างม็อด."),
            ("Manage compatibility settings", "จัดการการตั้งค่าความเข้ากันได้"), ("Configure application parameters such as DPI scaling behaviour and", "กำหนดค่าพารามิเตอร์แอป เช่น พฤติกรรม DPI scaling และ"),
            ("fullscreen optimizations.", "fullscreen optimizations."), ("Presets", "พรีเซ็ต"), ("Mouse cursor", "เคอร์เซอร์เมาส์"),
            ("Choose between using classic Roblox cursor styles.", "เลือกใช้รูปแบบเคอร์เซอร์ Roblox แบบคลาสสิก."),
            ("Use old avatar editor background", "ใช้พื้นหลังตัวแก้ไขอวตารแบบเก่า"), ("Bring back the old avatar editor background used in legacy app versions.", "นำพื้นหลังตัวแก้ไขอวตารเก่าที่ใช้ในเวอร์ชันก่อนกลับมา."),
            ("Emulate old character sounds", "จำลองเสียงตัวละครแบบเก่า"), ("Attempt to restore old footsteps and character movement sounds before 2014.", "พยายามคืนเสียงก้าวเท้าและการเคลื่อนไหวก่อนปี 2014."),
            ("Preferred emoji type", "ประเภทอีโมจิที่ต้องการ"), ("Choose what type of emoji should Roblox use.", "เลือกประเภทอีโมจิที่ Roblox ควรใช้."),
            ("Miscellaneous", "อื่นๆ"), ("Use custom font", "ใช้ฟอนต์กำหนดเอง"), ("Font size can still be adjusted in the Global tab.", "ขนาดฟอนต์ยังปรับได้ในแท็บ Global."),
            ("Choose font...", "เลือกฟอนต์..."), ("Remove", "ลบ"), ("Default", "ค่าเริ่มต้น"), ("From 2006", "จากปี 2006"), ("From 2013", "จากปี 2013"),
            ("Windows 11", "Windows 11"), ("Windows 10", "Windows 10"), ("Windows 8.1", "Windows 8.1"),
            ("Custom Shiftlock", "Shiftlock แบบกำหนดเอง"),
            ("Change the icon when Shift Lock is enabled. (Froststrap/Voidstrap Collection)", "เปลี่ยนไอคอนเมื่อเปิด Shift Lock (คอลเลกชัน Froststrap/Voidstrap)"),
            ("Advanced Modifications", "การปรับแต่งขั้นสูง"),
            ("Skybox Preset", "พรีเซ็ตสกายบ็อกซ์"),
            ("Change the in game sky texture. Some presets require a restart.", "เปลี่ยนเท็กซ์เจอร์ท้องฟ้าในเกม บางพรีเซ็ตต้องรีสตาร์ท"),
            ("Fullbright (No Shadows)", "ฟูลไบรต์ (ไม่มีเงา)"),
            ("Makes everything bright and removes shadows. (Experimental)", "ทำให้สว่างขึ้นและเอาเงาออก (ทดลอง)"),
            ("Visual Overlays & Stats", "โอเวอร์เลย์ภาพและสถิติ"),
            ("Lighting Overlays", "โอเวอร์เลย์แสง"),
            ("Motion Blur Effect", "เอฟเฟกต์มอชันบลัร์"),
            ("FPS Counter", "ตัวนับ FPS"),
            ("Server Location & Stats", "ตำแหน่งเซิร์ฟเวอร์และสถิติ"),
            ("Screen Brightness", "ความสว่างหน้าจอ"),
            ("Adjust screen brightness via overlay (50% is default).", "ปรับความสว่างผ่านโอเวอร์เลย์ (ค่าเริ่มต้น 50%)"));

        private static readonly Dictionary<string, string> Km = D(("Mods", "Mods"), ("Manage and apply file mods to the Roblox client.", "គ្រប់គ្រង និងអនុវត្តម៉ូដឯកសារទៅកាន់ Roblox client."),
            ("Open Mods Folder", "បើកថត Mods"), ("Manage custom Roblox mods here.", "គ្រប់គ្រងម៉ូដ Roblox ផ្ទាល់ខ្លួននៅទីនេះ."),
            ("Help", "ជំនួយ"), ("See info about managing and creating mods.", "មើលព័ត៌មានអំពីការគ្រប់គ្រង និងបង្កើតម៉ូដ."),
            ("Manage compatibility settings", "គ្រប់គ្រងការកំណត់ភាពឆបគ្នា"), ("Configure application parameters such as DPI scaling behaviour and", "កំណត់ប៉ារ៉ាម៉ែត្រកម្មវិធី ដូចជា DPI scaling និង"),
            ("fullscreen optimizations.", "fullscreen optimizations."), ("Presets", "Preset"), ("Mouse cursor", "ទ្រនិចកណ្ដុរ"),
            ("Choose between using classic Roblox cursor styles.", "ជ្រើសរើសរវាងរចនាប័ទ្មទ្រនិច Roblox បែបបុរាណ."),
            ("Use old avatar editor background", "ប្រើផ្ទៃខាងក្រោយកម្មវិធីកែ avatar ចាស់"), ("Bring back the old avatar editor background used in legacy app versions.", "នាំត្រឡប់ផ្ទៃខាងក្រោយកម្មវិធីកែ avatar ចាស់."),
            ("Emulate old character sounds", "ត្រាប់តាមសំឡេងតួអង្គចាស់"), ("Attempt to restore old footsteps and character movement sounds before 2014.", "ព្យាយាមស្ដារសំឡេងជើង និងចលនាតួអង្គមុន 2014."),
            ("Preferred emoji type", "ប្រភេទ emoji ដែលចូលចិត្ត"), ("Choose what type of emoji should Roblox use.", "ជ្រើសរើសប្រភេទ emoji ដែល Roblox គួរប្រើ."),
            ("Miscellaneous", "ផ្សេងៗ"), ("Use custom font", "ប្រើពុម្ពអក្សរផ្ទាល់ខ្លួន"), ("Font size can still be adjusted in the Global tab.", "ទំហំអក្សរនៅតែអាចកែបាននៅផ្ទាំង Global."),
            ("Choose font...", "ជ្រើសពុម្ពអក្សរ..."), ("Remove", "លុប"), ("Default", "លំនាំដើម"), ("From 2006", "ពី 2006"), ("From 2013", "ពី 2013"),
            ("Windows 11", "Windows 11"), ("Windows 10", "Windows 10"), ("Windows 8.1", "Windows 8.1"),
            ("Custom Shiftlock", "Shiftlock ផ្ទាល់ខ្លួន"),
            ("Change the icon when Shift Lock is enabled. (Froststrap/Voidstrap Collection)", "ផ្លាស់ប្តូររូបតំណាងនៅពេលបើក Shift Lock។ (ការប្រមូល Froststrap/Voidstrap)"),
            ("Advanced Modifications", "ការកែប្រែកម្រិតខ្ពស់"),
            ("Skybox Preset", "ការកំណត់ជាមុន skybox"),
            ("Change the in game sky texture. Some presets require a restart.", "ផ្លាស់ប្តូរវាលរូបភាពមេឃក្នុងហ្គេម។ ការកំណត់មួយចំនួនត្រូវការចាប់ផ្តើមឡើងវិញ។"),
            ("Fullbright (No Shadows)", "Fullbright (គ្មានស្រមោល)"),
            ("Makes everything bright and removes shadows. (Experimental)", "ធ្វើឱ្យភ្លឺឡើង និងយកស្រមោលចេញ។ (សាកល្បង)"),
            ("Visual Overlays & Stats", "ស្រទាប់រូបភាព និងស្ថិតិ"),
            ("Lighting Overlays", "ស្រទាប់ពន្លឺ"),
            ("Motion Blur Effect", "បែបផែន blur ចលនា"),
            ("FPS Counter", "ឧបករណ៍រាប់ FPS"),
            ("Server Location & Stats", "ទីតាំងម៉ាស៊ីនមេ និងស្ថិតិ"),
            ("Screen Brightness", "ពន្លឺអេក្រង់"),
            ("Adjust screen brightness via overlay (50% is default).", "កែពន្លឺតាមស្រទាប់ (50% ជាលំនាំដើម)។"));

        private static readonly Dictionary<string, string> Lo = D(("Mods", "Mods"), ("Manage and apply file mods to the Roblox client.", "ຈັດການ ແລະ ນຳໃຊ້ mod ໄຟລ໌ກັບ Roblox client."),
            ("Open Mods Folder", "ເປີດໂຟນເດີ Mods"), ("Manage custom Roblox mods here.", "ຈັດການ mod Roblox ແບບກຳນົດເອງທີ່ນີ້."),
            ("Help", "ຊ່ວຍເຫຼືອ"), ("See info about managing and creating mods.", "ເບິ່ງຂໍ້ມູນເກືອບກັບການຈັດການ ແລະ ສ້າງ mods."),
            ("Manage compatibility settings", "ຈັດການການຕັ້ງຄ່າຄວາມເຂົ້າກັນໄດ້"), ("Configure application parameters such as DPI scaling behaviour and", "ຕັ້ງຄ່າພາຣາມິເຕີແອັບ ເຊັ່ນ DPI scaling ແລະ"),
            ("fullscreen optimizations.", "fullscreen optimizations."), ("Presets", "ພຣີເຊັດ"), ("Mouse cursor", "ເຄີເຊີເມົາສ໌"),
            ("Choose between using classic Roblox cursor styles.", "ເລືອກຮູບແບບເຄີເຊີ Roblox ແບບຄລາສສິກ."),
            ("Use old avatar editor background", "ໃຊ້ພື້ນຫຼັງຕົວແກ້ avatar ເກົ່າ"), ("Bring back the old avatar editor background used in legacy app versions.", "ນຳພື້ນຫຼັງຕົວແກ້ avatar ເກົ່າກັບຄືນ."),
            ("Emulate old character sounds", "ຈຳລອງສຽງຕົວລະຄອນເກົ່າ"), ("Attempt to restore old footsteps and character movement sounds before 2014.", "ພະຍາຍามກູ້ຄືນສຽງກ້າວເທົ້າ ແລະ ສຽງເຄື່ອນໄຫວກ່ອນ 2014."),
            ("Preferred emoji type", "ປະເພດ emoji ທີ່ຕ້ອງການ"), ("Choose what type of emoji should Roblox use.", "ເລືອກປະເພດ emoji ທີ່ Roblox ຄວນໃຊ້."),
            ("Miscellaneous", "ອື່ນໆ"), ("Use custom font", "ໃຊ້ຟອນກຳນົດເเอง"), ("Font size can still be adjusted in the Global tab.", "ຂະໜາດຟອນຍັງປັບໄດ້ໃນແຖບ Global."),
            ("Choose font...", "ເລືອກຟອນ..."), ("Remove", "ລຶບ"), ("Default", "ຄ່າເລີ່ມຕົ້ນ"), ("From 2006", "ຈາກ 2006"), ("From 2013", "ຈາກ 2013"),
            ("Windows 11", "Windows 11"), ("Windows 10", "Windows 10"), ("Windows 8.1", "Windows 8.1"),
            ("Custom Shiftlock", "Shiftlock ກຳນົດເອງ"),
            ("Change the icon when Shift Lock is enabled. (Froststrap/Voidstrap Collection)", "ປ່ຽນໄອຄອນເມື່ອເປີດ Shift Lock (ຊຸດ Froststrap/Voidstrap)"),
            ("Advanced Modifications", "ການປັບແຕ່ງຂັ້ນສູງ"),
            ("Skybox Preset", "ພຣີເຊັດ skybox"),
            ("Change the in game sky texture. Some presets require a restart.", "ປ່ຽນເທັກເຈີຟ້າໃນເກມ. ບາງພຣີເຊັດຕ້ອງເລີ່ມໃໝ່."),
            ("Fullbright (No Shadows)", "Fullbright (ບໍ່ມີເງົາ)"),
            ("Makes everything bright and removes shadows. (Experimental)", "ເຮັດໃຫ້ສະຫວ່າງ ແລະ ລຶບເງົາ (ທົດລອງ)"),
            ("Visual Overlays & Stats", "ໂອເວເລย໌ພາບ ແລະ ສະຖິຕິ"),
            ("Lighting Overlays", "ໂອເວເລย໌ແສງ"),
            ("Motion Blur Effect", "ເອັບເຟັກ motion blur"),
            ("FPS Counter", "ຕົວນັບ FPS"),
            ("Server Location & Stats", "ທີ່ຕັ້ງເຊີເວີ ແລະ ສະຖິຕິ"),
            ("Screen Brightness", "ຄວາມສະຫວ່າງໜ້າຈໍ"),
            ("Adjust screen brightness via overlay (50% is default).", "ປັບຄວາມສະຫວ່າງດ້ວຍໂອເວເລย໌ (50% ເປັນຄ່າເລີ່ມຕົ້ນ)."));

        private static readonly Dictionary<string, string> Ko = D(("Mods", "Mods"), ("Manage and apply file mods to the Roblox client.", "Roblox 클라이언트에 파일 모드를 관리하고 적용합니다."),
            ("Open Mods Folder", "Mods 폴더 열기"), ("Manage custom Roblox mods here.", "여기에서 사용자 지정 Roblox 모드를 관리합니다."),
            ("Help", "도움말"), ("See info about managing and creating mods.", "모드 관리 및 생성 정보를 확인하세요."),
            ("Manage compatibility settings", "호환성 설정 관리"), ("Configure application parameters such as DPI scaling behaviour and", "DPI 스케일링 동작 등의 애플리케이션 매개변수와"),
            ("fullscreen optimizations.", "fullscreen optimizations."), ("Presets", "프리셋"), ("Mouse cursor", "마우스 커서"),
            ("Choose between using classic Roblox cursor styles.", "클래식 Roblox 커서 스타일을 선택하세요."),
            ("Use old avatar editor background", "구형 아바타 편집기 배경 사용"), ("Bring back the old avatar editor background used in legacy app versions.", "이전 버전 앱에서 사용하던 아바타 편집기 배경을 복원합니다."),
            ("Emulate old character sounds", "옛 캐릭터 사운드 에뮬레이션"), ("Attempt to restore old footsteps and character movement sounds before 2014.", "2014년 이전의 발소리와 이동 사운드를 복원합니다."),
            ("Preferred emoji type", "선호 이모지 유형"), ("Choose what type of emoji should Roblox use.", "Roblox에서 사용할 이모지 유형을 선택하세요."),
            ("Miscellaneous", "기타"), ("Use custom font", "사용자 지정 글꼴 사용"), ("Font size can still be adjusted in the Global tab.", "글꼴 크기는 Global 탭에서 조정할 수 있습니다."),
            ("Choose font...", "글꼴 선택..."), ("Remove", "제거"), ("Default", "기본값"), ("From 2006", "2006 스타일"), ("From 2013", "2013 스타일"),
            ("Windows 11", "Windows 11"), ("Windows 10", "Windows 10"), ("Windows 8.1", "Windows 8.1"),
            ("Custom Shiftlock", "사용자 지정 Shiftlock"),
            ("Change the icon when Shift Lock is enabled. (Froststrap/Voidstrap Collection)", "Shift Lock 사용 시 아이콘 변경(Froststrap/Voidstrap 컬렉션)."),
            ("Advanced Modifications", "고급 수정"),
            ("Skybox Preset", "스카이박스 프리셋"),
            ("Change the in game sky texture. Some presets require a restart.", "게임 내 하늘 텍스처를 변경합니다. 일부 프리셋은 재시작이 필요합니다."),
            ("Fullbright (No Shadows)", "풀브라이트(그림자 없음)"),
            ("Makes everything bright and removes shadows. (Experimental)", "전체를 밝게 하고 그림자를 제거합니다.(실험적)"),
            ("Visual Overlays & Stats", "비주얼 오버레이 및 통계"),
            ("Lighting Overlays", "조명 오버레이"),
            ("Motion Blur Effect", "모션 블러 효과"),
            ("FPS Counter", "FPS 카운터"),
            ("Server Location & Stats", "서버 위치 및 통계"),
            ("Screen Brightness", "화면 밝기"),
            ("Adjust screen brightness via overlay (50% is default).", "오버레이로 밝기 조절(기본 50%)."));

        private static readonly Dictionary<string, string> Ru = D(("Mods", "Mods"), ("Manage and apply file mods to the Roblox client.", "Управляйте и применяйте файловые моды к клиенту Roblox."),
            ("Open Mods Folder", "Открыть папку Mods"), ("Manage custom Roblox mods here.", "Управляйте пользовательскими модами Roblox здесь."),
            ("Help", "Помощь"), ("See info about managing and creating mods.", "Информация о создании и управлении модами."),
            ("Manage compatibility settings", "Управление параметрами совместимости"), ("Configure application parameters such as DPI scaling behaviour and", "Настройка параметров приложения, таких как DPI scaling и"),
            ("fullscreen optimizations.", "fullscreen optimizations."), ("Presets", "Пресеты"), ("Mouse cursor", "Курсор мыши"),
            ("Choose between using classic Roblox cursor styles.", "Выберите классический стиль курсора Roblox."),
            ("Use old avatar editor background", "Использовать старый фон редактора аватара"), ("Bring back the old avatar editor background used in legacy app versions.", "Вернуть старый фон редактора аватара из прошлых версий."),
            ("Emulate old character sounds", "Эмулировать старые звуки персонажа"), ("Attempt to restore old footsteps and character movement sounds before 2014.", "Попытка восстановить старые шаги и звуки движения до 2014."),
            ("Preferred emoji type", "Предпочитаемый тип эмодзи"), ("Choose what type of emoji should Roblox use.", "Выберите тип эмодзи для Roblox."),
            ("Miscellaneous", "Разное"), ("Use custom font", "Использовать свой шрифт"), ("Font size can still be adjusted in the Global tab.", "Размер шрифта можно менять во вкладке Global."),
            ("Choose font...", "Выбрать шрифт..."), ("Remove", "Удалить"), ("Default", "По умолчанию"), ("From 2006", "Из 2006"), ("From 2013", "Из 2013"),
            ("Windows 11", "Windows 11"), ("Windows 10", "Windows 10"), ("Windows 8.1", "Windows 8.1"),
            ("Custom Shiftlock", "Пользовательский Shiftlock"),
            ("Change the icon when Shift Lock is enabled. (Froststrap/Voidstrap Collection)", "Сменить значок при включённом Shift Lock (коллекция Froststrap/Voidstrap)."),
            ("Advanced Modifications", "Расширенные изменения"),
            ("Skybox Preset", "Пресет скайбокса"),
            ("Change the in game sky texture. Some presets require a restart.", "Меняет текстуру неба в игре. Некоторые пресеты требуют перезапуска."),
            ("Fullbright (No Shadows)", "Полное освещение (без теней)"),
            ("Makes everything bright and removes shadows. (Experimental)", "Делает сцену ярче и убирает тени. (Экспериментально)"),
            ("Visual Overlays & Stats", "Визуальные оверлеи и статистика"),
            ("Lighting Overlays", "Оверлеи освещения"),
            ("Motion Blur Effect", "Эффект размытия в движении"),
            ("FPS Counter", "Счётчик FPS"),
            ("Server Location & Stats", "Расположение сервера и статистика"),
            ("Screen Brightness", "Яркость экрана"),
            ("Adjust screen brightness via overlay (50% is default).", "Настройка яркости через оверлей (по умолчанию 50%)."));

        private static readonly Dictionary<string, string> Uk = D(("Mods", "Mods"), ("Manage and apply file mods to the Roblox client.", "Керуйте та застосовуйте файлові моди до клієнта Roblox."),
            ("Open Mods Folder", "Відкрити папку Mods"), ("Manage custom Roblox mods here.", "Керуйте користувацькими модами Roblox тут."),
            ("Help", "Довідка"), ("See info about managing and creating mods.", "Дивіться інформацію про керування та створення модів."),
            ("Manage compatibility settings", "Керування налаштуваннями сумісності"), ("Configure application parameters such as DPI scaling behaviour and", "Налаштуйте параметри застосунку, зокрема DPI scaling та"),
            ("fullscreen optimizations.", "fullscreen optimizations."), ("Presets", "Пресети"), ("Mouse cursor", "Курсор миші"),
            ("Choose between using classic Roblox cursor styles.", "Оберіть класичні стилі курсора Roblox."),
            ("Use old avatar editor background", "Використовувати старий фон редактора аватара"), ("Bring back the old avatar editor background used in legacy app versions.", "Повернути старий фон редактора аватара з попередніх версій."),
            ("Emulate old character sounds", "Емуляція старих звуків персонажа"), ("Attempt to restore old footsteps and character movement sounds before 2014.", "Спроба відновити старі звуки кроків і руху до 2014."),
            ("Preferred emoji type", "Бажаний тип emoji"), ("Choose what type of emoji should Roblox use.", "Оберіть тип emoji для Roblox."),
            ("Miscellaneous", "Інше"), ("Use custom font", "Використовувати власний шрифт"), ("Font size can still be adjusted in the Global tab.", "Розмір шрифту можна змінити у вкладці Global."),
            ("Choose font...", "Обрати шрифт..."), ("Remove", "Видалити"), ("Default", "Типово"), ("From 2006", "З 2006"), ("From 2013", "З 2013"),
            ("Windows 11", "Windows 11"), ("Windows 10", "Windows 10"), ("Windows 8.1", "Windows 8.1"),
            ("Custom Shiftlock", "Власний Shiftlock"),
            ("Change the icon when Shift Lock is enabled. (Froststrap/Voidstrap Collection)", "Змінити піктограму, коли увімкнено Shift Lock (колекція Froststrap/Voidstrap)."),
            ("Advanced Modifications", "Розширені зміни"),
            ("Skybox Preset", "Пресет скайбокса"),
            ("Change the in game sky texture. Some presets require a restart.", "Змінює текстуру неба в грі. Деякі пресети потребують перезапуску."),
            ("Fullbright (No Shadows)", "Повне освітлення (без тіней)"),
            ("Makes everything bright and removes shadows. (Experimental)", "Робить сцену яскравішою й прибирає тіні. (Експериментально)"),
            ("Visual Overlays & Stats", "Візуальні оверлеї та статистика"),
            ("Lighting Overlays", "Оверлеї освітлення"),
            ("Motion Blur Effect", "Ефект розмиття руху"),
            ("FPS Counter", "Лічильник FPS"),
            ("Server Location & Stats", "Розташування сервера та статистика"),
            ("Screen Brightness", "Яскравість екрана"),
            ("Adjust screen brightness via overlay (50% is default).", "Налаштування яскравості через оверлей (типово 50%)."));

        private static readonly Dictionary<string, string> Es = D(("Mods", "Mods"), ("Manage and apply file mods to the Roblox client.", "Administra y aplica mods de archivos al cliente de Roblox."),
            ("Open Mods Folder", "Abrir carpeta Mods"), ("Manage custom Roblox mods here.", "Administra aquí tus mods personalizados de Roblox."),
            ("Help", "Ayuda"), ("See info about managing and creating mods.", "Consulta información sobre gestión y creación de mods."),
            ("Manage compatibility settings", "Gestionar configuración de compatibilidad"), ("Configure application parameters such as DPI scaling behaviour and", "Configura parámetros de la aplicación como el comportamiento de DPI scaling y"),
            ("fullscreen optimizations.", "fullscreen optimizations."), ("Presets", "Preajustes"), ("Mouse cursor", "Cursor del mouse"),
            ("Choose between using classic Roblox cursor styles.", "Elige entre estilos de cursor clásico de Roblox."),
            ("Use old avatar editor background", "Usar fondo antiguo del editor de avatar"), ("Bring back the old avatar editor background used in legacy app versions.", "Recupera el fondo antiguo del editor de avatar usado en versiones anteriores."),
            ("Emulate old character sounds", "Emular sonidos antiguos de personaje"), ("Attempt to restore old footsteps and character movement sounds before 2014.", "Intenta restaurar sonidos de pasos y movimiento anteriores a 2014."),
            ("Preferred emoji type", "Tipo de emoji preferido"), ("Choose what type of emoji should Roblox use.", "Elige qué tipo de emoji debe usar Roblox."),
            ("Miscellaneous", "Misceláneo"), ("Use custom font", "Usar fuente personalizada"), ("Font size can still be adjusted in the Global tab.", "El tamaño de fuente aún se puede ajustar en la pestaña Global."),
            ("Choose font...", "Elegir fuente..."), ("Remove", "Quitar"), ("Default", "Predeterminado"), ("From 2006", "Desde 2006"), ("From 2013", "Desde 2013"),
            ("Windows 11", "Windows 11"), ("Windows 10", "Windows 10"), ("Windows 8.1", "Windows 8.1"),
            ("Custom Shiftlock", "Shiftlock personalizado"),
            ("Change the icon when Shift Lock is enabled. (Froststrap/Voidstrap Collection)", "Cambia el icono cuando Shift Lock está activo. (Colección Froststrap/Voidstrap)"),
            ("Advanced Modifications", "Modificaciones avanzadas"),
            ("Skybox Preset", "Preset de skybox"),
            ("Change the in game sky texture. Some presets require a restart.", "Cambia la textura del cielo en el juego. Algunos presets requieren reinicio."),
            ("Fullbright (No Shadows)", "Fullbright (sin sombras)"),
            ("Makes everything bright and removes shadows. (Experimental)", "Aclara todo y quita sombras. (Experimental)"),
            ("Visual Overlays & Stats", "Superposiciones visuales y estadísticas"),
            ("Lighting Overlays", "Superposiciones de iluminación"),
            ("Motion Blur Effect", "Efecto de desenfoque de movimiento"),
            ("FPS Counter", "Contador de FPS"),
            ("Server Location & Stats", "Ubicación del servidor y estadísticas"),
            ("Screen Brightness", "Brillo de pantalla"),
            ("Adjust screen brightness via overlay (50% is default).", "Ajusta el brillo con superposición (50% por defecto)."));

        private static readonly Dictionary<string, string> Fr = D(("Mods", "Mods"), ("Manage and apply file mods to the Roblox client.", "Gérez et appliquez des mods de fichiers au client Roblox."),
            ("Open Mods Folder", "Ouvrir le dossier Mods"), ("Manage custom Roblox mods here.", "Gérez ici vos mods Roblox personnalisés."),
            ("Help", "Aide"), ("See info about managing and creating mods.", "Voir les infos pour gérer et créer des mods."),
            ("Manage compatibility settings", "Gérer les paramètres de compatibilité"), ("Configure application parameters such as DPI scaling behaviour and", "Configurer les paramètres de l'application comme le DPI scaling et"),
            ("fullscreen optimizations.", "fullscreen optimizations."), ("Presets", "Préréglages"), ("Mouse cursor", "Curseur souris"),
            ("Choose between using classic Roblox cursor styles.", "Choisissez parmi les styles de curseur Roblox classiques."),
            ("Use old avatar editor background", "Utiliser l'ancien fond d'éditeur d'avatar"), ("Bring back the old avatar editor background used in legacy app versions.", "Rétablir l'ancien fond d'éditeur d'avatar des versions précédentes."),
            ("Emulate old character sounds", "Émuler les anciens sons de personnage"), ("Attempt to restore old footsteps and character movement sounds before 2014.", "Tenter de restaurer les sons de pas et de mouvement d'avant 2014."),
            ("Preferred emoji type", "Type d'emoji préféré"), ("Choose what type of emoji should Roblox use.", "Choisissez le type d'emoji que Roblox doit utiliser."),
            ("Miscellaneous", "Divers"), ("Use custom font", "Utiliser une police personnalisée"), ("Font size can still be adjusted in the Global tab.", "La taille de police peut encore être ajustée dans l'onglet Global."),
            ("Choose font...", "Choisir une police..."), ("Remove", "Retirer"), ("Default", "Par défaut"), ("From 2006", "Depuis 2006"), ("From 2013", "Depuis 2013"),
            ("Windows 11", "Windows 11"), ("Windows 10", "Windows 10"), ("Windows 8.1", "Windows 8.1"),
            ("Custom Shiftlock", "Shiftlock personnalisé"),
            ("Change the icon when Shift Lock is enabled. (Froststrap/Voidstrap Collection)", "Change l’icône quand Shift Lock est activé. (Collection Froststrap/Voidstrap)"),
            ("Advanced Modifications", "Modifications avancées"),
            ("Skybox Preset", "Préréglage skybox"),
            ("Change the in game sky texture. Some presets require a restart.", "Modifie la texture du ciel en jeu. Certains préréglages nécessitent un redémarrage."),
            ("Fullbright (No Shadows)", "Plein éclairage (sans ombres)"),
            ("Makes everything bright and removes shadows. (Experimental)", "Rend la scène plus lumineuse et supprime les ombres. (Expérimental)"),
            ("Visual Overlays & Stats", "Superpositions visuelles et statistiques"),
            ("Lighting Overlays", "Superpositions d’éclairage"),
            ("Motion Blur Effect", "Flou de mouvement"),
            ("FPS Counter", "Compteur FPS"),
            ("Server Location & Stats", "Emplacement du serveur et statistiques"),
            ("Screen Brightness", "Luminosité de l’écran"),
            ("Adjust screen brightness via overlay (50% is default).", "Réglez la luminosité via une superposition (50% par défaut)."));

        private static readonly Dictionary<string, string> He = D(("Mods", "Mods"), ("Manage and apply file mods to the Roblox client.", "ניהול והחלת מודים מבוססי קבצים על לקוח Roblox."),
            ("Open Mods Folder", "פתח תיקיית Mods"), ("Manage custom Roblox mods here.", "נהל כאן מודים מותאמים אישית של Roblox."),
            ("Help", "עזרה"), ("See info about managing and creating mods.", "ראה מידע על ניהול ויצירת מודים."),
            ("Manage compatibility settings", "ניהול הגדרות תאימות"), ("Configure application parameters such as DPI scaling behaviour and", "הגدر פרמترים של האפליקציה כמו DPI scaling ו-"),
            ("fullscreen optimizations.", "fullscreen optimizations."), ("Presets", "תצורות"), ("Mouse cursor", "סמן עכבר"),
            ("Choose between using classic Roblox cursor styles.", "בחר בין סגנונות הסמן הקלאסיים של Roblox."),
            ("Use old avatar editor background", "השתמש ברקע עורך אווטאר ישן"), ("Bring back the old avatar editor background used in legacy app versions.", "החזר את רקע עורך האווטאר הישן ששימש בגרسאות קודמות."),
            ("Emulate old character sounds", "אמולציה לצליلي דמות ישנים"), ("Attempt to restore old footsteps and character movement sounds before 2014.", "ניסיון לשחזר צליلي צעדים ותנועה מלפני 2014."),
            ("Preferred emoji type", "סוג אימוג'י מועדף"), ("Choose what type of emoji should Roblox use.", "בחר איזה סוג אימוג'י Roblox ישתמש."),
            ("Miscellaneous", "שונות"), ("Use custom font", "השתמש בגופן מותאם אישית"), ("Font size can still be adjusted in the Global tab.", "עדיין ניתן לכוון גودל גופן בלشונית Global."),
            ("Choose font...", "בחר גופן..."), ("Remove", "הסר"), ("Default", "ברירת מחדל"), ("From 2006", "מ-2006"), ("From 2013", "מ-2013"),
            ("Windows 11", "Windows 11"), ("Windows 10", "Windows 10"), ("Windows 8.1", "Windows 8.1"),
            ("Custom Shiftlock", "Shiftlock מותאם"),
            ("Change the icon when Shift Lock is enabled. (Froststrap/Voidstrap Collection)", "שנה את הסמל כש-Shift Lock פעיל. (אוסף Froststrap/Voidstrap)"),
            ("Advanced Modifications", "שינויים מתקדמים"),
            ("Skybox Preset", "ערכת skybox"),
            ("Change the in game sky texture. Some presets require a restart.", "משנה את מרקם השמיים במשחק. חלק מהערכות דורשות הפעלה מחדש."),
            ("Fullbright (No Shadows)", "Fullbright (ללא צללים)"),
            ("Makes everything bright and removes shadows. (Experimental)", "מבהיר ומסיר צללים. (ניסיוני)"),
            ("Visual Overlays & Stats", "שכבות ויזואליות וסטטיסטיקות"),
            ("Lighting Overlays", "שכבות תאורה"),
            ("Motion Blur Effect", "אפקט טשטוש תנועה"),
            ("FPS Counter", "מונה FPS"),
            ("Server Location & Stats", "מיקום שרת וסטטיסטיקות"),
            ("Screen Brightness", "בהירות המסך"),
            ("Adjust screen brightness via overlay (50% is default).", "כוונן בהירות דרך שכבת-על (ברירת מחדל 50%)."));

        private static readonly Dictionary<string, string> Tw = D(("Mods", "Mods"), ("Manage and apply file mods to the Roblox client.", "管理並套用檔案模組到 Roblox 用戶端。"),
            ("Open Mods Folder", "開啟 Mods 資料夾"), ("Manage custom Roblox mods here.", "在此管理自訂 Roblox 模組。"),
            ("Help", "說明"), ("See info about managing and creating mods.", "查看管理與建立模組的資訊。"),
            ("Manage compatibility settings", "管理相容性設定"), ("Configure application parameters such as DPI scaling behaviour and", "設定應用程式參數，例如 DPI scaling 行為與"),
            ("fullscreen optimizations.", "fullscreen optimizations."), ("Presets", "預設組"), ("Mouse cursor", "滑鼠游標"),
            ("Choose between using classic Roblox cursor styles.", "選擇經典 Roblox 游標樣式。"),
            ("Use old avatar editor background", "使用舊版頭像編輯器背景"), ("Bring back the old avatar editor background used in legacy app versions.", "還原舊版應用使用的頭像編輯器背景。"),
            ("Emulate old character sounds", "模擬舊版角色音效"), ("Attempt to restore old footsteps and character movement sounds before 2014.", "嘗試還原 2014 年前的腳步與角色移動音效。"),
            ("Preferred emoji type", "偏好 emoji 類型"), ("Choose what type of emoji should Roblox use.", "選擇 Roblox 要使用的 emoji 類型。"),
            ("Miscellaneous", "其他"), ("Use custom font", "使用自訂字型"), ("Font size can still be adjusted in the Global tab.", "字型大小仍可在 Global 分頁調整。"),
            ("Choose font...", "選擇字型..."), ("Remove", "移除"), ("Default", "預設"), ("From 2006", "2006 版本"), ("From 2013", "2013 版本"),
            ("Windows 11", "Windows 11"), ("Windows 10", "Windows 10"), ("Windows 8.1", "Windows 8.1"),
            ("Custom Shiftlock", "自訂 Shiftlock"),
            ("Change the icon when Shift Lock is enabled. (Froststrap/Voidstrap Collection)", "啟用 Shift Lock 時變更圖示。（Froststrap／Voidstrap 收藏）"),
            ("Advanced Modifications", "進階修改"),
            ("Skybox Preset", "天空盒預設組"),
            ("Change the in game sky texture. Some presets require a restart.", "變更遊戲內天空紋理。部分預設需重新啟動。"),
            ("Fullbright (No Shadows)", "全亮（無陰影）"),
            ("Makes everything bright and removes shadows. (Experimental)", "提高亮度並移除陰影。（實驗性）"),
            ("Visual Overlays & Stats", "視覺疊加與統計"),
            ("Lighting Overlays", "光照疊加"),
            ("Motion Blur Effect", "動態模糊效果"),
            ("FPS Counter", "FPS 計數器"),
            ("Server Location & Stats", "伺服器位置與統計"),
            ("Screen Brightness", "螢幕亮度"),
            ("Adjust screen brightness via overlay (50% is default).", "透過疊加層調整亮度（預設 50%）。"));

        private static readonly Dictionary<string, string> Tr = D(("Mods", "Mods"), ("Manage and apply file mods to the Roblox client.", "Dosya modlarını Roblox istemcisine yönetin ve uygulayın."),
            ("Open Mods Folder", "Mods klasörünü aç"), ("Manage custom Roblox mods here.", "Özel Roblox modlarını burada yönetin."),
            ("Help", "Yardım"), ("See info about managing and creating mods.", "Mod yönetimi ve oluşturma hakkında bilgileri görün."),
            ("Manage compatibility settings", "Uyumluluk ayarlarını yönet"), ("Configure application parameters such as DPI scaling behaviour and", "DPI scaling davranışı gibi uygulama parametrelerini ve"),
            ("fullscreen optimizations.", "fullscreen optimizations."), ("Presets", "Önayarlar"), ("Mouse cursor", "Fare imleci"),
            ("Choose between using classic Roblox cursor styles.", "Klasik Roblox imleç stilleri arasında seçim yapın."),
            ("Use old avatar editor background", "Eski avatar editörü arka planını kullan"), ("Bring back the old avatar editor background used in legacy app versions.", "Eski uygulama sürümlerinde kullanılan avatar editörü arka planını geri getirir."),
            ("Emulate old character sounds", "Eski karakter seslerini taklit et"), ("Attempt to restore old footsteps and character movement sounds before 2014.", "2014 öncesi adım ve hareket seslerini geri yüklemeyi dener."),
            ("Preferred emoji type", "Tercih edilen emoji türü"), ("Choose what type of emoji should Roblox use.", "Roblox'un hangi emoji türünü kullanacağını seçin."),
            ("Miscellaneous", "Diğer"), ("Use custom font", "Özel yazı tipi kullan"), ("Font size can still be adjusted in the Global tab.", "Yazı tipi boyutu Global sekmesinde ayarlanabilir."),
            ("Choose font...", "Yazı tipi seç..."), ("Remove", "Kaldır"), ("Default", "Varsayılan"), ("From 2006", "2006'dan"), ("From 2013", "2013'ten"),
            ("Windows 11", "Windows 11"), ("Windows 10", "Windows 10"), ("Windows 8.1", "Windows 8.1"),
            ("Custom Shiftlock", "Özel Shiftlock"),
            ("Change the icon when Shift Lock is enabled. (Froststrap/Voidstrap Collection)", "Shift Lock açıkken simgeyi değiştir. (Froststrap/Voidstrap koleksiyonu)"),
            ("Advanced Modifications", "Gelişmiş değişiklikler"),
            ("Skybox Preset", "Skybox ön ayarı"),
            ("Change the in game sky texture. Some presets require a restart.", "Oyun içi gökyüzü dokusunu değiştirir. Bazı ön ayarlar yeniden başlatma gerektirir."),
            ("Fullbright (No Shadows)", "Tam parlaklık (gölge yok)"),
            ("Makes everything bright and removes shadows. (Experimental)", "Her şeyi aydınlatır ve gölgeleri kaldırır. (Deneysel)"),
            ("Visual Overlays & Stats", "Görsel katmanlar ve istatistikler"),
            ("Lighting Overlays", "Işık katmanları"),
            ("Motion Blur Effect", "Hareket bulanıklığı efekti"),
            ("FPS Counter", "FPS sayacı"),
            ("Server Location & Stats", "Sunucu konumu ve istatistikler"),
            ("Screen Brightness", "Ekran parlaklığı"),
            ("Adjust screen brightness via overlay (50% is default).", "Parlaklığı katman üzerinden ayarla (varsayılan %50)."));

        private static readonly Dictionary<string, string> It = D(("Mods", "Mods"), ("Manage and apply file mods to the Roblox client.", "Gestisci e applica mod di file al client Roblox."),
            ("Open Mods Folder", "Apri cartella Mods"), ("Manage custom Roblox mods here.", "Gestisci qui i mod Roblox personalizzati."),
            ("Help", "Aiuto"), ("See info about managing and creating mods.", "Vedi info sulla gestione e creazione dei mod."),
            ("Manage compatibility settings", "Gestisci impostazioni di compatibilità"), ("Configure application parameters such as DPI scaling behaviour and", "Configura parametri dell'app come comportamento DPI scaling e"),
            ("fullscreen optimizations.", "fullscreen optimizations."), ("Presets", "Preset"), ("Mouse cursor", "Cursore mouse"),
            ("Choose between using classic Roblox cursor styles.", "Scegli tra gli stili cursore classici di Roblox."),
            ("Use old avatar editor background", "Usa lo sfondo vecchio editor avatar"), ("Bring back the old avatar editor background used in legacy app versions.", "Ripristina lo sfondo vecchio editor avatar delle versioni precedenti."),
            ("Emulate old character sounds", "Emula i vecchi suoni del personaggio"), ("Attempt to restore old footsteps and character movement sounds before 2014.", "Prova a ripristinare i suoni di passi e movimento precedenti al 2014."),
            ("Preferred emoji type", "Tipo emoji preferito"), ("Choose what type of emoji should Roblox use.", "Scegli il tipo di emoji che Roblox deve usare."),
            ("Miscellaneous", "Varie"), ("Use custom font", "Usa font personalizzato"), ("Font size can still be adjusted in the Global tab.", "La dimensione del font può ancora essere regolata nella scheda Global."),
            ("Choose font...", "Scegli font..."), ("Remove", "Rimuovi"), ("Default", "Predefinito"), ("From 2006", "Dal 2006"), ("From 2013", "Dal 2013"),
            ("Windows 11", "Windows 11"), ("Windows 10", "Windows 10"), ("Windows 8.1", "Windows 8.1"),
            ("Custom Shiftlock", "Shiftlock personalizzato"),
            ("Change the icon when Shift Lock is enabled. (Froststrap/Voidstrap Collection)", "Cambia l’icona quando Shift Lock è attivo. (Collezione Froststrap/Voidstrap)"),
            ("Advanced Modifications", "Modifiche avanzate"),
            ("Skybox Preset", "Preset skybox"),
            ("Change the in game sky texture. Some presets require a restart.", "Cambia la texture del cielo in gioco. Alcuni preset richiedono un riavvio."),
            ("Fullbright (No Shadows)", "Fullbright (senza ombre)"),
            ("Makes everything bright and removes shadows. (Experimental)", "Rende tutto più luminoso e rimuove le ombre. (Sperimentale)"),
            ("Visual Overlays & Stats", "Sovrapposizioni visive e statistiche"),
            ("Lighting Overlays", "Sovrapposizioni di illuminazione"),
            ("Motion Blur Effect", "Effetto motion blur"),
            ("FPS Counter", "Contatore FPS"),
            ("Server Location & Stats", "Posizione server e statistiche"),
            ("Screen Brightness", "Luminosità schermo"),
            ("Adjust screen brightness via overlay (50% is default).", "Regola la luminosità tramite overlay (50% predefinito)."));

        private static readonly Dictionary<string, string> ArAe = D(("Mods", "Mods"), ("Manage and apply file mods to the Roblox client.", "إدارة وتطبيق تعديلات الملفات على عميل Roblox."),
            ("Open Mods Folder", "فتح مجلد Mods"), ("Manage custom Roblox mods here.", "إدارة تعديلات Roblox المخصصة هنا."),
            ("Help", "مساعدة"), ("See info about managing and creating mods.", "عرض معلومات حول إدارة وإنشاء التعديلات."),
            ("Manage compatibility settings", "إدارة إعدادات التوافق"), ("Configure application parameters such as DPI scaling behaviour and", "تكوين معلمات التطبيق مثل سلوك DPI scaling و "),
            ("fullscreen optimizations.", "fullscreen optimizations."), ("Presets", "إعدادات مسبقة"), ("Mouse cursor", "مؤشر الفأرة"),
            ("Choose between using classic Roblox cursor styles.", "اختر بين أنماط مؤشر Roblox الكلاسيكية."),
            ("Use old avatar editor background", "استخدام خلفية محرر الأفاتار القديمة"), ("Bring back the old avatar editor background used in legacy app versions.", "استعادة خلفية محرر الأفاتار القديمة المستخدمة في الإصدارات السابقة."),
            ("Emulate old character sounds", "محاكاة أصوات الشخصيات القديمة"), ("Attempt to restore old footsteps and character movement sounds before 2014.", "محاولة استعادة أصوات الخطوات والحركة قبل 2014."),
            ("Preferred emoji type", "نوع الإيموجي المفضل"), ("Choose what type of emoji should Roblox use.", "اختر نوع الإيموجي الذي يجب أن يستخدمه Roblox."),
            ("Miscellaneous", "متفرقات"), ("Use custom font", "استخدام خط مخصص"), ("Font size can still be adjusted in the Global tab.", "لا يزال يمكن تعديل حجم الخط في تبويب Global."),
            ("Choose font...", "اختيار خط..."), ("Remove", "إزالة"), ("Default", "افتراضي"), ("From 2006", "من 2006"), ("From 2013", "من 2013"),
            ("Windows 11", "Windows 11"), ("Windows 10", "Windows 10"), ("Windows 8.1", "Windows 8.1"),
            ("Custom Shiftlock", "Shiftlock مخصص"),
            ("Change the icon when Shift Lock is enabled. (Froststrap/Voidstrap Collection)", "غيّر الأيقونة عند تفعيل Shift Lock. (مجموعة Froststrap/Voidstrap)"),
            ("Advanced Modifications", "تعديلات متقدمة"),
            ("Skybox Preset", "إعداد مسبق لصندوق السماء"),
            ("Change the in game sky texture. Some presets require a restart.", "يغيّر نسيج السماء داخل اللعبة. بعض الإعدادات تتطلب إعادة تشغيل."),
            ("Fullbright (No Shadows)", "إضاءة كاملة (بدون ظلال)"),
            ("Makes everything bright and removes shadows. (Experimental)", "يزيد السطوع ويزيل الظلال. (تجريبي)"),
            ("Visual Overlays & Stats", "طبقات مرئية وإحصاءات"),
            ("Lighting Overlays", "طبقات إضاءة"),
            ("Motion Blur Effect", "تأثير ضبابية الحركة"),
            ("FPS Counter", "عداد FPS"),
            ("Server Location & Stats", "موقع الخادم والإحصاءات"),
            ("Screen Brightness", "سطوع الشاشة"),
            ("Adjust screen brightness via overlay (50% is default).", "اضبط السطوع عبر طبقة علوية (50% افتراضيًا)."));

        private static readonly Dictionary<string, string> De = D(
            ("Mods", "Mods"),
            ("Manage game modifications.", "Game-Mods verwalten."),
            ("Enable", "Aktivieren"),
            ("Disable", "Deaktivieren"),
            ("Advanced Modifications", "Erweiterte Anpassungen"),
            ("Skybox Preset", "Skybox-Voreinstellung"),
            ("Change the in game sky texture. Some presets require a restart.", "Ändert die Himmelstextur im Spiel. Einige Voreinstellungen erfordern einen Neustart."),
            ("Fullbright (No Shadows)", "Volles Licht (ohne Schatten)"),
            ("Makes everything bright and removes shadows. (Experimental)", "Macht alles heller und entfernt Schatten. (Experimentell)"),
            ("Visual Overlays & Stats", "Visuelle Overlays und Statistiken"),
            ("Lighting Overlays", "Beleuchtungs-Overlays"),
            ("Motion Blur Effect", "Bewegungsunschärfe"),
            ("FPS Counter", "FPS-Zähler"),
            ("Server Location & Stats", "Serverstandort und Statistiken"),
            ("Screen Brightness", "Bildschirmhelligkeit"),
            ("Adjust screen brightness via overlay (50% is default).", "Helligkeit per Overlay anpassen (Standard 50%)."));

        private static readonly Dictionary<string, string> Ro = D(
            ("Mods", "Mods"),
            ("Manage game modifications.", "Gestionare moduri joc."),
            ("Enable", "Activare"),
            ("Disable", "Dezactivare"),
            ("Advanced Modifications", "Modificări avansate"),
            ("Skybox Preset", "Preset skybox"),
            ("Change the in game sky texture. Some presets require a restart.", "Schimbă textura cerului în joc. Unele preseturi necesită repornire."),
            ("Fullbright (No Shadows)", "Fullbright (fără umbre)"),
            ("Makes everything bright and removes shadows. (Experimental)", "Face totul mai luminos și elimină umbrele. (Experimental)"),
            ("Visual Overlays & Stats", "Suprapuneri vizuale și statistici"),
            ("Lighting Overlays", "Suprapuneri de iluminare"),
            ("Motion Blur Effect", "Efect motion blur"),
            ("FPS Counter", "Contor FPS"),
            ("Server Location & Stats", "Locație server și statistici"),
            ("Screen Brightness", "Luminozitate ecran"),
            ("Adjust screen brightness via overlay (50% is default).", "Ajustează luminozitatea prin overlay (implicit 50%)."));

        private static readonly Dictionary<string, string> Sv = D(
            ("Mods", "Mods"),
            ("Manage game modifications.", "Hantera spelmodifieringar."),
            ("Enable", "Aktivera"),
            ("Disable", "Inaktivera"),
            ("Advanced Modifications", "Avancerade ändringar"),
            ("Skybox Preset", "Skybox-förinställning"),
            ("Change the in game sky texture. Some presets require a restart.", "Ändrar himmelstexturen i spelet. Vissa förinställningar kräver omstart."),
            ("Fullbright (No Shadows)", "Fullbright (inga skuggor)"),
            ("Makes everything bright and removes shadows. (Experimental)", "Gör allt ljusare och tar bort skuggor. (Experimentellt)"),
            ("Visual Overlays & Stats", "Visuella överlägg och statistik"),
            ("Lighting Overlays", "Ljusöverlägg"),
            ("Motion Blur Effect", "Rörelseoskärpa"),
            ("FPS Counter", "FPS-räknare"),
            ("Server Location & Stats", "Serverplats och statistik"),
            ("Screen Brightness", "Skärm ljusstyrka"),
            ("Adjust screen brightness via overlay (50% is default).", "Justera ljusstyrka via överlägg (50% standard)."));

        private static readonly Dictionary<string, string> Nl = D(
            ("Mods", "Mods"),
            ("Manage game modifications.", "Spelmodificaties beheren."),
            ("Enable", "Inschakelen"),
            ("Disable", "Uitschakelen"),
            ("Advanced Modifications", "Geavanceerde aanpassingen"),
            ("Skybox Preset", "Skybox-voorinstelling"),
            ("Change the in game sky texture. Some presets require a restart.", "Wijzigt de hemeltextuur in het spel. Sommige voorinstellingen vereisen een herstart."),
            ("Fullbright (No Shadows)", "Fullbright (geen schaduwen)"),
            ("Makes everything bright and removes shadows. (Experimental)", "Maakt alles helderder en verwijdert schaduwen. (Experimenteel)"),
            ("Visual Overlays & Stats", "Visuele overlays en statistieken"),
            ("Lighting Overlays", "Verlichtingsoverlays"),
            ("Motion Blur Effect", "Bewegingsonscherpte"),
            ("FPS Counter", "FPS-teller"),
            ("Server Location & Stats", "Serverlocatie en statistieken"),
            ("Screen Brightness", "Schermhelderheid"),
            ("Adjust screen brightness via overlay (50% is default).", "Pas helderheid aan via overlay (standaard 50%)."));

        private static readonly Dictionary<string, string> Pl = D(
            ("Mods", "Mods"),
            ("Manage game modifications.", "Zarzadzaj modyfikacjami gier."),
            ("Enable", "Wlacz"),
            ("Disable", "Wylacz"),
            ("Advanced Modifications", "Zaawansowane modyfikacje"),
            ("Skybox Preset", "Preset skyboxa"),
            ("Change the in game sky texture. Some presets require a restart.", "Zmienia teksturę nieba w grze. Niektóre presety wymagają restartu."),
            ("Fullbright (No Shadows)", "Pełna jasność (bez cieni)"),
            ("Makes everything bright and removes shadows. (Experimental)", "Rozjaśnia scenę i usuwa cienie. (Eksperymentalne)"),
            ("Visual Overlays & Stats", "Nakładki wizualne i statystyki"),
            ("Lighting Overlays", "Nakładki oświetlenia"),
            ("Motion Blur Effect", "Efekt rozmycia ruchu"),
            ("FPS Counter", "Licznik FPS"),
            ("Server Location & Stats", "Lokalizacja serwera i statystyki"),
            ("Screen Brightness", "Jasność ekranu"),
            ("Adjust screen brightness via overlay (50% is default).", "Dostosuj jasność przez nakładkę (domyślnie 50%)."));
    }
}
