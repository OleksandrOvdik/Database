using Api.DTO;
using Models;
using Repository.Interface;


namespace Electronics;

public class DeviceService : IDeviceService
{
    
    private readonly IDeviceRepository _deviceRepository;
    

    public DeviceService(IDeviceRepository deviceRepository)
    {
        _deviceRepository = deviceRepository;
    }

    public IEnumerable<DeviceDTO> GetAllModels()
    {
        var result = _deviceRepository.GetAllModels();
        return result;
    }

    public Device GetDeviceById(string? id)
    {
        var result = _deviceRepository.GetDeviceById(id);
        return result;
    }

    public string CreateDevice(string jsonData)
    {
        var result = _deviceRepository.CreateDevice(jsonData);
        return result;
    }

    public bool UpdateDevice(string jsonData)
    {
        var result = _deviceRepository.UpdateDevice(jsonData);
        return result;
    }

    public bool DeleteDevice(string deviceId)
    {
        var result = _deviceRepository.DeleteDevice(deviceId);
        return result;
    }
}