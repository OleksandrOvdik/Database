using Electronics.Classes;
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
         public IResult AddDevice(Embeddeddevices device)
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
         public IResult UpdateEdDevice(string? id, [FromBody] Embeddeddevices device)
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
}