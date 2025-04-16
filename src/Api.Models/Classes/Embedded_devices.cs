using System.Text.RegularExpressions;
// Embedded_devices -> ED
namespace Models;

public class Embedded_devices : Device
{

    public string _IpAddress;
    
    public string IPAddress
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

    public string NetworkName { get; set; }
    
    
    public Embedded_devices(string id, string name, string ipAddress, string networkName) : base(id, name)
    {
        IPAddress = ipAddress;
        NetworkName = networkName;
    }

    public void Connect()
    {
        if (!NetworkName.Contains("MD Ltd"))
        {
            throw new ConnectionException();
        }
        
        Console.WriteLine($"Device: {Name}, IP: {IPAddress}, Connected to Network: {NetworkName}");
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
        return $"Device: {Name}, ID -> {ID}, IP: {IPAddress}, Mode: {(IsDeviceTurned ? "TurnedOn" : "TurnedOff")}, Network: {NetworkName}";
    }
}