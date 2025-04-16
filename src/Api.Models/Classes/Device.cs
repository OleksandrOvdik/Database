namespace Models;
    
public class Device
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public bool IsDeviceTurned { get; set; }

    protected Device(string? id, string? name)
    {
        Id = id;
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