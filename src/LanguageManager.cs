using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Masterstrap.Services
{
    public static class LanguageManager
    {
        public static string CurrentLanguage { get; private set; } = "en";

        private static readonly Dictionary<string, string> VietnameseTranslations = new()
        {
            { "Masterstrap", "Masterstrap" },
            { "Made By ©Dank1ngs", "được thực hiện bởi ©Dank1ngs" },
            { "Join Discord", "Tham gia Discord" },

            { "Home", "Trang chủ" },
            { "FastFlag", "FastFlag" },
            { "Global", "Toàn cục" },
            { "Games", "Trò chơi" },
            { "Settings", "Cài đặt" },
            { "FAQ", "Hỏi đáp" },

            { "Information System", "Thông Tin Hệ Thống" },
            { "FFlags:", "FFlags:" },
            { "Not loaded", "Chưa tải" },
            { "Count: 0", "Số lượng: 0" },
            { "Roblox Version:", "Phiên bản Roblox:" },
            { "version-unknown", "phiên bản-chưa rõ" },
            { "Last update: Unknown", "Cập nhật lần cuối: Không rõ" },
            { "Software Version:", "Phiên bản phần mềm:" },

            { "📁 Load FFlags JSON", "📁 Tải FFlags JSON" },
            { "📄 Load FFlag Addresses", "📄 Tải địa chỉ FFlag" },
            { "⚡ APPLY", "⚡ áp dụng" },
            { "↩️ UNAPPLY", "↩️ HỦY áp dụng" },
            { "Activity Log", "Nhật ký hoạt động" },
            { "0 entries", "0 mục" },
            { "Clear Log", "Xóa nhật ký" },

            { "FastFlag Editor", "Trình chỉnh sửa FastFlag" },
            { "manage your own Fast Flags. Use with caution", "quản lý Fast Flags của bạn. Sử dụng cẩn thận" },
            { "Allow Masterstrap to manage Fast Flags", "Cho phép Masterstrap quản lý Fast Flags" },
            { "Turning off this option will prevent any configuration here from being applied to Roblox.", "Tắt tùy chọn này sẽ ngăn mọi cấu hình ở đây được áp dụng vào Roblox." },
            { "Rendering and Graphics", "Kết xuất và Đồ họa" },
            { "Anti-aliasing quality (MSAA)", "Chất lượng khử răng cưa (MSAA)" },
            { "Preserve rendering quality with display scaling", "Giữ chất lượng kết xuất khi thu phóng màn hình" },
            { "Roblox reduces your rendering quality depending on how your display is scaled in Windows.", "Roblox giảm chất lượng kết xuất tùy theo cách màn hình được thu phóng trong Windows." },
            { "FRM Quality Override", "Ghi đè chất lượng FRM" },
            { "Choose the FRM quality that Roblox should use.", "Chọn chất lượng FRM mà Roblox sẽ sử dụng." },
            { "Lowest quality", "Chất lượng thấp nhất" },
            { "Highest quality", "Chất lượng cao nhất" },
            { "Rendering mode", "Chế độ kết xuất" },
            { "Texture quality", "Chất lượng kết cấu" },

            { "Set as Read-Only", "Đặt chỉ đọc" },
            { "Prevent Roblox from overriding global settings.", "Ngăn Roblox ghi đè cài đặt toàn cục." },
            { "Presets", "Cài đặt sẵn" },
            { "Graphics Quality", "Chất lượng đồ họa" },
            { "Set the graphics quality of your game", "Đặt chất lượng đồ họa cho trò chơi của bạn" },
            { "Max Quality Enabled", "Bật chất lượng tối đa" },
            { "Enables maximum graphics quality mode for enhanced visual effects and rendering detail.", "Bật chế độ chất lượng đồ họa tối đa để tăng hiệu ứng hình ảnh và chi tiết kết xuất." },
            { "Graphics Quality Level", "Mức chất lượng đồ họa" },
            { "Adjusts the in-game graphics quality level from low to maximum.", "Điều chỉnh mức chất lượng đồ họa trong trò chơi từ thấp đến tối đa." },
            { "Framerate Limit", "Giới hạn tốc độ khung hình" },
            { "Unlock framerate limit for Roblox. Going above 240 FPS is not recommended.", "Mở khóa giới hạn tốc độ khung hình cho Roblox. Không khuyến nghị vượt quá 240 FPS." },
            { "User Interface and Layout", "Giao diện người dùng và Bố cục" },
            { "Transparency", "Độ trong suốt" },
            { "Custom transparency for UI elements.", "Tùy chỉnh độ trong suốt cho các phần tử giao diện." },
            { "Reduced Motion", "Giảm chuyển động" },
            { "Removes the animation on the escape menu.", "Xóa hoạt ảnh trên menu thoát." },
            { "Font Size", "Cỡ chữ" },
            { "Choose how large the font should appear.", "Chọn kích thước phông chữ hiển thị." },
            { "Other", "Khác" },
            { "Mouse Sensitivity", "Độ nhạy chuột" },
            { "Change how fast the camera will move in-game.", "Thay đổi tốc độ camera di chuyển trong trò chơi." },
            { "VR Enabled", "Bật VR" },
            { "Player Name Visibility", "Hiển thị tên người chơi" },
            { "Hide name tags above other players for a cleaner screen experience.", "Ẩn thẻ tên phía trên người chơi khác để trải nghiệm màn hình gọn gàng hơn." },

            { "← Back", "← Quay Lại" },
            { "Back", "Quay Lại" },
            { "Add", "Thêm" },
            { "Delete", "Xóa" },
            { "Clear All", "Xóa Tất Cả" },
            { "Export", "Sao lưu lại" },
            { "Search", "Tìm Kiếm" },
            { "Filter:", "Bộ lọc:" },
            { "All", "Tất cả" },
            { "Graphics", "Đồ Họa" },
            { "Internet", "Mạng" },
            { "Optimizer", "Tối ưu hóa" },
            { "Physics", "Vật lý" },
            { "Audio", "Âm thanh" },

            { "Settings and Options", "Cài đặt và Tùy chọn" },
            { "Desktop Shortcut", "Lối tắt màn hình" },
            { "Create a shortcut on your Desktop for quick access to Masterstrap", "Tạo lối tắt trên Màn hình để truy cập nhanh Masterstrap" },
            { " (recommended)", " (khuyến nghị)" },
            { " General Settings", " Cài đặt chung" },
            { "Auto-load FFlags on startup", "Tự động tải FFlags khi khởi động" },
            { "Auto-load Cache on startup", "Tự động tải Cache khi khởi động" },
            { "Auto-apply when Roblox is detected", "Tự động áp dụng khi phát hiện Roblox" },
            { "Auto-check for updates on startup", "Tự động kiểm tra cập nhật khi khởi động" },
            { "Minimize to system tray", "Thu nhỏ xuống khay hệ thống" },
            { " Optimizer", " Tối ưu hóa" },
            { "Auto-cleanup temp files", "Tự động dọn dẹp tệp tạm" },
            { "Memory optimization", "Tối ưu bộ nhớ" },
            { "Auto-delete Roblox cache", "Tự động xóa bộ nhớ đệm Roblox" },

            { "🌍 Language Settings", "🌍 Cài đặt ngôn ngữ" },
            { "Select your preferred display language for the application interface.", "Chọn ngôn ngữ hiển thị ưa thích cho giao diện ứng dụng." },

            { "Select Game FFlags Preset", "Chọn FFlags theo trò chơi" },
            { "Update Soon...", "Cập Nhật Sớm..." },

            { "FAQ and Guide", "Hỏi đáp và Hướng dẫn" },
            { "How to Use Masterstrap", "Cách sử dụng Masterstrap" },
            { "1. Load FFlags JSON file", "1. Tải tệp FFlags JSON" },
            { "2. Load FFlag Addresses (optional)", "2. Tải địa chỉ FFlag (tùy chọn)" },
            { "3. Make sure Roblox is running", "3. Đảm bảo Roblox đang chạy" },
            { "4. Click APPLY button to apply FFlags", "4. Nhấn nút áp dụng để áp dụng FFlags" },
            { "5. Check Activity Log for results", "5. Kiểm tra Nhật ký hoạt động để xem kết quả" },
            { "✏️ How to Edit FFlags", "✏️ Cách chỉnh sửa FFlags" },
            { "• Go to Edit tab to modify loaded FFlags", "• Vào tab Chỉnh sửa để sửa FFlags đã tải" },
            { "• Click Add to create new FFlag entry", "• Nhấn Thêm để tạo mục FFlag mới" },
            { "• Click Delete to remove selected FFlag", "• Nhấn Xóa để xóa FFlag đã chọn" },
            { "• Use Search to find specific FFlags", "• Dùng Tìm kiếm để tìm FFlags cụ thể" },
            { "• Click Export to save modified FFlags", "• Nhấn Xuất để lưu FFlags đã sửa" },
            { "🔧 Troubleshooting", "🔧 Xử lý sự cố" },
            { "⚠️ Roblox not found?", "⚠️ Không tìm thấy Roblox?" },
            { "Make sure Roblox is running before applying", "Đảm bảo Roblox đang chạy trước khi áp dụng" },
            { "⚠️ Application failed?", "⚠️ Áp dụng thất bại?" },
            { "Please ensure that your Roblox version matches the version that Masterstrap has requested", "Vui lòng đảm bảo phiên bản Roblox của bạn khớp với phiên bản Masterstrap yêu cầu" },
            { "⚠️ FFlags not loading?", "⚠️ FFlags không tải được?" },
            { "Verify JSON file format is correct and valid", "Kiểm tra định dạng tệp JSON chính xác và hợp lệ" },
            { "⚠️ Game crash after applying?", "⚠️ Trò chơi bị lỗi sau khi áp dụng?" },
            { "Reason: FFlag has targetfps set too high, causing device overload and crash. Please click 'Edit FFlag' and change 'targetfps' value to 300-400", "Nguyên nhân: FFlag đặt targetfps quá cao, gây quá tải thiết bị và lỗi. Vui lòng nhấn 'Chỉnh sửa FFlag' và thay đổi giá trị 'targetfps' thành 300-400" },
            { "💡 Tips and Tricks", "💡 Mẹo và Thủ thuật" },
            { "• Keep your FFlag JSON file backed up", "• Sao lưu tệp FFlag JSON của bạn" },
            { "• Export frequently to save your changes", "• Xuất thường xuyên để lưu thay đổi" },
            { "• Use Search feature to quickly find FFlags", "• Dùng tính năng Tìm kiếm để tìm FFlags nhanh" },
            { "• Check Activity Log for application status", "• Kiểm tra Nhật ký hoạt động để xem trạng thái áp dụng" },

            { "Fast Mode", "Chế độ nhanh" },
            { "Ready", "Sẵn sàng" },
            { "0% • Idle", "0% • Rỗi" },
            { "Save and Launch", "Lưu và Khởi chạy" },
            { "Save", "Lưu" },
            { "Close", "Đóng" },

            { "Unsaved Changes", "Thay đổi chưa lưu" },
            { "You have unsaved changes. Do you want to save before exiting?", "Bạn có thay đổi chưa lưu. Bạn có muốn lưu trước khi thoát không?" },
            { "Cancel", "Hủy" },
            { "Don't Save", "Không lưu" },

            { "Automatic", "Tự động" },
            { "Off", "Tắt" },
            { "Low", "Thấp" },
            { "Medium", "Trung bình" },
            { "High", "Cao" },
            { "Default", "Mặc định" },
            { "Small", "Nhỏ" },
            { "Large", "Lớn" },

            { "English", "English" },
            { "Vietnamese", "Tiếng Việt" },
        };

        private static readonly Dictionary<string, string> ReverseVietnamese = BuildReverseDictionary();

        private static Dictionary<string, string> BuildReverseDictionary()
        {
            var reverse = new Dictionary<string, string>();
            foreach (var kvp in VietnameseTranslations)
            {
                if (!reverse.ContainsKey(kvp.Value) && kvp.Key != kvp.Value)
                    reverse[kvp.Value] = kvp.Key;
            }
            return reverse;
        }

        public static void SetLanguage(string langCode)
        {
            CurrentLanguage = langCode;
        }

        public static string Translate(string englishText)
        {
            if (CurrentLanguage == "vi" && VietnameseTranslations.TryGetValue(englishText, out string translated))
                return translated;
            return englishText;
        }

        public static string GetEnglishKey(string displayText)
        {
            if (ReverseVietnamese.TryGetValue(displayText, out string english))
                return english;
            if (VietnameseTranslations.ContainsKey(displayText))
                return displayText;
            return displayText;
        }

        public static void ApplyLanguage(DependencyObject root, string langCode)
        {
            CurrentLanguage = langCode;
            TranslateVisualTree(root);
        }

        private static readonly HashSet<string> SkipElementNames = new()
        {
            "InfoFFlagsName", "InfoFFlagsCount", "InfoRobloxVersion", "InfoRobloxUpdate",
            "InfoSoftwareVersion", "InfoSoftwareUpdate", "ActivityCountText",
            "GraphicsQualityValue", "GraphicsQualityLevelValue", "TransparencyValue",
            "FramerateLimitInput", "MouseSensitivityInput"
        };

        private static void TranslateVisualTree(DependencyObject parent)
        {
            if (parent == null) return;

            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);

                if (child is TextBlock textBlock)
                {
                    string name = textBlock.Name;
                    if (!string.IsNullOrEmpty(name) && SkipElementNames.Contains(name))
                    {
                        continue;
                    }

                    string text = textBlock.Text;
                    if (!string.IsNullOrEmpty(text))
                    {
                        string translated = TranslateText(text);
                        if (translated != text)
                            textBlock.Text = translated;
                    }
                }
                else if (child is Button button && button.Content is string btnText)
                {
                    string translated = TranslateText(btnText);
                    if (translated != btnText)
                        button.Content = translated;
                }
                else if (child is ContentControl cc && cc.Content is string ccText && !(cc is Button))
                {
                    string translated = TranslateText(ccText);
                    if (translated != ccText)
                        cc.Content = translated;
                }

                TranslateVisualTree(child);
            }
        }

        private static string TranslateText(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            if (CurrentLanguage == "vi")
            {
                if (VietnameseTranslations.TryGetValue(text, out string vi))
                    return vi;
            }
            else
            {
                if (ReverseVietnamese.TryGetValue(text, out string en))
                    return en;
            }

            return text;
        }

        public static void TranslateWindow(Window window)
        {
            if (window == null) return;

            if (window.Title != null)
            {
                string translated = TranslateText(window.Title);
                if (translated != window.Title)
                    window.Title = translated;
            }

            TranslateVisualTree(window);
        }
    }
}
