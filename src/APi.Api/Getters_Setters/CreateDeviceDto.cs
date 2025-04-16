namespace RestApi.DTO;

public class CreateDeviceDto
{
    public string DeviceType { get; set; } 
    public string ID { get; set; }
    public string Name { get; set; }
        
    public int? BatteryPercent { get; set; }
        
    public string OperatingSystem { get; set; }
        
    public string IPAddress { get; set; }
    public string NetworkName { get; set; }
    
}