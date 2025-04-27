/*using Electronics.Classes;
using Microsoft.AspNetCore.Mvc;
using Models.Classes;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/devices")]
    public class DeviceController : ControllerBase
    {
        [HttpGet]
        public IResult GetDevices()
        {
            return Results.Ok(DeviceManager.Instance.Devices);
        }

        [HttpGet(":id")]
        public IResult GetDevice(string? id)
        {
            try
            {
                return Results.Ok(DeviceManager.Instance.GetDeviceById(id));
            }
            catch
            {
                return Results.NotFound();
            }
        }

         [HttpPost("smartwatches")]
         public IResult AddDevice(SmartWatches device)
         {
             try
             {
                 DeviceManager.Instance.AddShit(device);
                 return Results.Ok();
             }
             catch
             {
                 return Results.Conflict("There is device with this id");

             }

         }

         [HttpPost("personal-computers")]
         public IResult AddDevice(PersonalComputer device)
         {
             DeviceManager.Instance.AddShit(device);
             return Results.Ok();
         }

         [HttpPost("embedded-devices")]
         public IResult AddDevice(EmbeddedDevices device)
         {
             DeviceManager.Instance.AddShit(device);
             return Results.Ok();
         }

         [HttpPut("smartwatches/:id")]
         public IResult UpdateSwDevice(string? id, [FromBody] SmartWatches device)
         {
             DeviceManager.Instance.EditShit(id, device);
             try
             {
                 return Results.Ok(DeviceManager.Instance.GetDeviceById(id));
             }
             catch
             {
                 return Results.NotFound();
             }
         }

         [HttpPut("personal-computer/:id")]
         public IResult UpdatePcDevice(string? id, [FromBody] PersonalComputer device)
         {
             DeviceManager.Instance.EditShit(id, device);
             try
             {
                 return Results.Ok(DeviceManager.Instance.GetDeviceById(id));
             }
             catch
             {
                 return Results.NotFound();
             }
         }

         [HttpPut("embedded-devices/:id")]
         public IResult UpdateEdDevice(string? id, [FromBody] EmbeddedDevices device)
         {
             DeviceManager.Instance.EditShit(id, device);
             try
             {
                 return Results.Ok(DeviceManager.Instance.GetDeviceById(id));
             }
             catch
             {
                 return Results.NotFound();
             }
         }

         [HttpDelete(":id")]
         public IResult DeleteDevice(string? id)
         {
             try
             {
                 DeviceManager.Instance.RemoveShit(id);
                 return Results.Ok();
             }
             catch
             {
                 return Results.NotFound();
             }
         }
    }

}*/


using System.Text.Json.Nodes;
using Electronics;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DevicesController : ControllerBase
    {
        private readonly DeviceService _deviceService;

        public DevicesController(DeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        // GET: api/devices
        [HttpGet]
        public IActionResult GetAllDevices()
        {
            var devices = _deviceService.GetAllModels();
            return Ok(devices);
        }

        // GET: api/devices/{id}
        [HttpGet("{id}")]
        public IActionResult GetDeviceById(string id)
        {
            var device = _deviceService.GetDeviceById(id);
            if (device == null)
            {
                return NotFound($"Device with id {id} not found.");
            }
            return Ok(device);
        }

        // POST: api/devices
        [HttpPost]
        [Consumes("application/json", "text/plain")]
        public async Task<IActionResult> CreateDevice()
        {
            string? contentType = Request.ContentType?.ToLower();

            switch (contentType)
            {
                case "application/json":
                {
                    using var reader = new StreamReader(Request.Body);
                    string rawJson = await reader.ReadToEndAsync();

                    var json = JsonNode.Parse(rawJson);
                    if (json == null)
                    {
                        return BadRequest("Invalid JSON format.");
                    }

                    var deviceType = json["deviceType"];
                    if (deviceType == null)
                    {
                        return BadRequest("Missing deviceType.");
                    }

                    try
                    {
                        string newId = _deviceService.CreateDevice(rawJson);
                        return CreatedAtAction(nameof(GetDeviceById), new { id = newId }, new { id = newId });
                    }
                    catch (Exception ex)
                    {
                        return BadRequest($"Error creating device: {ex.Message}");
                    }
                }

                case "text/plain":
                {
                    using var reader = new StreamReader(Request.Body);
                    string plainText = await reader.ReadToEndAsync();

                    // Тут можна розпарсити plain text формат, якщо потрібно
                    return BadRequest("Plain text import is not yet implemented.");
                }

                default:
                    return Conflict("Unsupported Content-Type. Only 'application/json' or 'text/plain' are accepted.");
            }
        }

        // PUT: api/devices
        [HttpPut]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateDevice()
        {
            using var reader = new StreamReader(Request.Body);
            string rawJson = await reader.ReadToEndAsync();

            var json = JsonNode.Parse(rawJson);
            if (json == null)
            {
                return BadRequest("Invalid JSON format.");
            }

            var deviceId = json["deviceId"];
            if (deviceId == null)
            {
                return BadRequest("Missing deviceId.");
            }

            try
            {
                _deviceService.UpdateDevice(rawJson);
                return NoContent(); // 204 Success
            }
            catch (Exception ex)
            {
                // return BadRequest($"Error updating device: {ex.Message}
                throw ex;
            }
        }

        // DELETE: api/devices/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteDevice(string id)
        {
            try
            {
                _deviceService.DeleteDevice(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting device: {ex.Message}");
            }
        }
    }
}

