using System.Data;
using Api.DTO;
using Azure.Core;
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


    public async Task<string> CreateDevice(string jsonData)
    {
        var json = JsonNode.Parse(jsonData);
        var deviceType = json["deviceType"]?.ToString().ToUpper();
        if (string.IsNullOrEmpty(deviceType))
        {
            throw new ArgumentException("Device type is required.");
        }
        var newId = $"{deviceType}-{Guid.NewGuid():N}";

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        
        // TRANSACTION
        SqlTransaction transaction = connection.BeginTransaction();
        
        var insertDevice = @"INSERT INTO Device (Id, Name, IsEnabled) 
                          VALUES (@Id, @Name, @IsEnabled)";
        
        using var deviceCommand = new SqlCommand(insertDevice, connection, transaction);

        
        // EXECUTE PROCEDURE
        // deviceCommand.CommandType = CommandType.StoredProcedure;
        deviceCommand.Parameters.AddWithValue("@Id", newId);
        deviceCommand.Parameters.AddWithValue("@Name", json["name"]?.ToString());
        deviceCommand.Parameters.AddWithValue("@IsEnabled", json["isEnabled"].GetValue<bool>());
        await deviceCommand.ExecuteNonQueryAsync();

        switch (deviceType)
        {
            case "ED":
            {
                var edQuery = @"INSERT INTO Embedded (lpAddress, NetworkName, DeviceId) 
                             VALUES (@DeviceId, @Network, @Id)";
                using var edCommand = new SqlCommand(edQuery, connection, transaction);
                
                //EXECUTE PROCEDURE
                // edCommand.CommandType = CommandType.StoredProcedure;
                edCommand.Parameters.AddWithValue("@DeviceId", newId);
                edCommand.Parameters.AddWithValue("@Ip", json["ipAddress"]?.ToString());
                edCommand.Parameters.AddWithValue("@Network", json["networkName"]?.ToString());
                await edCommand.ExecuteNonQueryAsync();
                break;
            }

            case "PC":
            {
                var pcQuery = @"INSERT INTO PersonalComputer (DeviceId, OperationSystem) 
                              VALUES (@DeviceId, @OS)";
                using var pcCommand = new SqlCommand(pcQuery, connection, transaction);
                
                //EXECUTE PROCEDURE
                // pcCommand.CommandType = CommandType.StoredProcedure;
                pcCommand.Parameters.AddWithValue("@DeviceId", newId);
                pcCommand.Parameters.AddWithValue("@OS", json["operationSystem"]?.ToString());
                await pcCommand.ExecuteNonQueryAsync();
                break;
            }

            case "SW":
            {
                var swQuery = @"INSERT INTO Smartwatch (BatteryPercentage, DeviceId) 
                              VALUES (@Battery, @DeviceId)";
                using var swCommand = new SqlCommand(swQuery, connection, transaction);
                
                //EXECUTE PROCEDURE
                // swCommand.CommandType = CommandType.StoredProcedure;
                swCommand.Parameters.AddWithValue("@DeviceId", newId);
                swCommand.Parameters.AddWithValue("@Battery", Convert.ToInt32(json["batteryPercentage"]));
                await swCommand.ExecuteNonQueryAsync();
                break;
            }

            default:
                transaction.Rollback();
                throw new ArgumentException("WRONG DEVICE TYPE");
        }
        transaction.Commit();
        return newId;
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
        deviceCommand.Parameters.AddWithValue("@RowVersion", takeRowVerDevice(deviceId));
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
                edCommand.Parameters.AddWithValue("@RowVersion", takeRowVerChild(deviceId));
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
                pcCommand.Parameters.AddWithValue("@RowVersion", takeRowVerChild(deviceId));
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
                swCommand.Parameters.AddWithValue("@RowVersion", takeRowVerChild(deviceId));
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

    public byte[] takeRowVerDevice(string id)
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
    
    public byte[] takeRowVerChild(string id)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        var result = null as byte[];
        
        var split = id.Split('-')[0].ToUpper();
        
        // var mainQuery = "SELECT RowVer FROM Device WHERE Id = @Id";
        // using var mainCommand = new SqlCommand(mainQuery, connection);
        // mainCommand.Parameters.AddWithValue("@Id", id);
        // using var mainReader = mainCommand.ExecuteReader();
        // mainReader.Read();
        // result = mainReader.GetValue(0) as byte[];
        // Console.WriteLine(result);
        // mainReader.Close();

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