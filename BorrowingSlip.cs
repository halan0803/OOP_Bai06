/********************************/
// Mã sinh viên: 202418931
// Họ tên: Trần Thị Hà Lan
/********************************/

using System;
using System.Collections.Generic;
using System.Linq;
namespace DeviceManagement
{
    public class BorrowingSlip
    {
        public string SlipId { get; }
        public IBorrower Borrower { get; }
        public string TeachingRoom { get; }
        public DateTime BorrowedDate { get; }
        
        // BorrowingSlip hợp thành các BorrowItem
        private readonly List<BorrowItem> _items = new();
        public IReadOnlyList<BorrowItem> Items => _items.AsReadOnly();

        public BorrowingSlip(string slipId, IBorrower borrower, string teachingRoom)
        {
            if (string.IsNullOrWhiteSpace(slipId)) throw new ArgumentException("Mã phiếu không rỗng");
            
            SlipId = slipId.Trim();
            Borrower = borrower ?? throw new ArgumentNullException(nameof(borrower));
            TeachingRoom = string.IsNullOrWhiteSpace(teachingRoom) ? throw new ArgumentException("Phòng dạy không rỗng") : teachingRoom;
            BorrowedDate = DateTime.Now;
        }

        public void AddDevice(Device device, int borrowDays)
        {
            if (device == null) throw new ArgumentNullException(nameof(device));
            if (borrowDays <= 0) throw new InvalidBorrowPeriodException(device.DeviceId, borrowDays, device.MaxBorrowDays);
            if (borrowDays > device.MaxBorrowDays) throw new InvalidBorrowPeriodException(device.DeviceId, borrowDays, device.MaxBorrowDays);
            if (_items.Any(i => string.Equals(i.Device.DeviceId.Trim(), device.DeviceId.Trim(), StringComparison.OrdinalIgnoreCase))) throw new DuplicateDeviceException($"Thiết bị {device.DeviceId} đã có trong phiếu.");
            if (_items.Count >= Borrower.GetMaximumDevices()) throw new BorrowLimitExceededException(Borrower.BorrowerId, Borrower.GetMaximumDevices());
            if (device.Status != DeviceStatus.Available) throw new DeviceUnavailableException(device.DeviceId, device.Status);

            // BorrowingSlip hợp thành các BorrowItem
            var item = new BorrowItem(device, BorrowedDate, BorrowedDate.AddDays(borrowDays));
            device.MarkAsBorrowed();
            _items.Add(item);
        }

        public decimal ReturnDevice(string deviceId, DateTime returnedDate)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
                throw new ArgumentException("Mã thiết bị không được để trống.", nameof(deviceId));
            
            var item = _items.FirstOrDefault(i => string.Equals(i.Device.DeviceId.Trim(), deviceId.Trim(), StringComparison.OrdinalIgnoreCase));

            if (item == null) 
                throw new DeviceNotInSlipException($"Thiết bị {deviceId} không thuộc phiếu mượn này.");
            
            item.Return(returnedDate);
            item.Device.MarkAsAvailable();
            
            // Tính phí phạt cơ bản nếu trả muộn (ví dụ: 50.000đ/ngày muộn)
            decimal lateFee = 0;
            if (returnedDate > item.DueDate)
            {
                // Quy ước: Chỉ tính số ngày muộn trọn vẹn,
                // phần thời gian lẻ dưới 24 giờ không tính thêm một ngày
                // Ví dụ: trả muộn 1 ngày 20 giờ thì chỉ tính 1 ngày
                int lateDays = (returnedDate - item.DueDate).Days;
                lateFee = lateDays * 50000m;
            }
            return lateFee;
        }

        public decimal CalculateTotalLateFee()
        {
            decimal totalFee = 0;
            foreach (var item in _items.Where(i => i.IsReturned && i.ReturnedDate > i.DueDate))
            {
                int lateDays = (item.ReturnedDate.Value - item.DueDate).Days;
                totalFee += lateDays * 50000m;
            }
            return totalFee;
        }
    }   
}