using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIGateway.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class HomeController : Controller
	{
		//[HttpGet]
		//public IActionResult Index()
		//{
		//	return View();
		//}

		[HttpGet]
		[Authorize]
		public IActionResult GetValues()
		{
			var request = Request;
			return Ok(new string[] { "value1", "value2" });
		}
	}
}
