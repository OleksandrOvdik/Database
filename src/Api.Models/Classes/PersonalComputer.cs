namespace Models.Classes;
// Personal_Computer -> P
public class PersonalComputer : Device
{
    public  string? OperatingSystem { get; set; }

    public PersonalComputer(string? id, string? name, string? operatingSystem) : base(id, name)
    {
        OperatingSystem = operatingSystem;
    }

    public PersonalComputer(Device baseDevice) : base("", "")
    {
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
        return $"Computer: {Name}, ID-> {Id}, {info}, Mode: {(IsDeviceTurned ? "TurnedOn" : "TurnedOff")}";
    }
}