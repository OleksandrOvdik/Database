using Api.DTO;
using Models;

namespace Repository.Interface;

public interface IDeviceRepository

{
    public IEnumerable<DeviceDTO> GetAllModels();
    public Device GetDeviceById(string? id);
    public Task<string> CreateDevice(string jsonData);
    public Task<bool> UpdateDevice(string jsonData);
    public Task<bool> DeleteDevice(string deviceId);
}