namespace Api.DTO;

public class DeviceDTO
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public bool IsDeviceTurned { get; set; }
    
    public string? OperatingSystem { get; set; }
    public string? batteryPercent { get; set; }
    
    public string? IpAddress { get; set; }
    public string? NetworkName { get; set; }
    
}