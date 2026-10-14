/***************/
// Mã sinh viên: 202418931
// Họ tên: Trần Thị Hà Lan
/***************/

using System;
using System.Text;
namespace DeviceManagement
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("=== HỆ THỐNG QUẢN LÝ THIẾT BỊ VÀ HỢP ĐỒNG BẢO TRÌ ===\n");

            // CHUẨN BỊ DỮ LIỆU 
            var gv = new Lecturer { BorrowerId = "GV01", FullName = "Nguyễn Văn A" };
            var tg = new TeachingAssistant { BorrowerId = "TG01", FullName = "Trần Trợ Giảng" };

            var pjRemote = new ProjectorRemote("PRJ01", "Điều khiển máy chiếu 1", 2023, 500000m, maxBorrowDays: 3);
            var acRemote = new AirConRemote("AC01", "Điều khiển điều hòa 1", 2023, 300000m, maxBorrowDays: 3);
            var mic1 = new WirelessMic("MIC01", "Micro không dây 1", 2022, 1200000m, maxBorrowDays: 1);
            
            var slip = new BorrowingSlip("SLIP-101", gv, "D9-202");

            // ==========================================
            // LUỒNG THÀNH CÔNG
            // ==========================================
            Console.WriteLine("---> [Luồng thành công]");
            try
            {
                // 4. Thêm điều khiển máy chiếu và micro vào phiếu
                slip.AddDevice(pjRemote, 1);
                slip.AddDevice(mic1, 1);
                Console.WriteLine("    [OK] Đã thêm PRJ01 và MIC01 vào phiếu mượn.");

                // 5. Trả điều khiển máy chiếu đúng hạn
                slip.ReturnDevice("PRJ01", DateTime.Now.AddHours(2));
                Console.WriteLine("    [OK] Đã trả thiết bị PRJ01 đúng hạn.");

                // 6. Trả micro muộn 2 ngày
                decimal lateFee = slip.ReturnDevice("MIC01", slip.BorrowedDate.AddDays(3));
                Console.WriteLine($"    [OK] Đã trả thiết bị MIC01 muộn 2 ngày. Phạt: {lateFee:N0} đ");

                // 7. Hiển thị các phiếu trả muộn, chưa trả
                Console.WriteLine($"    => Tổng tiền phạt của phiếu: {slip.CalculateTotalLateFee():N0} đ\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    [LỖI LUỒNG THÀNH CÔNG]: {ex.Message}\n");
            }


            // ==========================================
            // LUỒNG PHÁT SINH NGOẠI LỆ
            // ==========================================
            Console.WriteLine("---> [Luồng phát sinh ngoại lệ]");
            var slipTG = new BorrowingSlip("SLIP-102", tg, "D9-301");
            var mic2 = new WirelessMic("MIC02", "Micro dự phòng", 2024, 1500000m, 1);
            var acRemote2 = new AirConRemote("AC02", "Điều khiển điều hòa hỏng", 2020, 200000m);
            acRemote2.Status = DeviceStatus.UnderMaintenance; // Giả lập đang bảo trì

            // 1. Mượn điều khiển điều hòa đang UnderMaintenance
            TryCatchBlock("1. Mượn điều khiển điều hòa đang UnderMaintenance", () => {
                slipTG.AddDevice(acRemote2, 1);
            });

            // 2. Trợ giảng mượn thiết bị thứ ba 
            TryCatchBlock("2. Trợ giảng mượn thiết bị thứ ba", () => {
                slipTG.AddDevice(new WirelessMic("M1", "Mic 1", 2024, 1m), 1);
                slipTG.AddDevice(new WirelessMic("M2", "Mic 2", 2024, 1m), 1);
                slipTG.AddDevice(new WirelessMic("M3", "Mic 3", 2024, 1m), 1); // Bị văng lỗi ở đây
            });

            // 3. Thêm cùng một micro hai lần
            var slipLoi = new BorrowingSlip("SLIP-103", gv, "D9-401");
            TryCatchBlock("3. Thêm cùng một micro hai lần", () => {
                slipLoi.AddDevice(mic2, 1);
                slipLoi.AddDevice(mic2, 1);
            });

            // 4. Mượn micro trong 3 ngày dù giới hạn là 1 ngày
            var mic3 = new WirelessMic("MIC03", "Mic 3", 2023, 1m, maxBorrowDays: 1);
            TryCatchBlock("4. Mượn micro 3 ngày dù giới hạn là 1 ngày", () => {
                slipLoi.AddDevice(mic3, 3);
            });

            // 5. Trả một điều khiển máy chiếu không thuộc phiếu
            TryCatchBlock("5. Trả thiết bị không thuộc phiếu", () => {
                slipLoi.ReturnDevice("KHONG_CO_TRONG_PHIEU", DateTime.Now);
            });

            // 6. Trả cùng một thiết bị hai lần
            TryCatchBlock("6. Trả cùng một thiết bị hai lần", () => {
                slipLoi.ReturnDevice("MIC02", DateTime.Now.AddHours(1)); // Lần 1 thành công
                slipLoi.ReturnDevice("MIC02", DateTime.Now.AddHours(2)); // Lần 2 báo lỗi
            });

            // 7. Trả thiết bị với ngày trả trước ngày mượn
            var slipLoiDate = new BorrowingSlip("SLIP-104", gv, "D9-501");
            var acRemote3 = new AirConRemote("AC03", "AC 3", 2024, 300000m);
            slipLoiDate.AddDevice(acRemote3, 1);
            TryCatchBlock("7. Trả thiết bị với ngày trả trước ngày mượn", () => {
                slipLoiDate.ReturnDevice("AC03", slipLoiDate.BorrowedDate.AddDays(-1));
            });

            Console.WriteLine("\n============== HOÀN THÀNH KIỂM THỬ ==============");
            Console.ReadLine();
        }

        static void TryCatchBlock(string testName, Action action)
        {
            Console.WriteLine($"  * Test: {testName}");

            try
            {
                action();
                Console.WriteLine("    -> Thao tác thành công.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    -> [Bắt lỗi thành công]: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("    -> Kết thúc ca kiểm thử.");
            }
        }
    }
}