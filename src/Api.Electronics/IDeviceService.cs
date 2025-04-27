using Models;

namespace Electronics;

public interface IDeviceService
{
    public IEnumerable<DeviceDto> GetAllModels();
    public Device GetDeviceById(string? id);
    public string CreateDevice(string jsonData);
    public void UpdateDevice(string jsonData);
    public void DeleteDevice(string deviceId);
}