/***************/
// Mã sinh viên: 202418931
// Họ tên: Trần Thị Hà Lan
/***************/

using System;
namespace DeviceManagement
{
    // AirConRemote kế thừa từ Device
    public class AirConRemote : Device
    {
        public AirConRemote(string deviceId, string deviceName, int yearOfUse, decimal purchasePrice, int maxBorrowDays = 3, DeviceStatus status = DeviceStatus.Available) 
            : base(deviceId, deviceName, yearOfUse, purchasePrice, maxBorrowDays, status) { }

        // Chi phí bảo trì hằng năm bằng 5% giá mua
        public override decimal CalculateAnnualMaintenanceCost() => PurchasePrice * 0.05m;
    }
}