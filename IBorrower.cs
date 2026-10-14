/********************************/
// Mã sinh viên: 202418931
// Họ tên: Trần Thị Hà Lan
/********************************/

using System;
namespace DeviceManagement
{ 
    public interface IBorrower 
    { 
        string BorrowerId { get; } 
        string FullName { get; } 
        int GetMaximumDevices(); 
    }

    // Lecturer thực thi IBorrower
    public class Lecturer : IBorrower
    {
        public string BorrowerId { get; set; }
        public string FullName { get; set; }
        // Giảng viên được mượn tối đa 3 thiết bị trên một phiếu
        public int GetMaximumDevices() => 3; 
    }

    // TeachingAssistant thực thi IBorrower
    public class TeachingAssistant : IBorrower
    {
        public string BorrowerId { get; set; }
        public string FullName { get; set; }
        // Trợ giảng được mượn tối đa 2 thiết bị trên một phiếu
        public int GetMaximumDevices() => 2; 
    }
}