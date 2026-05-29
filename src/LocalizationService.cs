using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace Masterstrap.Services
{
    public static class LocalizationService
    {
        public const string English = "English";
        public const string Vietnamese = "Vietnamese";
        public const string Filipino = "Filipino";
        public const string Indonesian = "Indonesian";
        public const string Portuguese = "Portuguese";
        public const string Malay = "Malay";
        public const string Japanese = "Japanese";
        public const string Chinese = "Chinese";
        public const string Thai = "Thai";
        public const string Khmer = "Khmer";
        public const string Lao = "Lao";
        public const string Korean = "Korean";
        public const string Russian = "Russian";
        public const string Ukrainian = "Ukrainian";
        public const string SpanishLatin = "SpanishLatin";
        public const string SpanishArgentina = "SpanishArgentina";
        public const string French = "French";
        public const string Hebrew = "Hebrew";
        public const string EnglishCanada = "EnglishCanada";
        public const string Taiwan = "Taiwan";
        public const string Colombia = "Colombia";
        public const string Turkiye = "Turkiye";
        public const string Spain = "Spain";
        public const string Italy = "Italy";
        public const string Chile = "Chile";
        public const string UnitedArabEmirates = "UnitedArabEmirates";
        public const string Brazil = "Brazil";
        public const string SouthAfrica = "SouthAfrica";
        public const string German = "German";
        public const string Romanian = "Romanian";
        public const string Swedish = "Swedish";
        public const string Dutch = "Dutch";
        public const string Polish = "Polish";

        private static string _currentLanguage = English;

        private static Dictionary<string, string> LoadMdTranslations(string resourceName)
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                var asm = Assembly.GetExecutingAssembly();
                using var stream = asm.GetManifestResourceStream(resourceName);
                if (stream == null) return dict;
                using var reader = new StreamReader(stream, Encoding.UTF8);
                string? prevLine = null;
                while (reader.ReadLine() is string line)
                {
                    string t = line.Trim();
                    if (string.IsNullOrEmpty(t) || t.StartsWith("=", StringComparison.Ordinal))
                    {
                        prevLine = null;
                        continue;
                    }
                    if (t.StartsWith("→", StringComparison.Ordinal))
                    {
                        string value = t.Length > 1 ? t.Substring(1).Trim() : string.Empty;
                        if (!string.IsNullOrEmpty(prevLine) && !string.IsNullOrEmpty(value))
                            dict[prevLine] = value;
                        prevLine = null;
                        continue;
                    }
                    prevLine = t;
                }
            }
            catch { /* ignore */ }
            return dict;
        }

        private static readonly Dictionary<string, string> EnToVi = OverlayAboutTab(
            OverlayAboutTab(
            MergeTranslations(
            LoadMdTranslations("Masterstrap.Resources.vietnamese.md"),
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
            ["Settings and Options"] = "Cài đặt và Tùy chọn",
            ["Select Game FFlags Preset"] = "Chọn FFlags theo trò chơi",
            ["🎮 Select Game FFlags Preset"] = "🎮 Chọn FFlags theo trò chơi",
            ["Update Soon..."] = "Cập Nhật Sớm...",
            ["Information System"] = "Thông Tin Hệ Thống",
            ["Language Settings"] = "Cài đặt ngôn ngữ",
            ["Select your preferred display language for the application interface."] = "Chọn ngôn ngữ hiển thị bạn muốn sử dụng cho giao diện ứng dụng.",
            ["Vietnamese"] = "Tiếng Việt",
            ["Desktop Shortcut"] = "Lối tắt trên màn hình Desktop",
            ["Create a shortcut on your Desktop for quick access to Masterstrap (recommended)"] = "Tạo lối tắt trên Desktop để truy cập nhanh vào Masterstrap (đề xuất)",
            ["Create a shortcut on your Desktop for quick access to Masterstrap"] = "Tạo lối tắt trên Desktop để truy cập nhanh vào Masterstrap (đề xuất)",
            ["General Settings"] = "Cài đặt chung",
            ["📋 General Settings"] = "Cài đặt chung",
            ["Auto-load FFlags on startup (recommended)"] = "Tự động tải Flags khi khởi động (đề xuất)",
            ["Auto-load FFlags on startup"] = "Tự động tải Flags khi khởi động (đề xuất)",
            ["Auto-load Cache on startup (recommended)"] = "Tự động tải Cache khi khởi động (đề xuất)",
            ["Auto-load Cache on startup"] = "Tự động tải Cache khi khởi động (đề xuất)",
            ["Auto-apply when Roblox is detected (recommended)"] = "Tự động apply khi phát hiện Roblox (đề xuất)",
            ["Auto-check for updates on startup (recommended)"] = "Tự động kiểm tra cập nhật khi khởi động (đề xuất)",
            ["Minimize to system tray"] = "Thu nhỏ xuống khay hệ thống",
            ["Optimizer"] = "Trình tối ưu",
            ["⚡ Optimizer"] = "Trình tối ưu",
            ["Auto-cleanup temp files (recommended)"] = "Tự động dọn dẹp tệp tạm (đề xuất)",
            ["Auto-cleanup temp files"] = "Tự động dọn dẹp tệp tạm (đề xuất)",
            ["Memory optimization (recommended)"] = "Tối ưu bộ nhớ (đề xuất)",
            ["Memory optimization"] = "Tối ưu bộ nhớ (đề xuất)",
            [" (recommended)"] = " (đề xuất)",
            ["Auto-apply when Roblox is detected"] = "Tự động apply khi phát hiện Roblox",
            ["Auto-check for updates on startup"] = "Tự động kiểm tra cập nhật khi khởi động",
            ["Save and Launch"] = "Lưu và Khởi Động",
            ["Save"] = "Lưu",
            ["Close"] = "Đóng",
            ["Load FFlags JSON"] = "Tải FFlags JSON",
            ["Load FFlags Addresses"] = "Tải Địa Chỉ FFlags",
            ["Load FFlag Addresses"] = "Tải Địa Chỉ FFlags",
            ["Activity log"] = "Nhật Ký Hoạt Động",
            ["Activity Log"] = "Nhật Ký Hoạt Động",
            ["Clear Log"] = "Xóa Nhật Ký",
            ["0 entries"] = "0 mục",
            ["{0} entries"] = "{0} mục",
            ["System initialized"] = "Hệ thống đã khởi tạo",
            ["Ready to load FFlags"] = "Sẵn sàng tải FFlags",
            ["Not set"] = "Chưa đặt",
            ["Saved FFlags:"] = "FFlags đã lưu:",
            ["Enabled"] = "Đã bật",
            ["Disabled"] = "Đã tắt",
            ["Auto-load FFlags:"] = "Tự tải FFlags:",
            ["Auto-load Addresses:"] = "Tự tải Địa chỉ:",
            ["Not detected"] = "Chưa phát hiện",
            ["Roblox Version:"] = "Phiên bản Roblox:",
            ["Unknown"] = "Không rõ",
            ["Software Version:"] = "Phiên bản phần mềm:",
            ["Version Compatibility:"] = "Tương thích phiên bản:",
            ["MATCH"] = "KHỚP",
            ["MISMATCH"] = "KHÔNG KHỚP",
            ["UNKNOWN"] = "KHÔNG RÕ",
            ["Application successful ({0} FFlags)"] = "Áp dụng thành công ({0} FFlags)",
            ["Application failed ({0} errors)"] = "Áp dụng thất bại ({0} lỗi)",
            ["now"] = "vừa xong",
            ["s ago"] = " giây trước",
            ["m ago"] = " phút trước",
            ["Success"] = "Thành công",
            ["Pending"] = "Đang chờ",
            ["Mixed"] = "Hỗn hợp",
            ["Status"] = "Trạng thái",
            ["Session"] = "Phiên",
            ["Last"] = "Lần cuối",
            ["Actions"] = "Hành động",
            ["Activity log cleared"] = "Đã xóa nhật ký hoạt động",
            ["JSON file not found"] = "Không tìm thấy tệp JSON",
            ["JSON Content Preview:"] = "Xem trước nội dung JSON:",
            ["... and {0} more entries"] = "... và {0} mục nữa",
            ["Total entries in JSON: {0}"] = "Tổng mục trong JSON: {0}",
            ["Invalid JSON format"] = "Định dạng JSON không hợp lệ",
            ["Error parsing JSON"] = "Lỗi đọc JSON",
            [" Loading FFlag addresses..."] = " Đang tải địa chỉ FFlag...",
            [" Auto-loading FFlag addresses..."] = " Đang tự tải địa chỉ FFlag...",
            ["✓ Loaded FFlag addresses successfully"] = "✓ Đã tải địa chỉ FFlag thành công",
            ["Add"] = "Thêm",
            ["Delete"] = "Xóa",
            ["Clear All"] = "Xóa Tất Cả",
            ["Export"] = "Sao lưu lại",
            ["All"] = "Tất cả",
            ["Graphics"] = "Đồ Họa",
            ["Internet"] = "Mạng",
            ["Physics"] = "Vật lý",
            ["Audio"] = "Âm thanh",
            ["Back"] = "Quay Lại",
            ["← Back"] = "← Quay Lại",
            ["Search"] = "Tìm Kiếm",
            ["Filter:"] = "Bộ lọc:",
            ["📁 Load FFlags JSON"] = "📁 Tải FFlags JSON",
            ["📄 Load FFlag Addresses"] = "📄 Tải Địa Chỉ FFlag",
            ["Add New FFlag"] = "Thêm FFlag Mới",
            ["Add New FFlags"] = "Thêm FFlags Mới",
            ["Add or batch import new flags to your library"] = "Thêm hoặc nhập hàng loạt cờ mới vào thư viện của bạn",
            ["Flag Editor"] = "Trình chỉnh sửa cờ",
            ["FLAG EDITOR"] = "TRÌNH CHỈNH SỬA CỜ",
            ["Enter flags manually or load from JSON file"] = "Nhập cờ thủ công hoặc tải từ tệp JSON",
            ["FORMAT: name: value"] = "ĐỊNH DẠNG: tên: giá trị",
            ["Each line = 1 FFlags"] = "Mỗi dòng = 1 FFlag",
            ["(Each line = 1 FFlag. Example: MyFlag: true)"] = "(Mỗi dòng = 1 FFlag. Ví dụ: MyFlag: true)",
            ["Ready to add flags"] = "Sẵn sàng thêm cờ",
            ["⚡ APPLY"] = "⚡ Áp dụng",
            ["↩️ UNAPPLY"] = "↩️ Hủy áp dụng",
            ["APPLY"] = "Áp dụng",
            ["UNAPPLY"] = "Hủy áp dụng",
            ["Cancel"] = "Hủy",
            ["Don't Save"] = "Khong luu",
            ["Unsaved Changes"] = "Thay doi chua luu",
            ["You have unsaved changes. Do you want to save before exiting?"] = "Ban co thay doi chua luu. Ban co muon luu truoc khi thoat khong?",
            ["Ready"] = "San sang",
            ["Initializing..."] = "Dang khoi tao...",
            ["Loading..."] = "Dang tai...",
            ["Loading configuration..."] = "Đang tải cấu hình...",
            ["Configuration load failed"] = "Tải cấu hình thất bại",
            ["Loaded"] = "Đã tải xong",
            ["Applying..."] = "Dang apply...",
            ["Applied"] = "Da apply",
            ["Opening Roblox..."] = "Đang mở Roblox...",
            ["Fast Mode"] = "Chế độ nhanh",
            ["Roblox Launched"] = "Da mo Roblox",
            ["Roblox Not Found"] = "Khong tim thay Roblox",
            ["Version Mismatch"] = "Khong khop phien ban",
            ["Service Error"] = "Loi dich vu",
            ["Fatal Error"] = "Loi nghiem trong",
            ["Error"] = "Loi",
            ["Failed"] = "That bai",
            ["Auto-loading..."] = "Dang tu dong tai...",
            ["Auto-Applying..."] = "Dang tu dong apply...",
            ["Auto-Applied"] = "Da tu dong apply",
            ["Flag management disabled"] = "Da tat quan ly flag",
            ["Not Implemented"] = "Chua duoc ho tro",
            ["LaunchProgressWindow"] = "Cua so tien trinh khoi dong",
            ["Launching Roblox"] = "Đang khởi động Roblox",
            ["Masterstrap - Loading..."] = "Masterstrap - Dang tai...",
            ["Masterstrap • "] = "Masterstrap • ",
            ["Initializing launch process..."] = "Dang khoi tao qua trinh khoi dong...",
            ["Loading FastFlag configuration..."] = "Dang tai cau hinh FastFlag...",
            ["Preparing Roblox launcher..."] = "Dang chuan bi trinh khoi dong Roblox...",
            ["Starting Roblox client..."] = "Dang bat dau client Roblox...",
            ["Waiting for Roblox to load..."] = "Dang cho Roblox tai...",
            ["Roblox detected! Applying optimizations..."] = "Da phat hien Roblox! Dang ap dung toi uu...",
            ["Applying optimizations..."] = "Dang ap dung toi uu...",
            ["Done!"] = "Hoan tat!",
            ["Roblox Launch"] = "Khởi động Roblox",
            ["Starting Roblox..."] = "Đang khởi động Roblox...",
            ["Waiting for Roblox to open..."] = "Đang chờ Roblox mở...",
            ["Roblox opened. Launch complete."] = "Roblox đã mở. Khởi động hoàn tất.",
            ["Masterstrap deploying auto-apply..."] = "Masterstrap đang triển khai tự động apply...",
            ["Waiting for game to be ready..."] = "Đang chờ game sẵn sàng...",
            ["Roblox closed before application."] = "Roblox đã đóng trước khi apply.",
            ["Retrying application ({0}/{1})..."] = "Đang thử apply lại ({0}/{1})...",
            ["Masterstrap auto-applying..."] = "Masterstrap đang tự động apply...",
            ["Waiting for Roblox... ({0}s)"] = "Đang chờ Roblox... ({0}s)",
            ["Roblox not detected. Closing..."] = "Không phát hiện Roblox. Đang đóng...",
            ["Launch complete."] = "Khởi động hoàn tất.",
            ["Applied successfully. Closing..."] = "Apply thành công. Đang đóng...",
            ["Application failed."] = "Apply thất bại.",
            ["Roblox not detected."] = "Không phát hiện Roblox.",
            ["Cancelled."] = "Đã hủy.",
            ["Error: {0}"] = "Lỗi: {0}",
            ["Application applied. Waiting for game to apply..."] = "Đã áp dụng apply. Đang chờ game áp dụng...",
            ["Applied successfully."] = "Apply thành công.",
            ["Failed to launch Roblox: {0}"] = "Không thể khởi động Roblox: {0}",
            ["Launch Error"] = "Lỗi khởi động",
            ["Launch complete! Closing launcher..."] = "Khoi dong hoan tat! Dang dong trinh khoi dong...",
            ["Launch cancelled"] = "Da huy khoi dong",
            ["Configuration saved successfully!"] = "Đã lưu cấu hình thành công!",
            ["Configuration saved successfully"] = "Đã lưu cấu hình thành công",
            ["CompleteAdsenseDialog created successfully"] = "Đã tạo CompleteAdsenseDialog thành công",
            ["Complete the adsense to continue"] = "Hoàn thành quảng cáo để tiếp tục",
            ["Support"] = "Hỗ trợ",
            ["Please look at and click on ads so this software project can continue for free"] = "Vui lòng xem và nhấp vào quảng cáo để dự án phần mềm này có thể tiếp tục miễn phí",
            ["⏭ Skip ad 3:00"] = "⏭ Bỏ qua quảng cáo 3:00",
            ["Skip ad"] = "Bỏ qua quảng cáo",
            ["How to skip ad? "] = "Làm sao để bỏ qua quảng cáo? ",
            ["Click here"] = "Nhấp vào đây",
            ["✓ Continue"] = "✓ Tiếp tục",
            ["✓ Ad Complete"] = "✓ Đã xem quảng cáo",
            ["Ad Complete"] = "Đã xem quảng cáo",
            ["Please wait for the countdown to finish"] = "Vui lòng chờ đếm ngược kết thúc",
            ["Please wait"] = "Vui lòng đợi",
            ["Please click 'Continue' button to proceed"] = "Vui lòng nhấn nút 'Tiếp tục' để tiếp tục",
            ["Ad Completed"] = "Đã xem quảng cáo",
            ["Could not open support link"] = "Không thể mở liên kết hỗ trợ",
            ["Could not open help link"] = "Không thể mở liên kết trợ giúp",
            ["Could not open Discord link"] = "Không thể mở liên kết Discord",
            ["Adsense dialog marked as OPEN"] = "Hộp thoại Adsense đã đánh dấu là MỞ",
            ["Adsense dialog marked as CLOSED"] = "Hộp thoại Adsense đã đánh dấu là ĐÓNG",
            ["FFlags applied successfully!"] = "Đã áp dụng FFlags thành công!",
            ["Configuration saved and Roblox launched!"] = "Da luu cau hinh va khoi dong Roblox!",
            ["Launch failed!"] = "Khoi dong that bai!",
            ["Apply"] = "Áp dụng",
            ["FastFlag Editor"] = "Trình chỉnh sửa FastFlag",
            ["manage your own Fast Flags. Use with caution"] = "Quản lý các Fast Flag của riêng bạn. Hãy sử dụng cẩn thận.",
            ["Allow Masterstrap to manage Fast Flags"] = "Cho phép Masterstrap quản lý Fast Flag",
            ["Turning off this option will prevent any configuration here from being applied to Roblox."] = "Tắt tùy chọn này sẽ ngăn mọi cấu hình tại đây được áp dụng cho Roblox.",
            ["Rendering and Graphics"] = "Kết xuất và Đồ họa",
            ["Anti-aliasing quality (MSAA)"] = "Chất lượng khử răng cưa (MSAA)",
            ["Preserve rendering quality with display scaling"] = "Giữ nguyên chất lượng kết xuất khi dùng tỷ lệ thu phóng màn hình",
            ["Roblox reduces your rendering quality depending on how your display is scaled in Windows."] = "Roblox sẽ giảm chất lượng kết xuất tùy theo cách màn hình của bạn được thu phóng trong Windows.",
            ["FRM Quality Override"] = "Ghi đè chất lượng FRM",
            ["Choose the FRM quality that Roblox should use."] = "Chọn mức chất lượng FRM mà Roblox sẽ sử dụng.",
            ["Rendering mode"] = "Chế độ kết xuất",
            ["Texture quality"] = "Chất lượng texture (chất lượng bề mặt hình ảnh)",
            ["Set as Read-Only"] = "Đặt thành Chỉ đọc",
            ["Prevent Roblox from overriding global settings."] = "Ngăn Roblox ghi đè các cài đặt toàn cục.",
            ["Presets"] = "Cấu hình sẵn",
            ["Graphics Quality"] = "Chất lượng đồ họa",
            ["Graphic advanced"] = "Đồ họa nâng cao",
            ["Set the graphics quality of your game"] = "Thiết lập chất lượng đồ họa của trò chơi",
            ["Max Quality Enabled"] = "Bật chất lượng tối đa",
            ["Enables maximum graphics quality mode for enhanced visual effects and rendering detail."] = "Kích hoạt chế độ chất lượng đồ họa tối đa để tăng cường hiệu ứng hình ảnh và độ chi tiết khi kết xuất.",
            ["Graphics Quality Level"] = "Mức chất lượng đồ họa",
            ["Adjusts the in-game graphics quality level from low to maximum."] = "Điều chỉnh mức chất lượng đồ họa trong game từ thấp đến tối đa.",
            ["Framerate Limit"] = "Giới hạn khung hình",
            ["Unlock framerate limit for Roblox. Going above 240 FPS is not recommended."] = "Mở khóa giới hạn khung hình cho Roblox. Không khuyến nghị vượt quá 240 FPS.",
            ["User Interface and Layout"] = "Giao diện người dùng và Bố cục",
            ["Transparency"] = "Độ trong suốt",
            ["Custom transparency for UI elements."] = "Tùy chỉnh độ trong suốt cho các thành phần giao diện.",
            ["Reduced Motion"] = "Giảm chuyển động",
            ["Removes the animation on the escape menu."] = "Loại bỏ hiệu ứng chuyển động trong menu Escape.",
            ["Font Size"] = "Kích thước phông chữ",
            ["Choose how large the font should appear."] = "Chọn kích thước hiển thị của phông chữ.",
            ["Default"] = "Mặc định",
            ["Other"] = "Khác",
            ["Mouse Sensitivity"] = "Độ nhạy chuột",
            ["Change how fast the camera will move in-game."] = "Thay đổi tốc độ di chuyển của camera trong game.",
            ["VR Enabled"] = "Bật VR",
            ["Player Name Visibility"] = "Hiển thị tên người chơi",
            ["Hide name tags above other players for a cleaner screen experience."] = "Ẩn tên hiển thị phía trên người chơi khác để có trải nghiệm màn hình gọn gàng hơn.",
            ["Hide name tags above other players for a cleaner screen experience. Hotkey: Ctrl+Shift+N"] = "Ẩn tên trên đầu người chơi khác. Phím tắt trong Roblox: Ctrl+Shift+N",
            ["ON = show player name tags. OFF = hide tags (use Ctrl+Shift+N in Roblox). Does not modify FFlag apply files. Saved in Masterstrap settings."] = "BẬT = hiện tên người chơi. TẮT = ẩn tên (dùng Ctrl+Shift+N trong Roblox). Không sửa file FFlag apply. Lưu trong cài đặt Masterstrap.",
            ["In-game hotkey: Ctrl+Shift+N to toggle player names."] = "Phím tắt trong Roblox: Ctrl+Shift+N để bật/tắt tên người chơi.",
            ["Works with any FFlag preset (standalone). Roblox: Ctrl+Shift+N for names. Menu hotkeys (Ctrl+Shift+B/C) stay enabled. No Bloxstrap group."] = "Hoạt động với mọi preset FFlag (độc lập). Roblox: Ctrl+Shift+N ẩn/hiện tên. Phím menu (Ctrl+Shift+B/C) vẫn bật. Không cần group Bloxstrap.",
            ["Could not sync names to Roblox — press Ctrl+Shift+N in-game or focus the game window"] = "Không đồng bộ được — bấm Ctrl+Shift+N trong game hoặc focus cửa sổ Roblox",
            ["Player name hotkey is not ready yet — join a game first, then use Ctrl+Shift+N in Roblox"] = "Phím tắt tên người chơi chưa sẵn sàng — vào game trước, sau đó dùng Ctrl+Shift+N trong Roblox",
            ["Preference saved. Restart Roblox from Masterstrap, then use Ctrl+Shift+N in-game"] = "Đã lưu. Khởi động lại Roblox từ Masterstrap, rồi dùng Ctrl+Shift+N trong game",
            ["Preference saved — press Ctrl+Shift+N in Roblox to hide names"] = "Đã lưu — bấm Ctrl+Shift+N trong Roblox để ẩn tên",
            ["Preference saved — press Ctrl+Shift+N in Roblox to show names"] = "Đã lưu — bấm Ctrl+Shift+N trong Roblox để hiện tên",
            ["Roblox is not running"] = "Roblox chưa chạy",
            ["Player names hidden in-game"] = "Đã ẩn tên người chơi trong game",
            ["Player names shown in-game"] = "Đã hiện tên người chơi trong game",
            ["Player names will be hidden on next launch"] = "Tên người chơi sẽ được ẩn khi mở Roblox lần sau",
            ["Player names will be shown on next launch"] = "Tên người chơi sẽ được hiện khi mở Roblox lần sau",
            ["Could not find PlayerNamesEnabled in Roblox memory"] = "Không tìm thấy PlayerNamesEnabled trong bộ nhớ Roblox",
            ["Could not sync names to Roblox — press Ctrl+Shift+N in-game or focus the game window"] = "Không đồng bộ được — bấm Ctrl+Shift+N trong game hoặc focus cửa sổ Roblox",
            ["Native engine could not attach to Roblox"] = "Native engine không gắn được vào Roblox",
            ["Native write failed"] = "Ghi bộ nhớ Native thất bại",
            ["FAQ and Guide"] = "Câu hỏi thường gặp và Hướng dẫn",
            ["📖 FAQ and Guide"] = "Câu hỏi thường gặp và Hướng dẫn",
            ["How to Use Masterstrap"] = "Cách sử dụng Masterstrap",
            ["❔ How to Use Masterstrap"] = "Cách sử dụng Masterstrap",
            ["1. Load FFlags JSON file"] = "Tải tệp FFlags JSON",
            ["2. Load FFlag Addresses (optional)"] = "Tải địa chỉ FFlag (tùy chọn)",
            ["3. Make sure Roblox is running"] = "Đảm bảo Roblox đang chạy",
            ["4. Click APPLY button to apply FFlags"] = "Nhấn nút APPLY để apply FFlags",
            ["5. Check Activity Log for results"] = "Kiểm tra Activity Log để xem kết quả",
            ["Apply"] = "Áp dụng",
            ["How to Edit FFlags"] = "Cách chỉnh sửa FFlags",
            ["✏️ How to Edit FFlags"] = "Cách chỉnh sửa FFlags",
            ["• Go to Edit tab to modify loaded FFlags"] = "Vào tab Edit để chỉnh sửa các FFlags đã tải",
            ["• Click Add to create new FFlag entry"] = "Nhấn Add để tạo mục FFlag mới",
            ["• Click Delete to remove selected FFlag"] = "Nhấn Delete để xóa FFlag đã chọn",
            ["• Use Search to find specific FFlags"] = "Sử dụng Search để tìm FFlags cụ thể",
            ["• Click Export to save modified FFlags"] = "Nhấn Export để lưu các FFlags đã chỉnh sửa",
            ["Troubleshooting"] = "Khắc phục sự cố",
            ["🔧 Troubleshooting"] = "Khắc phục sự cố",
            ["Roblox not found?"] = "Không tìm thấy Roblox?",
            ["⚠️ Roblox not found?"] = "Không tìm thấy Roblox?",
            ["Make sure Roblox is running before applying"] = "Đảm bảo Roblox đang chạy trước khi apply",
            ["Application failed?"] = "Apply thất bại?",
            ["⚠️ Application failed?"] = "Apply thất bại?",
            ["Please ensure that your Roblox version matches the version that Masterstrap has requested"] = "Đảm bảo phiên bản Roblox của bạn khớp với phiên bản mà Masterstrap yêu cầu",
            ["FFlags not loading?"] = "FFlags không tải được?",
            ["⚠️ FFlags not loading?"] = "FFlags không tải được?",
            ["Verify JSON file format is correct and valid"] = "Kiểm tra định dạng tệp JSON có chính xác và hợp lệ hay không",
            ["Game crash after applying?"] = "Game bị crash sau khi apply?",
            ["⚠️ Game crash after applying?"] = "Game bị crash sau khi apply?",
            ["Reason: FFlag has targetfps set too high, causing device overload and crash. Please click 'Edit FFlag' and change 'targetfps' value to 300-400"] = "Nguyên nhân: FFlag đặt targetfps quá cao, gây quá tải và crash. Vui lòng nhấn 'Chỉnh sửa FFlag' và đổi giá trị 'targetfps' thành 300-400",
            ["Tips and Tricks"] = "Mẹo và Thủ thuật",
            ["💡 Tips and Tricks"] = "Mẹo và Thủ thuật",
            ["• Keep your FFlag JSON file backed up"] = "Sao lưu tệp FFlag JSON của bạn",
            ["• Export frequently to save your changes"] = "Xuất (Export) thường xuyên để lưu các thay đổi",
            ["• Use Search feature to quickly find FFlags"] = "Sử dụng tính năng Search để tìm FFlags nhanh hơn",
            ["• Check Activity Log for application status"] = "Kiểm tra Activity Log để xem trạng thái apply",
            ["Home"] = "Trang Chủ",
            ["Global"] = "Cài Đặt Nhanh",
            ["Games"] = "Trò Chơi",
            ["Settings"] = "Cài Đặt",
            ["FAQ"] = "Câu Hỏi Thường Gặp",
            ["⚡ FFlags"] = "⚡ Áp dụng",
            ["🌐 Global"] = "🌐 Cài Đặt Nhanh",
            ["🎮 Game FFlags"] = "🎮 Trò Chơi",
            ["⚙️ Settings"] = "⚙️ Cài Đặt",
            ["❓ FAQ"] = "❓ Câu Hỏi Thường Gặp",
            ["Join"] = "Tham gia",
            ["Join Discord"] = "Tham gia Discord",
            ["Open the editor to view and change flags, use presets, and choose whether Masterstrap may apply them when you launch."] = "Mở trình chỉnh sửa để xem và chỉnh sửa cờ, dùng preset, và chọn xem Masterstrap có áp dụng khi bạn khởi chạy hay không.",
            ["Adjust Roblox wide settings such as read only mode, rendering, and framerate limits."] = "Điều chỉnh cài đặt toàn cục Roblox như chế độ chỉ đọc, kết xuất đồ họa và giới hạn khung hình.",
            ["Edit the flag table, filter by category, search, then use Back and Save on the FastFlags page when you are done."] = "Chỉnh sửa bảng cờ, lọc theo danh mục, tìm kiếm, rồi dùng Quay lại và Lưu trên trang FastFlags khi hoàn tất.",
            ["Choose language, visual theme, and startup behavior for the app."] = "Chọn ngôn ngữ, giao diện và hành vi khởi động của ứng dụng.",
            ["Settings and Theme"] = "Cài đặt và giao diện",
            ["Choose light or dark interface colors."] = "Chọn màu giao diện sáng hoặc tối.",
            ["Effect Theme"] = "Chủ đề hiệu ứng",
            ["Background Image"] = "Hình nền",
            ["Upload image for glass modes only (Default mode keeps normal background)."] = "Tải ảnh chỉ dùng cho chế độ kính (chế độ Mặc định giữ nền bình thường).",
            ["Upload Background"] = "Tải lên hình nền",
            ["Reset Background"] = "Đặt lại hình nền",
            ["Light"] = "Sáng",
            ["Dark"] = "Tối",
            ["Load a curated flag set for a game into your list, then adjust or save as usual."] = "Tải bộ cờ được tuyển chọn cho một trò chơi vào danh sách, rồi chỉnh sửa hoặc lưu như bình thường.",
            ["Credits, FAQ, and short guides for using Masterstrap."] = "Credits, FAQ và hướng dẫn ngắn để sử dụng Masterstrap.",
            ["Made By ©Dank1ngs"] = "được thực hiện bởi ©Dank1ngs"
            }), AboutTabUiTranslations.EnToVi),
            MergeUiOverlays(MergeUiOverlays(BuildFlagUiTranslations.EnToVi, LaunchProgressUiTranslations.EnToVi), DialogsUiTranslations.EnToVi));

        private static readonly Dictionary<string, string> EnToFil = OverlayAboutTab(
            OverlayAboutTab(
            MergeTranslations(
            LoadMdTranslations("Masterstrap.Resources.filipino.md"),
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Home"] = "Bahay",
                ["FastFlag"] = "FastFlag",
                ["Global"] = "Global",
                ["Games"] = "Mga Laro",
                ["Settings"] = "Mga Setting",
                ["FAQ"] = "Mga Madalas Itanong",
                ["⚙️ Settings"] = "⚙️ Mga Setting",
                ["❓ FAQ"] = "❓ Mga Madalas Itanong",
                ["INFORMATION SYSTEM"] = "Sistema ng Impormasyon",
                ["FFlags:"] = "FFlags:",
                ["Count:"] = "Bilang:",
                ["Roblox Version:"] = "Bersyon ng Roblox:",
                ["Software Version:"] = "Bersyon ng Software:",
                ["Last update:"] = "Huling update:",
                ["📁 Load FFlags JSON"] = "📁 I-load ang FFlags JSON",
                ["📄 Load FFlag Addresses"] = "📄 I-load ang Mga Address ng FFlag",
                ["Add New FFlag"] = "Magdagdag ng Bagong FFlag",
                ["Add New FFlags"] = "Magdagdag ng Bagong FFlags",
                ["Add or batch import new flags to your library"] = "Magdagdag o batch import ng mga bagong flag sa iyong library",
                ["Flag Editor"] = "Flag Editor",
                ["FLAG EDITOR"] = "FLAG EDITOR",
                ["Enter flags manually or load from JSON file"] = "Ilagay ang mga flag nang manu-mano o mag-load mula sa JSON file",
                ["FORMAT: name: value"] = "FORMAT: pangalan: halaga",
                ["Each line = 1 FFlags"] = "Bawat linya = 1 FFlag",
                ["(Each line = 1 FFlag. Example: MyFlag: true)"] = "(Bawat linya = 1 FFlag. Hal: MyFlag: true)",
                ["Ready to add flags"] = "Handa nang magdagdag ng mga flag",
                ["Configuration saved successfully!"] = "Matagumpay na nai-save ang configuration!",
                ["Configuration saved successfully"] = "Matagumpay na nai-save ang configuration",
                ["CompleteAdsenseDialog created successfully"] = "Matagumpay na nalikha ang CompleteAdsenseDialog",
                ["Complete the adsense to continue"] = "Kumpletuhin ang adsense para magpatuloy",
                ["Support"] = "Suporta",
                ["Please look at and click on ads so this software project can continue for free"] = "Mangyaring tingnan at i-click ang mga ad upang ang proyektong ito ay maaaring magpatuloy nang libre",
                ["⏭ Skip ad 3:00"] = "⏭ Laktawan ang ad 3:00",
                ["Skip ad"] = "Laktawan ang ad",
                ["How to skip ad? "] = "Paano laktawan ang ad? ",
                ["Click here"] = "I-click dito",
                ["✓ Continue"] = "✓ Magpatuloy",
                ["✓ Ad Complete"] = "✓ Tapos na ang Ad",
                ["Ad Complete"] = "Tapos na ang Ad",
                ["Please wait for the countdown to finish"] = "Mangyaring maghintay hanggang matapos ang countdown",
                ["Please wait"] = "Mangyaring maghintay",
                ["Please click 'Continue' button to proceed"] = "Mangyaring i-click ang pindutang 'Magpatuloy' para magpatuloy",
                ["Ad Completed"] = "Tapos na ang Ad",
                ["Could not open support link"] = "Hindi mabuksan ang link ng suporta",
                ["Could not open help link"] = "Hindi mabuksan ang link ng tulong",
                ["Could not open Discord link"] = "Hindi mabuksan ang link ng Discord",
                ["Adsense dialog marked as OPEN"] = "Markado bilang OPEN ang dialog ng Adsense",
                ["Adsense dialog marked as CLOSED"] = "Markado bilang CLOSED ang dialog ng Adsense",
                ["FFlags applied successfully!"] = "Matagumpay na na-apply ang FFlags!",
                ["⚡ APPLY"] = "⚡ I-apply",
                ["↩️ UNAPPLY"] = "↩️ I-restore",
                ["Load FFlags JSON"] = "I-load ang FFlags JSON",
                ["Load FFlag Addresses"] = "I-load ang Mga Address ng FFlag",
                ["APPLY"] = "I-apply",
                ["UNAPPLY"] = "I-restore",
                ["Auto-check for updates on startup (recommended)"] = "Awomatikong suriin ang mga update sa pagsisimula (inirerekomenda)",
                ["Auto-check for updates on startup"] = "Awomatikong suriin ang mga update sa pagsisimula",
                ["🔹 Activity Log"] = "🔹 Talaan ng Aktibidad",
                ["Activity log"] = "Talaan ng Aktibidad",
                ["Activity Log"] = "Talaan ng Aktibidad",
                ["Clear Log"] = "Burahin ang Talaan",
                ["0 entries"] = "0 mga entry",
                ["System initialized"] = "Na-initialize ang system",
                ["Ready to load FFlags"] = "Handa nang i-load ang FFlags",
                ["Not set"] = "Hindi naka-set",
                ["Saved FFlags:"] = "Naka-save na FFlags:",
                ["Enabled"] = "Na-enable",
                ["Disabled"] = "Na-disable",
                ["Auto-load FFlags:"] = "Awtomatikong i-load ang FFlags:",
                ["Auto-load Addresses:"] = "Awtomatikong i-load ang mga Address:",
                ["Not detected"] = "Hindi na-detect",
                ["Roblox Version:"] = "Bersyon ng Roblox:",
                ["Unknown"] = "Hindi alam",
                ["Software Version:"] = "Bersyon ng Software:",
                ["Version Compatibility:"] = "Pagkakatugma ng Bersyon:",
                ["MATCH"] = "TUGMA",
                ["MISMATCH"] = "HINDI TUGMA",
                ["UNKNOWN"] = "HINDI ALAM",
                ["Application successful ({0} FFlags)"] = "Matagumpay ang application ({0} FFlags)",
                ["Application failed ({0} errors)"] = "Nabigo ang application ({0} mga error)",
                ["now"] = "ngayon",
                ["s ago"] = " segundong nakalipas",
                ["m ago"] = " minutong nakalipas",
                ["Success"] = "Tagumpay",
                ["Failed"] = "Nabigo",
                ["Pending"] = "Naghihintay",
                ["Mixed"] = "Halo-halo",
                ["Status"] = "Estado",
                ["Session"] = "Session",
                ["Last"] = "Huli",
                ["Actions"] = "Mga Aksyon",
                ["Activity log cleared"] = "Nabura ang talaan ng aktibidad",
                ["JSON file not found"] = "Hindi mahanap ang file na JSON",
                ["JSON Content Preview:"] = "Preview ng Nilalaman ng JSON:",
                ["... and {0} more entries"] = "... at {0} pang mga entry",
                ["Total entries in JSON: {0}"] = "Kabuuang mga entry sa JSON: {0}",
                ["Invalid JSON format"] = "Hindi wastong format ng JSON",
                ["Error parsing JSON"] = "Error sa pag-parse ng JSON",
                [" Loading FFlag addresses..."] = " Naglo-load ng mga address ng FFlag...",
                [" Auto-loading FFlag addresses..."] = " Awtomatikong naglo-load ng mga address ng FFlag...",
                ["Not loaded"] = "Hindi na-load",
                ["Save and Launch"] = "I-save at I-launch",
                ["Save"] = "I-save",
                ["Close"] = "Isara",
                ["Cancel"] = "Kanselahin",
                ["Fast Mode"] = "Mabilis na Mode",
                ["Loaded"] = "Na-load",
                ["Opening Roblox..."] = "Binubuksan ang Roblox...",
                ["Idle"] = "Idle",
                ["Join"] = "Sumali",
                ["Join Discord"] = "Sumali sa Discord",
                ["Roblox Launch"] = "Paglulunsad ng Roblox",
                ["Launching Roblox"] = "Inilulunsad ang Roblox",
                ["Masterstrap - Loading..."] = "Masterstrap - Naglo-load...",
                ["Masterstrap • "] = "Masterstrap • ",
                ["Loading FastFlag configuration..."] = "Naglo-load ng configuration ng FastFlag...",
                ["Starting Roblox..."] = "Sinimulan ang Roblox...",
                ["Waiting for Roblox to open..."] = "Naghhintay na magbukas ang Roblox...",
                ["Roblox opened. Launch complete."] = "Nagbukas na ang Roblox. Kumpleto na ang paglulunsad.",
                ["Masterstrap deploying auto-apply..."] = "Masterstrap nagde-deploy ng auto-apply...",
                ["Waiting for game to be ready..."] = "Naghhintay na maging handa ang laro...",
                ["Roblox closed before application."] = "Isinara ang Roblox bago ang application.",
                ["Retrying application ({0}/{1})..."] = "Sinusubukang muli ang application ({0}/{1})...",
                ["Masterstrap auto-applying..."] = "Masterstrap nag-o-auto-apply...",
                ["Auto-applying..."] = "Nag-o-auto-apply...",
                ["Waiting for Roblox... ({0}s)"] = "Naghhintay ng Roblox... ({0}s)",
                ["Roblox not detected. Closing..."] = "Hindi nakita ang Roblox. Isinasarado...",
                ["Launch complete."] = "Kumpleto na ang paglulunsad.",
                ["Applied successfully. Closing..."] = "Matagumpay ang application. Isinasarado...",
                ["Application failed."] = "Nabigo ang application.",
                ["Roblox not detected."] = "Hindi nakita ang Roblox.",
                ["Cancelled."] = "Kanselado.",
                ["Error: {0}"] = "Error: {0}",
                ["Application applied. Waiting for game to apply..."] = "Na-apply na ang application. Naghhintay na i-apply ng laro...",
                ["Applied successfully."] = "Matagumpay ang application.",
                ["Failed to launch Roblox: {0}"] = "Nabigo ang paglulunsad ng Roblox: {0}",
                ["Launch Error"] = "Error sa Paglulunsad",
                ["Open the editor to view and change flags, use presets, and choose whether Masterstrap may apply them when you launch."] = "Buksan ang editor para tingnan at baguhin ang mga flag, gumamit ng preset, at piliin kung ilalapat ng Masterstrap ang mga ito kapag nagla-launch ka.",
                ["Adjust Roblox wide settings such as read only mode, rendering, and framerate limits."] = "Ayusin ang mga pandaigdigang setting ng Roblox tulad ng read-only mode, rendering, at limitasyon ng framerate.",
                ["Edit the flag table, filter by category, search, then use Back and Save on the FastFlags page when you are done."] = "I-edit ang talahanayan ng flag, salain ayon sa kategorya, maghanap, gamitin ang Back at Save sa pahina ng FastFlags kapag tapos ka na.",
                ["Choose language, visual theme, and startup behavior for the app."] = "Pumili ng wika, visual theme, at ugali sa startup ng app.",
                ["Load a curated flag set for a game into your list, then adjust or save as usual."] = "Mag-load ng piniling set ng flag para sa isang laro sa iyong listahan, pagkatapos ay ayusin o i-save gaya ng dati.",
                ["Credits, FAQ, and short guides for using Masterstrap."] = "Credits, FAQ, at maikling gabay sa paggamit ng Masterstrap.",
                ["Made By ©Dank1ngs"] = "gawa ni ©Dank1ngs",
                ["Auto-load FFlags on startup (recommended)"] = "Awtomatikong i-load ang mga FFlag sa pag-start (inirerekomenda)",
                ["Auto-load FFlags on startup"] = "Awtomatikong i-load ang mga FFlag sa pag-start",
                ["Select Game FFlags Preset"] = "Piliin ang Preset ng FFlags ng Laro",
                ["🎮 Select Game FFlags Preset"] = "🎮 Piliin ang Preset ng FFlags ng Laro",
                ["Graphic advanced"] = "Mas advanced na graphics",
                ["Apply"] = "Ilapat",
                [" (recommended)"] = " (inirerekomenda)",
                ["English"] = "English",
                ["Filipino"] = "Filipino"
            }), AboutTabUiTranslations.EnToFil),
            MergeUiOverlays(MergeUiOverlays(BuildFlagUiTranslations.EnToFil, LaunchProgressUiTranslations.EnToFil), DialogsUiTranslations.EnToFil));

        private static readonly Dictionary<string, string> EnToId = OverlayAboutTab(
            OverlayAboutTab(
            MergeTranslations(
            LoadMdTranslations("Masterstrap.Resources.Indonesian.md"),
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Settings and Options"] = "Pengaturan dan Opsi",
                ["Select Game FFlags Preset"] = "Pilih Preset FFlags Permainan",
                ["🎮 Select Game FFlags Preset"] = "🎮 Pilih Preset FFlags Permainan",
                ["Update Soon..."] = "Segera Hadir...",
                ["Information System"] = "Sistem Informasi",
                ["INFORMATION SYSTEM"] = "SISTEM INFORMASI",
                ["FFlags:"] = "FFlags:",
                ["Count:"] = "Jumlah:",
                ["Roblox Version:"] = "Versi Roblox:",
                ["Software Version:"] = "Versi Perangkat Lunak:",
                ["Last update:"] = "Pembaruan terakhir:",
                ["0 entries"] = "0 entri",
                ["Not loaded"] = "Belum dimuat",
                ["Fast Mode"] = "Mode Cepat",
                ["Loaded"] = "Dimuat",
                ["Made By ©Dank1ngs"] = "dibuat oleh ©Dank1ngs",
                ["Idle"] = "Menganggur",
                ["Join"] = "Bergabung",
                ["Join Discord"] = "Bergabung Discord",
                ["Language Settings"] = "Pengaturan Bahasa",
                ["Select your preferred display language for the application interface."] = "Pilih bahasa tampilan yang Anda inginkan untuk antarmuka aplikasi.",
                ["Vietnamese"] = "Bahasa Vietnam",
                ["Desktop Shortcut"] = "Pintasan Desktop",
                ["Create a shortcut on your Desktop for quick access to Masterstrap (recommended)"] = "Buat pintasan di Desktop untuk akses cepat ke Masterstrap (disarankan)",
                ["Create a shortcut on your Desktop for quick access to Masterstrap"] = "Buat pintasan di Desktop untuk akses cepat ke Masterstrap (disarankan)",
                ["General Settings"] = "Pengaturan Umum",
                ["📋 General Settings"] = "Pengaturan Umum",
                ["Auto-load FFlags on startup (recommended)"] = "Muat FFlags otomatis saat startup (disarankan)",
                ["Auto-load FFlags on startup"] = "Muat FFlags otomatis saat startup",
                ["Auto-load Cache on startup (recommended)"] = "Muat Cache otomatis saat startup (disarankan)",
                ["Auto-load Cache on startup"] = "Muat Cache otomatis saat startup",
                ["Auto-apply when Roblox is detected (recommended)"] = "Apply otomatis saat Roblox terdeteksi (disarankan)",
                ["Auto-check for updates on startup (recommended)"] = "Periksa pembaruan otomatis saat startup (disarankan)",
                ["Minimize to system tray"] = "Minimalkan ke baki sistem",
                ["Optimizer"] = "Pengoptimal",
                ["⚡ Optimizer"] = "Pengoptimal",
                ["Auto-cleanup temp files (recommended)"] = "Bersihkan file sementara otomatis (disarankan)",
                ["Auto-cleanup temp files"] = "Bersihkan file sementara otomatis",
                ["Memory optimization (recommended)"] = "Optimasi memori (disarankan)",
                ["Memory optimization"] = "Optimasi memori",
                [" (recommended)"] = " (disarankan)",
                ["Auto-apply when Roblox is detected"] = "Apply otomatis saat Roblox terdeteksi",
                ["Auto-check for updates on startup"] = "Periksa pembaruan saat startup",
                ["Save and Launch"] = "Simpan dan Luncurkan",
                ["Save"] = "Simpan",
                ["Close"] = "Tutup",
                ["Load FFlags JSON"] = "Muat FFlags JSON",
                ["Load FFlags Addresses"] = "Muat Alamat FFlags",
                ["Load FFlag Addresses"] = "Muat Alamat FFlag",
                ["Activity log"] = "Log Aktivitas",
                ["Activity Log"] = "Log Aktivitas",
                ["Clear Log"] = "Hapus Log",
                ["0 entries"] = "0 entri",
                ["System initialized"] = "Sistem diinisialisasi",
                ["Ready to load FFlags"] = "Siap memuat FFlags",
                ["Not set"] = "Belum diatur",
                ["Saved FFlags:"] = "FFlags tersimpan:",
                ["Enabled"] = "Diaktifkan",
                ["Disabled"] = "Nonaktif",
                ["Auto-load FFlags:"] = "Muat otomatis FFlags:",
                ["Auto-load Addresses:"] = "Muat otomatis Alamat:",
                ["Not detected"] = "Tidak terdeteksi",
                ["Roblox Version:"] = "Versi Roblox:",
                ["Unknown"] = "Tidak diketahui",
                ["Software Version:"] = "Versi Perangkat Lunak:",
                ["Version Compatibility:"] = "Kompatibilitas Versi:",
                ["MATCH"] = "COCOK",
                ["MISMATCH"] = "TIDAK COCOK",
                ["UNKNOWN"] = "TIDAK DIKETAHUI",
                ["Application successful ({0} FFlags)"] = "Apply berhasil ({0} FFlags)",
                ["Application failed ({0} errors)"] = "Apply gagal ({0} error)",
                ["now"] = "baru saja",
                ["s ago"] = " detik lalu",
                ["m ago"] = " menit lalu",
                ["Success"] = "Berhasil",
                ["Failed"] = "Gagal",
                ["Pending"] = "Menunggu",
                ["Mixed"] = "Campuran",
                ["Status"] = "Status",
                ["Session"] = "Sesi",
                ["Last"] = "Terakhir",
                ["Actions"] = "Tindakan",
                ["Activity log cleared"] = "Log aktivitas dibersihkan",
                ["JSON file not found"] = "File JSON tidak ditemukan",
                ["JSON Content Preview:"] = "Pratinjau Konten JSON:",
                ["... and {0} more entries"] = "... dan {0} entri lainnya",
                ["Total entries in JSON: {0}"] = "Total entri dalam JSON: {0}",
                ["Invalid JSON format"] = "Format JSON tidak valid",
                ["Error parsing JSON"] = "Error mem-parsing JSON",
                [" Loading FFlag addresses..."] = " Memuat alamat FFlag...",
                [" Auto-loading FFlag addresses..."] = " Memuat otomatis alamat FFlag...",
                ["Add"] = "Tambah",
                ["Delete"] = "Hapus",
                ["Clear All"] = "Hapus Semua",
                ["Export"] = "Ekspor",
                ["All"] = "Semua",
                ["Graphics"] = "Grafis",
                ["Internet"] = "Internet",
                ["Physics"] = "Fisika",
                ["Audio"] = "Audio",
                ["Back"] = "Kembali",
                ["← Back"] = "← Kembali",
                ["Search"] = "Cari",
                ["Filter:"] = "Filter:",
                ["📁 Load FFlags JSON"] = "📁 Muat FFlags JSON",
                ["📄 Load FFlag Addresses"] = "📄 Muat Alamat FFlag",
                ["Add New FFlag"] = "Tambah FFlag Baru",
                ["Add New FFlags"] = "Tambah FFlags Baru",
                ["Add or batch import new flags to your library"] = "Tambah atau impor batch flag baru ke perpustakaan Anda",
                ["Flag Editor"] = "Editor Flag",
                ["FLAG EDITOR"] = "EDITOR FLAG",
                ["Enter flags manually or load from JSON file"] = "Masukkan flag secara manual atau muat dari file JSON",
                ["FORMAT: name: value"] = "FORMAT: nama: nilai",
                ["Each line = 1 FFlags"] = "Setiap baris = 1 FFlag",
                ["(Each line = 1 FFlag. Example: MyFlag: true)"] = "(Setiap baris = 1 FFlag. Contoh: MyFlag: true)",
                ["Ready to add flags"] = "Siap menambah flag",
                ["Configuration saved successfully!"] = "Konfigurasi berhasil disimpan!",
                ["Configuration saved successfully"] = "Konfigurasi berhasil disimpan",
                ["CompleteAdsenseDialog created successfully"] = "CompleteAdsenseDialog berhasil dibuat",
                ["Complete the adsense to continue"] = "Selesaikan iklan untuk melanjutkan",
                ["Support"] = "Dukungan",
                ["Please look at and click on ads so this software project can continue for free"] = "Silakan lihat dan klik iklan agar proyek perangkat lunak ini dapat terus berjalan secara gratis",
                ["⏭ Skip ad 3:00"] = "⏭ Lewati iklan 3:00",
                ["Skip ad"] = "Lewati iklan",
                ["How to skip ad? "] = "Cara melewati iklan? ",
                ["Click here"] = "Klik di sini",
                ["✓ Continue"] = "✓ Lanjutkan",
                ["✓ Ad Complete"] = "✓ Iklan selesai",
                ["Ad Complete"] = "Iklan selesai",
                ["Please wait for the countdown to finish"] = "Harap tunggu hingga hitungan mundur selesai",
                ["Please wait"] = "Harap tunggu",
                ["Please click 'Continue' button to proceed"] = "Silakan klik tombol 'Lanjutkan' untuk melanjutkan",
                ["Ad Completed"] = "Iklan selesai",
                ["Could not open support link"] = "Tidak dapat membuka tautan dukungan",
                ["Could not open help link"] = "Tidak dapat membuka tautan bantuan",
                ["Could not open Discord link"] = "Tidak dapat membuka tautan Discord",
                ["Adsense dialog marked as OPEN"] = "Dialog Adsense ditandai sebagai BUKA",
                ["Adsense dialog marked as CLOSED"] = "Dialog Adsense ditandai sebagai TUTUP",
                ["FFlags applied successfully!"] = "FFlags berhasil disuntikkan!",
                ["⚡ APPLY"] = "⚡ Terapkan",
                ["↩️ UNAPPLY"] = "↩️ Batalkan suntik",
                ["APPLY"] = "Terapkan",
                ["UNAPPLY"] = "Batalkan suntik",
                ["Cancel"] = "Batal",
                ["Don't Save"] = "Jangan Simpan",
                ["Unsaved Changes"] = "Perubahan Belum Disimpan",
                ["You have unsaved changes. Do you want to save before exiting?"] = "Anda memiliki perubahan yang belum disimpan. Simpan sebelum keluar?",
                ["Ready"] = "Siap",
                ["Initializing..."] = "Menginisialisasi...",
                ["Loading..."] = "Memuat...",
                ["Loaded"] = "Dimuat",
                ["Applying..."] = "Menyuntikkan...",
                ["Applied"] = "Tersuntik",
                ["Opening Roblox..."] = "Membuka Roblox...",
                ["Roblox Launched"] = "Roblox Diluncurkan",
                ["Roblox Not Found"] = "Roblox Tidak Ditemukan",
                ["Version Mismatch"] = "Versi Tidak Cocok",
                ["Service Error"] = "Kesalahan Layanan",
                ["Fatal Error"] = "Kesalahan Fatal",
                ["Error"] = "Kesalahan",
                ["Failed"] = "Gagal",
                ["Auto-loading..."] = "Memuat otomatis...",
                ["Auto-Applying..."] = "Apply otomatis...",
                ["Auto-Applied"] = "Tersuntik otomatis",
                ["Flag management disabled"] = "Manajemen flag dinonaktifkan",
                ["Not Implemented"] = "Belum Diimplementasikan",
                ["LaunchProgressWindow"] = "Jendela Proses Peluncuran",
                ["Launching Roblox"] = "Meluncurkan Roblox",
                ["Masterstrap - Loading..."] = "Masterstrap - Memuat...",
                ["Masterstrap • "] = "Masterstrap • ",
                ["Initializing launch process..."] = "Menginisialisasi proses peluncuran...",
                ["Loading FastFlag configuration..."] = "Memuat konfigurasi FastFlag...",
                ["Preparing Roblox launcher..."] = "Mempersiapkan peluncur Roblox...",
                ["Starting Roblox client..."] = "Memulai klien Roblox...",
                ["Waiting for Roblox to load..."] = "Menunggu Roblox dimuat...",
                ["Roblox detected! Applying optimizations..."] = "Roblox terdeteksi! Menerapkan optimasi...",
                ["Applying optimizations..."] = "Menerapkan optimasi...",
                ["Done!"] = "Selesai!",
                ["Roblox Launch"] = "Peluncuran Roblox",
                ["Launching Roblox"] = "Meluncurkan Roblox",
                ["Masterstrap - Loading..."] = "Masterstrap - Memuat...",
                ["Masterstrap • "] = "Masterstrap • ",
                ["Loading FastFlag configuration..."] = "Memuat konfigurasi FastFlag...",
                ["Starting Roblox..."] = "Memulai Roblox...",
                ["Waiting for Roblox to open..."] = "Menunggu Roblox terbuka...",
                ["Roblox opened. Launch complete."] = "Roblox terbuka. Peluncuran selesai.",
                ["Masterstrap deploying auto-apply..."] = "Masterstrap menerapkan auto-apply...",
                ["Waiting for game to be ready..."] = "Menunggu game siap...",
                ["Roblox closed before application."] = "Roblox ditutup sebelum injeksi.",
                ["Retrying application ({0}/{1})..."] = "Mencoba injeksi lagi ({0}/{1})...",
                ["Masterstrap auto-applying..."] = "Masterstrap auto-apply...",
                ["Auto-applying..."] = "Auto-apply...",
                ["Waiting for Roblox... ({0}s)"] = "Menunggu Roblox... ({0}s)",
                ["Roblox not detected. Closing..."] = "Roblox tidak terdeteksi. Menutup...",
                ["Launch complete."] = "Peluncuran selesai.",
                ["Applied successfully. Closing..."] = "Injeksi berhasil. Menutup...",
                ["Application failed."] = "Injeksi gagal.",
                ["Roblox not detected."] = "Roblox tidak terdeteksi.",
                ["Cancelled."] = "Dibatalkan.",
                ["Error: {0}"] = "Kesalahan: {0}",
                ["Application applied. Waiting for game to apply..."] = "Injeksi diterapkan. Menunggu game menerapkan...",
                ["Applied successfully."] = "Injeksi berhasil.",
                ["Failed to launch Roblox: {0}"] = "Gagal meluncurkan Roblox: {0}",
                ["Launch Error"] = "Kesalahan Peluncuran",
                ["Launch complete! Closing launcher..."] = "Peluncuran selesai! Menutup peluncur...",
                ["Launch cancelled"] = "Peluncuran dibatalkan",
                ["Configuration saved successfully!"] = "Konfigurasi berhasil disimpan!",
                ["Configuration saved and Roblox launched!"] = "Konfigurasi disimpan dan Roblox diluncurkan!",
                ["Launch failed!"] = "Peluncuran gagal!",
                ["Apply"] = "Apply",
                ["FastFlag Editor"] = "Editor FastFlag",
                ["manage your own Fast Flags. Use with caution"] = "kelola Fast Flag Anda sendiri. Gunakan dengan hati-hati.",
                ["Allow Masterstrap to manage Fast Flags"] = "Izinkan Masterstrap mengelola Fast Flags",
                ["Turning off this option will prevent any configuration here from being applied to Roblox."] = "Menonaktifkan opsi ini akan mencegah konfigurasi di sini diterapkan ke Roblox.",
                ["Rendering and Graphics"] = "Rendering dan Grafis",
                ["Automatic"] = "Otomatis",
                ["Anti-aliasing quality (MSAA)"] = "Kualitas anti-aliasing (MSAA)",
                ["Preserve rendering quality with display scaling"] = "Pertahankan kualitas rendering dengan penskalaan tampilan",
                ["Roblox reduces your rendering quality depending on how your display is scaled in Windows."] = "Roblox mengurangi kualitas rendering sesuai penskalaan tampilan di Windows.",
                ["FRM Quality Override"] = "Ganti Kualitas FRM",
                ["Choose the FRM quality that Roblox should use."] = "Pilih kualitas FRM yang akan digunakan Roblox.",
                ["Rendering mode"] = "Mode rendering",
                ["Texture quality"] = "Kualitas tekstur",
                ["Set as Read-Only"] = "Atur sebagai Hanya-Baca",
                ["Prevent Roblox from overriding global settings."] = "Cegah Roblox menimpa pengaturan global.",
                ["Presets"] = "Preset",
                ["Graphics Quality"] = "Kualitas Grafis",
                ["Graphic advanced"] = "Grafik tingkat lanjut",
                ["Set the graphics quality of your game"] = "Atur kualitas grafis permainan Anda",
                ["Max Quality Enabled"] = "Kualitas Maksimal Diaktifkan",
                ["Enables maximum graphics quality mode for enhanced visual effects and rendering detail."] = "Aktifkan mode kualitas grafis maksimal untuk efek visual dan detail rendering yang lebih baik.",
                ["Graphics Quality Level"] = "Tingkat Kualitas Grafis",
                ["Adjusts the in-game graphics quality level from low to maximum."] = "Sesuaikan tingkat kualitas grafis dalam game dari rendah hingga maksimal.",
                ["Framerate Limit"] = "Batas Framerate",
                ["Unlock framerate limit for Roblox. Going above 240 FPS is not recommended."] = "Buka batas framerate untuk Roblox. Tidak disarankan di atas 240 FPS.",
                ["User Interface and Layout"] = "Antarmuka dan Tata Letak",
                ["Transparency"] = "Transparansi",
                ["Custom transparency for UI elements."] = "Transparansi kustom untuk elemen UI.",
                ["Reduced Motion"] = "Kurangi Gerakan",
                ["Removes the animation on the escape menu."] = "Hapus animasi pada menu escape.",
                ["Font Size"] = "Ukuran Font",
                ["Choose how large the font should appear."] = "Pilih seberapa besar font ditampilkan.",
                ["Default"] = "Default",
                ["Other"] = "Lainnya",
                ["Mouse Sensitivity"] = "Sensitivitas Mouse",
                ["Change how fast the camera will move in-game."] = "Ubah kecepatan pergerakan kamera dalam game.",
                ["VR Enabled"] = "VR Diaktifkan",
                ["Player Name Visibility"] = "Visibilitas Nama Pemain",
                ["Hide name tags above other players for a cleaner screen experience."] = "Sembunyikan nama di atas pemain lain untuk tampilan yang lebih bersih.",
                ["FAQ and Guide"] = "FAQ dan Panduan",
                ["📖 FAQ and Guide"] = "📖 FAQ dan Panduan",
                ["How to Use Masterstrap"] = "Cara Menggunakan Masterstrap",
                ["❔ How to Use Masterstrap"] = "❔ Cara Menggunakan Masterstrap",
                ["1. Load FFlags JSON file"] = "1. Muat file FFlags JSON",
                ["2. Load FFlag Addresses (optional)"] = "2. Muat Alamat FFlag (opsional)",
                ["3. Make sure Roblox is running"] = "3. Pastikan Roblox sedang berjalan",
                ["4. Click APPLY button to apply FFlags"] = "4. Klik tombol APPLY untuk menyuntikkan FFlags",
                ["5. Check Activity Log for results"] = "5. Periksa Log Aktivitas untuk hasil",
                ["Apply"] = "Terapkan",
                ["How to Edit FFlags"] = "Cara Mengedit FFlags",
                ["✏️ How to Edit FFlags"] = "✏️ Cara Mengedit FFlags",
                ["• Go to Edit tab to modify loaded FFlags"] = "• Buka tab Edit untuk mengubah FFlags yang dimuat",
                ["• Click Add to create new FFlag entry"] = "• Klik Tambah untuk membuat entri FFlag baru",
                ["• Click Delete to remove selected FFlag"] = "• Klik Hapus untuk menghapus FFlag yang dipilih",
                ["• Use Search to find specific FFlags"] = "• Gunakan Cari untuk menemukan FFlags tertentu",
                ["• Click Export to save modified FFlags"] = "• Klik Ekspor untuk menyimpan FFlags yang diubah",
                ["Troubleshooting"] = "Pemecahan Masalah",
                ["🔧 Troubleshooting"] = "🔧 Pemecahan Masalah",
                ["Roblox not found?"] = "Roblox tidak ditemukan?",
                ["⚠️ Roblox not found?"] = "⚠️ Roblox tidak ditemukan?",
                ["Make sure Roblox is running before applying"] = "Pastikan Roblox berjalan sebelum menyuntikkan",
                ["Application failed?"] = "Apply gagal?",
                ["⚠️ Application failed?"] = "⚠️ Apply gagal?",
                ["Please ensure that your Roblox version matches the version that Masterstrap has requested"] = "Pastikan versi Roblox Anda cocok dengan versi yang diminta Masterstrap",
                ["FFlags not loading?"] = "FFlags tidak dimuat?",
                ["⚠️ FFlags not loading?"] = "⚠️ FFlags tidak dimuat?",
                ["Verify JSON file format is correct and valid"] = "Periksa format file JSON benar dan valid",
                ["Game crash after applying?"] = "Game crash setelah apply?",
                ["⚠️ Game crash after applying?"] = "⚠️ Game crash setelah apply?",
                ["Reason: FFlag has targetfps set too high, causing device overload and crash. Please click 'Edit FFlag' and change 'targetfps' value to 300-400"] = "Penyebab: FFlag memiliki targetfps terlalu tinggi, menyebabkan overload dan crash. Klik 'Edit FFlag' dan ubah nilai 'targetfps' menjadi 300-400",
                ["Tips and Tricks"] = "Tips dan Trik",
                ["💡 Tips and Tricks"] = "💡 Tips dan Trik",
                ["• Keep your FFlag JSON file backed up"] = "• Cadangkan file FFlag JSON Anda",
                ["• Export frequently to save your changes"] = "• Ekspor secara rutin untuk menyimpan perubahan",
                ["• Use Search feature to quickly find FFlags"] = "• Gunakan fitur Cari untuk menemukan FFlags dengan cepat",
                ["• Check Activity Log for application status"] = "• Periksa Log Aktivitas untuk status apply",
                ["Home"] = "Beranda",
                ["Global"] = "Global",
                ["Games"] = "Permainan",
                ["Settings"] = "Pengaturan",
                ["FAQ"] = "FAQ",
                ["⚡ FFlags"] = "⚡ apply",
                ["🌐 Global"] = "🌐 Global",
                ["🎮 Game FFlags"] = "🎮 FFlags Permainan",
                ["⚙️ Settings"] = "⚙️ Pengaturan",
                ["❓ FAQ"] = "❓ FAQ",
                ["Open the editor to view and change flags, use presets, and choose whether Masterstrap may apply them when you launch."] = "Buka editor untuk melihat dan mengubah flag, memakai preset, dan memilih apakah Masterstrap menerapkannya saat Anda meluncurkan.",
                ["Adjust Roblox wide settings such as read only mode, rendering, and framerate limits."] = "Sesuaikan pengaturan menyeluruh Roblox seperti mode hanya-baca, rendering, dan batas framerate.",
                ["Edit the flag table, filter by category, search, then use Back and Save on the FastFlags page when you are done."] = "Edit tabel flag, filter menurut kategori, cari, lalu gunakan Kembali dan Simpan di halaman FastFlags setelah selesai.",
                ["Choose language, visual theme, and startup behavior for the app."] = "Pilih bahasa, tema tampilan, dan perilaku saat startup aplikasi.",
                ["Load a curated flag set for a game into your list, then adjust or save as usual."] = "Muat set flag pilihan untuk sebuah game ke daftar Anda, lalu sesuaikan atau simpan seperti biasa.",
                ["Credits, FAQ, and short guides for using Masterstrap."] = "Kredit, FAQ, dan panduan singkat penggunaan Masterstrap.",
                ["Indonesian"] = "Bahasa Indonesia",
                ["English"] = "English",
                ["Filipino"] = "Filipino"
            }), AboutTabUiTranslations.EnToId),
            MergeUiOverlays(MergeUiOverlays(BuildFlagUiTranslations.EnToId, LaunchProgressUiTranslations.EnToId), DialogsUiTranslations.EnToId));

        private static readonly Dictionary<string, string> EnToPt = OverlayAboutTab(
            OverlayAboutTab(
            MergeTranslations(
            LoadMdTranslations("Masterstrap.Resources.Portuguese.md"),
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Settings and Options"] = "Configurações e Opções",
                ["Select Game FFlags Preset"] = "Selecionar predefinição de FFlags do jogo",
                ["🎮 Select Game FFlags Preset"] = "🎮 Selecionar predefinição de FFlags do jogo",
                ["Update Soon..."] = "Em breve...",
                ["Information System"] = "Sistema de Informações",
                ["INFORMATION SYSTEM"] = "SISTEMA DE INFORMAÇÕES",
                ["FFlags:"] = "FFlags:",
                ["Count:"] = "Contagem:",
                ["Roblox Version:"] = "Versão do Roblox:",
                ["Software Version:"] = "Versão do Software:",
                ["Last update:"] = "Última atualização:",
                ["0 entries"] = "0 entradas",
                ["Not loaded"] = "Não carregado",
                ["Fast Mode"] = "Modo Rápido",
                ["Loaded"] = "Carregado",
                ["Made By ©Dank1ngs"] = "feito por ©Dank1ngs",
                ["Idle"] = "Inativo",
                ["Join"] = "Entrar",
                ["Join Discord"] = "Entrar no Discord",
                ["Language Settings"] = "Configurações de Idioma",
                ["Select your preferred display language for the application interface."] = "Selecione o idioma de exibição preferido para a interface do aplicativo.",
                ["Vietnamese"] = "Vietnamita",
                ["Desktop Shortcut"] = "Atalho na Área de Trabalho",
                ["Create a shortcut on your Desktop for quick access to Masterstrap (recommended)"] = "Criar um atalho na Área de Trabalho para acesso rápido ao Masterstrap (recomendado)",
                ["Create a shortcut on your Desktop for quick access to Masterstrap"] = "Criar um atalho na Área de Trabalho para acesso rápido ao Masterstrap (recomendado)",
                ["General Settings"] = "Configurações Gerais",
                ["📋 General Settings"] = "Configurações Gerais",
                ["Auto-load FFlags on startup (recommended)"] = "Carregar FFlags automaticamente ao iniciar (recomendado)",
                ["Auto-load FFlags on startup"] = "Carregar FFlags automaticamente ao iniciar",
                ["Auto-load Cache on startup (recommended)"] = "Carregar Cache automaticamente ao iniciar (recomendado)",
                ["Auto-load Cache on startup"] = "Carregar Cache automaticamente ao iniciar",
                ["Auto-apply when Roblox is detected (recommended)"] = "Aplicar automaticamente quando o Roblox for detectado (recomendado)",
                ["Auto-check for updates on startup (recommended)"] = "Verificar atualizações automaticamente ao iniciar (recomendado)",
                ["Minimize to system tray"] = "Minimizar para a bandeja do sistema",
                ["Optimizer"] = "Otimizador",
                ["⚡ Optimizer"] = "Otimizador",
                ["Auto-cleanup temp files (recommended)"] = "Limpar arquivos temporários automaticamente (recomendado)",
                ["Auto-cleanup temp files"] = "Limpar arquivos temporários automaticamente",
                ["Memory optimization (recommended)"] = "Otimização de memória (recomendado)",
                ["Memory optimization"] = "Otimização de memória",
                [" (recommended)"] = " (recomendado)",
                ["Auto-apply when Roblox is detected"] = "Aplicar automaticamente quando o Roblox for detectado",
                ["Auto-check for updates on startup"] = "Verificar atualizações ao iniciar",
                ["Save and Launch"] = "Salvar e Iniciar",
                ["Save"] = "Salvar",
                ["Close"] = "Fechar",
                ["Load FFlags JSON"] = "Carregar FFlags JSON",
                ["Load FFlags Addresses"] = "Carregar Endereços de FFlags",
                ["Load FFlag Addresses"] = "Carregar Endereços de FFlag",
                ["Activity log"] = "Log de Atividade",
                ["Activity Log"] = "Log de Atividade",
                ["Clear Log"] = "Limpar Log",
                ["0 entries"] = "0 entradas",
                ["System initialized"] = "Sistema inicializado",
                ["Ready to load FFlags"] = "Pronto para carregar FFlags",
                ["Not set"] = "Não definido",
                ["Saved FFlags:"] = "FFlags salvos:",
                ["Enabled"] = "Ativado",
                ["Disabled"] = "Desativado",
                ["Auto-load FFlags:"] = "Carregar automaticamente FFlags:",
                ["Auto-load Addresses:"] = "Carregar automaticamente Endereços:",
                ["Not detected"] = "Não detectado",
                ["Roblox Version:"] = "Versão do Roblox:",
                ["Unknown"] = "Desconhecido",
                ["Software Version:"] = "Versão do Software:",
                ["Version Compatibility:"] = "Compatibilidade de Versão:",
                ["MATCH"] = "CORRESPONDE",
                ["MISMATCH"] = "INCOMPATÍVEL",
                ["UNKNOWN"] = "DESCONHECIDO",
                ["Application successful ({0} FFlags)"] = "Injeção bem-sucedida ({0} FFlags)",
                ["Application failed ({0} errors)"] = "Injeção falhou ({0} erros)",
                ["now"] = "agora",
                ["s ago"] = " s atrás",
                ["m ago"] = " min atrás",
                ["Success"] = "Sucesso",
                ["Failed"] = "Falhou",
                ["Pending"] = "Pendente",
                ["Mixed"] = "Misto",
                ["Status"] = "Status",
                ["Session"] = "Sessão",
                ["Last"] = "Último",
                ["Actions"] = "Ações",
                ["Activity log cleared"] = "Log de atividade limpo",
                ["JSON file not found"] = "Arquivo JSON não encontrado",
                ["JSON Content Preview:"] = "Visualização do conteúdo JSON:",
                ["... and {0} more entries"] = "... e mais {0} entradas",
                ["Total entries in JSON: {0}"] = "Total de entradas no JSON: {0}",
                ["Invalid JSON format"] = "Formato JSON inválido",
                ["Error parsing JSON"] = "Erro ao analisar JSON",
                [" Loading FFlag addresses..."] = " Carregando endereços de FFlag...",
                [" Auto-loading FFlag addresses..."] = " Carregando automaticamente endereços de FFlag...",
                ["Add"] = "Adicionar",
                ["Delete"] = "Excluir",
                ["Clear All"] = "Limpar Tudo",
                ["Export"] = "Exportar",
                ["All"] = "Tudo",
                ["Graphics"] = "Gráficos",
                ["Internet"] = "Internet",
                ["Physics"] = "Física",
                ["Audio"] = "Áudio",
                ["Back"] = "Voltar",
                ["← Back"] = "← Voltar",
                ["Search"] = "Pesquisar",
                ["Filter:"] = "Filtro:",
                ["📁 Load FFlags JSON"] = "📁 Carregar FFlags JSON",
                ["📄 Load FFlag Addresses"] = "📄 Carregar Endereços de FFlag",
                ["Add New FFlag"] = "Adicionar Novo FFlag",
                ["Add New FFlags"] = "Adicionar Novos FFlags",
                ["Add or batch import new flags to your library"] = "Adicione ou importe em lote novos flags à sua biblioteca",
                ["Flag Editor"] = "Editor de Flags",
                ["FLAG EDITOR"] = "EDITOR DE FLAGS",
                ["Enter flags manually or load from JSON file"] = "Digite os flags manualmente ou carregue de um arquivo JSON",
                ["FORMAT: name: value"] = "FORMATO: nome: valor",
                ["Each line = 1 FFlags"] = "Cada linha = 1 FFlag",
                ["(Each line = 1 FFlag. Example: MyFlag: true)"] = "(Cada linha = 1 FFlag. Exemplo: MyFlag: true)",
                ["Ready to add flags"] = "Pronto para adicionar flags",
                ["Configuration saved successfully!"] = "Configuração salva com sucesso!",
                ["Configuration saved successfully"] = "Configuração salva com sucesso",
                ["CompleteAdsenseDialog created successfully"] = "CompleteAdsenseDialog criado com sucesso",
                ["Complete the adsense to continue"] = "Conclua o adsense para continuar",
                ["Support"] = "Suporte",
                ["Please look at and click on ads so this software project can continue for free"] = "Por favor, veja e clique nos anúncios para que este projeto de software possa continuar gratuitamente",
                ["⏭ Skip ad 3:00"] = "⏭ Pular anúncio 3:00",
                ["Skip ad"] = "Pular anúncio",
                ["How to skip ad? "] = "Como pular o anúncio? ",
                ["Click here"] = "Clique aqui",
                ["✓ Continue"] = "✓ Continuar",
                ["✓ Ad Complete"] = "✓ Anúncio concluído",
                ["Ad Complete"] = "Anúncio concluído",
                ["Please wait for the countdown to finish"] = "Aguarde a contagem regressiva terminar",
                ["Please wait"] = "Por favor, aguarde",
                ["Please click 'Continue' button to proceed"] = "Clique no botão 'Continuar' para prosseguir",
                ["Ad Completed"] = "Anúncio concluído",
                ["Could not open support link"] = "Não foi possível abrir o link de suporte",
                ["Could not open help link"] = "Não foi possível abrir o link de ajuda",
                ["Could not open Discord link"] = "Não foi possível abrir o link do Discord",
                ["Adsense dialog marked as OPEN"] = "Diálogo Adsense marcado como ABERTO",
                ["Adsense dialog marked as CLOSED"] = "Diálogo Adsense marcado como FECHADO",
                ["FFlags applied successfully!"] = "FFlags injetadas com sucesso!",
                ["⚡ APPLY"] = "⚡ Aplicar",
                ["↩️ UNAPPLY"] = "↩️ Desinjetar",
                ["APPLY"] = "Aplicar",
                ["UNAPPLY"] = "Desinjetar",
                ["Cancel"] = "Cancelar",
                ["Don't Save"] = "Não Salvar",
                ["Unsaved Changes"] = "Alterações não salvas",
                ["You have unsaved changes. Do you want to save before exiting?"] = "Você tem alterações não salvas. Deseja salvar antes de sair?",
                ["Ready"] = "Pronto",
                ["Initializing..."] = "Inicializando...",
                ["Loading..."] = "Carregando...",
                ["Loaded"] = "Carregado",
                ["Applying..."] = "Injetando...",
                ["Applied"] = "Injetado",
                ["Opening Roblox..."] = "Abrindo Roblox...",
                ["Roblox Launched"] = "Roblox Iniciado",
                ["Roblox Not Found"] = "Roblox não encontrado",
                ["Version Mismatch"] = "Incompatibilidade de versão",
                ["Service Error"] = "Erro do serviço",
                ["Fatal Error"] = "Erro fatal",
                ["Error"] = "Erro",
                ["Failed"] = "Falhou",
                ["Auto-loading..."] = "Carregando automaticamente...",
                ["Auto-Applying..."] = "Injetando automaticamente...",
                ["Auto-Applied"] = "Injetado automaticamente",
                ["Flag management disabled"] = "Gerenciamento de flags desativado",
                ["Not Implemented"] = "Não implementado",
                ["LaunchProgressWindow"] = "Janela de progresso de inicialização",
                ["Launching Roblox"] = "Iniciando Roblox",
                ["Masterstrap - Loading..."] = "Masterstrap - Carregando...",
                ["Masterstrap • "] = "Masterstrap • ",
                ["Initializing launch process..."] = "Inicializando processo de inicialização...",
                ["Loading FastFlag configuration..."] = "Carregando configuração do FastFlag...",
                ["Preparing Roblox launcher..."] = "Preparando o iniciador do Roblox...",
                ["Starting Roblox client..."] = "Iniciando o cliente Roblox...",
                ["Waiting for Roblox to load..."] = "Aguardando o Roblox carregar...",
                ["Roblox detected! Applying optimizations..."] = "Roblox detectado! Aplicando otimizações...",
                ["Applying optimizations..."] = "Aplicando otimizações...",
                ["Done!"] = "Concluído!",
                ["Roblox Launch"] = "Inicialização do Roblox",
                ["Launching Roblox"] = "Iniciando Roblox",
                ["Masterstrap - Loading..."] = "Masterstrap - Carregando...",
                ["Masterstrap • "] = "Masterstrap • ",
                ["Loading FastFlag configuration..."] = "Carregando configuração FastFlag...",
                ["Starting Roblox..."] = "Iniciando Roblox...",
                ["Waiting for Roblox to open..."] = "Aguardando Roblox abrir...",
                ["Roblox opened. Launch complete."] = "Roblox aberto. Inicialização concluída.",
                ["Masterstrap deploying auto-apply..."] = "Masterstrap implantando auto-apply...",
                ["Waiting for game to be ready..."] = "Aguardando o jogo ficar pronto...",
                ["Roblox closed before application."] = "Roblox fechado antes da injeção.",
                ["Retrying application ({0}/{1})..."] = "Tentando injeção novamente ({0}/{1})...",
                ["Masterstrap auto-applying..."] = "Masterstrap auto-applyando...",
                ["Auto-applying..."] = "Auto-applyando...",
                ["Waiting for Roblox... ({0}s)"] = "Aguardando Roblox... ({0}s)",
                ["Roblox not detected. Closing..."] = "Roblox não detectado. Fechando...",
                ["Launch complete."] = "Inicialização concluída.",
                ["Applied successfully. Closing..."] = "Injetado com sucesso. Fechando...",
                ["Application failed."] = "Injeção falhou.",
                ["Roblox not detected."] = "Roblox não detectado.",
                ["Cancelled."] = "Cancelado.",
                ["Error: {0}"] = "Erro: {0}",
                ["Application applied. Waiting for game to apply..."] = "Injeção aplicada. Aguardando o jogo aplicar...",
                ["Applied successfully."] = "Injetado com sucesso.",
                ["Failed to launch Roblox: {0}"] = "Falha ao iniciar Roblox: {0}",
                ["Launch Error"] = "Erro de Inicialização",
                ["Launch complete! Closing launcher..."] = "Inicialização concluída! Fechando o iniciador...",
                ["Launch cancelled"] = "Inicialização cancelada",
                ["Configuration saved successfully!"] = "Configuração salva com sucesso!",
                ["Configuration saved and Roblox launched!"] = "Configuração salva e Roblox iniciado!",
                ["Launch failed!"] = "Falha na inicialização!",
                ["Apply"] = "Aplicar",
                ["FastFlag Editor"] = "Editor de FastFlag",
                ["manage your own Fast Flags. Use with caution"] = "Gerencie suas próprias Fast Flags. Use com cautela.",
                ["Allow Masterstrap to manage Fast Flags"] = "Permitir que o Masterstrap gerencie as Fast Flags",
                ["Turning off this option will prevent any configuration here from being applied to Roblox."] = "Desativar esta opção impedirá que qualquer configuração aqui seja aplicada ao Roblox.",
                ["Rendering and Graphics"] = "Renderização e Gráficos",
                ["Automatic"] = "Automático",
                ["Anti-aliasing quality (MSAA)"] = "Qualidade de anti-aliasing (MSAA)",
                ["Preserve rendering quality with display scaling"] = "Preservar a qualidade de renderização com a escala de exibição",
                ["Roblox reduces your rendering quality depending on how your display is scaled in Windows."] = "O Roblox reduz a qualidade de renderização dependendo de como a tela está escalada no Windows.",
                ["FRM Quality Override"] = "Substituição da Qualidade de FRM",
                ["Choose the FRM quality that Roblox should use."] = "Escolha a qualidade de FRM que o Roblox deve usar.",
                ["Rendering mode"] = "Modo de renderização",
                ["Texture quality"] = "Qualidade da textura",
                ["Set as Read-Only"] = "Definir como Somente Leitura",
                ["Prevent Roblox from overriding global settings."] = "Impedir que o Roblox substitua as configurações globais.",
                ["Presets"] = "Predefinições",
                ["Graphics Quality"] = "Qualidade Gráfica",
                ["Graphic advanced"] = "Gráficos avançados",
                ["Set the graphics quality of your game"] = "Definir a qualidade gráfica do seu jogo",
                ["Max Quality Enabled"] = "Qualidade Máxima Ativada",
                ["Enables maximum graphics quality mode for enhanced visual effects and rendering detail."] = "Ativa o modo de qualidade gráfica máxima para melhorar os efeitos visuais e os detalhes de renderização.",
                ["Graphics Quality Level"] = "Nível de Qualidade Gráfica",
                ["Adjusts the in-game graphics quality level from low to maximum."] = "Ajusta o nível de qualidade gráfica no jogo de baixo até o máximo.",
                ["Framerate Limit"] = "Limite de Taxa de Quadros",
                ["Unlock framerate limit for Roblox. Going above 240 FPS is not recommended."] = "Desbloqueia o limite de taxa de quadros do Roblox. Não é recomendado ultrapassar 240 FPS.",
                ["User Interface and Layout"] = "Interface do Usuário e Layout",
                ["Transparency"] = "Transparência",
                ["Custom transparency for UI elements."] = "Transparência personalizada para elementos da interface.",
                ["Reduced Motion"] = "Movimento Reduzido",
                ["Removes the animation on the escape menu."] = "Remove a animação do menu Escape.",
                ["Font Size"] = "Tamanho da Fonte",
                ["Choose how large the font should appear."] = "Escolha o tamanho em que a fonte deve aparecer.",
                ["Default"] = "Padrão",
                ["Other"] = "Outros",
                ["Mouse Sensitivity"] = "Sensibilidade do Mouse",
                ["Change how fast the camera will move in-game."] = "Altere a velocidade com que a câmera se move no jogo.",
                ["VR Enabled"] = "VR Ativado",
                ["Player Name Visibility"] = "Visibilidade do Nome do Jogador",
                ["Hide name tags above other players for a cleaner screen experience."] = "Ocultar os nomes acima de outros jogadores para uma experiência de tela mais limpa.",
                ["FAQ and Guide"] = "FAQ e Guia",
                ["📖 FAQ and Guide"] = "📖 FAQ e Guia",
                ["How to Use Masterstrap"] = "Como Usar o Masterstrap",
                ["❔ How to Use Masterstrap"] = "❔ Como Usar o Masterstrap",
                ["1. Load FFlags JSON file"] = "1. Carregar arquivo JSON de FFlags",
                ["2. Load FFlag Addresses (optional)"] = "2. Carregar Endereços de FFlag (opcional)",
                ["3. Make sure Roblox is running"] = "3. Certifique-se de que o Roblox esteja em execução",
                ["4. Click APPLY button to apply FFlags"] = "4. Clique no botão APPLY para injetar as FFlags",
                ["5. Check Activity Log for results"] = "5. Verifique o Activity Log para ver os resultados",
                ["Apply"] = "Aplicar",
                ["How to Edit FFlags"] = "Como Editar FFlags",
                ["✏️ How to Edit FFlags"] = "✏️ Como Editar FFlags",
                ["• Go to Edit tab to modify loaded FFlags"] = "• Vá até a aba Edit para modificar as FFlags carregadas",
                ["• Click Add to create new FFlag entry"] = "• Clique em Add para criar uma nova entrada de FFlag",
                ["• Click Delete to remove selected FFlag"] = "• Clique em Delete para remover a FFlag selecionada",
                ["• Use Search to find specific FFlags"] = "• Use Search para encontrar FFlags específicas",
                ["• Click Export to save modified FFlags"] = "• Clique em Export para salvar as FFlags modificadas",
                ["Troubleshooting"] = "Solução de Problemas",
                ["🔧 Troubleshooting"] = "🔧 Solução de Problemas",
                ["Roblox not found?"] = "Roblox não encontrado?",
                ["⚠️ Roblox not found?"] = "⚠️ Roblox não encontrado?",
                ["Make sure Roblox is running before applying"] = "Certifique-se de que o Roblox esteja em execução antes de injetar",
                ["Application failed?"] = "Falha na injeção?",
                ["⚠️ Application failed?"] = "⚠️ Falha na injeção?",
                ["Please ensure that your Roblox version matches the version that Masterstrap has requested"] = "Certifique-se de que sua versão do Roblox corresponde à versão exigida pelo Masterstrap",
                ["FFlags not loading?"] = "FFlags não estão carregando?",
                ["⚠️ FFlags not loading?"] = "⚠️ FFlags não estão carregando?",
                ["Verify JSON file format is correct and valid"] = "Verifique se o formato do arquivo JSON está correto e válido",
                ["Game crash after applying?"] = "O jogo travou após a injeção?",
                ["⚠️ Game crash after applying?"] = "⚠️ O jogo travou após a injeção?",
                ["Reason: FFlag has targetfps set too high, causing device overload and crash. Please click 'Edit FFlag' and change 'targetfps' value to 300-400"] = "Motivo: o FFlag tem targetfps muito alto, causando sobrecarga e travamento. Clique em 'Editar FFlag' e altere o valor de 'targetfps' para 300-400",
                ["Tips and Tricks"] = "Dicas e Truques",
                ["💡 Tips and Tricks"] = "💡 Dicas e Truques",
                ["• Keep your FFlag JSON file backed up"] = "• Mantenha um backup do seu arquivo JSON de FFlag",
                ["• Export frequently to save your changes"] = "• Exporte com frequência para salvar suas alterações",
                ["• Use Search feature to quickly find FFlags"] = "• Use o recurso Search para encontrar FFlags rapidamente",
                ["• Check Activity Log for application status"] = "• Verifique o Activity Log para ver o status da injeção",
                ["Home"] = "Início",
                ["Global"] = "Global",
                ["Games"] = "Jogos",
                ["Settings"] = "Configurações",
                ["FAQ"] = "FAQ",
                ["⚡ FFlags"] = "⚡ Aplicar",
                ["🌐 Global"] = "🌐 Global",
                ["🎮 Game FFlags"] = "🎮 FFlags do Jogo",
                ["⚙️ Settings"] = "⚙️ Configurações",
                ["❓ FAQ"] = "❓ FAQ",
                ["Open the editor to view and change flags, use presets, and choose whether Masterstrap may apply them when you launch."] = "Abra o editor para ver e alterar flags, usar predefinições e escolher se o Masterstrap pode aplicá-las ao iniciar.",
                ["Adjust Roblox wide settings such as read only mode, rendering, and framerate limits."] = "Ajuste configurações globais do Roblox, como modo somente leitura, renderização e limites de taxa de quadros.",
                ["Edit the flag table, filter by category, search, then use Back and Save on the FastFlags page when you are done."] = "Edite a tabela de flags, filtre por categoria, pesquise e use Voltar e Salvar na página FastFlags quando terminar.",
                ["Choose language, visual theme, and startup behavior for the app."] = "Escolha o idioma, o tema visual e o comportamento de inicialização do aplicativo.",
                ["Load a curated flag set for a game into your list, then adjust or save as usual."] = "Carregue um conjunto de flags selecionado para um jogo na sua lista e ajuste ou salve como de costume.",
                ["Credits, FAQ, and short guides for using Masterstrap."] = "Créditos, FAQ e guias curtos para usar o Masterstrap.",
                ["Portuguese"] = "Português",
                ["Indonesian"] = "Indonésio",
                ["English"] = "English",
                ["Filipino"] = "Filipino"
            }), AboutTabUiTranslations.EnToPt),
            MergeUiOverlays(MergeUiOverlays(BuildFlagUiTranslations.EnToPt, LaunchProgressUiTranslations.EnToPt), DialogsUiTranslations.EnToPt));

        private static readonly Dictionary<string, string> EnToMal = OverlayAboutTab(
            OverlayAboutTab(
            MergeTranslations(
            LoadMdTranslations("Masterstrap.Resources.Malay.md"),
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Settings and Options"] = "Tetapan dan Pilihan",
                ["Select Game FFlags Preset"] = "Pilih Preset FFlags Permainan",
                ["🎮 Select Game FFlags Preset"] = "🎮 Pilih Preset FFlags Permainan",
                ["Update Soon..."] = "Akan Datang...",
                ["Information System"] = "Sistem Maklumat",
                ["INFORMATION SYSTEM"] = "SISTEM MAKLUMAT",
                ["FFlags:"] = "FFlags:",
                ["Count:"] = "Bilangan:",
                ["Roblox Version:"] = "Versi Roblox:",
                ["Software Version:"] = "Versi Perisian:",
                ["Last update:"] = "Kemas kini terakhir:",
                ["0 entries"] = "0 entri",
                ["Not loaded"] = "Belum dimuatkan",
                ["Fast Mode"] = "Mod Pantas",
                ["Loaded"] = "Dimuatkan",
                ["Made By ©Dank1ngs"] = "dibuat oleh ©Dank1ngs",
                ["Idle"] = "Rehat",
                ["Join"] = "Sertai",
                ["Join Discord"] = "Sertai Discord",
                ["Language Settings"] = "Tetapan Bahasa",
                ["Select your preferred display language for the application interface."] = "Pilih bahasa paparan pilihan anda untuk antara muka aplikasi.",
                ["Vietnamese"] = "Vietnam",
                ["Desktop Shortcut"] = "Pintasan Desktop",
                ["Create a shortcut on your Desktop for quick access to Masterstrap (recommended)"] = "Cipta pintasan di Desktop untuk akses pantas ke Masterstrap (disyorkan)",
                ["Create a shortcut on your Desktop for quick access to Masterstrap"] = "Cipta pintasan di Desktop untuk akses pantas ke Masterstrap (disyorkan)",
                ["General Settings"] = "Tetapan Umum",
                ["📋 General Settings"] = "Tetapan Umum",
                ["Auto-load FFlags on startup (recommended)"] = "Muat FFlags secara automatik semasa permulaan (disyorkan)",
                ["Auto-load FFlags on startup"] = "Muat FFlags secara automatik semasa permulaan",
                ["Auto-load Cache on startup (recommended)"] = "Muat Cache secara automatik semasa permulaan (disyorkan)",
                ["Auto-load Cache on startup"] = "Muat Cache secara automatik semasa permulaan",
                ["Auto-apply when Roblox is detected (recommended)"] = "Injeksi secara automatik apabila Roblox dikesan (disyorkan)",
                ["Auto-check for updates on startup (recommended)"] = "Semak kemas kini secara automatik semasa permulaan (disyorkan)",
                ["Minimize to system tray"] = "Minimumkan ke dulang sistem",
                ["Optimizer"] = "Pengoptimum",
                ["⚡ Optimizer"] = "Pengoptimum",
                ["Auto-cleanup temp files (recommended)"] = "Bersihkan fail sementara secara automatik (disyorkan)",
                ["Auto-cleanup temp files"] = "Bersihkan fail sementara secara automatik",
                ["Memory optimization (recommended)"] = "Pengoptimuman memori (disyorkan)",
                ["Memory optimization"] = "Pengoptimuman memori",
                [" (recommended)"] = " (disyorkan)",
                ["Auto-apply when Roblox is detected"] = "Injeksi secara automatik apabila Roblox dikesan",
                ["Auto-check for updates on startup"] = "Semak kemas kini semasa permulaan",
                ["Save and Launch"] = "Simpan dan Lancarkan",
                ["Save"] = "Simpan",
                ["Close"] = "Tutup",
                ["Load FFlags JSON"] = "Muat FFlags JSON",
                ["Load FFlags Addresses"] = "Muat Alamat FFlags",
                ["Load FFlag Addresses"] = "Muat Alamat FFlag",
                ["Activity log"] = "Log Aktiviti",
                ["Activity Log"] = "Log Aktiviti",
                ["Clear Log"] = "Kosongkan Log",
                ["0 entries"] = "0 entri",
                ["System initialized"] = "Sistem diinisialisasi",
                ["Ready to load FFlags"] = "Sedia memuat FFlags",
                ["Not set"] = "Belum ditetapkan",
                ["Saved FFlags:"] = "FFlags disimpan:",
                ["Enabled"] = "Didayakan",
                ["Disabled"] = "Dilumpuhkan",
                ["Auto-load FFlags:"] = "Muat auto FFlags:",
                ["Auto-load Addresses:"] = "Muat auto Alamat:",
                ["Not detected"] = "Tidak dikesan",
                ["Roblox Version:"] = "Versi Roblox:",
                ["Unknown"] = "Tidak diketahui",
                ["Software Version:"] = "Versi Perisian:",
                ["Version Compatibility:"] = "Keserasian Versi:",
                ["MATCH"] = "PADAN",
                ["MISMATCH"] = "TIDAK PADAN",
                ["UNKNOWN"] = "TIDAK DIKETAHUI",
                ["Application successful ({0} FFlags)"] = "Terapkanan berjaya ({0} FFlags)",
                ["Application failed ({0} errors)"] = "Terapkanan gagal ({0} ralat)",
                ["now"] = "sekarang",
                ["s ago"] = " saat lalu",
                ["m ago"] = " minit lalu",
                ["Success"] = "Berjaya",
                ["Failed"] = "Gagal",
                ["Pending"] = "Menunggu",
                ["Mixed"] = "Campuran",
                ["Status"] = "Status",
                ["Session"] = "Sesi",
                ["Last"] = "Terakhir",
                ["Actions"] = "Tindakan",
                ["Activity log cleared"] = "Log aktiviti dikosongkan",
                ["JSON file not found"] = "Fail JSON tidak dijumpai",
                ["JSON Content Preview:"] = "Pratonton Kandungan JSON:",
                ["... and {0} more entries"] = "... dan {0} entri lagi",
                ["Total entries in JSON: {0}"] = "Jumlah entri dalam JSON: {0}",
                ["Invalid JSON format"] = "Format JSON tidak sah",
                ["Error parsing JSON"] = "Ralat menghurai JSON",
                [" Loading FFlag addresses..."] = " Memuatkan alamat FFlag...",
                [" Auto-loading FFlag addresses..."] = " Memuatkan auto alamat FFlag...",
                ["Add"] = "Tambah",
                ["Delete"] = "Padam",
                ["Clear All"] = "Kosongkan Semua",
                ["Export"] = "Eksport",
                ["All"] = "Semua",
                ["Graphics"] = "Grafik",
                ["Internet"] = "Internet",
                ["Physics"] = "Fizik",
                ["Audio"] = "Audio",
                ["Back"] = "Kembali",
                ["← Back"] = "← Kembali",
                ["Search"] = "Cari",
                ["Filter:"] = "Tapis:",
                ["📁 Load FFlags JSON"] = "📁 Muat FFlags JSON",
                ["📄 Load FFlag Addresses"] = "📄 Muat Alamat FFlag",
                ["Add New FFlag"] = "Tambah FFlag Baharu",
                ["Add New FFlags"] = "Tambah FFlags Baharu",
                ["Add or batch import new flags to your library"] = "Tambah atau import kelompok flag baharu ke perpustakaan anda",
                ["Flag Editor"] = "Editor Flag",
                ["FLAG EDITOR"] = "EDITOR FLAG",
                ["Enter flags manually or load from JSON file"] = "Masukkan flag secara manual atau muat dari fail JSON",
                ["FORMAT: name: value"] = "FORMAT: nama: nilai",
                ["Each line = 1 FFlags"] = "Setiap baris = 1 FFlag",
                ["(Each line = 1 FFlag. Example: MyFlag: true)"] = "(Setiap baris = 1 FFlag. Contoh: MyFlag: true)",
                ["Ready to add flags"] = "Sedia untuk menambah flag",
                ["Configuration saved successfully!"] = "Konfigurasi berjaya disimpan!",
                ["Configuration saved successfully"] = "Konfigurasi berjaya disimpan",
                ["CompleteAdsenseDialog created successfully"] = "CompleteAdsenseDialog berjaya dicipta",
                ["Complete the adsense to continue"] = "Lengkapkan iklan untuk teruskan",
                ["Support"] = "Sokongan",
                ["Please look at and click on ads so this software project can continue for free"] = "Sila lihat dan klik iklan supaya projek perisian ini dapat diteruskan secara percuma",
                ["⏭ Skip ad 3:00"] = "⏭ Langkau iklan 3:00",
                ["Skip ad"] = "Langkau iklan",
                ["How to skip ad? "] = "Bagaimana untuk langkau iklan? ",
                ["Click here"] = "Klik di sini",
                ["✓ Continue"] = "✓ Teruskan",
                ["✓ Ad Complete"] = "✓ Iklan selesai",
                ["Ad Complete"] = "Iklan selesai",
                ["Please wait for the countdown to finish"] = "Sila tunggu sehingga kiraan tamat",
                ["Please wait"] = "Sila tunggu",
                ["Please click 'Continue' button to proceed"] = "Sila klik butang 'Teruskan' untuk meneruskan",
                ["Ad Completed"] = "Iklan selesai",
                ["Could not open support link"] = "Tidak dapat membuka pautan sokongan",
                ["Could not open help link"] = "Tidak dapat membuka pautan bantuan",
                ["Could not open Discord link"] = "Tidak dapat membuka pautan Discord",
                ["Adsense dialog marked as OPEN"] = "Dialog Adsense ditanda sebagai BUKA",
                ["Adsense dialog marked as CLOSED"] = "Dialog Adsense ditanda sebagai TUTUP",
                ["FFlags applied successfully!"] = "FFlags berjaya disuntik!",
                ["⚡ APPLY"] = "⚡ Terapkan",
                ["↩️ UNAPPLY"] = "↩️ Nyah-suntik",
                ["APPLY"] = "Terapkan",
                ["UNAPPLY"] = "Nyah-suntik",
                ["Cancel"] = "Batal",
                ["Don't Save"] = "Jangan Simpan",
                ["Unsaved Changes"] = "Perubahan Belum Disimpan",
                ["You have unsaved changes. Do you want to save before exiting?"] = "Anda mempunyai perubahan yang belum disimpan. Simpan sebelum keluar?",
                ["Ready"] = "Sedia",
                ["Initializing..."] = "Memulakan...",
                ["Loading..."] = "Memuatkan...",
                ["Loaded"] = "Dimuatkan",
                ["Applying..."] = "Menyuntik...",
                ["Applied"] = "Tersuntik",
                ["Opening Roblox..."] = "Membuka Roblox...",
                ["Roblox Launched"] = "Roblox Dilancarkan",
                ["Roblox Not Found"] = "Roblox Tidak Ditemui",
                ["Version Mismatch"] = "Versi Tidak Sepadan",
                ["Service Error"] = "Ralat Perkhidmatan",
                ["Fatal Error"] = "Ralat Fatal",
                ["Error"] = "Ralat",
                ["Failed"] = "Gagal",
                ["Auto-loading..."] = "Memuatkan secara automatik...",
                ["Auto-Applying..."] = "Menyuntik secara automatik...",
                ["Auto-Applied"] = "Tersuntik secara automatik",
                ["Flag management disabled"] = "Pengurusan flag dilumpuhkan",
                ["Not Implemented"] = "Belum Dilaksanakan",
                ["LaunchProgressWindow"] = "Tetingkap Kemajuan Pelancaran",
                ["Launching Roblox"] = "Melancarkan Roblox",
                ["Masterstrap - Loading..."] = "Masterstrap - Memuatkan...",
                ["Masterstrap • "] = "Masterstrap • ",
                ["Initializing launch process..."] = "Memulakan proses pelancaran...",
                ["Loading FastFlag configuration..."] = "Memuatkan konfigurasi FastFlag...",
                ["Preparing Roblox launcher..."] = "Menyediakan pelancar Roblox...",
                ["Starting Roblox client..."] = "Memulakan klien Roblox...",
                ["Waiting for Roblox to load..."] = "Menunggu Roblox dimuatkan...",
                ["Roblox detected! Applying optimizations..."] = "Roblox dikesan! Menerapkan pengoptimuman...",
                ["Applying optimizations..."] = "Menerapkan pengoptimuman...",
                ["Done!"] = "Selesai!",
                ["Roblox Launch"] = "Pelancaran Roblox",
                ["Launching Roblox"] = "Melancarkan Roblox",
                ["Masterstrap - Loading..."] = "Masterstrap - Memuatkan...",
                ["Masterstrap • "] = "Masterstrap • ",
                ["Loading FastFlag configuration..."] = "Memuatkan konfigurasi FastFlag...",
                ["Starting Roblox..."] = "Memulakan Roblox...",
                ["Waiting for Roblox to open..."] = "Menunggu Roblox dibuka...",
                ["Roblox opened. Launch complete."] = "Roblox dibuka. Pelancaran selesai.",
                ["Masterstrap deploying auto-apply..."] = "Masterstrap mengerahkan auto-apply...",
                ["Waiting for game to be ready..."] = "Menunggu permainan sedia...",
                ["Roblox closed before application."] = "Roblox ditutup sebelum suntikan.",
                ["Retrying application ({0}/{1})..."] = "Mencuba suntikan semula ({0}/{1})...",
                ["Masterstrap auto-applying..."] = "Masterstrap auto-apply...",
                ["Auto-applying..."] = "Auto-apply...",
                ["Waiting for Roblox... ({0}s)"] = "Menunggu Roblox... ({0}s)",
                ["Roblox not detected. Closing..."] = "Roblox tidak dikesan. Menutup...",
                ["Launch complete."] = "Pelancaran selesai.",
                ["Applied successfully. Closing..."] = "Terapkanan berjaya. Menutup...",
                ["Application failed."] = "Terapkanan gagal.",
                ["Roblox not detected."] = "Roblox tidak dikesan.",
                ["Cancelled."] = "Dibatalkan.",
                ["Error: {0}"] = "Ralat: {0}",
                ["Application applied. Waiting for game to apply..."] = "Terapkanan digunakan. Menunggu permainan mengaplikasikan...",
                ["Applied successfully."] = "Terapkanan berjaya.",
                ["Failed to launch Roblox: {0}"] = "Gagal melancarkan Roblox: {0}",
                ["Launch Error"] = "Ralat Pelancaran",
                ["Launch complete! Closing launcher..."] = "Pelancaran selesai! Menutup pelancar...",
                ["Launch cancelled"] = "Pelancaran dibatalkan",
                ["Configuration saved successfully!"] = "Konfigurasi berjaya disimpan!",
                ["Configuration saved and Roblox launched!"] = "Konfigurasi disimpan dan Roblox dilancarkan!",
                ["Launch failed!"] = "Pelancaran gagal!",
                ["Apply"] = "Terapkan",
                ["FastFlag Editor"] = "Editor FastFlag",
                ["manage your own Fast Flags. Use with caution"] = "Urus Fast Flag anda sendiri. Gunakan dengan berhati-hati.",
                ["Allow Masterstrap to manage Fast Flags"] = "Benarkan Masterstrap mengurus Fast Flag",
                ["Turning off this option will prevent any configuration here from being applied to Roblox."] = "Mematikan pilihan ini akan menghalang sebarang konfigurasi di sini daripada digunakan pada Roblox.",
                ["Rendering and Graphics"] = "Rendering dan Grafik",
                ["Automatic"] = "Automatik",
                ["Anti-aliasing quality (MSAA)"] = "Kualiti anti-aliasing (MSAA)",
                ["Preserve rendering quality with display scaling"] = "Kekalkan kualiti rendering apabila menggunakan penskalaan paparan",
                ["Roblox reduces your rendering quality depending on how your display is scaled in Windows."] = "Roblox mengurangkan kualiti rendering bergantung pada cara paparan anda diskalakan di Windows.",
                ["FRM Quality Override"] = "Gantikan Kualiti FRM",
                ["Choose the FRM quality that Roblox should use."] = "Pilih kualiti FRM yang patut digunakan oleh Roblox.",
                ["Rendering mode"] = "Mod rendering",
                ["Texture quality"] = "Kualiti tekstur",
                ["Set as Read-Only"] = "Tetapkan sebagai Read-Only",
                ["Prevent Roblox from overriding global settings."] = "Halang Roblox daripada menggantikan tetapan global.",
                ["Presets"] = "Preset",
                ["Graphics Quality"] = "Kualiti Grafik",
                ["Graphic advanced"] = "Grafik lanjutan",
                ["Set the graphics quality of your game"] = "Tetapkan kualiti grafik permainan anda",
                ["Max Quality Enabled"] = "Kualiti Maksimum Diaktifkan",
                ["Enables maximum graphics quality mode for enhanced visual effects and rendering detail."] = "Mengaktifkan mod kualiti grafik maksimum untuk kesan visual dan perincian rendering yang lebih baik.",
                ["Graphics Quality Level"] = "Tahap Kualiti Grafik",
                ["Adjusts the in-game graphics quality level from low to maximum."] = "Laraskan tahap kualiti grafik dalam permainan daripada rendah ke maksimum.",
                ["Framerate Limit"] = "Had Framerate",
                ["Unlock framerate limit for Roblox. Going above 240 FPS is not recommended."] = "Nyahkunci had framerate untuk Roblox. Melebihi 240 FPS tidak disyorkan.",
                ["User Interface and Layout"] = "Antara Muka Pengguna dan Susun Atur",
                ["Transparency"] = "Ketelusan",
                ["Custom transparency for UI elements."] = "Ketelusan tersuai untuk elemen UI.",
                ["Reduced Motion"] = "Kurangkan Animasi",
                ["Removes the animation on the escape menu."] = "Menghapuskan animasi pada menu escape.",
                ["Font Size"] = "Saiz Fon",
                ["Choose how large the font should appear."] = "Pilih saiz fon yang ingin dipaparkan.",
                ["Default"] = "Lalai",
                ["Other"] = "Lain-lain",
                ["Mouse Sensitivity"] = "Sensitiviti Tetikus",
                ["Change how fast the camera will move in-game."] = "Ubah seberapa pantas kamera bergerak dalam permainan.",
                ["VR Enabled"] = "VR Diaktifkan",
                ["Player Name Visibility"] = "Keterlihatan Nama Pemain",
                ["Hide name tags above other players for a cleaner screen experience."] = "Sembunyikan tag nama di atas pemain lain untuk paparan skrin yang lebih bersih.",
                ["FAQ and Guide"] = "FAQ dan Panduan",
                ["📖 FAQ and Guide"] = "📖 FAQ dan Panduan",
                ["How to Use Masterstrap"] = "Cara Menggunakan Masterstrap",
                ["❔ How to Use Masterstrap"] = "❔ Cara Menggunakan Masterstrap",
                ["1. Load FFlags JSON file"] = "1. Muat fail JSON FFlags",
                ["2. Load FFlag Addresses (optional)"] = "2. Muat Alamat FFlag (pilihan)",
                ["3. Make sure Roblox is running"] = "3. Pastikan Roblox sedang berjalan",
                ["4. Click APPLY button to apply FFlags"] = "4. Klik butang APPLY untuk melakukan injeksi FFlags",
                ["5. Check Activity Log for results"] = "5. Semak Log Aktiviti untuk melihat keputusan",
                ["Apply"] = "Guna",
                ["How to Edit FFlags"] = "Cara Edit FFlags",
                ["✏️ How to Edit FFlags"] = "✏️ Cara Edit FFlags",
                ["• Go to Edit tab to modify loaded FFlags"] = "• Buka tab Edit untuk mengubahsuai FFlags yang dimuatkan",
                ["• Click Add to create new FFlag entry"] = "• Klik Tambah untuk mencipta entri FFlag baru",
                ["• Click Delete to remove selected FFlag"] = "• Klik Padam untuk membuang FFlag yang dipilih",
                ["• Use Search to find specific FFlags"] = "• Gunakan Cari untuk mencari FFlags tertentu",
                ["• Click Export to save modified FFlags"] = "• Klik Eksport untuk menyimpan FFlags yang diubahsuai",
                ["Troubleshooting"] = "Penyelesaian Masalah",
                ["🔧 Troubleshooting"] = "🔧 Penyelesaian Masalah",
                ["Roblox not found?"] = "Roblox tidak ditemui?",
                ["⚠️ Roblox not found?"] = "⚠️ Roblox tidak ditemui?",
                ["Make sure Roblox is running before applying"] = "Pastikan Roblox sedang berjalan sebelum menyuntik",
                ["Application failed?"] = "Injeksi gagal?",
                ["⚠️ Application failed?"] = "⚠️ Injeksi gagal?",
                ["Please ensure that your Roblox version matches the version that Masterstrap has requested"] = "Pastikan versi Roblox anda sepadan dengan versi yang diminta Masterstrap",
                ["FFlags not loading?"] = "FFlags tidak dimuatkan?",
                ["⚠️ FFlags not loading?"] = "⚠️ FFlags tidak dimuatkan?",
                ["Verify JSON file format is correct and valid"] = "Sahkan format fail JSON betul dan sah",
                ["Game crash after applying?"] = "Permainan crash selepas injeksi?",
                ["⚠️ Game crash after applying?"] = "⚠️ Permainan crash selepas injeksi?",
                ["Reason: FFlag has targetfps set too high, causing device overload and crash. Please click 'Edit FFlag' and change 'targetfps' value to 300-400"] = "Sebab: FFlag mempunyai targetfps terlalu tinggi, menyebabkan peranti berlebihan dan crash. Klik 'Edit FFlag' dan tukar nilai 'targetfps' kepada 300-400",
                ["Tips and Tricks"] = "Tip dan Helah",
                ["💡 Tips and Tricks"] = "💡 Tip dan Helah",
                ["• Keep your FFlag JSON file backed up"] = "• Simpan sandaran fail JSON FFlag anda",
                ["• Export frequently to save your changes"] = "• Eksport kerap untuk menyimpan perubahan anda",
                ["• Use Search feature to quickly find FFlags"] = "• Gunakan ciri Cari untuk mencari FFlags dengan pantas",
                ["• Check Activity Log for application status"] = "• Semak Log Aktiviti untuk status injeksi",
                ["Home"] = "Laman Utama",
                ["Global"] = "Global",
                ["Games"] = "Permainan",
                ["Settings"] = "Tetapan",
                ["FAQ"] = "FAQ",
                ["⚡ FFlags"] = "⚡ Terapkan",
                ["🌐 Global"] = "🌐 Global",
                ["🎮 Game FFlags"] = "🎮 FFlags Permainan",
                ["⚙️ Settings"] = "⚙️ Tetapan",
                ["❓ FAQ"] = "❓ FAQ",
                ["Open the editor to view and change flags, use presets, and choose whether Masterstrap may apply them when you launch."] = "Buka editor untuk melihat dan mengubah flag, guna pratetap, dan pilih sama ada Masterstrap boleh melakukannya semasa anda melancarkan.",
                ["Adjust Roblox wide settings such as read only mode, rendering, and framerate limits."] = "Laraskan tetapan menyeluruh Roblox seperti mod baca sahaja, rendering, dan had kadar bingkai.",
                ["Edit the flag table, filter by category, search, then use Back and Save on the FastFlags page when you are done."] = "Edit jadual flag, tapis mengikut kategori, cari, kemudian gunakan Kembali dan Simpan di halaman FastFlags apabila selesai.",
                ["Choose language, visual theme, and startup behavior for the app."] = "Pilih bahasa, tema visual, dan kelakuan permulaan untuk aplikasi.",
                ["Load a curated flag set for a game into your list, then adjust or save as usual."] = "Muat set flag terpilih untuk permainan ke dalam senarai anda, kemudian laraskan atau simpan seperti biasa.",
                ["Credits, FAQ, and short guides for using Masterstrap."] = "Kredit, FAQ, dan panduan ringkas untuk menggunakan Masterstrap.",
                ["Malay"] = "Bahasa Melayu",
                ["Indonesian"] = "Indonesia",
                ["Portuguese"] = "Portugis",
                ["English"] = "English",
                ["Filipino"] = "Filipino"
            }), AboutTabUiTranslations.EnToMal),
            MergeUiOverlays(MergeUiOverlays(BuildFlagUiTranslations.EnToMal, LaunchProgressUiTranslations.EnToMal), DialogsUiTranslations.EnToMal));

        private static readonly Dictionary<string, string> EnToJa = OverlayAboutTab(
            OverlayAboutTab(
            MergeTranslations(
            LoadMdTranslations("Masterstrap.Resources.Japanese.md"),
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["📁 Load FFlags JSON"] = "📁 FFlags JSONを読み込む",
                ["📄 Load FFlag Addresses"] = "📄 FFlagアドレスを読み込む",
                ["Add New FFlag"] = "新しいFFlagを追加",
                ["Add New FFlags"] = "新しいFFlagsを追加",
                ["Add or batch import new flags to your library"] = "新しいフラグを追加または一括インポートしてライブラリに登録",
                ["Flag Editor"] = "フラグエディター",
                ["FLAG EDITOR"] = "フラグエディター",
                ["Enter flags manually or load from JSON file"] = "フラグを手動で入力するか、JSONファイルから読み込み",
                ["FORMAT: name: value"] = "形式: 名前: 値",
                ["Each line = 1 FFlags"] = "1行 = 1 FFlag",
                ["(Each line = 1 FFlag. Example: MyFlag: true)"] = "（1行 = 1 FFlag。例: MyFlag: true）",
                ["Ready to add flags"] = "フラグを追加する準備ができました",
                ["Configuration saved successfully!"] = "設定が正常に保存されました！",
                ["Configuration saved successfully"] = "設定が正常に保存されました",
                ["CompleteAdsenseDialog created successfully"] = "CompleteAdsenseDialogが正常に作成されました",
                ["Complete the adsense to continue"] = "続行するには広告を完了してください",
                ["Support"] = "サポート",
                ["Please look at and click on ads so this software project can continue for free"] = "このソフトウェアプロジェクトを無料で続けるため、広告をご覧いただきクリックしてください",
                ["⏭ Skip ad 3:00"] = "⏭ 広告をスキップ 3:00",
                ["Skip ad"] = "広告をスキップ",
                ["How to skip ad? "] = "広告をスキップする方法は？ ",
                ["Click here"] = "ここをクリック",
                ["✓ Continue"] = "✓ 続行",
                ["✓ Ad Complete"] = "✓ 広告完了",
                ["Ad Complete"] = "広告完了",
                ["Please wait for the countdown to finish"] = "カウントダウンが終わるまでお待ちください",
                ["Please wait"] = "お待ちください",
                ["Please click 'Continue' button to proceed"] = "続行するには「続行」ボタンをクリックしてください",
                ["Ad Completed"] = "広告完了",
                ["Could not open support link"] = "サポートリンクを開けませんでした",
                ["Could not open help link"] = "ヘルプリンクを開けませんでした",
                ["Could not open Discord link"] = "Discordリンクを開けませんでした",
                ["Roblox Launch"] = "Roblox起動",
                ["Launching Roblox"] = "Robloxを起動しています",
                ["Masterstrap - Loading..."] = "Masterstrap - 読み込み中...",
                ["Masterstrap • "] = "Masterstrap • ",
                ["Loading FastFlag configuration..."] = "FastFlag設定を読み込み中...",
                ["Starting Roblox..."] = "Robloxを起動中...",
                ["Waiting for Roblox to open..."] = "Robloxの起動を待っています...",
                ["Roblox opened. Launch complete."] = "Robloxが起動しました。起動完了。",
                ["Masterstrap deploying auto-apply..."] = "Masterstrapが自動インジェクトを展開中...",
                ["Waiting for game to be ready..."] = "ゲームの準備を待っています...",
                ["Roblox closed before application."] = "インジェクト前にRobloxが終了しました。",
                ["Retrying application ({0}/{1})..."] = "インジェクトを再試行中 ({0}/{1})...",
                ["Masterstrap auto-applying..."] = "Masterstrapが自動インジェクト中...",
                ["Auto-applying..."] = "自動インジェクト中...",
                ["Waiting for Roblox... ({0}s)"] = "Robloxを待機中... ({0}秒)",
                ["Roblox not detected. Closing..."] = "Robloxが検出されません。終了しています...",
                ["Launch complete."] = "起動完了。",
                ["Applied successfully. Closing..."] = "インジェクト成功。終了しています...",
                ["Application failed."] = "インジェクト失敗。",
                ["Roblox not detected."] = "Robloxが検出されません。",
                ["Cancelled."] = "キャンセルしました。",
                ["Error: {0}"] = "エラー: {0}",
                ["Application applied. Waiting for game to apply..."] = "インジェクトを適用しました。ゲームの適用を待っています...",
                ["Applied successfully."] = "インジェクト成功。",
                ["Failed to launch Roblox: {0}"] = "Robloxの起動に失敗しました: {0}",
                ["Launch Error"] = "起動エラー",
                ["Adsense dialog marked as OPEN"] = "AdsenseダイアログをOPENとしてマークしました",
                ["Adsense dialog marked as CLOSED"] = "AdsenseダイアログをCLOSEDとしてマークしました",
                ["FFlags applied successfully!"] = "FFlagsの注入に成功しました！",
                ["⚡ APPLY"] = "⚡ インジェクト",
                ["↩️ UNAPPLY"] = "↩️ アンインジェクト",
                ["Load FFlags JSON"] = "FFlags JSONを読み込む",
                ["Load FFlag Addresses"] = "FFlagアドレスを読み込む",
                ["APPLY"] = "インジェクト",
                ["UNAPPLY"] = "アンインジェクト",
                ["Activity Log"] = "活動ログ",
                ["0 entries"] = "0件の記録",
                ["Clear Log"] = "ログをクリア",
                ["Add"] = "追加",
                ["Delete"] = "削除",
                ["Clear All"] = "すべてクリア",
                ["Export"] = "エクスポート",
                ["Cancel"] = "キャンセル",
                ["FastFlag Editor"] = "FastFlagエディター",
                ["manage your own Fast Flags. Use with caution"] = "自分のFast Flagを管理します。注意して使用してください。",
                ["Allow Masterstrap to manage Fast Flags"] = "MasterstrapによるFast Flagの管理を許可する",
                ["Settings and Options"] = "設定とオプション",
                ["Language Settings"] = "言語設定",
                ["Select your preferred display language for the application interface."] = "アプリケーションのインターフェースで使用する表示言語を選択してください。",
                ["Vietnamese"] = "ベトナム語",
                ["Desktop Shortcut"] = "デスクトップショートカット",
                ["Create a shortcut on your Desktop for quick access to Masterstrap (recommended)"] = "Masterstrapへすばやくアクセスできるように、デスクトップにショートカットを作成します（推奨）",
                ["Create a shortcut on your Desktop for quick access to Masterstrap"] = "Masterstrapへすばやくアクセスできるように、デスクトップにショートカットを作成します（推奨）",
                ["General Settings"] = "一般設定",
                ["Auto-load FFlags on startup (recommended)"] = "起動時にFFlagsを自動読み込み（推奨）",
                ["Auto-load FFlags on startup"] = "起動時にFFlagsを自動読み込み",
                ["Auto-load Cache on startup (recommended)"] = "起動時にキャッシュを自動読み込み（推奨）",
                ["Auto-load Cache on startup"] = "起動時にキャッシュを自動読み込み",
                ["Auto-apply when Roblox is detected (recommended)"] = "Robloxが検出されたときに自動インジェクト（推奨）",
                ["Auto-check for updates on startup (recommended)"] = "起動時にアップデートを自動確認（推奨）",
                ["Auto-check for updates on startup"] = "起動時にアップデートを自動確認",
                ["Minimize to system tray"] = "システムトレイに最小化",
                ["Optimizer"] = "最適化",
                ["Auto-cleanup temp files (recommended)"] = "一時ファイルを自動クリーンアップ（推奨）",
                ["Auto-cleanup temp files"] = "一時ファイルを自動クリーンアップ",
                ["Memory optimization (recommended)"] = "メモリ最適化（推奨）",
                ["Memory optimization"] = "メモリ最適化",
                [" (recommended)"] = " （推奨）",
                ["Save and Launch"] = "保存して起動",
                ["Save"] = "保存",
                ["Close"] = "閉じる",
                ["Rendering and Graphics"] = "レンダリングとグラフィック",
                ["Automatic"] = "自動",
                ["Anti-aliasing quality (MSAA)"] = "アンチエイリアス品質（MSAA）",
                ["Preserve rendering quality with display scaling"] = "ディスプレイスケーリング時にレンダリング品質を維持する",
                ["Roblox reduces your rendering quality depending on how your display is scaled in Windows."] = "Windowsでのディスプレイのスケーリング設定に応じて、Robloxはレンダリング品質を下げる場合があります。",
                ["FRM Quality Override"] = "FRM品質を上書き",
                ["Choose the FRM quality that Roblox should use."] = "Robloxが使用するFRM品質を選択します。",
                ["Rendering mode"] = "レンダリングモード",
                ["Texture quality"] = "テクスチャ品質",
                ["Set as Read-Only"] = "読み取り専用に設定",
                ["Prevent Roblox from overriding global settings."] = "Robloxがグローバル設定を上書きするのを防ぎます。",
                ["Graphics Quality"] = "グラフィック品質",
                ["Graphic advanced"] = "高度なグラフィック",
                ["Set the graphics quality of your game"] = "ゲームのグラフィック品質を設定します",
                ["Max Quality Enabled"] = "最高品質を有効化",
                ["Enables maximum graphics quality mode for enhanced visual effects and rendering detail."] = "視覚効果とレンダリングの詳細を向上させるため、最高グラフィック品質モードを有効にします。",
                ["Graphics Quality Level"] = "グラフィック品質レベル",
                ["Adjusts the in-game graphics quality level from low to maximum."] = "ゲーム内のグラフィック品質レベルを低から最高まで調整します。",
                ["Framerate Limit"] = "フレームレート制限",
                ["Unlock framerate limit for Roblox. Going above 240 FPS is not recommended."] = "Robloxのフレームレート制限を解除します。240FPSを超えることは推奨されません。",
                ["User Interface and Layout"] = "ユーザーインターフェースとレイアウト",
                ["Transparency"] = "透明度",
                ["Custom transparency for UI elements."] = "UI要素の透明度をカスタマイズします。",
                ["Reduced Motion"] = "モーションを減らす",
                ["Removes the animation on the escape menu."] = "エスケープメニューのアニメーションを削除します。",
                ["Font Size"] = "フォントサイズ",
                ["Choose how large the font should appear."] = "フォントの表示サイズを選択します。",
                ["Default"] = "デフォルト",
                ["Other"] = "その他",
                ["Mouse Sensitivity"] = "マウス感度",
                ["Change how fast the camera will move in-game."] = "ゲーム内でのカメラの移動速度を変更します。",
                ["VR Enabled"] = "VRを有効化",
                ["Player Name Visibility"] = "プレイヤー名の表示",
                ["Hide name tags above other players for a cleaner screen experience."] = "画面をすっきりさせるため、他のプレイヤーの上に表示されるネームタグを非表示にします。",
                ["FAQ and Guide"] = "FAQとガイド",
                ["How to Use Masterstrap"] = "Masterstrapの使い方",
                ["1. Load FFlags JSON file"] = "FFlagsのJSONファイルを読み込む",
                ["2. Load FFlag Addresses (optional)"] = "FFlagのアドレスを読み込む（任意）",
                ["3. Make sure Roblox is running"] = "Robloxが起動していることを確認する",
                ["4. Click APPLY button to apply FFlags"] = "APPLYボタンをクリックしてFFlagsをインジェクトする",
                ["5. Check Activity Log for results"] = "結果はアクティビティログで確認する",
                ["Apply"] = "適用",
                ["Select Game FFlags Preset"] = "ゲームFFlagsプリセットを選択",
                ["🎮 Select Game FFlags Preset"] = "🎮 ゲームFFlagsプリセットを選択",
                ["How to Edit FFlags"] = "FFlagsの編集方法",
                ["• Go to Edit tab to modify loaded FFlags"] = "Editタブに移動して読み込んだFFlagsを変更する",
                ["• Click Add to create new FFlag entry"] = "Addをクリックして新しいFFlagエントリを作成する",
                ["• Click Delete to remove selected FFlag"] = "Deleteをクリックして選択したFFlagを削除する",
                ["• Use Search to find specific FFlags"] = "Searchを使用して特定のFFlagsを検索する",
                ["• Click Export to save modified FFlags"] = "Exportをクリックして変更したFFlagsを保存する",
                ["Troubleshooting"] = "トラブルシューティング",
                ["Roblox not found?"] = "Robloxが見つかりませんか？",
                ["Make sure Roblox is running before applying"] = "インジェクトする前にRobloxが起動していることを確認してください",
                ["Application failed?"] = "インジェクトに失敗しましたか？",
                ["Please ensure that your Roblox version matches the version that Masterstrap has requested"] = "RobloxのバージョンがMasterstrapで要求されているバージョンと一致していることを確認してください",
                ["FFlags not loading?"] = "FFlagsが読み込まれませんか？",
                ["Verify JSON file format is correct and valid"] = "JSONファイルの形式が正しく有効であることを確認してください",
                ["Game crash after applying?"] = "インジェクト後にゲームがクラッシュしますか？",
                ["Tips and Tricks"] = "ヒントとコツ",
                ["• Keep your FFlag JSON file backed up"] = "FFlagのJSONファイルをバックアップしてください",
                ["• Export frequently to save your changes"] = "変更を保存するため、こまめにエクスポートしてください",
                ["• Use Search feature to quickly find FFlags"] = "Search機能を使用してFFlagsをすばやく見つけてください",
                ["• Check Activity Log for application status"] = "インジェクトの状態はアクティビティログで確認してください",
                ["Export"] = "エクスポート",
                ["Physics"] = "物理",
                ["Audio"] = "オーディオ",
                ["Information System"] = "情報システム",
                ["INFORMATION SYSTEM"] = "情報システム",
                ["Home"] = "ホーム",
                ["Global"] = "グローバル",
                ["Games"] = "ゲーム",
                ["Settings"] = "設定",
                ["FAQ"] = "FAQ",
                ["Japanese"] = "日本語",
                ["English"] = "English",
                ["Filipino"] = "フィリピン語",
                ["Indonesian"] = "インドネシア語",
                ["Portuguese"] = "ポルトガル語",
                ["Malay"] = "マレー語",
                ["Loaded"] = "読み込み完了",
                ["Fast Mode"] = "高速モード",
                ["Opening Roblox..."] = "Robloxを開いています...",
                ["Activity log"] = "アクティビティログ",
                ["Activity Log"] = "アクティビティログ",
                ["Clear Log"] = "ログをクリア",
                ["0 entries"] = "0件の記録",
                ["System initialized"] = "システムを初期化しました",
                ["Ready to load FFlags"] = "FFlagsの読み込み準備完了",
                ["Not set"] = "未設定",
                ["Saved FFlags:"] = "保存したFFlags:",
                ["Enabled"] = "有効",
                ["Disabled"] = "無効",
                ["Auto-load FFlags:"] = "FFlagsの自動読み込み:",
                ["Auto-load Addresses:"] = "アドレスの自動読み込み:",
                ["Not detected"] = "未検出",
                ["Roblox Version:"] = "Robloxバージョン:",
                ["Unknown"] = "不明",
                ["Software Version:"] = "ソフトウェアバージョン:",
                ["Version Compatibility:"] = "バージョン互換性:",
                ["MATCH"] = "一致",
                ["MISMATCH"] = "不一致",
                ["UNKNOWN"] = "不明",
                ["Application successful ({0} FFlags)"] = "インジェクト成功（{0} FFlags）",
                ["Application failed ({0} errors)"] = "インジェクト失敗（{0}件のエラー）",
                ["now"] = "たった今",
                ["s ago"] = " 秒前",
                ["m ago"] = " 分前",
                ["Success"] = "成功",
                ["Failed"] = "失敗",
                ["Pending"] = "保留中",
                ["Mixed"] = "混合",
                ["Status"] = "状態",
                ["Session"] = "セッション",
                ["Last"] = "最終",
                ["Actions"] = "アクション",
                ["Activity log cleared"] = "アクティビティログをクリアしました",
                ["JSON file not found"] = "JSONファイルが見つかりません",
                ["JSON Content Preview:"] = "JSONコンテンツプレビュー:",
                ["... and {0} more entries"] = "... とあと{0}件",
                ["Total entries in JSON: {0}"] = "JSON内の総エントリ数: {0}",
                ["Invalid JSON format"] = "無効なJSON形式",
                ["Error parsing JSON"] = "JSONの解析エラー",
                [" Loading FFlag addresses..."] = " FFlagアドレスを読み込み中...",
                [" Auto-loading FFlag addresses..."] = " FFlagアドレスを自動読み込み中...",
                ["Join"] = "参加",
                ["Join Discord"] = "Discordに参加",
                ["Open the editor to view and change flags, use presets, and choose whether Masterstrap may apply them when you launch."] = "エディターを開いてフラグの表示・変更、プリセットの利用、起動時に Masterstrap が適用するかどうかを選びます。",
                ["Adjust Roblox wide settings such as read only mode, rendering, and framerate limits."] = "読み取り専用モード、レンダリング、フレームレート上限など、Roblox 全体の設定を調整します。",
                ["Edit the flag table, filter by category, search, then use Back and Save on the FastFlags page when you are done."] = "フラグ表を編集し、カテゴリで絞り込み、検索し、完了したら FastFlags ページで戻ると保存を使います。",
                ["Choose language, visual theme, and startup behavior for the app."] = "アプリの言語、ビジュアルテーマ、起動時の動作を選びます。",
                ["Load a curated flag set for a game into your list, then adjust or save as usual."] = "ゲーム用に選ばれたフラグセットをリストに読み込み、いつも通り調整または保存します。",
                ["Credits, FAQ, and short guides for using Masterstrap."] = "クレジット、FAQ、Masterstrap の簡単なガイド。",
                ["Made By ©Dank1ngs"] = "制作 ©Dank1ngs"
            }), AboutTabUiTranslations.EnToJa),
            MergeUiOverlays(MergeUiOverlays(BuildFlagUiTranslations.EnToJa, LaunchProgressUiTranslations.EnToJa), DialogsUiTranslations.EnToJa));

        private static readonly Dictionary<string, string> EnToZh = OverlayAboutTab(
            OverlayAboutTab(
            MergeTranslations(
            LoadMdTranslations("Masterstrap.Resources.Chinese.md"),
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["FastFlag Editor"] = "FastFlag 编辑器",
                ["manage your own Fast Flags. Use with caution"] = "管理您自己的 Fast Flag。请谨慎使用。",
                ["Allow Masterstrap to manage Fast Flags"] = "允许 Masterstrap 管理 Fast Flag",
                ["Turning off this option will prevent any configuration here from being applied to Roblox."] = "关闭此选项将阻止此处的任何配置应用到 Roblox。",
                ["Rendering and Graphics"] = "渲染与图形",
                ["Anti-aliasing quality (MSAA)"] = "抗锯齿质量（MSAA）",
                ["Automatic"] = "自动",
                ["Preserve rendering quality with display scaling"] = "在使用显示缩放时保持渲染质量",
                ["Roblox reduces your rendering quality depending on how your display is scaled in Windows."] = "Roblox 会根据 Windows 中的显示缩放设置降低渲染质量。",
                ["FRM Quality Override"] = "覆盖 FRM 质量",
                ["Choose the FRM quality that Roblox should use."] = "选择 Roblox 应使用的 FRM 质量。",
                ["Rendering mode"] = "渲染模式",
                ["Texture quality"] = "纹理质量",
                ["Settings and Options"] = "设置与选项",
                ["Language Settings"] = "语言设置",
                ["Select your preferred display language for the application interface."] = "选择应用程序界面的首选显示语言。",
                ["Desktop Shortcut"] = "桌面快捷方式",
                ["Create a shortcut on your Desktop for quick access to Masterstrap (recommended)"] = "在桌面创建快捷方式以快速访问 Masterstrap（推荐）",
                ["Create a shortcut on your Desktop for quick access to Masterstrap"] = "在桌面创建快捷方式以快速访问 Masterstrap（推荐）",
                ["General Settings"] = "常规设置",
                ["Auto-load FFlags on startup (recommended)"] = "启动时自动加载 FFlags（推荐）",
                ["Auto-load FFlags on startup"] = "启动时自动加载 FFlags",
                ["Auto-load Cache on startup (recommended)"] = "启动时自动加载缓存（推荐）",
                ["Auto-load Cache on startup"] = "启动时自动加载缓存",
                ["Auto-apply when Roblox is detected (recommended)"] = "检测到 Roblox 时自动注入（推荐）",
                ["Auto-check for updates on startup (recommended)"] = "启动时自动检查更新（推荐）",
                ["Auto-check for updates on startup"] = "启动时自动检查更新",
                ["Minimize to system tray"] = "最小化到系统托盘",
                ["Optimizer"] = "优化器",
                ["Auto-cleanup temp files (recommended)"] = "自动清理临时文件（推荐）",
                ["Auto-cleanup temp files"] = "自动清理临时文件",
                ["Memory optimization (recommended)"] = "内存优化（推荐）",
                ["Memory optimization"] = "内存优化",
                [" (recommended)"] = " （推荐）",
                ["Save and Launch"] = "保存并启动",
                ["Save"] = "保存",
                ["Close"] = "关闭",
                ["Cancel"] = "取消",
                ["Load FFlags JSON"] = "加载 FFlags JSON",
                ["Load FFlags Addresses"] = "加载 FFlags 地址",
                ["Load FFlag Addresses"] = "加载 FFlag 地址",
                ["📁 Load FFlags JSON"] = "📁 加载 FFlags JSON",
                ["📄 Load FFlag Addresses"] = "📄 加载 FFlag 地址",
                ["Add New FFlag"] = "添加新 FFlag",
                ["Add New FFlags"] = "添加新 FFlags",
                ["Add or batch import new flags to your library"] = "添加或批量导入新标志到您的库",
                ["Flag Editor"] = "标志编辑器",
                ["FLAG EDITOR"] = "标志编辑器",
                ["Enter flags manually or load from JSON file"] = "手动输入标志或从 JSON 文件加载",
                ["FORMAT: name: value"] = "格式：名称: 值",
                ["Each line = 1 FFlags"] = "每行 = 1 个 FFlag",
                ["(Each line = 1 FFlag. Example: MyFlag: true)"] = "（每行 = 1 个 FFlag。例如：MyFlag: true）",
                ["Ready to add flags"] = "准备添加标志",
                ["Configuration saved successfully!"] = "配置保存成功！",
                ["Configuration saved successfully"] = "配置保存成功",
                ["CompleteAdsenseDialog created successfully"] = "CompleteAdsenseDialog 创建成功",
                ["Complete the adsense to continue"] = "完成广告以继续",
                ["Support"] = "支持",
                ["Please look at and click on ads so this software project can continue for free"] = "请查看并点击广告，以便此软件项目可以免费继续",
                ["⏭ Skip ad 3:00"] = "⏭ 跳过广告 3:00",
                ["Skip ad"] = "跳过广告",
                ["How to skip ad? "] = "如何跳过广告？ ",
                ["Click here"] = "点击这里",
                ["✓ Continue"] = "✓ 继续",
                ["✓ Ad Complete"] = "✓ 广告完成",
                ["Ad Complete"] = "广告完成",
                ["Please wait for the countdown to finish"] = "请等待倒计时结束",
                ["Please wait"] = "请稍候",
                ["Please click 'Continue' button to proceed"] = "请点击“继续”按钮以继续",
                ["Ad Completed"] = "广告已完成",
                ["Could not open support link"] = "无法打开支持链接",
                ["Could not open help link"] = "无法打开帮助链接",
                ["Could not open Discord link"] = "无法打开 Discord 链接",
                ["Roblox Launch"] = "启动 Roblox",
                ["Launching Roblox"] = "正在启动 Roblox",
                ["Masterstrap - Loading..."] = "Masterstrap - 加载中...",
                ["Masterstrap • "] = "Masterstrap • ",
                ["Loading FastFlag configuration..."] = "正在加载 FastFlag 配置...",
                ["Starting Roblox..."] = "正在启动 Roblox...",
                ["Waiting for Roblox to open..."] = "等待 Roblox 打开...",
                ["Roblox opened. Launch complete."] = "Roblox 已打开。启动完成。",
                ["Masterstrap deploying auto-apply..."] = "Masterstrap 正在部署自动注入...",
                ["Waiting for game to be ready..."] = "等待游戏准备就绪...",
                ["Roblox closed before application."] = "注入前 Roblox 已关闭。",
                ["Retrying application ({0}/{1})..."] = "正在重试注入 ({0}/{1})...",
                ["Masterstrap auto-applying..."] = "Masterstrap 正在自动注入...",
                ["Auto-applying..."] = "正在自动注入...",
                ["Waiting for Roblox... ({0}s)"] = "等待 Roblox... ({0}秒)",
                ["Roblox not detected. Closing..."] = "未检测到 Roblox。正在关闭...",
                ["Launch complete."] = "启动完成。",
                ["Applied successfully. Closing..."] = "注入成功。正在关闭...",
                ["Application failed."] = "注入失败。",
                ["Roblox not detected."] = "未检测到 Roblox。",
                ["Cancelled."] = "已取消。",
                ["Error: {0}"] = "错误：{0}",
                ["Application applied. Waiting for game to apply..."] = "已应用注入。等待游戏应用...",
                ["Applied successfully."] = "注入成功。",
                ["Failed to launch Roblox: {0}"] = "启动 Roblox 失败：{0}",
                ["Launch Error"] = "启动错误",
                ["Adsense dialog marked as OPEN"] = "Adsense 对话框已标记为打开",
                ["Adsense dialog marked as CLOSED"] = "Adsense 对话框已标记为关闭",
                ["FFlags applied successfully!"] = "FFlags 注入成功！",
                ["⚡ APPLY"] = "⚡ 注入",
                ["↩️ UNAPPLY"] = "↩️ 取消注入",
                ["APPLY"] = "注入",
                ["UNAPPLY"] = "取消注入",
                ["Activity Log"] = "活动日志",
                ["Clear Log"] = "清除日志",
                ["Add"] = "添加",
                ["Delete"] = "删除",
                ["Clear All"] = "清除全部",
                ["Export"] = "导出",
                ["Set as Read-Only"] = "设置为只读",
                ["Prevent Roblox from overriding global settings."] = "防止 Roblox 覆盖全局设置。",
                ["Graphics Quality"] = "图形质量",
                ["Set the graphics quality of your game"] = "设置您的游戏图形质量",
                ["Max Quality Enabled"] = "启用最高质量",
                ["Enables maximum graphics quality mode for enhanced visual effects and rendering detail."] = "启用最高图形质量模式，以增强视觉效果和渲染细节。",
                ["Graphics Quality Level"] = "图形质量等级",
                ["Adjusts the in-game graphics quality level from low to maximum."] = "将游戏内图形质量等级从低调整到最高。",
                ["Framerate Limit"] = "帧率限制",
                ["Unlock framerate limit for Roblox. Going above 240 FPS is not recommended."] = "解除 Roblox 的帧率限制。不建议超过 240 FPS。",
                ["User Interface and Layout"] = "用户界面与布局",
                ["Transparency"] = "透明度",
                ["Custom transparency for UI elements."] = "为 UI 元素设置自定义透明度。",
                ["Reduced Motion"] = "减少动画",
                ["Removes the animation on the escape menu."] = "移除 Esc 菜单中的动画效果。",
                ["Font Size"] = "字体大小",
                ["Choose how large the font should appear."] = "选择字体显示的大小。",
                ["Default"] = "默认",
                ["Other"] = "其他",
                ["Mouse Sensitivity"] = "鼠标灵敏度",
                ["Change how fast the camera will move in-game."] = "更改游戏中摄像机移动的速度。",
                ["VR Enabled"] = "启用 VR",
                ["Player Name Visibility"] = "玩家名称可见性",
                ["Hide name tags above other players for a cleaner screen experience."] = "隐藏其他玩家头上的名称标签，以获得更清晰的屏幕体验。",
                ["FAQ and Guide"] = "常见问题与指南",
                ["How to Use Masterstrap"] = "如何使用 Masterstrap",
                ["1. Load FFlags JSON file"] = "1. 加载 FFlags JSON 文件",
                ["2. Load FFlag Addresses (optional)"] = "2. 加载 FFlag 地址（可选）",
                ["3. Make sure Roblox is running"] = "3. 确保 Roblox 正在运行",
                ["4. Click APPLY button to apply FFlags"] = "4. 点击 APPLY 按钮以注入 FFlags",
                ["5. Check Activity Log for results"] = "5. 查看活动日志以获取结果",
                ["Apply"] = "应用",
                ["Select Game FFlags Preset"] = "选择游戏 FFlags 预设",
                ["🎮 Select Game FFlags Preset"] = "🎮 选择游戏 FFlags 预设",
                ["How to Edit FFlags"] = "如何编辑 FFlags",
                ["• Go to Edit tab to modify loaded FFlags"] = "前往 Edit 选项卡以修改已加载的 FFlags",
                ["• Click Add to create new FFlag entry"] = "点击 Add 创建新的 FFlag 条目",
                ["• Click Delete to remove selected FFlag"] = "点击 Delete 删除所选的 FFlag",
                ["• Use Search to find specific FFlags"] = "使用 Search 查找特定的 FFlags",
                ["• Click Export to save modified FFlags"] = "点击 Export 保存修改后的 FFlags",
                ["Troubleshooting"] = "故障排除",
                ["Roblox not found?"] = "未找到 Roblox？",
                ["Make sure Roblox is running before applying"] = "在注入之前确保 Roblox 正在运行",
                ["Application failed?"] = "注入失败？",
                ["Please ensure that your Roblox version matches the version that Masterstrap has requested"] = "请确保您的 Roblox 版本与 Masterstrap 所要求的版本一致",
                ["FFlags not loading?"] = "FFlags 未加载？",
                ["Verify JSON file format is correct and valid"] = "请确认 JSON 文件格式正确且有效",
                ["Game crash after applying?"] = "注入后游戏崩溃？",
                ["Tips and Tricks"] = "提示与技巧",
                ["• Keep your FFlag JSON file backed up"] = "备份您的 FFlag JSON 文件",
                ["• Export frequently to save your changes"] = "经常导出以保存您的更改",
                ["• Use Search feature to quickly find FFlags"] = "使用 Search 功能快速查找 FFlags",
                ["• Check Activity Log for application status"] = "查看活动日志以了解注入状态",
                ["Information System"] = "信息系统",
                ["INFORMATION SYSTEM"] = "信息系统",
                ["FFlags:"] = "FFlags:",
                ["Count:"] = "数量:",
                ["Roblox Version:"] = "Roblox 版本:",
                ["Software Version:"] = "软件版本:",
                ["Last update:"] = "上次更新:",
                ["Not loaded"] = "未加载",
                ["Fast Mode"] = "快速模式",
                ["Loaded"] = "已加载",
                ["Opening Roblox..."] = "正在打开 Roblox...",
                ["Idle"] = "空闲",
                ["Join"] = "加入",
                ["Join Discord"] = "加入 Discord",
                ["Made By ©Dank1ngs"] = "由 ©Dank1ngs 制作",
                ["Home"] = "首页",
                ["Global"] = "全局",
                ["Games"] = "游戏",
                ["Settings"] = "设置",
                ["FAQ"] = "FAQ",
                ["Chinese"] = "中国",
                ["Thai"] = "泰语",
                ["English"] = "English",
                ["Vietnamese"] = "越南语",
                ["Filipino"] = "菲律宾语",
                ["Indonesian"] = "印度尼西亚语",
                ["Portuguese"] = "葡萄牙语",
                ["Malay"] = "马来语",
                ["Japanese"] = "日语",
                ["Physics"] = "物理",
                ["Audio"] = "音频",
                ["0 entries"] = "0 条",
                ["Activity log"] = "活动日志",
                ["Activity Log"] = "活动日志",
                ["Clear Log"] = "清除日志",
                ["System initialized"] = "系统已初始化",
                ["Ready to load FFlags"] = "准备加载 FFlags",
                ["Not set"] = "未设置",
                ["Saved FFlags:"] = "已保存的 FFlags:",
                ["Enabled"] = "已启用",
                ["Disabled"] = "已禁用",
                ["Auto-load FFlags:"] = "自动加载 FFlags:",
                ["Auto-load Addresses:"] = "自动加载地址:",
                ["Not detected"] = "未检测到",
                ["Roblox Version:"] = "Roblox 版本:",
                ["Unknown"] = "未知",
                ["Software Version:"] = "软件版本:",
                ["Version Compatibility:"] = "版本兼容性:",
                ["MATCH"] = "匹配",
                ["MISMATCH"] = "不匹配",
                ["UNKNOWN"] = "未知",
                ["Application successful ({0} FFlags)"] = "注入成功（{0} 个 FFlags）",
                ["Application failed ({0} errors)"] = "注入失败（{0} 个错误）",
                ["now"] = "刚刚",
                ["s ago"] = " 秒前",
                ["m ago"] = " 分钟前",
                ["Success"] = "成功",
                ["Failed"] = "失败",
                ["Pending"] = "待处理",
                ["Mixed"] = "混合",
                ["Status"] = "状态",
                ["Session"] = "会话",
                ["Last"] = "最后",
                ["Actions"] = "操作",
                ["Activity log cleared"] = "已清除活动日志",
                ["JSON file not found"] = "未找到 JSON 文件",
                ["JSON Content Preview:"] = "JSON 内容预览:",
                ["... and {0} more entries"] = "... 还有 {0} 条",
                ["Total entries in JSON: {0}"] = "JSON 中总条数: {0}",
                ["Invalid JSON format"] = "JSON 格式无效",
                ["Error parsing JSON"] = "解析 JSON 时出错",
                [" Loading FFlag addresses..."] = " 正在加载 FFlag 地址...",
                [" Auto-loading FFlag addresses..."] = " 正在自动加载 FFlag 地址...",
                ["Presets"] = "预设",
                ["Graphic advanced"] = "高级图形",
                ["Lowest quality"] = "最低质量",
                ["Highest quality"] = "最高质量",
                ["Open the editor to view and change flags, use presets, and choose whether Masterstrap may apply them when you launch."] = "打开编辑器查看和修改标志、使用预设，并选择启动时是否由 Masterstrap 应用它们。",
                ["Adjust Roblox wide settings such as read only mode, rendering, and framerate limits."] = "调整 Roblox 的全局设置，例如只读模式、渲染和帧率限制。",
                ["Edit the flag table, filter by category, search, then use Back and Save on the FastFlags page when you are done."] = "编辑标志表、按类别筛选、搜索，完成后在 FastFlags 页面使用返回和保存。",
                ["Choose language, visual theme, and startup behavior for the app."] = "选择应用的语言、视觉主题和启动行为。",
                ["Load a curated flag set for a game into your list, then adjust or save as usual."] = "将某款游戏的精选标志集加载到列表中，然后照常调整或保存。",
                ["Credits, FAQ, and short guides for using Masterstrap."] = "致谢、常见问题与使用 Masterstrap 的简短指南。"
            }), AboutTabUiTranslations.EnToZh),
            MergeUiOverlays(MergeUiOverlays(BuildFlagUiTranslations.EnToZh, LaunchProgressUiTranslations.EnToZh), DialogsUiTranslations.EnToZh));

        private static readonly Dictionary<string, string> EnToTh = OverlayAboutTab(
            OverlayAboutTab(
            MergeTranslations(
            LoadMdTranslations("Masterstrap.Resources.Thai.md"),
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["FastFlag Editor"] = "ตัวแก้ไข FastFlag",
                ["manage your own Fast Flags. Use with caution"] = "จัดการ Fast Flag ของคุณเอง ใช้อย่างระมัดระวัง",
                ["Allow Masterstrap to manage Fast Flags"] = "อนุญาตให้ Masterstrap จัดการ Fast Flag",
                ["Turning off this option will prevent any configuration here from being applied to Roblox."] = "การปิดตัวเลือกนี้จะป้องกันไม่ให้การตั้งค่าใด ๆ ที่นี่ถูกนำไปใช้กับ Roblox",
                ["Rendering and Graphics"] = "การเรนเดอร์และกราฟิก",
                ["Anti-aliasing quality (MSAA)"] = "คุณภาพ Anti-aliasing (MSAA)",
                ["Automatic"] = "อัตโนมัติ",
                ["Preserve rendering quality with display scaling"] = "รักษาคุณภาพการเรนเดอร์เมื่อใช้การปรับขนาดหน้าจอ",
                ["Roblox reduces your rendering quality depending on how your display is scaled in Windows."] = "Roblox จะลดคุณภาพการเรนเดอร์ตามการปรับขนาดหน้าจอใน Windows",
                ["FRM Quality Override"] = "แทนที่คุณภาพ FRM",
                ["Choose the FRM quality that Roblox should use."] = "เลือกคุณภาพ FRM ที่ Roblox ควรใช้",
                ["Rendering mode"] = "โหมดการเรนเดอร์",
                ["Texture quality"] = "คุณภาพพื้นผิว",
                ["Settings and Options"] = "การตั้งค่าและตัวเลือก",
                ["Language Settings"] = "การตั้งค่าภาษา",
                ["Select your preferred display language for the application interface."] = "เลือกภาษาที่ต้องการใช้สำหรับอินเทอร์เฟซของแอปพลิเคชัน",
                ["Desktop Shortcut"] = "ทางลัดบนเดสก์ท็อป",
                ["Create a shortcut on your Desktop for quick access to Masterstrap (recommended)"] = "สร้างทางลัดบนเดสก์ท็อปเพื่อเข้าถึง Masterstrap อย่างรวดเร็ว (แนะนำ)",
                ["Create a shortcut on your Desktop for quick access to Masterstrap"] = "สร้างทางลัดบนเดสก์ท็อปเพื่อเข้าถึง Masterstrap อย่างรวดเร็ว (แนะนำ)",
                ["General Settings"] = "การตั้งค่าทั่วไป",
                ["Auto-load FFlags on startup (recommended)"] = "โหลด FFlags อัตโนมัติเมื่อเริ่มต้นระบบ (แนะนำ)",
                ["Auto-load FFlags on startup"] = "โหลด FFlags อัตโนมัติเมื่อเริ่มต้นระบบ",
                ["Auto-load Cache on startup (recommended)"] = "โหลด Cache อัตโนมัติเมื่อเริ่มต้นระบบ (แนะนำ)",
                ["Auto-load Cache on startup"] = "โหลด Cache อัตโนมัติเมื่อเริ่มต้นระบบ",
                ["Auto-apply when Roblox is detected (recommended)"] = "ฉีดอัตโนมัติเมื่อพบ Roblox (แนะนำ)",
                ["Auto-check for updates on startup (recommended)"] = "ตรวจสอบการอัปเดตอัตโนมัติเมื่อเริ่มต้นระบบ (แนะนำ)",
                ["Auto-check for updates on startup"] = "ตรวจสอบการอัปเดตอัตโนมัติเมื่อเริ่มต้นระบบ",
                ["Minimize to system tray"] = "ย่อไปที่ถาดระบบ",
                ["Optimizer"] = "ตัวเพิ่มประสิทธิภาพ",
                ["Auto-cleanup temp files (recommended)"] = "ล้างไฟล์ชั่วคราวอัตโนมัติ (แนะนำ)",
                ["Auto-cleanup temp files"] = "ล้างไฟล์ชั่วคราวอัตโนมัติ",
                ["Memory optimization (recommended)"] = "เพิ่มประสิทธิภาพหน่วยความจำ (แนะนำ)",
                ["Memory optimization"] = "เพิ่มประสิทธิภาพหน่วยความจำ",
                [" (recommended)"] = " (แนะนำ)",
                ["Save and Launch"] = "บันทึกและเปิดใช้",
                ["Save"] = "บันทึก",
                ["Close"] = "ปิด",
                ["Cancel"] = "ยกเลิก",
                ["Load FFlags JSON"] = "โหลด FFlags JSON",
                ["Load FFlags Addresses"] = "โหลดที่อยู่ FFlags",
                ["Load FFlag Addresses"] = "โหลดที่อยู่ FFlag",
                ["📁 Load FFlags JSON"] = "📁 โหลด FFlags JSON",
                ["📄 Load FFlag Addresses"] = "📄 โหลดที่อยู่ FFlag",
                ["Add New FFlag"] = "เพิ่ม FFlag ใหม่",
                ["Add New FFlags"] = "เพิ่ม FFlags ใหม่",
                ["Add or batch import new flags to your library"] = "เพิ่มหรือนำเข้าพร้อมกันแฟล็กใหม่ไปยังคลังของคุณ",
                ["Flag Editor"] = "ตัวแก้ไขแฟล็ก",
                ["FLAG EDITOR"] = "ตัวแก้ไขแฟล็ก",
                ["Enter flags manually or load from JSON file"] = "ป้อนแฟล็กด้วยตนเองหรือโหลดจากไฟล์ JSON",
                ["FORMAT: name: value"] = "รูปแบบ: ชื่อ: ค่า",
                ["Each line = 1 FFlags"] = "แต่ละบรรทัด = 1 FFlag",
                ["(Each line = 1 FFlag. Example: MyFlag: true)"] = "(แต่ละบรรทัด = 1 FFlag ตัวอย่าง: MyFlag: true)",
                ["Ready to add flags"] = "พร้อมเพิ่มแฟล็ก",
                ["Configuration saved successfully!"] = "บันทึกการตั้งค่าสำเร็จ!",
                ["Configuration saved successfully"] = "บันทึกการตั้งค่าสำเร็จ",
                ["CompleteAdsenseDialog created successfully"] = "สร้าง CompleteAdsenseDialog สำเร็จ",
                ["Complete the adsense to continue"] = "ดำเนินการโฆษณาให้เสร็จเพื่อดำเนินการต่อ",
                ["Support"] = "สนับสนุน",
                ["Please look at and click on ads so this software project can continue for free"] = "กรุณาดูและคลิกโฆษณาเพื่อให้โปรเจกต์ซอฟต์แวร์นี้ดำเนินการต่อได้ฟรี",
                ["⏭ Skip ad 3:00"] = "⏭ ข้ามโฆษณา 3:00",
                ["Skip ad"] = "ข้ามโฆษณา",
                ["How to skip ad? "] = "ข้ามโฆษณาอย่างไร? ",
                ["Click here"] = "คลิกที่นี่",
                ["✓ Continue"] = "✓ ดำเนินการต่อ",
                ["✓ Ad Complete"] = "✓ ดูโฆษณาแล้ว",
                ["Ad Complete"] = "ดูโฆษณาแล้ว",
                ["Please wait for the countdown to finish"] = "กรุณารอจนนับถอยหลังเสร็จ",
                ["Please wait"] = "กรุณารอสักครู่",
                ["Please click 'Continue' button to proceed"] = "กรุณาคลิกปุ่ม 'ดำเนินการต่อ' เพื่อดำเนินการ",
                ["Ad Completed"] = "ดูโฆษณาแล้ว",
                ["Could not open support link"] = "เปิดลิงก์สนับสนุนไม่ได้",
                ["Could not open help link"] = "เปิดลิงก์ช่วยเหลือไม่ได้",
                ["Could not open Discord link"] = "เปิดลิงก์ Discord ไม่ได้",
                ["Roblox Launch"] = "เปิดตัว Roblox",
                ["Launching Roblox"] = "กำลังเปิดตัว Roblox",
                ["Masterstrap - Loading..."] = "Masterstrap - กำลังโหลด...",
                ["Masterstrap • "] = "Masterstrap • ",
                ["Loading FastFlag configuration..."] = "กำลังโหลดการกำหนดค่า FastFlag...",
                ["Starting Roblox..."] = "กำลังเริ่ม Roblox...",
                ["Waiting for Roblox to open..."] = "กำลังรอ Roblox เปิด...",
                ["Roblox opened. Launch complete."] = "Roblox เปิดแล้ว เปิดตัวเสร็จสมบูรณ์",
                ["Masterstrap deploying auto-apply..."] = "Masterstrap กำลังปรับใช้ auto-apply...",
                ["Waiting for game to be ready..."] = "กำลังรอเกมพร้อม...",
                ["Roblox closed before application."] = "Roblox ปิดก่อนฉีด",
                ["Retrying application ({0}/{1})..."] = "กำลังลองฉีดอีกครั้ง ({0}/{1})...",
                ["Masterstrap auto-applying..."] = "Masterstrap กำลัง auto-apply...",
                ["Auto-applying..."] = "กำลัง auto-apply...",
                ["Waiting for Roblox... ({0}s)"] = "กำลังรอ Roblox... ({0} วินาที)",
                ["Roblox not detected. Closing..."] = "ไม่พบ Roblox กำลังปิด...",
                ["Launch complete."] = "เปิดตัวเสร็จสมบูรณ์",
                ["Applied successfully. Closing..."] = "ฉีดสำเร็จ กำลังปิด...",
                ["Application failed."] = "ฉีดไม่สำเร็จ",
                ["Roblox not detected."] = "ไม่พบ Roblox",
                ["Cancelled."] = "ยกเลิกแล้ว",
                ["Error: {0}"] = "ข้อผิดพลาด: {0}",
                ["Application applied. Waiting for game to apply..."] = "ฉีดแล้ว กำลังรอเกมปรับใช้...",
                ["Applied successfully."] = "ฉีดสำเร็จ",
                ["Failed to launch Roblox: {0}"] = "เปิดตัว Roblox ไม่สำเร็จ: {0}",
                ["Launch Error"] = "ข้อผิดพลาดในการเปิดตัว",
                ["Adsense dialog marked as OPEN"] = "กำหนดไดอะล็อก Adsense เป็น OPEN",
                ["Adsense dialog marked as CLOSED"] = "กำหนดไดอะล็อก Adsense เป็น CLOSED",
                ["FFlags applied successfully!"] = "ฉีด FFlags สำเร็จ!",
                ["⚡ APPLY"] = "⚡ ฉีด",
                ["↩️ UNAPPLY"] = "↩️ ยกเลิกการฉีด",
                ["APPLY"] = "ฉีด",
                ["UNAPPLY"] = "ยกเลิกการฉีด",
                ["Activity Log"] = "บันทึกกิจกรรม",
                ["Clear Log"] = "ล้างบันทึก",
                ["Add"] = "เพิ่ม",
                ["Delete"] = "ลบ",
                ["Clear All"] = "ล้างทั้งหมด",
                ["Export"] = "ส่งออก",
                ["Set as Read-Only"] = "ตั้งค่าเป็นอ่านอย่างเดียว",
                ["Prevent Roblox from overriding global settings."] = "ป้องกันไม่ให้ Roblox เขียนทับการตั้งค่าทั่วไป",
                ["Graphics Quality"] = "คุณภาพกราฟิก",
                ["Set the graphics quality of your game"] = "ตั้งค่าคุณภาพกราฟิกของเกมของคุณ",
                ["Max Quality Enabled"] = "เปิดใช้งานคุณภาพสูงสุด",
                ["Enables maximum graphics quality mode for enhanced visual effects and rendering detail."] = "เปิดโหมดคุณภาพกราฟิกสูงสุดเพื่อเอฟเฟกต์ภาพและรายละเอียดการเรนเดอร์ที่ดีขึ้น",
                ["Graphics Quality Level"] = "ระดับคุณภาพกราฟิก",
                ["Adjusts the in-game graphics quality level from low to maximum."] = "ปรับระดับคุณภาพกราฟิกในเกมจากต่ำสุดถึงสูงสุด",
                ["Framerate Limit"] = "จำกัดเฟรมเรต",
                ["Unlock framerate limit for Roblox. Going above 240 FPS is not recommended."] = "ปลดล็อกการจำกัดเฟรมเรตสำหรับ Roblox ไม่แนะนำให้เกิน 240 FPS",
                ["User Interface and Layout"] = "อินเทอร์เฟซผู้ใช้และเลย์เอาต์",
                ["Transparency"] = "ความโปร่งใส",
                ["Custom transparency for UI elements."] = "ปรับความโปร่งใสสำหรับองค์ประกอบ UI",
                ["Reduced Motion"] = "ลดการเคลื่อนไหว",
                ["Removes the animation on the escape menu."] = "ลบแอนิเมชันในเมนู Escape",
                ["Font Size"] = "ขนาดตัวอักษร",
                ["Choose how large the font should appear."] = "เลือกขนาดตัวอักษรที่ต้องการแสดง",
                ["Default"] = "ค่าเริ่มต้น",
                ["Other"] = "อื่น ๆ",
                ["Mouse Sensitivity"] = "ความไวของเมาส์",
                ["Change how fast the camera will move in-game."] = "เปลี่ยนความเร็วในการเคลื่อนที่ของกล้องภายในเกม",
                ["VR Enabled"] = "เปิดใช้งาน VR",
                ["Player Name Visibility"] = "การแสดงชื่อผู้เล่น",
                ["Hide name tags above other players for a cleaner screen experience."] = "ซ่อนป้ายชื่อเหนือผู้เล่นคนอื่นเพื่อให้หน้าจอดูสะอาดขึ้น",
                ["FAQ and Guide"] = "คำถามที่พบบ่อยและคู่มือ",
                ["How to Use Masterstrap"] = "วิธีใช้ Masterstrap",
                ["1. Load FFlags JSON file"] = "1. โหลดไฟล์ FFlags JSON",
                ["2. Load FFlag Addresses (optional)"] = "2. โหลดที่อยู่ FFlag (ตัวเลือก)",
                ["3. Make sure Roblox is running"] = "3. ตรวจสอบให้แน่ใจว่า Roblox กำลังทำงานอยู่",
                ["4. Click APPLY button to apply FFlags"] = "4. คลิกปุ่ม APPLY เพื่อฉีด FFlags",
                ["5. Check Activity Log for results"] = "5. ตรวจสอบ Activity Log เพื่อดูผลลัพธ์",
                ["Apply"] = "ใช้",
                ["Select Game FFlags Preset"] = "เลือกค่าที่ตั้งไว้ FFlags เกม",
                ["🎮 Select Game FFlags Preset"] = "🎮 เลือกค่าที่ตั้งไว้ FFlags เกม",
                ["How to Edit FFlags"] = "วิธีแก้ไข FFlags",
                ["• Go to Edit tab to modify loaded FFlags"] = "ไปที่แท็บ Edit เพื่อแก้ไข FFlags ที่โหลดแล้ว",
                ["• Click Add to create new FFlag entry"] = "คลิก Add เพื่อสร้างรายการ FFlag ใหม่",
                ["• Click Delete to remove selected FFlag"] = "คลิก Delete เพื่อลบ FFlag ที่เลือก",
                ["• Use Search to find specific FFlags"] = "ใช้ Search เพื่อค้นหา FFlags ที่ต้องการ",
                ["• Click Export to save modified FFlags"] = "คลิก Export เพื่อบันทึก FFlags ที่แก้ไขแล้ว",
                ["Troubleshooting"] = "การแก้ไขปัญหา",
                ["Roblox not found?"] = "ไม่พบ Roblox?",
                ["Make sure Roblox is running before applying"] = "ตรวจสอบให้แน่ใจว่า Roblox กำลังทำงานก่อนทำการฉีด",
                ["Application failed?"] = "การฉีดล้มเหลว?",
                ["Please ensure that your Roblox version matches the version that Masterstrap has requested"] = "โปรดตรวจสอบให้แน่ใจว่าเวอร์ชัน Roblox ของคุณตรงกับเวอร์ชันที่ Masterstrap ต้องการ",
                ["FFlags not loading?"] = "FFlags ไม่โหลด?",
                ["Verify JSON file format is correct and valid"] = "ตรวจสอบให้แน่ใจว่ารูปแบบไฟล์ JSON ถูกต้องและใช้งานได้",
                ["Game crash after applying?"] = "เกมล่มหลังจากฉีด?",
                ["Tips and Tricks"] = "เคล็ดลับและคำแนะนำ",
                ["• Keep your FFlag JSON file backed up"] = "สำรองไฟล์ FFlag JSON ของคุณไว้เสมอ",
                ["• Export frequently to save your changes"] = "ทำการ Export บ่อย ๆ เพื่อบันทึกการเปลี่ยนแปลง",
                ["• Use Search feature to quickly find FFlags"] = "ใช้ฟีเจอร์ Search เพื่อค้นหา FFlags อย่างรวดเร็ว",
                ["• Check Activity Log for application status"] = "ตรวจสอบ Activity Log เพื่อดูสถานะการฉีด",
                ["Information System"] = "ระบบข้อมูล",
                ["INFORMATION SYSTEM"] = "ระบบข้อมูล",
                ["FFlags:"] = "FFlags:",
                ["Count:"] = "จำนวน:",
                ["Roblox Version:"] = "เวอร์ชัน Roblox:",
                ["Software Version:"] = "เวอร์ชันซอฟต์แวร์:",
                ["Last update:"] = "อัปเดตล่าสุด:",
                ["Not loaded"] = "ยังไม่ได้โหลด",
                ["Fast Mode"] = "โหมดด่วน",
                ["Loaded"] = "โหลดแล้ว",
                ["Opening Roblox..."] = "กำลังเปิด Roblox...",
                ["Activity log"] = "บันทึกกิจกรรม",
                ["Activity Log"] = "บันทึกกิจกรรม",
                ["Clear Log"] = "ล้างบันทึก",
                ["0 entries"] = "0 รายการ",
                ["System initialized"] = "เริ่มต้นระบบแล้ว",
                ["Ready to load FFlags"] = "พร้อมโหลด FFlags",
                ["Not set"] = "ยังไม่ได้ตั้งค่า",
                ["Saved FFlags:"] = "FFlags ที่บันทึก:",
                ["Enabled"] = "เปิดใช้งาน",
                ["Disabled"] = "ปิดใช้งาน",
                ["Auto-load FFlags:"] = "โหลด FFlags อัตโนมัติ:",
                ["Auto-load Addresses:"] = "โหลดที่อยู่อัตโนมัติ:",
                ["Not detected"] = "ไม่พบ",
                ["Roblox Version:"] = "เวอร์ชัน Roblox:",
                ["Unknown"] = "ไม่ทราบ",
                ["Software Version:"] = "เวอร์ชันซอฟต์แวร์:",
                ["Version Compatibility:"] = "ความเข้ากันได้ของเวอร์ชัน:",
                ["MATCH"] = "ตรงกัน",
                ["MISMATCH"] = "ไม่ตรงกัน",
                ["UNKNOWN"] = "ไม่ทราบ",
                ["Application successful ({0} FFlags)"] = "ฉีดสำเร็จ ({0} FFlags)",
                ["Application failed ({0} errors)"] = "ฉีดล้มเหลว ({0} ข้อผิดพลาด)",
                ["now"] = "เมื่อสักครู่",
                ["s ago"] = " วินาทีที่แล้ว",
                ["m ago"] = " นาทีที่แล้ว",
                ["Success"] = "สำเร็จ",
                ["Failed"] = "ล้มเหลว",
                ["Pending"] = "รอดำเนินการ",
                ["Mixed"] = "ผสม",
                ["Status"] = "สถานะ",
                ["Session"] = "เซสชัน",
                ["Last"] = "ล่าสุด",
                ["Actions"] = "การดำเนินการ",
                ["Activity log cleared"] = "ล้างบันทึกกิจกรรมแล้ว",
                ["JSON file not found"] = "ไม่พบไฟล์ JSON",
                ["JSON Content Preview:"] = "ตัวอย่างเนื้อหา JSON:",
                ["... and {0} more entries"] = "... และอีก {0} รายการ",
                ["Total entries in JSON: {0}"] = "รายการทั้งหมดใน JSON: {0}",
                ["Invalid JSON format"] = "รูปแบบ JSON ไม่ถูกต้อง",
                ["Error parsing JSON"] = "ข้อผิดพลาดในการแยก JSON",
                [" Loading FFlag addresses..."] = " กำลังโหลดที่อยู่ FFlag...",
                [" Auto-loading FFlag addresses..."] = " กำลังโหลดที่อยู่ FFlag อัตโนมัติ...",
                ["Idle"] = "ไม่ได้ใช้งาน",
                ["Join"] = "เข้าร่วม",
                ["Join Discord"] = "เข้าร่วม Discord",
                ["Made By ©Dank1ngs"] = "สร้างโดย ©Dank1ngs",
                ["Home"] = "หน้าแรก",
                ["Global"] = "ทั่วโลก",
                ["Games"] = "เกม",
                ["Settings"] = "การตั้งค่า",
                ["FAQ"] = "คำถามที่พบบ่อย",
                ["Thai"] = "ภาษาไทย",
                ["Chinese"] = "จีน",
                ["English"] = "English",
                ["Vietnamese"] = "ภาษาเวียดนาม",
                ["Filipino"] = "ฟิลิปปินส์",
                ["Indonesian"] = "อินโดนีเซีย",
                ["Portuguese"] = "โปรตุเกส",
                ["Malay"] = "มาเลย์",
                ["Japanese"] = "ญี่ปุ่น",
                ["Physics"] = "ฟิสิกส์",
                ["Audio"] = "เสียง",
                ["0 entries"] = "0 รายการ",
                ["Presets"] = "ค่าที่ตั้งไว้",
                ["Graphic advanced"] = "กราฟิกขั้นสูง",
                ["Lowest quality"] = "คุณภาพต่ำสุด",
                ["Highest quality"] = "คุณภาพสูงสุด",
                ["Open the editor to view and change flags, use presets, and choose whether Masterstrap may apply them when you launch."] = "เปิดตัวแก้ไขเพื่อดูและเปลี่ยนแปลงแฟล็ก ใช้พรีเซ็ต และเลือกว่า Masterstrap จะนำไปใช้เมื่อคุณเปิดเกมหรือไม่",
                ["Adjust Roblox wide settings such as read only mode, rendering, and framerate limits."] = "ปรับการตั้งค่าทั่วทั้ง Roblox เช่น โหมดอ่านอย่างเดียว การเรนเดอร์ และขีดจำกัดเฟรมเรต",
                ["Edit the flag table, filter by category, search, then use Back and Save on the FastFlags page when you are done."] = "แก้ไขตารางแฟล็ก กรองตามหมวด ค้นหา แล้วใช้กลับและบันทึกในหน้า FastFlags เมื่อเสร็จ",
                ["Choose language, visual theme, and startup behavior for the app."] = "เลือกภาษา ธีมภาพ และพฤติกรรมเมื่อเริ่มแอป",
                ["Load a curated flag set for a game into your list, then adjust or save as usual."] = "โหลดชุดแฟล็กที่คัดสรรสำหรับเกมลงในรายการของคุณ แล้วปรับหรือบันทึกตามปกติ",
                ["Credits, FAQ, and short guides for using Masterstrap."] = "เครดิต คำถามที่พบบ่อย และคู่มือสั้นๆ สำหรับใช้ Masterstrap"
            }), AboutTabUiTranslations.EnToTh),
            MergeUiOverlays(MergeUiOverlays(BuildFlagUiTranslations.EnToTh, LaunchProgressUiTranslations.EnToTh), DialogsUiTranslations.EnToTh));

        private static readonly Dictionary<string, string> EnToKm = OverlayAboutTab(
            OverlayAboutTab(
            MergeTranslations(
            LoadMdTranslations("Masterstrap.Resources.Khmer.md"),
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Auto-check for updates on startup (recommended)"] = "ពិនិត្យការអាប់ដេតស្វ័យប្រវត្តិនៅពេលចាប់ផ្តើម (ណែនាំ)",
                ["Auto-check for updates on startup"] = "ពិនិត្យការអាប់ដេតនៅពេលចាប់ផ្តើម",
                ["Load FFlags JSON"] = "ផ្ទុក FFlags JSON",
                ["Load FFlag Addresses"] = "ផ្ទុកអាសយដ្ឋាន FFlag",
                ["📁 Load FFlags JSON"] = "📁 ផ្ទុក FFlags JSON",
                ["📄 Load FFlag Addresses"] = "📄 ផ្ទុកអាសយដ្ឋាន FFlag",
                ["Add New FFlag"] = "បន្ថែម FFlag ថ្មី",
                ["Add New FFlags"] = "បន្ថែម FFlags ថ្មី",
                ["Add or batch import new flags to your library"] = "បន្ថែម ឬនាំចូលជាក្រុមទង់ថ្មីទៅបណ្ណាល័យរបស់អ្នក",
                ["Flag Editor"] = "កម្មវិធីកែសម្រួលទង់",
                ["FLAG EDITOR"] = "កម្មវិធីកែសម្រួលទង់",
                ["Enter flags manually or load from JSON file"] = "បញ្ចូលទង់ដោយដៃ ឬផ្ទុកពីឯកសារ JSON",
                ["FORMAT: name: value"] = "ទម្រង់៖ ឈ្មោះ៖ តម្លៃ",
                ["Each line = 1 FFlags"] = "រាយការណ៍មួយ = 1 FFlag",
                ["(Each line = 1 FFlag. Example: MyFlag: true)"] = "(រាយការណ៍មួយ = 1 FFlag ឧទាហរណ៍៖ MyFlag: true)",
                ["Ready to add flags"] = "រួចរាល់បន្ថែមទង់",
                ["Configuration saved successfully!"] = "រក្សាទុកការកំណត់ជោគជ័យ!",
                ["Configuration saved successfully"] = "រក្សាទុកការកំណត់ជោគជ័យ",
                ["CompleteAdsenseDialog created successfully"] = "បង្កើត CompleteAdsenseDialog ជោគជ័យ",
                ["Complete the adsense to continue"] = "បំពេញផ្នែកផ្សាយដំណឹងដើម្បីបន្ត",
                ["Support"] = "គាំទ្រ",
                ["Please look at and click on ads so this software project can continue for free"] = "សូមមើល និងចុចលើផ្សាយដំណឹង ដើម្បីឱ្យគម្រោងកម្មវិធីនេះបន្តឥតគិតថ្លៃ",
                ["⏭ Skip ad 3:00"] = "⏭ រំលងផ្សាយដំណឹង 3:00",
                ["Skip ad"] = "រំលងផ្សាយដំណឹង",
                ["How to skip ad? "] = "របៀបរំលងផ្សាយដំណឹង? ",
                ["Click here"] = "ចុចទីនេះ",
                ["✓ Continue"] = "✓ បន្ត",
                ["✓ Ad Complete"] = "✓ ផ្សាយដំណឹងរួចរាល់",
                ["Ad Complete"] = "ផ្សាយដំណឹងរួចរាល់",
                ["Please wait for the countdown to finish"] = "សូមរងចាំរហូតដល់ការរាប់ថយចប់",
                ["Please wait"] = "សូមរងចាំ",
                ["Please click 'Continue' button to proceed"] = "សូមចុចប៊ូតុង 'បន្ត' ដើម្បីបន្ត",
                ["Ad Completed"] = "ផ្សាយដំណឹងរួចរាល់",
                ["Could not open support link"] = "មិនអាចបើកតំណភ្ជាប់គាំទ្រ",
                ["Could not open help link"] = "មិនអាចបើកតំណភ្ជាប់ជំនួយ",
                ["Could not open Discord link"] = "មិនអាចបើកតំណភ្ជាប់ Discord",
                ["Roblox Launch"] = "ចាប់ផ្តើម Roblox",
                ["Launching Roblox"] = "កំពុងចាប់ផ្តើម Roblox",
                ["Masterstrap - Loading..."] = "Masterstrap - កំពុងផ្ទុក...",
                ["Masterstrap • "] = "Masterstrap • ",
                ["Loading FastFlag configuration..."] = "កំពុងផ្ទុកការកំណត់ FastFlag...",
                ["Starting Roblox..."] = "កំពុងចាប់ផ្តើម Roblox...",
                ["Waiting for Roblox to open..."] = "កំពុងរងចាំ Roblox បើក...",
                ["Roblox opened. Launch complete."] = "Roblox បើករួច។ ចាប់ផ្តើមរួចរាល់។",
                ["Masterstrap deploying auto-apply..."] = "Masterstrap កំពុងចាកចេញ auto-apply...",
                ["Waiting for game to be ready..."] = "រងចាំឲ្យហ្គេមរួចរាល់...",
                ["Roblox closed before application."] = "Roblox បិទមុនចាកចេញ។",
                ["Retrying application ({0}/{1})..."] = "ព្យាយាមចាកចេញម្តងទៀត ({0}/{1})...",
                ["Masterstrap auto-applying..."] = "Masterstrap កំពុង auto-apply...",
                ["Auto-applying..."] = "កំពុង auto-apply...",
                ["Waiting for Roblox... ({0}s)"] = "រងចាំ Roblox... ({0}s)",
                ["Roblox not detected. Closing..."] = "មិនរកឃើញ Roblox។ កំពុងបិទ...",
                ["Launch complete."] = "ចាប់ផ្តើមរួចរាល់។",
                ["Applied successfully. Closing..."] = "ចាកចេញជោគជ័យ។ កំពុងបិទ...",
                ["Application failed."] = "ចាកចេញមិនជោគជ័យ។",
                ["Roblox not detected."] = "មិនរកឃើញ Roblox។",
                ["Cancelled."] = "បានលុបចោល។",
                ["Error: {0}"] = "កំហុស៖ {0}",
                ["Application applied. Waiting for game to apply..."] = "ចាកចេញអនុវត្តរួច។ រងចាំឲ្យហ្គេមអនុវត្ត...",
                ["Applied successfully."] = "ចាកចេញជោគជ័យ។",
                ["Failed to launch Roblox: {0}"] = "ចាប់ផ្តើម Roblox មិនជោគជ័យ៖ {0}",
                ["Launch Error"] = "កំហុសចាប់ផ្តើម",
                ["Adsense dialog marked as OPEN"] = "ក្របអេក្រង់ Adsense ត្រូវបានគូសចំណាំថា បើក",
                ["Adsense dialog marked as CLOSED"] = "ក្របអេក្រង់ Adsense ត្រូវបានគូសចំណាំថា បិទ",
                ["FFlags applied successfully!"] = "ចាក់ចូល FFlags ជោគជ័យ!",
                ["⚡ APPLY"] = "⚡ ចាក់ចូល",
                ["↩️ UNAPPLY"] = "↩️ ដកចាក់ចូល",
                ["APPLY"] = "ចាក់ចូល",
                ["UNAPPLY"] = "ដកចាក់ចូល",
                ["Save and Launch"] = "រក្សាទុក និងចាប់ផ្តើម",
                ["Save"] = "រក្សាទុក",
                ["Close"] = "បិទ",
                ["Activity Log"] = "កំណត់ហេតុសកម្មភាព",
                ["Clear Log"] = "សំអាតកំណត់ហេតុ",
                ["Add"] = "បន្ថែម",
                ["Delete"] = "លុប",
                ["Clear All"] = "សំអាតទាំងអស់",
                ["Export"] = "នាំចេញ",
                [" (recommended)"] = " (ណែនាំ)",
                ["Language Settings"] = "ការកំណត់ភាសា",
                ["Select your preferred display language for the application interface."] = "ជ្រើសរើសភាសាបង្ហាញដែលអ្នកចូលចិត្តសម្រាប់ចំណុចប្រទាក់កម្មវិធី។",
                ["Home"] = "ផ្ទះ",
                ["Global"] = "សាកល",
                ["Games"] = "ហ្គេម",
                ["Settings"] = "ការកំណត់",
                ["FAQ"] = "សំណួរញឹកញាប់",
                ["Chinese"] = "ចិន",
                ["English"] = "English",
                ["Vietnamese"] = "វៀតណាម",
                ["Filipino"] = "ហ្វីលីពីន",
                ["Indonesian"] = "ឥណ្ឌូណេស៊ី",
                ["Portuguese"] = "ព័រទុយហ្គាល់",
                ["Malay"] = "ម៉ាឡេ",
                ["Japanese"] = "ជប៉ុន",
                ["Thai"] = "ថៃ",
                ["Khmer"] = "កម្ពុជា",
                ["Physics"] = "រូបវិទ្យា",
                ["Audio"] = "សំឡេង",
                ["Graphics"] = "ក្រាហ្វិក",
                ["Internet"] = "អ៊ីនធឺណិត",
                ["Search"] = "ស្វែងរក",
                ["Filter:"] = "ចម្រោះ៖",
                ["Cancel"] = "បោះបង់",
                ["Apply"] = "អនុវត្ត",
                ["Select Game FFlags Preset"] = "ជ្រើសរើសការកំណត់រួម FFlags ហ្គេម",
                ["🎮 Select Game FFlags Preset"] = "🎮 ជ្រើសរើសការកំណត់រួម FFlags ហ្គេម",
                ["Not loaded"] = "មិនបានផ្ទុក",
                ["Loaded"] = "បានផ្ទុក",
                ["Opening Roblox..."] = "កំពុងបើក Roblox...",
                ["Idle"] = "អសកម្ម",
                ["Fast Mode"] = "របៀបរហ័ស",
                ["Join"] = "ចូលរួម",
                ["Join Discord"] = "ចូលរួម Discord",
                ["Made By ©Dank1ngs"] = "ធ្វើឡើងដោយ ©Dank1ngs",
                ["Presets"] = "ការកំណត់រួម",
                ["0 entries"] = "០ ធាតុ",
                ["Count:"] = "ចំនួន៖",
                ["Roblox Version:"] = "កំណែ Roblox៖",
                ["Software Version:"] = "កំណែកម្មវិធី៖",
                ["Last update:"] = "អាប់ដេតចុងក្រោយ៖",
                ["FFlags:"] = "FFlags៖",
                ["Information System"] = "ប្រព័ន្ធព័ត៌មាន",
                ["INFORMATION SYSTEM"] = "ប្រព័ន្ធព័ត៌មាន",
                ["Don't Save"] = "មិនរក្សាទុក",
                ["Unsaved Changes"] = "ការផ្លាស់ប្តូរមិនបានរក្សាទុក",
                ["You have unsaved changes. Do you want to save before exiting?"] = "អ្នកមានការផ្លាស់ប្តូរមិនបានរក្សាទុក។ តើអ្នកចង់រក្សាទុកមុនចាកចេញទេ?",
                ["Ready"] = "រួចរាល់",
                ["Initializing..."] = "កំពុងចាប់ផ្តើម...",
                ["Loading..."] = "កំពុងផ្ទុក...",
                ["Applying..."] = "កំពុងចាក់ចូល...",
                ["Back"] = "ត្រឡប់",
                ["← Back"] = "← ត្រឡប់",
                ["All"] = "ទាំងអស់",
                ["Default"] = "លំនាំដើម",
                ["Other"] = "ផ្សេងៗ",
                ["Set as Read-Only"] = "កំណត់ជាអានតែប៉ុណ្ណោះ",
                ["Prevent Roblox from overriding global settings."] = "រារាំង Roblox ពីការបដិសេធការកំណត់សាកល។",
                ["How to Use Masterstrap"] = "របៀបប្រើ Masterstrap",
                ["1. Load FFlags JSON file"] = "១. ផ្ទុកឯកសារ FFlags JSON",
                ["2. Load FFlag Addresses (optional)"] = "២. ផ្ទុកអាសយដ្ឋាន FFlag (ជម្រើស)",
                ["3. Make sure Roblox is running"] = "៣. ត្រូវប្រាកដថា Roblox កំពុងដំណើរការ",
                ["4. Click APPLY button to apply FFlags"] = "៤. ចុចប៊ូតុង APPLY ដើម្បីចាក់ចូល FFlags",
                ["5. Check Activity Log for results"] = "៥. ពិនិត្យកំណត់ហេតុសកម្មភាពដើម្បីមើលលទ្ធផល",
                ["How to Edit FFlags"] = "របៀបកែសម្រួល FFlags",
                ["• Go to Edit tab to modify loaded FFlags"] = "ទៅផ្ទាំង Edit ដើម្បីកែសម្រួល FFlags ដែលបានផ្ទុក",
                ["• Click Add to create new FFlag entry"] = "ចុច Add ដើម្បីបង្កើតធាតុ FFlag ថ្មី",
                ["• Click Delete to remove selected FFlag"] = "ចុច Delete ដើម្បីយក FFlag ដែលជ្រើសចេញ",
                ["• Use Search to find specific FFlags"] = "ប្រើ Search ដើម្បីរក FFlags ជាក់លាក់",
                ["• Click Export to save modified FFlags"] = "ចុច Export ដើម្បីរក្សាទុក FFlags ដែលបានកែសម្រួល",
                ["Troubleshooting"] = "ការដោះស្រាយបញ្ហា",
                ["Roblox not found?"] = "រកមិនឃើញ Roblox?",
                ["Make sure Roblox is running before applying"] = "ត្រូវប្រាកដថា Roblox កំពុងដំណើរការមុនចាក់ចូល",
                ["Application failed?"] = "ការចាក់ចូលបរាជ័យ?",
                ["Please ensure that your Roblox version matches the version that Masterstrap has requested"] = "សូមធានាថាកំណែ Roblox របស់អ្នកត្រូវគ្នានឹងកំណែដែល Masterstrap ស្នើសុំ។",
                ["FFlags not loading?"] = "FFlags មិនផ្ទុក?",
                ["Verify JSON file format is correct and valid"] = "ផ្ទៀងផ្ទាត់ថាទម្រង់ឯកសារ JSON ត្រឹមត្រូវ និងមានសុពលភាព។",
                ["Game crash after applying?"] = "ហ្គេមរលំបន្ទាប់ពីចាក់ចូល?",
                ["Tips and Tricks"] = "គន្លឹះ និងល្បិច",
                ["• Keep your FFlag JSON file backed up"] = "រក្សាទុកឯកសារ FFlag JSON របស់អ្នកជាប្រធាន",
                ["• Export frequently to save your changes"] = "ធ្វើ Export ញឹកញាប់ដើម្បីរក្សាទុកការផ្លាស់ប្តូររបស់អ្នក",
                ["• Use Search feature to quickly find FFlags"] = "ប្រើមុខងារ Search ដើម្បីរក FFlags យ៉ាងរហ័ស",
                ["• Check Activity Log for application status"] = "ពិនិត្យកំណត់ហេតុសកម្មភាពដើម្បីមើលស្ថានភាពចាក់ចូល",
                ["Auto-load FFlags on startup (recommended)"] = "ផ្ទុក FFlags ស្វ័យប្រវត្តិនៅពេលចាប់ផ្តើម (ណែនាំ)",
                ["Auto-load FFlags on startup"] = "ផ្ទុក FFlags ស្វ័យប្រវត្តិនៅពេលចាប់ផ្តើម",
                ["Auto-load Cache on startup (recommended)"] = "ផ្ទុក Cache ស្វ័យប្រវត្តិនៅពេលចាប់ផ្តើម (ណែនាំ)",
                ["Auto-load Cache on startup"] = "ផ្ទុក Cache ស្វ័យប្រវត្តិនៅពេលចាប់ផ្តើម",
                ["Auto-apply when Roblox is detected (recommended)"] = "ចាក់ចូលស្វ័យប្រវត្តិនៅពេលរកឃើញ Roblox (ណែនាំ)",
                ["Minimize to system tray"] = "បង្រួមទៅរបារប្រព័ន្ធ",
                ["Optimizer"] = "ឧបករណ៍បង្កើនប្រសិទ្ធភាព",
                ["Auto-cleanup temp files (recommended)"] = "សំអាតឯកសារបណ្តោះអាសន្នស្វ័យប្រវត្តិ (ណែនាំ)",
                ["Auto-cleanup temp files"] = "សំអាតឯកសារបណ្តោះអាសន្ន",
                ["Memory optimization (recommended)"] = "ការបង្កើនប្រសិទ្ធភាពអង្គចងចាំ (ណែនាំ)",
                ["Memory optimization"] = "ការបង្កើនប្រសិទ្ធភាពអង្គចងចាំ",
                ["Graphics Quality"] = "គុណភាពក្រាហ្វិក",
                ["Set the graphics quality of your game"] = "កំណត់គុណភាពក្រាហ្វិកនៃហ្គេមរបស់អ្នក",
                ["Max Quality Enabled"] = "បើកគុណភាពអតិបរមា",
                ["Enables maximum graphics quality mode for enhanced visual effects and rendering detail."] = "បើករបៀបគុណភាពក្រាហ្វិកអតិបរមាសម្រាប់ផលរូបភាព និងរូបភាពលម្អិត។",
                ["Graphics Quality Level"] = "កម្រិតគុណភាពក្រាហ្វិក",
                ["Adjusts the in-game graphics quality level from low to maximum."] = "កែធ្វើមាត្រដានគុណភាពក្រាហ្វិកក្នុងហ្គេមពីទាបដល់អតិបរមា។",
                ["Framerate Limit"] = "កំណត់អត្រាហ្វ្រេម",
                ["Unlock framerate limit for Roblox. Going above 240 FPS is not recommended."] = "ដោះសោកំណត់អត្រាហ្វ្រេមសម្រាប់ Roblox។ ការលើស 240 FPS មិនត្រូវបានណែនាំទេ។",
                ["User Interface and Layout"] = "ចំណុចប្រទាក់អ្នកប្រើ និងប្លង់",
                ["Transparency"] = "ភាពថ្លា",
                ["Custom transparency for UI elements."] = "ភាពថ្លាតាមចិត្តសម្រាប់ធាតុ UI។",
                ["Reduced Motion"] = "ការកម្រើកថយចុះ",
                ["Removes the animation on the escape menu."] = "យកចក្ខុវិស័យចេញពីម៉ឺនុយចាកចេញ។",
                ["Font Size"] = "ទំហំអក្សរ",
                ["Choose how large the font should appear."] = "ជ្រើសរើសទំហំអក្សរដែលអ្នកចង់បង្ហាញ។",
                ["Mouse Sensitivity"] = "ភាពរសើបរបស់កណ្តុរ",
                ["Change how fast the camera will move in-game."] = "ផ្លាស់ប្តូរល្បឿនកាមេរ៉ាផ្លាស់ទីក្នុងហ្គេម។",
                ["VR Enabled"] = "បើក VR",
                ["Player Name Visibility"] = "ភាពមើលឃើញឈ្មោះអ្នកលេង",
                ["Hide name tags above other players for a cleaner screen experience."] = "លាក់ប្លាកឈ្មោះពីលើអ្នកលេងដទៃដើម្បីរូបភាពអេក្រង់ស្អាត។",
                ["Activity log"] = "កំណត់ហេតុសកម្មភាព",
                ["Activity Log"] = "កំណត់ហេតុសកម្មភាព",
                ["Clear Log"] = "សំអាតកំណត់ហេតុ",
                ["0 entries"] = "០ ធាតុ",
                ["System initialized"] = "ប្រព័ន្ធបានចាប់ផ្តើម",
                ["Ready to load FFlags"] = "រួចរាល់ផ្ទុក FFlags",
                ["Not set"] = "មិនបានកំណត់",
                ["Saved FFlags:"] = "FFlags ដែលបានរក្សាទុក:",
                ["Enabled"] = "បានបើក",
                ["Disabled"] = "បានបិទ",
                ["Auto-load FFlags:"] = "ផ្ទុក FFlags ស្វ័យប្រវត្តិ:",
                ["Auto-load Addresses:"] = "ផ្ទុកអាសយដ្ឋានស្វ័យប្រវត្តិ:",
                ["Not detected"] = "មិនបានរកឃើញ",
                ["Roblox Version:"] = "កំណែ Roblox:",
                ["Unknown"] = "មិនស្គាល់",
                ["Software Version:"] = "កំណែកម្មវិធី:",
                ["Version Compatibility:"] = "ភាពឆបគ្នាកំណែ:",
                ["MATCH"] = "ផ្គូផ្គង",
                ["MISMATCH"] = "មិនផ្គូផ្គង",
                ["UNKNOWN"] = "មិនស្គាល់",
                ["Application successful ({0} FFlags)"] = "ចាក់ចូលជោគជ័យ ({0} FFlags)",
                ["Application failed ({0} errors)"] = "ចាក់ចូលបរាជ័យ ({0} កំហុស)",
                ["now"] = "ឥឡូវ",
                ["s ago"] = " វិនាទីមុន",
                ["m ago"] = " នាទីមុន",
                ["Success"] = "ជោគជ័យ",
                ["Failed"] = "បរាជ័យ",
                ["Pending"] = "រងចាំ",
                ["Mixed"] = "ចម្រុះ",
                ["Status"] = "ស្ថានភាព",
                ["Session"] = "វគ្គ",
                ["Last"] = "ចុងក្រោយ",
                ["Actions"] = "សកម្មភាព",
                ["Activity log cleared"] = "បានសំអាតកំណត់ហេតុសកម្មភាព",
                ["JSON file not found"] = "រកមិនឃើញឯកសារ JSON",
                ["JSON Content Preview:"] = "មើលការមុនខ្លឹមសារ JSON:",
                ["... and {0} more entries"] = "... និង {0} ធាតុទៀត",
                ["Total entries in JSON: {0}"] = "ចំនួនធាតុក្នុង JSON: {0}",
                ["Invalid JSON format"] = "ទម្រង់ JSON មិនត្រឹមត្រូវ",
                ["Error parsing JSON"] = "កំហុសរក្សាយ JSON",
                [" Loading FFlag addresses..."] = " កំពុងផ្ទុកអាសយដ្ឋាន FFlag...",
                [" Auto-loading FFlag addresses..."] = " កំពុងផ្ទុកអាសយដ្ឋាន FFlag ស្វ័យប្រវត្តិ...",
                ["Open the editor to view and change flags, use presets, and choose whether Masterstrap may apply them when you launch."] = "បើកកម្មវិធីកែសម្រួលដើម្បីមើល និងផ្លាស់ប្តូរទង់ ប្រើការកំណត់រួម និងជ្រើសរើសថាតើ Masterstrap អាចអនុវត្តពួកវានៅពេលអ្នកចាប់ផ្តើមឬទេ។",
                ["Adjust Roblox wide settings such as read only mode, rendering, and framerate limits."] = "កែតម្រូវការកំណត់ទូទាំង Roblox ដូចជារបៀបអានតែប៉ុណ្ណោះ ការបង្ហាញរូប និងដែនកំណត់អត្រាស៊ុយ។",
                ["Edit the flag table, filter by category, search, then use Back and Save on the FastFlags page when you are done."] = "កែតារាងទង់ ចម្រោះតាមប្រភេទ ស្វែងរក បន្ទាប់មកប្រើត្រឡប់ និងរក្សាទុកនៅទំព័រ FastFlags នៅពេលរួចរាល់។",
                ["Choose language, visual theme, and startup behavior for the app."] = "ជ្រើសរើសភាសា ស្បែករូបភាព និងអាកប្បកិរិយាពេលចាប់ផ្តើមកម្មវិធី។",
                ["Load a curated flag set for a game into your list, then adjust or save as usual."] = "ផ្ទុកឈុតទង់ដែលបានជ្រើសសម្រាប់ហ្គេមទៅក្នុងបញ្ជីរបស់អ្នក បន្ទាប់មកកែសម្រួល ឬរក្សាទុកដូចធម្មតា។",
                ["Credits, FAQ, and short guides for using Masterstrap."] = "ក្រេឌីត សំណួរញឹកញាប់ និងមគ្គុទ្ទេសក៍ខ្លីសម្រាប់ប្រើ Masterstrap។",
                ["Graphic advanced"] = "ក្រាហ្វិកកម្រិតខ្ពស់",
                ["Lowest quality"] = "គុណភាពទាបបំផុត",
                ["Highest quality"] = "គុណភាពខ្ពស់បំផុត"
            }), AboutTabUiTranslations.EnToKm),
            MergeUiOverlays(MergeUiOverlays(BuildFlagUiTranslations.EnToKm, LaunchProgressUiTranslations.EnToKm), DialogsUiTranslations.EnToKm));

        private static readonly Dictionary<string, string> EnToLo = OverlayAboutTab(
            OverlayAboutTab(
            MergeTranslations(
            LoadMdTranslations("Masterstrap.Resources.Lao.md"),
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Auto-check for updates on startup (recommended)"] = "ກວດອັບເດດອັດຕະໂນມັດເມື່ອເປີດໃຊ້ (ແນະນຳ)",
                ["Auto-check for updates on startup"] = "ກວດອັບເດດເມື່ອເປີດໃຊ້",
                ["Load FFlags JSON"] = "ໂຫຼດ FFlags JSON",
                ["Load FFlag Addresses"] = "ໂຫຼດທີ່ຢູ່ FFlag",
                ["📁 Load FFlags JSON"] = "📁 ໂຫຼດ FFlags JSON",
                ["📄 Load FFlag Addresses"] = "📄 ໂຫຼດທີ່ຢູ່ FFlag",
                ["Add New FFlag"] = "ເພີ່ມ FFlag ໃໝ່",
                ["Add New FFlags"] = "ເພີ່ມ FFlags ໃໝ່",
                ["Add or batch import new flags to your library"] = "ເພີ່ມ ຫຼືນຳເຂົ້າຊຸດທຸງໃໝ່ເຂົ້າຄັງສະໝຸດຂອງທ່ານ",
                ["Flag Editor"] = "ເຄື່ອງແກ້ໄຂທຸງ",
                ["FLAG EDITOR"] = "ເຄື່ອງແກ້ໄຂທຸງ",
                ["Enter flags manually or load from JSON file"] = "ປ້ອນທຸງດ້ວຍມື ຫຼືໂຫຼດຈາກໄຟລ໌ JSON",
                ["FORMAT: name: value"] = "ຮູບແບບ: ຊື່: ຄ່າ",
                ["Each line = 1 FFlags"] = "ແຕ່ລະແຖວ = 1 FFlag",
                ["(Each line = 1 FFlag. Example: MyFlag: true)"] = "(ແຕ່ລະແຖວ = 1 FFlag ຕົວຢ່າງ: MyFlag: true)",
                ["Ready to add flags"] = "ພ້ອມເພີ່ມທຸງ",
                ["Configuration saved successfully!"] = "ບັນທຶກການຕັ້ງຄ່າສຳເລັດ!",
                ["Configuration saved successfully"] = "ບັນທຶກການຕັ້ງຄ່າສຳເລັດ",
                ["CompleteAdsenseDialog created successfully"] = "ສ້າງ CompleteAdsenseDialog ສຳເລັດ",
                ["Complete the adsense to continue"] = "ເຮັດໃຫ້ສຳເລັດການໂຄສະນາເພື່ອດຳເນີນການຕໍ່",
                ["Support"] = "ສະໜັບສະໜູນ",
                ["Please look at and click on ads so this software project can continue for free"] = "ກະລຸນາເບິ່ງ ແລະຄລິກໂຄສະນາເພື່ອໃຫ້ໂຄງການຊອບແວນີ້ສາມາດດຳເນີນຕໍ່ໄດ້ຟຣີ",
                ["⏭ Skip ad 3:00"] = "⏭ ຂ້າມໂຄສະນາ 3:00",
                ["Skip ad"] = "ຂ້າມໂຄສະນາ",
                ["How to skip ad? "] = "ວິທີຂ້າມໂຄສະນາ? ",
                ["Click here"] = "ຄລິກທີ່ນີ້",
                ["✓ Continue"] = "✓ ດຳເນີນການຕໍ່",
                ["✓ Ad Complete"] = "✓ ເບິ່ງໂຄສະນາແລ້ວ",
                ["Ad Complete"] = "ເບິ່ງໂຄສະນາແລ້ວ",
                ["Please wait for the countdown to finish"] = "ກະລຸນາລໍຖ້າຈົນກວ່າການນັບຖອຍຫຼັງຈະສຳເລັດ",
                ["Please wait"] = "ກະລຸນາລໍຖ້າ",
                ["Please click 'Continue' button to proceed"] = "ກະລຸນາຄລິກປຸ່ມ 'ດຳເນີນການຕໍ່' ເພື່ອດຳເນີນການ",
                ["Ad Completed"] = "ເບິ່ງໂຄສະນາແລ້ວ",
                ["Could not open support link"] = "ເປີດລິ້ງສະໜັບສະໜູນບໍ່ໄດ້",
                ["Could not open help link"] = "ເປີດລິ້ງຄວາມຊ່ວຍເຫຼືອບໍ່ໄດ້",
                ["Could not open Discord link"] = "ເປີດລິ້ງ Discord ບໍ່ໄດ້",
                ["Roblox Launch"] = "ເປີດໃຊ້ Roblox",
                ["Launching Roblox"] = "ກຳລັງເປີດໃຊ້ Roblox",
                ["Masterstrap - Loading..."] = "Masterstrap - ກຳລັງໂຫຼດ...",
                ["Masterstrap • "] = "Masterstrap • ",
                ["Loading FastFlag configuration..."] = "ກຳລັງໂຫຼດການຕັ້ງຄ່າ FastFlag...",
                ["Starting Roblox..."] = "ກຳລັງເລີ່ມ Roblox...",
                ["Waiting for Roblox to open..."] = "ກຳລັງລໍຖ້າ Roblox ເປີດ...",
                ["Roblox opened. Launch complete."] = "Roblox ເປີດແລ້ວ. ເປີດໃຊ້ສຳເລັດ.",
                ["Masterstrap deploying auto-apply..."] = "Masterstrap ກຳລັງແຈກຢາຍ auto-apply...",
                ["Waiting for game to be ready..."] = "ລໍຖ້າເກມພ້ອມ...",
                ["Roblox closed before application."] = "Roblox ປິດກ່ອນແຊັກ.",
                ["Retrying application ({0}/{1})..."] = "ລອງແຊັກອີກ ({0}/{1})...",
                ["Masterstrap auto-applying..."] = "Masterstrap ກຳລັງ auto-apply...",
                ["Auto-applying..."] = "ກຳລັງ auto-apply...",
                ["Waiting for Roblox... ({0}s)"] = "ລໍຖ້າ Roblox... ({0}s)",
                ["Roblox not detected. Closing..."] = "ບໍ່ພົບ Roblox. ກຳລັງປິດ...",
                ["Launch complete."] = "ເປີດໃຊ້ສຳເລັດ.",
                ["Applied successfully. Closing..."] = "ແຊັກສຳເລັດ. ກຳລັງປິດ...",
                ["Application failed."] = "ແຊັກບໍ່ສຳເລັດ.",
                ["Roblox not detected."] = "ບໍ່ພົບ Roblox.",
                ["Cancelled."] = "ຍົກເລີກແລ້ວ.",
                ["Error: {0}"] = "ຜິດພາດ: {0}",
                ["Application applied. Waiting for game to apply..."] = "ແຊັກແລ້ວ. ລໍຖ້າເກມນຳໃຊ້...",
                ["Applied successfully."] = "ແຊັກສຳເລັດ.",
                ["Failed to launch Roblox: {0}"] = "ເປີດໃຊ້ Roblox ບໍ່ສຳເລັດ: {0}",
                ["Launch Error"] = "ຜິດພາດເປີດໃຊ້",
                ["Adsense dialog marked as OPEN"] = "ເຄື່ອງແບບ Adsense ຖືກໝາຍວ່າ ເປີດ",
                ["Adsense dialog marked as CLOSED"] = "ເຄື່ອງແບບ Adsense ຖືກໝາຍວ່າ ປິດ",
                ["FFlags applied successfully!"] = "ສັກຢັບ FFlags ສຳເລັດ!",
                ["⚡ APPLY"] = "⚡ ສັກຢັບ",
                ["↩️ UNAPPLY"] = "↩️ ຖອນສັກຢັບ",
                ["APPLY"] = "ສັກຢັບ",
                ["UNAPPLY"] = "ຖອນສັກຢັບ",
                ["Save and Launch"] = "ບັນທຶກ ແລະ ເປີດໃຊ້",
                ["Save"] = "ບັນທຶກ",
                ["Close"] = "ປິດ",
                ["Activity Log"] = "ບັນທຶກກິດຈະກຳ",
                ["Clear Log"] = "ລ້າງບັນທຶກ",
                ["Add"] = "ເພີ່ມ",
                ["Delete"] = "ລຶບ",
                ["Clear All"] = "ລ້າງທັງໝົດ",
                ["Export"] = "ສົ່ງອອກ",
                [" (recommended)"] = " (ແນະນຳ)",
                ["Language Settings"] = "ການຕັ້ງຄ່າພາສາ",
                ["Select your preferred display language for the application interface."] = "ເລືອກພາສາໃຊ້ສະແດງທີ່ທ່ານຕ້ອງການສຳລັບກ່ອງແອັບຯ.",
                ["Home"] = "ໜ້າຫຼັກ",
                ["Global"] = "ທົ່ວໂລກ",
                ["Games"] = "ເກມ",
                ["Settings"] = "ການຕັ້ງຄ່າ",
                ["FAQ"] = "ຄຳຖາມທີ່ພົບເລື້ອຍ",
                ["Chinese"] = "ຈີນ",
                ["English"] = "English",
                ["Vietnamese"] = "ຫວຽດນາມ",
                ["Filipino"] = "ຟິລິປິນ",
                ["Indonesian"] = "ອິນໂດເນເຊຍ",
                ["Portuguese"] = "ປອກຕຸຍການ",
                ["Malay"] = "ມາເລ",
                ["Japanese"] = "ຍີ່ປຸ່ນ",
                ["Thai"] = "ໄທ",
                ["Khmer"] = "ຂະເໝນ",
                ["Lao"] = "ພາສາລາວ",
                ["Physics"] = "ຟີຊິກ",
                ["Audio"] = "ສຽງ",
                ["Graphics"] = "ກຣາບຟິກ",
                ["Internet"] = "ອິນເຕີເນັດ",
                ["Search"] = "ຊອກຫາ",
                ["Filter:"] = "ກອງ:",
                ["Cancel"] = "ຍົກເລີກ",
                ["Apply"] = "ນຳໃຊ້",
                ["Select Game FFlags Preset"] = "ເລືອກຄ່າຕັ້ງລ່ວງໜ້າ FFlags ເກມ",
                ["🎮 Select Game FFlags Preset"] = "🎮 ເລືອກຄ່າຕັ້ງລ່ວງໜ້າ FFlags ເກມ",
                ["Not loaded"] = "ຍັງບໍ່ໄດ້ໂຫຼດ",
                ["Loaded"] = "ໂຫຼດແລ້ວ",
                ["Opening Roblox..."] = "ກຳລັງເປີດ Roblox...",
                ["Activity log"] = "ບັນທຶກກິດຈະກຳ",
                ["Activity Log"] = "ບັນທຶກກິດຈະກຳ",
                ["Clear Log"] = "ລ້າງບັນທຶກ",
                ["0 entries"] = "໐ ລາຍການ",
                ["System initialized"] = "ລະບົບເລີ່ມຕົ້ນແລ້ວ",
                ["Ready to load FFlags"] = "ພ້ອມໂຫຼດ FFlags",
                ["Not set"] = "ຍັງບໍ່ໄດ້ຕັ້ງຄ່າ",
                ["Saved FFlags:"] = "FFlags ທີ່ບັນທຶກ:",
                ["Enabled"] = "ເປີດໃຊ້ງານ",
                ["Disabled"] = "ປິດໃຊ້ງານ",
                ["Auto-load FFlags:"] = "ໂຫຼດ FFlags ອັດຕະໂນມັດ:",
                ["Auto-load Addresses:"] = "ໂຫຼດທີ່ຢູ່ອັດຕະໂນມັດ:",
                ["Not detected"] = "ບໍ່ພົບ",
                ["Roblox Version:"] = "ລູກຄ້າ Roblox:",
                ["Unknown"] = "ບໍ່ຮູ້",
                ["Software Version:"] = "ລູກຄ້າໂປຣແກຣມ:",
                ["Version Compatibility:"] = "ຄວາມເຂົ້າກັນໄດ້ລູກຄ້າ:",
                ["MATCH"] = "ກົງກັນ",
                ["MISMATCH"] = "ບໍ່ກົງກັນ",
                ["UNKNOWN"] = "ບໍ່ຮູ້",
                ["Application successful ({0} FFlags)"] = "ສັກຢັບສຳເລັດ ({0} FFlags)",
                ["Application failed ({0} errors)"] = "ສັກຢັບບໍ່ສຳເລັດ ({0} ຄວາມຜິດພາດ)",
                ["now"] = "ເມື່ອສັກກີ້",
                ["s ago"] = " ວິນາທີກ່ອນ",
                ["m ago"] = " ນາທີກ່ອນ",
                ["Success"] = "ສຳເລັດ",
                ["Failed"] = "ບໍ່ສຳເລັດ",
                ["Pending"] = "ລໍຖ້າ",
                ["Mixed"] = "ປົນ",
                ["Status"] = "ສະຖານະ",
                ["Session"] = "ເຊດຊັນ",
                ["Last"] = "ຄັ້ງສຸດທ້າຍ",
                ["Actions"] = "ການດຳເນີນການ",
                ["Activity log cleared"] = "ລ້າງບັນທຶກກິດຈະກຳແລ້ວ",
                ["JSON file not found"] = "ບໍ່ພົບໄຟລ໌ JSON",
                ["JSON Content Preview:"] = "ຕົວຢ່າງເນື້ອໃນ JSON:",
                ["... and {0} more entries"] = "... ແລະ {0} ລາຍການເພີ່ມ",
                ["Total entries in JSON: {0}"] = "ລາຍການທັງໝົດໃນ JSON: {0}",
                ["Invalid JSON format"] = "ຮູບແບບ JSON ບໍ່ຖືກຕ້ອງ",
                ["Error parsing JSON"] = "ຜິດພາດໃນການວິເຄາະ JSON",
                [" Loading FFlag addresses..."] = " ກຳລັງໂຫຼດທີ່ຢູ່ FFlag...",
                [" Auto-loading FFlag addresses..."] = " ກຳລັງໂຫຼດທີ່ຢູ່ FFlag ອັດຕະໂນມັດ...",
                ["Idle"] = "ວ່າງ",
                ["Fast Mode"] = "ໂໝດໄວ",
                ["Join"] = "ເຂົ້າຮ່ວມ",
                ["Join Discord"] = "ເຂົ້າຮ່ວມ Discord",
                ["Made By ©Dank1ngs"] = "ສ້າງໂດຍ ©Dank1ngs",
                ["Presets"] = "ຄ່າຕັ້ງລ່ວງໜ້າ",
                ["Count:"] = "ຈຳນວນ:",
                ["Roblox Version:"] = "ລູກຄ້າ Roblox:",
                ["Software Version:"] = "ລູກຄ້າໂປຣແກຣມ:",
                ["Last update:"] = "ອັບເດດຫຼ້າສຸດ:",
                ["FFlags:"] = "FFlags:",
                ["Information System"] = "ລະບົບຂໍ້ມູນ",
                ["INFORMATION SYSTEM"] = "ລະບົບຂໍ້ມູນ",
                ["Don't Save"] = "ບໍ່ບັນທຶກ",
                ["Unsaved Changes"] = "ການປ່ຽນແປງທີ່ຍັງບໍ່ໄດ້ບັນທຶກ",
                ["You have unsaved changes. Do you want to save before exiting?"] = "ທ່ານມີການປ່ຽນແປງທີ່ຍັງບໍ່ໄດ້ບັນທຶກ. ທ່ານຕ້ອງການບັນທຶກກ່ອນອອກບໍ?",
                ["Ready"] = "ພ້ອມ",
                ["Initializing..."] = "ກຳລັງເລີ່ມຕົ້ນ...",
                ["Loading..."] = "ກຳລັງໂຫຼດ...",
                ["Applying..."] = "ກຳລັງສັກຢັບ...",
                ["Back"] = "ກັບຄືນ",
                ["← Back"] = "← ກັບຄືນ",
                ["All"] = "ທັງໝົດ",
                ["Default"] = "ເລີ່ມຕົ້ນ",
                ["Other"] = "ອື່ນໆ",
                ["Set as Read-Only"] = "ຕັ້ງເປັນອ່ານຢ່າງດຽວ",
                ["Prevent Roblox from overriding global settings."] = "ປ້ອງກັນ Roblox ບໍ່ໃຫ້ຂຽນທົ່ວການຕັ້ງຄ່າທົ່ວໂລກ.",
                ["How to Use Masterstrap"] = "ວິທີໃຊ້ Masterstrap",
                ["1. Load FFlags JSON file"] = "໑. ໂຫຼດໄຟລ໌ FFlags JSON",
                ["2. Load FFlag Addresses (optional)"] = "໒. ໂຫຼດທີ່ຢູ່ FFlag (ທາງເລືອກ)",
                ["3. Make sure Roblox is running"] = "໓. ໃຫ້ແນ່ໃຈວ່າ Roblox ກຳລັງເຮັດວຽກ",
                ["4. Click APPLY button to apply FFlags"] = "໔. ກົດປຸ່ມ APPLY ເພື່ອສັກຢັບ FFlags",
                ["5. Check Activity Log for results"] = "໕. ກວດບັນທຶກກິດຈະກຳເພື່ອເບິ່ງຜົນ",
                ["How to Edit FFlags"] = "ວິທີແກ້ໄຂ FFlags",
                ["• Go to Edit tab to modify loaded FFlags"] = "ໄປທີ່ແທັບ Edit ເພື່ອແກ້ໄຂ FFlags ທີ່ໂຫຼດແລ້ວ",
                ["• Click Add to create new FFlag entry"] = "ກົດ Add ເພື່ອສ້າງລາຍການ FFlag ໃໝ່",
                ["• Click Delete to remove selected FFlag"] = "ກົດ Delete ເພື່ອລຶບ FFlag ທີ່ເລືອກ",
                ["• Use Search to find specific FFlags"] = "ໃຊ້ Search ເພື່ອຊອກຫາ FFlags ສະເພາະ",
                ["• Click Export to save modified FFlags"] = "ກົດ Export ເພື່ອບັນທຶກ FFlags ທີ່ແກ້ໄຂແລ້ວ",
                ["Troubleshooting"] = "ການແກ້ໄຂບັນຫາ",
                ["Roblox not found?"] = "ບໍ່ພົບ Roblox?",
                ["Make sure Roblox is running before applying"] = "ໃຫ້ແນ່ໃຈວ່າ Roblox ກຳລັງເຮັດວຽກກ່ອນສັກຢັບ",
                ["Application failed?"] = "ສັກຢັບບໍ່ສຳເລັດ?",
                ["Please ensure that your Roblox version matches the version that Masterstrap has requested"] = "ກະລຸນາຮັບປະກັນວ່າລູກຄ້າ Roblox ຂອງທ່ານຕົງກັບລູກຄ້າທີ່ Masterstrap ຕ້ອງການ",
                ["FFlags not loading?"] = "FFlags ບໍ່ໂຫຼດ?",
                ["Verify JSON file format is correct and valid"] = "ກວດສອບຮູບແບບໄຟລ໌ JSON ວ່າຖືກຕ້ອງ ແລະ ຊົ່ວຄາວ.",
                ["Game crash after applying?"] = "ເກມລົ້ມຫຼັງສັກຢັບ?",
                ["Tips and Tricks"] = "ເຄັດລັບ ແລະ ວິທີ",
                ["• Keep your FFlag JSON file backed up"] = "ບັນທຶກໄຟລ໌ FFlag JSON ຂອງທ່ານໄວ້ສຳຮອງ",
                ["• Export frequently to save your changes"] = "ເຮັດ Export ບໍ່ຂາດເພື່ອບັນທຶກການປ່ຽນແປງຂອງທ່ານ",
                ["• Use Search feature to quickly find FFlags"] = "ໃຊ້ຄຸນສຳຄັນ Search ເພື່ອຊອກຫາ FFlags ໄວ",
                ["• Check Activity Log for application status"] = "ກວດບັນທຶກກິດຈະກຳເພື່ອເບິ່ງສະຖານະສັກຢັບ",
                ["Auto-load FFlags on startup (recommended)"] = "ໂຫຼດ FFlags ອັດຕະໂນມັດເມື່ອເປີດໃຊ້ (ແນະນຳ)",
                ["Auto-load FFlags on startup"] = "ໂຫຼດ FFlags ອັດຕະໂນມັດເມື່ອເປີດໃຊ້",
                ["Auto-load Cache on startup (recommended)"] = "ໂຫຼດ Cache ອັດຕະໂນມັດເມື່ອເປີດໃຊ້ (ແນະນຳ)",
                ["Auto-load Cache on startup"] = "ໂຫຼດ Cache ອັດຕະໂນມັດເມື່ອເປີດໃຊ້",
                ["Auto-apply when Roblox is detected (recommended)"] = "ສັກຢັບອັດຕະໂນມັດເມື່ອພົບ Roblox (ແນະນຳ)",
                ["Minimize to system tray"] = "ຫຍໍ້ລົງໄປຖານລະບົບ",
                ["Optimizer"] = "ເຄື່ອງມືເພີ່ມປະສິດທິພາບ",
                ["Auto-cleanup temp files (recommended)"] = "ລຶບໄຟລ໌ຊົ່ວຄາວອັດຕະໂນມັດ (ແນະນຳ)",
                ["Auto-cleanup temp files"] = "ລຶບໄຟລ໌ຊົ່ວຄາວ",
                ["Memory optimization (recommended)"] = "ເພີ່ມປະສິດທິພາບຄວາມຈຳ (ແນະນຳ)",
                ["Memory optimization"] = "ເພີ່ມປະສິດທິພາບຄວາມຈຳ",
                ["Graphics Quality"] = "ຄຸນນະພາບກຣາບຟິກ",
                ["Set the graphics quality of your game"] = "ຕັ້ງຄຸນນະພາບກຣາບຟິກຂອງເກມຂອງທ່ານ",
                ["Max Quality Enabled"] = "ເປີດໃຊ້ຄຸນນະພາບສູງສຸດ",
                ["Enables maximum graphics quality mode for enhanced visual effects and rendering detail."] = "ເປີດໂໝດຄຸນນະພາບກຣາບຟິກສູງສຸດສຳລັບຜົນພາບ ແລະ ລາຍລະອຽດການເຮັດພາບ.",
                ["Graphics Quality Level"] = "ລະດັບຄຸນນະພາບກຣາບຟິກ",
                ["Adjusts the in-game graphics quality level from low to maximum."] = "ປັບລະດັບຄຸນນະພາບກຣາບຟິກໃນເກມຈາກຕ່ຳຫາສູງສຸດ.",
                ["Framerate Limit"] = "ຈຳກັດເຟຣມເຣດ",
                ["Unlock framerate limit for Roblox. Going above 240 FPS is not recommended."] = "ປົດລ໋ອກຈຳກັດເຟຣມເຣດສຳລັບ Roblox. ບໍ່ແນະນຳໃຫ້ເກີນ 240 FPS.",
                ["User Interface and Layout"] = "ກ່ອງໃຊ້ສະແດງ ແລະ ຮູບຈັງ",
                ["Transparency"] = "ຄວາມໂປ່ງໃສ",
                ["Custom transparency for UI elements."] = "ຄວາມໂປ່ງໃສກຳນົດເອງສຳລັບອົງປະກອບ UI.",
                ["Reduced Motion"] = "ການເຄື່ອນໄຫວຫຼຸດລົງ",
                ["Removes the animation on the escape menu."] = "ລຶບເອນິເມຊັນອອກຈາກເມນູອອກ.",
                ["Font Size"] = "ຂະໜາດຟອນ",
                ["Choose how large the font should appear."] = "ເລືອກຂະໜາດຟອນທີ່ຕ້ອງການສະແດງ.",
                ["Mouse Sensitivity"] = "ຄວາມອ່ອນໄຫວຂອງເມົາ",
                ["Change how fast the camera will move in-game."] = "ປ່ຽນຄວາມໄວກ້ອງເຄື່ອນໃນເກມ.",
                ["VR Enabled"] = "ເປີດໃຊ້ VR",
                ["Player Name Visibility"] = "ການເບິ່ງເຫັນຊື່ຜູ້ຫຼິ້ນ",
                ["Hide name tags above other players for a cleaner screen experience."] = "ເຊື່ອງແທັກຊື່ເທິງຜູ້ຫຼິ້ນອື່ນເພື່ອຈໍສະອາດຂຶ້ນ.",
                ["Open the editor to view and change flags, use presets, and choose whether Masterstrap may apply them when you launch."] = "ເປີດຕົວແກ້ໄຂເພື່ອເບິ່ງ ແລະ ປ່ຽນແທັກ ໃຊ້ຄ່າຕັ້ງລ່ວງໜ້າ ແລະ ເລືອກວ່າ Masterstrap ຈະນຳໃຊ້ເມື່ອທ່ານເປີດຫຼືບໍ່.",
                ["Adjust Roblox wide settings such as read only mode, rendering, and framerate limits."] = "ປັບການຕັ້ງຄ່າທົ່ວ Roblox ເຊັ່ນ ໂໝດອ່ານຢ່າງດຽວ ການແສດງຮູບ ແລະ ຂີດຈຳກັດເຟຣມເຣດ.",
                ["Edit the flag table, filter by category, search, then use Back and Save on the FastFlags page when you are done."] = "ແກ້ໄຂຕາຕະລາງແທັກ ກອງຕາມໝວດ ຊອກຫາ ຈາກນັ້ນໃຊ້ກັບຄືນ ແລະ ບັນທຶກທີ່ໜ້າ FastFlags ເມື່ອສຳເລັດ.",
                ["Choose language, visual theme, and startup behavior for the app."] = "ເລືອກພາສາ ຮູບແບບສາຍຕາ ແລະ ພຶດຕິກຳເມື່ອເປີດແອັບ.",
                ["Load a curated flag set for a game into your list, then adjust or save as usual."] = "ໂຫຼດຊຸດແທັກທີ່ຄັດເລືອກສຳລັບເກມເຂົ້າລາຍການຂອງທ່ານ ຈາກນັ້ນປັບ ຫຼື ບັນທຶກຕາມປົກກະຕິ.",
                ["Credits, FAQ, and short guides for using Masterstrap."] = "ເຄຣດິດ ຄຳຖາມທີ່ພົບເປັນປົກກະຕິ ແລະ ຄູ່ມືສັ້ນໆ ສຳລັບໃຊ້ Masterstrap.",
                ["Graphic advanced"] = "ກຣາບຟິກຂັ້ນສູງ",
                ["Lowest quality"] = "ຄຸນນະພາບຕ່ຳສຸດ",
                ["Highest quality"] = "ຄຸນນະພາບສູງສຸດ"
            }), AboutTabUiTranslations.EnToLo),
            MergeUiOverlays(MergeUiOverlays(BuildFlagUiTranslations.EnToLo, LaunchProgressUiTranslations.EnToLo), DialogsUiTranslations.EnToLo));


        private static readonly Dictionary<string, string> EnToKo = OverlayAboutTab(
            OverlayAboutTab(
            MergeTranslations(
            LoadMdTranslations("Masterstrap.Resources.Korean.md"),
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Auto-check for updates on startup (recommended)"] = "시작 시 업데이트 자동 확인 (권장)",
                ["Auto-check for updates on startup"] = "시작 시 업데이트 자동 확인",
                ["Load FFlags JSON"] = "FFlags JSON 로드",
                ["Load FFlag Addresses"] = "FFlag 주소 로드",
                ["📁 Load FFlags JSON"] = "📁 FFlags JSON 로드",
                ["📄 Load FFlag Addresses"] = "📄 FFlag 주소 로드",
                ["Add New FFlag"] = "새 FFlag 추가",
                ["Add New FFlags"] = "새 FFlags 추가",
                ["Add or batch import new flags to your library"] = "새 플래그를 추가하거나 일괄 가져와 라이브러리에 넣기",
                ["Flag Editor"] = "플래그 편집기",
                ["FLAG EDITOR"] = "플래그 편집기",
                ["Enter flags manually or load from JSON file"] = "플래그를 수동으로 입력하거나 JSON 파일에서 로드",
                ["FORMAT: name: value"] = "형식: 이름: 값",
                ["Each line = 1 FFlags"] = "한 줄 = 1 FFlag",
                ["(Each line = 1 FFlag. Example: MyFlag: true)"] = "(한 줄 = 1 FFlag. 예: MyFlag: true)",
                ["Ready to add flags"] = "플래그 추가 준비됨",
                ["Configuration saved successfully!"] = "구성이 성공적으로 저장되었습니다!",
                ["Configuration saved successfully"] = "구성이 성공적으로 저장되었습니다",
                ["CompleteAdsenseDialog created successfully"] = "CompleteAdsenseDialog가 성공적으로 생성되었습니다",
                ["Complete the adsense to continue"] = "계속하려면 광고를 완료하세요",
                ["Support"] = "지원",
                ["Please look at and click on ads so this software project can continue for free"] = "이 소프트웨어 프로젝트가 무료로 계속될 수 있도록 광고를 보고 클릭해 주세요",
                ["⏭ Skip ad 3:00"] = "⏭ 광고 건너뛰기 3:00",
                ["Skip ad"] = "광고 건너뛰기",
                ["How to skip ad? "] = "광고를 건너뛰는 방법? ",
                ["Click here"] = "여기를 클릭",
                ["✓ Continue"] = "✓ 계속",
                ["✓ Ad Complete"] = "✓ 광고 완료",
                ["Ad Complete"] = "광고 완료",
                ["Please wait for the countdown to finish"] = "카운트다운이 끝날 때까지 기다려 주세요",
                ["Please wait"] = "잠시만 기다려 주세요",
                ["Please click 'Continue' button to proceed"] = "계속하려면 '계속' 버튼을 클릭하세요",
                ["Ad Completed"] = "광고 완료",
                ["Could not open support link"] = "지원 링크를 열 수 없습니다",
                ["Could not open help link"] = "도움말 링크를 열 수 없습니다",
                ["Could not open Discord link"] = "Discord 링크를 열 수 없습니다",
                ["Roblox Launch"] = "Roblox 실행",
                ["Launching Roblox"] = "Roblox 실행 중",
                ["Masterstrap - Loading..."] = "Masterstrap - 로드 중...",
                ["Masterstrap • "] = "Masterstrap • ",
                ["Loading FastFlag configuration..."] = "FastFlag 구성 로드 중...",
                ["Starting Roblox..."] = "Roblox 시작 중...",
                ["Waiting for Roblox to open..."] = "Roblox 열림 대기 중...",
                ["Roblox opened. Launch complete."] = "Roblox가 열렸습니다. 실행 완료.",
                ["Masterstrap deploying auto-apply..."] = "Masterstrap 자동 주입 배포 중...",
                ["Waiting for game to be ready..."] = "게임 준비 대기 중...",
                ["Roblox closed before application."] = "주입 전에 Roblox가 종료되었습니다.",
                ["Retrying application ({0}/{1})..."] = "주입 재시도 중 ({0}/{1})...",
                ["Masterstrap auto-applying..."] = "Masterstrap 자동 주입 중...",
                ["Auto-applying..."] = "자동 주입 중...",
                ["Waiting for Roblox... ({0}s)"] = "Roblox 대기 중... ({0}초)",
                ["Roblox not detected. Closing..."] = "Roblox가 감지되지 않습니다. 종료 중...",
                ["Launch complete."] = "실행 완료.",
                ["Applied successfully. Closing..."] = "주입 성공. 종료 중...",
                ["Application failed."] = "주입 실패.",
                ["Roblox not detected."] = "Roblox가 감지되지 않습니다.",
                ["Cancelled."] = "취소됨.",
                ["Error: {0}"] = "오류: {0}",
                ["Application applied. Waiting for game to apply..."] = "주입 적용됨. 게임 적용 대기 중...",
                ["Applied successfully."] = "주입 성공.",
                ["Failed to launch Roblox: {0}"] = "Roblox 실행 실패: {0}",
                ["Launch Error"] = "실행 오류",
                ["Adsense dialog marked as OPEN"] = "Adsense 대화 상자가 열림으로 표시됨",
                ["Adsense dialog marked as CLOSED"] = "Adsense 대화 상자가 닫힘으로 표시됨",
                ["FFlags applied successfully!"] = "FFlags 주입 성공!",
                ["⚡ APPLY"] = "⚡ 주입",
                ["↩️ UNAPPLY"] = "↩️ 주입 해제",
                ["APPLY"] = "주입",
                ["UNAPPLY"] = "주입 해제",
                ["Save and Launch"] = "저장 후 실행",
                ["Save"] = "저장",
                ["Close"] = "닫기",
                ["Activity Log"] = "활동 로그",
                ["Clear Log"] = "로그 지우기",
                ["Add"] = "추가",
                ["Delete"] = "삭제",
                ["Clear All"] = "모두 지우기",
                ["Export"] = "내보내기",
                [" (recommended)"] = " (권장)",
                ["Language Settings"] = "언어 설정",
                ["Select your preferred display language for the application interface."] = "앱 인터페이스에 사용할 표시 언어를 선택하세요.",
                ["Home"] = "홈",
                ["Global"] = "전역",
                ["Games"] = "게임",
                ["Settings"] = "설정",
                ["FAQ"] = "자주 묻는 질문",
                ["Chinese"] = "중국어",
                ["English"] = "English",
                ["Vietnamese"] = "베트남어",
                ["Filipino"] = "필리핀어",
                ["Indonesian"] = "인도네시아어",
                ["Portuguese"] = "포르투갈어",
                ["Malay"] = "말레이어",
                ["Japanese"] = "일본어",
                ["Thai"] = "태국어",
                ["Khmer"] = "크메어",
                ["Lao"] = "라오어",
                ["Korean"] = "한국어",
                ["Physics"] = "물리",
                ["Audio"] = "오디오",
                ["Graphics"] = "그래픽",
                ["Internet"] = "인터넷",
                ["Search"] = "검색",
                ["Filter:"] = "필터:",
                ["Cancel"] = "취소",
                ["Apply"] = "적용",
                ["Select Game FFlags Preset"] = "게임 FFlags 사전 설정 선택",
                ["🎮 Select Game FFlags Preset"] = "🎮 게임 FFlags 사전 설정 선택",
                ["Not loaded"] = "로드 안 됨",
                ["Loaded"] = "로드됨",
                ["Opening Roblox..."] = "Roblox 열기 중...",
                ["Activity log"] = "활동 로그",
                ["Activity Log"] = "활동 로그",
                ["Clear Log"] = "로그 지우기",
                ["0 entries"] = "0개 항목",
                ["System initialized"] = "시스템이 초기화되었습니다",
                ["Ready to load FFlags"] = "FFlags 로드 준비됨",
                ["Not set"] = "설정 안 함",
                ["Saved FFlags:"] = "저장된 FFlags:",
                ["Enabled"] = "사용함",
                ["Disabled"] = "사용 안 함",
                ["Auto-load FFlags:"] = "FFlags 자동 로드:",
                ["Auto-load Addresses:"] = "주소 자동 로드:",
                ["Not detected"] = "미검출",
                ["Roblox Version:"] = "Roblox 버전:",
                ["Unknown"] = "알 수 없음",
                ["Software Version:"] = "소프트웨어 버전:",
                ["Version Compatibility:"] = "버전 호환성:",
                ["MATCH"] = "일치",
                ["MISMATCH"] = "불일치",
                ["UNKNOWN"] = "알 수 없음",
                ["Application successful ({0} FFlags)"] = "주입 성공 ({0}개 FFlags)",
                ["Application failed ({0} errors)"] = "주입 실패 ({0}개 오류)",
                ["now"] = "방금",
                ["s ago"] = " 초 전",
                ["m ago"] = " 분 전",
                ["Success"] = "성공",
                ["Failed"] = "실패",
                ["Pending"] = "대기 중",
                ["Mixed"] = "혼합",
                ["Status"] = "상태",
                ["Session"] = "세션",
                ["Last"] = "마지막",
                ["Actions"] = "작업",
                ["Activity log cleared"] = "활동 로그가 지워졌습니다",
                ["JSON file not found"] = "JSON 파일을 찾을 수 없습니다",
                ["JSON Content Preview:"] = "JSON 내용 미리보기:",
                ["... and {0} more entries"] = "... 및 {0}개 더",
                ["Total entries in JSON: {0}"] = "JSON 내 총 항목: {0}",
                ["Invalid JSON format"] = "잘못된 JSON 형식",
                ["Error parsing JSON"] = "JSON 구문 분석 오류",
                [" Loading FFlag addresses..."] = " FFlag 주소 로드 중...",
                [" Auto-loading FFlag addresses..."] = " FFlag 주소 자동 로드 중...",
                ["Idle"] = "대기",
                ["Fast Mode"] = "빠른 모드",
                ["Join"] = "참여",
                ["Join Discord"] = "Discord 참여",
                ["Made By ©Dank1ngs"] = "제작 ©Dank1ngs",
                ["Presets"] = "사전 설정",
                ["Count:"] = "개수:",
                ["Roblox Version:"] = "Roblox 버전:",
                ["Software Version:"] = "소프트웨어 버전:",
                ["Last update:"] = "최종 업데이트:",
                ["FFlags:"] = "FFlags:",
                ["Information System"] = "정보 시스템",
                ["INFORMATION SYSTEM"] = "정보 시스템",
                ["Don't Save"] = "저장 안 함",
                ["Unsaved Changes"] = "저장되지 않은 변경 사항",
                ["You have unsaved changes. Do you want to save before exiting?"] = "저장되지 않은 변경 사항이 있습니다. 종료 전에 저장할까요?",
                ["Ready"] = "준비됨",
                ["Initializing..."] = "초기화 중...",
                ["Loading..."] = "로드 중...",
                ["Applying..."] = "주입 중...",
                ["Back"] = "뒤로",
                ["← Back"] = "← 뒤로",
                ["All"] = "전체",
                ["Default"] = "기본값",
                ["Other"] = "기타",
                ["Set as Read-Only"] = "읽기 전용으로 설정",
                ["Prevent Roblox from overriding global settings."] = "Roblox가 전역 설정을 덮어쓰지 못하게 합니다.",
                ["How to Use Masterstrap"] = "Masterstrap 사용 방법",
                ["1. Load FFlags JSON file"] = "1. FFlags JSON 파일 로드",
                ["2. Load FFlag Addresses (optional)"] = "2. FFlag 주소 로드 (선택 사항)",
                ["3. Make sure Roblox is running"] = "3. Roblox가 실행 중인지 확인하세요",
                ["4. Click APPLY button to apply FFlags"] = "4. APPLY 버튼을 클릭하여 FFlags 주입",
                ["5. Check Activity Log for results"] = "5. 결과는 활동 로그에서 확인하세요",
                ["How to Edit FFlags"] = "FFlags 편집 방법",
                ["• Go to Edit tab to modify loaded FFlags"] = "로드된 FFlags를 수정하려면 Edit 탭으로 이동하세요",
                ["• Click Add to create new FFlag entry"] = "새 FFlag 항목을 만들려면 Add를 클릭하세요",
                ["• Click Delete to remove selected FFlag"] = "선택한 FFlag를 제거하려면 Delete를 클릭하세요",
                ["• Use Search to find specific FFlags"] = "특정 FFlags를 찾으려면 Search를 사용하세요",
                ["• Click Export to save modified FFlags"] = "수정한 FFlags를 저장하려면 Export를 클릭하세요",
                ["Troubleshooting"] = "문제 해결",
                ["Roblox not found?"] = "Roblox를 찾을 수 없나요?",
                ["Make sure Roblox is running before applying"] = "주입 전에 Roblox가 실행 중인지 확인하세요",
                ["Application failed?"] = "주입에 실패했나요?",
                ["Please ensure that your Roblox version matches the version that Masterstrap has requested"] = "Roblox 버전이 Masterstrap가 요청한 버전과 일치하는지 확인하세요",
                ["FFlags not loading?"] = "FFlags가 로드되지 않나요?",
                ["Verify JSON file format is correct and valid"] = "JSON 파일 형식이 올바르고 유효한지 확인하세요",
                ["Game crash after applying?"] = "주입 후 게임이 충돌하나요?",
                ["Tips and Tricks"] = "팁과 요령",
                ["• Keep your FFlag JSON file backed up"] = "FFlag JSON 파일을 백업해 두세요",
                ["• Export frequently to save your changes"] = "변경 사항을 저장하려면 자주 내보내기하세요",
                ["• Use Search feature to quickly find FFlags"] = "Search 기능으로 FFlags를 빠르게 찾으세요",
                ["• Check Activity Log for application status"] = "주입 상태는 활동 로그에서 확인하세요",
                ["Auto-load FFlags on startup (recommended)"] = "시작 시 FFlags 자동 로드 (권장)",
                ["Auto-load FFlags on startup"] = "시작 시 FFlags 자동 로드",
                ["Auto-load Cache on startup (recommended)"] = "시작 시 Cache 자동 로드 (권장)",
                ["Auto-load Cache on startup"] = "시작 시 Cache 자동 로드",
                ["Auto-apply when Roblox is detected (recommended)"] = "Roblox 감지 시 자동 주입 (권장)",
                ["Minimize to system tray"] = "시스템 트레이로 최소화",
                ["Optimizer"] = "최적화 도구",
                ["Auto-cleanup temp files (recommended)"] = "임시 파일 자동 정리 (권장)",
                ["Auto-cleanup temp files"] = "임시 파일 자동 정리",
                ["Memory optimization (recommended)"] = "메모리 최적화 (권장)",
                ["Memory optimization"] = "메모리 최적화",
                ["Graphics Quality"] = "그래픽 품질",
                ["Set the graphics quality of your game"] = "게임의 그래픽 품질 설정",
                ["Max Quality Enabled"] = "최고 품질 사용",
                ["Enables maximum graphics quality mode for enhanced visual effects and rendering detail."] = "향상된 시각 효과와 렌더링 디테일을 위해 최대 그래픽 품질 모드를 사용합니다.",
                ["Graphics Quality Level"] = "그래픽 품질 수준",
                ["Adjusts the in-game graphics quality level from low to maximum."] = "게임 내 그래픽 품질 수준을 낮음에서 최대까지 조정합니다.",
                ["Framerate Limit"] = "프레임률 제한",
                ["Unlock framerate limit for Roblox. Going above 240 FPS is not recommended."] = "Roblox의 프레임률 제한을 해제합니다. 240 FPS 이상은 권장하지 않습니다.",
                ["User Interface and Layout"] = "사용자 인터페이스 및 레이아웃",
                ["Transparency"] = "투명도",
                ["Custom transparency for UI elements."] = "UI 요소의 사용자 지정 투명도.",
                ["Reduced Motion"] = "동작 감소",
                ["Removes the animation on the escape menu."] = "이스케이프 메뉴의 애니메이션을 제거합니다.",
                ["Font Size"] = "글꼴 크기",
                ["Choose how large the font should appear."] = "글꼴이 표시될 크기를 선택하세요.",
                ["Mouse Sensitivity"] = "마우스 감도",
                ["Change how fast the camera will move in-game."] = "게임 내 카메라 이동 속도를 변경합니다.",
                ["VR Enabled"] = "VR 사용",
                ["Player Name Visibility"] = "플레이어 이름 표시",
                ["Hide name tags above other players for a cleaner screen experience."] = "더 깔끔한 화면을 위해 다른 플레이어 위의 이름표를 숨깁니다.",
                ["Open the editor to view and change flags, use presets, and choose whether Masterstrap may apply them when you launch."] = "편집기를 열어 플래그를 보고 변경하고, 프리셋을 사용하며, 실행 시 Masterstrap이 적용할지 선택하세요.",
                ["Adjust Roblox wide settings such as read only mode, rendering, and framerate limits."] = "읽기 전용 모드, 렌더링, 프레임률 제한 등 Roblox 전역 설정을 조정합니다.",
                ["Edit the flag table, filter by category, search, then use Back and Save on the FastFlags page when you are done."] = "플래그 표를 편집하고, 범주로 필터링하고, 검색한 뒤 완료되면 FastFlags 페이지에서 뒤로와 저장을 사용하세요.",
                ["Choose language, visual theme, and startup behavior for the app."] = "앱의 언어, 시각적 테마, 시작 시 동작을 선택하세요.",
                ["Load a curated flag set for a game into your list, then adjust or save as usual."] = "게임용으로 선별된 플래그 세트를 목록에 로드한 뒤 평소처럼 조정하거나 저장하세요.",
                ["Credits, FAQ, and short guides for using Masterstrap."] = "크레딧, FAQ 및 Masterstrap 사용을 위한 짧은 안내.",
                ["Graphic advanced"] = "고급 그래픽",
                ["Lowest quality"] = "최저 품질",
                ["Highest quality"] = "최고 품질"
            }), AboutTabUiTranslations.EnToKo),
            MergeUiOverlays(MergeUiOverlays(BuildFlagUiTranslations.EnToKo, LaunchProgressUiTranslations.EnToKo), DialogsUiTranslations.EnToKo));

        private static readonly Dictionary<string, string> EnToRu =
            OverlayAboutTab(MergePreferSecond(EnToFil, LoadMdTranslations("Masterstrap.Resources.russian.md")),
                MergeUiOverlays(MergeUiOverlays(BuildFlagUiTranslations.EnToRu, LaunchProgressUiTranslations.EnToRu), DialogsUiTranslations.EnToRu));

        private static readonly Dictionary<string, string> EnToUk =
            OverlayAboutTab(MergePreferSecond(EnToFil, LoadMdTranslations("Masterstrap.Resources.ukrainian.md")),
                MergeUiOverlays(MergeUiOverlays(BuildFlagUiTranslations.EnToUk, LaunchProgressUiTranslations.EnToUk), DialogsUiTranslations.EnToUk));

        private static readonly Dictionary<string, string> EnToEsLat =
            OverlayAboutTab(MergePreferSecond(EnToFil, LoadMdTranslations("Masterstrap.Resources.spanish_latam.md")),
                MergeUiOverlays(MergeUiOverlays(BuildFlagUiTranslations.EnToEsLatAm, LaunchProgressUiTranslations.EnToEsLatAm), DialogsUiTranslations.EnToEsLatAm));

        private static readonly Dictionary<string, string> EnToFr =
            OverlayAboutTab(MergePreferSecond(EnToFil, LoadMdTranslations("Masterstrap.Resources.french.md")),
                MergeUiOverlays(MergeUiOverlays(BuildFlagUiTranslations.EnToFr, LaunchProgressUiTranslations.EnToFr), DialogsUiTranslations.EnToFr));

        private static readonly Dictionary<string, string> EnToHe =
            OverlayAboutTab(MergePreferSecond(EnToFil, LoadMdTranslations("Masterstrap.Resources.hebrew.md")),
                MergeUiOverlays(MergeUiOverlays(BuildFlagUiTranslations.EnToHe, LaunchProgressUiTranslations.EnToHe), DialogsUiTranslations.EnToHe));

        private static readonly Dictionary<string, string> EnToTw = OverlayAboutTab(
            OverlayAboutTab(
            MergePreferSecond(
                MergePreferSecond(EnToZh, LoadMdTranslations("Masterstrap.Resources.taiwan.md")),
                LoadMdTranslations("Masterstrap.Resources.taiwan_ui_overlay.md")),
            AboutTabUiTranslations.EnToTw),
            MergeUiOverlays(MergeUiOverlays(BuildFlagUiTranslations.EnToTw, LaunchProgressUiTranslations.EnToTw), DialogsUiTranslations.EnToTw));

        private static readonly Dictionary<string, string> EnToEsCo =
            MergePreferSecond(EnToEsLat, LoadMdTranslations("Masterstrap.Resources.colombia.md"));

        private static readonly Dictionary<string, string> EnToTr =
            OverlayAboutTab(MergePreferSecond(EnToFil, LoadMdTranslations("Masterstrap.Resources.turkiye.md")),
                MergeUiOverlays(MergeUiOverlays(BuildFlagUiTranslations.EnToTr, LaunchProgressUiTranslations.EnToTr), DialogsUiTranslations.EnToTr));

        private static readonly Dictionary<string, string> EnToEsSp =
            OverlayAboutTab(MergePreferSecond(EnToEsCo, LoadMdTranslations("Masterstrap.Resources.spain.md")),
                MergeUiOverlays(MergeUiOverlays(BuildFlagUiTranslations.EnToEsSp, LaunchProgressUiTranslations.EnToEsSp), DialogsUiTranslations.EnToEsSp));

        private static readonly Dictionary<string, string> EnToEsCl =
            MergePreferSecond(EnToEsCo, LoadMdTranslations("Masterstrap.Resources.chile.md"));

        private static readonly Dictionary<string, string> EnToIt =
            OverlayAboutTab(MergePreferSecond(EnToFil, LoadMdTranslations("Masterstrap.Resources.italian.md")),
                MergeUiOverlays(MergeUiOverlays(BuildFlagUiTranslations.EnToIt, LaunchProgressUiTranslations.EnToIt), DialogsUiTranslations.EnToIt));

        private static readonly Dictionary<string, string> EnToArAe =
            OverlayAboutTab(MergePreferSecond(EnToFil, LoadMdTranslations("Masterstrap.Resources.arabic_uae.md")),
                MergeUiOverlays(MergeUiOverlays(BuildFlagUiTranslations.EnToArAe, LaunchProgressUiTranslations.EnToArAe), DialogsUiTranslations.EnToArAe));

        private static Dictionary<string, string> MergeUiOverlays(
            Dictionary<string, string> keyPackages,
            Dictionary<string, string> buildFlags,
            Dictionary<string, string> launchProgress)
        {
            var result = new Dictionary<string, string>(keyPackages, StringComparer.OrdinalIgnoreCase);
            foreach (var kv in buildFlags)
                result[kv.Key] = kv.Value;
            foreach (var kv in launchProgress)
                result[kv.Key] = kv.Value;
            return result;
        }

        private static Dictionary<string, string> MergeUiOverlays(Dictionary<string, string> baseline, Dictionary<string, string> overlay)
        {
            var result = new Dictionary<string, string>(baseline, StringComparer.OrdinalIgnoreCase);
            foreach (var kv in overlay)
                result[kv.Key] = kv.Value;
            return result;
        }

        private static Dictionary<string, string> OverlayAboutTab(Dictionary<string, string> merged, Dictionary<string, string> aboutTab)
        {
            var result = new Dictionary<string, string>(merged, StringComparer.OrdinalIgnoreCase);
            foreach (var kv in aboutTab)
                result[kv.Key] = kv.Value;
            return result;
        }

        private static Dictionary<string, string> MergeTranslations(Dictionary<string, string> primary, Dictionary<string, string> fallback)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var p in primary) result[p.Key] = p.Value;
            foreach (var f in fallback) if (!result.ContainsKey(f.Key)) result[f.Key] = f.Value;
            return result;
        }

        private static Dictionary<string, string> MergePreferSecond(Dictionary<string, string> baseline, Dictionary<string, string> overrides)
        {
            var result = new Dictionary<string, string>(baseline, StringComparer.OrdinalIgnoreCase);
            foreach (var kv in overrides)
                result[kv.Key] = kv.Value;
            return result;
        }

        private static readonly Dictionary<string, string> ViToEn = BuildReverseMap(EnToVi);
        private static readonly Dictionary<string, string> FilToEn = BuildReverseMap(EnToFil);
        private static readonly Dictionary<string, string> IdToEn = BuildReverseMap(EnToId);
        private static readonly Dictionary<string, string> PtToEn = BuildReverseMap(EnToPt);
        private static readonly Dictionary<string, string> MalToEn = BuildReverseMap(EnToMal);
        private static readonly Dictionary<string, string> JaToEn = BuildReverseMap(EnToJa);
        private static readonly Dictionary<string, string> ZhToEn = BuildReverseMap(EnToZh);
        private static readonly Dictionary<string, string> ThToEn = BuildReverseMap(EnToTh);
        private static readonly Dictionary<string, string> KmToEn = BuildReverseMap(EnToKm);
        private static readonly Dictionary<string, string> LoToEn = BuildReverseMap(EnToLo);
        private static readonly Dictionary<string, string> KoToEn = BuildReverseMap(EnToKo);
        private static readonly Dictionary<string, string> RuToEn = BuildReverseMap(EnToRu);
        private static readonly Dictionary<string, string> UkToEn = BuildReverseMap(EnToUk);
        private static readonly Dictionary<string, string> EsLatToEn = BuildReverseMap(EnToEsLat);
        private static readonly Dictionary<string, string> FrToEn = BuildReverseMap(EnToFr);
        private static readonly Dictionary<string, string> HeToEn = BuildReverseMap(EnToHe);
        private static readonly Dictionary<string, string> TwToEn = BuildReverseMap(EnToTw);
        private static readonly Dictionary<string, string> EsColToEn = BuildReverseMap(EnToEsCo);
        private static readonly Dictionary<string, string> TrToEn = BuildReverseMap(EnToTr);
        private static readonly Dictionary<string, string> EsSpToEn = BuildReverseMap(EnToEsSp);
        private static readonly Dictionary<string, string> EsClToEn = BuildReverseMap(EnToEsCl);
        private static readonly Dictionary<string, string> ItToEn = BuildReverseMap(EnToIt);
        private static readonly Dictionary<string, string> ArAeToEn = BuildReverseMap(EnToArAe);
        private static readonly Dictionary<string, string> EnToDe =
            OverlayAboutTab(MergePreferSecond(EnToFil, LoadMdTranslations("Masterstrap.Resources.german.md")),
                MergeUiOverlays(MergeUiOverlays(BuildFlagUiTranslations.EnToDe, LaunchProgressUiTranslations.EnToDe), DialogsUiTranslations.EnToDe));

        private static readonly Dictionary<string, string> EnToRo =
            OverlayAboutTab(MergePreferSecond(EnToFil, LoadMdTranslations("Masterstrap.Resources.romanian.md")),
                MergeUiOverlays(MergeUiOverlays(BuildFlagUiTranslations.EnToRo, LaunchProgressUiTranslations.EnToRo), DialogsUiTranslations.EnToRo));

        private static readonly Dictionary<string, string> EnToSv =
            OverlayAboutTab(MergePreferSecond(EnToFil, LoadMdTranslations("Masterstrap.Resources.swedish.md")),
                MergeUiOverlays(MergeUiOverlays(BuildFlagUiTranslations.EnToSv, LaunchProgressUiTranslations.EnToSv), DialogsUiTranslations.EnToSv));

        private static readonly Dictionary<string, string> EnToNl =
            OverlayAboutTab(MergePreferSecond(EnToFil, LoadMdTranslations("Masterstrap.Resources.dutch.md")),
                MergeUiOverlays(MergeUiOverlays(BuildFlagUiTranslations.EnToNl, LaunchProgressUiTranslations.EnToNl), DialogsUiTranslations.EnToNl));

        private static readonly Dictionary<string, string> EnToPl =
            OverlayAboutTab(MergePreferSecond(EnToFil, LoadMdTranslations("Masterstrap.Resources.polish.md")),
                MergeUiOverlays(MergeUiOverlays(BuildFlagUiTranslations.EnToPl, LaunchProgressUiTranslations.EnToPl), DialogsUiTranslations.EnToPl));

        private static readonly Dictionary<string, string> DeToEn = BuildReverseMap(EnToDe);
        private static readonly Dictionary<string, string> RoToEn = BuildReverseMap(EnToRo);
        private static readonly Dictionary<string, string> SvToEn = BuildReverseMap(EnToSv);
        private static readonly Dictionary<string, string> NlToEn = BuildReverseMap(EnToNl);
        private static readonly Dictionary<string, string> PlToEn = BuildReverseMap(EnToPl);

        public static string CurrentLanguage => _currentLanguage;

        public static void SetLanguage(string language)
        {
            _currentLanguage = NormalizeLanguage(language);
        }

        public static string NormalizeLanguage(string language)
        {
            if (string.IsNullOrWhiteSpace(language))
                return English;

            var trimmed = language.Trim();
            if (trimmed.Equals("vi", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("vietnamese", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("tieng viet", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("Tiếng Việt", StringComparison.OrdinalIgnoreCase))
                return Vietnamese;
            if (trimmed.Equals("ph", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("filipino", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("tagalog", StringComparison.OrdinalIgnoreCase))
                return Filipino;
            if (trimmed.Equals("id", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("indonesian", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("bahasa indonesia", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("Bahasa Indonesia", StringComparison.OrdinalIgnoreCase))
                return Indonesian;
            if (trimmed.Equals("pt", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("portuguese", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("português", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("Português", StringComparison.OrdinalIgnoreCase))
                return Portuguese;
            if (trimmed.Equals("ms", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("malay", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("bahasa melayu", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("Bahasa Melayu", StringComparison.OrdinalIgnoreCase))
                return Malay;
            if (trimmed.Equals("ja", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("japanese", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("日本語", StringComparison.OrdinalIgnoreCase))
                return Japanese;
            if (trimmed.Equals("zh", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("chinese", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("中国", StringComparison.OrdinalIgnoreCase))
                return Chinese;
            if (trimmed.Equals("th", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("thai", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("ภาษาไทย", StringComparison.OrdinalIgnoreCase))
                return Thai;
            if (trimmed.Equals("km", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("khmer", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("cambodian", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("កម្ពុជា", StringComparison.OrdinalIgnoreCase))
                return Khmer;
            if (trimmed.Equals("lo", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("lao", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("ພາສາລາວ", StringComparison.OrdinalIgnoreCase))
                return Lao;
            if (trimmed.Equals("ko", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("korean", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("한국어", StringComparison.OrdinalIgnoreCase))
                return Korean;
            if (trimmed.Equals(Russian, StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("ru", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("russian", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("русский", StringComparison.OrdinalIgnoreCase))
                return Russian;
            if (trimmed.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("uk", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("ua", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("ukrainian", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("українська", StringComparison.OrdinalIgnoreCase))
                return Ukrainian;
            if (trimmed.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("es-419", StringComparison.OrdinalIgnoreCase))
                return SpanishLatin;
            if (trimmed.Equals(SpanishArgentina, StringComparison.OrdinalIgnoreCase))
                return SpanishLatin;
            if (trimmed.Equals(French, StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("fr", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("fr-FR", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("français", StringComparison.OrdinalIgnoreCase))
                return French;
            if (trimmed.Equals(Hebrew, StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("he", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("he-IL", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("עברית", StringComparison.OrdinalIgnoreCase))
                return Hebrew;
            if (trimmed.Equals(EnglishCanada, StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("en-CA", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("English (Canada)", StringComparison.OrdinalIgnoreCase))
                return English;
            if (trimmed.Equals(Taiwan, StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("zh-TW", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("zh-Hant", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("繁體中文", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("繁體中文（台灣）", StringComparison.OrdinalIgnoreCase))
                return Taiwan;
            if (trimmed.Equals(Colombia, StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("SpanishColombia", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("es-CO", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("Español (Colombia)", StringComparison.OrdinalIgnoreCase))
                return Colombia;
            if (trimmed.Equals(Turkiye, StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("Turkey", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("Turkish", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("tr", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("tr-TR", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("Türkçe", StringComparison.OrdinalIgnoreCase))
                return Turkiye;
            if (trimmed.Equals(Spain, StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("es-ES", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("Español (España)", StringComparison.OrdinalIgnoreCase))
                return Spain;
            if (trimmed.Equals(Italy, StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("it", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("it-IT", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("Italiano", StringComparison.OrdinalIgnoreCase))
                return Italy;
            if (trimmed.Equals(Chile, StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("es-CL", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("Español (Chile)", StringComparison.OrdinalIgnoreCase))
                return Chile;
            if (trimmed.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("ar-AE", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("UAE", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("العربية (الإمارات)", StringComparison.OrdinalIgnoreCase))
                return UnitedArabEmirates;
            if (trimmed.Equals(Brazil, StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("pt-BR", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("Português (Brasil)", StringComparison.OrdinalIgnoreCase))
                return Brazil;
            if (trimmed.Equals(SouthAfrica, StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("en-ZA", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("English (South Africa)", StringComparison.OrdinalIgnoreCase))
                return English;
            if (trimmed.Equals(German, StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("de", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("Deutsch", StringComparison.OrdinalIgnoreCase))
                return German;
            if (trimmed.Equals(Romanian, StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("ro", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("Română", StringComparison.OrdinalIgnoreCase))
                return Romanian;
            if (trimmed.Equals(Swedish, StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("sv", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("Svenska", StringComparison.OrdinalIgnoreCase))
                return Swedish;
            if (trimmed.Equals(Dutch, StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("nl", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("Nederlands", StringComparison.OrdinalIgnoreCase))
                return Dutch;
            if (trimmed.Equals(Polish, StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("pl", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("Polski", StringComparison.OrdinalIgnoreCase))
                return Polish;
            return English;
        }

        public static string Translate(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;
            string key = text.Trim();
            if (string.Equals(key, "Create a shortcut on your Desktop for quick access to Masterstrap (recommended).", StringComparison.Ordinal))
                key = "Create a shortcut on your Desktop for quick access to Masterstrap (recommended)";
            else if (string.Equals(key, "Create a shortcut on your Desktop for quick access to Masterstrap (recommanded)", StringComparison.OrdinalIgnoreCase)
                     || string.Equals(key, "Create a shortcut on your Desktop for quick access to Masterstrap (recommanded).", StringComparison.OrdinalIgnoreCase))
                key = "Create a shortcut on your Desktop for quick access to Masterstrap (recommended)";
            else if (string.Equals(key, "Desktop icon", StringComparison.OrdinalIgnoreCase))
                key = "Desktop Shortcut";
            else if (string.Equals(key, "Control how detailed meshes appear in game.", StringComparison.OrdinalIgnoreCase))
                key = "Control how detailed meshes appear in-game.";

            string forced = TranslateCommonUiKeys(key);
            if (!string.IsNullOrEmpty(forced))
                return forced;

            if (QuickLaunchShellTranslations.TryTranslate(_currentLanguage, key, out var shellTr))
                return shellTr;
            if (ModsTabUiTranslations.TryTranslate(_currentLanguage, key, out var modsTr))
                return modsTr;
            if (LinkPsAccountManagerUiTranslations.TryTranslate(_currentLanguage, key, out var linkPsAccountTr))
                return linkPsAccountTr;
            if (FastFlagSettingsUiTranslations.TryTranslate(_currentLanguage, key, out var fastFlagTr))
                return fastFlagTr;
            if (AppearanceUiTranslations.TryTranslate(_currentLanguage, key, out var appearanceTr))
                return appearanceTr;
            if (ActivityLogTranslations.TryTranslate(_currentLanguage, key, out var actLogTr))
                return actLogTr;

            if (_currentLanguage.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase))
                return EnToVi.TryGetValue(key, out var vi) ? vi : text;
            if (_currentLanguage.Equals(Filipino, StringComparison.OrdinalIgnoreCase))
                return EnToFil.TryGetValue(key, out var fil) ? fil : text;
            if (_currentLanguage.Equals(Indonesian, StringComparison.OrdinalIgnoreCase))
                return EnToId.TryGetValue(key, out var id) ? id : text;
            if (_currentLanguage.Equals(Portuguese, StringComparison.OrdinalIgnoreCase))
                return EnToPt.TryGetValue(key, out var pt) ? pt : text;
            if (_currentLanguage.Equals(Malay, StringComparison.OrdinalIgnoreCase))
                return EnToMal.TryGetValue(key, out var mal) ? mal : text;
            if (_currentLanguage.Equals(Japanese, StringComparison.OrdinalIgnoreCase))
                return EnToJa.TryGetValue(key, out var ja) ? ja : text;
            if (_currentLanguage.Equals(Chinese, StringComparison.OrdinalIgnoreCase))
                return EnToZh.TryGetValue(key, out var zh) ? zh : text;
            if (_currentLanguage.Equals(Thai, StringComparison.OrdinalIgnoreCase))
                return EnToTh.TryGetValue(key, out var th) ? th : text;
            if (_currentLanguage.Equals(Khmer, StringComparison.OrdinalIgnoreCase))
                return EnToKm.TryGetValue(key, out var km) ? km : text;
            if (_currentLanguage.Equals(Lao, StringComparison.OrdinalIgnoreCase))
                return EnToLo.TryGetValue(key, out var lo) ? lo : text;
            if (_currentLanguage.Equals(Korean, StringComparison.OrdinalIgnoreCase))
                return EnToKo.TryGetValue(key, out var ko) ? ko : text;
            if (_currentLanguage.Equals(Russian, StringComparison.OrdinalIgnoreCase))
                return EnToRu.TryGetValue(key, out var ru) ? ru : text;
            if (_currentLanguage.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase))
                return EnToUk.TryGetValue(key, out var uk) ? uk : text;
            if (_currentLanguage.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase))
                return EnToEsLat.TryGetValue(key, out var es) ? es : text;
            if (_currentLanguage.Equals(French, StringComparison.OrdinalIgnoreCase))
                return EnToFr.TryGetValue(key, out var fr) ? fr : text;
            if (_currentLanguage.Equals(Hebrew, StringComparison.OrdinalIgnoreCase))
                return EnToHe.TryGetValue(key, out var he) ? he : text;
            if (_currentLanguage.Equals(Taiwan, StringComparison.OrdinalIgnoreCase))
                return EnToTw.TryGetValue(key, out var tw) ? tw : text;
            if (_currentLanguage.Equals(Colombia, StringComparison.OrdinalIgnoreCase))
                return EnToEsCo.TryGetValue(key, out var eco) ? eco : text;
            if (_currentLanguage.Equals(Turkiye, StringComparison.OrdinalIgnoreCase))
                return EnToTr.TryGetValue(key, out var tr) ? tr : text;
            if (_currentLanguage.Equals(Spain, StringComparison.OrdinalIgnoreCase))
                return EnToEsSp.TryGetValue(key, out var esp) ? esp : text;
            if (_currentLanguage.Equals(Italy, StringComparison.OrdinalIgnoreCase))
                return EnToIt.TryGetValue(key, out var it) ? it : text;
            if (_currentLanguage.Equals(Chile, StringComparison.OrdinalIgnoreCase))
                return EnToEsCl.TryGetValue(key, out var ecl) ? ecl : text;
            if (_currentLanguage.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase))
                return EnToArAe.TryGetValue(key, out var ar) ? ar : text;
            if (_currentLanguage.Equals(Brazil, StringComparison.OrdinalIgnoreCase))
                return EnToPt.TryGetValue(key, out var ptbr) ? ptbr : text;
            if (_currentLanguage.Equals(German, StringComparison.OrdinalIgnoreCase))
                return EnToDe.TryGetValue(key, out var de) ? de : text;
            if (_currentLanguage.Equals(Romanian, StringComparison.OrdinalIgnoreCase))
                return EnToRo.TryGetValue(key, out var ro) ? ro : text;
            if (_currentLanguage.Equals(Swedish, StringComparison.OrdinalIgnoreCase))
                return EnToSv.TryGetValue(key, out var sv) ? sv : text;
            if (_currentLanguage.Equals(Dutch, StringComparison.OrdinalIgnoreCase))
                return EnToNl.TryGetValue(key, out var nl) ? nl : text;
            if (_currentLanguage.Equals(Polish, StringComparison.OrdinalIgnoreCase))
                return EnToPl.TryGetValue(key, out var pl) ? pl : text;
            if (ViToEn.TryGetValue(key, out var en)) return en;
            if (FilToEn.TryGetValue(key, out en)) return en;
            if (IdToEn.TryGetValue(key, out en)) return en;
            if (PtToEn.TryGetValue(key, out en)) return en;
            if (MalToEn.TryGetValue(key, out en)) return en;
            if (JaToEn.TryGetValue(key, out en)) return en;
            if (ZhToEn.TryGetValue(key, out en)) return en;
            if (ThToEn.TryGetValue(key, out en)) return en;
            if (KmToEn.TryGetValue(key, out en)) return en;
            if (LoToEn.TryGetValue(key, out en)) return en;
            if (KoToEn.TryGetValue(key, out en)) return en;
            if (RuToEn.TryGetValue(key, out en)) return en;
            if (UkToEn.TryGetValue(key, out en)) return en;
            if (EsLatToEn.TryGetValue(key, out en)) return en;
            if (FrToEn.TryGetValue(key, out en)) return en;
            if (HeToEn.TryGetValue(key, out en)) return en;
            if (TwToEn.TryGetValue(key, out en)) return en;
            if (EsColToEn.TryGetValue(key, out en)) return en;
            if (TrToEn.TryGetValue(key, out en)) return en;
            if (EsSpToEn.TryGetValue(key, out en)) return en;
            if (EsClToEn.TryGetValue(key, out en)) return en;
            if (ItToEn.TryGetValue(key, out en)) return en;
            if (ArAeToEn.TryGetValue(key, out en)) return en;
            if (DeToEn.TryGetValue(key, out en)) return en;
            if (RoToEn.TryGetValue(key, out en)) return en;
            if (SvToEn.TryGetValue(key, out en)) return en;
            if (NlToEn.TryGetValue(key, out en)) return en;
            if (PlToEn.TryGetValue(key, out en)) return en;
            return text;
        }

        private static string TranslateCommonUiKeys(string key)
        {
            string lang = _currentLanguage ?? English;

            if (lang.Equals(English, StringComparison.OrdinalIgnoreCase))
            {
                if (key.Equals("mặc định", StringComparison.OrdinalIgnoreCase)) return "Default";
                if (key.Equals("Account", StringComparison.OrdinalIgnoreCase)) return "Account";
                if (key.Equals("enter a lisense key and click Confirm to validate", StringComparison.OrdinalIgnoreCase)) return "Enter a account and click Confirm to validate.";
                return string.Empty;
            }

            if (key.Equals("Update Error", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Lỗi cập nhật";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Error sa Pag-update";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Kesalahan Pembaruan";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Erro de Atualização";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Ralat Kemas Kini";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "アップデートエラー";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "更新错误";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "更新錯誤";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "ข้อผิดพลาดในการอัปเดต";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "កំហុសបច្ចុប្បន្នភាព";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ຂໍ້ຜິດພາດໃນການອັບເດດ";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "업데이트 오류";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Ошибка обновления";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Помилка оновлення";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Error de actualización";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Erreur de mise à jour";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "שגיאת עדכון";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Güncelleme Hatası";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Errore di aggiornamento";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "خطأ في التحديث";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "Update-Fehler";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "Eroare de actualizare";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "Uppdateringsfel";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "Updatefout";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "Błąd aktualizacji";
                return "Update Error";
            }

            if (key.Equals("Update failed:\n{0}\n\nOpening download page instead.", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Cập nhật thất bại:\n{0}\n\nĐang mở trang tải xuống để thay thế.";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Nabigo ang pag-update:\n{0}\n\nBinubuksan ang pahina ng pag-download.";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Pembaruan gagal:\n{0}\n\nMembuka halaman unduhan sebagai gantinya.";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Falha na atualização:\n{0}\n\nAbrindo a página de download.";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Kemas kini gagal:\n{0}\n\nMembuka halaman muat turun sebagai ganti.";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "アップデートに失敗しました:\n{0}\n\n代わりにダウンロードページを開きます。";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "更新失败:\n{0}\n\n正在打开下载页面。";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "更新失敗:\n{0}\n\n正在打開下載頁面。";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "การอัปเดตล้มเหลว:\n{0}\n\nกำลังเปิดหน้าดาวน์โหลดแทน";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "ការធ្វើបច្ចុប្បន្នភាពបានបរាជ័យ:\n{0}\n\nកំពុងបើកទំព័រទាញយកជំនួសវិញ។";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ອັບເດດບໍ່ສຳເລັດ:\n{0}\n\nກຳລังເລີ່ມດາວໂຫຼດແທນ.";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "업데이트 실패:\n{0}\n\n대신 다운로드 페이지를 엽니다.";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Обновление не удалось:\n{0}\n\nОткрытие страницы загрузки.";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Оновлення не вдалося:\n{0}\n\nВідкриття сторінки завантаження.";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Actualización fallida:\n{0}\n\nAbriendo la página de descarga en su lugar.";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Échec de la mise à jour :\n{0}\n\nOuverture de la page de téléchargement.";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "העדכון נכשל:\n{0}\n\nפותח את דף ההורדה במקום זאת.";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Güncelleme başarısız oldu:\n{0}\n\nYenine indirme sayfası açılıyor.";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Aggiornamento fallito:\n{0}\n\nApertura della pagina di download.";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "فشل التحديث:\n{0}\n\nجاري فتح صفحة التنزيل بدلاً من ذلك.";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "Update fehlgeschlagen:\n{0}\n\nDownload-Seite wird stattdessen geöffnet.";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "Actualizarea a eșuat:\n{0}\n\nSe deschide pagina de descărcare în schimb.";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "Uppdateringen misslyckades:\n{0}\n\nÖppnar nedladdningssidan istället.";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "Update mislukt:\n{0}\n\nDownloadpagina wordt geopend.";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "Aktualizacja nie powiodła się:\n{0}\n\nOtwieranie strony pobierania.";
                return key;
            }

            if (key.Equals("Updating Masterstrap", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Đang cập nhật Masterstrap";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Nag-a-update ng Masterstrap";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Memperbarui Masterstrap";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Atualizando o Masterstrap";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Mengemas kini Masterstrap";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "Masterstrap をアップデート中";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "正在更新 Masterstrap";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "正在更新 Masterstrap";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "กำลังอัปเดต Masterstrap";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "កំពុងធ្វើបច្ចុប្បន្នភាព Masterstrap";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ກຳລังອັບເດດ Masterstrap";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "Masterstrap 업데이트 중";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Обновление Masterstrap";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Оновлення Masterstrap";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Actualizando Masterstrap";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Mise à jour de Masterstrap";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "מעדכן את Masterstrap";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Masterstrap güncelleniyor";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Aggiornamento di Masterstrap";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "جاري تحديث Masterstrap";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "Masterstrap wird aktualisiert";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "Se actualizează Masterstrap";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "Uppdaterar Masterstrap";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "Masterstrap bijwerken";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "Aktualizacja Masterstrap";
                return "Updating Masterstrap";
            }

            if (key.Equals("UPDATE REQUIRED", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "YÊU CẦU CẬP NHẬT";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "KAILANGAN NG UPDATE";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "PEMBARUAN DIPERLUKAN";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "ATUALIZAÇÃO OBRIGATÓRIA";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "KEMAS KINI DIPERLUKAN";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "アップデートが必要です";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "需要更新";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "需要更新";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "จำเป็นต้องอัปเดต";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "តម្រូវឱ្យធ្វើបច្ចុប្បន្នភាព";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ຕ້ອງການອັບເດດ";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "업데이트 필요";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "ТРЕБУЕТСЯ ОБНОВЛЕНИЕ";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "ПОТРІБНО ОБНОВЛЕННЯ";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "ACTUALIZACIÓN REQUERIDA";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "MISE À JOUR REQUISE";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "נדרש עדכון";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "GÜNCELLEME GEREKLİ";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "AGGIORNAMENTO RICHIESTO";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "التحديث مطلوب";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "UPDATE ERFORDERLICH";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "UPDATE OBLIGATORIU";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "UPPDATERING KRÄVS";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "UPDATE VEREIST";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "WYMAGANA AKTUALIZACJA";
                return "UPDATE REQUIRED";
            }

            if (key.Equals("A critical update is required to continue using Masterstrap. Please download the latest version immediately.", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Cần có bản cập nhật quan trọng để tiếp tục sử dụng Masterstrap. Vui lòng cập nhật lên phiên bản mới nhất ngay lập tức.";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Kailangan ng kritikal na update para magpatuloy sa paggamit ng Masterstrap. Mangyaring i-download ang pinakabagong bersyon kaagad.";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Pembaruan kritis diperlukan untuk terus menggunakan Masterstrap. Silakan unduh versi terbaru segera.";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Uma atualização crítica é necessária para continuar usando o Masterstrap. Baixe a versão mais recente imediatamente.";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Kemas kini kritikal diperlukan untuk terus menggunakan Masterstrap. Sila muat turun versi terkini dengan segera.";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "Masterstrapを継続して使用するには、重要なアップデートが必要です。すぐに最新バージョンをダウンロードしてください。";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "需要进行关键更新才能继续使用 Masterstrap。请立即下载最新版本。";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "需要進行關鍵更新才能繼續使用 Masterstrap。請立即下載最新版本。";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "จำเป็นต้องอัปเดตที่สำคัญเพื่อใช้ Masterstrap ต่อไป โปรดดาวน์โหลดเวอร์ชันล่าสุดทันที";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "ការធ្វើបច្ចុប្បន្នភាពសំខាន់គឺត្រូវបានតម្រូវដើម្បីបន្តប្រើប្រាស់ Masterstrap។ សូមទាញយកកំណែចុងក្រោយបំផុតភ្លាមៗ។";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ຈຳເປັນຕ້ອງມີການອັບເດດທີ່ສຳຄັນເພື່ອໃຊ້ Masterstrap ຕໍ່ໄປ. ກະລຸນາດາວໂຫຼດເວີຊັນຫຼ້າສຸດທັນທີ.";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "Masterstrap을 계속 사용하려면 중요 업데이트가 필요합니다. 즉시 최신 버전을 다운로드하십시오.";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Для продолжения использования Masterstrap требуется критическое обновление. Пожалуйста, немедленно установите последнюю версию.";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Для продовження використання Masterstrap потрібне критичне оновлення. Будь ласка, негайно встановіть останню версію.";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Se requiere una actualización crítica para continuar usando Masterstrap. Descargue la última versión de inmediato.";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Une mise à jour critique est requise pour continuer à utiliser Masterstrap. Veuillez télécharger immédiatement la dernière version.";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "נדרש עדכון קריטי כדי להמשיך להשתמש ב-Masterstrap. אנא הורד את הגרסה העדכנית ביותר באופן מיידי.";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Masterstrap kullanmaya devam etmek için kritik bir güncelleme gerekiyor. Lütfen en son sürümü hemen indirin.";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "È richiesto un aggiornamento critico per continuare a utilizzare Masterstrap. Si prega di scaricare immediatamente l'ultima versione.";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "مطلوب تحديث هام لمواصلة استخدام Masterstrap. يرجى تنزيل أحدث إصدار على الفور.";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "Ein kritisches Update ist erforderlich, um Masterstrap weiterhin zu nutzen. Bitte laden Sie die neueste Version sofort herunter.";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "Este necesară o actualizare critică pentru a continua să utilizați Masterstrap. Vă rugăm să descărcați imediat cea mai recentă versiune.";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "En kritisk uppdatering krävs för att fortsätta använda Masterstrap. Ladda ner den senaste versionen omedelbart.";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "Een kritieke update is vereist om Masterstrap te blijven gebruiken. Download onmiddellijk de nieuwste versie.";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "Wymagana jest krytyczna aktualizacja, aby kontynuować korzystanie z Masterstrap. Pobierz natychmiast najnowszą wersję.";
                return key;
            }

            if (key.Equals("Your Version:", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Phiên bản của bạn:";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Iyong Bersyon:";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Versi Anda:";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Sua Versão:";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Versi Anda:";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "現在のバージョン:";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "您的版本:";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "您的版本:";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "เวอร์ชันของคุณ:";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "កំណែរបស់អ្នក:";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ເວີຊັນຂອງທ່ານ:";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "현재 버전:";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Ваша версия:";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Ваша версія:";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Tu versión:";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Votre version:";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "הגרסה שלך:";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Sürümünüz:";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "La tua versione:";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "إصدارك:";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "Ihre Version:";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "Versiunea ta:";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "Din version:";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "Jouw versie:";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "Twoja wersja:";
                return "Your Version:";
            }

            if (key.Equals("Latest Version:", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Phiên bản mới nhất:";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Pinakabagong Bersyon:";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Versi Terbaru:";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Versão Mais Recente:";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Versi Terkini:";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "最新のバージョン:";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "最新版本:";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "最新版本:";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "เวอร์ชันล่าสุด:";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "កំណែចុងក្រោយបំផុត:";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ເວີຊັນຫຼ້າສຸດ:";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "최신 버전:";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Последняя версия:";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Остання версія:";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Última versión:";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Dernière version:";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "הגרסה האחרונה:";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "En son sürüm:";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Ultima versione:";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "أحدث إصدار:";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "Neueste Version:";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "Ultima versiune:";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "Senaste versionen:";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "Nieuwste versie:";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "Najnowsza wersja:";
                return "Latest Version:";
            }

            if (key.Equals("Update Now", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Cập nhật ngay";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Mag-update Ngayon";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Perbarui Sekarang";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Atualizar Agora";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Kemas Kini Sekarang";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "今すぐアップデート";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "立即更新";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "立即更新";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "อัปเดตตอนนี้";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "ធ្វើបច្ចុប្បន្នភាពឥឡូវនេះ";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ອັບເດດດຽוນີ້";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "지금 업데이트";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Обновить сейчас";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Оновити зараз";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Actualizar ahora";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Mettre à jour maintenant";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "עדכן כעת";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Şimdi Güncelle";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Aggiorna ora";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "تحديث الآن";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "Jetzt aktualisieren";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "Actualizează acum";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "Uppdatera nu";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "Nu bijwerken";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "Aktualizuj teraz";
                return "Update Now";
            }

            if (key.Equals("Closing this window will exit the application", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Đóng cửa sổ này sẽ thoát ứng dụng";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Ang pagsasara ng window na ito ay lalabas sa application";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Menutup jendela ini akan keluar dari aplikasi";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Fechar esta janela sairá do aplicativo";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Menutup tetingkap ini akan keluar dari aplikasi";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "このウィンドウを閉じるとアプリケーションが終了します";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "关闭此窗口将退出应用程序";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "關閉此視窗將退出應用程式";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "การปิดหน้าต่างนี้จะเป็นการออกจากแอปพลิเคชัน";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "ការបិទបង្អួចនេះនឹងចាកចេញពីកម្មវិធី";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "การປິດຕ່າງນີ້ຈະອອກຈາກແອັບພລິເຄຊັນ";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "이 창을 닫으면 애플리케이션이 종료됩니다";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Закрытие этого окна приведет к выходу из приложения";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Закриття цього вікна призведе до виходу з програми";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Cerrar esta ventana saldrá de la aplicación";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Fermer cette fenêtre quittera l'application";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "סגירת חלון זה תביא ליציאה מהאפליקציה";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Bu pencereyi kapatmak uygulamadan çıkış yapacaktır";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "La chiusura di questa finestra uscirà dall'applicazione";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "إغلاق هذه النافذة سيؤدي إلى الخروج من التطبيق";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "Das Schließen dieses Fensters beendet die Anwendung";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "Închiderea acestei ferestre va închide aplicația";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "Att stänga detta fönster kommer att avsluta programmet";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "Het sluiten van dit venster zal de applicatie afsluiten";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "Zamknięcie tego okna spowoduje wyjście z aplikacji";
                return "Closing this window will exit the application";
            }

            if (key.Equals("Downloading update...", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Đang tải bản cập nhật...";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Dina-download ang update...";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Mengunduh pembaruan...";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Baixando atualização...";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Memuat turun kemas kini...";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "アップデートをダウンロード中...";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "正在下载更新...";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "正在下載更新...";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "กำลังดาวน์โหลดอัปเดต...";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "កំពុងទាញយកបច្ចុប្បន្នភាព...";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ກຳລັງດາວໂຫຼດອັບເດດ...";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "업데이트 다운로드 중...";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Загрузка обновления...";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Завантаження оновлення...";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Descargando actualización...";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Téléchargement de la mise à jour...";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "מוריד עדכון...";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Güncelleme indiriliyor...";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Download dell'aggiornamento...";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "جاري تنزيل التحديث...";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "Update wird heruntergeladen...";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "Se descarcă actualizarea...";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "Laddar ner uppdatering...";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "Update downloaden...";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "Pobieranie aktualizacji...";
                return "Downloading update...";
            }

            if (key.Equals("Downloading... {0}MB / {1}MB", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Đang tải... {0}MB / {1}MB";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Dina-download... {0}MB / {1}MB";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Mengunduh... {0}MB / {1}MB";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Baixando... {0}MB / {1}MB";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Memuat turun... {0}MB / {1}MB";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "ダウンロード中... {0}MB / {1}MB";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "正在下载... {0}MB / {1}MB";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "正在下載... {0}MB / {1}MB";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "กำลังดาวน์โหลด... {0}MB / {1}MB";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "កំពុងទាញយក... {0}MB / {1}MB";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ກຳລັງດາວໂຫຼດ... {0}MB / {1}MB";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "다운로드 중... {0}MB / {1}MB";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Скачивание... {0}MB из {1}MB";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Завантаження... {0}MB з {1}MB";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Descargando... {0}MB / {1}MB";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Téléchargement... {0}Mo / {1}Mo";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "מוריד... {0}MB / {1}MB";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "İndiriliyor... {0}MB / {1}MB";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Download in corso... {0}MB / {1}MB";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "جاري التنزيل... {0} ميجابايت / {1} ميجابايت";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "Wird heruntergeladen... {0}MB / {1}MB";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "Se descarcă... {0}MB / {1}MB";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "Laddar ner... {0}MB / {1}MB";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "Downloaden... {0}MB / {1}MB";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "Pobieranie... {0}MB / {1}MB";
                return "Downloading... {0}MB / {1}MB";
            }

            if (key.Equals("Extracting files...", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Đang giải nén các tệp...";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Ine-extract ang mga tệp...";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Mengekstrak file...";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Extraindo arquivos...";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Mengekstrak fail...";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "ファイルを展開中...";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "正在解压文件...";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "正在解壓文件...";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "กำลังขยายไฟล์...";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "កំពុងស្រង់ចេញឯកសារ...";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ກຳລັງແຕກໄຟລ໌...";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "파일 압축 푸는 중...";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Распаковка файлов...";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Розпакування файлів...";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Extrayendo archivos...";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Extraction des fichiers...";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "מחלץ קבצים...";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Dosyalar ayıklanıyor...";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Estrazione dei file...";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "جاري استخراج الملفات...";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "Dateien werden entpackt...";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "Se extrag fișierele...";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "Extraherar filer...";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "Bestanden uitpakken...";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "Rozpakowywanie plików...";
                return "Extracting files...";
            }

            if (key.Equals("Preparing update...", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Đang chuẩn bị cập nhật...";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Inihahanda ang update...";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Mempersiapkan pembaruan...";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Preparando atualização...";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Menyediakan kemas kini...";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "アップデートの準備中...";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "正在准备更新...";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "正在準備更新...";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "กำลังเตรียมการอัปเดต...";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "កំពុងរៀបចំបច្ចុប្បន្នភាព...";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ກຳລັງກຽມອັບເດດ...";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "업데이트 준비 중...";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Подготовка обновления...";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Підготовка оновлення...";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Preparando atualização...";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Préparation de la mise à jour...";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "מכין עדכון...";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Güncelleme hazırlanıyor...";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Preparazione dell'aggiornamento...";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "جاري التحضير للتحديث...";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "Update wird vorbereitet...";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "Se pregătește actualizarea...";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "Förbereder uppdatering...";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "Update voorbereiden...";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "Przygotowanie aktualizacji...";
                return "Preparing update...";
            }

            if (key.Equals("Update ready! Restarting...", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Bản cập nhật đã sẵn sàng! Đang khởi động lại...";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Handa na ang update! Muling naglalunsad...";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Pembaruan siap! Memulai ulang...";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Atualização pronta! Reiniciando...";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Kemas kini sedia! Memulakan semula...";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "アップデート完了！再起動中...";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "更新就绪！正在重启...";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "更新就緒！正在重啟...";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "อัปเดตพร้อมแล้ว! กำลังเริ่มระบบใหม่...";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "បច្ចុប្បន្នភាពរួចរាល់ហើយ! កំពុងចាប់ផ្តើមឡើងវិញ...";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ອັບເດດພ້ອມແລ้ວ! ກຳລັງເລີ່ມໃໝ່...";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "업데이트 완료! 재시작 중...";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Обновление готово! Перезапуск...";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Оновлення готове! Перезапуск...";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "¡Actualización lista! Reiniciando...";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Mise à jour prête ! Redémarrage...";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "העדכון מוכן! מפעיל מחדש...";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Güncelleme hazır! Yeniden başlatılıyor...";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Aggiornamento pronto! Riavvio in corso...";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "التحديث جاهز! جاري إعادة التشغيل...";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "Update bereit! Neustart...";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "Actualizare gata! Se repornește...";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "Uppdatering klar! Startar om...";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "Update gereed! Opnieuw opstarten...";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "Aktualizacja gotowa! Restartowanie...";
                return "Update ready! Restarting...";
            }

            if (key.Equals("Automatic", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Tự động";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Awtomatiko";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Otomatis";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Automático";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Automatik";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "自動";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "自动";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "自動";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "อัตโนมัติ";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "ស្វ័យប្រវត្តិ";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ອັດຕະໂນມັດ";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "자동";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Автоматически";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Автоматично";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Automático";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Automatique";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "אוטומטי";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Otomatik";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Automatico";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "تلقائي";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "Automatisch";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "Automat";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "Automatisk";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "Automatisch";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "Automatycznie";
                return "Automatic";
            }

            if (key.Equals("Low", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Thấp";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Mababa";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Rendah";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Baixo";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Rendah";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "低";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "低";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "低";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "ต่ำ";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "ទាប";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ຕໍ່າ";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "낮음";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Низкое";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Низька";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Bajo";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Faible";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "נמוך";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Düşük";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Basso";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "منخفض";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "Niedrig";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "Scăzut";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "Låg";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "Laag";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "Niski";
                return "Low";
            }

            if (key.Equals("Medium", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Trung bình";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Katamtaman";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Sedang";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Médio";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Sederhana";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "中";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "中";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "中";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "ปานกลาง";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "មធ្យម";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ປານກາງ";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "중간";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Среднее";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Середня";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Medio";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Moyen";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "בינוני";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Orta";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Medio";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "متوسط";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "Mittel";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "Mediu";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "Medium";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "Gemiddeld";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "Średni";
                return "Medium";
            }

            if (key.Equals("High", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Cao";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Mataas";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Tinggi";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Alto";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Tinggi";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "高";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "高";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "高";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "สูง";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "ខ្ពស់";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ສູງ";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "높음";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Высокое";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Висока";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Alto";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Élevé";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "גבוה";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Yüksek";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Alto";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "مرتفع";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "Hoch";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "Ridicat";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "Hög";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "Hoog";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "Wysoki";
                return "High";
            }

            if (key.Equals("Default", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Mặc định";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Default";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Default";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Padrão";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Lalai";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "デフォルト";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "默认";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "預設";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "ค่าเริ่มต้น";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "លំនាំដើម";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ຄ່າເລີ່ມຕົ້ນ";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "기본값";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "По умолчанию";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "За замовчуванням";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Predeterminado";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Par défaut";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "ברירת מחדל";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Varsayılan";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Predefinito";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "افتراضي";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "Standard";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "Implicit";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "Standard";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "Standaard";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "Domyślny";
                return "Default";
            }

            if (key.Equals("Small", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Nhỏ";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Maliit";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Kecil";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Pequeno";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Kecil";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "小";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "小";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "小";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "เล็ก";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "តូច";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ນ້ອຍ";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "작게";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Маленький";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Мала";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Pequeño";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Petit";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "קטן";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Küçük";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Piccolo";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "صغير";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "Klein";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "Mic";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "Liten";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "Klein";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "Mały";
                return "Small";
            }

            if (key.Equals("Large", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Lớn";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Malaki";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Besar";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Grande";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Besar";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "大";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "大";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "大";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "ใหญ่";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "ធំ";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ໃຫຍ່";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "크게";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Большой";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Велика";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Grande";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Grand";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "גדול";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Büyük";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Grande";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "كبير";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "Groß";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "Mare";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "Stor";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "Groot";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "Duży";
                return "Large";
            }

            if (key.Equals("Auto-apply", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Tự động áp dụng";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Auto-apply";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Apply Otomatis";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Auto-injetar";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Auto-Injek";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "自動インジェクト";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "自动注入";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "自動注入";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "ฉีดอัตโนมัติ";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "ចាក់បញ្ចូលស្វ័យប្រវត្តិ";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ຕິດຕັ້ງອັດຕະໂນມັດ";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "자동 인젝션";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Авто-инъекция";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Авто-ін'єкція";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Auto-inyectar";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Auto-applyer";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "הזרקה אוטומטית";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Otomatik Enjekte";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Auto-iniezione";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "حقн تلقائي";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "Auto-Injektion";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "Auto-applyare";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "Auto-injektion";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "Auto-applyeren";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "Auto-wstrzykiwanie";
                return "Auto-apply";
            }

            if (key.Equals("Manage Flags", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Quản lý cờ";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Pamahalaan ang mga Flag";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Kelola Flag";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Gerenciar flags";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Urus Flag";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "フラグ管理";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "管理标志";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "管理旗標";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "จัดการแฟลก";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "គ្រប់គ្រង flags";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ຈັດການ flags";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "플래그 관리";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Управление флагами";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Керування прапорами";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Gestionar flags";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Gérer les flags";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "ניהول דגלים";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Bayrakları yönet";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Gestisci flag";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "إدارة العلامات";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "Flags verwalten";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "Gestionare flaguri";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "Hantera flaggor";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "Flags beheren";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "Zarządzaj flagami";
                return "Manage Flags";
            }

            if (key.Equals("Rendering", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Kết xuất";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Rendering";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Rendering";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Renderização";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Penyediaan";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "レンダリング";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "渲染";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "渲染";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "การเรนเดอร์";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "ការបង្ហាញរូបភាព";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ການສະແດງຜົນ";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "렌더링";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Рендеринг";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Рендеринг";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Renderizado";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Rendu";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "רינדور";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Oluşturma";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Rendering";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "صيرورة";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "Rendering";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "Randare";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "Rendering";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "Rendering";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "Renderowanie";
                return "Rendering";
            }

            if (key.Equals("Region:", StringComparison.Ordinal))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Khu vực:";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Rehiyon:";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Wilayah:";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Regiao:";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Rantau:";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "地域:";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "地区:";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "地區:";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "ภูมิภาค:";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "តំបន់:";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ພື້ນທີ່:";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "지역:";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Регион:";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Регіон:";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Region:";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Region:";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "אזור:";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Bolge:";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Regione:";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "المنطقة:";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "Region:";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "Regiune:";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "Region:";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "Regio:";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "Region:";
                return "Region:";
            }

            if (key.Equals("Server Size:", StringComparison.Ordinal))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Kich thuoc server:";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Laki ng Server:";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Ukuran Server:";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Tamanho do Servidor:";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Saiz Server:";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "サーバー規模:";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "服务器规模:";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "伺服器規模:";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "ขนาดเซิร์ฟเวอร์:";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "ទំហំម៉ាស៊ីនមេ:";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ຂະໜາດເຊີບເວີ:";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "서버 크기:";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Размер сервера:";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Розмір сервера:";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Tamano del servidor:";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Taille du serveur:";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "גודל שרת:";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Sunucu Boyutu:";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Dimensione server:";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "حجم الخادم:";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "Servergroesse:";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "Dimensiune server:";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "Serverstorlek:";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "Servergrootte:";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "Rozmiar serwera:";
                return "Server Size:";
            }

            if (key.Equals("Auto (Best)", StringComparison.Ordinal))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Tự động (Tốt nhất)";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Auto (Pinakamahusay)";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Otomatis (Terbaik)";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Automatico (Melhor)";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Auto (Terbaik)";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "自動（最適）";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "自动（最佳）";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "自動（最佳）";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "อัตโนมัติ (ดีที่สุด)";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "ស្វ័យប្រវត្តិ (ល្អបំផុត)";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ອັດຕະໂນມັດ (ດີທີ່ສຸດ)";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "자동 (최적)";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Авто (Лучший)";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Авто (Найкращий)";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Automatico (Mejor)";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Auto (Meilleur)";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "אוטומטי (הטוב ביותר)";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Otomatik (En Iyi)";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Automatico (Migliore)";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "تلقائي (الأفضل)";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "Auto (Beste)";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "Auto (Cel mai bun)";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "Auto (Bast)";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "Auto (Beste)";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "Auto (Najlepszy)";
                return "Auto (Best)";
            }

            if (key.Equals("Large Servers", StringComparison.Ordinal))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Server đông";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Malalaking Server";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Server Besar";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Servidores grandes";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Server Besar";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "大規模サーバー";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "大型服务器";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "大型伺服器";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "เซิร์ฟเวอร์ใหญ่";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "ម៉ាស៊ីនមេធំ";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ເຊີບເວີໃຫຍ່";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "대형 서버";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Большие серверы";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Великі сервери";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Servidores grandes";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Grands serveurs";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "שרתים גדולים";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Buyuk Sunucular";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Server grandi";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "خوادم كبيرة";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "Grosse Server";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "Servere mari";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "Stora servrar";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "Grote servers";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "Duze serwery";
                return "Large Servers";
            }

            if (key.Equals("Small Servers", StringComparison.Ordinal))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Server ít người";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Maliit na Server";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Server Kecil";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Servidores pequenos";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Server Kecil";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "小規模サーバー";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "小型服务器";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "小型伺服器";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "เซิร์ฟเวอร์เล็ก";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "ម៉ាស៊ីនមេតូច";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ເຊີບເວີນ້ອຍ";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "소형 서버";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Малые серверы";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Малі сервери";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Servidores pequenos";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Petits serveurs";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "שרתים קטנים";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Kucuk Sunucular";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Server piccoli";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "خوادم صغيرة";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "Kleine Server";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "Servere mici";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "Små servrar";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "Kleine servers";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "Male serwery";
                return "Small Servers";
            }

            if (key.Equals("JOIN SERVER", StringComparison.Ordinal))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "VÀO SERVER";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "SUMALI SA SERVER";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "GABUNG SERVER";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "ENTRAR NO SERVIDOR";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "SERTAI SERVER";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "サーバーに参加";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "加入服务器";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "加入伺服器";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "เข้าเซิร์ฟเวอร์";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "ចូលម៉ាស៊ីនមេ";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ເຂົ້າເຊີບເວີ";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "서버 참가";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "ВОЙТИ НА СЕРВЕР";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "ПРИЄДНАТИСЯ ДО СЕРВЕРА";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "UNIRSE AL SERVIDOR";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "REJOINDRE LE SERVEUR";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "הצטרף לשרת";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "SUNUCUYA KATIL";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "ENTRA NEL SERVER";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "انضم إلى الخادم";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "SERVER BEITRETEN";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "INTRA PE SERVER";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "GÅ MED I SERVER";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "DEELNEMEN AAN SERVER";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "DOŁĄCZ DO SERWERA";
                return "JOIN SERVER";
            }

            if (key.Equals("{0} Friends   {1} Followers   {2} Following", StringComparison.Ordinal))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "{0} Bạn bè   {1} Người theo dõi   {2} Đang theo dõi";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "{0} Kaibigan   {1} Tagasunod   {2} Sinusundan";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "{0} Teman   {1} Pengikut   {2} Mengikuti";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "{0} Amigos   {1} Seguidores   {2} Seguindo";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "{0} Rakan   {1} Pengikut   {2} Mengikuti";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "{0} 友達   {1} フォロワー   {2} フォロー中";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "{0} 好友   {1} 粉丝   {2} 关注中";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "{0} 好友   {1} 追蹤者   {2} 追蹤中";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "{0} เพื่อน   {1} ผู้ติดตาม   {2} กำลังติดตาม";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "{0} មិត្តភក្តិ   {1} អ្នកតាមដាន   {2} កំពុងតាមដាន";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "{0} ໝູ່   {1} ຜູ້ຕິດຕາມ   {2} ກຳລັງຕິດຕາມ";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "{0} 친구   {1} 팔로워   {2} 팔로잉";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "{0} Друзья   {1} Подписчики   {2} Подписки";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "{0} Друзі   {1} Підписники   {2} Підписки";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "{0} Amigos   {1} Seguidores   {2} Siguiendo";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "{0} Amis   {1} Abonnés   {2} Abonnements";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "{0} חברים   {1} עוקבים   {2} עוקב אחרי";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "{0} Arkadas   {1} Takipci   {2} Takip";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "{0} Amici   {1} Follower   {2} Seguiti";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "{0} أصدقاء   {1} متابعون   {2} يتابع";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "{0} Freunde   {1} Follower   {2} Folgt";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "{0} Prieteni   {1} Urmaritori   {2} Urmareste";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "{0} Vanner   {1} Foljare   {2} Foljer";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "{0} Vrienden   {1} Volgers   {2} Volgend";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "{0} Znajomi   {1} Obserwujacy   {2} Obserwuje";
                return "{0} Friends   {1} Followers   {2} Following";
            }

            if (key.Equals("Singapore", StringComparison.Ordinal))
            {
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "シンガポール";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "新加坡";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "新加坡";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "싱가포르";
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Singapore";
                return "Singapore";
            }

            if (key.Equals("United States", StringComparison.Ordinal))
            {
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "アメリカ合衆国";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "美国";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "美國";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "미국";
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Hoa Ky";
                return "United States";
            }

            if (key.Equals("Japan", StringComparison.Ordinal))
            {
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "日本";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "日本";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "일본";
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Nhat Ban";
                return "Japan";
            }

            if (key.Equals("Germany", StringComparison.Ordinal))
            {
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "ドイツ";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "德国";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "德國";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "독일";
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Duc";
                return "Germany";
            }

            if (key.Equals("Finding best server...", StringComparison.Ordinal))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Đang tìm server tốt nhất...";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "最適なサーバーを検索中...";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "正在寻找最佳服务器...";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "正在尋找最佳伺服器...";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "최적 서버 찾는 중...";
                return "Finding best server...";
            }

            if (key.Equals("✔ Found best server: {0} - {1}ms", StringComparison.Ordinal))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "✔ Đã tìm thấy server tốt nhất: {0} - {1}ms";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "✔ 最適サーバーを検出: {0} - {1}ms";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "✔ 已找到最佳服务器: {0} - {1}ms";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "✔ 已找到最佳伺服器: {0} - {1}ms";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "✔ 최적 서버 찾음: {0} - {1}ms";
                return "✔ Found best server: {0} - {1}ms";
            }

            if (key.Equals("⚠ No regional servers found. Using default.", StringComparison.Ordinal))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "⚠ Khong tim thay server theo khu vuc. Su dung mac dinh.";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "⚠ 地域サーバーが見つかりません。既定を使用します。";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "⚠ 未找到对应地区服务器，使用默认。";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "⚠ 找不到對應地區伺服器，改用預設。";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "⚠ 지역 서버를 찾지 못했습니다. 기본값을 사용합니다.";
                return "⚠ No regional servers found. Using default.";
            }

            if (key.Equals("Unlock 240FPS", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Mở khóa 240FPS";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "I-unlock ang 240FPS";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Buka kunci 240FPS";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Desbloquear 240FPS";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Buka kunci 240FPS";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "240FPSのロック解除";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "解锁 240FPS";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "解鎖 240FPS";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "ปลดล็อก 240FPS";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "ដោះសោ 240FPS";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ປົດລັອກ 240FPS";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "240FPS 잠금 해제";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Разблокировать 240FPS";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Розблокувати 240FPS";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Desbloquear 240FPS";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Déverrouiller 240FPS";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "ביטול נעילת 240FPS";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "240FPS Kilidini Aç";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Sblocca 240FPS";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "فتح 240FPS";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "240FPS entsperren";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "Deblochează 240FPS";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "Lås upp 240FPS";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "Ontgrendel 240FPS";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "Odblokuj 240FPS";
                return "Unlock 240FPS";
            }

            if (key.Equals("Choose how FPS unlocking works: OFF keeps 240 FPS, Global uses Global tab Framerate Limit, FFlag uses package FPS flags.", StringComparison.Ordinal)
                || key.Equals("Choose how FPS unlocking works: OFF keeps 240 FPS, Global applies your Framerate Limit in settings then a native OS boost on apply when available, FFlag uses package FPS flags.", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Chọn cách mở khóa FPS hoạt động: Tắt giữ 240 FPS, Toàn cục áp dụng Framerate Limit trong cài đặt rồi tăng tốc native OS khi apply nếu có, FFlag dùng cờ FPS theo gói.";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Piliin kung paano gagana ang FPS unlock: OFF ay mananatili sa 240 FPS, Global ay ilalapat ang Framerate Limit sa settings at native OS boost sa apply kung available, FFlag ay gagamit ng package FPS flags.";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Pilih cara kerja unlock FPS: OFF tetap 240 FPS, Global menerapkan Framerate Limit di pengaturan lalu peningkatan OS native saat apply jika tersedia, FFlag memakai flag FPS berbasis paket.";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Escolha como o desbloqueio de FPS funciona: OFF mantém 240 FPS, Global aplica o limite de FPS nas configurações e um impulso nativo do SO na injeção quando disponível, FFlag usa flags de FPS por pacote.";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Pilih cara buka kunci FPS berfungsi: OFF kekalkan 240 FPS, Global guna Had Framerate dalam tetapan kemudian peningkatan OS asli semasa apply jika tersedia, FFlag guna flag FPS mengikut pakej.";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "FPSアンロックの動作を選択: OFFは240 FPS固定、Globalは設定のFramerate Limitを適用し、利用可能なら注入時にOSネイティブのブースト、FFlagはパッケージFPSフラグを使用。";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "选择 FPS 解锁方式：OFF 保持 240 FPS，Global 在设置中应用帧率上限并在注入时尽可能使用系统原生提升，FFlag 使用套餐 FPS 标志。";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "選擇 FPS 解鎖方式：OFF 維持 240 FPS，Global 在設定套用幀率上限並在注入時盡可能使用系統原生提升，FFlag 使用方案 FPS 旗標。";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "เลือกวิธีการปลดล็อก FPS: OFF คง 240 FPS, Global ใช้ Framerate Limit ในการตั้งค่าแล้วเพิ่มประสิทธิภาพ OS แบบเนทิฟตอน apply เมื่อมี, FFlag ใช้แฟล็ก FPS ตามแพ็กเกจ";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "ជ្រើសរើសរបៀបដោះសោ FPS៖ OFF រក្សា 240 FPS, Global អនុវត្ត Framerate Limit ក្នុងការកំណត់ រួចបង្កើនប្រព័ន្ធចល័តនៅពេល apply បើមាន, FFlag ប្រើ flags FPS តាម package។";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ເລືອກວິທີການປົດລັອກ FPS: OFF ຄົງ 240 FPS, Global ນຳໃຊ້ Framerate Limit ໃນການຕັ້ງຄ່າ ແລ້ວເພີ່ມປະສິດທິພາບ OS ໃນ apply ເມື່ອມີ, FFlag ໃຊ້ flag FPS ຕາມ package.";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "FPS 잠금 해제 방식 선택: OFF는 240 FPS 유지, Global은 설정의 Framerate Limit을 적용하고 주입 시 가능하면 OS 네이티브 부스트, FFlag는 패키지 FPS 플래그를 사용합니다.";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Выберите режим разблокировки FPS: OFF оставляет 240 FPS, Global применяет лимит кадров в настройках и нативное ускорение ОС при инжекте, если доступно, FFlag использует пакетные FPS-флаги.";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Оберіть режим розблокування FPS: OFF залишає 240 FPS, Global застосовує Framerate Limit у налаштуваннях і нативне прискорення ОС під час інжекту за наявності, FFlag використовує пакетні FPS-прапорці.";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Elige cómo funciona el desbloqueo de FPS: OFF mantiene 240 FPS, Global aplica tu límite de FPS en ajustes y un impulso nativo del SO al inyectar si está disponible, FFlag usa los flags FPS por paquete.";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Choisissez le mode de déverrouillage FPS : OFF garde 240 FPS, Global applique la limite d’images dans les réglages puis une accélération native de l’OS à l’application si disponible, FFlag utilise les flags FPS par package.";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "בחר איך ביטול נעילת FPS יעבוד: OFF שומר על 240 FPS, Global מיישם את מגבלת הפריימים בהגדרות ואז דחיפה נייטיבית של מערכת ההפעלה בהזרקה כשזמין, FFlag משתמש בדגלי FPS לפי חבילה.";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "FPS kilidinin nasıl çalışacağını seçin: OFF 240 FPS'te tutar, Global ayarlardaki Framerate Limit'i uygular ve mümkünse enjekte sırasında yerel OS artışı kullanır, FFlag paket FPS bayraklarını kullanır.";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Scegli come funziona lo sblocco FPS: OFF mantiene 240 FPS, Global applica il Framerate Limit nelle impostazioni e un boost nativo dell’OS all’iniezione se disponibile, FFlag usa i flag FPS del pacchetto.";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "اختر طريقة عمل فتح FPS: وضع OFF يبقي 240 FPS، وGlobal يطبق حد الإطارات في الإعدادات ثم تعزيزًا أصليًا من نظام التشغيل عند الحقن عند التوفر، وFFlag يستخدم أعلام FPS الخاصة بالحزمة.";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "Wähle, wie FPS-Unlock funktioniert: OFF hält 240 FPS, Global wendet dein Framerate-Limit in den Einstellungen an und bei Injektion ggf. einen nativen OS-Boost, FFlag nutzt paketbasierte FPS-Flags.";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "Alege cum funcționează deblocarea FPS: OFF păstrează 240 FPS, Global aplică limita de cadre în setări și un impuls nativ al SO la injecție când e disponibil, FFlag folosește flag-urile FPS pe pachet.";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "Välj hur FPS-upplåsning fungerar: OFF håller 240 FPS, Global tillämpar din bildfrekvensgräns i inställningar och en inbyggd OS-boost vid injektion när det finns, FFlag använder paketets FPS-flaggor.";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "Kies hoe FPS-ontgrendeling werkt: OFF houdt 240 FPS, Global past je framerate-limiet in instellingen toe en een native OS-boost bij Toepassing wanneer beschikbaar, FFlag gebruikt pakket-FPS-flags.";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "Wybierz sposób działania odblokowania FPS: OFF utrzymuje 240 FPS, Global stosuje limit FPS w ustawieniach i natywne przyspieszenie OS przy wstrzyknięciu, gdy dostępne, FFlag używa flag FPS pakietu.";
                return key;
            }

            if (key.Equals("OFF", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "TẮT";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "ARRÊT";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "DESLIGADO";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "APAGADO";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "AUS";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "SPENTO";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "ВЫКЛ";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "ВИМК";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "WYŁ.";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "KAPALI";
                return "OFF";
            }

            if (key.Equals("FFlag", StringComparison.OrdinalIgnoreCase))
                return "FFlag";

            if (key.Equals("Appearance", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Giao diện";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Hitsura";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Tampilan";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase)) return "Aparência";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Paparan";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "外観";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "外观";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "รูปลักษณ์";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "រូបរាង";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ຮູບແບບ";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "모양";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Оформление";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Вигляд";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase)) return "Apariencia";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Apparence";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "מראה";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "外觀";
                if (lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase)) return "Apariencia";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Görünüm";
                if (lang.Equals(Spain, StringComparison.OrdinalIgnoreCase)) return "Apariencia";
                if (lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Apariencia";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Aspetto";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "المظهر";
                return "Appearance";
            }

            if (key.Equals("General", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Chung";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Pangkalahatan";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Umum";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase)) return "Geral";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Umum";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "一般";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "常规";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "ทั่วไป";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "ទូទៅ";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ທົ່ວໄປ";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "일반";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Общие";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Загальні";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase)) return "General";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Général";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "כללי";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "一般";
                if (lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase)) return "General";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Genel";
                if (lang.Equals(Spain, StringComparison.OrdinalIgnoreCase)) return "General";
                if (lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "General";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Generale";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "عام";
                return "General";
            }

            if (key.Equals("Geometry", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Hình học";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Heometriya";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Geometri";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Geometria";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Geometri";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "ジオメトリ";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "几何";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "เรขาคณิต";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "ធរណីមាត្រ";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ເລຂາຄະນິດ";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "기하";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Геометрия";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Геометрія";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Geometría";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Géométrie";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "גאומטריה";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "幾何";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Geometri";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Geometria";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "الهندسة";
                return "Geometry";
            }

            if (key.Equals("Mesh detail", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Độ chi tiết mesh";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Detalye ng mesh";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Detail mesh";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Detalhe da malha";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Perincian mesh";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "メッシュ詳細";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "网格细节";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "รายละเอียดเมช";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "កម្រិតលម្អិតមេស";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ລາຍລະອຽດເມັດ";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "메시 디테일";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Детализация мешей";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Деталізація мешів";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Detalle de malla";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Détail du maillage";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "פירוט רשת";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "網格細節";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Mesh ayrıntısı";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Dettaglio mesh";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "تفاصيل الشبكة";
                return "Mesh detail";
            }

            if (key.Equals("Control how detailed meshes appear in-game.", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Điều chỉnh mức độ chi tiết của mesh hiển thị trong game.";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Kontrolin kung gaano kadetalye ang mga mesh sa laro.";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Atur seberapa detail mesh yang tampil di dalam game.";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Controle o nível de detalhe das malhas exibidas no jogo.";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Kawal tahap perincian mesh yang dipaparkan dalam permainan.";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "ゲーム内で表示されるメッシュの詳細度を調整します。";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "控制游戏中网格显示的细节程度。";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "ควบคุมระดับความละเอียดของเมชที่แสดงในเกม";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "គ្រប់គ្រងកម្រិតលម្អិតនៃមេសដែលបង្ហាញក្នុងហ្គេម។";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ຄວບຄຸມລະດັບລາຍລະອຽດຂອງເມັດທີ່ສະແດງໃນເກມ.";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "게임 내 메시에 표시되는 디테일 수준을 제어합니다.";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Управляет уровнем детализации мешей в игре.";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Керує рівнем деталізації мешів у грі.";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Controla el nivel de detalle de las mallas que se muestran en el juego.";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Contrôle le niveau de détail des maillages affichés en jeu.";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "שלוט ברמת הפירוט של הרשתות שמופיעות במשחק.";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "控制遊戲內網格顯示的細節程度。";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Oyunda görünen mesh ayrıntı düzeyini kontrol edin.";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Controlla il livello di dettaglio delle mesh visualizzate in gioco.";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "تحكم في مستوى تفاصيل الشبكات التي تظهر داخل اللعبة.";
                return "Control how detailed meshes appear in-game.";
            }

            if (key.Equals("Start Menu icon", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Biểu tượng menu Start";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Icon ng Start Menu";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Ikon menu Start";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Ícone do menu Iniciar";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Ikon menu Start";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "スタートメニューのアイコン";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "“开始”菜单图标";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "開始功能表圖示";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "ไอคอนเมนู Start";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "រូបតំណាងម៉ឺនុយ Start";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ໄອຄອນເມນູ Start";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "시작 메뉴 아이콘";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Значок меню «Пуск»";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Піктограма меню «Пуск»";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Icono del menú Inicio";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Icône du menu Démarrer";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "סמל תפריט התחל";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Başlat menüsü simgesi";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Icona del menu Start";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "أيقونة قائمة ابدأ";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "Startmenü-Symbol";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "Pictogramă meniu Start";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "Startmenyikon";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "Startmenu-pictogram";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "Ikona menu Start";
                return "Start Menu icon";
            }

            if (key.Equals("Roblox Launch Interception", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Chặn khởi chạy Roblox";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Pagharang sa paglulunsad ng Roblox";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Intersepsi peluncuran Roblox";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Interceptação do lançamento do Roblox";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Pencegatan pelancaran Roblox";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "Roblox 起動のインターセプト";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "拦截 Roblox 启动";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "攔截 Roblox 啟動";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "ดักจับการเปิด Roblox";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "ការចាប់ផ្ដើម Roblox ដោយរារាំង";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ສະກັດການເປີດ Roblox";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "Roblox 실행 가로채기";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Перехват запуска Roblox";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Перехоплення запуску Roblox";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Interceptación del inicio de Roblox";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Interception du lancement de Roblox";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "יירוט הפעלה של Roblox";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Roblox başlatma yakalama";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Intercettazione avvio Roblox";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "اعتراض تشغيل Roblox";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "Roblox-Start abfangen";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "Interceptare lansare Roblox";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "Roblox-start avlyssning";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "Roblox-start onderscheppen";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "Przechwytywanie uruchomienia Roblox";
                return "Roblox Launch Interception";
            }

            if (key.Equals("When disabled, Roblox will launch directly without Masterstrap application.", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Khi tắt, Roblox sẽ mở trực tiếp mà không qua áp dụng Masterstrap.";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Kapag naka-off, diretso magbubukas ang Roblox nang walang application ng Masterstrap.";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Jika dimatikan, Roblox akan langsung dibuka tanpa injeksi Masterstrap.";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Quando desativado, o Roblox abre diretamente sem injeção do Masterstrap.";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Apabila dimatikan, Roblox akan dilancarkan terus tanpa suntikan Masterstrap.";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "オフにすると、Masterstrap の注入なしで Roblox が直接起動します。";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "关闭后，Roblox 将直接启动，不会经过 Masterstrap 注入。";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "關閉後，Roblox 將直接啟動，不經過 Masterstrap 注入。";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "เมื่อปิด Roblox จะเปิดโดยตรงโดยไม่มีการฉีด Masterstrap";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "នៅពេលបិទ Roblox នឹងបើកដោយផ្ទាល់ដោយគ្មានការចាក់ Masterstrap។";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ເມື່ອປິດ Roblox ຈະເປີດໂດຍກົງໂດຍບໍ່ມີການສັກ Masterstrap.";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "끄면 Masterstrap 주입 없이 Roblox가 바로 실행됩니다.";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Если отключено, Roblox запускается напрямую без инжекции Masterstrap.";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Якщо вимкнено, Roblox запускається напряму без інжекції Masterstrap.";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Si está desactivado, Roblox se abrirá directamente sin inyección de Masterstrap.";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Désactivé : Roblox démarre directement sans application Masterstrap.";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "כבוי: Roblox ייפתח ישירות בלי הזרקת Masterstrap.";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Kapalıyken Roblox, Masterstrap enjeksiyonu olmadan doğrudan açılır.";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Se disattivato, Roblox si avvia direttamente senza iniezione Masterstrap.";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "عند التعطيل، يُشغَّل Roblox مباشرةً دون حقن Masterstrap.";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "Wenn aus, startet Roblox direkt ohne Masterstrap-Injektion.";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "Dezactivat: Roblox pornește direct fără injecție Masterstrap.";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "Av: Roblox startar direkt utan Masterstrap-injektion.";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "Uit: Roblox start direct zonder Masterstrap-Toepassing.";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "Wyłączone: Roblox uruchamia się bezpośrednio bez wstrzyknięcia Masterstrap.";
                return "When disabled, Roblox will launch directly without Masterstrap application.";
            }

            if (key.Equals("Preserve rendering quality with display scaling", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Giữ chất lượng kết xuất khi thu phóng hiển thị";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Panatilihin ang rendering quality sa display scaling";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Pertahankan kualitas rendering dengan penskalaan tampilan";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Preservar qualidade de renderização com escala de exibição";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Kekalkan kualiti rendering dengan penskalaan paparan";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "ディスプレイ拡大縮小でも描画品質を維持";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "在显示缩放时保持渲染质量";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "在顯示縮放時維持彩現品質";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "รักษาคุณภาพการเรนเดอร์เมื่อปรับสเกลจอ";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "រក្សាគុណភាពរូបភាពនៅពេលមាន display scaling";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ຮັກສາຄຸນນະພາບການເຣນເດີເມື່ອມີ display scaling";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "디스플레이 배율에서 렌더링 품질 유지";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Сохранять качество рендеринга при масштабировании дисплея";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Зберігати якість рендерингу при масштабуванні дисплея";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Conservar la calidad de renderizado con el escalado de pantalla";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Préserver la qualité de rendu avec le redimensionnement d’affichage";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "שמירה על איכות רינדור עם קנה מידה של התצוגה";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Görüntü ölçeklendirmede işleme kalitesini koru";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Mantieni la qualità di rendering con il ridimensionamento del display";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "الحفاظ على جودة العرض مع تحجيم الشاشة";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "Renderqualität bei Anzeigeskalierung beibehalten";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "Păstrează calitatea randării la scalarea afișajului";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "Behåll renderingskvalitet vid skalning";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "Renderkwaliteit behouden bij schaal van beeld";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "Zachowaj jakość renderowania przy skalowaniu wyświetlacza";
                return "Preserve rendering quality with display scaling";
            }

            if (key.Equals("Roblox reduces your rendering quality depending on how your display is scaled in Windows.", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Roblox giảm chất lượng kết xuất tùy theo mức thu phóng hiển thị trong Windows.";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Binabawasan ng Roblox ang rendering quality depende sa display scaling sa Windows.";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Roblox menurunkan kualitas rendering sesuai penskalaan tampilan di Windows.";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "O Roblox reduz a qualidade de renderização conforme a escala de exibição no Windows.";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Roblox mengurangkan kualiti rendering bergantung pada penskalaan paparan dalam Windows.";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "Roblox は Windows のディスプレイ拡大率に応じて描画品質を下げます。";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "Roblox 会根据 Windows 中的显示缩放降低渲染质量。";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "Roblox 會依 Windows 的顯示縮放降低彩現品質。";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "Roblox จะลดคุณภาพการเรนเดอร์ตามการปรับสเกลจอใน Windows";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "Roblox បន្ថយគុណភាពរូបភាពតាមការពង្រីកបង្រួមបញ្ចាំងនៅក្នុង Windows។";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "Roblox ຫຼຸດຄຸນນະພາບການເຣນເດີຕາມການປັບມາດຕະຖານຈໍໃນ Windows.";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "Roblox는 Windows의 디스플레이 배율에 따라 렌더링 품질을 낮춥니다.";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Roblox снижает качество рендеринга в зависимости от масштаба дисплея в Windows.";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Roblox знижує якість рендерингу залежно від масштабу дисплея в Windows.";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Roblox reduce la calidad de renderizado según el escalado de pantalla en Windows.";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Roblox réduit la qualité de rendu selon le facteur d’échelle d’affichage dans Windows.";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "Roblox מפחית את איכות הרינדור לפי קנה המידה של התצוגה ב-Windows.";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Roblox, Windows’taki görüntü ölçeğine bağlı olarak işleme kalitesini düşürür.";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Roblox riduce la qualità di rendering in base al ridimensionamento del display in Windows.";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "يخفّض Roblox جودة العرض حسب مقياس الشاشة في Windows.";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "Roblox senkt die Renderqualität je nach Anzeigeskalierung in Windows.";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "Roblox reduce calitatea randării în funcție de scalarea afișajului în Windows.";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "Roblox sänker renderingskvaliteten beroende på skalning i Windows.";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "Roblox verlaagt de renderkwaliteit afhankelijk van de schaal in Windows.";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "Roblox obniża jakość renderowania w zależności od skalowania wyświetlacza w systemie Windows.";
                return "Roblox reduces your rendering quality depending on how your display is scaled in Windows.";
            }

            if (key.Equals("Disable player shadows", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Tắt bóng nhân vật";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "I-disable ang mga shadow ng player";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Nonaktifkan bayangan pemain";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Desativar sombras do jogador";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Lumpuhkan bayang pemain";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "プレイヤーの影を無効化";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "禁用玩家阴影";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "停用玩家陰影";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "ปิดเงาตัวละคร";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "បិទស្រមោលអ្នកលេង";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ປິດເງົາຜູ້ຫຼິ້ນ";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "플레이어 그림자 끄기";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Отключить тени игрока";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Вимкнути тіні гравця";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Desactivar sombras del jugador";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Désactiver les ombres du joueur";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "בטל צללי שחקן";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Oyuncu gölgelerini kapat";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Disabilita ombre del giocatore";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "تعطيل ظلال اللاعب";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "Spielerschatten deaktivieren";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "Dezactivează umbrele jucătorului";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "Inaktivera spelarskuggor";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "Spelersschaduwen uitschakelen";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "Wyłącz cienie gracza";
                return "Disable player shadows";
            }

            if (key.Equals("Disables character/player shadow rendering for performance.", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Tắt vẽ bóng nhân vật để tăng hiệu năng.";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Ina-off ang shadow rendering para sa performance.";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Menonaktifkan rendering bayangan karakter untuk performa.";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Desativa a renderização de sombras do personagem para desempenho.";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Melumpuhkan rendering bayang watak untuk prestasi.";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "パフォーマンスのためキャラクター/プレイヤーの影描画を無効化します。";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "为提升性能，禁用角色/玩家阴影渲染。";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "為提升效能，停用角色／玩家陰影繪製。";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "ปิดการเรนเดอร์เงาตัวละครเพื่อประสิทธิภาพ";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "បិទការបង្ហាញស្រមោលតួអង្គដើម្បីដំណើរការលឿន។";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ປິດການເຣນເດີເງົາຕົວລະຄອນເພື່ອປະສິດທິພາບ.";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "성능을 위해 캐릭터/플레이어 그림자 렌더링을 끕니다.";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Отключает отрисовку теней персонажа для производительности.";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Вимикає відтворення тіней персонажа для продуктивності.";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Desactiva el renderizado de sombras del personaje para rendimiento.";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Désactive le rendu des ombres du personnage pour les performances.";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "משבית צללים של הדמות לשיפור ביצועים.";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Performans için karakter/oyuncu gölge oluşturmayı kapatır.";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Disabilita il rendering delle ombre del personaggio per le prestazioni.";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "يعطّل رسم ظلال الشخصية لتحسين الأداء.";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "Deaktiviert Schatten von Charakteren/Spielern für bessere Leistung.";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "Dezactivează umbrele personajului pentru performanță.";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "Stänger av skuggrendering för karaktär/spelare för prestanda.";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "Schakelt schaduwen van personage/speler uit voor prestaties.";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "Wyłącza renderowanie cieni postaci/gracza dla wydajności.";
                return "Disables character/player shadow rendering for performance.";
            }

            if (key.Equals("Disable post-processing effects", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Tắt hiệu ứng hậu kỳ";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "I-disable ang post-processing effects";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Nonaktifkan efek pasca-pemrosesan";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Desativar efeitos de pós-processamento";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Lumpuhkan kesan post-processing";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "ポストプロセス効果を無効化";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "禁用后期处理效果";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "停用後製效果";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "ปิดเอฟเฟกต์หลังประมวลผล";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "បិទបែបផែនក្រោយប្រតិបត្តិការ";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ປິດເອັບເຟັກຫຼັງປະມວນຜົນ";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "후처리 효과 끄기";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Отключить постобработку";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Вимкнути постобробку";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Desactivar efectos de posprocesado";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Désactiver les effets de post-traitement";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "בטל אפקטי עיבוד לאחר";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Son işleme efektlerini kapat";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Disabilita effetti post-elaborazione";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "تعطيل تأثيرات ما بعد المعالجة";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "Nachbearbeitungseffekte deaktivieren";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "Dezactivează efectele de post-procesare";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "Inaktivera efterbehandlingseffekter";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "Nabewerkingseffecten uitschakelen";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "Wyłącz efekty postprocessingu";
                return "Disable post-processing effects";
            }

            if (key.Equals("Disables post FX like bloom/blur/DOF for performance.", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Tắt hiệu ứng hậu kỳ như bloom/mờ/DOF để tăng hiệu năng.";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Ina-off ang post FX tulad ng bloom/blur/DOF para sa performance.";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Menonaktifkan post-FX seperti bloom/blur/DOF untuk performa.";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Desativa pós-efeitos como bloom/blur/DOF para desempenho.";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Melumpuhkan post FX seperti bloom/kabur/DOF untuk prestasi.";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "bloom/ぼかし/DOF などのポストFXをパフォーマンスのため無効化します。";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "为提升性能，禁用 bloom/模糊/景深等后处理特效。";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "為提升效能，停用 bloom／模糊／景深等後製特效。";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "ปิดเอฟเฟกต์หลังประมวลผล เช่น bloom/blur/DOF เพื่อประสิทธิภาพ";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "បិទ post FX ដូចជា bloom/blur/DOF ដើម្បីដំណើរការលឿន។";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ປິດ post FX ເຊັ່ນ bloom/blur/DOF ເພື່ອປະສິດທິພາບ.";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "bloom/블러/DOF 같은 후처리 FX를 성능을 위해 끕니다.";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Отключает пост-эффекты (bloom/размытие/DOF) для производительности.";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Вимикає пост-ефекти (bloom/розмиття/DOF) для продуктивності.";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Desactiva post-FX como bloom/desenfoque/DOF para rendimiento.";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Désactive les post-FX (flou bloom/DOF) pour les performances.";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "משבית אפקטי פוסט כמו bloom/טשטוש/DOF לשיפור ביצועים.";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Performans için bloom/bulanık/DOF gibi son-işlem FX’lerini kapatır.";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Disabilita post-FX come bloom/sfocatura/DOF per le prestazioni.";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "يعطّل مؤثرات ما بعد المعالجة مثل bloom/ضبابية/DOF لتحسين الأداء.";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "Deaktiviert Post-FX wie Bloom/Unschärfe/DOF für bessere Leistung.";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "Dezactivează post-FX precum bloom/blur/DOF pentru performanță.";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "Stänger av post-FX som bloom/sudd/DOF för prestanda.";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "Schakelt post-FX zoals bloom/vervaging/DOF uit voor prestaties.";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "Wyłącza post-FX (bloom/rozmycie/DOF) dla wydajności.";
                return "Disables post FX like bloom/blur/DOF for performance.";
            }

            if (key.Equals("Disable terrain textures", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Tắt kết cấu địa hình";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "I-disable ang terrain textures";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Nonaktifkan tekstur medan";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Desativar texturas do terreno";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Lumpuhkan tekstur rupa bumi";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "地形テクスチャを無効化";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "禁用地形纹理";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "停用地形紋理";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "ปิดเท็กซ์เจอร์ภูมิประเทศ";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "បិទវាលរូបភាពដី";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ປິດເທັກເຈີພື້ນທີ່";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "지형 텍스처 끄기";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Отключить текстуры ландшафта";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Вимкнути текстури рельєфу";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Desactivar texturas del terreno";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Désactiver les textures du terrain";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "בטל מרקמי שטח";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Arazi dokularını kapat";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Disabilita texture del terreno";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "تعطيل نسيج التضاريس";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "Geländetexturen deaktivieren";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "Dezactivează texturile terenului";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "Inaktivera terrängtexturer";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "Terreintextures uitschakelen";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "Wyłącz tekstury terenu";
                return "Disable terrain textures";
            }

            if (key.Equals("Disables terrain material textures for performance.", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Tắt kết cấu vật liệu địa hình để tăng hiệu năng.";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Ina-off ang terrain material textures para sa performance.";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Menonaktifkan tekstur material medan untuk performa.";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Desativa texturas de material do terreno para desempenho.";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Melumpuhkan tekstur bahan rupa bumi untuk prestasi.";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "パフォーマンスのため地形マテリアルのテクスチャを無効化します。";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "为提升性能，禁用地形材质纹理。";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "為提升效能，停用地形材質紋理。";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "ปิดเท็กซ์เจอร์วัสดุภูมิประเทศเพื่อประสิทธิภาพ";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "បិទវាលរូបភាពសម្ភារៈដីដើម្បីដំណើរការលឿន។";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ປິດເທັກເຈີວັດສະດຸພື້ນທີ່ເພື່ອປະສິດທິພາບ.";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "성능을 위해 지형 재질 텍스처를 끕니다.";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Отключает текстуры материалов ландшафта для производительности.";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Вимикає текстури матеріалів рельєфу для продуктивності.";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Desactiva texturas de materiales del terreno para rendimiento.";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Désactive les textures de matériaux du terrain pour les performances.";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "משבית מרקמי חומר של שטח לשיפור ביצועים.";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Performans için arazi malzeme dokularını kapatır.";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Disabilita le texture dei materiali del terreno per le prestazioni.";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "يعطّل نسيج مواد التضاريس لتحسين الأداء.";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "Deaktiviert Geländematerial-Texturen für bessere Leistung.";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "Dezactivează texturile materialelor terenului pentru performanță.";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "Stänger av terrängmaterialtexturer för prestanda.";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "Schakelt terreinmateriaaltexturen uit voor prestaties.";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "Wyłącza tekstury materiałów terenu dla wydajności.";
                return "Disables terrain material textures for performance.";
            }

            if (key.Equals("Preferred lighting technology", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Công nghệ chiếu sáng ưu tiên";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Preferred na lighting technology";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Teknologi pencahayaan pilihan";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Tecnologia de iluminação preferida";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Teknologi pencahayaan pilihan";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "優先するライティング技術";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "首选光照技术";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "偏好的光源技術";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "เทคโนโลยีแสงที่ต้องการ";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "បច្ចេកទេសពន្លឺដែលចង់ប្រើ";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ເທັກໂນໂລຢີແສງທີ່ຕ້ອງການ";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "선호 조명 기술";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Предпочитаемая технология освещения";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Бажана технологія освітлення";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Tecnología de iluminación preferida";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Technologie d’éclairage préférée";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "טכנולוגיית תאורה מועדפת";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Tercih edilen aydınlatma teknolojisi";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Tecnologia di illuminazione preferita";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "تقنية الإضاءة المفضلة";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "Bevorzugte Beleuchtungstechnologie";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "Tehnologia de iluminare preferată";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "Föredraget ljussystem";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "Voorkeursbelichtingstechnologie";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "Preferowana technika oświetlenia";
                return "Preferred lighting technology";
            }

            if (key.Equals("Force a lighting technology. Changes apply on next launch.", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Ép dùng một công nghệ chiếu sáng. Thay đổi áp dụng ở lần mở tiếp theo.";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Pilitin ang lighting technology. Mag-a-apply sa susunod na launch.";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Memaksa teknologi pencahayaan. Berlaku pada peluncuran berikutnya.";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Força uma tecnologia de iluminação. As mudanças valem no próximo lançamento.";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Paksa teknologi pencahayaan. Perubahan dikenakan pada pelancaran seterusnya.";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "ライティング技術を強制します。変更は次回起動から適用されます。";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "强制使用某种光照技术。更改在下次启动时生效。";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "強制使用某種光源技術。變更於下次啟動生效。";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "บังคับใช้เทคโนโลยีแสง การเปลี่ยนแปลงมีผลในการเปิดครั้งถัดไป";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "បង្ខំបច្ចេកទេសពន្លើ។ ការផ្លាស់ប្តូរនឹងអនុវត្តនៅពេលបើកបន្ទាប់។";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ບັງຄັບເທັກໂນໂລຢີແສງ. ການປ່ຽນແປງໃຊ້ຕອນເປີດຄັ້ງຫນ້າ.";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "조명 기술을 강제합니다. 변경 사항은 다음 실행부터 적용됩니다.";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Принудительно задать технологию освещения. Изменения применятся при следующем запуске.";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Примусово встановити технологію освітлення. Зміни застосуються під час наступного запуску.";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Fuerza una tecnología de iluminación. Los cambios se aplican en el próximo inicio.";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Force une technologie d’éclairage. Les changements s’appliquent au prochain lancement.";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "כפה טכנולוגיית תאורה. השינויים יחולו בהפעלה הבאה.";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Bir aydınlatma teknolojisini zorunlu kılar. Değişiklikler bir sonraki başlatmada uygulanır.";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Forza una tecnologia di illuminazione. Le modifiche si applicano al prossimo avvio.";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "فرض تقنية إضاءة. تُطبَّق التغييرات عند التشغيل التالي.";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "Erzwingt eine Beleuchtungstechnologie. Änderungen gelten beim nächsten Start.";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "Forțează o tehnologie de iluminare. Modificările se aplică la următoarea lansare.";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "Tvinga ljussystem. Ändringar gäller vid nästa start.";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "Dwingt een belichtingstechnologie af. Wijzigingen gelden bij de volgende start.";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "Wymuszaj technikę oświetlenia. Zmiany obowiązują przy następnym uruchomieniu.";
                return "Force a lighting technology. Changes apply on next launch.";
            }

            if (key.Equals("Chosen by game", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Do trò chơi chọn";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Pinili ng laro";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Dipilih oleh game";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Escolhido pelo jogo";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Dipilih oleh permainan";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "ゲームに任せる";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "由游戏决定";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "由遊戲決定";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "ตามที่เกมเลือก";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "ជ្រើសដោយហ្គេម";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ເລືອກໂດຍເກມ";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "게임이 선택";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Как в игре";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Як у грі";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Elegido por el juego";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Choisi par le jeu";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "נקבע על ידי המשחק";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Oyunun seçtiği";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Scelto dal gioco";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "يختاره اللعبة";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "Vom Spiel gewählt";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "Ales de joc";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "Valt av spelet";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "Gekozen door de game";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "Wybrane przez grę";
                return "Chosen by game";
            }

            if (key.Equals("Status: VALID ({0}) - Expires: {1}", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Trạng thái: HỢP LỆ ({0}) - Hết hạn: {1}";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Status: VALID ({0}) - Mag-e-expire: {1}";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Status: VALID ({0}) - Kedaluwarsa: {1}";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Status: VÁLIDA ({0}) - Expira em: {1}";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Status: SAH ({0}) - Tamat tempoh: {1}";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "状態: 有効 ({0}) - 有効期限: {1}";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "状态：有效 ({0}) - 到期：{1}";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "狀態：有效 ({0}) - 到期：{1}";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "สถานะ: ใช้ได้ ({0}) - หมดอายุ: {1}";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "ស្ថានភាព៖ ត្រឹមត្រូវ ({0}) - ផុតកំណត់៖ {1}";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ສະຖານະ: ຖືກຕ້ອງ ({0}) - ໝົດອາຍຸ: {1}";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "상태: 유효 ({0}) - 만료: {1}";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Статус: ДЕЙСТВУЕТ ({0}) — истекает: {1}";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Стан: ДІЙСНИЙ ({0}) — закінчується: {1}";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Estado: VÁLIDA ({0}) — vence: {1}";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "État : VALIDE ({0}) — expire le : {1}";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "סטטוס: תקף ({0}) — תפוגה: {1}";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Durum: GEÇERLİ ({0}) — bitiş: {1}";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Stato: VALIDA ({0}) — scade: {1}";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "الحالة: صالح ({0}) — تنتهي في: {1}";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "Status: GÜLTIG ({0}) — läuft ab: {1}";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "Stare: VALIDĂ ({0}) — expiră: {1}";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "Status: GILTIG ({0}) — utgår: {1}";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "Status: GELDIG ({0}) — verloopt: {1}";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "Status: WAŻNY ({0}) — wygasa: {1}";
                return "Status: VALID ({0}) - Expires: {1}";
            }

            if (key.Equals("Status: VALID ({0})", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Trạng thái: HỢP LỆ ({0})";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Status: VALID ({0})";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Status: VALID ({0})";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Status: VÁLIDA ({0})";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Status: SAH ({0})";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "状態: 有効 ({0})";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "状态：有效 ({0})";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "狀態：有效 ({0})";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "สถานะ: ใช้ได้ ({0})";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "ស្ថានភាព៖ ត្រឹមត្រូវ ({0})";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ສະຖານະ: ຖືກຕ້ອງ ({0})";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "상태: 유효 ({0})";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Статус: ДЕЙСТВУЕТ ({0})";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Стан: ДІЙСНИЙ ({0})";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Estado: VÁLIDA ({0})";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "État : VALIDE ({0})";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "סטטוס: תקף ({0})";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Durum: GEÇERLİ ({0})";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Stato: VALIDA ({0})";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "الحالة: صالح ({0})";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "Status: GÜLTIG ({0})";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "Stare: VALIDĂ ({0})";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "Status: GILTIG ({0})";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "Status: GELDIG ({0})";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "Status: WAŻNY ({0})";
                return "Status: VALID ({0})";
            }

            if (key.Equals("Status: INVALID ({0})", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Trạng thái: KHÔNG HỢP LỆ ({0})";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Status: INVALID ({0})";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Status: TIDAK VALID ({0})";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Status: INVÁLIDA ({0})";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Status: TIDAK SAH ({0})";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "状態: 無効 ({0})";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "状态：无效 ({0})";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "狀態：無效 ({0})";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "สถานะ: ไม่ถูกต้อง ({0})";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "ស្ថានភាព៖ មិនត្រឹមត្រូវ ({0})";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ສະຖານະ: ບໍ່ຖືກຕ້ອງ ({0})";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "상태: 무효 ({0})";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Статус: НЕДЕЙСТВИТЕЛЬНО ({0})";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Стан: НЕДІЙСНИЙ ({0})";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Estado: INVÁLIDA ({0})";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "État : INVALIDE ({0})";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "סטטוס: לא תקף ({0})";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Durum: GEÇERSİZ ({0})";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Stato: NON VALIDA ({0})";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "الحالة: غير صالح ({0})";
                if (lang.Equals(German, StringComparison.OrdinalIgnoreCase)) return "Status: UNGÜLTIG ({0})";
                if (lang.Equals(Romanian, StringComparison.OrdinalIgnoreCase)) return "Stare: INVALIDĂ ({0})";
                if (lang.Equals(Swedish, StringComparison.OrdinalIgnoreCase)) return "Status: OGILTIG ({0})";
                if (lang.Equals(Dutch, StringComparison.OrdinalIgnoreCase)) return "Status: ONGELDIG ({0})";
                if (lang.Equals(Polish, StringComparison.OrdinalIgnoreCase)) return "Status: NIEWAŻNY ({0})";
                return "Status: INVALID ({0})";
            }

            if (key.Equals("Build Flag", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Tạo cờ";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Gumawa ng flag";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Buat flag";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Criar flag";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Bina flag";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "フラグ作成";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "创建标志";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "สร้างแฟลก";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "បង្កើត flag";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ສ້າງ flag";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "플래그 생성";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Создать флаг";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Створити прапор";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Crear flag";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Créer un flag";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "צור דגל";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "建立旗標";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Bayrak oluştur";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Crea flag";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "إنشاء علامة";
                return "Build Flag";
            }

            if (key.Equals("Show Preset Flags", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Hiện cờ preset";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Ipakita ang preset flags";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Tampilkan preset flag";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Mostrar flags predefinidas";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Tunjukkan flag praset";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "プリセットフラグを表示";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "显示预设标志";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "แสดงแฟลกพรีเซ็ต";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "បង្ហាញ preset flags";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ສະແດງ preset flags";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "프리셋 플래그 표시";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Показать пресет-флаги";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Показати пресет-прапори";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Mostrar flags preestablecidos";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Afficher les flags prédéfinis";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "הצג דגלים מוגדרים מראש";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "顯示預設旗標";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Hazır bayrakları göster";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Mostra flag preimpostati";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "عرض العلامات المسبقة";
                return "Show Preset Flags";
            }

            if (key.Equals("Telemetry", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Dữ liệu đo lường";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Telemetriya";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Telemetri";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Telemetria";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Telemetri";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "テレメトリ";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "遥测";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "เทเลเมทรี";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "ទិន្នន័យតេលេមេទ្រី";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ເທເລເມຕຣີ";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "텔레메트리";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Телеметрия";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Телеметрія";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Telemetría";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Télémétrie";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "טלמטריה";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "遙測";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Telemetri";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Telemetria";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "القياس عن بعد";
                return "Telemetry";
            }

            if (key.Equals("Debug", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Gỡ lỗi";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Debug";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Debug";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Depuração";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Nyahpepijat";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "デバッグ";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "调试";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "ดีบัก";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "Debug";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ດີບັກ";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "디버그";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Отладка";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Налагодження";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Depuración";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Débogage";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "ניפוי שגיאות";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "除錯";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Hata ayıklama";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Debug";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "تصحيح الأخطاء";
                return "Debug";
            }

            if (key.Equals("Memory/Cache", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Bộ nhớ/Bộ đệm";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Memory/Cache";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Memori/Cache";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Memória/Cache";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Memori/Cache";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "メモリ/キャッシュ";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "内存/缓存";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "หน่วยความจำ/แคช";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "អង្គចងចាំ/ឃ្លាំងសម្ងាត់";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ໜ່ວຍຄວາມຈຳ/ແຄຊ";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "메모리/캐시";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Память/Кэш";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Пам'ять/Кеш";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Memoria/Caché";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Mémoire/Cache";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "זיכרון/מטמון";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "記憶體/快取";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Bellek/Önbellek";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Memoria/Cache";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "الذاكرة/التخزين المؤقت";
                return "Memory/Cache";
            }

            if (key.Equals("Avatar/Character", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Avatar/Nhân vật";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Avatar/Character";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Avatar/Karakter";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Avatar/Personagem";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Avatar/Karakter";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "アバター/キャラクター";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "头像/角色";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "อวาตาร์/ตัวละคร";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "រូបតំណាង/តួអង្គ";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ອະວາຕ້າ/ຕົວລະຄອນ";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "아바타/캐릭터";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Аватар/Персонаж";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Аватар/Персонаж";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Avatar/Personaje";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Avatar/Personnage";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "אווטאר/דמות";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "頭像/角色";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Avatar/Karakter";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Avatar/Personaggio";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "الأفاتار/الشخصية";
                return "Avatar/Character";
            }

            if (key.Equals("Security", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Bảo mật";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Seguridad";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Keamanan";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) || lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase)) return "Segurança";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Keselamatan";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "セキュリティ";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "安全";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "ความปลอดภัย";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "សុវត្ថិភាព";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ຄວາມປອດໄພ";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "보안";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Безопасность";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Безпека";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase) || lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase) || lang.Equals(Spain, StringComparison.OrdinalIgnoreCase) || lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Seguridad";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Sécurité";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "אבטחה";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "安全性";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Güvenlik";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Sicurezza";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "الأمان";
                return "Security";
            }

            if (key.Equals("These are the shortcuts that bring up the multi-choice launch menu.", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Đây là các lối tắt dùng để mở menu khởi chạy nhiều lựa chọn.";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Ito ang mga shortcut na nagbubukas ng multi-choice launch menu.";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Ini adalah shortcut yang membuka menu peluncuran multi-pilihan.";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase)) return "Estes são os atalhos que abrem o menu de inicialização com múltiplas opções.";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Ini ialah pintasan yang membuka menu pelancaran pelbagai pilihan.";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "これらは複数選択の起動メニューを開くショートカットです。";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "这些快捷方式用于打开多选启动菜单。";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "นี่คือทางลัดสำหรับเปิดเมนูเปิดใช้งานแบบหลายตัวเลือก";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "នេះជាផ្លូវកាត់សម្រាប់បើកម៉ឺនុយបើកដំណើរការជម្រើសច្រើន។";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ນີ້ແມ່ນທາງລັດທີ່ໃຊ້ເປີດເມນູເລີ່ມຕົ້ນແບບຫຼາຍຕົວເລືອກ.";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "여러 선택 실행 메뉴를 여는 바로가기입니다.";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Это ярлыки, которые открывают меню запуска с несколькими вариантами.";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Це ярлики, які відкривають меню запуску з кількома варіантами.";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase)) return "Estos son los accesos directos que abren el menú de inicio con múltiples opciones.";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Ce sont les raccourcis qui ouvrent le menu de lancement multi-choix.";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "אלו קיצורי הדרך שפותחים את תפריט ההפעלה מרובה-האפשרויות.";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "這些捷徑可開啟多選啟動選單。";
                if (lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase)) return "Estos son los accesos directos que abren el menú de inicio con múltiples opciones.";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Bunlar, çoklu seçenekli başlatma menüsünü açan kısayollardır.";
                if (lang.Equals(Spain, StringComparison.OrdinalIgnoreCase)) return "Estos son los accesos directos que abren el menú de inicio con múltiples opciones.";
                if (lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Estos son los accesos directos que abren el menú de inicio con múltiples opciones.";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Questi sono i collegamenti che aprono il menu di avvio a scelta multipla.";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "هذه هي الاختصارات التي تفتح قائمة التشغيل متعددة الخيارات.";
                return "These are the shortcuts that bring up the multi-choice launch menu.";
            }

            if (key.Equals("Create a Windows shortcut that runs Save and Launch directly.", StringComparison.OrdinalIgnoreCase))
            {
                if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase)) return "Tạo lối tắt Windows để chạy Lưu và Khởi động trực tiếp.";
                if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase)) return "Gumawa ng Windows shortcut na direktang nagpapatakbo ng Save and Launch.";
                if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase)) return "Buat pintasan Windows yang langsung menjalankan Save and Launch.";
                if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase)) return "Crie um atalho do Windows que execute Salvar e Iniciar diretamente.";
                if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase)) return "Cipta pintasan Windows yang terus menjalankan Save and Launch.";
                if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase)) return "Save and Launch を直接実行する Windows ショートカットを作成します。";
                if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase)) return "创建一个可直接运行“保存并启动”的 Windows 快捷方式。";
                if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase)) return "สร้างทางลัด Windows ที่รัน Save and Launch ได้โดยตรง";
                if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase)) return "បង្កើតផ្លូវកាត់ Windows ដើម្បីដំណើរការ Save and Launch ដោយផ្ទាល់។";
                if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase)) return "ສ້າງທາງລັດ Windows ທີ່ເຮັດວຽກ Save and Launch ໂດຍກົງ.";
                if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase)) return "Save and Launch를 바로 실행하는 Windows 바로가기를 만듭니다.";
                if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase)) return "Создайте ярлык Windows, который напрямую запускает Save and Launch.";
                if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase)) return "Створіть ярлик Windows, який напряму запускає Save and Launch.";
                if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase)) return "Crea un acceso directo de Windows que ejecute Guardar y Lanzar directamente.";
                if (lang.Equals(French, StringComparison.OrdinalIgnoreCase)) return "Créez un raccourci Windows qui exécute Enregistrer et Lancer directement.";
                if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase)) return "צור קיצור דרך ב-Windows שמריץ ישירות את Save and Launch.";
                if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase)) return "建立可直接執行「儲存並啟動」的 Windows 捷徑。";
                if (lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase)) return "Crea un acceso directo de Windows que ejecute Guardar y Lanzar directamente.";
                if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase)) return "Doğrudan Save and Launch çalıştıran bir Windows kısayolu oluşturun.";
                if (lang.Equals(Spain, StringComparison.OrdinalIgnoreCase)) return "Crea un acceso directo de Windows que ejecute Guardar y Lanzar directamente.";
                if (lang.Equals(Chile, StringComparison.OrdinalIgnoreCase)) return "Crea un acceso directo de Windows que ejecute Guardar y Lanzar directamente.";
                if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase)) return "Crea un collegamento Windows che esegue direttamente Salva e Avvia.";
                if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase)) return "أنشئ اختصار Windows يقوم بتشغيل الحفظ والتشغيل مباشرة.";
                return "Create a Windows shortcut that runs Save and Launch directly.";
            }

            bool isCheckVersion = key.Equals("Check Version", StringComparison.OrdinalIgnoreCase);
            bool isGlobalSettings = key.Equals("Global Settings", StringComparison.OrdinalIgnoreCase);
            bool isGlobalTheme = key.Equals("Global Theme", StringComparison.OrdinalIgnoreCase);
            bool isInstallingRoblox = key.Equals("Installing Roblox", StringComparison.OrdinalIgnoreCase);
            bool isLaunchingRoblox = key.Equals("Launching Roblox", StringComparison.OrdinalIgnoreCase);
            bool isApplyWord = key.Equals("Apply", StringComparison.OrdinalIgnoreCase) || key.Equals("⚡ FFlags", StringComparison.OrdinalIgnoreCase);
            bool isUpdatingMasterstrap = key.Equals("Updating Masterstrap", StringComparison.OrdinalIgnoreCase);
            bool isThemeDesc = key.Equals("Choose app visual mode: Default, Glassmorphic, or Glassmorphic + Blur.", StringComparison.OrdinalIgnoreCase)
                            || key.Equals("Choose app visual mode; Default, Glassmorphic or Glassmorphic + blur", StringComparison.OrdinalIgnoreCase);
            bool isDefaultVi = key.Equals("mặc định", StringComparison.OrdinalIgnoreCase) || key.Equals("Default", StringComparison.OrdinalIgnoreCase);
            bool isLicenseKey = key.Equals("Account", StringComparison.OrdinalIgnoreCase) || key.Equals("Account", StringComparison.OrdinalIgnoreCase);
            bool isLicenseHint = key.Equals("enter a lisense key and click Confirm to validate", StringComparison.OrdinalIgnoreCase)
                              || key.Equals("Enter a account and click Confirm to validate.", StringComparison.OrdinalIgnoreCase);
            bool isGetKey = key.Equals("Get key", StringComparison.OrdinalIgnoreCase) || key.Equals("Get Key", StringComparison.OrdinalIgnoreCase);
            bool isCheckKey = key.Equals("Confirm", StringComparison.OrdinalIgnoreCase);
            bool isStatusPrefix = key.Equals("Status: VALID (Active) - Expires:", StringComparison.OrdinalIgnoreCase);
            bool isCancel = key.Equals("cancel", StringComparison.OrdinalIgnoreCase) || key.Equals("Cancel", StringComparison.OrdinalIgnoreCase);

            if (lang.Equals(Vietnamese, StringComparison.OrdinalIgnoreCase))
            {
                if (isCheckVersion) return "Kiểm tra phiên bản";
                if (isGlobalSettings) return "Cài đặt toàn cục";
                if (isGlobalTheme) return "Chủ đề toàn cục";
                if (isInstallingRoblox) return "Đang cài đặt Roblox";
                if (isLaunchingRoblox) return "Đang khởi động Roblox";
                if (isApplyWord) return "Áp dụng";
                if (isUpdatingMasterstrap) return "Đang cập nhật Masterstrap";
                if (isThemeDesc) return "Chọn chế độ giao diện: Mặc định, Glassmorphic, hoặc Glassmorphic + Blur.";
                if (isDefaultVi) return "mặc định";
                if (isLicenseKey) return "Khóa giấy phép";
                if (isLicenseHint) return "Nhập khóa giấy phép và nhấn Confirm để xác thực.";
                if (isGetKey) return "Lấy key";
                if (isCheckKey) return "Kiểm tra key";
                if (isStatusPrefix) return "Trạng thái: HỢP LỆ (Hoạt động) - Hết hạn:";
                if (isCancel) return "hủy";
            }
            else if (lang.Equals(Filipino, StringComparison.OrdinalIgnoreCase))
            {
                if (isCheckVersion) return "Suriin ang Bersyon";
                if (isGlobalSettings) return "Pangkalahatang Settings";
                if (isGlobalTheme) return "Global Theme";
                if (isInstallingRoblox) return "Ini-install ang Roblox";
                if (isLaunchingRoblox) return "Inilulunsad ang Roblox";
                if (isApplyWord) return "apply";
                if (isUpdatingMasterstrap) return "Ina-update ang Masterstrap";
                if (isThemeDesc) return "Piliin ang visual mode ng app: Default, Glassmorphic, o Glassmorphic + Blur.";
                if (isDefaultVi) return "Default";
                if (isLicenseKey) return "Account";
                if (isLicenseHint) return "Ilagay ang account at i-click ang Confirm para ma-validate.";
                if (isGetKey) return "Kunin ang key";
                if (isCheckKey) return "Suriin ang key";
                if (isStatusPrefix) return "Status: VALID (Active) - Expiration:";
                if (isCancel) return "Cancel";
            }
            else if (lang.Equals(Indonesian, StringComparison.OrdinalIgnoreCase))
            {
                if (isCheckVersion) return "Periksa Versi";
                if (isGlobalSettings) return "Pengaturan Global";
                if (isGlobalTheme) return "Tema Global";
                if (isInstallingRoblox) return "Menginstal Roblox";
                if (isLaunchingRoblox) return "Meluncurkan Roblox";
                if (isApplyWord) return "apply";
                if (isUpdatingMasterstrap) return "Memperbarui Masterstrap";
                if (isThemeDesc) return "Pilih mode visual aplikasi: Default, Glassmorphic, atau Glassmorphic + Blur.";
                if (isDefaultVi) return "Default";
                if (isLicenseKey) return "Kunci Lisensi";
                if (isLicenseHint) return "Masukkan kunci lisensi lalu klik Confirm untuk validasi.";
                if (isGetKey) return "Ambil key";
                if (isCheckKey) return "Periksa key";
                if (isStatusPrefix) return "Status: VALID (Aktif) - Kedaluwarsa:";
                if (isCancel) return "Batal";
            }
            else if (lang.Equals(Portuguese, StringComparison.OrdinalIgnoreCase) ||
                     lang.Equals(Brazil, StringComparison.OrdinalIgnoreCase))
            {
                if (isCheckVersion) return "Verificar Versao";
                if (isGlobalSettings) return "Configuracoes Globais";
                if (isGlobalTheme) return "Tema Global";
                if (isInstallingRoblox) return "Instalando Roblox";
                if (isLaunchingRoblox) return "Iniciando Roblox";
                if (isApplyWord) return "apply";
                if (isUpdatingMasterstrap) return "Atualizando Masterstrap";
                if (isThemeDesc) return "Escolha o modo visual do app: Default, Glassmorphic ou Glassmorphic + Blur.";
                if (isDefaultVi) return "Padrao";
                if (isLicenseKey) return "Chave de Licenca";
                if (isLicenseHint) return "Insira uma chave de licenca e clique em Confirm para validar.";
                if (isGetKey) return "Obter key";
                if (isCheckKey) return "Verificar key";
                if (isStatusPrefix) return "Status: VALIDO (Ativo) - Expira em:";
                if (isCancel) return "Cancelar";
            }
            else if (lang.Equals(Malay, StringComparison.OrdinalIgnoreCase))
            {
                if (isCheckVersion) return "Semak Versi";
                if (isGlobalSettings) return "Tetapan Global";
                if (isGlobalTheme) return "Tema Global";
                if (isInstallingRoblox) return "Memasang Roblox";
                if (isLaunchingRoblox) return "Melancarkan Roblox";
                if (isApplyWord) return "apply";
                if (isUpdatingMasterstrap) return "Mengemas kini Masterstrap";
                if (isThemeDesc) return "Pilih mod visual aplikasi: Default, Glassmorphic, atau Glassmorphic + Blur.";
                if (isDefaultVi) return "Default";
                if (isLicenseKey) return "Kunci Lesen";
                if (isLicenseHint) return "Masukkan kunci lesen dan klik Confirm untuk sahkan.";
                if (isGetKey) return "Dapatkan key";
                if (isCheckKey) return "Semak key";
                if (isStatusPrefix) return "Status: SAH (Aktif) - Tamat tempoh:";
                if (isCancel) return "Batal";
            }
            else if (lang.Equals(Japanese, StringComparison.OrdinalIgnoreCase))
            {
                if (isCheckVersion) return "バージョン確認";
                if (isGlobalSettings) return "グローバル設定";
                if (isGlobalTheme) return "グローバルテーマ";
                if (isInstallingRoblox) return "Roblox をインストール中";
                if (isLaunchingRoblox) return "Roblox を起動中";
                if (isApplyWord) return "インジェクト";
                if (isUpdatingMasterstrap) return "Masterstrap を更新中";
                if (isThemeDesc) return "アプリの表示モードを選択: Default、Glassmorphic、または Glassmorphic + Blur。";
                if (isDefaultVi) return "デフォルト";
                if (isLicenseKey) return "ライセンスキー";
                if (isLicenseHint) return "ライセンスキーを入力し、Confirm をクリックして確認します。";
                if (isGetKey) return "キー取得";
                if (isCheckKey) return "キー確認";
                if (isStatusPrefix) return "状態: 有効 (Active) - 有効期限:";
                if (isCancel) return "キャンセル";
            }
            else if (lang.Equals(Chinese, StringComparison.OrdinalIgnoreCase))
            {
                if (isCheckVersion) return "检查版本";
                if (isGlobalSettings) return "全局设置";
                if (isGlobalTheme) return "全局主题";
                if (isInstallingRoblox) return "正在安装 Roblox";
                if (isLaunchingRoblox) return "正在启动 Roblox";
                if (isApplyWord) return "套用";
                if (isUpdatingMasterstrap) return "正在更新 Masterstrap";
                if (isThemeDesc) return "选择应用视觉模式: Default、Glassmorphic 或 Glassmorphic + Blur。";
                if (isDefaultVi) return "默认";
                if (isLicenseKey) return "许可证密钥";
                if (isLicenseHint) return "输入许可证密钥并点击 Confirm 进行验证。";
                if (isGetKey) return "获取密钥";
                if (isCheckKey) return "检查密钥";
                if (isStatusPrefix) return "状态: 有效 (Active) - 到期:";
                if (isCancel) return "取消";
            }
            else if (lang.Equals(Thai, StringComparison.OrdinalIgnoreCase))
            {
                if (isCheckVersion) return "ตรวจสอบเวอร์ชัน";
                if (isGlobalSettings) return "การตั้งค่าทั่วไป";
                if (isGlobalTheme) return "ธีมทั้งหมด";
                if (isInstallingRoblox) return "กำลังติดตั้ง Roblox";
                if (isLaunchingRoblox) return "กำลังเปิด Roblox";
                if (isApplyWord) return "apply";
                if (isUpdatingMasterstrap) return "กำลังอัปเดต Masterstrap";
                if (isThemeDesc) return "เลือกโหมดหน้าตาแอป: Default, Glassmorphic หรือ Glassmorphic + Blur";
                if (isDefaultVi) return "ค่าเริ่มต้น";
                if (isLicenseKey) return "คีย์ไลเซนส์";
                if (isLicenseHint) return "กรอกคีย์ไลเซนส์แล้วคลิก Confirm เพื่อตรวจสอบ";
                if (isGetKey) return "รับคีย์";
                if (isCheckKey) return "ตรวจสอบคีย์";
                if (isStatusPrefix) return "สถานะ: ใช้งานได้ (Active) - หมดอายุ:";
                if (isCancel) return "ยกเลิก";
            }
            else if (lang.Equals(Khmer, StringComparison.OrdinalIgnoreCase))
            {
                if (isCheckVersion) return "ពិនិត្យកំណែ";
                if (isGlobalSettings) return "ការកំណត់សកល";
                if (isGlobalTheme) return "រចនាប័ទ្មសកល";
                if (isInstallingRoblox) return "កំពុងដំឡើង Roblox";
                if (isLaunchingRoblox) return "កំពុងបើក Roblox";
                if (isApplyWord) return "apply";
                if (isUpdatingMasterstrap) return "កំពុងអាប់ដេត Masterstrap";
                if (isThemeDesc) return "ជ្រើសរើសរបៀបរូបរាងកម្មវិធី: Default, Glassmorphic ឬ Glassmorphic + Blur។";
                if (isDefaultVi) return "លំនាំដើម";
                if (isLicenseKey) return "សោអាជ្ញាប័ណ្ណ";
                if (isLicenseHint) return "បញ្ចូលសោអាជ្ញាប័ណ្ណ ហើយចុច Confirm ដើម្បីផ្ទៀងផ្ទាត់។";
                if (isGetKey) return "យក key";
                if (isCheckKey) return "ពិនិត្យ key";
                if (isStatusPrefix) return "ស្ថានភាព៖ មានសុពលភាព (Active) - ផុតកំណត់:";
                if (isCancel) return "បោះបង់";
            }
            else if (lang.Equals(Lao, StringComparison.OrdinalIgnoreCase))
            {
                if (isCheckVersion) return "ກວດສອບເວີຊັນ";
                if (isGlobalSettings) return "ການຕັ້ງຄ່າທົ່ວໄປ";
                if (isGlobalTheme) return "ຮູບແບບທົ່ວໄປ";
                if (isInstallingRoblox) return "ກຳລັງຕິດຕັ້ງ Roblox";
                if (isLaunchingRoblox) return "ກຳລັງເປີດ Roblox";
                if (isApplyWord) return "apply";
                if (isUpdatingMasterstrap) return "ກຳລັງອັບເດດ Masterstrap";
                if (isThemeDesc) return "ເລືອກໂໝດຮູບແບບແອັບ: Default, Glassmorphic ຫຼື Glassmorphic + Blur.";
                if (isDefaultVi) return "ຄ່າເລີ່ມຕົ້ນ";
                if (isLicenseKey) return "ຄີໄລເຊັນ";
                if (isLicenseHint) return "ໃສ່ຄີໄລເຊັນ ແລ້ວກົດ Confirm ເພື່ອກວດສອບ.";
                if (isGetKey) return "ເອົາ key";
                if (isCheckKey) return "ກວດ key";
                if (isStatusPrefix) return "ສະຖານະ: ໃຊ້ໄດ້ (Active) - ໝົດອາຍຸ:";
                if (isCancel) return "ຍົກເລີກ";
            }
            else if (lang.Equals(Korean, StringComparison.OrdinalIgnoreCase))
            {
                if (isCheckVersion) return "버전 확인";
                if (isGlobalSettings) return "전역 설정";
                if (isGlobalTheme) return "전역 테마";
                if (isInstallingRoblox) return "Roblox 설치 중";
                if (isLaunchingRoblox) return "Roblox 실행 중";
                if (isApplyWord) return "적용";
                if (isUpdatingMasterstrap) return "Masterstrap 업데이트 중";
                if (isThemeDesc) return "앱 시각 모드 선택: Default, Glassmorphic 또는 Glassmorphic + Blur.";
                if (isDefaultVi) return "기본값";
                if (isLicenseKey) return "라이선스 키";
                if (isLicenseHint) return "라이선스 키를 입력하고 Confirm를 눌러 검증하세요.";
                if (isGetKey) return "키 받기";
                if (isCheckKey) return "키 확인";
                if (isStatusPrefix) return "상태: 유효 (Active) - 만료:";
                if (isCancel) return "취소";
            }
            else if (lang.Equals(Russian, StringComparison.OrdinalIgnoreCase))
            {
                if (isCheckVersion) return "Проверить версию";
                if (isGlobalSettings) return "Глобальные настройки";
                if (isGlobalTheme) return "Глобальная тема";
                if (isInstallingRoblox) return "Установка Roblox";
                if (isLaunchingRoblox) return "Запуск Roblox";
                if (isApplyWord) return "Применить";
                if (isUpdatingMasterstrap) return "Обновление Masterstrap";
                if (isThemeDesc) return "Выберите режим оформления: по умолчанию, Glassmorphic или Glassmorphic с размытием.";
                if (isDefaultVi) return "по умолчанию";
                if (isLicenseKey) return "Ключ лицензии";
                if (isLicenseHint) return "Введите ключ лицензии и нажмите «Проверить ключ» для проверки.";
                if (isGetKey) return "Получить ключ";
                if (isCheckKey) return "Проверить ключ";
                if (isStatusPrefix) return "Состояние: ДЕЙСТВУЕТ (активна) — истекает:";
                if (isCancel) return "Отмена";
            }
            else if (lang.Equals(Ukrainian, StringComparison.OrdinalIgnoreCase))
            {
                if (isCheckVersion) return "Перевірити версію";
                if (isGlobalSettings) return "Глобальні налаштування";
                if (isGlobalTheme) return "Глобальна тема";
                if (isInstallingRoblox) return "Встановлення Roblox";
                if (isLaunchingRoblox) return "Запуск Roblox";
                if (isApplyWord) return "Застосувати";
                if (isUpdatingMasterstrap) return "Оновлення Masterstrap";
                if (isThemeDesc) return "Оберіть режим інтерфейсу: за замовчуванням, Glassmorphic або Glassmorphic з розмиттям.";
                if (isDefaultVi) return "за замовчуванням";
                if (isLicenseKey) return "Ліцензійний ключ";
                if (isLicenseHint) return "Введіть ліцензійний ключ і натисніть «Перевірити ключ» для перевірки.";
                if (isGetKey) return "Отримати ключ";
                if (isCheckKey) return "Перевірити ключ";
                if (isStatusPrefix) return "Стан: ДІЙСНИЙ (активний) — закінчується:";
                if (isCancel) return "Скасувати";
            }
            else if (lang.Equals(SpanishLatin, StringComparison.OrdinalIgnoreCase))
            {
                if (isCheckVersion) return "Comprobar versión";
                if (isGlobalSettings) return "Ajustes globales";
                if (isGlobalTheme) return "Tema global";
                if (isInstallingRoblox) return "Instalando Roblox";
                if (isLaunchingRoblox) return "Iniciando Roblox";
                if (isApplyWord) return "Aplicar";
                if (isUpdatingMasterstrap) return "Actualizando Masterstrap";
                if (isThemeDesc) return "Elegí el modo visual de la app: predeterminado, Glassmorphic o Glassmorphic con desenfoque.";
                if (isDefaultVi) return "predeterminado";
                if (isLicenseKey) return "Clave de licencia";
                if (isLicenseHint) return "Ingresá la clave de licencia y tocá Comprobar clave para validar.";
                if (isGetKey) return "Obtener clave";
                if (isCheckKey) return "Comprobar clave";
                if (isStatusPrefix) return "Estado: VÁLIDA (activa) — vence:";
                if (isCancel) return "Cancelar";
            }
            else if (lang.Equals(French, StringComparison.OrdinalIgnoreCase))
            {
                if (isCheckVersion) return "Vérifier la version";
                if (isGlobalSettings) return "Paramètres globaux";
                if (isGlobalTheme) return "Thème global";
                if (isInstallingRoblox) return "Installation de Roblox";
                if (isLaunchingRoblox) return "Lancement de Roblox";
                if (isApplyWord) return "Appliquer";
                if (isUpdatingMasterstrap) return "Mise à jour de Masterstrap";
                if (isThemeDesc) return "Choisissez le mode visuel de l'application : par défaut, Glassmorphic ou Glassmorphic + flou.";
                if (isDefaultVi) return "par défaut";
                if (isLicenseKey) return "Clé de licence";
                if (isLicenseHint) return "Saisissez la clé de licence et cliquez sur Vérifier la clé pour valider.";
                if (isGetKey) return "Obtenir la clé";
                if (isCheckKey) return "Vérifier la clé";
                if (isStatusPrefix) return "État : VALIDE (actif) — expire le :";
                if (isCancel) return "Annuler";
            }
            else if (lang.Equals(Hebrew, StringComparison.OrdinalIgnoreCase))
            {
                if (isCheckVersion) return "בדיקת גרסה";
                if (isGlobalSettings) return "הגדרות כלליות";
                if (isGlobalTheme) return "ערכת נושא כללית";
                if (isInstallingRoblox) return "מתקין את Roblox";
                if (isLaunchingRoblox) return "מפעיל את Roblox";
                if (isApplyWord) return "החלה";
                if (isUpdatingMasterstrap) return "מעדכן את Masterstrap";
                if (isThemeDesc) return "בחרו מצב תצוגה: ברירת מחדל, Glassmorphic או Glassmorphic + טשטוש.";
                if (isDefaultVi) return "ברירת מחדל";
                if (isLicenseKey) return "מפתח רישיון";
                if (isLicenseHint) return "הזינו מפתח רישיון ולחצו על בדיקת מפתח לאימות.";
                if (isGetKey) return "קבלת מפתח";
                if (isCheckKey) return "בדיקת מפתח";
                if (isStatusPrefix) return "סטטוס: תקף (פעיל) — תפוגה:";
                if (isCancel) return "ביטול";
            }
            else if (lang.Equals(Taiwan, StringComparison.OrdinalIgnoreCase))
            {
                if (isCheckVersion) return "檢查版本";
                if (isGlobalSettings) return "全域設定";
                if (isGlobalTheme) return "全域主題";
                if (isInstallingRoblox) return "正在安裝 Roblox";
                if (isLaunchingRoblox) return "正在啟動 Roblox";
                if (isApplyWord) return "套用";
                if (isUpdatingMasterstrap) return "正在更新 Masterstrap";
                if (isThemeDesc) return "選擇應用程式外觀模式：預設、Glassmorphic 或 Glassmorphic + 模糊。";
                if (isDefaultVi) return "預設";
                if (isLicenseKey) return "授權金鑰";
                if (isLicenseHint) return "輸入授權金鑰並按「檢查金鑰」以驗證。";
                if (isGetKey) return "取得金鑰";
                if (isCheckKey) return "檢查金鑰";
                if (isStatusPrefix) return "狀態：有效（使用中）— 到期：";
                if (isCancel) return "取消";
            }
            else if (lang.Equals(Colombia, StringComparison.OrdinalIgnoreCase))
            {
                if (isCheckVersion) return "Comprobar versión";
                if (isGlobalSettings) return "Ajustes globales";
                if (isGlobalTheme) return "Tema global";
                if (isInstallingRoblox) return "Instalando Roblox";
                if (isLaunchingRoblox) return "Iniciando Roblox";
                if (isApplyWord) return "Aplicar";
                if (isUpdatingMasterstrap) return "Actualizando Masterstrap";
                if (isThemeDesc) return "Elige el aspecto de la app: predeterminado, Glassmorphic o Glassmorphic con desenfoque.";
                if (isDefaultVi) return "predeterminado";
                if (isLicenseKey) return "Clave de licencia";
                if (isLicenseHint) return "Ingresa la clave de licencia y haz clic en Comprobar clave para validar.";
                if (isGetKey) return "Obtener clave";
                if (isCheckKey) return "Comprobar clave";
                if (isStatusPrefix) return "Estado: VÁLIDA (activa) — vence:";
                if (isCancel) return "Cancelar";
            }
            else if (lang.Equals(Turkiye, StringComparison.OrdinalIgnoreCase))
            {
                if (isCheckVersion) return "Sürümü kontrol et";
                if (isGlobalSettings) return "Genel ayarlar";
                if (isGlobalTheme) return "Genel tema";
                if (isInstallingRoblox) return "Roblox yükleniyor";
                if (isLaunchingRoblox) return "Roblox başlatılıyor";
                if (isApplyWord) return "Enjekte et";
                if (isUpdatingMasterstrap) return "Masterstrap güncelleniyor";
                if (isThemeDesc) return "Uygulama görünümü: Varsayılan, Glassmorphic veya Glassmorphic + Bulanıklık.";
                if (isDefaultVi) return "varsayılan";
                if (isLicenseKey) return "Lisans anahtarı";
                if (isLicenseHint) return "Lisans anahtarını girin ve doğrulamak için Anahtarı kontrol et’e tıklayın.";
                if (isGetKey) return "Anahtar al";
                if (isCheckKey) return "Anahtarı kontrol et";
                if (isStatusPrefix) return "Durum: GEÇERLİ (etkin) — bitiş:";
                if (isCancel) return "İptal";
            }
            else if (lang.Equals(Spain, StringComparison.OrdinalIgnoreCase))
            {
                if (isCheckVersion) return "Comprobar versión";
                if (isGlobalSettings) return "Ajustes globales";
                if (isGlobalTheme) return "Tema global";
                if (isInstallingRoblox) return "Instalando Roblox";
                if (isLaunchingRoblox) return "Iniciando Roblox";
                if (isApplyWord) return "Aplicar";
                if (isUpdatingMasterstrap) return "Actualizando Masterstrap";
                if (isThemeDesc) return "Elige el modo visual: predeterminado, Glassmorphic o Glassmorphic con desenfoque.";
                if (isDefaultVi) return "predeterminado";
                if (isLicenseKey) return "Clave de licencia";
                if (isLicenseHint) return "Introduce la clave de licencia y pulsa Comprobar clave para validar.";
                if (isGetKey) return "Obtener clave";
                if (isCheckKey) return "Comprobar clave";
                if (isStatusPrefix) return "Estado: VÁLIDA (activa) — caduca:";
                if (isCancel) return "Cancelar";
            }
            else if (lang.Equals(Chile, StringComparison.OrdinalIgnoreCase))
            {
                if (isCheckVersion) return "Comprobar versión";
                if (isGlobalSettings) return "Ajustes globales";
                if (isGlobalTheme) return "Tema global";
                if (isInstallingRoblox) return "Instalando Roblox";
                if (isLaunchingRoblox) return "Iniciando Roblox";
                if (isApplyWord) return "Aplicar";
                if (isUpdatingMasterstrap) return "Actualizando Masterstrap";
                if (isThemeDesc) return "Elige el modo visual de la app: predeterminado, Glassmorphic o Glassmorphic con desenfoque.";
                if (isDefaultVi) return "predeterminado";
                if (isLicenseKey) return "Clave de licencia";
                if (isLicenseHint) return "Ingresa la clave de licencia y haz clic en Comprobar clave para validar.";
                if (isGetKey) return "Obtener clave";
                if (isCheckKey) return "Comprobar clave";
                if (isStatusPrefix) return "Estado: VÁLIDA (activa) — vence:";
                if (isCancel) return "Cancelar";
            }
            else if (lang.Equals(Italy, StringComparison.OrdinalIgnoreCase))
            {
                if (isCheckVersion) return "Verifica versione";
                if (isGlobalSettings) return "Impostazioni globali";
                if (isGlobalTheme) return "Tema globale";
                if (isInstallingRoblox) return "Installazione di Roblox";
                if (isLaunchingRoblox) return "Avvio di Roblox";
                if (isApplyWord) return "Inietta";
                if (isUpdatingMasterstrap) return "Aggiornamento di Masterstrap";
                if (isThemeDesc) return "Scegli la modalità visiva: predefinita, Glassmorphic o Glassmorphic con sfocatura.";
                if (isDefaultVi) return "predefinito";
                if (isLicenseKey) return "Chiave di licenza";
                if (isLicenseHint) return "Inserisci la chiave di licenza e fai clic su Verifica chiave per convalidare.";
                if (isGetKey) return "Ottieni chiave";
                if (isCheckKey) return "Verifica chiave";
                if (isStatusPrefix) return "Stato: VALIDA (attiva) — scade:";
                if (isCancel) return "Annulla";
            }
            else if (lang.Equals(UnitedArabEmirates, StringComparison.OrdinalIgnoreCase))
            {
                if (isCheckVersion) return "التحقق من الإصدار";
                if (isGlobalSettings) return "الإعدادات العامة";
                if (isGlobalTheme) return "المظهر العام";
                if (isInstallingRoblox) return "جاري تثبيت Roblox";
                if (isLaunchingRoblox) return "جاري تشغيل Roblox";
                if (isApplyWord) return "حقن";
                if (isUpdatingMasterstrap) return "جاري تحديث Masterstrap";
                if (isThemeDesc) return "اختر وضع العرض: افتراضي، Glassmorphic أو Glassmorphic مع ضبابية.";
                if (isDefaultVi) return "افتراضي";
                if (isLicenseKey) return "مفتاح الترخيص";
                if (isLicenseHint) return "أدخل مفتاح الترخيص وانقر «التحقق من المفتاح» للتحقق.";
                if (isGetKey) return "احصل على المفتاح";
                if (isCheckKey) return "التحقق من المفتاح";
                if (isStatusPrefix) return "الحالة: صالح (نشط) — تنتهي في:";
                if (isCancel) return "إلغاء";
            }

            return string.Empty;
        }

        public static void ApplyToWindow(Window window, DependencyObject excludeSubtreeFromTranslation = null)
        {
            if (window == null)
                return;
            try
            {
                if (!string.IsNullOrWhiteSpace(window.Title))
                    window.Title = Translate(window.Title);
                TraverseAndTranslate(window, excludeSubtreeFromTranslation);
            }
            catch
            {
            }
        }

        public static void ApplyToAllWindows()
        {
            try
            {
                if (Application.Current?.Windows == null)
                    return;
                foreach (Window w in Application.Current.Windows)
                {
                    if (w != null)
                        ApplyToWindow(w);
                }
            }
            catch { /* ignore */ }
        }

        public static void ApplyTranslationToTextBlock(TextBlock textBlock)
        {
            if (textBlock == null)
                return;
            try
            {
                if (IsInsideComboVisualTree(textBlock))
                    return;

                ValueSource valueSource = DependencyPropertyHelper.GetValueSource(textBlock, TextBlock.TextProperty);
                if (valueSource.IsExpression || BindingOperations.GetBindingBase(textBlock, TextBlock.TextProperty) != null)
                    return;

                if (textBlock.Inlines != null && textBlock.Inlines.Count > 0)
                {
                    foreach (Inline inline in textBlock.Inlines)
                    {
                        if (inline is Run run && !string.IsNullOrWhiteSpace(run.Text))
                            run.Text = Translate(run.Text);
                        else if (inline is Span span)
                            TranslateSpanInlines(span);
                    }
                    return;
                }

                if (!string.IsNullOrWhiteSpace(textBlock.Text))
                    textBlock.Text = Translate(textBlock.Text);
            }
            catch
            {
            }
        }

        private static bool IsInsideComboVisualTree(DependencyObject node)
        {
            DependencyObject current = node;
            while (current != null)
            {
                if (current is ComboBox || current is ComboBoxItem)
                    return true;

                DependencyObject visualParent = null;
                if (current is Visual || current is System.Windows.Media.Media3D.Visual3D)
                    visualParent = VisualTreeHelper.GetParent(current);

                current = visualParent ?? LogicalTreeHelper.GetParent(current);
            }

            return false;
        }

        private static void TraverseAndTranslate(DependencyObject root, DependencyObject excludeSubtree = null)
        {
            if (root == null)
                return;
            if (excludeSubtree != null && root == excludeSubtree)
                return;
            try
            {
                if (root is TextBlock textBlock)
                    ApplyTranslationToTextBlock(textBlock);
                else if (root is Button button && button.Content is string buttonText && !string.IsNullOrWhiteSpace(buttonText))
                {
                    button.Content = Translate(buttonText);
                }
                else if (root is TabItem tabItem && tabItem.Header is string headerText && !string.IsNullOrWhiteSpace(headerText))
                {
                    tabItem.Header = Translate(headerText);
                }
                else if (root is ComboBoxItem cbi && cbi.Content is string cbiStr && !string.IsNullOrWhiteSpace(cbiStr))
                {
                    cbi.Content = Translate(cbiStr);
                }
                else if (root is Label lbl && lbl.Content is string lblStr && !string.IsNullOrWhiteSpace(lblStr))
                {
                    lbl.Content = Translate(lblStr);
                }
                else if (root is CheckBox chk && chk.Content is string chkStr && !string.IsNullOrWhiteSpace(chkStr))
                {
                    chk.Content = Translate(chkStr);
                }
                else if (root is HeaderedContentControl hcc && hcc.Header is string hdrStr && !string.IsNullOrWhiteSpace(hdrStr))
                {
                    hcc.Header = Translate(hdrStr);
                }
                else if (root is ToolTip tt && tt.Content is string ttStr && !string.IsNullOrWhiteSpace(ttStr))
                {
                    tt.Content = Translate(ttStr);
                }
                else if (root is Expander exp && exp.Header is string expStr && !string.IsNullOrWhiteSpace(expStr))
                {
                    exp.Header = Translate(expStr);
                }
                else if (root is RadioButton rb && rb.Content is string rbStr && !string.IsNullOrWhiteSpace(rbStr))
                {
                    rb.Content = Translate(rbStr);
                }

                int childrenCount = VisualTreeHelper.GetChildrenCount(root);
                for (int i = 0; i < childrenCount; i++)
                {
                    TraverseAndTranslate(VisualTreeHelper.GetChild(root, i), excludeSubtree);
                }
            }
            catch
            {
            }
        }

        private static void TranslateSpanInlines(Span span)
        {
            if (span?.Inlines == null) return;
            foreach (Inline inline in span.Inlines)
            {
                if (inline is Run run && !string.IsNullOrWhiteSpace(run.Text))
                    run.Text = Translate(run.Text);
                else if (inline is Span childSpan)
                    TranslateSpanInlines(childSpan);
            }
        }

        private static Dictionary<string, string> BuildReverseMap(Dictionary<string, string> enToLang)
        {
            var reverse = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var pair in enToLang)
            {
                if (!string.IsNullOrEmpty(pair.Value) && !reverse.ContainsKey(pair.Value))
                    reverse[pair.Value] = pair.Key;
            }
            return reverse;
        }
    }
}
