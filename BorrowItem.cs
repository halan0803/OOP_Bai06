/********************************/
// Mã sinh viên: 202418931
// Họ tên: Trần Thị Hà Lan
/********************************/

using System;
namespace DeviceManagement
{
    public class BorrowItem
    {
        public Device Device { get; }
        public DateTime BorrowedDate { get; }
        public int FromPeriod { get; set; }
        public int ToPeriod { get; set; }
        public DateTime? ReturnedDate { get; private set; }
        public bool IsReturned => ReturnedDate.HasValue;
        
        public DateTime DueDate { get; }

        public BorrowItem(Device device, DateTime borrowedDate, DateTime dueDate)
        {
            Device = device ?? throw new ArgumentNullException(nameof(device));
            BorrowedDate = borrowedDate;
            // DueDate phải sau hoặc bằng BorrowedDate
            DueDate = dueDate >= borrowedDate ? dueDate : throw new ArgumentException("Ngày hết hạn phải sau hoặc bằng ngày mượn");
            ReturnedDate = null;
        }

        public void Return(DateTime returnedDate)
        {
            // Không được trả một BorrowItem hai lần
            if (IsReturned) 
                throw new DeviceAlreadyReturnedException($"Thiết bị [{Device.DeviceId}] đã được trả trước đó.");
            // Ngày trả không được trước ngày mượn
            if (returnedDate < BorrowedDate) 
                throw new ArgumentException("Ngày trả không được trước ngày mượn.");
            
            ReturnedDate = returnedDate;
        }
    }
}