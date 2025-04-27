
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

        [HttpGet]
        public IActionResult GetAllDevices()
        {
            var devices = _deviceService.GetAllModels();
            return Ok(devices);
        }

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
                    return BadRequest("Plain text import is not yet implemented.");
                }

                default:
                    return Conflict("Unsupported Content-Type. Only 'application/json' or 'text/plain' are accepted.");
            }
        }

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
                return NoContent(); 
            }
            catch (Exception ex)
            {
                return BadRequest("Error updating device: " + ex.Message);
            }
        }

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

