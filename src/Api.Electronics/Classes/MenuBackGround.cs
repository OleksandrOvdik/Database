using Models;
using Models.Classes;

namespace Electronics.Classes;

public class MenuBackGround
{
    public static void AddingMenu(DeviceManager manager)
    {
        Console.WriteLine("\n=== ADDING ===");
        Console.WriteLine("1. Smartwatch");
        Console.WriteLine("2. Personal Computer");
        Console.WriteLine("3. Embedded Device");
        Console.WriteLine("0. Back");
        
        Console.Write("Your choice: ");
        string? choice = Console.ReadLine();
        
        switch(choice)
        {
            case "1":
                AddSw(manager);
                break;
                
            case "2":
                AddPc(manager);
                break;
                
            case "3":
                AddEd(manager);
                break;
                
            case "0":
                return;
            
            default:
                Console.WriteLine("CHOOSE SMTH FROM THE LIST");
                break;
        }
    }


    private static void AddSw(DeviceManager manager)
    {
        try
        {
            Console.WriteLine("\n=== Adding Smartwatch ===");
            
            Console.Write($"Enter ID, (start with SW): ");
            string? id = Console.ReadLine();
            
            Console.Write("Enter name: ");
            string? name = Console.ReadLine();
            
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
    
    static void AddPc(DeviceManager manager)
    {
        try
        {
            Console.WriteLine("\n=== Adding Personal Computer ===");
            
            Console.Write("Enter ID (start with P): ");
            string? id = Console.ReadLine();
            
            Console.Write("Enter name: ");
            string? name = Console.ReadLine();
            
            Console.Write("Enter operating system (or empty string): ");
            string? os = Console.ReadLine();
            
            PersonalComputer pc = new PersonalComputer(id, name, string.IsNullOrEmpty(os) ? null : os);
            manager.AddShit(pc);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"KURBA ERRORE: {ex.Message}");
        }
    }

    private static void AddEd(DeviceManager manager)
    {
        try
        {
            Console.WriteLine("\n=== Adding Embedded Device ===");
            
            Console.Write("Enter ID (start with ED): ");
            string? id = Console.ReadLine();
            
            Console.Write("Enter name: ");
            string? name = Console.ReadLine();
            
            Console.Write("Enter IP address: ");
            string? ip = Console.ReadLine();
            
            Console.Write("Enter network name (should contain 'MD Ltd.', but can be different): ");
            string? network = Console.ReadLine();
            
            Embeddeddevices device = new Embeddeddevices(id, name, ip, network);
            manager.AddShit(device);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"KURBA ERRORE: {ex.Message}");
        }
    }


    public static void RemovingDevice(DeviceManager manager)
    {
        Console.WriteLine("\n=== Remove Device ===");
        
        manager.ShowAllShits();
        
        Console.Write("Enter ID of device to remove: ");
        string? id = Console.ReadLine();
        
        manager.RemoveShit(id);
    }


    public static void TurningOn(DeviceManager manager)
    {
        Console.WriteLine("\n=== Turn On Device ===");
        
        manager.ShowAllShits();
        
        Console.Write("Enter ID of device to turn on: ");
        string? id = Console.ReadLine();
        
        manager.TurnOnShit(id);
    }

    public static void TurningOff(DeviceManager manager)
    {
        Console.WriteLine("\n=== Turn Off Device ===");
        
        manager.ShowAllShits();
        
        Console.Write("Enter ID of device to turn off: ");
        string? id = Console.ReadLine();
        
        manager.TurnOffShit(id);
    }


