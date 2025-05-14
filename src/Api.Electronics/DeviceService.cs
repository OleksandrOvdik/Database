using System.Text.Json;
using Api.DTO;
using Models;
using Models.Classes;
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

    public Device GetDeviceById(string id)
    {
        var device = _deviceRepository.GetDeviceById(id);
        return device;
    }

    public async Task<string> CreateDevice(string jsonData)
    {

        if (isJson(jsonData))
        {
            return await _deviceRepository.CreateDevice(jsonData);
        }
        else
        {
            var device = ParseTextData(jsonData);
            var json = ToJson(device);
            return await _deviceRepository.CreateDevice(json);
        }
        
    }

    public bool isJson(string input)
    {
        try
        {
            JsonDocument.Parse(input);
            return true;
        } 
        catch (Exception)
        {
            return false;
        }
    }

    public Device ParseTextData(string textData)
    {
        var parts = textData.Split(',', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 3)  throw new ArgumentException("WRONG DATA");
        
        var deviceType = parts[0].Trim().ToUpper();
        var name = parts[1].Trim();
        var isEnabled = bool.Parse(parts[2].Trim());
        var id = $"{deviceType}-{Guid.NewGuid():N}";

        return deviceType switch
        {
            "ED" => new EmbeddedDevices(id, name, parts[3].Trim(), parts[4].Trim())
            {
                IsDeviceTurned = isEnabled
            },
            "PC" => new PersonalComputer(id, name, parts.Length > 3 ? parts[3].Trim() : null)
            {
                IsDeviceTurned = isEnabled
            },
            "SW" => new SmartWatches(id, name, int.Parse(parts[3].Trim().TrimEnd('%')))
            {
                IsDeviceTurned = isEnabled
            },
            _ => throw new ArgumentException("Invalid device type")
        };
        
    }

    public string ToJson(Device device)
    {
        return JsonSerializer.Serialize(new
        {
            deviceType = device.Id.Split('-')[0],
            name = device.Name,
            isEnabled = device.IsDeviceTurned,
            ipAddress = (device as EmbeddedDevices)?.IpAddress,
            networkName = (device as EmbeddedDevices)?.NetworkName,
            operationSystem = (device as PersonalComputer)?.OperatingSystem,
            batteryPercentage = (device as SmartWatches)?.batteryPercent
        });
    }
    

    public Task<bool> UpdateDevice(string jsonData)
    {
        var result = _deviceRepository.UpdateDevice(jsonData);
        return result;
    }

    public Task<bool> DeleteDevice(string deviceId)
    {
        var result = _deviceRepository.DeleteDevice(deviceId);
        return result;
    }
}