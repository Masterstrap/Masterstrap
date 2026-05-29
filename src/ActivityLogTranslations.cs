using System;
using System.Collections.Generic;

namespace Masterstrap.Services
{
    internal static class ActivityLogTranslations
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
            ("System initialized", "Hệ thống đã khởi tạo"),
            ("Ready to load FFlags", "Sẵn sàng tải FFlags"),
            ("Not set", "Chưa thiết lập"),
            ("Saved FFlags:", "FFlags đã lưu:"),
            ("Auto-load FFlags:", "Tự động tải FFlags:"),
            ("Enabled", "Đã bật"),
            ("Disabled", "Đã tắt"),
            ("Auto-load Addresses:", "Tự động tải địa chỉ:"),
            ("Roblox Version:", "Phiên bản Roblox:"),
            ("Not detected", "Không phát hiện"),
            ("Software Version:", "Phiên bản phần mềm:"),
            ("Unknown", "Không xác định"),
            ("Version Compatibility:", "Tương thích phiên bản:"),
            ("MATCH", "TRÙNG KHỚP"),
            ("MISMATCH", "KHÔNG TRÙNG KHỚP"),
            ("UNKNOWN", "KHÔNG XÁC ĐỊNH"),
            ("Current Configuration:", "Cấu hình hiện tại:"),
            ("now", "vừa xong"),
            ("s ago", "giây trước"),
            ("m ago", "phút trước"),
            ("Success", "Thành công"),
            ("Failed", "Thất bại"),
            ("Mixed", "Hỗn hợp"),
            ("Pending", "Đang chờ"),
            ("Status", "Trạng thái"),
            ("Actions", "Hành động"),
            ("Session", "Phiên"),
            ("Last", "Mới nhất"),
            ("{0} entries", "{0} mục nhật ký"),
            ("Activity log cleared", "Đã xóa lịch sử hoạt động"),
            ("Application successful ({0} FFlags)", "Cài đặt thành công ({0} FFlags)"),
            ("Application failed ({0} errors)", "Cài đặt thất bại ({0} lỗi)")
        );

        private static readonly Dictionary<string, string> Fil = D(
            ("System initialized", "System initialized"),
            ("Ready to load FFlags", "Ready to load FFlags"),
            ("Not set", "Hindi nakatakda"),
            ("Saved FFlags:", "Naka-save na FFlags:"),
            ("Auto-load FFlags:", "Auto-load FFlags:"),
            ("Enabled", "Naka-enable"),
            ("Disabled", "Naka-disable"),
            ("Auto-load Addresses:", "Auto-load Addresses:"),
            ("Roblox Version:", "Bersyon ng Roblox:"),
            ("Not detected", "Hindi nakita"),
            ("Software Version:", "Bersyon ng Software:"),
            ("Unknown", "Hindi alam"),
            ("Version Compatibility:", "Pagkatugma ng Bersyon:"),
            ("MATCH", "TUGMA"),
            ("MISMATCH", "HINDI TUGMA"),
            ("UNKNOWN", "HINDI ALAM"),
            ("Current Configuration:", "Kasalukuyang Configuration:"),
            ("now", "ngayon"),
            ("s ago", "segundo ang nakalipas"),
            ("m ago", "minuto ang nakalipas"),
            ("Success", "Tagumpay"),
            ("Failed", "Bigo"),
            ("Mixed", "Halo-halo"),
            ("Pending", "Nakabinbin"),
            ("Status", "Katayuan"),
            ("Actions", "Mga Kilos"),
            ("Session", "Sesyon"),
            ("Last", "Huli"),
            ("{0} entries", "{0} entries"),
            ("Activity log cleared", "Na-clear ang activity log"),
            ("Application successful ({0} FFlags)", "Matagumpay na iniksyon ({0} FFlags)"),
            ("Application failed ({0} errors)", "Bigo ang iniksyon ({0} errors)")
        );

        private static readonly Dictionary<string, string> Id = D(
            ("System initialized", "Sistem diinisialisasi"),
            ("Ready to load FFlags", "Siap memuat FFlags"),
            ("Not set", "Tidak diatur"),
            ("Saved FFlags:", "FFlags Disimpan:"),
            ("Auto-load FFlags:", "Muat Otomatis FFlags:"),
            ("Enabled", "Diaktifkan"),
            ("Disabled", "Dinonaktifkan"),
            ("Auto-load Addresses:", "Muat Otomatis Alamat:"),
            ("Roblox Version:", "Versi Roblox:"),
            ("Not detected", "Tidak terdeteksi"),
            ("Software Version:", "Versi Perangkat Lunak:"),
            ("Unknown", "Tidak dikenal"),
            ("Version Compatibility:", "Kompatibilitas Versi:"),
            ("MATCH", "COCOK"),
            ("MISMATCH", "TIDAK COCOK"),
            ("UNKNOWN", "TIDAK DIKETAHUI"),
            ("Current Configuration:", "Konfigurasi Saat Ini:"),
            ("now", "sekarang"),
            ("s ago", "detik lalu"),
            ("m ago", "menit lalu"),
            ("Success", "Berhasil"),
            ("Failed", "Gagal"),
            ("Mixed", "Campuran"),
            ("Pending", "Tertunda"),
            ("Status", "Status"),
            ("Actions", "Tindakan"),
            ("Session", "Sesi"),
            ("Last", "Terakhir"),
            ("{0} entries", "{0} entri"),
            ("Activity log cleared", "Log aktivitas dibersihkan"),
            ("Application successful ({0} FFlags)", "Injeksi berhasil ({0} FFlags)"),
            ("Application failed ({0} errors)", "Injeksi gagal ({0} kesalahan)")
        );

        private static readonly Dictionary<string, string> Pt = D(
            ("System initialized", "Sistema inicializado"),
            ("Ready to load FFlags", "Pronto para carregar FFlags"),
            ("Not set", "Não definido"),
            ("Saved FFlags:", "FFlags Salvas:"),
            ("Auto-load FFlags:", "Auto-carregar FFlags:"),
            ("Enabled", "Ativado"),
            ("Disabled", "Desativado"),
            ("Auto-load Addresses:", "Auto-carregar Endereços:"),
            ("Roblox Version:", "Versão do Roblox:"),
            ("Not detected", "Não detectado"),
            ("Software Version:", "Versão do Software:"),
            ("Unknown", "Desconhecido"),
            ("Version Compatibility:", "Compatibilidade de Versão:"),
            ("MATCH", "CORRESPONDE"),
            ("MISMATCH", "INCOMPATÍVEL"),
            ("UNKNOWN", "DESCONHECIDO"),
            ("Current Configuration:", "Configuração Atual:"),
            ("now", "agora"),
            ("s ago", "s atrás"),
            ("m ago", "m atrás"),
            ("Success", "Sucesso"),
            ("Failed", "Falhou"),
            ("Mixed", "Misto"),
            ("Pending", "Pendente"),
            ("Status", "Status"),
            ("Actions", "Ações"),
            ("Session", "Sessão"),
            ("Last", "Última"),
            ("{0} entries", "{0} entradas"),
            ("Activity log cleared", "Log de atividades limpo"),
            ("Application successful ({0} FFlags)", "Injeção bem-sucedida ({0} FFlags)"),
            ("Application failed ({0} errors)", "Injeção falhou ({0} erros)")
        );

        private static readonly Dictionary<string, string> Ms = D(
            ("System initialized", "Sistem dimulakan"),
            ("Ready to load FFlags", "Sedia untuk memuatkan FFlags"),
            ("Not set", "Tidak ditetapkan"),
            ("Saved FFlags:", "FFlags Disimpan:"),
            ("Auto-load FFlags:", "Muat auto FFlags:"),
            ("Enabled", "Didayakan"),
            ("Disabled", "Dilumpuhkan"),
            ("Auto-load Addresses:", "Muat auto Alamat:"),
            ("Roblox Version:", "Versi Roblox:"),
            ("Not detected", "Tidak dikesan"),
            ("Software Version:", "Versi Perisian:"),
            ("Unknown", "Tidak diketahui"),
            ("Version Compatibility:", "Keserasian Versi:"),
            ("MATCH", "PADAN"),
            ("MISMATCH", "TIDAK PADAN"),
            ("UNKNOWN", "TIDAK DIKETAHUI"),
            ("Current Configuration:", "Konfigurasi Semasa:"),
            ("now", "sekarang"),
            ("s ago", "saat lalu"),
            ("m ago", "minit lalu"),
            ("Success", "Berjaya"),
            ("Failed", "Gagal"),
            ("Mixed", "Bercampur"),
            ("Pending", "Belum Selesai"),
            ("Status", "Status"),
            ("Actions", "Tindakan"),
            ("Session", "Sesi"),
            ("Last", "Terakhir"),
            ("{0} entries", "{0} entri"),
            ("Activity log cleared", "Log aktiviti dikosongkan"),
            ("Application successful ({0} FFlags)", "Terapkanan berjaya ({0} FFlags)"),
            ("Application failed ({0} errors)", "Terapkanan gagal ({0} ralat)")
        );

        private static readonly Dictionary<string, string> Ja = D(
            ("System initialized", "システムが初期化されました"),
            ("Ready to load FFlags", "FFlagのロード準備完了"),
            ("Not set", "未設定"),
            ("Saved FFlags:", "保存された FFlags:"),
            ("Auto-load FFlags:", "FFlagの自動ロード:"),
            ("Enabled", "有効"),
            ("Disabled", "無効"),
            ("Auto-load Addresses:", "アドレスの自動ロード:"),
            ("Roblox Version:", "Robloxバージョン:"),
            ("Not detected", "検出されず"),
            ("Software Version:", "ソフトウェアバージョン:"),
            ("Unknown", "不明"),
            ("Version Compatibility:", "バージョン互換性:"),
            ("MATCH", "一致"),
            ("MISMATCH", "不一致"),
            ("UNKNOWN", "不明"),
            ("Current Configuration:", "現在の設定:"),
            ("now", "たった今"),
            ("s ago", "秒前"),
            ("m ago", "分前"),
            ("Success", "成功"),
            ("Failed", "失敗"),
            ("Mixed", "混合"),
            ("Pending", "保留中"),
            ("Status", "ステータス"),
            ("Actions", "アクション"),
            ("Session", "セッション"),
            ("Last", "最新"),
            ("{0} entries", "{0} 件のエントリ"),
            ("Activity log cleared", "アクティビティログが消去されました"),
            ("Application successful ({0} FFlags)", "インジェクション成功 ({0} FFlags)"),
            ("Application failed ({0} errors)", "インジェクション失敗 ({0} 個のエラー)")
        );

        private static readonly Dictionary<string, string> Zh = D(
            ("System initialized", "系统初始化完毕"),
            ("Ready to load FFlags", "已准备好加载 FFlags"),
            ("Not set", "未设置"),
            ("Saved FFlags:", "已保存的 FFlags:"),
            ("Auto-load FFlags:", "自动加载 FFlags:"),
            ("Enabled", "已启用"),
            ("Disabled", "已禁用"),
            ("Auto-load Addresses:", "自动加载地址:"),
            ("Roblox Version:", "Roblox 版本:"),
            ("Not detected", "未检测到"),
            ("Software Version:", "软件版本:"),
            ("Unknown", "未知"),
            ("Version Compatibility:", "版本兼容性:"),
            ("MATCH", "兼容/匹配"),
            ("MISMATCH", "不兼容/不匹配"),
            ("UNKNOWN", "未知"),
            ("Current Configuration:", "当前配置汇总:"),
            ("now", "刚刚"),
            ("s ago", "秒前"),
            ("m ago", "分钟前"),
            ("Success", "成功"),
            ("Failed", "失败"),
            ("Mixed", "混合"),
            ("Pending", "等待中"),
            ("Status", "状态"),
            ("Actions", "行动次数"),
            ("Session", "本次会话"),
            ("Last", "最近更改"),
            ("{0} entries", "{0} 条日志项"),
            ("Activity log cleared", "活动日志已清空"),
            ("Application successful ({0} FFlags)", "注入成功 (共 {0} 个 FFlags)"),
            ("Application failed ({0} errors)", "注入失败 (发生 {0} 处错误)")
        );

        private static readonly Dictionary<string, string> Th = D(
            ("System initialized", "เริ่มต้นระบบแล้ว"),
            ("Ready to load FFlags", "พร้อมโหลด FFlags แล้ว"),
            ("Not set", "ไม่ได้ตั้งค่า"),
            ("Saved FFlags:", "FFlags ที่บันทึกไว้:"),
            ("Auto-load FFlags:", "โหลด FFlags อัตโนมัติ:"),
            ("Enabled", "เปิดใช้งานแล้ว"),
            ("Disabled", "ปิดใช้งานแล้ว"),
            ("Auto-load Addresses:", "โหลดที่อยู่อัตโนมัติ:"),
            ("Roblox Version:", "เวอร์ชัน Roblox:"),
            ("Not detected", "ตรวจไม่พบ"),
            ("Software Version:", "เวอร์ชันซอฟต์แวร์:"),
            ("Unknown", "ไม่รู้จัก"),
            ("Version Compatibility:", "ความเข้ากันได้ของเวอร์ชัน:"),
            ("MATCH", "เข้ากันได้"),
            ("MISMATCH", "ไม่เข้ากัน"),
            ("UNKNOWN", "ไม่รู้จัก"),
            ("Current Configuration:", "การกำหนดค่าปัจจุบัน:"),
            ("now", "เมื่อกี้"),
            ("s ago", "วินาทีที่แล้ว"),
            ("m ago", "นาทีที่แล้ว"),
            ("Success", "สำเร็จ"),
            ("Failed", "ล้มเหลว"),
            ("Mixed", "ผสม"),
            ("Pending", "รอดำเนินการ"),
            ("Status", "สถานะ"),
            ("Actions", "การดำเนินการ"),
            ("Session", "เซสชัน"),
            ("Last", "ล่าสุด"),
            ("{0} entries", "{0} รายการ"),
            ("Activity log cleared", "ล้างบันทึกกิจกรรมแล้ว"),
            ("Application successful ({0} FFlags)", "ติดตั้งสำเร็จแล้ว ({0} FFlags)"),
            ("Application failed ({0} errors)", "ติดตั้งล้มเหลว ({0} ข้อผิดพลาด)")
        );

        private static readonly Dictionary<string, string> Km = D(
            ("System initialized", "ប្រព័ន្ធត្រូវបានចាប់ផ្តើម"),
            ("Ready to load FFlags", "រួចរាល់ក្នុងការផ្ទុក FFlags"),
            ("Not set", "មិនទាន់កំណត់"),
            ("Saved FFlags:", "FFlags ដែលបានរក្សាទុក៖"),
            ("Auto-load FFlags:", "ផ្ទុក FFlags ស្វ័យប្រវត្ត៖"),
            ("Enabled", "បានបើក"),
            ("Disabled", "បានបិទ"),
            ("Auto-load Addresses:", "ផ្ទុកអាសយដ្ឋានស្វ័យប្រវត្ត៖"),
            ("Roblox Version:", "កំណែ Roblox៖"),
            ("Not detected", "មិនរកឃើញ"),
            ("Software Version:", "កំណែផ្នែកទន់៖"),
            ("Unknown", "មិនស្គាល់"),
            ("Version Compatibility:", "ភាពត្រូវគ្នានៃកំណែ៖"),
            ("MATCH", "ត្រូវគ្នា"),
            ("MISMATCH", "មិនត្រូវគ្នា"),
            ("UNKNOWN", "មិនស្គាល់"),
            ("Current Configuration:", "ការកំណត់បច្ចុប្បន្ន៖"),
            ("now", "ឥឡូវនេះ"),
            ("s ago", "វិនាទីមុន"),
            ("m ago", "នាទីមុន"),
            ("Success", "ជោគជ័យ"),
            ("Failed", "បរាជ័យ"),
            ("Mixed", "លាយបញ្ចូលគ្នា"),
            ("Pending", "កំពុងរង់ចាំ"),
            ("Status", "ស្ថានភាព"),
            ("Actions", "សកម្មភាព"),
            ("Session", "វគ្គ"),
            ("Last", "ចុងក្រោយ"),
            ("{0} entries", "{0} បញ្ជី"),
            ("Activity log cleared", "បានសម្អាតកំណត់ហេតុសកម្មភាព"),
            ("Application successful ({0} FFlags)", "ការចាក់បញ្ចូលបានជោគជ័យ ({0} FFlags)"),
            ("Application failed ({0} errors)", "ការចាក់បញ្ចូលបានបរាជ័យ ({0} កំហុស)")
        );

        private static readonly Dictionary<string, string> Lo = D(
            ("System initialized", "ລະບົບຖືກເລີ່ມຕົ້ນແລ້ວ"),
            ("Ready to load FFlags", "ພ້ອມທີ່ຈະໂຫຼດ FFlags"),
            ("Not set", "ບໍ່ທັນໄດ້ຕັ້ງຄ່າ"),
            ("Saved FFlags:", "FFlags ທີ່ບັນທຶກໄວ້:"),
            ("Auto-load FFlags:", "ໂຫຼດ FFlags ອັດຕະໂນມັດ:"),
            ("Enabled", "ເປີດໃຊ້ແລ້ວ"),
            ("Disabled", "ປິດໃຊ້ແລ້ວ"),
            ("Auto-load Addresses:", "ໂຫຼດທີ່ຢູ່ອັດຕະໂນມັດ:"),
            ("Roblox Version:", "ເວີຊັນ Roblox:"),
            ("Not detected", "ກວດບໍ່ພົບ"),
            ("Software Version:", "ເວີຊັນຊອບແວ:"),
            ("Unknown", "ບໍ່ຮູ້ຈັກ"),
            ("Version Compatibility:", "ຄວາມເຂົ້າກັນໄດ້ຂອງເວີຊັນ:"),
            ("MATCH", "ເຂົ້າກັນໄດ້"),
            ("MISMATCH", "ບໍ່ເຂົ້າກັນ"),
            ("UNKNOWN", "ບໍ່ຮູ້ຈັກ"),
            ("Current Configuration:", "ການກຳນົດຄ່າປັດຈຸບັນ:"),
            ("now", "ຫວ່າງກີ້ນີ້"),
            ("s ago", "ວິນາທີກ່ອນ"),
            ("m ago", "ນາທີກ່ອນ"),
            ("Success", "ສຳເລັດ"),
            ("Failed", "ຫຼົ້ມເຫຼວ"),
            ("Mixed", "ປະສົມ"),
            ("Pending", "ຮອງດຳເນີນການ"),
            ("Status", "ສະຖານະ"),
            ("Actions", "ການດຳເນີນການ"),
            ("Session", "ເຊດຊັນ"),
            ("Last", "ຫຼ້າສຸດ"),
            ("{0} entries", "{0} ລາຍການ"),
            ("Activity log cleared", "ລ້າງບັນທຶກກິດຈະກຳແລ້ວ"),
            ("Application successful ({0} FFlags)", "ການສີດສຳເລັດແລ້ວ ({0} FFlags)"),
            ("Application failed ({0} errors)", "ການສີດຫຼົ້ມເຫຼວ ({0} ຂໍ້ຜິດພາດ)")
        );

        private static readonly Dictionary<string, string> Ko = D(
            ("System initialized", "시스템이 초기화되었습니다"),
            ("Ready to load FFlags", "FFlag 로드 준비 완료"),
            ("Not set", "설정되지 않음"),
            ("Saved FFlags:", "저장된 FFlags:"),
            ("Auto-load FFlags:", "FFlag 자동 로드:"),
            ("Enabled", "활성화됨"),
            ("Disabled", "비활성화됨"),
            ("Auto-load Addresses:", "오프셋 주소 자동 로드:"),
            ("Roblox Version:", "Roblox 버전:"),
            ("Not detected", "감지되지 않음"),
            ("Software Version:", "소프트웨어 버전:"),
            ("Unknown", "알 수 없음"),
            ("Version Compatibility:", "버전 호환성:"),
            ("MATCH", "일치함"),
            ("MISMATCH", "일치하지 않음"),
            ("UNKNOWN", "알 수 없음"),
            ("Current Configuration:", "현재 구성 상태:"),
            ("now", "방금 전"),
            ("s ago", "초 전"),
            ("m ago", "분 전"),
            ("Success", "성공"),
            ("Failed", "실패"),
            ("Mixed", "혼합"),
            ("Pending", "대기 중"),
            ("Status", "상태"),
            ("Actions", "동작 횟수"),
            ("Session", "세션 시간"),
            ("Last", "최근 활동"),
            ("{0} entries", "{0}개 항목"),
            ("Activity log cleared", "활동 로그가 초기화되었습니다"),
            ("Application successful ({0} FFlags)", "인젝션 성공 ({0} FFlags 적용됨)"),
            ("Application failed ({0} errors)", "인젝션 실패 ({0}개 오류 발생)")
        );

        private static readonly Dictionary<string, string> Ru = D(
            ("System initialized", "Система инициализирована"),
            ("Ready to load FFlags", "Готов к загрузке FFlags"),
            ("Not set", "Не задано"),
            ("Saved FFlags:", "Сохраненные FFlags:"),
            ("Auto-load FFlags:", "Автозагрузка FFlags:"),
            ("Enabled", "Включено"),
            ("Disabled", "Отключено"),
            ("Auto-load Addresses:", "Автозагрузка адресов:"),
            ("Roblox Version:", "Версия Roblox:"),
            ("Not detected", "Не обнаружено"),
            ("Software Version:", "Версия программы:"),
            ("Unknown", "Неизвестно"),
            ("Version Compatibility:", "Совместимость версий:"),
            ("MATCH", "СОВПАДАЕТ"),
            ("MISMATCH", "НЕ СОВПАДАЕТ"),
            ("UNKNOWN", "НЕИЗВЕСТНО"),
            ("Current Configuration:", "Текущая конфигурация:"),
            ("now", "только что"),
            ("s ago", "сек. назад"),
            ("m ago", "мин. назад"),
            ("Success", "Успешно"),
            ("Failed", "Ошибка"),
            ("Mixed", "Смешанно"),
            ("Pending", "Ожидание"),
            ("Status", "Статус"),
            ("Actions", "Действия"),
            ("Session", "Сессия"),
            ("Last", "Последнее"),
            ("{0} entries", "Записей: {0}"),
            ("Activity log cleared", "Журнал активности очищен"),
            ("Application successful ({0} FFlags)", "Успешная инъекция ({0} FFlags)"),
            ("Application failed ({0} errors)", "Ошибка инъекции ({0} ошиб.)")
        );

        private static readonly Dictionary<string, string> Uk = D(
            ("System initialized", "Система ініціалізована"),
            ("Ready to load FFlags", "Готовий до завантаження FFlags"),
            ("Not set", "Не встановлено"),
            ("Saved FFlags:", "Збережені FFlags:"),
            ("Auto-load FFlags:", "Автозавантаження FFlags:"),
            ("Enabled", "Увімкнено"),
            ("Disabled", "Вимкнено"),
            ("Auto-load Addresses:", "Автозавантаження адрес:"),
            ("Roblox Version:", "Версія Roblox:"),
            ("Not detected", "Не виявлено"),
            ("Software Version:", "Версія програми:"),
            ("Unknown", "Невідомо"),
            ("Version Compatibility:", "Сумісність версій:"),
            ("MATCH", "СУМІСНО"),
            ("MISMATCH", "НЕСУМІСНО"),
            ("UNKNOWN", "НЕВІДОМО"),
            ("Current Configuration:", "Поточна конфігурація:"),
            ("now", "щойно"),
            ("s ago", "сек. тому"),
            ("m ago", "хв. тому"),
            ("Success", "Успішно"),
            ("Failed", "Помилка"),
            ("Mixed", "Змішано"),
            ("Pending", "Очікування"),
            ("Status", "Статус"),
            ("Actions", "Дії"),
            ("Session", "Сесія"),
            ("Last", "Останнє"),
            ("{0} entries", "Записів: {0}"),
            ("Activity log cleared", "Журнал активності очищено"),
            ("Application successful ({0} FFlags)", "Успішна ін'єкція ({0} FFlags)"),
            ("Application failed ({0} errors)", "Помилка ін'єкції ({0} пом.)")
        );

        private static readonly Dictionary<string, string> Es = D(
            ("System initialized", "Sistema inicializado"),
            ("Ready to load FFlags", "Listo para cargar FFlags"),
            ("Not set", "No establecido"),
            ("Saved FFlags:", "FFlags Guardados:"),
            ("Auto-load FFlags:", "Autocargar FFlags:"),
            ("Enabled", "Habilitado"),
            ("Disabled", "Deshabilitado"),
            ("Auto-load Addresses:", "Autocargar Direcciones:"),
            ("Roblox Version:", "Versión de Roblox:"),
            ("Not detected", "No detectado"),
            ("Software Version:", "Versión del Software:"),
            ("Unknown", "Desconocido"),
            ("Version Compatibility:", "Compatibilidad de Versión:"),
            ("MATCH", "COMPATIBLE"),
            ("MISMATCH", "INCOMPATIBLE"),
            ("UNKNOWN", "DESCONOCIDO"),
            ("Current Configuration:", "Configuración Actual:"),
            ("now", "ahora"),
            ("s ago", "s atrás"),
            ("m ago", "m atrás"),
            ("Success", "Éxito"),
            ("Failed", "Fallido"),
            ("Mixed", "Mixto"),
            ("Pending", "Pendiente"),
            ("Status", "Estado"),
            ("Actions", "Acciones"),
            ("Session", "Sesión"),
            ("Last", "Último"),
            ("{0} entries", "{0} entradas"),
            ("Activity log cleared", "Registro de actividad borrado"),
            ("Application successful ({0} FFlags)", "Inyección exitosa ({0} FFlags)"),
            ("Application failed ({0} errors)", "Inyección fallida ({0} errores)")
        );

        private static readonly Dictionary<string, string> Fr = D(
            ("System initialized", "Système initialisé"),
            ("Ready to load FFlags", "Prêt à charger FFlags"),
            ("Not set", "Non défini"),
            ("Saved FFlags:", "FFlags Sauvegardés :"),
            ("Auto-load FFlags:", "Chargement auto FFlags :"),
            ("Enabled", "Activé"),
            ("Disabled", "Désactivé"),
            ("Auto-load Addresses:", "Chargement auto Adresses :"),
            ("Roblox Version:", "Version Roblox :"),
            ("Not detected", "Non détecté"),
            ("Software Version:", "Version du Logiciel :"),
            ("Unknown", "Inconnu"),
            ("Version Compatibility:", "Compatibilité Version :"),
            ("MATCH", "COMPATIBLE"),
            ("MISMATCH", "INCOMPATIBLE"),
            ("UNKNOWN", "INCONNU"),
            ("Current Configuration:", "Configuration Actuelle :"),
            ("now", "à l'instant"),
            ("s ago", "s auparavant"),
            ("m ago", "m auparavant"),
            ("Success", "Succès"),
            ("Failed", "Échec"),
            ("Mixed", "Mixte"),
            ("Pending", "En attente"),
            ("Status", "Statut"),
            ("Actions", "Actions"),
            ("Session", "Session"),
            ("Last", "Dernier"),
            ("{0} entries", "{0} entrées"),
            ("Activity log cleared", "Journal d'activité effacé"),
            ("Application successful ({0} FFlags)", "Application réussie ({0} FFlags)"),
            ("Application failed ({0} errors)", "Application échouée ({0} erreurs)")
        );

        private static readonly Dictionary<string, string> He = D(
            ("System initialized", "המערכת אותחלה"),
            ("Ready to load FFlags", "מוכן לטעינת FFlags"),
            ("Not set", "לא הוגדר"),
            ("Saved FFlags:", "FFlags שמורים:"),
            ("Auto-load FFlags:", "טעינה אוטומטית של FFlags:"),
            ("Enabled", "מופעל"),
            ("Disabled", "מושבת"),
            ("Auto-load Addresses:", "טעינה אוטומטית של כתובות:"),
            ("Roblox Version:", "גרסת Roblox:"),
            ("Not detected", "לא זוהה"),
            ("Software Version:", "גרסת התוכנה:"),
            ("Unknown", "לא ידוע"),
            ("Version Compatibility:", "תאימות גרסה:"),
            ("MATCH", "תואם"),
            ("MISMATCH", "לא תואם"),
            ("UNKNOWN", "לא ידוע"),
            ("Current Configuration:", "תצורה נוכחית:"),
            ("now", "הרגע"),
            ("s ago", "שניות לפני"),
            ("m ago", "דקות לפני"),
            ("Success", "הצלחה"),
            ("Failed", "נכשל"),
            ("Mixed", "מעורב"),
            ("Pending", "ממתין"),
            ("Status", "סטטוס"),
            ("Actions", "פעולות"),
            ("Session", "סשן"),
            ("Last", "אחרון"),
            ("{0} entries", "{0} רשומות"),
            ("Activity log cleared", "יומן הפעילות נוקה"),
            ("Application successful ({0} FFlags)", "ההזרקה הצליחה ({0} FFlags)"),
            ("Application failed ({0} errors)", "ההזרקה נכשלה ({0} שגיאות)")
        );

        private static readonly Dictionary<string, string> Tw = D(
            ("System initialized", "系統初始化完畢"),
            ("Ready to load FFlags", "已準備好載入 FFlags"),
            ("Not set", "未設置"),
            ("Saved FFlags:", "已儲存的 FFlags:"),
            ("Auto-load FFlags:", "自動載入 FFlags:"),
            ("Enabled", "已啟用"),
            ("Disabled", "已禁用"),
            ("Auto-load Addresses:", "自動載入地址:"),
            ("Roblox Version:", "Roblox 版本:"),
            ("Not detected", "未檢測到"),
            ("Software Version:", "軟體版本:"),
            ("Unknown", "未知"),
            ("Version Compatibility:", "版本相容性:"),
            ("MATCH", "相容/匹配"),
            ("MISMATCH", "不相容/不匹配"),
            ("UNKNOWN", "未知"),
            ("Current Configuration:", "目前配置匯總:"),
            ("now", "剛剛"),
            ("s ago", "秒前"),
            ("m ago", "分鐘前"),
            ("Success", "成功"),
            ("Failed", "失敗"),
            ("Mixed", "混合"),
            ("Pending", "等待中"),
            ("Status", "狀態"),
            ("Actions", "行動次數"),
            ("Session", "本次會話"),
            ("Last", "最近更改"),
            ("{0} entries", "{0} 條記錄"),
            ("Activity log cleared", "活動記錄已清空"),
            ("Application successful ({0} FFlags)", "寫入成功 (共 {0} 个 FFlags)"),
            ("Application failed ({0} errors)", "寫入失敗 (發生 {0} 處錯誤)")
        );

        private static readonly Dictionary<string, string> Tr = D(
            ("System initialized", "Sistem başlatıldı"),
            ("Ready to load FFlags", "FFlag yüklemeye hazır"),
            ("Not set", "Ayarlanmadı"),
            ("Saved FFlags:", "Kaydedilen FFlags:"),
            ("Auto-load FFlags:", "FFlag Otomatik Yükleme:"),
            ("Enabled", "Etkinleştirildi"),
            ("Disabled", "Devre Dışı"),
            ("Auto-load Addresses:", "Adres Otomatik Yükleme:"),
            ("Roblox Version:", "Roblox Sürümü:"),
            ("Not detected", "Algılanmadı"),
            ("Software Version:", "Yazılım Sürümü:"),
            ("Unknown", "Bilinmeyen"),
            ("Version Compatibility:", "Sürüm Uyumluluğu:"),
            ("MATCH", "UYUŞUYOR"),
            ("MISMATCH", "UYUŞMUYOR"),
            ("UNKNOWN", "BİLİNMİYOR"),
            ("Current Configuration:", "Mevcut Yapılandırma:"),
            ("now", "şimdi"),
            ("s ago", "sn önce"),
            ("m ago", "dk önce"),
            ("Success", "Başarılı"),
            ("Failed", "Başarısız"),
            ("Mixed", "Karışık"),
            ("Pending", "Beklemede"),
            ("Status", "Durum"),
            ("Actions", "Eylemler"),
            ("Session", "Oturum"),
            ("Last", "Son"),
            ("{0} entries", "{0} kayıt"),
            ("Activity log cleared", "Etkinlik günlüğü temizlendi"),
            ("Application successful ({0} FFlags)", "Enjeksiyon başarılı ({0} FFlags)"),
            ("Application failed ({0} errors)", "Enjeksiyon başarısız ({0} hata)")
        );

        private static readonly Dictionary<string, string> It = D(
            ("System initialized", "Sistema inizializzato"),
            ("Ready to load FFlags", "Pronto a caricare FFlags"),
            ("Not set", "Non impostato"),
            ("Saved FFlags:", "FFlags Salvati:"),
            ("Auto-load FFlags:", "Caricamento automatico FFlags:"),
            ("Enabled", "Abilitato"),
            ("Disabled", "Disabilitato"),
            ("Auto-load Addresses:", "Caricamento automatico Indirizzi:"),
            ("Roblox Version:", "Versione Roblox:"),
            ("Not detected", "Non rilevato"),
            ("Software Version:", "Versione Software:"),
            ("Unknown", "Sconosciuto"),
            ("Version Compatibility:", "Compatibilità Versione:"),
            ("MATCH", "CORRISPONDE"),
            ("MISMATCH", "INCOMPATIBILE"),
            ("UNKNOWN", "SCONOSCIUTO"),
            ("Current Configuration:", "Configurazione Corrente:"),
            ("now", "ora"),
            ("s ago", "s fa"),
            ("m ago", "m fa"),
            ("Success", "Successo"),
            ("Failed", "Fallito"),
            ("Mixed", "Misto"),
            ("Pending", "In attesa"),
            ("Status", "Stato"),
            ("Actions", "Azioni"),
            ("Session", "Sessione"),
            ("Last", "Ultima"),
            ("{0} entries", "{0} voci"),
            ("Activity log cleared", "Registro attività svuotato"),
            ("Application successful ({0} FFlags)", "Iniezione riuscita ({0} FFlags)"),
            ("Application failed ({0} errors)", "Iniezione fallita ({0} errori)")
        );

        private static readonly Dictionary<string, string> ArAe = D(
            ("System initialized", "تم تهيئة النظام"),
            ("Ready to load FFlags", "جاهز لتحميل FFlags"),
            ("Not set", "لم يحدد"),
            ("Saved FFlags:", "ملفات FFlags المحفوظة:"),
            ("Auto-load FFlags:", "تحميل تلقائي لملفات FFlags:"),
            ("Enabled", "مفعل"),
            ("Disabled", "معطل"),
            ("Auto-load Addresses:", "تحميل تلقائي للعناوين:"),
            ("Roblox Version:", "نسخة Roblox:"),
            ("Not detected", "لم يتم اكتشافه"),
            ("Software Version:", "نسخة البرنامج:"),
            ("Unknown", "غير معروف"),
            ("Version Compatibility:", "توافق النسخة:"),
            ("MATCH", "متطابق"),
            ("MISMATCH", "غير متطابق"),
            ("UNKNOWN", "غير معروف"),
            ("Current Configuration:", "التكوين الحالي:"),
            ("now", "الآن"),
            ("s ago", "ثانية مضت"),
            ("m ago", "دقيقة مضت"),
            ("Success", "نجاح"),
            ("Failed", "فشل"),
            ("Mixed", "مختلط"),
            ("Pending", "قيد الانتظار"),
            ("Status", "الحالة"),
            ("Actions", "العمليات"),
            ("Session", "الجلسة"),
            ("Last", "الأخيرة"),
            ("{0} entries", "{0} سجلات"),
            ("Activity log cleared", "تم مسح سجل النشاط"),
            ("Application successful ({0} FFlags)", "تم الحقن بنجاح ({0} FFlags)"),
            ("Application failed ({0} errors)", "فشل الحقن ({0} أخطاء)")
        );

        private static readonly Dictionary<string, string> De = D(
            ("System initialized", "System initialisiert"),
            ("Ready to load FFlags", "Bereit zum Laden von FFlags"),
            ("Not set", "Nicht festgelegt"),
            ("Saved FFlags:", "Gespeicherte FFlags:"),
            ("Auto-load FFlags:", "Auto-Laden FFlags:"),
            ("Enabled", "Aktiviert"),
            ("Disabled", "Deaktiviert"),
            ("Auto-load Addresses:", "Auto-Laden Adressen:"),
            ("Roblox Version:", "Roblox-Version:"),
            ("Not detected", "Nicht erkannt"),
            ("Software Version:", "Software-Version:"),
            ("Unknown", "Unbekannt"),
            ("Version Compatibility:", "Versionskompatibilität:"),
            ("MATCH", "ÜBEREINSTIMMUNG"),
            ("MISMATCH", "KEINE ÜBEREINSTIMMUNG"),
            ("UNKNOWN", "UNBEKANNT"),
            ("Current Configuration:", "Aktuelle Konfiguration:"),
            ("now", "gerade eben"),
            ("s ago", "Sek. vor"),
            ("m ago", "Min. vor"),
            ("Success", "Erfolgreich"),
            ("Failed", "Fehlgeschlagen"),
            ("Mixed", "Gemischt"),
            ("Pending", "Ausstehend"),
            ("Status", "Status"),
            ("Actions", "Aktionen"),
            ("Session", "Sitzung"),
            ("Last", "Letzte"),
            ("{0} entries", "{0} Einträge"),
            ("Activity log cleared", "Aktivitätsprotokoll gelöscht"),
            ("Application successful ({0} FFlags)", "Injektion erfolgreich ({0} FFlags)"),
            ("Application failed ({0} errors)", "Injektion fehlgeschlagen ({0} Fehler)")
        );

        private static readonly Dictionary<string, string> Ro = D(
            ("System initialized", "Sistem inițializat"),
            ("Ready to load FFlags", "Pregătit pentru a încărca FFlags"),
            ("Not set", "Nesetat"),
            ("Saved FFlags:", "FFlags Salvate:"),
            ("Auto-load FFlags:", "Auto-încărcare FFlags:"),
            ("Enabled", "Activat"),
            ("Disabled", "Dezactivat"),
            ("Auto-load Addresses:", "Auto-încărcare Adrese:"),
            ("Roblox Version:", "Versiune Roblox:"),
            ("Not detected", "Nedetectat"),
            ("Software Version:", "Versiune Software:"),
            ("Unknown", "Necunoscut"),
            ("Version Compatibility:", "Compatibilitate Versiune:"),
            ("MATCH", "POTRIVIRE"),
            ("MISMATCH", "NEPOTRIVIRE"),
            ("UNKNOWN", "NECUNOSCUT"),
            ("Current Configuration:", "Configurație Curentă:"),
            ("now", "acum"),
            ("s ago", "s în urmă"),
            ("m ago", "m în urmă"),
            ("Success", "Succes"),
            ("Failed", "Eșuat"),
            ("Mixed", "Mixt"),
            ("Pending", "În așteptare"),
            ("Status", "Stare"),
            ("Actions", "Acțiuni"),
            ("Session", "Sesiune"),
            ("Last", "Ultima"),
            ("{0} entries", "{0} intrări"),
            ("Activity log cleared", "Jurnal activitate curățat"),
            ("Application successful ({0} FFlags)", "Aplicare reușită ({0} FFlags)"),
            ("Application failed ({0} errors)", "Aplicare eșuată ({0} erori)")
        );

        private static readonly Dictionary<string, string> Sv = D(
            ("System initialized", "Systemet initierat"),
            ("Ready to load FFlags", "Redo att ladda FFlags"),
            ("Not set", "Ej inställt"),
            ("Saved FFlags:", "Sparade FFlags:"),
            ("Auto-load FFlags:", "Ladda FFlags automatiskt:"),
            ("Enabled", "Aktiverad"),
            ("Disabled", "Inaktiverad"),
            ("Auto-load Addresses:", "Ladda adresser automatiskt:"),
            ("Roblox Version:", "Roblox-version:"),
            ("Not detected", "Hittades inte"),
            ("Software Version:", "Programvaruversion:"),
            ("Unknown", "Okänd"),
            ("Version Compatibility:", "Versionskompatibilitet:"),
            ("MATCH", "MATCHAR"),
            ("MISMATCH", "MATCHAR EJ"),
            ("UNKNOWN", "OKÄND"),
            ("Current Configuration:", "Nuvarande konfiguration:"),
            ("now", "just nu"),
            ("s ago", "sek sedan"),
            ("m ago", "min sedan"),
            ("Success", "Lyckades"),
            ("Failed", "Misslyckades"),
            ("Mixed", "Blandat"),
            ("Pending", "Väntar"),
            ("Status", "Status"),
            ("Actions", "Åtgärder"),
            ("Session", "Session"),
            ("Last", "Senaste"),
            ("{0} entries", "{0} logginlägg"),
            ("Activity log cleared", "Aktivitetsloggen rensad"),
            ("Application successful ({0} FFlags)", "Injektion lyckades ({0} FFlags)"),
            ("Application failed ({0} errors)", "Injektion misslyckades ({0} fel)")
        );

        private static readonly Dictionary<string, string> Nl = D(
            ("System initialized", "Systeem geïnitialiseerd"),
            ("Ready to load FFlags", "Klaar om FFlags te laden"),
            ("Not set", "Niet ingesteld"),
            ("Saved FFlags:", "Opgeslagen FFlags:"),
            ("Auto-load FFlags:", "FFlags automatisch laden:"),
            ("Enabled", "Ingeschakeld"),
            ("Disabled", "Uitgeschakeld"),
            ("Auto-load Addresses:", "Adressen automatisch laden:"),
            ("Roblox Version:", "Roblox-versie:"),
            ("Not detected", "Niet gedetecteerd"),
            ("Software Version:", "Softwareversie:"),
            ("Unknown", "Onbekend"),
            ("Version Compatibility:", "Versiecompatibiliteit:"),
            ("MATCH", "MATCHT"),
            ("MISMATCH", "MATCHT NIET"),
            ("UNKNOWN", "ONBEKEND"),
            ("Current Configuration:", "Huidige configuratie:"),
            ("now", "zojuist"),
            ("s ago", "sec geleden"),
            ("m ago", "min geleden"),
            ("Success", "Succes"),
            ("Failed", "Mislukt"),
            ("Mixed", "Gemengd"),
            ("Pending", "In afwachting"),
            ("Status", "Status"),
            ("Actions", "Acties"),
            ("Session", "Sessie"),
            ("Last", "Laatste"),
            ("{0} entries", "{0} regels"),
            ("Activity log cleared", "Activiteitenlogboek gewist"),
            ("Application successful ({0} FFlags)", "Toepassing succesvol ({0} FFlags)"),
            ("Application failed ({0} errors)", "Toepassing mislukt ({0} fouten)")
        );

        private static readonly Dictionary<string, string> Pl = D(
            ("System initialized", "System zainicjalizowany"),
            ("Ready to load FFlags", "Gotowy do załadowania FFlags"),
            ("Not set", "Nie ustawiono"),
            ("Saved FFlags:", "Zapisane FFlags:"),
            ("Auto-load FFlags:", "Autouruchamianie FFlags:"),
            ("Enabled", "Włączone"),
            ("Disabled", "Wyłączone"),
            ("Auto-load Addresses:", "Autouruchamianie adresów:"),
            ("Roblox Version:", "Wersja Roblox:"),
            ("Not detected", "Nie wykryto"),
            ("Software Version:", "Wersja oprogramowania:"),
            ("Unknown", "Nieznana"),
            ("Version Compatibility:", "Zgodność wersji:"),
            ("MATCH", "ZGODNE"),
            ("MISMATCH", "NIEZGODNE"),
            ("UNKNOWN", "NIEZNANA"),
            ("Current Configuration:", "Aktualna konfiguracja:"),
            ("now", "przed chwilą"),
            ("s ago", "sek. temu"),
            ("m ago", "min. temu"),
            ("Success", "Sukces"),
            ("Failed", "Błąd"),
            ("Mixed", "Mieszany"),
            ("Pending", "Oczekujące"),
            ("Status", "Status"),
            ("Actions", "Działania"),
            ("Session", "Sesja"),
            ("Last", "Ostatnie"),
            ("{0} entries", "Wpisów: {0}"),
            ("Activity log cleared", "Wyczyszczono dziennik aktywności"),
            ("Application successful ({0} FFlags)", "Pomyślny zapis ({0} FFlags)"),
            ("Application failed ({0} errors)", "Błąd zapisu ({0} błędów)")
        );
    }
}
