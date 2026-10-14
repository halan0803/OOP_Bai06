/***************/
// Mã sinh viên: 202418931
// Họ tên: Trần Thị Hà Lan
/***************/

using System;
namespace DeviceManagement
{
    // WirelessMic kế thừa từ Device
    public class WirelessMic : Device
    {
        public WirelessMic(string deviceId, string deviceName, int yearOfUse, decimal purchasePrice, int maxBorrowDays = 1) 
            : base(deviceId, deviceName, yearOfUse, purchasePrice, maxBorrowDays) { }

        // Chi phí bảo trì hằng năm bằng 15% giá mua
        public override decimal CalculateAnnualMaintenanceCost() => PurchasePrice * 0.15m;
    }
}