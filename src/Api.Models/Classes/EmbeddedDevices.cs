using System.Text.RegularExpressions;

// Embedded_devices -> ED
namespace Models.Classes;
public class EmbeddedDevices : Device
{

    public string? _IpAddress;
    
    public string? IpAddress
    {
        get { return _IpAddress;} 
        set
        {
            string regex = @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
            if (!Regex.IsMatch(value, regex))
            {
                throw new ArgumentException("Invalid IP address");
            }
            _IpAddress = value;
        }
    }

    public string? NetworkName { get; set; }
    
    
    public EmbeddedDevices(string? id, string? name, string? ipAddress, string? networkName) : base(id, name)
    {
        IpAddress = ipAddress;
        NetworkName = networkName;
    }

    public EmbeddedDevices(Device baseDevice) : base("","")
    {
    }

    public void Connect()
    {
        if (!NetworkName.Contains("MD Ltd"))
        {
            throw new ConnectionException();
        }
        
        Console.WriteLine($"Device: {Name}, IP: {IpAddress}, Connected to Network: {NetworkName}");
    }

    public override void TurnedOn()
    {
        try
        {
            Connect();
            
            base.TurnedOn();
            Console.WriteLine($"    TURNED ON -> povelzo :) {Name}");
        }
        catch (ConnectionException e)
        {
            Console.WriteLine($"    NOT CONNECTED -> ne povezlo :(  {e.Message}");
            throw;
        }
    }

    public override string ToString()
    {
        return $"Device: {Name}, ID -> {Id}, IP: {IpAddress}, Mode: {(IsDeviceTurned ? "TurnedOn" : "TurnedOff")}, Network: {NetworkName}";
    }
}