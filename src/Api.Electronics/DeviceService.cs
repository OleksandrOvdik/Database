using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Api.DTO;
using Models;
using Models.Classes;
using Repository.Interface;


namespace Electronics;

public class DeviceService : IDeviceService
{
    
    private readonly IDeviceRepository _deviceRepository;
    
    private readonly Regex regex =  new Regex(@"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$");
    

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

    public async Task<string> CreateDevice(string data)
    {
        Device device;
        if (IsJson(data))
        {
            var json = JsonDocument.Parse(data).RootElement;
            device = ParseJsonData(json);
        }
        else
        {
            device = ParseTextData(data);
        }

        ValidateDevice(device); 

        var deviceType = device.Id.Split('-')[0].ToUpper();
        return deviceType switch
        {
            "ED" => await _deviceRepository.CreateEmbeddedDevice((EmbeddedDevices)device),
            "PC" => await _deviceRepository.CreatePersonalComputerDevice((PersonalComputer)device),
            "SW" => await _deviceRepository.CreateSmartwatchDevice((SmartWatches)device),
            _ => throw new ArgumentException("Invalid device type")
        };
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

   


    public void ValidateDevice(Device device)
    {
        switch (device)
        {
            case SmartWatches sw:
                
                if(sw.batteryPercent > 100 || sw.batteryPercent < 0) throw new ArgumentException("Battery percent must be between 0 and 100");
                if (sw.IsDeviceTurned)
                {
                    if (sw.batteryPercent < 11)  
                        throw new EmptyBatteryException();

                    sw.batteryPercent -= 10;

                }
                break;
            case PersonalComputer pc:
                
                if(pc.IsDeviceTurned && string.IsNullOrEmpty(pc.OperatingSystem)) throw new EmptySystemException();
                
                break;
            case EmbeddedDevices ed:
                
                if(!regex.IsMatch(ed.IpAddress)) throw new ArgumentException("IP address is invalid");

                if (ed.IsDeviceTurned)
                {
                    if (!ed.NetworkName.Contains("MD Ltd.")) throw new ConnectionException();
                }
                
                break;
        }
    }
    

    public bool IsJson(string input)
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
    
    
    public  Device ParseJsonData(JsonElement json)
    {
        var deviceType = json.GetProperty("deviceType").GetString().ToUpper();
        var name = json.GetProperty("name").GetString();
        var isEnabled = json.GetProperty("isEnabled").GetBoolean();

        var id = $"{deviceType}-{Guid.NewGuid():N}";

        return deviceType switch
        {
            "ED" => new EmbeddedDevices(
                id: id,
                name: name,
                ipAddress: json.GetProperty("ipAddress").GetString(),
                networkName: json.GetProperty("networkName").GetString()
            ) { IsDeviceTurned = isEnabled },

            "PC" => new PersonalComputer(
                id: id,
                name: name,
                operatingSystem: json.GetProperty("operationSystem").GetString()
            ) { IsDeviceTurned = isEnabled },

            "SW" => new SmartWatches(
                id: id,
                name: name,
                batteryPercent: json.GetProperty("batteryPercentage").GetInt32()
            ) { IsDeviceTurned = isEnabled },

            _ => throw new ArgumentException($"Невідомий тип пристрою: {deviceType}")
        };
    }
    

    
}