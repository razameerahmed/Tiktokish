using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;
using System.Configuration;
using NLog;
using NLog.Web;
using Common.Implementation;
using Common.Interface;


namespace UserService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserServiceController : ControllerBase
    {
		[HttpGet]
        public IActionResult TestConnection()
		{
			var correlationId = HttpContext.Request.Headers["X-Correlation-ID"].ToString();

            ActivityLogger.Instance.SystemLog(NLog.LogLevel.Info, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), correlationId, "User", "machine name", this.GetType().Name, "User account created", 1);

            return Ok(new { Message = "Hello from UserService!", CorrelationId = correlationId });

			//return Ok(new
			//{
			//	ok = true,
			//	service = "UserService",
			//	timeUtc = DateTime.UtcNow
			//});

		}
    }
}
