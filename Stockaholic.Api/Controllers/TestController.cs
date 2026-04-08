using Microsoft.AspNetCore.Mvc;

namespace Stockaholic.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("API is working!");
        }
    }
}