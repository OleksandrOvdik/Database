using System.Data;
using Api.DTO;
using Repository.Interface;
namespace Repository;
using Microsoft.Data.SqlClient;
using Models;
using Models.Classes;
using System.Text.Json.Nodes;

public class DeviceRepository : IDeviceRepository
{
    
    private readonly string _connectionString;
    
    public DeviceRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IEnumerable<DeviceDTO> GetAllModels()
    {
        var devices = new List<DeviceDTO>();
        const string query = "SELECT * FROM Device";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(query, connection);
        connection.Open();
        using var reader =  command.ExecuteReader();

        while (reader.Read())
        {
            devices.Add(new DeviceDTO
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

        var split = id.Split('-')[0].ToUpper();

        var deviceQuery = "SELECT Id, Name, IsEnabled FROM Device WHERE Id = @Id";
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
        
        switch (split)
        {
            case "ED":
            {
                Console.WriteLine("ED CASE OPEN");
                var query = @"SELECT IpAddress, NetworkName FROM Embedded WHERE DeviceId = @DeviceId";
                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@DeviceId", id);
                using var reader = command.ExecuteReader();
            
                if (reader.Read())
                {
                    return new EmbeddedDevices(baseDevice.Id, baseDevice.Name, reader["IpAddress"].ToString(),
                        reader["NetworkName"].ToString());
                }
                Console.WriteLine("ED CASE CLOSE");
                reader.Close();
                break;
            }
            case "PC":
                {
                    Console.WriteLine("CASE PC OPEN");
                    var query = @"SELECT OperationSystem FROM PersonalComputer WHERE DeviceId = @DeviceId";
                    Console.WriteLine("AFTER QUERY");
                    using var command = new SqlCommand(query, connection);
                    Console.WriteLine("AFTER command");
                    command.Parameters.AddWithValue("@DeviceId", id);
                    Console.WriteLine("AFTER add");
                    using var reader = command.ExecuteReader();
                    Console.WriteLine("AFTER reader");
                    
                    if(!reader.HasRows) throw new ArgumentException("NO DATA");
            
                    if (reader.Read())
                    {
                        Console.WriteLine("READING");
                         return new PersonalComputer(baseDevice.Id, baseDevice.Name,
                            reader["OperationSystem"].ToString());

                    }
                    Console.WriteLine("CASE PC CLOSE");
                    reader.Close();
                    break;
                }
            case "SW" :
                {
                    Console.WriteLine("SW OPEN");
                    var query = @"SELECT BatteryPercentage FROM Smartwatch WHERE DeviceId = @DeviceId";
                    using var command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@DeviceId", id);
                    using var reader = command.ExecuteReader();
            
                    if (reader.Read())
                    {
                        return new SmartWatches(baseDevice.Id, baseDevice.Name,
                            Convert.ToInt32(reader["BatteryPercentage"]));
                    }
                    Console.WriteLine("SW CLOSE");
                    reader.Close();
                    
                    break;
                }
        }
        return baseDevice;
    }

    

    public async Task<string> CreateEmbeddedDevice(EmbeddedDevices ed)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand("AddEmbedded", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("@DeviceId", ed.Id);
        command.Parameters.AddWithValue("@Name", ed.Name);
        command.Parameters.AddWithValue("@IsEnabled", ed.IsDeviceTurned);
        command.Parameters.AddWithValue("@IpAddress", ed.IpAddress);
        command.Parameters.AddWithValue("@NetworkName", ed.NetworkName);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
        return ed.Id;
    }

    public async Task<string> CreateSmartwatchDevice(SmartWatches sw)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand("AddSmartwatch", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("@DeviceId", sw.Id);
        command.Parameters.AddWithValue("@Name", sw.Name);
        command.Parameters.AddWithValue("@IsEnabled", sw.IsDeviceTurned);
        command.Parameters.AddWithValue("@BatteryPercentage", sw.batteryPercent);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
        return sw.Id;
    }


    public async Task<string> CreatePersonalComputerDevice(PersonalComputer pc)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand("AddPersonalComputer", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("@DeviceId", pc.Id);
        command.Parameters.AddWithValue("@Name", pc.Name);
        command.Parameters.AddWithValue("@IsEnabled", pc.IsDeviceTurned);
        command.Parameters.AddWithValue("@OperationSystem", pc.OperatingSystem);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
        return pc.Id;
    }
    
    public async Task<bool> UpdateDevice(string jsonData)
    {
        var json = JsonNode.Parse(jsonData);
        var deviceId = json["deviceId"]?.ToString();

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        
        SqlTransaction transaction = connection.BeginTransaction();

        var updateDevice = @"UPDATE Device SET 
                           Name = @Name, 
                           IsEnabled = @IsEnabled 
                           WHERE Id = @DeviceId and RowVer = @RowVersion";
        
        using var deviceCommand = new SqlCommand(updateDevice, connection, transaction);
        deviceCommand.Parameters.AddWithValue("@DeviceId", deviceId);
        deviceCommand.Parameters.AddWithValue("@Name", json["name"]?.ToString());
        deviceCommand.Parameters.AddWithValue("@IsEnabled", json["isEnabled"].GetValue<bool>());
        deviceCommand.Parameters.AddWithValue("@RowVersion", TakeRowVerDevice(deviceId));
        var  affectedRows = await deviceCommand.ExecuteNonQueryAsync();
        if (affectedRows == 0 )
        {
            transaction.Rollback();
        }

        switch (deviceId.Split('-')[0])
        {
            case "ED":
            {
                var edQuery = @"UPDATE Embedded SET 
                              IpAddress = @Ip, 
                              NetworkName = @Network 
                              WHERE DeviceId = @DeviceId and RowVer = @RowVersion";
                          
                using var edCommand = new SqlCommand(edQuery, connection, transaction);
                edCommand.Parameters.AddWithValue("@DeviceId", deviceId);
                edCommand.Parameters.AddWithValue("@Ip", json["IpAddress"]?.ToString());
                edCommand.Parameters.AddWithValue("@Network", json["networkName"]?.ToString());
                edCommand.Parameters.AddWithValue("@RowVersion", TakeRowVerChild(deviceId));
                var affectedRowsED = await edCommand.ExecuteNonQueryAsync();
                if (affectedRowsED == 0)
                {
                    transaction.Rollback();
                    return false;
                }
                break;
            }

            case "PC":
            {
                var pcQuery = @"UPDATE PersonalComputer SET 
                               OperationSystem = @OS 
                               WHERE DeviceId = @DeviceId and RowVer = @RowVersion";
                using var pcCommand = new SqlCommand(pcQuery, connection, transaction);
                pcCommand.Parameters.AddWithValue("@DeviceId", deviceId);
                pcCommand.Parameters.AddWithValue("@OS", json["operationSystem"]?.ToString());
                pcCommand.Parameters.AddWithValue("@RowVersion", TakeRowVerChild(deviceId));
                var affectedRowsPC = await pcCommand.ExecuteNonQueryAsync();

                if (affectedRowsPC == 0)
                {
                    transaction.Rollback();
                    return false;
                }
                break;
            }

            case "SW":
            {
                var swQuery = @"UPDATE Smartwatch SET 
                               BatteryPercentage = @Battery 
                               WHERE DeviceId = @DeviceId and RowVer = @RowVersion";
                using var swCommand = new SqlCommand(swQuery, connection, transaction);
                swCommand.Parameters.AddWithValue("@DeviceId", deviceId);
                swCommand.Parameters.AddWithValue("@Battery", Convert.ToInt32(json["batteryPercentage"]));
                swCommand.Parameters.AddWithValue("@RowVersion", TakeRowVerChild(deviceId));
                var affectedRowsSW = await swCommand.ExecuteNonQueryAsync();

                if (affectedRowsSW == 0)
                {
                    transaction.Rollback();
                    return false;
                }
                break;
            }
            
            default:
                transaction.Rollback();
                return false;
        }
        transaction.Commit();
        return true;
    }

    public async Task<bool> DeleteDevice(string deviceId)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        
        SqlTransaction transaction = connection.BeginTransaction();

        var deleteQuery = "DELETE FROM Device WHERE Id = @Id";
        using var command = new SqlCommand(deleteQuery, connection,transaction);
        command.Parameters.AddWithValue("@Id", deviceId);
        await command.ExecuteNonQueryAsync();
        transaction.Commit();
        return true;
    }

    public byte[] TakeRowVerDevice(string id)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        
        var query = "SELECT RowVer FROM Device WHERE Id = @Id";
        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@Id", id);
        using var reader = command.ExecuteReader();
        reader.Read();
        var result =  reader.GetValue(0) as byte[];
        reader.Close();
        return result;
        
    }
    
    public byte[] TakeRowVerChild(string id)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        var result = null as byte[];
        
        var split = id.Split('-')[0].ToUpper();
        
        switch (split)
        {
            case "ED":
            {
                var query = "SELECT RowVer FROM Embedded WHERE DeviceId = @Id";
                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);
                using var reader = command.ExecuteReader();
                reader.Read();
                result =  reader.GetValue(0) as byte[];
                reader.Close();
                break;
            }
            case "PC":
            {
                var query = "SELECT RowVer FROM PersonalComputer WHERE DeviceId = @Id";
                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);
                using var reader = command.ExecuteReader();
                reader.Read();
                result = reader.GetValue(0) as byte[];
                reader.Close();
                break;
            }
            case "SW":
            {
                var query = "SELECT RowVer FROM Smartwatch WHERE DeviceId = @Id";
                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);
                using var reader = command.ExecuteReader();
                reader.Read();
                result = reader.GetValue(0) as byte[];
                reader.Close();
                break;
            }
        }
        return result;
    }
}