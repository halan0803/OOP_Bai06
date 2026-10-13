/********************************/
// Mã sinh viên: 202418931
// Họ tên: Trần Thị Hà Lan
/********************************/

using System;
namespace DeviceManagement
{
    public enum DeviceStatus 
    { 
        Available, 
        Borrowed, 
        UnderMaintenance 
    }
    
    public abstract class Device
    {
        public string DeviceId { get; init; }
        public string DeviceName { get; set; }
        private int _yearOfUse;
        public int YearOfUse
        {
            get => _yearOfUse;
            set
            {
                if (value <= 0 || value > DateTime.Now.Year)
                    throw new ArgumentException("Năm sử dụng không hợp lệ.");

                _yearOfUse = value;
            }
        }
        private decimal _purchasePrice;
        public decimal PurchasePrice
        {
            get => _purchasePrice;
            set => _purchasePrice = value > 0 ? value
                : throw new ArgumentException("Giá mua phải lớn hơn 0.");
        }
        public DeviceStatus Status { get; set; }

        public Device(string deviceId, string deviceName, int yearOfUse, decimal purchasePrice, DeviceStatus status = DeviceStatus.Available)
        {
            // Mã thiết bị không được rỗng
            if (string.IsNullOrWhiteSpace(deviceId))
                throw new ArgumentException("Mã thiết bị không được rỗng.");

            DeviceId = deviceId.Trim();
            DeviceName = deviceName;
            YearOfUse = yearOfUse;
            PurchasePrice = purchasePrice;
            Status = status;
        }
    
        // Phương thức trừu tượng tính chi phí bảo trì dư kiến trong 1 năm
        public abstract decimal CalculateMaintenanceCost();

        // Ghi đè phương thức biểu diễn thông tin của thiết bị dưới dạng chuỗi
        public override string ToString()
            {
                return $"[{DeviceId}] {DeviceName} | Năm SD: {YearOfUse} | Giá mua: {PurchasePrice:N0} đ | Trạng thái: {Status} | Phí bảo trì: {CalculateMaintenanceCost():N0} đ";
            }
    }
}