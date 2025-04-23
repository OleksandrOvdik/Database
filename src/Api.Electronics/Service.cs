using System.ComponentModel;
using Microsoft.Data.SqlClient;
using Models;
using Models.Classes;

namespace Electronics;

public class Service
{
    private string _connectionString;

    public Service(string connectionString)
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
                    const string swString = "select * from Devices where Id = @id join SmartWatches on SmartWatches.DeviceId = DeviceId";
                    SqlCommand commandSw = new SqlCommand(swString, connection);
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
                        

                if (id.Contains("P"))
                {
                    const string pcString = "select * from Devices where Id = @id join PersonalComputer on PersonalComputer.DeviceId = DeviceId";
                    SqlCommand commandPc = new SqlCommand(pcString, connection);
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
                if (id.Contains("P"))
                {
                    const string edString = "select * from Devices where Id = @id join EmbeddedDevices on EmbeddedDevices.DeviceId = DeviceId";
                    SqlCommand commandEd = new SqlCommand(edString, connection);
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
                
            }
        }
        return device;
    }
    
}