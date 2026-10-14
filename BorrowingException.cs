/********************************/
// Mã sinh viên: 202418931
// Họ tên: Trần Thị Hà Lan
/********************************/

using System;
namespace DeviceManagement
{
    // Ngoại lệ cơ sở 
    public class BorrowingException : Exception
    {
        public BorrowingException(string message) : base(message) { }
    }

    // Các ngoại lệ dẫn xuất bắt buộc
    // Phát sinh khi thiết bị không ở trạng thái Available
    public class DeviceUnavailableException : BorrowingException
    {
        public string DeviceId { get; }
        public DeviceStatus CurrentStatus { get; }
        public DeviceUnavailableException(string deviceId, DeviceStatus status) 
            : base($"Thiết bị {deviceId} hiện đang ở trạng thái {status}, không thể mượn.") 
        {
            DeviceId = deviceId;
            CurrentStatus = status;
        }
    }

    // Phát sinh khi người mượn đã đạt số thiết bị tối đa
    public class BorrowLimitExceededException : BorrowingException 
    { 
        public string BorrowerId { get; }
        public int MaximumDevices { get; }
        public BorrowLimitExceededException(string borrowerId, int maxDevices) 
            : base($"Người mượn {borrowerId} đã đạt giới hạn tối đa {maxDevices} thiết bị.") 
        { 
            BorrowerId = borrowerId;
            MaximumDevices = maxDevices;
        } 
    }

    // Phát sinh khi số ngày mượn không dương
    public class InvalidBorrowPeriodException : BorrowingException 
    {
        public InvalidBorrowPeriodException(string deviceId, int requestDays, int maxDays)
            : base($"Thiết bị {deviceId} chỉ được mượn tối đa {maxDays} ngày, yêu cầu mượn {requestDays} ngày là không hợp lệ.") { }
    }

    // Phát sinh khi cùng một thiết bị đã tồn tại trong phiếu
    public class DuplicateDeviceException : BorrowingException { public DuplicateDeviceException(string msg) : base(msg) { } }
    
    // Phát sinh khi yêu cầu trả một thiết bị không thuộc phiếu
    public class DeviceNotInSlipException : BorrowingException { public DeviceNotInSlipException(string msg) : base(msg) { } }
    
    // Phát sinh khi thiết bị trong phiếu đã được trả trước đó
    public class DeviceAlreadyReturnedException : BorrowingException { public DeviceAlreadyReturnedException(string msg) : base(msg) { } }
}