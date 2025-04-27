using System.ComponentModel;
using Microsoft.Data.SqlClient;
using Models;
using Models.Classes;
using System.Text.Json;
using System.Text.Json.Nodes;


namespace Electronics;

public class DeviceService
{
    private string _connectionString;

    public DeviceService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IEnumerable<DeviceDto> GetAllModels()
    {
        var devices = new List<DeviceDto>();
        const string queryString =
            "select * from Devices";


        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            SqlCommand command = new SqlCommand(queryString, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var deviceRow = new DeviceDto()
                        {
                            Id = reader["Id"].ToString(),
                            Name = reader["Name"].ToString(),
                            IsDeviceTurned = (bool)reader["IsDeviceTurned"],
                        };
                        devices.Add(deviceRow);
                    }
                }
            }
            finally
            {
                reader.Close();
            }
            return devices;
        }
    }
    
    public Device GetDeviceById(string? id)
    {
        Device device = null; 
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            try
            {
                if (id.Contains("SW"))
                {
                    const string swString = "select * from Devices join SmartWatches on SmartWatches.DeviceId = DeviceId where Id = @id";
                    SqlCommand commandSw = new SqlCommand(swString, connection);
                    commandSw.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    SqlDataReader readerSw = commandSw.ExecuteReader();

                            if (readerSw.HasRows)
                            {
                                while (readerSw.Read())
                                {
                                    SmartWatches swInfo = new SmartWatches
                                    {
                                        Id = readerSw.GetString(0),
                                        Name = readerSw["Name"].ToString(),
                                        IsDeviceTurned = (bool)readerSw["IsDeviceTurned"],
                                        batteryPercent = Convert.ToInt32(readerSw["batteryPercent"])
                                
                                    };
                                    device = (Device)swInfo;
                                }
                            }
                            readerSw.Close();
                }
                        

                if (id.Contains("PC"))
                {
                    const string pcString = "select * from Devices join PersonalComputers on PersonalComputers.DeviceId = DeviceId where Id = @id"; 
                    SqlCommand commandPc = new SqlCommand(pcString, connection);
                    commandPc.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    SqlDataReader readerPc = commandPc.ExecuteReader();

                    if (readerPc.HasRows)
                    {
                        while (readerPc.Read())
                        {
                            PersonalComputer pcInfo = new PersonalComputer
                            {
                                Id = readerPc.GetString(0),
                                Name = readerPc["Name"].ToString(),
                                IsDeviceTurned = (bool)readerPc["IsDeviceTurned"],
                                OperatingSystem = readerPc["OperatingSystem"].ToString(),                                
                            };
                            device = (Device)pcInfo;
                        }
                    }
                    readerPc.Close();
                }
                if (id.Contains("ED"))
                {
                    const string edString = "select * from Devices join EmbeddedDevices on EmbeddedDevices.DeviceId = DeviceId where Id = @id "; 
                    SqlCommand commandEd = new SqlCommand(edString, connection);
                    commandEd.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    SqlDataReader readerEd = commandEd.ExecuteReader();

                    if (readerEd.HasRows)
                    {
                        while (readerEd.Read())
                        {
                            EmbeddedDevices edInfo = new EmbeddedDevices
                            {
                                Id = readerEd.GetString(0),
                                Name = readerEd["Name"].ToString(),
                                IsDeviceTurned = (bool)readerEd["IsDeviceTurned"],
                                IpAddress = readerEd["IpAddress"].ToString(),
                                NetworkName = readerEd["NetworkName"].ToString(),
                            };
                            device = (Device)edInfo;
                        }
                    }
                    readerEd.Close();
                }
            }
            finally
            {
                connection.Close();
            }
        }
        return device;
    }
    
    
     public string CreateDevice(string jsonDeviceSpecificData)
    {
        var json = JsonNode.Parse(jsonDeviceSpecificData);
        
        var deviceType = json["deviceType"]?.ToString();
        var name = json["name"]?.ToString();
        var isTurnedOn = json["isTurnedOn"]?.ToString();


        string newId = $"{deviceType.ToUpper()}-{Guid.NewGuid().ToString().Substring(0, 8)}";

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            

            switch (deviceType.ToUpper())
            {
                case "SW":
                    var watchData = JsonSerializer.Deserialize<SmartWatches>(jsonDeviceSpecificData);
                    var insertSw = @"INSERT INTO Devices (Id, Name, IsDeviceTurned, Type) 
                                 VALUES (@Id, @Name, @IsDeviceTurned, @Type)";
                    using (var command = new SqlCommand(insertSw, connection))
                    {
                        command.Parameters.AddWithValue("@Id", newId);
                        command.Parameters.AddWithValue("@Name", name);
                        command.Parameters.AddWithValue("@IsDeviceTurned", isTurnedOn);
                        command.Parameters.AddWithValue("@Type", deviceType.ToUpper());
                        command.ExecuteNonQuery();
                    }
                    var insertWatch = @"INSERT INTO SmartWatches (DeviceId, BatteryPercent) VALUES (@Id, @Battery)";
                    using (var command = new SqlCommand(insertWatch, connection))
                    {
                        command.Parameters.AddWithValue("@Id", newId);
                        command.Parameters.AddWithValue("@Battery", watchData.batteryPercent);
                        command.ExecuteNonQuery();
                    }
                    break;

                case "PS":
                    var pcData = JsonSerializer.Deserialize<PersonalComputer>(jsonDeviceSpecificData);
                    
                    var insertComputer = @"INSERT INTO Devices (Id, Name, IsDeviceTurned, Type) 
                                 VALUES (@Id, @Name, @IsDeviceTurned, @Type)";
                    using (var command = new SqlCommand(insertComputer, connection))
                    {
                        command.Parameters.AddWithValue("@Id", newId);
                        command.Parameters.AddWithValue("@Name", name);
                        command.Parameters.AddWithValue("@IsDeviceTurned", isTurnedOn);
                        command.Parameters.AddWithValue("@Type", deviceType.ToUpper());
                        command.ExecuteNonQuery();
                    }
                    
                    var insertPc = @"INSERT INTO PersonalComputers (DeviceId, OperatingSystem) VALUES (@Id, @OS)";
                    using (var command = new SqlCommand(insertPc, connection))
                    {
                        command.Parameters.AddWithValue("@Id", newId);
                        command.Parameters.AddWithValue("@OS", pcData.OperatingSystem);
                        command.ExecuteNonQuery();
                    }
                    break;

                case "ED":
                    var embeddedData = JsonSerializer.Deserialize<EmbeddedDevices>(jsonDeviceSpecificData);
                    
                    var insertED = @"INSERT INTO Devices (Id, Name, IsDeviceTurned, Type) 
                                 VALUES (@Id, @Name, @IsDeviceTurned, @Type)";
                    using (var command = new SqlCommand(insertED, connection))
                    {
                        command.Parameters.AddWithValue("@Id", newId);
                        command.Parameters.AddWithValue("@Name", name);
                        command.Parameters.AddWithValue("@IsDeviceTurned", isTurnedOn);
                        command.Parameters.AddWithValue("@Type", deviceType.ToUpper());
                        command.ExecuteNonQuery();
                    }
                    
                    var insertEd = @"INSERT INTO EmbeddedDevices (DeviceId, IpAddress, NetworkName) VALUES (@Id, @Ip, @Network)";
                    using (var command = new SqlCommand(insertEd, connection))
                    {
                        command.Parameters.AddWithValue("@Id", newId);
                        command.Parameters.AddWithValue("@Ip", embeddedData.IpAddress);
                        command.Parameters.AddWithValue("@Network", embeddedData.NetworkName);
                        command.ExecuteNonQuery();
                    }
                    break;

                default:
                    throw new ArgumentException("Invalid device type");
            }

            return newId;
        }
    }
     
      public void UpdateDevice(string jsonDeviceSpecificData)
    {
        var json = JsonNode.Parse(jsonDeviceSpecificData);
        
        var deviceType = json["deviceType"]?.ToString();
        var name = json["name"]?.ToString();
        var isTurnedOn = json["isTurnedOn"]?.ToString();
        var deviceId = json["deviceId"]?.ToString();
        
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            var updateDevice = @"UPDATE Devices SET Name = @Name, IsDeviceTurned = @IsDeviceTurned WHERE Id = @Id";
            using (var command = new SqlCommand(updateDevice, connection))
            {
                command.Parameters.AddWithValue("@Id", deviceId);
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@IsDeviceTurned", isTurnedOn);
                command.ExecuteNonQuery();
            }
            switch (deviceType.ToUpper())
            {
                case "SW":
                    var watchData = JsonSerializer.Deserialize<SmartWatches>(jsonDeviceSpecificData);
                    var updateSw = @"UPDATE SmartWatches SET BatteryPercent = @Battery WHERE DeviceId = @Id";
                    using (var nigger = new SqlCommand(updateSw, connection))
                    {
                        nigger.Parameters.AddWithValue("@Id", deviceId);
                        nigger.Parameters.AddWithValue("@Battery", watchData.batteryPercent);
                        nigger.ExecuteNonQuery();
                    }
                    break;
                case "PS":
                    var pcData = JsonSerializer.Deserialize<PersonalComputer>(jsonDeviceSpecificData);
                    var updatePc = @"UPDATE PersonalComputers SET OperatingSystem = @OS WHERE DeviceId = @Id";
                    using (var command = new SqlCommand(updatePc, connection))
                    {
                        command.Parameters.AddWithValue("@Id", deviceId);
                        command.Parameters.AddWithValue("@OS", pcData.OperatingSystem);
                        command.ExecuteNonQuery();
                    }
                    break;
                case "ED":
                    var embeddedData = JsonSerializer.Deserialize<EmbeddedDevices>(jsonDeviceSpecificData);
                    var updateEd = @"UPDATE EmbeddedDevices SET IpAddress = @Ip, NetworkName = @Network WHERE DeviceId = @Id";
                    using (var command = new SqlCommand(updateEd, connection))
                    {
                        command.Parameters.AddWithValue("@Id", deviceId);
                        command.Parameters.AddWithValue("@Ip", embeddedData.IpAddress);
                        command.Parameters.AddWithValue("@Network", embeddedData.NetworkName);
                        command.ExecuteNonQuery();
                    }
                    break;
                default:
                    throw new ArgumentException("Invalid device type");
            }
        }
    }
      
    public void DeleteDevice(string deviceId)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            var deleteDevice = @"DELETE FROM Devices WHERE Id = @Id";
            using (var command = new SqlCommand(deleteDevice, connection))
            {
                command.Parameters.AddWithValue("@Id", deviceId);
                command.ExecuteNonQuery();
            }
        }
    }

    /*public string CreateSmartWatch(string name, bool isTurnedOn, string jsonDeviceSpecificData)
    {
        string newId = $"SW-{Guid.NewGuid().ToString().Substring(0, 8)}";

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            var data = JsonConvert.DeserializeObject<SmartWatches>(jsonDeviceSpecificData);

            var insertDevice = @"INSERT INTO Devices (Id, Name, IsDeviceTurned, Type) VALUES (@Id, @Name, @IsDeviceTurned, 'SW')";
            using (var command = new SqlCommand(insertDevice, connection))
            {
                command.Parameters.AddWithValue("@Id", newId);
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@IsDeviceTurned", isTurnedOn);
                command.ExecuteNonQuery();
            }

            var insertSw = @"INSERT INTO SmartWatches (DeviceId, BatteryPercent) VALUES (@Id, @Battery)";
            using (var command = new SqlCommand(insertSw, connection))
            {
                command.Parameters.AddWithValue("@Id", newId);
                command.Parameters.AddWithValue("@Battery", data.batteryPercent);
                command.ExecuteNonQuery();
            }

            return newId;
        }
    }

    public string CreatePersonalComputer(string name, bool isTurnedOn, string jsonDeviceSpecificData)
    {
        string newId = $"PS-{Guid.NewGuid().ToString().Substring(0, 8)}";

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            var data = JsonConvert.DeserializeObject<PersonalComputer>(jsonDeviceSpecificData);

            var insertDevice = @"INSERT INTO Devices (Id, Name, IsDeviceTurned, Type) VALUES (@Id, @Name, @IsDeviceTurned, 'PS')";
            using (var command = new SqlCommand(insertDevice, connection))
            {
                command.Parameters.AddWithValue("@Id", newId);
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@IsDeviceTurned", isTurnedOn);
                command.ExecuteNonQuery();
            }

            var insertPc = @"INSERT INTO PersonalComputers (DeviceId, OperatingSystem) VALUES (@Id, @OS)";
            using (var command = new SqlCommand(insertPc, connection))
            {
                command.Parameters.AddWithValue("@Id", newId);
                command.Parameters.AddWithValue("@OS", data.OperatingSystem);
                command.ExecuteNonQuery();
            }

            return newId;
        }
    }

    public string CreateEmbeddedDevice(string name, bool isTurnedOn, string jsonDeviceSpecificData)
    {
        string newId = $"ED-{Guid.NewGuid().ToString().Substring(0, 8)}";

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            var data = JsonConvert.DeserializeObject<EmbeddedDevices>(jsonDeviceSpecificData);

            var insertDevice = @"INSERT INTO Devices (Id, Name, IsDeviceTurned, Type) VALUES (@Id, @Name, @IsDeviceTurned, 'ED')";
            using (var command = new SqlCommand(insertDevice, connection))
            {
                command.Parameters.AddWithValue("@Id", newId);
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@IsDeviceTurned", isTurnedOn);
                command.ExecuteNonQuery();
            }

            var insertEd = @"INSERT INTO EmbeddedDevices (DeviceId, IpAddress, NetworkName) VALUES (@Id, @Ip, @Network)";
            using (var command = new SqlCommand(insertEd, connection))
            {
                command.Parameters.AddWithValue("@Id", newId);
                command.Parameters.AddWithValue("@Ip", data.IpAddress);
                command.Parameters.AddWithValue("@Network", data.NetworkName);
                command.ExecuteNonQuery();
            }

            return newId;
        }
    }*/

}