using Microsoft.AspNetCore.Mvc;
using Models;
using RestApi.DTO;
namespace RestApi.Controllers

{
    [ApiController]
    [Route("api/[controller]")]
    public class DeviceController : ControllerBase
    {
        private static List<Device> _devices = new List<Device>();
        private const int MaxDevices = 15;

        public DeviceController()
        {
            if (_devices.Count == 0)
            {
                _devices.Add(new SmartWatches("SW-1", "Apple Watch SE2", 85));
                _devices.Add(new Personal_Computer("P-1", "LinuxPC", "Linux Mint"));
                _devices.Add(new Embedded_devices("ED-1", "RaspberryPi", "192.168.1.5", "MD Ltd Network"));
            }
        }

        [HttpGet("getAllShits")]
        public ActionResult<IEnumerable<DeviceDto>> getAllShits()
        {
            var deviceDtos = new List<DeviceDto>();

            foreach (var device in _devices)
            {
                deviceDtos.Add(new DeviceDto
                {
                    ID = device.ID,
                    Name = device.Name,
                    IsDeviceTurned = device.IsDeviceTurned
                });
            } 
            
            return Ok(deviceDtos);
        }

        [HttpGet("GetShitByID")]
        public ActionResult<object> getShitByID(string id)
        {
            var device = _devices.Find(d => d.ID == id);
            
            if (device == null)
            {
                return NotFound();
            }

            if (device is SmartWatches smartWatch)
            {
                return Ok(new
                {
                    ID = device.ID,
                    Name = device.Name,
                    IsDeviceTurned = device.IsDeviceTurned,
                    BatteryPercent = smartWatch.batteryPercent
                });
            }
            else if (device is Personal_Computer pc)
            {
                return Ok(new
                {
                    ID = device.ID,
                    Name = device.Name,
                    IsDeviceTurned = device.IsDeviceTurned,
                    OperatingSystem = pc.OperatingSystem
                });
            }
            else if (device is Embedded_devices embedded)
            {
                return Ok(new
                {
                    ID = device.ID,
                    Name = device.Name,
                    IsDeviceTurned = device.IsDeviceTurned,
                    IPAddress = embedded.IPAddress,
                    NetworkName = embedded.NetworkName
                });
            }
            return Ok(new DeviceDto
            {
                ID = device.ID,
                Name = device.Name,
                IsDeviceTurned = device.IsDeviceTurned
            });
        }
    
        [HttpPost("CreateShit")]
        public ActionResult<object> CreateShit(CreateDeviceDto createDto)
        {
            if (_devices.Count >= MaxDevices)
            {
                return BadRequest("Send me a 100 dollars, if you want to create a new device, my number -> 787 325 044");
            }

            try
            {
                Device newDevice;

                switch (createDto.DeviceType.ToLower())
                {
                    case "sw":
                        if (!createDto.BatteryPercent.HasValue)
                            return BadRequest("Input a baterry %");
                            
                        newDevice = new SmartWatches(
                            createDto.ID, 
                            createDto.Name, 
                            createDto.BatteryPercent.Value);
                        break;
                        
                    case "pc":
                        newDevice = new Personal_Computer(
                            createDto.ID,
                            createDto.Name, 
                            createDto.OperatingSystem);
                        break;
                        
                    case "ed":
                        if (string.IsNullOrEmpty(createDto.IPAddress) || string.IsNullOrEmpty(createDto.NetworkName))
                            return BadRequest("Input IP Address and Network Name");
                            
                        newDevice = new Embedded_devices(
                            createDto.ID,
                            createDto.Name,
                            createDto.IPAddress,
                            createDto.NetworkName);
                        break;
                        
                    default:
                        return BadRequest("NOT GOOD DEVICE TYPE");
                }

                if (_devices.Any(d => d.ID == newDevice.ID))
                {
                    return BadRequest($"Shit with ID {newDevice.ID} already exists");
                }

                _devices.Add(newDevice);

                return CreatedAtAction(nameof(getShitByID), new { id = newDevice.ID }, new DeviceDto
                {
                    ID = newDevice.ID,
                    Name = newDevice.Name,
                    IsDeviceTurned = newDevice.IsDeviceTurned
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateShitByID")]
        public IActionResult UdpateShit(string id, UpdateDeviceDto updateDto)
        {
            if (id != updateDto.ID)
            {
                return BadRequest("Input correct id");
            }

            var deviceIndex = _devices.FindIndex(d => d.ID == id);
            
            if (deviceIndex == -1)
            {
                return NotFound();
            }

            var existingDevice = _devices[deviceIndex];

            try
            {
                Device updatedDevice;

                if (existingDevice is SmartWatches)
                {
                    if (!updateDto.BatteryPercent.HasValue)
                    {
                        return BadRequest("Input a baterry %");
                    }

                    updatedDevice = new SmartWatches(
                        updateDto.ID,
                        updateDto.Name ?? existingDevice.Name,
                        updateDto.BatteryPercent.Value);
                    
                    if (existingDevice.IsDeviceTurned)
                    {
                        try { updatedDevice.TurnedOn(); } 
                        catch { /* Ignore error */ }
                    }
                }
                else if (existingDevice is Personal_Computer)
                {
                    var pc = (Personal_Computer)existingDevice;
                    
                    updatedDevice = new Personal_Computer(
                        updateDto.ID,
                        updateDto.Name ?? existingDevice.Name,
                        updateDto.OperatingSystem ?? pc.OperatingSystem);
                    
                    if (existingDevice.IsDeviceTurned)
                    {
                        try { updatedDevice.TurnedOn(); } 
                        catch { /* Ignore error */ }
                    }
                }
                else if (existingDevice is Embedded_devices)
                {
                    var embedded = (Embedded_devices)existingDevice;
                    
                    updatedDevice = new Embedded_devices(
                        updateDto.ID,
                        updateDto.Name ?? existingDevice.Name,
                        updateDto.IPAddress ?? embedded.IPAddress,
                        updateDto.NetworkName ?? embedded.NetworkName);
                    
                    if (existingDevice.IsDeviceTurned)
                    {
                        try { updatedDevice.TurnedOn(); } 
                        catch { /* Ignore error */ }
                    }
                }
                else
                {
                    updatedDevice = new Device(
                        updateDto.ID, 
                        updateDto.Name ?? existingDevice.Name);
                    
                    if (existingDevice.IsDeviceTurned)
                    {
                        updatedDevice.TurnedOn();
                    }
                }

                _devices[deviceIndex] = updatedDevice;
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteShitByID")]
        public IActionResult DeleteShit(string id)
        {
            var device = _devices.Find(d => d.ID == id);
            
            if (device == null)
            {
                return NotFound();
            }
            
            _devices.Remove(device);
            return NoContent();
        }
    }
}