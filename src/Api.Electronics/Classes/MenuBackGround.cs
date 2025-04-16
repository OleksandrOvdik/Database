using Models;

namespace Electronics;

public class MenuBackGround
{
    public static void AddingMenu(Device_Manager manager)
    {
        Console.WriteLine("\n=== ADDING ===");
        Console.WriteLine("1. Smartwatch");
        Console.WriteLine("2. Personal Computer");
        Console.WriteLine("3. Embedded Device");
        Console.WriteLine("0. Back");
        
        Console.Write("Your choice: ");
        string choice = Console.ReadLine();
        
        switch(choice)
        {
            case "1":
                AddSW(manager);
                break;
                
            case "2":
                AddPC(manager);
                break;
                
            case "3":
                AddED(manager);
                break;
                
            case "0":
                return;
            
            default:
                Console.WriteLine("CHOOSE SMTH FROM THE LIST");
                break;
        }
    }
    
    
    static void AddSW(Device_Manager manager)
    {
        try
        {
            Console.WriteLine("\n=== Adding Smartwatch ===");
            
            Console.Write($"Enter ID, (start with SW): ");
            string id = Console.ReadLine();
            
            Console.Write("Enter name: ");
            string name = Console.ReadLine();
            
            Console.Write("Enter battery percentage (0-100): ");
            int battery = int.Parse(Console.ReadLine());
            
            SmartWatches watch = new SmartWatches(id, name, battery);
            manager.AddShit(watch);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"KURBA ERRORE: {ex.Message}");
        }
    }
    
    static void AddPC(Device_Manager manager)
    {
        try
        {
            Console.WriteLine("\n=== Adding Personal Computer ===");
            
            Console.Write("Enter ID (start with P): ");
            string id = Console.ReadLine();
            
            Console.Write("Enter name: ");
            string name = Console.ReadLine();
            
            Console.Write("Enter operating system (or empty string): ");
            string os = Console.ReadLine();
            
            Personal_Computer pc = new Personal_Computer(id, name, string.IsNullOrEmpty(os) ? null : os);
            manager.AddShit(pc);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"KURBA ERRORE: {ex.Message}");
        }
    }
    
    static void AddED(Device_Manager manager)
    {
        try
        {
            Console.WriteLine("\n=== Adding Embedded Device ===");
            
            Console.Write("Enter ID (start with ED): ");
            string id = Console.ReadLine();
            
            Console.Write("Enter name: ");
            string name = Console.ReadLine();
            
            Console.Write("Enter IP address: ");
            string ip = Console.ReadLine();
            
            Console.Write("Enter network name (should contain 'MD Ltd.', but can be different): ");
            string network = Console.ReadLine();
            
            Embedded_devices device = new Embedded_devices(id, name, ip, network);
            manager.AddShit(device);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"KURBA ERRORE: {ex.Message}");
        }
    }


    public static void RemovingDevice(Device_Manager manager)
    {
        Console.WriteLine("\n=== Remove Device ===");
        
        manager.ShowAllShits();
        
        Console.Write("Enter ID of device to remove: ");
        string id = Console.ReadLine();
        
        manager.RemoveShit(id);
    }


    public static void TurningOn(Device_Manager manager)
    {
        Console.WriteLine("\n=== Turn On Device ===");
        
        manager.ShowAllShits();
        
        Console.Write("Enter ID of device to turn on: ");
        string id = Console.ReadLine();
        
        manager.TurnOnShit(id);
    }

    public static void TurningOff(Device_Manager manager)
    {
        Console.WriteLine("\n=== Turn Off Device ===");
        
        manager.ShowAllShits();
        
        Console.Write("Enter ID of device to turn off: ");
        string id = Console.ReadLine();
        
        manager.TurnOffShit(id);
    }


    public static void EditMenu(Device_Manager manager)
    {
        Console.WriteLine("\n=== EDITING DEVICE ===");
    
        manager.ShowAllShits();
    
        Console.Write("\nENTER DEVICE ID TO EDIT: ");
        string deviceId = Console.ReadLine();
    
        Device deviceToEdit = manager.FindShit(deviceId);
    
        if (deviceToEdit == null)
        {
            Console.WriteLine("DEVICE NOT FOUND");
            return;
        }
    
        if (deviceToEdit is SmartWatches smartWatch)
        {
            EditingSW(manager, smartWatch);
        }
        else if (deviceToEdit is Personal_Computer pc)
        {
            EditingPC(manager, pc);
        }
        else if (deviceToEdit is Embedded_devices embeddedDevice)
        {
            EditingED(manager, embeddedDevice);
        }
        else
        {
            Console.WriteLine("UNKNOWN DEVICE TYPE. CANNOT EDIT");
        }
    }
    
    
    static void EditingSW(Device_Manager manager, SmartWatches oldWatch)
    {
        Console.WriteLine($"EDITING SMARTWATCH: {oldWatch.Name}");
    
        Console.Write("ENTER NEW NAME (EMPTY STRING WILL PUT CURRENT): ");
        string newName = Console.ReadLine();
    
        if (string.IsNullOrEmpty(newName))
        {
            newName = oldWatch.Name;
        }
    
        Console.Write("ENTER NEW BATTERY PERCENTAGE (EMPTY STRING WILL PUT CURRENT): ");
        string batteryInput = Console.ReadLine();
        int newBattery = oldWatch.batteryPercent;
    
        if (!string.IsNullOrEmpty(batteryInput) && int.TryParse(batteryInput, out int parsedBattery))
        {
            if (parsedBattery >= 0 && parsedBattery <= 100)
            {
                newBattery = parsedBattery;
            }
            else
            {
                Console.WriteLine("KURBA INVALID BATTERY VALUE (0-100). KEEPING OLD VALUE");
            }
        }
    
        SmartWatches newWatch = new SmartWatches(oldWatch.ID, newName, newBattery);
    
        if (oldWatch.IsDeviceTurned)
        {
            try
            {
                newWatch.TurnedOn();
            }
            catch (EmptyBatteryException)
            {
                Console.WriteLine("KURBA CANNOT TURN ON WITH LOW BATTERY");
            }
        }
    
        manager.EditShit(oldWatch.ID, newWatch);
    }
    
    static void EditingPC(Device_Manager manager, Personal_Computer oldPC)
    {
        Console.WriteLine($"EDITING COMPUTER: {oldPC.Name}");
    
        Console.Write("ENTER NEW NAME (EMPTY STRING WILL PUT CURRENT): ");
        string newName = Console.ReadLine();
    
        if (string.IsNullOrEmpty(newName))
        {
            newName = oldPC.Name;
        }
    
        Console.Write("ENTER NEW OS (EMPTY STRING WILL PUT CURRENT, 'null' FOR NO OS): ");
        string osInput = Console.ReadLine();
        string newOS = oldPC.OperatingSystem;
    
        if (!string.IsNullOrEmpty(osInput))
        {
            if (osInput.ToLower() == "null")
            {
                newOS = null;
            }
            else
            {
                newOS = osInput;
            }
        }
    
        Personal_Computer newPC = new Personal_Computer(oldPC.ID, newName, newOS);
    
        if (oldPC.IsDeviceTurned && !string.IsNullOrEmpty(newOS))
        {
            try
            {
                newPC.TurnedOn();
            }
            catch (EmptySystemException)
            {
                Console.WriteLine("KURBA CANNOT TURN ON WITHOUT OS");
            }
        }
    
        manager.EditShit(oldPC.ID, newPC);
    }
    
    
    static void EditingED(Device_Manager manager, Embedded_devices oldDevice)
    {
        Console.WriteLine($"EDITING EMBEDDED DEVICE: {oldDevice.Name}");
    
        Console.Write("ENTER NEW NAME (EMPTY STRING WILL PUT CURRENT): ");
        string newName = Console.ReadLine();
    
        if (string.IsNullOrEmpty(newName))
        {
            newName = oldDevice.Name;
        }
    
        Console.Write("ENTER NEW IP ADDRESS (EMPTY STRING WILL PUT CURRENT): ");
        string ipInput = Console.ReadLine();
        string newIP = oldDevice.IPAddress;
    
        if (!string.IsNullOrEmpty(ipInput))
        {
            newIP = ipInput;
        }
    
        Console.Write("ENTER NEW NETWORK NAME (EMPTY STRING WILL PUT CURRENT): ");
        string networkInput = Console.ReadLine();
        string newNetwork = oldDevice.NetworkName;
    
        if (!string.IsNullOrEmpty(networkInput))
        {
            newNetwork = networkInput;
        }
    
        try
        {
            Embedded_devices newDevice = new Embedded_devices(oldDevice.ID, newName, newIP, newNetwork);
        
            manager.EditShit(oldDevice.ID, newDevice);
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"KURBA INVALID DATA: {ex.Message}");
        }
    }
}