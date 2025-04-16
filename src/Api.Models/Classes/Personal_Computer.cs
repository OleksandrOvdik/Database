namespace Models;
// Personal_Computer -> P
public class Personal_Computer : Device
    
{
    public  string OperatingSystem { get; set; }

    public Personal_Computer(string id, string name, string operatingSystem = null) : base(id, name)
    {
        OperatingSystem = operatingSystem;
    }


    public override void TurnedOn()
    {
        if (string.IsNullOrEmpty(OperatingSystem))
        {
            throw new EmptySystemException();
        }
        
        base.TurnedOn();
        Console.WriteLine($"    LOG -> Computer: {Name}, Operating System: {OperatingSystem}, ACTIVATED");
    }

    public override string ToString()
    {
        string info = string.IsNullOrEmpty(OperatingSystem) ? "OS unknown" : $"OS: {OperatingSystem}";
        return $"Computer: {Name}, ID-> {ID}, {info}, Mode: {(IsDeviceTurned ? "TurnedOn" : "TurnedOff")}";
    }
}