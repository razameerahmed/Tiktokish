using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIGateway.Controllers
{
	[ApiController]
	[Route("")]
	public class HomeController : Controller
	{
		//[HttpGet]
		//[Authorize]
		//public IActionResult GetValues()
		//{
		//	var request = Request;
		//	return Ok(new string[] { "value1", "value2" });
		//}

        [HttpGet]
        public IActionResult Index()
        {
            var request = Request;
            return Ok("Gateway Service is running");
        }
    }
}
