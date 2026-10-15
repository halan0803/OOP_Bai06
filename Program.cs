/***************/
// Mã sinh viên: 202418931
// Họ tên: Trần Thị Hà Lan
/***************/

using System;
using System.Text;
using System.Linq;
namespace DeviceManagement
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("=== HỆ THỐNG QUẢN LÝ THIẾT BỊ VÀ HỢP ĐỒNG BẢO TRÌ ===\n");

            // CHUẨN BỊ DỮ LIỆU 
            var gv = new Lecturer("GV01", "Nguyễn Văn A");
            var tg = new TeachingAssistant("TG01", "Trần Trợ Giảng");

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
                slip.AddDevice(acRemote, 1);
                Console.WriteLine("    [OK] Đã thêm PRJ01, MIC01 và AC01 vào phiếu mượn.");

                // 5. Trả điều khiển máy chiếu đúng hạn
                slip.ReturnDevice("PRJ01", DateTime.Now.AddHours(2));
                Console.WriteLine("    [OK] Đã trả thiết bị PRJ01 đúng hạn.");

                // 6. Trả micro muộn 2 ngày
                decimal lateFee = slip.ReturnDevice("MIC01", slip.BorrowedDate.AddDays(3));
                Console.WriteLine($"    [OK] Đã trả thiết bị MIC01 muộn 2 ngày. Phạt: {lateFee:N0} đ");

                
                // 7. Hiển thị các phiếu trả muộn, chưa trả
                Console.WriteLine("\n    -- Danh sách thiết bị trả muộn --");

                var lateItems = slip.Items
                    .Where(item => item.IsReturned &&
                                    item.ReturnedDate > item.DueDate)
                    .ToList();

                if (lateItems.Count == 0)
                {
                    Console.WriteLine("    Không có thiết bị trả muộn.");
                }
                else
                {
                    foreach (var item in lateItems)
                    {
                        int lateDays = (item.ReturnedDate.Value - item.DueDate).Days;

                        Console.WriteLine($"    {item.Device.DeviceId} - {item.Device.DeviceName}, " + $"trả muộn {lateDays} ngày.");
                    }
                }

                Console.WriteLine("\n    -- Danh sách thiết bị chưa trả --");

                var unreturnedItems = slip.Items
                    .Where(item => !item.IsReturned)
                    .ToList();

                if (unreturnedItems.Count == 0)
                {
                    Console.WriteLine("    Không có thiết bị chưa trả.");
                }
                else
                {
                    foreach (var item in unreturnedItems)
                    {
                        string status = DateTime.Now > item.DueDate ? "Đã quá hạn, chưa trả" : "Chưa trả, chưa quá hạn";

                        Console.WriteLine($"    {item.Device.DeviceId} - {item.Device.DeviceName}, " + $"hạn trả: {item.DueDate:dd/MM/yyyy}, trạng thái: {status}.");
                    }
                }

                Console.WriteLine($"\n    => Tổng tiền phạt của phiếu: " + $"{slip.CalculateTotalLateFee():N0} đ\n");

            }
            catch (BorrowingException ex)
            {
                Console.WriteLine($"    [LỖI NGHIỆP VỤ - {ex.GetType().Name}]: {ex.Message}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    [LỖI HỆ THỐNG - {ex.GetType().Name}]: {ex.Message}\n");
            }


            // ==========================================
            // LUỒNG PHÁT SINH NGOẠI LỆ
            // ==========================================
            Console.WriteLine("---> [Luồng phát sinh ngoại lệ]");
            var slipTG = new BorrowingSlip("SLIP-102", tg, "D9-301");
            var mic2 = new WirelessMic("MIC02", "Micro dự phòng", 2024, 1500000m, 1);
            var acRemote2 = new AirConRemote("AC02", "Điều khiển điều hòa hỏng", 2020, 200000m);
            acRemote2.SendToMaintenance();
            var m1 = new WirelessMic("M1", "Mic 1", 2024, 1m);
            var m2 = new WirelessMic("M2", "Mic 2", 2024, 1m);
            var m3 = new WirelessMic("M3", "Mic 3", 2024, 1m);

            // 1. Mượn thiết bị đang UnderMaintenance
            TryCatchBlock<DeviceUnavailableException>("1. Mượn điều khiển điều hòa đang UnderMaintenance",
                () => slipTG.AddDevice(acRemote2, 1),
                () =>
                {
                    Check("AC02 vẫn UnderMaintenance", acRemote2.Status == DeviceStatus.UnderMaintenance);
                    Check("Phiếu SLIP-102 vẫn 0 item", slipTG.Items.Count == 0);
                });

            // 2. Trợ giảng mượn thiết bị thứ ba
            TryCatchBlock<BorrowLimitExceededException>("2. Trợ giảng mượn thiết bị thứ ba",
                () =>
                {
                    slipTG.AddDevice(m1, 1);
                    slipTG.AddDevice(m2, 1);
                    slipTG.AddDevice(m3, 1); // lỗi ở đây
                },
                () =>
                {
                    Check("Phiếu vẫn đúng 2 item", slipTG.Items.Count == 2);
                    Check("M3 vẫn Available", m3.Status == DeviceStatus.Available);
                });

            // 3. Thêm cùng một micro hai lần
            var slipLoi = new BorrowingSlip("SLIP-103", gv, "D9-401");
            TryCatchBlock<DuplicateDeviceException>("3. Thêm cùng một micro hai lần",
                () =>
                {
                    slipLoi.AddDevice(mic2, 1);
                    slipLoi.AddDevice(mic2, 1);
                },
                () =>
                {
                    Check("Phiếu chỉ có 1 item (lần thêm thứ hai không được ghi)", slipLoi.Items.Count == 1);
                    Check("MIC02 vẫn Borrowed", mic2.Status == DeviceStatus.Borrowed);
                });

            // 4. Mượn micro 3 ngày dù giới hạn 1 ngày
            var mic3 = new WirelessMic("MIC03", "Mic 3", 2023, 1m, maxBorrowDays: 1);
            TryCatchBlock<InvalidBorrowPeriodException>("4. Mượn micro 3 ngày dù giới hạn là 1 ngày",
                () => slipLoi.AddDevice(mic3, 3),
                () =>
                {
                    Check("MIC03 vẫn Available", mic3.Status == DeviceStatus.Available);
                    Check("Phiếu SLIP-103 vẫn 1 item", slipLoi.Items.Count == 1);
                });

            // 5. Trả thiết bị không thuộc phiếu
            TryCatchBlock<DeviceNotInSlipException>("5. Trả thiết bị không thuộc phiếu",
                () => slipLoi.ReturnDevice("KHONG_CO_TRONG_PHIEU", DateTime.Now),
                () => Check("Phiếu SLIP-103 vẫn 1 item", slipLoi.Items.Count == 1));

            // 6. Trả cùng một thiết bị hai lần
            TryCatchBlock<DeviceAlreadyReturnedException>("6. Trả cùng một thiết bị hai lần",
                () =>
                {
                    slipLoi.ReturnDevice("MIC02", DateTime.Now.AddHours(1)); // lần 1 thành công
                    slipLoi.ReturnDevice("MIC02", DateTime.Now.AddHours(2)); // lần 2 báo lỗi
                },
                () =>
                {
                    Check("MIC02 là Available sau lần trả đầu", mic2.Status == DeviceStatus.Available);
                    Check("MIC02 vẫn chỉ được ghi nhận trả một lần", slipLoi.Items[0].IsReturned);
                });

            // 7. Ngày trả trước ngày mượn
            var slipLoiDate = new BorrowingSlip("SLIP-104", gv, "D9-501");
            var acRemote3 = new AirConRemote("AC03", "AC 3", 2024, 300000m);
            slipLoiDate.AddDevice(acRemote3, 1);
            TryCatchBlock<ArgumentException>("7. Trả thiết bị với ngày trả trước ngày mượn",
                () => slipLoiDate.ReturnDevice("AC03", slipLoiDate.BorrowedDate.AddDays(-1)),
                () =>
                {
                    Check("AC03 vẫn Borrowed", acRemote3.Status == DeviceStatus.Borrowed);
                    Check("Item chưa bị đánh dấu đã trả", !slipLoiDate.Items[0].IsReturned);
                });

            Console.WriteLine("\n============== HOÀN THÀNH KIỂM THỬ ==============");
            Console.ReadLine();
        }

        // Chạy action, mong đợi ném đúng TExpected. Sau đó chạy verify để kiểm tra trạng thái.
        static void TryCatchBlock<TExpected>(string testName, Action action, Action? verify = null)
            where TExpected : Exception
        {
            Console.WriteLine($"  * Test: {testName}");
            try
            {
                action();
                Console.WriteLine($"    -> [FAIL] Không có ngoại lệ nào, mong đợi {typeof(TExpected).Name}.");
            }
            catch (TExpected ex)
            {
                Console.WriteLine($"    -> [PASS] Bắt đúng {ex.GetType().Name}: {ex.Message}");
                verify?.Invoke();
            }
            catch (BorrowingException ex)
            {
                Console.WriteLine($"    -> [FAIL] Sai loại nghiệp vụ: nhận {ex.GetType().Name}, mong đợi {typeof(TExpected).Name}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    -> [FAIL] Ngoại lệ ngoài dự kiến {ex.GetType().Name}: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("    -> Kết thúc ca kiểm thử.");
            }
        }

        // In kết quả một điều kiện kiểm tra trạng thái
        static void Check(string description, bool condition)
        {
            Console.WriteLine($"       {(condition ? "[OK]  " : "[LỖI]")} {description}");
        }
    }
}