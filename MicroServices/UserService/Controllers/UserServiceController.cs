using Common.Implementation;
using Common.Interface;
using Common.Model;
using DataAccess;
using DataAccessLayer.Models;
using Extension.Security.Implementation;
using Extension.Security.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;


namespace UserService.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class UserServiceController : ControllerBase
	{
		private IUserManager _userManager;
		private readonly IConfiguration _configuration;

		public UserServiceController(IUserManager userManager, IConfiguration configuration)
		{
			_userManager = userManager;
			_configuration = configuration;
		}

		[HttpGet]
		public IActionResult Index()
		{
			var request = Request;
			return Ok("User Service is running");
		}

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
				ActivityLogger.Instance.SystemLog(NLog.LogLevel.Error, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), request.correlationId, request.Identifier, "machine name", this.GetType().Name, ex.Message, 0, ex);
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

		[HttpPost("edituser")]
		public IActionResult EditUser([FromBody] CommonUser request)
		{
			try
			{
				ActivityLogger.Instance.SystemLog(NLog.LogLevel.Info, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), request.CorrelationId, request.Username, "machine name", this.GetType().Name, "User Login", 1);

				ResponseModel<LoginResponse> response = new ResponseModel<LoginResponse>();
				var res = _userManager.UpdateUser(request, response);
				return Ok(res);
			}
			catch (Exception ex)
			{
				ActivityLogger.Instance.SystemLog(NLog.LogLevel.Error, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), request.CorrelationId, request.Username, "machine name", this.GetType().Name, ex.Message, 0, ex);
				return BadRequest(ex.Message);
			}
		}

		[HttpPost("validateusername")]
		public IActionResult ValidateUsername([FromBody] CommonUser request)
		{
			try
			{
				ActivityLogger.Instance.SystemLog(NLog.LogLevel.Info, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), request.CorrelationId, request.Username, "machine name", this.GetType().Name, "Validate User", 1);

				ResponseModel<LoginResponse> response = new ResponseModel<LoginResponse>();
				var res = _userManager.ValidateUserForAdd(request, response);
				return Ok(res);
			}
			catch (Exception ex)
			{
				ActivityLogger.Instance.SystemLog(NLog.LogLevel.Error, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), request.CorrelationId, request.Username, "machine name", this.GetType().Name, ex.Message, 0, ex);
				return BadRequest(ex.Message);
			}
		}
	}
}
