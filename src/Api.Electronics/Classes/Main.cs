
namespace Electronics.Classes;

class MainClass
{
    static void Main(string[] args)
    {
        
        Console.WriteLine("Welcome to the Api.Electronics Device Managing System");

        DeviceManager deviceManager = new DeviceManager("input.txt");
        
        bool isRunning = true;

        while (isRunning)
        {
            Menu();
            
            Console.WriteLine("Select an option:");
            string? option = Console.ReadLine();
            
            switch (option)
            {
                case "1":
                    deviceManager.ShowAllShits();
                    break;
                case "2":
                    MenuBackGround.AddingMenu(deviceManager);
                    break;
                case "3":
                    MenuBackGround.RemovingDevice(deviceManager);
                    break;
                case "4":
                    MenuBackGround.EditMenu(deviceManager);
                    break;
                case "5":
                    MenuBackGround.TurningOn(deviceManager);
                    break;
                case "6":
                    MenuBackGround.TurningOff(deviceManager);
                    break;
                case "7":
                    deviceManager.SaveToFile();
                    break;
                case "0":
                    Console.WriteLine("Thanks GOD you are leaving!");
                    isRunning = false;
                    break;
                default:
                    Console.WriteLine("YOU NEED TO CHOOSE FROM 1 to 0, DO NOT TRY SMTH ELSE");
                    break;
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
    
    static void Menu()
    {
        Console.WriteLine("\n=== MENUSHKA ===");
        Console.WriteLine("1. Show all devices");
        Console.WriteLine("2. Add device");
        Console.WriteLine("3. Remove device");
        Console.WriteLine("4. Edit device"); 
        Console.WriteLine("5. Turn on device");
        Console.WriteLine("6. Turn off device");
        Console.WriteLine("7. Save to file");
        Console.WriteLine("0. Exit");
    }
}
