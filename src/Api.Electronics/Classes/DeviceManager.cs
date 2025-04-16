using Models;
using Models.Classes;

namespace Electronics.Classes;



public class DeviceManager
{
    private const int MaxNumberOfDevice = 15;
    
    public readonly List<Device> Devices;

    private readonly string _filePath;
    
    private static DeviceManager? _instance;
    
    

    public static DeviceManager  Instance
    {
        get
        {
            if (_instance == null)
                _instance = new DeviceManager("input.txt");
            
            return _instance;
        }
    }



    public DeviceManager(string filePath)
    {
        this._filePath = filePath;
        Devices = new List<Device>();

        LoadFromFile();
        

    }
    

    private void LoadFromFile()
    {
        if (!File.Exists(_filePath))
        {
            Console.WriteLine("File does not exist");
        }

        try
        {
            string[] lines = File.ReadAllLines(_filePath);

            foreach (string line in lines)
            {
                try
                {
                    if (string.IsNullOrEmpty(line))
                        continue;

                    string[] parts = line.Split(',');

                    if (parts.Length < 2)
                    {
                        Console.WriteLine("WRITE A NORMAL LINE, YOU SON OF A BITCH");
                        continue;
                    }

                    string deviceFullName = parts[0].Trim();
                    string[] deviceParts = deviceFullName.Split('-');
                    string deviceType = deviceParts[0].Trim();
                    switch (deviceType)
                    {
                        case "SW":
                            if (parts.Length >= 4)
                            {
                                string? id = parts[0].Trim();
                                string? name = parts[1].Trim();
                                bool isTurnedOn = bool.Parse(parts[2].Trim());
                                int battery = int.Parse(parts[3].TrimEnd('%'));

                                SmartWatches watch = new SmartWatches(id, name, battery);
                                if (isTurnedOn) watch.TurnedOn();
                                Devices.Add(watch);
                            }

                            break;
                        case "P":
                            if (parts.Length >= 3)
                            {
                                string? id = parts[0].Trim();
                                string? name = parts[1].Trim();
                                bool isTurnedOn = bool.Parse(parts[2].Trim());

                                string? os = null;
                                if (parts.Length > 3)
                                {
                                    os = parts[3].Trim();
                                    if (os.ToLower() == "null")
                                        os = null;
                                }

                                PersonalComputer pc = new PersonalComputer(id, name, os);
                                if (isTurnedOn && !string.IsNullOrEmpty(os)) pc.TurnedOn();
                                Devices.Add(pc);
                            }

                            break;
                        case "ED":
                            if (parts.Length >= 4)
                            {
                                string? id = parts[0].Trim();
                                string? name = parts[1].Trim();
                                string? ip = parts[2].Trim();
                                string? network = parts[3].Trim();

                                try
                                {
                                    Embeddeddevices embedded = new Embeddeddevices(id, name, ip, network);
                                    Devices.Add(embedded);
                                }
                                catch (ArgumentException ex)
                                {
                                    Console.WriteLine($"KURBA EROR OF CREATING DEVICE '{line}': {ex.Message}");
                                }
                            }
                            break;

                        default:
                            Console.WriteLine($"V dushi ne chalu sho za device: {deviceType}");
                            break;
                    }
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"KURBA EROR WITH READING LINE -> '{line}': {ex.Message}");
                }
            }

            Console.WriteLine($"ADDED {Devices.Count} devices");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"KURBA NOT GOOD FILE: {ex.Message}");
        }
    }

    public void SaveToFile()
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(_filePath))
            {
                foreach (Device device in Devices)
                {
                    if (device is SmartWatches sw)
                    {
                        writer.WriteLine($"{sw.Id},Apple Watch SE2,{(sw.IsDeviceTurned ? "True" : "False")},{sw.batteryPercent}%");
                    }
                    else if (device is PersonalComputer pc)
                    {
                        string os = pc.OperatingSystem ?? "null";
                        writer.WriteLine($"{pc.Id},{pc.Name},{(pc.IsDeviceTurned ? "True" : "False")},{os}");
                    }
                    else if (device is Embeddeddevices ed)
                    {
                        writer.WriteLine($"{ed.Id},{ed.Name},{ed.IpAddress},{ed.NetworkName}");
                    }
                }
            }

            Console.WriteLine($"SAVED {Devices.Count} DEVICES TO KURBA -> {_filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"KURBA CAN NOT TO SAVE: {ex.Message}");
        }
    }

    public void AddShit(Device device)
    {
        if (Devices.Count >= MaxNumberOfDevice)
        {
            Console.WriteLine("BUY 100 DOLLARS FOR MORE DEVICE SPACE");
            return;
        }

        if (Devices.Exists(d => d.Id == device.Id))
        {
            Console.WriteLine("That is ilegal to add this device");
            return;
        }
        
        Devices.Add(device);
        Console.WriteLine($"ADD NEW DEVICE {device.Name}");
    }

    public void RemoveShit(string? deviceId)
    {
        Device device = null;

        foreach (Device d in Devices)
        {
            if (d.Id == deviceId)
            {
                device = d;
                break;
            }
            
        }

        if (device != null)
        {
            Devices.Remove(device);
            Console.WriteLine($"REMOVE DEVICE {device.Name}");
        }
        else
        {
            Console.WriteLine("DEVICE NOT FOUND");
        }
    }


    public void EditShit(string? deviceId, object newDevice)
    {
        int index = -1;
        for (int i = 0; i < Devices.Count; i++)
        {
            if (Devices[i].Id == deviceId)
            {
                index = i;
                break;
            }
            
        }
        if (index != -1 && newDevice is Device)
        {
            Devices[index] = (Device)newDevice;
            Console.WriteLine($"UPDATE DEVICE {deviceId}");
        }
        else
        {
            Console.WriteLine("FAILED TO EDIT DEVICE");            
        }
    }

    public Device FindShit(string? id)
    {
        foreach (Device device in Devices)
        {
            if (device.Id == id)
            {
                return device;
            }
        }
        Console.WriteLine("DEVICE NOT FOUND");
        return null;
    }

    public void ShowAllShits()
    {
        if (Devices.Count == 0)
        {
            Console.WriteLine("BRO THERE ARE NO DEVICES");
            return;
        }
        Console.WriteLine("DEVICES");
        for (int i = 0; i < Devices.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {Devices[i].ToString()}");
        }
    }

   public void TurnOnShit(string? id)
{
    Device device = FindShit(id);

    if (device != null)
    {

        if (device.IsDeviceTurned)
        {
            Console.WriteLine("KYDA, I TAK TURNED ON");
            return;
        }
        try
        {
            if (device is SmartWatches smartWatch)
            {
                if (smartWatch.batteryPercent < 11)
                {
                    Console.WriteLine($"KURBA CANNOT TURN ON {smartWatch.Name} - BATTERY TOO LOW: {smartWatch.batteryPercent}%");
                    return;
                }
                
                smartWatch.TurnedOn();
                Console.WriteLine($"SMARTWATCH {smartWatch.Name} TURNED ON");
            }
            else if (device is PersonalComputer pc)
            {
                if (string.IsNullOrEmpty(pc.OperatingSystem))
                {
                    Console.WriteLine($"KURBA CANNOT TURN ON {pc.Name} - NO OPERATING SYSTEM INSTALLED");
                    return;
                }
                
                pc.TurnedOn();
                Console.WriteLine($"COMPUTER {pc.Name} TURNED ON WITH OS {pc.OperatingSystem}");
            }
            else if (device is Embeddeddevices embedded)
            {
                try
                {
                    embedded.TurnedOn(); 
                    Console.WriteLine($"EMBEDDED DEVICE {embedded.Name} TURNED ON AND CONNECTED TO {embedded.NetworkName}");
                }
                catch (ConnectionException cex)
                {
                    Console.WriteLine($"KURBA CANNOT CONNECT {embedded.Name}: {cex.Message}");
                }
            }
            else
            {
                device.TurnedOn();
                Console.WriteLine($"DEVICE {device.Name} TURNED ON");
            }
        }
        catch (EmptyBatteryException ex)
        {
            Console.WriteLine($"KURBA BATTERY EMPTY: {ex.Message}");
        }
        catch (EmptySystemException ex)
        {
            Console.WriteLine($"KURBA SYSTEM ERROR: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"KURBA CAN NOT TURN ON DEVICE: {ex.Message}");
        }
    }
}

    public void TurnOffShit(string? id)
    {
        Device device = FindShit(id);

        if (device != null)
        {
            device.TurnedOff();
            Console.WriteLine($"DEVICE {device.Name} TURNED OFF");
        }
    }

    public int GetDeviceCount()
    {
        return Devices.Count;
    }

    public Device GetDeviceById(string? id)
    {
        return FindShit(id);
    }
    
}