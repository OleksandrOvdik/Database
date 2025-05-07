using Api.DTO;
using Models;

namespace Repository.Interface;

public interface IDeviceRepository

{
    public IEnumerable<DeviceDTO> GetAllModels();
    public Device GetDeviceById(string? id);
    public string CreateDevice(string jsonData);
    public bool UpdateDevice(string jsonData);
    public bool DeleteDevice(string deviceId);
}