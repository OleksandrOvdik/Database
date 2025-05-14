
using System.Text.Json;
using System.Text.Json.Nodes;
using Electronics;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class DevicesController : ControllerBase
    {
        private readonly IDeviceService _deviceService;

        public DevicesController(IDeviceService deviceService)
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

            try
            {
                using var reader = new StreamReader(Request.Body);
                var rawData = await reader.ReadToEndAsync();

                if (string.IsNullOrWhiteSpace(rawData))
                    return BadRequest("Request body is empty");

                switch (contentType)
                {
                    case "application/json":
                    case "text/plain":
                        var newId = await _deviceService.CreateDevice(rawData);
                        return CreatedAtAction(nameof(GetDeviceById), new { id = newId }, new { id = newId });

                    default:
                        return Conflict(
                            "Unsupported Content-Type. Only 'application/json' or 'text/plain' are accepted.");
                }
            }
            catch (JsonException)
            {
                return BadRequest("Invalid JSON format");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
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
                return Ok(); 
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
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting device: {ex.Message}");
            }
        }
    }
}

