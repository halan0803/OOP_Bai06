/***************/
// Mã sinh viên: 202418931
// Họ tên: Trần Thị Hà Lan
/***************/

using System;
namespace DeviceManagement
{
    // ProjectorRemote kế thừa từ Device
    public class ProjectorRemote : Device
    {
        public ProjectorRemote(string deviceId, string deviceName, int yearOfUse, decimal purchasePrice, int maxBorrowDays = 3) 
            : base(deviceId, deviceName, yearOfUse, purchasePrice, maxBorrowDays) { }

        // Chi phí bảo trì hằng năm bằng 10% giá mua
        public override decimal CalculateAnnualMaintenanceCost() => PurchasePrice * 0.1m;
    }
}