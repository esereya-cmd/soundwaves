using Microsoft.AspNetCore.Mvc;

namespace Soundwaves.Api.Controllers
{
    [ApiController]
    [Route("api/health")]
    public class HealthController : ControllerBase
    {
    [HttpGet]
        public IActionResult GetHealth()
        {
            return Ok("Soundwaves API is healthy!");
        }
    }
}