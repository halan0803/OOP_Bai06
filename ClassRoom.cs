/********************************/
// Mã sinh viên: 202418931
// Họ tên: Trần Thị Hà Lan
/********************************/

using System;
using System.Collections.Generic;
using System.Linq;
namespace DeviceManagement
{
    public class ClassRoom
    {
        private string _roomId = string.Empty;
        public string RoomId
        {
            get => _roomId;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Mã phòng không được để trống.");
                _roomId = value.Trim();
            }
        }

        private string _roomName = string.Empty;
        public string RoomName
        {
            get => _roomName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Tên phòng không được để trống.");
                _roomName = value.Trim();
            }
        }

        private int _capacity;
        public int Capacity
        {
            get => _capacity;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Sức chứa phải lớn hơn 0.");
                _capacity = value;
            }
        }

        private readonly List<Device> _devices = new();
        public IReadOnlyList<Device> Devices => _devices;

        public ClassRoom(string roomId, string roomName, int capacity)
        {
            RoomId = roomId;
            RoomName = roomName;
            Capacity = capacity;
        }

        // Thêm thiết bị
        public void AddDevice(Device device)
        {
            if (device == null)
                throw new ArgumentNullException(nameof(device), "Không thể thêm thiết bị null vào phòng.");

            if (FindDevice(device.DeviceId) != null)
                throw new InvalidOperationException($"Thiết bị mã '{device.DeviceId}' đã tồn tại trong phòng.");

            _devices.Add(device);
        }

        // Xóa thiết bị theo mã thiết bị
        public bool RemoveDevice(string deviceId)
        {
            var device = FindDevice(deviceId);
            if (device == null) return false;
            return _devices.Remove(device);
        }

        
        // Tìm thiết bị theo mã thiết bị
        public Device? FindDevice(string deviceId)
        {
            if (string.IsNullOrWhiteSpace(deviceId)) return null;
            return _devices.FirstOrDefault(d => d.DeviceId.Equals(deviceId.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        // Tính tổng chi phí bảo trì bằng tính đa hình
        public decimal CalculateAnnualMaintenanceCost()
        {
            decimal total = 0;
            foreach (var device in _devices)
            {
                total += device.CalculateAnnualMaintenanceCost(); 
            }
            return total;
        }

        // Lấy danh sách thiết bị đang UnderMaintenance hoặc đã sử dụng trên 5 năm
        public List<Device> GetDevicesRequiringMaintenance()
        {
            return _devices
                .Where(d => d.Status == DeviceStatus.UnderMaintenance || (DateTime.Now.Year - d.YearOfUse > 5))
                .ToList();
        }
    }
}