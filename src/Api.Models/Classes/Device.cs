namespace Models;
    
public class Device
{
    public string ID { get; set; }
    public string Name { get; set; }
    public bool IsDeviceTurned { get; set; }

    public Device(string id, string name)
    {
        ID = id;
        Name = name;
        IsDeviceTurned = true;
    }

    public virtual void TurnedOn()
    {
        IsDeviceTurned = true;
    }

    public virtual void TurnedOff()
    {
        IsDeviceTurned = false;
    }
}


public class ConnectionException : Exception { }

public class EmptySystemException : Exception { }

public class EmptyBatteryException : Exception { }