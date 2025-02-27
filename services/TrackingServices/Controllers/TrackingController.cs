using Microsoft.AspNetCore.Mvc;
using TrackingService.Services;
using TrackingServices.Models;

namespace TrackingServices.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrackingController(ITrackingService trackingService) : ControllerBase
    {
       
        private readonly ITrackingService _trackingService = trackingService;

        [HttpPost("track")]
        public async Task<IActionResult> TrackActivity([FromBody] UserActivityEvent activityEvent)
        {
            await _trackingService.TrackActivityAsync(activityEvent);
           
            return Ok();
        }
    }
}
