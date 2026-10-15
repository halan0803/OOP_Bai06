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
        // Mã người mượn chỉ được thiết lập khi khởi tạo
        public string BorrowerId { get; }

        private string _fullName = string.Empty;
        public string FullName
        {
            get => _fullName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Họ tên không được để trống.");
                _fullName = value.Trim();
            }
        }

        public Lecturer(string borrowerId, string fullName)
        {
            if (string.IsNullOrWhiteSpace(borrowerId))
                throw new ArgumentException("Mã người mượn không được để trống.");

            BorrowerId = borrowerId.Trim();
            FullName = fullName; // đi qua setter nên được kiểm tra
        }

        // Giảng viên được mượn tối đa 3 thiết bị trên một phiếu
        public int GetMaximumDevices() => 3;
    }

    // TeachingAssistant thực thi IBorrower
    public class TeachingAssistant : IBorrower
    {
        // Mã người mượn chỉ được thiết lập khi khởi tạo
        public string BorrowerId { get; }

        private string _fullName = string.Empty;
        public string FullName
        {
            get => _fullName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Họ tên không được để trống.");
                _fullName = value.Trim();
            }
        }

        public TeachingAssistant(string borrowerId, string fullName)
        {
            if (string.IsNullOrWhiteSpace(borrowerId))
                throw new ArgumentException("Mã người mượn không được để trống.");

            BorrowerId = borrowerId.Trim();
            FullName = fullName;
        }

        // Trợ giảng được mượn tối đa 2 thiết bị trên một phiếu
        public int GetMaximumDevices() => 2;
    }
}