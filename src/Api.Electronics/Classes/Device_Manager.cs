using Models;

namespace Electronics;



public class Device_Manager
{
    public const int MaxNumberOfDevice = 15;
    
    public List<Device> Devices;
    
    public string FilePath;



    public Device_Manager(string filePath)
    {
        this.FilePath = filePath;
        Devices = new List<Device>();

        loadFromFile();

    }

    private void loadFromFile()
    {
        if (!File.Exists(FilePath))
        {
            Console.WriteLine("File does not exist");
        }

        try
        {
            string[] lines = File.ReadAllLines(FilePath);

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
                                string Id = parts[0].Trim();
                                string Name = parts[1].Trim();
                                bool isTurnedOn = bool.Parse(parts[2].Trim());
                                int battery = int.Parse(parts[3].TrimEnd('%'));

                                SmartWatches watch = new SmartWatches(Id, Name, battery);
                                if (isTurnedOn) watch.TurnedOn();
                                Devices.Add(watch);
                            }

                            break;
                        case "P":
                            if (parts.Length >= 3)
                            {
                                string id = parts[0].Trim();
                                string name = parts[1].Trim();
                                bool isTurnedOn = bool.Parse(parts[2].Trim());

                                string os = null;
                                if (parts.Length > 3)
                                {
                                    os = parts[3].Trim();
                                    if (os.ToLower() == "null")
                                        os = null;
                                }

                                Personal_Computer pc = new Personal_Computer(id, name, os);
                                if (isTurnedOn && !string.IsNullOrEmpty(os)) pc.TurnedOn();
                                Devices.Add(pc);
                            }

                            break;
                        case "ED":
                            if (parts.Length >= 4)
                            {
                                string id = parts[0].Trim();
                                string name = parts[1].Trim();
                                string ip = parts[2].Trim();
                                string network = parts[3].Trim();

                                try
                                {
                                    Embedded_devices embedded = new Embedded_devices(id, name, ip, network);
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
            using (StreamWriter writer = new StreamWriter(FilePath))
            {
                foreach (Device device in Devices)
                {
                    if (device is SmartWatches sw)
                    {
                        writer.WriteLine($"{sw.ID},Apple Watch SE2,{(sw.IsDeviceTurned ? "True" : "False")},{sw.batteryPercent}%");
                    }
                    else if (device is Personal_Computer pc)
                    {
                        string os = pc.OperatingSystem ?? "null";
                        writer.WriteLine($"{pc.ID},{pc.Name},{(pc.IsDeviceTurned ? "True" : "False")},{os}");
                    }
                    else if (device is Embedded_devices ed)
                    {
                        writer.WriteLine($"{ed.ID},{ed.Name},{ed.IPAddress},{ed.NetworkName}");
                    }
                }
            }

            Console.WriteLine($"SAVED {Devices.Count} DEVICES TO KURBA -> {FilePath}");
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
        
        Devices.Add(device);
        Console.WriteLine($"ADD NEW DEVICE {device.Name}");
    }

    public void RemoveShit(string deviceId)
    {
        Device device = null;

        foreach (Device d in Devices)
        {
            if (d.ID == deviceId)
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


    public void EditShit(string deviceId, object newDevice)
    {
        int Index = -1;
        for (int i = 0; i < Devices.Count; i++)
        {
            if (Devices[i].ID == deviceId)
            {
                Index = i;
                break;
            }
            
        }

        if (Index != -1 && newDevice is Device)
        {
            Devices[Index] = (Device)newDevice;
            Console.WriteLine($"UPDATE DEVICE {deviceId}");
        }
        else
        {
            Console.WriteLine("FAILED TO EDIT DEVICE");            
        }
    }

    public Device FindShit(string id)
    {
        foreach (Device device in Devices)
        {
            if (device.ID == id)
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

   public void TurnOnShit(string id)
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
            else if (device is Personal_Computer pc)
            {
                if (string.IsNullOrEmpty(pc.OperatingSystem))
                {
                    Console.WriteLine($"KURBA CANNOT TURN ON {pc.Name} - NO OPERATING SYSTEM INSTALLED");
                    return;
                }
                
                pc.TurnedOn();
                Console.WriteLine($"COMPUTER {pc.Name} TURNED ON WITH OS {pc.OperatingSystem}");
            }
            else if (device is Embedded_devices embedded)
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

    public void TurnOffShit(string id)
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

    public Device GetDeviceById(string id)
    {
        return FindShit(id);
    }
    
}