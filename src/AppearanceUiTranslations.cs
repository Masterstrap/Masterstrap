using System;
using System.Collections.Generic;

namespace Masterstrap.Services
{
    internal static class AppearanceUiTranslations
    {
        private static Dictionary<string, string> D(params (string en, string tr)[] rows)
        {
            var d = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var (en, tr) in rows) d[en] = tr;
            return d;
        }

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
                        var maps = new[] { Vi, Fil, Id, Pt, Ms, Ja, Zh, Th, Km, Lo, Ko, Ru, Uk, Es, Fr, He, Tw, Tr, It, ArAe, De, Ro, Sv, Nl, Pl };
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

        public static bool TryTranslate(string currentLanguage, string key, out string value)
        {
            value = key;
            if (string.IsNullOrWhiteSpace(key)) return false;
            string normalizedKey = key.Trim();

            string englishKey = ResolveEnglishKey(normalizedKey);

            if (!Vi.ContainsKey(englishKey))
            {
                return false;
            }

            if (currentLanguage.Equals(LocalizationService.English, StringComparison.OrdinalIgnoreCase) ||
                currentLanguage.Equals(LocalizationService.EnglishCanada, StringComparison.OrdinalIgnoreCase) ||
                currentLanguage.Equals(LocalizationService.SouthAfrica, StringComparison.OrdinalIgnoreCase))
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
            if (lang.Equals(LocalizationService.SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(LocalizationService.SpanishArgentina, StringComparison.OrdinalIgnoreCase) || lang.Equals(LocalizationService.Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(LocalizationService.Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(LocalizationService.Chile, StringComparison.OrdinalIgnoreCase)) return Es;
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
            ("Appearance", "Diện mạo"),
            ("Choose language, visual theme, and startup behavior for the app.", "Chọn ngôn ngữ, chủ đề giao diện và hành vi khởi động cho ứng dụng."),
            ("Language Settings", "Cài đặt ngôn ngữ"),
            ("Select your preferred display language for the application interface.", "Chọn ngôn ngữ hiển thị ưu tiên cho giao diện ứng dụng."),
            ("Global Theme", "Chủ đề tổng thể"),
            ("Choose light or dark interface colors.", "Chọn màu sắc giao diện sáng hoặc tối."),
            ("Effect Theme", "Hiệu ứng giao diện"),
            ("Choose app visual mode: Default, Glassmorphic, or Glassmorphic + Blur.", "Chọn chế độ hiển thị: Mặc định, Kính mờ hoặc Kính mờ + Nhòe."),
            ("Light", "Sáng"),
            ("Dark", "Tối"),
            ("Default", "Mặc định"),
            ("glassmorphic", "Kính mờ"),
            ("glassmorphic + blur", "Kính mờ + Nhòe"),
            ("Background Image", "Ảnh nền"),
            ("Upload image for glass modes only (Default mode keeps normal background).", "Tải lên ảnh nền chỉ dành cho chế độ kính mờ (Chế độ mặc định giữ nền thường)."),
            ("Upload Background", "Tải lên ảnh nền"),
            ("Reset Background", "Đặt lại ảnh nền"),
            ("Shortcuts", "Phím tắt & Lối tắt"),
            ("These are the shortcuts that bring up the multi-choice launch menu.", "Đây là các lối tắt dùng để mở menu khởi chạy nhiều lựa chọn."),
            ("Desktop icon", "Biểu tượng màn hình chính"),
            ("Start Menu icon", "Biểu tượng Menu Start"),
            ("Launch Roblox", "Khởi chạy Roblox"),
            ("Create a Windows shortcut that runs Save and Launch directly.", "Tạo một lối tắt Windows để chạy trực tiếp Lưu và Khởi chạy."),
            ("Roblox Launch Interception", "Chặn khởi chạy Roblox"),
            ("When disabled, Roblox will launch directly without Masterstrap application.", "Khi tắt, Roblox sẽ khởi chạy trực tiếp mà không cần Masterstrap can thiệp."),
            ("Account", "Khóa bản quyền"),
            ("Enter a account and click Confirm to validate.", "Nhập khóa bản quyền và nhấp vào Kiểm tra khóa để xác thực."),
            ("Get Key", "Lấy khóa"),
            ("Confirm", "Kiểm tra khóa"),
            ("Status: Not checked", "Trạng thái: Chưa kiểm tra"),
            ("Status: Checked", "Trạng thái: Đã kiểm tra"),
            ("Status: Invalid", "Trạng thái: Không hợp lệ"),
            ("Status: Valid", "Trạng thái: Hợp lệ")
        );

        private static readonly Dictionary<string, string> Fil = D(
            ("Appearance", "Hitsura"),
            ("Choose language, visual theme, and startup behavior for the app.", "Pumili ng wika, visual theme, at gawi sa pagsisimula ng app."),
            ("Language Settings", "Mga Setting ng Wika"),
            ("Select your preferred display language for the application interface.", "Pumili ng gusto mong wika ng pagpapakita para sa interface ng application."),
            ("Global Theme", "Pandaigdigang Tema"),
            ("Choose light or dark interface colors.", "Pumili ng maliwanag o madilim na kulay ng interface."),
            ("Effect Theme", "Tema ng Epekto"),
            ("Choose app visual mode: Default, Glassmorphic, or Glassmorphic + Blur.", "Pumili ng visual mode ng app: Default, Glassmorphic, o Glassmorphic + Blur."),
            ("Light", "Maliwanag"),
            ("Dark", "Madilim"),
            ("Default", "Default"),
            ("glassmorphic", "Glassmorphic"),
            ("glassmorphic + blur", "Glassmorphic + Blur"),
            ("Background Image", "Larawan sa Background"),
            ("Upload image for glass modes only (Default mode keeps normal background).", "Mag-upload ng larawan para sa mga glass mode lang (Default mode ay nagpapanatili ng normal na background)."),
            ("Upload Background", "Mag-upload ng Background"),
            ("Reset Background", "I-reset ang Background"),
            ("Shortcuts", "Mga Shortcut"),
            ("These are the shortcuts that bring up the multi-choice launch menu.", "Ito ang mga shortcut na nagpapakita ng menu ng pag-launch na may maraming pagpipilian."),
            ("Desktop icon", "Icon sa Desktop"),
            ("Start Menu icon", "Icon sa Start Menu"),
            ("Launch Roblox", "Ilunsad ang Roblox"),
            ("Create a Windows shortcut that runs Save and Launch directly.", "Gumawa ng Windows shortcut na direktang nagpapatakbo ng I-save at Ilunsad."),
            ("Roblox Launch Interception", "Interception sa Pag-launch ng Roblox"),
            ("When disabled, Roblox will launch directly without Masterstrap application.", "Kapag naka-disable, direktang ilulunsad ang Roblox nang walang Masterstrap application."),
            ("Account", "Lisensya Key"),
            ("Enter a account and click Confirm to validate.", "Maglagay ng account at i-click ang Suriin ang Key para mag-validate."),
            ("Get Key", "Kumuha ng Key"),
            ("Confirm", "Suriin ang Key"),
            ("Status: Not checked", "Katayuan: Hindi nasuri"),
            ("Status: Checked", "Katayuan: Nasuri na"),
            ("Status: Invalid", "Katayuan: Invalid"),
            ("Status: Valid", "Katayuan: Valid")
        );

        private static readonly Dictionary<string, string> Id = D(
            ("Appearance", "Tampilan"),
            ("Choose language, visual theme, and startup behavior for the app.", "Pilih bahasa, tema visual, dan perilaku mulai untuk aplikasi."),
            ("Language Settings", "Pengaturan Bahasa"),
            ("Select your preferred display language for the application interface.", "Pilih bahasa tampilan pilihan Anda untuk antarmuka aplikasi."),
            ("Global Theme", "Tema Global"),
            ("Choose light or dark interface colors.", "Pilih warna antarmuka terang atau gelap."),
            ("Effect Theme", "Efek Tema"),
            ("Choose app visual mode: Default, Glassmorphic, or Glassmorphic + Blur.", "Pilih mode visual aplikasi: Default, Glassmorphic, atau Glassmorphic + Blur."),
            ("Light", "Terang"),
            ("Dark", "Gelap"),
            ("Default", "Default"),
            ("glassmorphic", "Glassmorphic"),
            ("glassmorphic + blur", "Glassmorphic + Blur"),
            ("Background Image", "Gambar Latar Belakang"),
            ("Upload image for glass modes only (Default mode keeps normal background).", "Unggah gambar untuk mode kaca saja (Mode default mempertahankan latar belakang normal)."),
            ("Upload Background", "Unggah Latar Belakang"),
            ("Reset Background", "Atur Ulang Latar Belakang"),
            ("Shortcuts", "Pintasan"),
            ("These are the shortcuts that bring up the multi-choice launch menu.", "Ini adalah pintasan yang memunculkan menu peluncuran pilihan ganda."),
            ("Desktop icon", "Ikon Desktop"),
            ("Start Menu icon", "Ikon Menu Mulai"),
            ("Launch Roblox", "Luncurkan Roblox"),
            ("Create a Windows shortcut that runs Save and Launch directly.", "Buat pintasan Windows yang menjalankan Simpan dan Luncurkan secara langsung."),
            ("Roblox Launch Interception", "Intersepsi Peluncuran Roblox"),
            ("When disabled, Roblox will launch directly without Masterstrap application.", "Jika dinonaktifkan, Roblox akan diluncurkan langsung tanpa injeksi Masterstrap."),
            ("Account", "Kunci Lisensi"),
            ("Enter a account and click Confirm to validate.", "Masukkan kunci lisensi dan klik Periksa Kunci untuk memvalidasi."),
            ("Get Key", "Dapatkan Kunci"),
            ("Confirm", "Periksa Kunci"),
            ("Status: Not checked", "Status: Belum diperiksa"),
            ("Status: Checked", "Status: Diperiksa"),
            ("Status: Invalid", "Status: Tidak Valid"),
            ("Status: Valid", "Status: Valid")
        );

        private static readonly Dictionary<string, string> Pt = D(
            ("Appearance", "Aparência"),
            ("Choose language, visual theme, and startup behavior for the app.", "Escolha o idioma, o tema visual e o comportamento de inicialização do aplicativo."),
            ("Language Settings", "Configurações de Idioma"),
            ("Select your preferred display language for the application interface.", "Selecione o seu idioma de exibição preferido para a interface do aplicativo."),
            ("Global Theme", "Tema Global"),
            ("Choose light or dark interface colors.", "Escolha as cores claras ou escuras da interface."),
            ("Effect Theme", "Tema de Efeitos"),
            ("Choose app visual mode: Default, Glassmorphic, or Glassmorphic + Blur.", "Escolha o modo visual do app: Padrão, Glassmorphic ou Glassmorphic + Blur."),
            ("Light", "Claro"),
            ("Dark", "Escuro"),
            ("Default", "Padrão"),
            ("glassmorphic", "Glassmorphic"),
            ("glassmorphic + blur", "Glassmorphic + Blur"),
            ("Background Image", "Imagem de Fundo"),
            ("Upload image for glass modes only (Default mode keeps normal background).", "Envie uma imagem apenas para os modos de vidro (O modo padrão mantém o fundo normal)."),
            ("Upload Background", "Enviar Imagem de Fundo"),
            ("Reset Background", "Redefinir Imagem de Fundo"),
            ("Shortcuts", "Atalhos"),
            ("These are the shortcuts that bring up the multi-choice launch menu.", "Estes são os atalhos que abrem o menu de inicialização de múltipla escolha."),
            ("Desktop icon", "Ícone da Área de Trabalho"),
            ("Start Menu icon", "Ícone do Menu Iniciar"),
            ("Launch Roblox", "Iniciar Roblox"),
            ("Create a Windows shortcut that runs Save and Launch directly.", "Crie um atalho do Windows que execute Salvar e Iniciar diretamente."),
            ("Roblox Launch Interception", "Interceptação de Inicialização do Roblox"),
            ("When disabled, Roblox will launch directly without Masterstrap application.", "Quando desativado, o Roblox iniciará diretamente sem a injeção do Masterstrap."),
            ("Account", "Chave de Licença"),
            ("Enter a account and click Confirm to validate.", "Insira uma chave de licença e clique em Verificar Chave para validar."),
            ("Get Key", "Obter Chave"),
            ("Confirm", "Verificar Chave"),
            ("Status: Not checked", "Status: Não verificado"),
            ("Status: Checked", "Status: Verificado"),
            ("Status: Invalid", "Status: Inválido"),
            ("Status: Valid", "Status: Válido")
        );

        private static readonly Dictionary<string, string> Ms = D(
            ("Appearance", "Penampilan"),
            ("Choose language, visual theme, and startup behavior for the app.", "Pilih bahasa, tema visual, dan tingkah laku permulaan untuk aplikasi."),
            ("Language Settings", "Tetapan Bahasa"),
            ("Select your preferred display language for the application interface.", "Pilih bahasa paparan pilihan anda untuk antara muka aplikasi."),
            ("Global Theme", "Tema Global"),
            ("Choose light or dark interface colors.", "Pilih warna antara muka terang atau gelap."),
            ("Effect Theme", "Tema Kesan"),
            ("Choose app visual mode: Default, Glassmorphic, or Glassmorphic + Blur.", "Pilih mod visual aplikasi: Lalai, Glassmorphic, atau Glassmorphic + Blur."),
            ("Light", "Terang"),
            ("Dark", "Gelap"),
            ("Default", "Lalai"),
            ("glassmorphic", "Glassmorphic"),
            ("glassmorphic + blur", "Glassmorphic + Blur"),
            ("Background Image", "Imej Latar Belakang"),
            ("Upload image for glass modes only (Default mode keeps normal background).", "Muat naik imej untuk mod kaca sahaja (Mod lalai mengekalkan latar belakang biasa)."),
            ("Upload Background", "Muat Naik Latar Belakang"),
            ("Reset Background", "Set Semula Latar Belakang"),
            ("Shortcuts", "Pintasan"),
            ("These are the shortcuts that bring up the multi-choice launch menu.", "Ini adalah pintasan yang memaparkan menu pelancaran pelbagai pilihan."),
            ("Desktop icon", "Ikon Desktop"),
            ("Start Menu icon", "Ikon Menu Mula"),
            ("Launch Roblox", "Lancarkan Roblox"),
            ("Create a Windows shortcut that runs Save and Launch directly.", "Cipta pintasan Windows yang menjalankan Simpan dan Lancar secara langsung."),
            ("Roblox Launch Interception", "Pintasan Pelancaran Roblox"),
            ("When disabled, Roblox will launch directly without Masterstrap application.", "Apabila dinyahaktifkan, Roblox akan dilancarkan secara langsung tanpa suntikan Masterstrap."),
            ("Account", "Kunci Lesen"),
            ("Enter a account and click Confirm to validate.", "Masukkan kunci lesen dan klik Semak Kunci untuk mengesahkan."),
            ("Get Key", "Dapatkan Kunci"),
            ("Confirm", "Semak Kunci"),
            ("Status: Not checked", "Status: Belum disemak"),
            ("Status: Checked", "Status: Telah disemak"),
            ("Status: Invalid", "Status: Tidak Sah"),
            ("Status: Valid", "Status: Sah")
        );

        private static readonly Dictionary<string, string> Ja = D(
            ("Appearance", "外観設定"),
            ("Choose language, visual theme, and startup behavior for the app.", "言語、ビジュアルテーマ、アプリの起動時の動作を選択します。"),
            ("Language Settings", "言語設定"),
            ("Select your preferred display language for the application interface.", "アプリケーションインターフェースの表示言語を選択します。"),
            ("Global Theme", "グローバルテーマ"),
            ("Choose light or dark interface colors.", "ライトまたはダークのインターフェースカラーを選択します。"),
            ("Effect Theme", "エフェクトテーマ"),
            ("Choose app visual mode: Default, Glassmorphic, or Glassmorphic + Blur.", "アプリのビジュアルモード（デフォルト、グラスモルフィック、グラスモルフィック＋ブラー）を選択します。"),
            ("Light", "ライト"),
            ("Dark", "ダーク"),
            ("Default", "デフォルト"),
            ("glassmorphic", "グラスモルフィック"),
            ("glassmorphic + blur", "グラスモルフィック + ブラー"),
            ("Background Image", "背景画像"),
            ("Upload image for glass modes only (Default mode keeps normal background).", "背景画像はグラスモードにのみ適用されます（デフォルトモードは通常の背景を維持します）。"),
            ("Upload Background", "背景をアップロード"),
            ("Reset Background", "背景をリセット"),
            ("Shortcuts", "ショートカット"),
            ("These are the shortcuts that bring up the multi-choice launch menu.", "これらはマルチ選択起動メニューを表示するためのショートカットです。"),
            ("Desktop icon", "デスクトップアイコン"),
            ("Start Menu icon", "スタートメニューアイコン"),
            ("Launch Roblox", "Robloxを起動"),
            ("Create a Windows shortcut that runs Save and Launch directly.", "「保存して起動」を直接実行するWindowsショートカットを作成します。"),
            ("Roblox Launch Interception", "Roblox起動のインターセプト"),
            ("When disabled, Roblox will launch directly without Masterstrap application.", "無効にすると、RobloxはMasterstrapインジェクションなしで直接起動します。"),
            ("Account", "ライセンスキー"),
            ("Enter a account and click Confirm to validate.", "ライセンスキーを入力し、「キーを確認」をクリックして検証します。"),
            ("Get Key", "キーを取得"),
            ("Confirm", "キーを確認"),
            ("Status: Not checked", "ステータス: 未確認"),
            ("Status: Checked", "ステータス: 確認済み"),
            ("Status: Invalid", "ステータス: 無効"),
            ("Status: Valid", "ステータス: 有効")
        );

        private static readonly Dictionary<string, string> Zh = D(
            ("Appearance", "外观设置"),
            ("Choose language, visual theme, and startup behavior for the app.", "选择应用语言、视觉主题和启动行为。"),
            ("Language Settings", "语言设置"),
            ("Select your preferred display language for the application interface.", "选择您偏好的应用界面显示语言。"),
            ("Global Theme", "全局主题"),
            ("Choose light or dark interface colors.", "选择浅色或深色界面颜色。"),
            ("Effect Theme", "特效主题"),
            ("Choose app visual mode: Default, Glassmorphic, or Glassmorphic + Blur.", "选择应用视觉模式：默认、毛玻璃或毛玻璃+模糊。"),
            ("Light", "浅色主题"),
            ("Dark", "深色主题"),
            ("Default", "默认效果"),
            ("glassmorphic", "毛玻璃特效"),
            ("glassmorphic + blur", "毛玻璃 + 模糊"),
            ("Background Image", "背景图片"),
            ("Upload image for glass modes only (Default mode keeps normal background).", "上传仅适用于毛玻璃模式的背景图片（默认模式保持普通背景）。"),
            ("Upload Background", "上传背景"),
            ("Reset Background", "重置背景"),
            ("Shortcuts", "快捷方式"),
            ("These are the shortcuts that bring up the multi-choice launch menu.", "这些是唤起多选启动菜单的快捷方式。"),
            ("Desktop icon", "桌面图标"),
            ("Start Menu icon", "开始菜单图标"),
            ("Launch Roblox", "启动 Roblox"),
            ("Create a Windows shortcut that runs Save and Launch directly.", "创建直接运行“保存并启动”的 Windows 快捷方式。"),
            ("Roblox Launch Interception", "Roblox 启动拦截"),
            ("When disabled, Roblox will launch directly without Masterstrap application.", "禁用时，Roblox 将直接启动，无需 Masterstrap 注入。"),
            ("Account", "授权密钥"),
            ("Enter a account and click Confirm to validate.", "输入授权密钥并点击“验证密钥”进行验证。"),
            ("Get Key", "获取密钥"),
            ("Confirm", "验证密钥"),
            ("Status: Not checked", "状态：未验证"),
            ("Status: Checked", "状态：已验证"),
            ("Status: Invalid", "状态：无效"),
            ("Status: Valid", "状态：有效")
        );

        private static readonly Dictionary<string, string> Th = D(
            ("Appearance", "รูปลักษณ์"),
            ("Choose language, visual theme, and startup behavior for the app.", "เลือกภาษา ธีมภาพ และลักษณะการทำงานเมื่อเริ่มต้นระบบสำหรับแอป"),
            ("Language Settings", "การตั้งค่าภาษา"),
            ("Select your preferred display language for the application interface.", "เลือกภาษาในการแสดงผลที่คุณต้องการสำหรับอินเทอร์เฟซของแอปพลิเคชัน"),
            ("Global Theme", "ธีมหลัก"),
            ("Choose light or dark interface colors.", "เลือกสีอินเทอร์เฟซแบบสว่างหรือมืด"),
            ("Effect Theme", "ธีมเอฟเฟกต์"),
            ("Choose app visual mode: Default, Glassmorphic, or Glassmorphic + Blur.", "เลือกโหมดภาพของแอป: เริ่มต้น, Glassmorphic หรือ Glassmorphic + Blur"),
            ("Light", "สว่าง"),
            ("Dark", "มืด"),
            ("Default", "เริ่มต้น"),
            ("glassmorphic", "Glassmorphic"),
            ("glassmorphic + blur", "Glassmorphic + Blur"),
            ("Background Image", "ภาพพื้นหลัง"),
            ("Upload image for glass modes only (Default mode keeps normal background).", "อัปโหลดภาพสำหรับโหมดกระจกเท่านั้น (โหมดเริ่มต้นจะใช้พื้นหลังปกติ)"),
            ("Upload Background", "อัปโหลดพื้นหลัง"),
            ("Reset Background", "รีเซ็ตพื้นหลัง"),
            ("Shortcuts", "ทางลัด"),
            ("These are the shortcuts that bring up the multi-choice launch menu.", "นี่คือทางลัดที่ใช้เปิดเมนูเปิดตัวแบบหลายตัวเลือก"),
            ("Desktop icon", "ไอคอนเดสก์ท็อป"),
            ("Start Menu icon", "ไอคอนเมนูเริ่ม"),
            ("Launch Roblox", "เปิด Roblox"),
            ("Create a Windows shortcut that runs Save and Launch directly.", "สร้างทางลัด Windows ที่เรียกใช้บันทึกและเปิดตัวโดยตรง"),
            ("Roblox Launch Interception", "การสกัดกั้นการเปิด Roblox"),
            ("When disabled, Roblox will launch directly without Masterstrap application.", "เมื่อปิดการใช้งาน Roblox จะเปิดโดยตรงโดยไม่มีการฉีด Masterstrap"),
            ("Account", "คีย์ใบอนุญาต"),
            ("Enter a account and click Confirm to validate.", "ป้อนคีย์ใบอนุญาตและคลิก ตรวจสอบคีย์ เพื่อตรวจสอบความถูกต้อง"),
            ("Get Key", "รับคีย์"),
            ("Confirm", "ตรวจสอบคีย์"),
            ("Status: Not checked", "สถานะ: ยังไม่ได้ตรวจสอบ"),
            ("Status: Checked", "สถานะ: ตรวจสอบแล้ว"),
            ("Status: Invalid", "สถานะ: ไม่ถูกต้อง"),
            ("Status: Valid", "สถานะ: ถูกต้อง")
        );

        private static readonly Dictionary<string, string> Km = D(
            ("Appearance", "រូបរាង"),
            ("Choose language, visual theme, and startup behavior for the app.", "ជ្រើសរើសភាសា ស្បែករូបភាព និងឥរិយាបថចាប់ផ្តើមសម្រាប់កម្មវិធី។"),
            ("Language Settings", "ការកំណត់ភាសា"),
            ("Select your preferred display language for the application interface.", "ជ្រើសរើសភាសាបង្ហាញដែលអ្នកចង់បានសម្រាប់កម្មវិធី។"),
            ("Global Theme", "ស្បែកសកល"),
            ("Choose light or dark interface colors.", "ជ្រើសរើសពណ៌ចំណុចប្រទាក់ភ្លឺ ឬងងឹត។"),
            ("Effect Theme", "ស្បែកបែបផែន"),
            ("Choose app visual mode: Default, Glassmorphic, or Glassmorphic + Blur.", "ជ្រើសរើសរបៀបរូបភាពកម្មវិធី៖ លំនាំដើម, Glassmorphic ឬ Glassmorphic + ព្រាល។"),
            ("Light", "ភ្លឺ"),
            ("Dark", "ងងឹត"),
            ("Default", "លំនាំដើម"),
            ("glassmorphic", "Glassmorphic"),
            ("glassmorphic + blur", "Glassmorphic + ព្រាល"),
            ("Background Image", "រូបភាពផ្ទៃខាងក្រោយ"),
            ("Upload image for glass modes only (Default mode keeps normal background).", "ផ្ទុកឡើងរូបភាពសម្រាប់តែរបៀបកញ្ចក់ប៉ុណ្ណោះ (របៀបលំនាំដើមរក្សាផ្ទៃខាងក្រោយធម្មតា)។"),
            ("Upload Background", "ផ្ទុកឡើងផ្ទៃខាងក្រោយ"),
            ("Reset Background", "កំណត់ផ្ទៃខាងក្រោយឡើងវិញ"),
            ("Shortcuts", "ផ្លូវកាត់"),
            ("These are the shortcuts that bring up the multi-choice launch menu.", "ទាំងនេះគឺជាផ្លូវកាត់ដែលបើកម៉ឺនុយបើកដំណើរការច្រើនជម្រើស។"),
            ("Desktop icon", "រូបតំណាងផ្ទៃតុ"),
            ("Start Menu icon", "រូបតំណាងម៉ឺនុយចាប់ផ្តើម"),
            ("Launch Roblox", "បើក Roblox"),
            ("Create a Windows shortcut that runs Save and Launch directly.", "បង្កើតផ្លូវកាត់ Windows ដែលដំណើរការ រក្សាទុក និងបើកដំណើរការដោយផ្ទាល់។"),
            ("Roblox Launch Interception", "ការស្ទាក់ចាប់ការបើកដំណើរការ Roblox"),
            ("When disabled, Roblox will launch directly without Masterstrap application.", "នៅពេលបិទ Roblox នឹងបើកដំណើរការដោយផ្ទាល់ដោយមិនចាំបាច់មានការចាក់បញ្ចូល Masterstrap ឡើយ។"),
            ("Account", "សោអាជ្ញាប័ណ្ណ"),
            ("Enter a account and click Confirm to validate.", "បញ្ចូលសោអាជ្ញាប័ណ្ណ ហើយចុច ពិនិត្យសោ ដើម្បីផ្ទៀងផ្ទាត់។"),
            ("Get Key", "យកសោ"),
            ("Confirm", "ពិនិត្យសោ"),
            ("Status: Not checked", "ស្ថានភាព៖ មិនទាន់ពិនិត្យ"),
            ("Status: Checked", "ស្ថានភាព៖ បានពិនិត្យ"),
            ("Status: Invalid", "ស្ថានភាព៖ មិនត្រឹមត្រូវ"),
            ("Status: Valid", "ស្ថានភាព៖ ត្រឹមត្រូវ")
        );

        private static readonly Dictionary<string, string> Lo = D(
            ("Appearance", "ຮູບລັກສະນະ"),
            ("Choose language, visual theme, and startup behavior for the app.", "ເລືອກພາສາ, ທີມພາບ ແລະພຶດຕິກຳການເລີ່ມຕົ້ນສຳລັບແອັບ."),
            ("Language Settings", "ການຕັ້ງຄ່າພາສາ"),
            ("Select your preferred display language for the application interface.", "ເລືອກພາສາສະແດງຜົນທີ່ທ່ານຕ້ອງການສຳລັບສ່ວນຕິດຕໍ່ຂອງແອັບພລິເຄຊັນ."),
            ("Global Theme", "ທີມທົ່ວໂລກ"),
            ("Choose light or dark interface colors.", "ເລືອກສີສ່ວນຕິດຕໍ່ແບບສະຫວ່າງ ຫຼືມືດ."),
            ("Effect Theme", "ທີມເອັບເຟັກ"),
            ("Choose app visual mode: Default, Glassmorphic, or Glassmorphic + Blur.", "ເລືອກໂໝດພາບຂອງແອັບ: ເລີ່ມຕົ້ນ, Glassmorphic ຫຼື Glassmorphic + ມົວ."),
            ("Light", "ສະຫວ່າງ"),
            ("Dark", "ມືດ"),
            ("Default", "ເລີ່ມຕົ້ນ"),
            ("glassmorphic", "Glassmorphic"),
            ("glassmorphic + blur", "Glassmorphic + ມົວ"),
            ("Background Image", "ຮູບພື້ນຫຼັງ"),
            ("Upload image for glass modes only (Default mode keeps normal background).", "ອັບໂຫຼດຮູບສຳລັບໂໝດກະຈົກເທົ່ານັ້ນ (ໂໝດເລີ່ມຕົ້ນຈະໃຊ້ພື້ນຫຼັງປົກກະຕິ)."),
            ("Upload Background", "ອັບໂຫຼດພື້ນຫຼັງ"),
            ("Reset Background", "ຕັ້ງຄ່າພື້ນຫຼັງໃໝ່"),
            ("Shortcuts", "ທາງລັດ"),
            ("These are the shortcuts that bring up the multi-choice launch menu.", "ນີ້ແມ່ນທາງລັດທີ່ໃຊ້ເປີດເມນູເປີດຕົວແບບຫຼາຍຕົວເລືອກ."),
            ("Desktop icon", "ໄອຄອນເດັສທັອບ"),
            ("Start Menu icon", "ໄອຄອນເມນູເລີ່ມຕົ້ນ"),
            ("Launch Roblox", "ເປີດ Roblox"),
            ("Create a Windows shortcut that runs Save and Launch directly.", "ສ້າງທາງລັດ Windows ທີ່ເອີ້ນໃຊ້ ບັນທຶກ ແລະເປີດຕົວໂດຍກົງ."),
            ("Roblox Launch Interception", "ການສະກັດກັ້ນການເປີດ Roblox"),
            ("When disabled, Roblox will launch directly without Masterstrap application.", "ເມື່ອປິດການໃຊ້ງານ, Roblox ຈະເປີດໂດຍກົງໂດຍບໍ່ມີການສີດ Masterstrap."),
            ("Account", "ຄີໃບອະນຸຍາດ"),
            ("Enter a account and click Confirm to validate.", "ປ້ອນຄີໃບອະນຸຍາດ ແລະຄລິກ ກວດສອບຄີ ເພື່ອກວດສອບຄວາມຖືກຕ້ອງ."),
            ("Get Key", "ຮັບຄີ"),
            ("Confirm", "ກວດສອບຄີ"),
            ("Status: Not checked", "ສະຖານະ: ຍັງບໍ່ໄດ້ກວດສອບ"),
            ("Status: Checked", "ສະຖານະ: ກວດສອບແລ້ວ"),
            ("Status: Invalid", "ສະຖານະ: ບໍ່ຖືກຕ້ອງ"),
            ("Status: Valid", "ສະຖານະ: ຖືກຕ້ອງ")
        );

        private static readonly Dictionary<string, string> Ko = D(
            ("Appearance", "디자인 설정"),
            ("Choose language, visual theme, and startup behavior for the app.", "앱의 언어, 테마 및 시작 모드를 선택합니다."),
            ("Language Settings", "언어 설정"),
            ("Select your preferred display language for the application interface.", "애플리케이션 인터페이스의 표시 언어를 선택합니다."),
            ("Global Theme", "기본 테마"),
            ("Choose light or dark interface colors.", "밝은 테마 또는 어두운 테마를 선택합니다."),
            ("Effect Theme", "시각 효과"),
            ("Choose app visual mode: Default, Glassmorphic, or Glassmorphic + Blur.", "시각 모드를 선택합니다: 기본, 글래스모피즘 또는 글래스모피즘 + 블러."),
            ("Light", "라이트"),
            ("Dark", "다크"),
            ("Default", "기본 효과"),
            ("glassmorphic", "글래스모피즘"),
            ("glassmorphic + blur", "글래스모피즘 + 블러"),
            ("Background Image", "배경 이미지"),
            ("Upload image for glass modes only (Default mode keeps normal background).", "글래스 모드 전용 배경 이미지를 업로드합니다 (기본 모드는 일반 배경 유지)."),
            ("Upload Background", "배경 업로드"),
            ("Reset Background", "배경 초기화"),
            ("Shortcuts", "바로 가기"),
            ("These are the shortcuts that bring up the multi-choice launch menu.", "멀티 선택 실행 메뉴를 표시하는 바로 가기 키입니다."),
            ("Desktop icon", "바탕 화면 아이콘"),
            ("Start Menu icon", "시작 메뉴 아이콘"),
            ("Launch Roblox", "Roblox 실행"),
            ("Create a Windows shortcut that runs Save and Launch directly.", "'저장 후 실행'으로 직접 작동하는 Windows 단축키를 만듭니다."),
            ("Roblox Launch Interception", "Roblox 실행 가로채기"),
            ("When disabled, Roblox will launch directly without Masterstrap application.", "비활성화 시 Roblox는 Masterstrap 인젝션 없이 직접 실행됩니다."),
            ("Account", "라이선스 키"),
            ("Enter a account and click Confirm to validate.", "라이선스 키를 입력하고 '키 확인'을 클릭하여 인증하세요."),
            ("Get Key", "키 받기"),
            ("Confirm", "키 확인"),
            ("Status: Not checked", "상태: 미인증"),
            ("Status: Checked", "상태: 인증됨"),
            ("Status: Invalid", "상태: 유효하지 않음"),
            ("Status: Valid", "상태: 유효함")
        );

        private static readonly Dictionary<string, string> Ru = D(
            ("Appearance", "Внешний вид"),
            ("Choose language, visual theme, and startup behavior for the app.", "Выберите язык, тему оформления и параметры запуска приложения."),
            ("Language Settings", "Языковые настройки"),
            ("Select your preferred display language for the application interface.", "Выберите предпочтительный язык интерфейса приложения."),
            ("Global Theme", "Основная тема"),
            ("Choose light or dark interface colors.", "Выберите светлые или темные цвета интерфейса."),
            ("Effect Theme", "Тема эффектов"),
            ("Choose app visual mode: Default, Glassmorphic, or Glassmorphic + Blur.", "Выберите визуальный режим: Стандартный, Эффект стекла или Стекло + Размытие."),
            ("Light", "Светлая"),
            ("Dark", "Темная"),
            ("Default", "Стандартная"),
            ("glassmorphic", "Эффект стекла"),
            ("glassmorphic + blur", "Стекло + Размытие"),
            ("Background Image", "Фоновое изображение"),
            ("Upload image for glass modes only (Default mode keeps normal background).", "Загрузите фоновый рисунок для стеклянных режимов (обычный фон останется по умолчанию)."),
            ("Upload Background", "Загрузить фон"),
            ("Reset Background", "Сбросить фон"),
            ("Shortcuts", "Ярлыки запуска"),
            ("These are the shortcuts that bring up the multi-choice launch menu.", "Ярлыки, которые вызывают меню выбора режимов запуска Roblox."),
            ("Desktop icon", "Значок на рабочем столе"),
            ("Start Menu icon", "Значок в меню Пуск"),
            ("Launch Roblox", "Запустить Roblox"),
            ("Create a Windows shortcut that runs Save and Launch directly.", "Создать ярлык Windows для прямого запуска приложения без меню выбора."),
            ("Roblox Launch Interception", "Перехват запуска Roblox"),
            ("When disabled, Roblox will launch directly without Masterstrap application.", "При отключении Roblox будет запускаться напрямую без инъекции настроек Masterstrap."),
            ("Account", "Лицензионный ключ"),
            ("Enter a account and click Confirm to validate.", "Введите лицензионный ключ и нажмите «Проверить ключ» для валидации."),
            ("Get Key", "Получить ключ"),
            ("Confirm", "Проверить ключ"),
            ("Status: Not checked", "Статус: Не проверен"),
            ("Status: Checked", "Статус: Проверен"),
            ("Status: Invalid", "Статус: Недействителен"),
            ("Status: Valid", "Статус: Действителен")
        );

        private static readonly Dictionary<string, string> Uk = D(
            ("Appearance", "Зовнішній вигляд"),
            ("Choose language, visual theme, and startup behavior for the app.", "Виберіть мову, тему оформлення та параметри запуску додатка."),
            ("Language Settings", "Мовні налаштування"),
            ("Select your preferred display language for the application interface.", "Виберіть бажану мову інтерфейсу додатка."),
            ("Global Theme", "Основна тема"),
            ("Choose light or dark interface colors.", "Виберіть світлі або темні кольори інтерфейсу."),
            ("Effect Theme", "Тема ефектів"),
            ("Choose app visual mode: Default, Glassmorphic, or Glassmorphic + Blur.", "Виберіть візуальний режим: Стандартний, Ефект скла або Скло + Розмиття."),
            ("Light", "Світла"),
            ("Dark", "Темна"),
            ("Default", "Стандартна"),
            ("glassmorphic", "Ефект скла"),
            ("glassmorphic + blur", "Скло + Розмиття"),
            ("Background Image", "Фонове зображення"),
            ("Upload image for glass modes only (Default mode keeps normal background).", "Завантажте фоновий малюнок для скляних режимів (звичайний фон залишиться за замовчуванням)."),
            ("Upload Background", "Завантажити фон"),
            ("Reset Background", "Скинути фон"),
            ("Shortcuts", "Ярлики запуску"),
            ("These are the shortcuts that bring up the multi-choice launch menu.", "Ярлики, які викликають меню вибору режимів запуску Roblox."),
            ("Desktop icon", "Значок на робочому столі"),
            ("Start Menu icon", "Значок у меню Пуск"),
            ("Launch Roblox", "Запустити Roblox"),
            ("Create a Windows shortcut that runs Save and Launch directly.", "Створити ярлик Windows для прямого запуску програми без додаткового меню."),
            ("Roblox Launch Interception", "Перехоплення запуску Roblox"),
            ("When disabled, Roblox will launch directly without Masterstrap application.", "При відключенні Roblox буде запускатися напряму без ін'єкції налаштувань Masterstrap."),
            ("Account", "Ліцензійний ключ"),
            ("Enter a account and click Confirm to validate.", "Введіть ліцензійний ключ і натисніть «Перевірити ключ» для валідації."),
            ("Get Key", "Отримати ключ"),
            ("Confirm", "Перевірити ключ"),
            ("Status: Not checked", "Статус: Не перевірено"),
            ("Status: Checked", "Статус: Перевірено"),
            ("Status: Invalid", "Статус: Недійсний"),
            ("Status: Valid", "Статус: Дійсний")
        );

        private static readonly Dictionary<string, string> Es = D(
            ("Appearance", "Apariencia"),
            ("Choose language, visual theme, and startup behavior for the app.", "Elige el idioma, el tema visual y el comportamiento de inicio de la aplicación."),
            ("Language Settings", "Ajustes de Idioma"),
            ("Select your preferred display language for the application interface.", "Selecciona tu idioma preferido para la interfaz de la aplicación."),
            ("Global Theme", "Tema Global"),
            ("Choose light or dark interface colors.", "Elige colores de interfaz claros u oscuros."),
            ("Effect Theme", "Efectos Visuales"),
            ("Choose app visual mode: Default, Glassmorphic, or Glassmorphic + Blur.", "Elige el modo visual: Por defecto, Glassmorphic o Glassmorphic + Desfoque."),
            ("Light", "Claro"),
            ("Dark", "Oscuro"),
            ("Default", "Por defecto"),
            ("glassmorphic", "Glassmorphic"),
            ("glassmorphic + blur", "Glassmorphic + Desfoque"),
            ("Background Image", "Imagen de Fondo"),
            ("Upload image for glass modes only (Default mode keeps normal background).", "Sube una imagen solo para modos de vidrio (El modo por defecto mantiene el fondo normal)."),
            ("Upload Background", "Subir Fondo"),
            ("Reset Background", "Restablecer Fondo"),
            ("Shortcuts", "Accesos Directos"),
            ("These are the shortcuts that bring up the multi-choice launch menu.", "Estos son los accesos directos que abren el menú de inicio de múltiples opciones."),
            ("Desktop icon", "Icono de Escritorio"),
            ("Start Menu icon", "Icono del Menú Inicio"),
            ("Launch Roblox", "Iniciar Roblox"),
            ("Create a Windows shortcut that runs Save and Launch directly.", "Crea un acceso directo de Windows para ejecutar Guardar e Iniciar directamente."),
            ("Roblox Launch Interception", "Intercepción de Inicio de Roblox"),
            ("When disabled, Roblox will launch directly without Masterstrap application.", "Cuando está deshabilitado, Roblox se iniciará directamente sin la inyección de Masterstrap."),
            ("Account", "Clave de Licencia"),
            ("Enter a account and click Confirm to validate.", "Introduce una clave de licencia y pulsa Verificar Clave para validar."),
            ("Get Key", "Obtener Clave"),
            ("Confirm", "Verificar Clave"),
            ("Status: Not checked", "Estado: No verificado"),
            ("Status: Checked", "Estado: Verificado"),
            ("Status: Invalid", "Estado: No válido"),
            ("Status: Valid", "Estado: Válido")
        );

        private static readonly Dictionary<string, string> Fr = D(
            ("Appearance", "Apparence"),
            ("Choose language, visual theme, and startup behavior for the app.", "Choisissez la langue, le thème visuel et le comportement de démarrage de l'application."),
            ("Language Settings", "Paramètres de Langue"),
            ("Select your preferred display language for the application interface.", "Sélectionnez votre langue d'affichage préférée pour l'interface de l'application."),
            ("Global Theme", "Thème Global"),
            ("Choose light or dark interface colors.", "Choisissez des couleurs d'interface claires ou sombres."),
            ("Effect Theme", "Effets de Thème"),
            ("Choose app visual mode: Default, Glassmorphic, or Glassmorphic + Blur.", "Choisissez le mode visuel : Par défaut, Glassmorphic ou Glassmorphic + Flou."),
            ("Light", "Clair"),
            ("Dark", "Sombre"),
            ("Default", "Par défaut"),
            ("glassmorphic", "Glassmorphic"),
            ("glassmorphic + blur", "Glassmorphic + Flou"),
            ("Background Image", "Image d'arrière-plan"),
            ("Upload image for glass modes only (Default mode keeps normal background).", "Téléchargez une image pour les modes verre uniquement (le mode par défaut conserve l'arrière-plan normal)."),
            ("Upload Background", "Télécharger l'arrière-plan"),
            ("Reset Background", "Réinitialiser l'arrière-plan"),
            ("Shortcuts", "Raccourcis"),
            ("These are the shortcuts that bring up the multi-choice launch menu.", "Ce sont les raccourcis qui ouvrent le menu de démarrage à choix multiples."),
            ("Desktop icon", "Icône du bureau"),
            ("Start Menu icon", "Icône du menu Démarrer"),
            ("Launch Roblox", "Lancer Roblox"),
            ("Create a Windows shortcut that runs Save and Launch directly.", "Créez un raccourci Windows qui exécute directement Enregistrer et Lancer."),
            ("Roblox Launch Interception", "Interception de lancement Roblox"),
            ("When disabled, Roblox will launch directly without Masterstrap application.", "Une fois désactivé, Roblox se lancera directement sans application Masterstrap."),
            ("Account", "Clé de Licence"),
            ("Enter a account and click Confirm to validate.", "Saisissez une clé de licence et cliquez sur Vérifier la Clé pour valider."),
            ("Get Key", "Obtenir une Clé"),
            ("Confirm", "Vérifier la Clé"),
            ("Status: Not checked", "Statut: Non vérifié"),
            ("Status: Checked", "Statut: Vérifié"),
            ("Status: Invalid", "Statut: Invalide"),
            ("Status: Valid", "Statut: Valide")
        );

        private static readonly Dictionary<string, string> He = D(
            ("Appearance", "מראה עיצובי"),
            ("Choose language, visual theme, and startup behavior for the app.", "בחר שפה, ערכת נושא והתנהגות הפעלה עבור האפליקציה."),
            ("Language Settings", "הגדרות שפה"),
            ("Select your preferred display language for the application interface.", "בחר את שפת התצוגה המועדפת עליך עבור ממשק האפליקציה."),
            ("Global Theme", "ערכת נושא כללית"),
            ("Choose light or dark interface colors.", "בחר צבעי ממשק בהירים או כהים."),
            ("Effect Theme", "אפקט עיצובי"),
            ("Choose app visual mode: Default, Glassmorphic, or Glassmorphic + Blur.", "בחר מצב תצוגה: ברירת מחדל, Glassmorphic או Glassmorphic + טשטוש."),
            ("Light", "בהיר"),
            ("Dark", "כהה"),
            ("Default", "ברירת מחדל"),
            ("glassmorphic", "Glassmorphic"),
            ("glassmorphic + blur", "Glassmorphic + טשטוש"),
            ("Background Image", "תמונת רקע"),
            ("Upload image for glass modes only (Default mode keeps normal background).", "העלה תמונת רקע עבור מצבי זכוכית בלבד (מצב ברירת מחדל שומר על רקע רגיל)."),
            ("Upload Background", "העלה תמונת רקע"),
            ("Reset Background", "אפס תמונת רקע"),
            ("Shortcuts", "קיצורי דרך"),
            ("These are the shortcuts that bring up the multi-choice launch menu.", "אלה קיצורי הדרך שמפעילים את תפריט הבחירה המרובה של ההרצה."),
            ("Desktop icon", "סמל שולחן עבודה"),
            ("Start Menu icon", "סמל תפריט התחלה"),
            ("Launch Roblox", "הפעל את Roblox"),
            ("Create a Windows shortcut that runs Save and Launch directly.", "צור קיצור דרך של Windows שמריץ את שמירה והפעלה ישירות."),
            ("Roblox Launch Interception", "יירוט הפעלת Roblox"),
            ("When disabled, Roblox will launch directly without Masterstrap application.", "כאשר מושבת, Roblox יופעל ישירות ללא הזרקה של Masterstrap."),
            ("Account", "מפתח רישיון"),
            ("Enter a account and click Confirm to validate.", "הזן מפתח רישיון ולחץ על בדוק מפתח כדי לאמת."),
            ("Get Key", "קבל מפתח"),
            ("Confirm", "בדוק מפתח"),
            ("Status: Not checked", "סטטוס: לא נבדק"),
            ("Status: Checked", "סטטוס: נבדק"),
            ("Status: Invalid", "סטטוס: לא תקין"),
            ("Status: Valid", "סטטוס: תקין")
        );

        private static readonly Dictionary<string, string> Tw = D(
            ("Appearance", "外觀設定"),
            ("Choose language, visual theme, and startup behavior for the app.", "選擇應用程式語言、視覺主題和啟動行為。"),
            ("Language Settings", "語言設定"),
            ("Select your preferred display language for the application interface.", "選擇您偏好的應用程式介面顯示語言。"),
            ("Global Theme", "全域主題"),
            ("Choose light or dark interface colors.", "選擇淺色或深色介面顏色。"),
            ("Effect Theme", "特效主題"),
            ("Choose app visual mode: Default, Glassmorphic, or Glassmorphic + Blur.", "選擇視覺效果模式：預設、磨砂玻璃或磨砂玻璃+模糊。"),
            ("Light", "淺色"),
            ("Dark", "深色"),
            ("Default", "預設"),
            ("glassmorphic", "磨砂玻璃"),
            ("glassmorphic + blur", "磨砂玻璃 + 模糊"),
            ("Background Image", "背景圖片"),
            ("Upload image for glass modes only (Default mode keeps normal background).", "上傳僅適用於磨砂玻璃模式的背景圖片（預設模式保持普通背景）。"),
            ("Upload Background", "上傳背景圖片"),
            ("Reset Background", "重置背景圖片"),
            ("Shortcuts", "捷徑"),
            ("These are the shortcuts that bring up the multi-choice launch menu.", "這些是喚起多選啟動選單的捷徑。"),
            ("Desktop icon", "桌面圖示"),
            ("Start Menu icon", "開始功能表圖示"),
            ("Launch Roblox", "啟動 Roblox"),
            ("Create a Windows shortcut that runs Save and Launch directly.", "建立直接執行「儲存並啟動」的 Windows 捷徑。"),
            ("Roblox Launch Interception", "Roblox 啟動攔截"),
            ("When disabled, Roblox will launch directly without Masterstrap application.", "禁用時，Roblox 將直接啟動，無需 Masterstrap 注入。"),
            ("Account", "授權金鑰"),
            ("Enter a account and click Confirm to validate.", "輸入授權金鑰並點擊「驗證金鑰」進行驗證。"),
            ("Get Key", "獲取金鑰"),
            ("Confirm", "驗證金鑰"),
            ("Status: Not checked", "狀態：未驗證"),
            ("Status: Checked", "狀態：已驗證"),
            ("Status: Invalid", "狀態：無效"),
            ("Status: Valid", "狀態：有效")
        );

        private static readonly Dictionary<string, string> Tr = D(
            ("Appearance", "Görünüm"),
            ("Choose language, visual theme, and startup behavior for the app.", "Uygulama için dili, görsel temayı ve başlangıç davranışını seçin."),
            ("Language Settings", "Dil Ayarları"),
            ("Select your preferred display language for the application interface.", "Uygulama arayüzü için tercih ettiğiniz ekran dilini seçin."),
            ("Global Theme", "Genel Tema"),
            ("Choose light or dark interface colors.", "Açık veya koyu arayüz renklerini seçin."),
            ("Effect Theme", "Efekt Teması"),
            ("Choose app visual mode: Default, Glassmorphic, or Glassmorphic + Blur.", "Uygulama görsel modunu seçin: Varsayılan, Glassmorphic veya Glassmorphic + Bulanık."),
            ("Light", "Açık"),
            ("Dark", "Koyu"),
            ("Default", "Varsayılan"),
            ("glassmorphic", "Glassmorphic"),
            ("glassmorphic + blur", "Glassmorphic + Bulanık"),
            ("Background Image", "Arka Plan Resmi"),
            ("Upload image for glass modes only (Default mode keeps normal background).", "Yalnızca cam modları için resim yükleyin (Varsayılan mod normal arka planı korur)."),
            ("Upload Background", "Arka Plan Yükle"),
            ("Reset Background", "Arka Planı Sıfırla"),
            ("Shortcuts", "Kısayollar"),
            ("These are the shortcuts that bring up the multi-choice launch menu.", "Bunlar çoklu seçim başlatma menüsünü açan kısayollardır."),
            ("Desktop icon", "Masaüstü simgesi"),
            ("Start Menu icon", "Başlat Menüsü simgesi"),
            ("Launch Roblox", "Roblox'u Başlat"),
            ("Create a Windows shortcut that runs Save and Launch directly.", "Doğrudan Kaydet ve Başlat'ı çalıştıran bir Windows kısayolu oluşturun."),
            ("Roblox Launch Interception", "Roblox Başlatma Engellemesi"),
            ("When disabled, Roblox will launch directly without Masterstrap application.", "Devre dışı bırakıldığında, Roblox Masterstrap enjeksiyonu olmadan doğrudan başlatılacaktır."),
            ("Account", "Lisans Anahtarı"),
            ("Enter a account and click Confirm to validate.", "Bir lisans anahtarı girin ve doğrulamak için Anahtarı Kontrol Et'e tıklayın."),
            ("Get Key", "Anahtar Al"),
            ("Confirm", "Anahtarı Kontrol Et"),
            ("Status: Not checked", "Durum: Kontrol edilmedi"),
            ("Status: Checked", "Durum: Kontrol edildi"),
            ("Status: Invalid", "Durum: Geçersiz"),
            ("Status: Valid", "Durum: Geçerli")
        );

        private static readonly Dictionary<string, string> It = D(
            ("Appearance", "Aspetto"),
            ("Choose language, visual theme, and startup behavior for the app.", "Scegli la lingua, il tema visivo e il comportamento all'avvio dell'applicazione."),
            ("Language Settings", "Impostazioni Lingua"),
            ("Select your preferred display language for the application interface.", "Seleziona la lingua di visualizzazione preferita per l'interfaccia dell'applicazione."),
            ("Global Theme", "Tema Globale"),
            ("Choose light or dark interface colors.", "Scegli colori dell'interfaccia chiari o scuri."),
            ("Effect Theme", "Tema Effetti"),
            ("Choose app visual mode: Default, Glassmorphic, or Glassmorphic + Blur.", "Scegli la modalità visiva: Predefinita, Glassmorphic o Glassmorphic + Sfocatura."),
            ("Light", "Chiaro"),
            ("Dark", "Scuro"),
            ("Default", "Predefinito"),
            ("glassmorphic", "Glassmorphic"),
            ("glassmorphic + blur", "Glassmorphic + Sfocatura"),
            ("Background Image", "Immagine di Sfondo"),
            ("Upload image for glass modes only (Default mode keeps normal background).", "Carica un'immagine solo per le modalità vetro (la modalità predefinita mantiene lo sfondo normale)."),
            ("Upload Background", "Carica Sfondo"),
            ("Reset Background", "Ripristina Sfondo"),
            ("Shortcuts", "Scorciatoie"),
            ("These are the shortcuts that bring up the multi-choice launch menu.", "Queste sono le scorciatoie che mostrano il menu di avvio a scelta multipla."),
            ("Desktop icon", "Icona desktop"),
            ("Start Menu icon", "Icona del menu Start"),
            ("Launch Roblox", "Avvia Roblox"),
            ("Create a Windows shortcut that runs Save and Launch directly.", "Crea una scorciatoia di Windows che esegue direttamente Salva e Avvia."),
            ("Roblox Launch Interception", "Intercettazione Avvio Roblox"),
            ("When disabled, Roblox will launch directly without Masterstrap application.", "Se disabilitato, Roblox si avvierà direttamente senza l'iniezione di Masterstrap."),
            ("Account", "Chiave di Licenza"),
            ("Enter a account and click Confirm to validate.", "Inserisci una chiave di licenza e fai clic su Verifica Chiave per validarla."),
            ("Get Key", "Ottieni Chiave"),
            ("Confirm", "Verifica Chiave"),
            ("Status: Not checked", "Stato: Non verificato"),
            ("Status: Checked", "Stato: Verificato"),
            ("Status: Invalid", "Stato: Non valido"),
            ("Status: Valid", "Stato: Valido")
        );

        private static readonly Dictionary<string, string> ArAe = D(
            ("Appearance", "المظهر"),
            ("Choose language, visual theme, and startup behavior for the app.", "اختر اللغة، والمظهر المرئي، وسلوك بدء تشغيل التطبيق."),
            ("Language Settings", "إعدادات اللغة"),
            ("Select your preferred display language for the application interface.", "اختر لغة العرض المفضلة لديك لواجهة التطبيق."),
            ("Global Theme", "المظهر العام"),
            ("Choose light or dark interface colors.", "اختر ألوان الواجهة الفاتحة أو الداكنة."),
            ("Effect Theme", "تأثير المظهر"),
            ("Choose app visual mode: Default, Glassmorphic, or Glassmorphic + Blur.", "اختر الوضع المرئي للتطبيق: الافتراضي، أو الزجاجي، أو الزجاجي + الضبابي."),
            ("Light", "فاتح"),
            ("Dark", "داكن"),
            ("Default", "الافتراضي"),
            ("glassmorphic", "الزجاجي"),
            ("glassmorphic + blur", "الزجاجي + الضبابي"),
            ("Background Image", "صورة الخلفية"),
            ("Upload image for glass modes only (Default mode keeps normal background).", "قم بتحميل صورة للوضعين الزجاجيين فقط (الوضع الافتراضي يحتفظ بالخلفية العادية)."),
            ("Upload Background", "تحميل الخلفية"),
            ("Reset Background", "إعادة تعيين الخلفية"),
            ("Shortcuts", "الاختصارات"),
            ("These are the shortcuts that bring up the multi-choice launch menu.", "هذه هي الاختصارات التي تظهر قائمة التشغيل متعددة الخيارات."),
            ("Desktop icon", "أيقونة سطح المكتب"),
            ("Start Menu icon", "أيقونة قائمة ابدأ"),
            ("Launch Roblox", "تشغيل Roblox"),
            ("Create a Windows shortcut that runs Save and Launch directly.", "قم بإنشاء اختصار لـ Windows يقوم بتشغيل الحفظ والتشغيل مباشرة."),
            ("Roblox Launch Interception", "اعتراض تشغيل Roblox"),
            ("When disabled, Roblox will launch directly without Masterstrap application.", "عند التعطيل، سيتم تشغيل Roblox مباشرة بدون حقن Masterstrap."),
            ("Account", "مفتاح الترخيص"),
            ("Enter a account and click Confirm to validate.", "أدخل مفتاح الترخيص وانقر فوق التحقق من المفتاح للتحقق من الصلاحية."),
            ("Get Key", "الحصول على مفتاح"),
            ("Confirm", "التحقق من المفتاح"),
            ("Status: Not checked", "الحالة: لم يتم التحقق"),
            ("Status: Checked", "الحالة: تم التحقق"),
            ("Status: Invalid", "الحالة: غير صالح"),
            ("Status: Valid", "الحالة: صالح")
        );

        private static readonly Dictionary<string, string> De = D(
            ("Appearance", "Aussehen"),
            ("Choose language, visual theme, and startup behavior for the app.", "Wählen Sie Sprache, visuelles Thema und Startverhalten für die App."),
            ("Language Settings", "Spracheinstellungen"),
            ("Select your preferred display language for the application interface.", "Wählen Sie Ihre bevorzugte Anzeigesprache für die Benutzeroberfläche."),
            ("Global Theme", "Globales Thema"),
            ("Choose light or dark interface colors.", "Wählen Sie helle oder dunkle Farben für die Benutzeroberfläche."),
            ("Effect Theme", "Effekt-Thema"),
            ("Choose app visual mode: Default, Glassmorphic, or Glassmorphic + Blur.", "Wählen Sie den visuellen App-Modus: Standard, Glassmorphic oder Glassmorphic + Blur."),
            ("Light", "Hell"),
            ("Dark", "Dunkel"),
            ("Default", "Standard"),
            ("glassmorphic", "Glassmorphic"),
            ("glassmorphic + blur", "Glassmorphic + Blur"),
            ("Background Image", "Hintergrundbild"),
            ("Upload image for glass modes only (Default mode keeps normal background).", "Bild nur für Glas-Modi hochladen (Standardmodus behält normalen Hintergrund bei)."),
            ("Upload Background", "Hintergrund hochladen"),
            ("Reset Background", "Hintergrund zurücksetzen"),
            ("Shortcuts", "Verknüpfungen"),
            ("These are the shortcuts that bring up the multi-choice launch menu.", "Dies sind die Tastenkombinationen, die das Mehrfachauswahl-Startmenü aufrufen."),
            ("Desktop icon", "Desktop-Symbol"),
            ("Start Menu icon", "Startmenü-Symbol"),
            ("Launch Roblox", "Roblox starten"),
            ("Create a Windows shortcut that runs Save and Launch directly.", "Erstellen Sie eine Windows-Verknüpfung, die direkt Speichern und Starten ausführt."),
            ("Roblox Launch Interception", "Roblox-Startabfangung"),
            ("When disabled, Roblox will launch directly without Masterstrap application.", "Wenn deaktiviert, startet Roblox direkt ohne Masterstrap-Injektion."),
            ("Account", "Lizenzschlüssel"),
            ("Enter a account and click Confirm to validate.", "Geben Sie einen Lizenzschlüssel ein und klicken Sie auf Schlüssel prüfen, um ihn zu validieren."),
            ("Get Key", "Schlüssel holen"),
            ("Confirm", "Schlüssel prüfen"),
            ("Status: Not checked", "Status: Nicht geprüft"),
            ("Status: Checked", "Status: Geprüft"),
            ("Status: Invalid", "Status: Ungültig"),
            ("Status: Valid", "Status: Gültig")
        );

        private static readonly Dictionary<string, string> Ro = D(
            ("Appearance", "Aspect"),
            ("Choose language, visual theme, and startup behavior for the app.", "Alege limba, tema vizuală și comportamentul de pornire pentru aplicație."),
            ("Language Settings", "Setări Limbă"),
            ("Select your preferred display language for the application interface.", "Selectează limba de afișare preferată pentru interfața aplicației."),
            ("Global Theme", "Temă Globală"),
            ("Choose light or dark interface colors.", "Alege culorile deschise sau închise ale interfeței."),
            ("Effect Theme", "Temă Efecte"),
            ("Choose app visual mode: Default, Glassmorphic, or Glassmorphic + Blur.", "Alege modul vizual al aplicației: Implicit, Glassmorphic sau Glassmorphic + Blur."),
            ("Light", "Luminos"),
            ("Dark", "Întunecat"),
            ("Default", "Implicit"),
            ("glassmorphic", "Glassmorphic"),
            ("glassmorphic + blur", "Glassmorphic + Blur"),
            ("Background Image", "Imagine de Fundal"),
            ("Upload image for glass modes only (Default mode keeps normal background).", "Încarcă imagine doar pentru modurile sticlă (Modul implicit păstrează fundalul normal)."),
            ("Upload Background", "Încarcă Fundal"),
            ("Reset Background", "Resetează Fundal"),
            ("Shortcuts", "Scurtături"),
            ("These are the shortcuts that bring up the multi-choice launch menu.", "Acestea sunt scurtăturile care afișează meniul de lansare cu opțiuni multiple."),
            ("Desktop icon", "Pictogramă Desktop"),
            ("Start Menu icon", "Pictogramă Meniu Start"),
            ("Launch Roblox", "Lansează Roblox"),
            ("Create a Windows shortcut that runs Save and Launch directly.", "Creează o scurtătură Windows care rulează direct Salvează și Lansează."),
            ("Roblox Launch Interception", "Interceptare Lansare Roblox"),
            ("When disabled, Roblox will launch directly without Masterstrap application.", "Când este dezactivat, Roblox va porni direct fără injecția Masterstrap."),
            ("Account", "Cheie Licență"),
            ("Enter a account and click Confirm to validate.", "Introdu o cheie de licență și fă clic pe Verifică Cheia pentru validare."),
            ("Get Key", "Obține Cheie"),
            ("Confirm", "Verifică Cheia"),
            ("Status: Not checked", "Stare: Neverificată"),
            ("Status: Checked", "Stare: Verificată"),
            ("Status: Invalid", "Stare: Invalidă"),
            ("Status: Valid", "Stare: Validă")
        );

        private static readonly Dictionary<string, string> Sv = D(
            ("Appearance", "Utseende"),
            ("Choose language, visual theme, and startup behavior for the app.", "Välj språk, visuellt tema och startbeteende för appen."),
            ("Language Settings", "Språkinställningar"),
            ("Select your preferred display language for the application interface.", "Välj ditt föredragna visningsspråk för applikationsgränssnittet."),
            ("Global Theme", "Globalt Tema"),
            ("Choose light or dark interface colors.", "Välj ljusa eller mörka gränssnittsfärger."),
            ("Effect Theme", "Effekttema"),
            ("Choose app visual mode: Default, Glassmorphic, or Glassmorphic + Blur.", "Välj visuellt läge för appen: Standard, Glassmorphic eller Glassmorphic + Blur."),
            ("Light", "Ljust"),
            ("Dark", "Mörkt"),
            ("Default", "Standard"),
            ("glassmorphic", "Glassmorphic"),
            ("glassmorphic + blur", "Glassmorphic + Blur"),
            ("Background Image", "Bakgrundsbild"),
            ("Upload image for glass modes only (Default mode keeps normal background).", "Ladda upp bild endast för glas-lägen (Standardläget behåller normal bakgrund)."),
            ("Upload Background", "Ladda upp Bakgrund"),
            ("Reset Background", "Återställ Bakgrund"),
            ("Shortcuts", "Genvägar"),
            ("These are the shortcuts that bring up the multi-choice launch menu.", "Dessa är genvägarna som visar startmenyn med flera val."),
            ("Desktop icon", "Desktop-ikon"),
            ("Start Menu icon", "Startmeny-ikon"),
            ("Launch Roblox", "Starta Roblox"),
            ("Create a Windows shortcut that runs Save and Launch directly.", "Skapa en Windows-genväg som kör Spara och Starta direkt."),
            ("Roblox Launch Interception", "Roblox Start-avlyssning"),
            ("When disabled, Roblox will launch directly without Masterstrap application.", "När den är inaktiverad startar Roblox direkt utan Masterstrap-injektion."),
            ("Account", "Licensnyckel"),
            ("Enter a account and click Confirm to validate.", "Ange en licensnyckel och klicka på Kontrollera Nyckel för att validera."),
            ("Get Key", "Hämta Nyckel"),
            ("Confirm", "Kontrollera Nyckel"),
            ("Status: Not checked", "Status: Ej kontrollerad"),
            ("Status: Checked", "Status: Kontrollerad"),
            ("Status: Invalid", "Status: Ogiltig"),
            ("Status: Valid", "Status: Giltig")
        );

        private static readonly Dictionary<string, string> Nl = D(
            ("Appearance", "Uiterlijk"),
            ("Choose language, visual theme, and startup behavior for the app.", "Kies taal, visueel thema en opstartgedrag voor de app."),
            ("Language Settings", "Taalinstellingen"),
            ("Select your preferred display language for the application interface.", "Selecteer uw voorkeurstaal voor de applicatie-interface."),
            ("Global Theme", "Globaal Thema"),
            ("Choose light or dark interface colors.", "Kies lichte of donkere interfacekleuren."),
            ("Effect Theme", "Effectthema"),
            ("Choose app visual mode: Default, Glassmorphic, or Glassmorphic + Blur.", "Kies visuele modus van de app: Standaard, Glassmorphic of Glassmorphic + Blur."),
            ("Light", "Licht"),
            ("Dark", "Donker"),
            ("Default", "Standaard"),
            ("glassmorphic", "Glassmorphic"),
            ("glassmorphic + blur", "Glassmorphic + Blur"),
            ("Background Image", "Achtergrondafbeelding"),
            ("Upload image for glass modes only (Default mode keeps normal background).", "Upload alleen een afbeelding voor glasmodi (Standaardmodus behoudt normale achtergrond)."),
            ("Upload Background", "Upload Achtergrond"),
            ("Reset Background", "Reset Achtergrond"),
            ("Shortcuts", "Snelkoppelingen"),
            ("These are the shortcuts that bring up the multi-choice launch menu.", "Dit zijn de snelkoppelingen die het startmenu met meerdere keuzes openen."),
            ("Desktop icon", "Bureaubladpictogram"),
            ("Start Menu icon", "Startmenupictogram"),
            ("Launch Roblox", "Roblox starten"),
            ("Create a Windows shortcut that runs Save and Launch directly.", "Maak een Windows-snelkoppeling die direct Opslaan en Starten uitvoert."),
            ("Roblox Launch Interception", "Roblox Start-onderschepping"),
            ("When disabled, Roblox will launch directly without Masterstrap application.", "Indien uitgeschakeld, start Roblox rechtstreeks zonder Masterstrap-Toepassing."),
            ("Account", "Licentiesleutel"),
            ("Enter a account and click Confirm to validate.", "Voer een licentiesleutel in en klik op Controleer Sleutel om te valideren."),
            ("Get Key", "Sleutel Ophalen"),
            ("Confirm", "Controleer Sleutel"),
            ("Status: Not checked", "Status: Niet gecontroleerd"),
            ("Status: Checked", "Status: Gecontroleerd"),
            ("Status: Invalid", "Status: Ongeldig"),
            ("Status: Valid", "Status: Geldig")
        );

        private static readonly Dictionary<string, string> Pl = D(
            ("Appearance", "Wygląd"),
            ("Choose language, visual theme, and startup behavior for the app.", "Wybierz język, motyw wizualny i zachowanie aplikacji podczas uruchamiania."),
            ("Language Settings", "Ustawienia języka"),
            ("Select your preferred display language for the application interface.", "Wybierz preferowany język wyświetlania dla interfejsu aplikacji."),
            ("Global Theme", "Motyw globalny"),
            ("Choose light or dark interface colors.", "Wybierz jasny lub ciemny kolor interfejsu."),
            ("Effect Theme", "Motyw efektów"),
            ("Choose app visual mode: Default, Glassmorphic, or Glassmorphic + Blur.", "Wybierz tryb wizualny aplikacji: Domyślny, Glassmorphic lub Glassmorphic + Blur."),
            ("Light", "Jasny"),
            ("Dark", "Ciemny"),
            ("Default", "Domyślny"),
            ("glassmorphic", "Glassmorphic"),
            ("glassmorphic + blur", "Glassmorphic + Blur"),
            ("Background Image", "Obraz tła"),
            ("Upload image for glass modes only (Default mode keeps normal background).", "Prześlij obraz tylko dla trybów szklanych (tryb domyślny zachowuje normalne tło)."),
            ("Upload Background", "Prześlij tło"),
            ("Reset Background", "Resetuj tło"),
            ("Shortcuts", "Skróty"),
            ("These are the shortcuts that bring up the multi-choice launch menu.", "Są to skróty, które wywołują menu uruchamiania wielokrotnego wyboru."),
            ("Desktop icon", "Ikona na pulpicie"),
            ("Start Menu icon", "Ikona w Menu Start"),
            ("Launch Roblox", "Uruchom Roblox"),
            ("Create a Windows shortcut that runs Save and Launch directly.", "Utwórz skrót systemu Windows, który bezpośrednio uruchamia Zapisz i Uruchom."),
            ("Roblox Launch Interception", "Przechwytywanie uruchamiania Roblox"),
            ("When disabled, Roblox will launch directly without Masterstrap application.", "Po wyłączeniu Roblox uruchomi się bezpośrednio bez wstrzykiwania Masterstrap."),
            ("Account", "Klucz licencyjny"),
            ("Enter a account and click Confirm to validate.", "Wprowadź klucz licencyjny i kliknij Sprawdź klucz, aby zweryfikować."),
            ("Get Key", "Pobierz klucz"),
            ("Confirm", "Sprawdź klucz"),
            ("Status: Not checked", "Status: Nie sprawdzono"),
            ("Status: Checked", "Status: Sprawdzono"),
            ("Status: Invalid", "Status: Nieprawidłowy"),
            ("Status: Valid", "Status: Prawidłowy")
        );
    }
}
