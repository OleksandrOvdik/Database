using System.ComponentModel;
using Microsoft.Data.SqlClient;
using Models;
using Models.Classes;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Electronics;

public class DeviceService : IDeviceService
{
    private readonly string _connectionString;
    
    public DeviceService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IEnumerable<DeviceDto> GetAllModels()
    {
        var devices = new List<DeviceDto>();
        const string query = "SELECT * FROM Device";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(query, connection);
        connection.Open();
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            devices.Add(new DeviceDto
            {
                Id = reader["Id"].ToString(),
                Name = reader["Name"].ToString(),
                IsDeviceTurned = (bool)reader["IsEnabled"]
            });
        }

        return devices;
    }
    

    public Device GetDeviceById(string id)
    {
        Device device = null;

        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        var deviceQuery = "SELECT * FROM Device WHERE Id = @Id";
        using var deviceCommand = new SqlCommand(deviceQuery, connection);
        deviceCommand.Parameters.AddWithValue("@Id", id);
        using var deviceReader = deviceCommand.ExecuteReader();

        if (!deviceReader.Read()) return null;
        
        var baseDevice = new Device
        {
            Id = deviceReader["Id"].ToString(),
            Name = deviceReader["Name"].ToString(),
            IsDeviceTurned = (bool)deviceReader["IsEnabled"]
        };
        deviceReader.Close();

        if (id.StartsWith("ED"))
        {
            var query = @"SELECT * FROM Embedded WHERE DeviceId = @Id";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            using var reader = command.ExecuteReader();
            
            if (reader.Read())
            {
                device = new EmbeddedDevices(baseDevice)
                {
                    IpAddress = reader["lpAddress"].ToString(),
                    NetworkName = reader["NetworkName"].ToString()
                };
            }
        }
        else if (id.StartsWith("PC"))
        {
            var query = @"SELECT * FROM PersonalComputer WHERE DeviceId = @Id";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            using var reader = command.ExecuteReader();
            
            if (reader.Read())
            {
                device = new PersonalComputer(baseDevice)
                {
                    OperatingSystem = reader["OperationSystem"].ToString()
                };
            }
        }
        else if (id.StartsWith("SW"))
        {
            var query = @"SELECT * FROM Smartwatch WHERE DeviceId = @Id";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            using var reader = command.ExecuteReader();
            
            if (reader.Read())
            {
                device = new SmartWatches(baseDevice)
                {
                    batteryPercent = Convert.ToInt32(reader["BatteryPercentage"])
                };
            }
        }

        return device;
    }

    public string CreateDevice(string jsonData)
    {
        var json = JsonNode.Parse(jsonData);
        var deviceType = json["deviceType"]?.ToString().ToUpper();
        var newId = $"{deviceType}-{Guid.NewGuid():N}";

        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        
        var insertDevice = @"INSERT INTO Device (Id, Name, IsEnabled) 
                          VALUES (@Id, @Name, @IsEnabled)";
        
        using var deviceCommand = new SqlCommand(insertDevice, connection);
        deviceCommand.Parameters.AddWithValue("@Id", newId);
        deviceCommand.Parameters.AddWithValue("@Name", json["name"]?.ToString());
        deviceCommand.Parameters.AddWithValue("@IsEnabled", Convert.ToBoolean(json["isEnabled"]));
        deviceCommand.ExecuteNonQuery();

        switch (deviceType)
        {
            case "ED":
            {
                var edQuery = @"INSERT INTO Embedded (lpAddress, NetworkName, DeviceId) 
                             VALUES (@Ip, @Network, @Id)";
                using var edCommand = new SqlCommand(edQuery, connection);
                edCommand.Parameters.AddWithValue("@Id", newId);
                edCommand.Parameters.AddWithValue("@Ip", json["ipAddress"]?.ToString());
                edCommand.Parameters.AddWithValue("@Network", json["networkName"]?.ToString());
                edCommand.ExecuteNonQuery();
                break;
            }

            case "PC":
            {
                var pcQuery = @"INSERT INTO PersonalComputer (OperationSystem, DeviceId) 
                              VALUES (@OS, @Id)";
                using var pcCommand = new SqlCommand(pcQuery, connection);
                pcCommand.Parameters.AddWithValue("@Id", newId);
                pcCommand.Parameters.AddWithValue("@OS", json["operationSystem"]?.ToString());
                pcCommand.ExecuteNonQuery();
                break;
            }

            case "SW":
            {
                var swQuery = @"INSERT INTO Smartwatch (BatteryPercentage, DeviceId) 
                              VALUES (@Battery, @Id)";
                using var swCommand = new SqlCommand(swQuery, connection);
                swCommand.Parameters.AddWithValue("@Id", newId);
                swCommand.Parameters.AddWithValue("@Battery", Convert.ToInt32(json["batteryPercentage"]));
                swCommand.ExecuteNonQuery();
                break;
            }

            default:
                throw new ArgumentException("Invalid device type");
        }

        return newId;
    }

    public void UpdateDevice(string jsonData)
    {
        var json = JsonNode.Parse(jsonData);
        var deviceId = json["id"]?.ToString();

        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        var updateDevice = @"UPDATE Device SET 
                           Name = @Name, 
                           IsEnabled = @IsEnabled 
                           WHERE Id = @Id";
        
        using var deviceCommand = new SqlCommand(updateDevice, connection);
        deviceCommand.Parameters.AddWithValue("@Id", deviceId);
        deviceCommand.Parameters.AddWithValue("@Name", json["name"]?.ToString());
        deviceCommand.Parameters.AddWithValue("@IsEnabled", Convert.ToBoolean(json["isEnabled"]));
        deviceCommand.ExecuteNonQuery();

        switch (deviceId.Split('-')[0])
        {
            case "ED":
            {
                var edQuery = @"UPDATE Embedded SET 
                              lpAddress = @Ip, 
                              NetworkName = @Network 
                              WHERE DeviceId = @Id";
                using var edCommand = new SqlCommand(edQuery, connection);
                edCommand.Parameters.AddWithValue("@Id", deviceId);
                edCommand.Parameters.AddWithValue("@Ip", json["ipAddress"]?.ToString());
                edCommand.Parameters.AddWithValue("@Network", json["networkName"]?.ToString());
                edCommand.ExecuteNonQuery();
                break;
            }

            case "PC":
            {
                var pcQuery = @"UPDATE PersonalComputer SET 
                               OperationSystem = @OS 
                               WHERE DeviceId = @Id";
                using var pcCommand = new SqlCommand(pcQuery, connection);
                pcCommand.Parameters.AddWithValue("@Id", deviceId);
                pcCommand.Parameters.AddWithValue("@OS", json["operationSystem"]?.ToString());
                pcCommand.ExecuteNonQuery();
                break;
            }

            case "SW":
            {
                var swQuery = @"UPDATE Smartwatch SET 
                               BatteryPercentage = @Battery 
                               WHERE DeviceId = @Id";
                using var swCommand = new SqlCommand(swQuery, connection);
                swCommand.Parameters.AddWithValue("@Id", deviceId);
                swCommand.Parameters.AddWithValue("@Battery", Convert.ToInt32(json["batteryPercentage"]));
                swCommand.ExecuteNonQuery();
                break;
            }
        }
    }

    public void DeleteDevice(string deviceId)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        var deleteQuery = "DELETE FROM Device WHERE Id = @Id";
        using var command = new SqlCommand(deleteQuery, connection);
        command.Parameters.AddWithValue("@Id", deviceId);
        command.ExecuteNonQuery();
    }
}