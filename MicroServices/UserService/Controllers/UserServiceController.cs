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
			//ActivityLogger.Instance.SystemLog(NLog.LogLevel.Info, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), correlationId, "User", "machine name", this.GetType().Name, "User account created", 1);

			//if (string.IsNullOrWhiteSpace(request.Identifier) || string.IsNullOrWhiteSpace(request.Password))
			//	return BadRequest("Username/Email/Phone and Password are required");

			var res = _userManager.ValidateLogin(request);
			return Ok(res);
			//var user = _context.UserInfos
			//	.FirstOrDefault(u =>
			//		u.Username == request.Identifier ||
			//		u.Email == request.Identifier ||
			//		u.PhoneNumber == request.Identifier);

			//if (user == null || !user.IsActive)
			//	return Unauthorized("Invalid login credentials");

			//bool passwordValid = true;// BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
			//if (!passwordValid)
			//	return Unauthorized("Invalid login credentials");

			////string token = GenerateJwtToken(user);

			//user.LastLoginAt = DateTime.UtcNow;
			//_context.SaveChanges();

			//return Ok(new LoginResponse
			//{
			//	//Token = token,
			//	Username = user.Username,
			//	AvatarUrl = user.AvatarUrl,
			//	Role = user.Role,
			//	IsVerified = user.IsVerified
			//});

			//return Ok(new LoginResponse
			//{
			//	IsVerified = true,
			//	Username = "UserService",
			//	Token = ""			});
		}

	}
}