    public static void EditMenu(DeviceManager manager)
    {
        Console.WriteLine("\n=== EDITING DEVICE ===");
    
        manager.ShowAllShits();
    
        Console.Write("\nENTER DEVICE ID TO EDIT: ");
        string? deviceId = Console.ReadLine();
    
        Device deviceToEdit = manager.FindShit(deviceId);

        if (deviceToEdit is SmartWatches smartWatch)
        {
            EditingSw(manager, smartWatch);
        }
        else if (deviceToEdit is PersonalComputer pc)
        {
            EditingPc(manager, pc);
        }
        else if (deviceToEdit is Embeddeddevices embeddedDevice)
        {
            EditingEd(manager, embeddedDevice);
        }
        else
        {
            Console.WriteLine("UNKNOWN DEVICE TYPE. CANNOT EDIT");
        }
    }


    private static void EditingSw(DeviceManager manager, SmartWatches oldWatch)
    {
        Console.WriteLine($"EDITING SMARTWATCH: {oldWatch.Name}");
    
        Console.Write("ENTER NEW NAME (EMPTY STRING WILL PUT CURRENT): ");
        string? newName = Console.ReadLine();
    
        if (string.IsNullOrEmpty(newName))
        {
            newName = oldWatch.Name;
        }
    
        Console.Write("ENTER NEW BATTERY PERCENTAGE (EMPTY STRING WILL PUT CURRENT): ");
        string? batteryInput = Console.ReadLine();
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
    
        SmartWatches newWatch = new SmartWatches(oldWatch.Id, newName, newBattery);
    
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
    
        manager.EditShit(oldWatch.Id, newWatch);
    }

    private static void EditingPc(DeviceManager manager, PersonalComputer oldPc)
    {
        Console.WriteLine($"EDITING COMPUTER: {oldPc.Name}");
    
        Console.Write("ENTER NEW NAME (EMPTY STRING WILL PUT CURRENT): ");
        string? newName = Console.ReadLine();
    
        if (string.IsNullOrEmpty(newName))
        {
            newName = oldPc.Name;
        }
    
        Console.Write("ENTER NEW OS (EMPTY STRING WILL PUT CURRENT, 'null' FOR NO OS): ");
        string? osInput = Console.ReadLine();
        string? newOs = oldPc.OperatingSystem;
    
        if (!string.IsNullOrEmpty(osInput))
        {
            if (osInput.ToLower() == "null")
            {
                newOs = null;
            }
            else
            {
                newOs = osInput;
            }
        }
    
        PersonalComputer newPc = new PersonalComputer(oldPc.Id, newName, newOs);
    
        if (oldPc.IsDeviceTurned && !string.IsNullOrEmpty(newOs))
        {
            try
            {
                newPc.TurnedOn();
            }
            catch (EmptySystemException)
            {
                Console.WriteLine("KURBA CANNOT TURN ON WITHOUT OS");
            }
        }
    
        manager.EditShit(oldPc.Id, newPc);
    }


    private static void EditingEd(DeviceManager manager, Embeddeddevices oldDevice)
    {
        Console.WriteLine($"EDITING EMBEDDED DEVICE: {oldDevice.Name}");
    
        Console.Write("ENTER NEW NAME (EMPTY STRING WILL PUT CURRENT): ");
        string? newName = Console.ReadLine();
    
        if (string.IsNullOrEmpty(newName))
        {
            newName = oldDevice.Name;
        }
    
        Console.Write("ENTER NEW IP ADDRESS (EMPTY STRING WILL PUT CURRENT): ");
        string? ipInput = Console.ReadLine();
        string? newIp = oldDevice.IpAddress;
    
        if (!string.IsNullOrEmpty(ipInput))
        {
            newIp = ipInput;
        }
    
        Console.Write("ENTER NEW NETWORK NAME (EMPTY STRING WILL PUT CURRENT): ");
        string? networkInput = Console.ReadLine();
        string? newNetwork = oldDevice.NetworkName;
    
        if (!string.IsNullOrEmpty(networkInput))
        {
            newNetwork = networkInput;
        }
    
        try
        {
            Embeddeddevices newDevice = new Embeddeddevices(oldDevice.Id, newName, newIp, newNetwork);
        
            manager.EditShit(oldDevice.Id, newDevice);
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"KURBA INVALID DATA: {ex.Message}");
        }
    }
}