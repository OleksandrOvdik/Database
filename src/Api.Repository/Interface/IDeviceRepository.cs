using Api.DTO;
using Models;
using Models.Classes;

namespace Repository.Interface;
public interface IDeviceRepository
{
    public IEnumerable<DeviceDTO> GetAllModels();
    public Device GetDeviceById(string? id);
    
    Task<string> CreateEmbeddedDevice(EmbeddedDevices embeddedDevices);
    Task<string> CreateSmartwatchDevice(SmartWatches smartWatches);
    Task<string> CreatePersonalComputerDevice(PersonalComputer personalComputer);
    public Task<bool> UpdateDevice(string jsonData);
    public Task<bool> DeleteDevice(string deviceId);

    public byte[] TakeRowVerChild(string id);

    public byte[] TakeRowVerDevice(string id);
}