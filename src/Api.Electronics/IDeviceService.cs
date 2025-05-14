using System.Text.Json;
using Api.DTO;
using Models;

namespace Electronics;

public interface IDeviceService
{
    public IEnumerable<DeviceDTO> GetAllModels();
    public Device GetDeviceById(string? id);
    public Task<string> CreateDevice(string data);
    public Task<bool> UpdateDevice(string jsonData);
    public Task<bool> DeleteDevice(string deviceId);
    public bool IsJson(string input);
    public Device ParseTextData(string textData);
    public string ToJson(Device device);
    public Device ParseJsonData(JsonElement json);

    public void ValidateDevice(Device device);
}