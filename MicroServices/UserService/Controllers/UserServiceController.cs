using Common.Implementation;
using Common.Interface;
using Common.Model;
using DataAccess;
using DataAccessLayer.Models;
using Extension.Security.Implementation;
using Extension.Security.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;


namespace UserService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserServiceController : ControllerBase
    {
		private IUserManager _userManager;

		public UserServiceController(IUserManager userManager) {
			_userManager = userManager;
		}

		[HttpGet]
		public IActionResult Index()
		{
			var request = Request;
			return Ok("User Service is running");
		}
		//private readonly TiktokishContext _context;

		//public UserServiceController(TiktokishContext context)
		//{
		//	_context = context;
		//}

		//[HttpGet]
		//      public IActionResult TestConnection()
		//{
		//	var correlationId = HttpContext.Request.Headers["X-Correlation-ID"].ToString();

		//         ActivityLogger.Instance.SystemLog(NLog.LogLevel.Info, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), correlationId, "User", "machine name", this.GetType().Name, "User account created", 1);

		//          return Ok(new { Message = "Hello from UserService!", CorrelationId = correlationId });

		//	//return Ok(new
		//	//{
		//	//	ok = true,
		//	//	service = "UserService",
		//	//	timeUtc = DateTime.UtcNow
		//	//});

		//}


		[HttpPost("validatelogin")]
		public IActionResult ValidateLogin([FromBody] LoginRequest request)
		{
			try
			{
				ActivityLogger.Instance.SystemLog(NLog.LogLevel.Info, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), request.correlationId, request.Identifier, "machine name", this.GetType().Name, "User Login", 1);

				if (string.IsNullOrWhiteSpace(request.Identifier) || string.IsNullOrWhiteSpace(request.Password))
					return BadRequest("Username/Email/Phone and Password are required");

				ResponseModel<LoginResponse> response = new ResponseModel<LoginResponse>();
				var res = _userManager.ValidateLogin(request, response);
				return Ok(res);
			}
			catch (Exception ex)
			{
				ActivityLogger.Instance.SystemLog(NLog.LogLevel.Error, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), request.correlationId, request.Identifier, "machine name", this.GetType().Name, ex.Message, 0,ex);
				return BadRequest(ex.Message);
			}
		}

		[HttpPost("createuseraccount")]
		public IActionResult CreateUserAccount([FromBody] LoginRequest request)
		{
			try
			{
				ActivityLogger.Instance.SystemLog(NLog.LogLevel.Info, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), request.correlationId, request.Identifier, "machine name", this.GetType().Name, "User Login", 1);

				if (string.IsNullOrWhiteSpace(request.Identifier) || string.IsNullOrWhiteSpace(request.Password))
					return BadRequest("Username/Email/Phone and Password are required");

				ResponseModel<LoginResponse> response = new ResponseModel<LoginResponse>();
				var res = _userManager.AddUser(request, response);
				return Ok(res);
			}
			catch (Exception ex)
			{
				ActivityLogger.Instance.SystemLog(NLog.LogLevel.Error, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), request.correlationId, request.Identifier, "machine name", this.GetType().Name, ex.Message, 0, ex);
				return BadRequest(ex.Message);
			}
		}
	}
}
