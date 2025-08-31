using Common.Implementation;
using Common.Interface;
using Microsoft.AspNetCore.Http;
using NLog;
using System.Threading.Tasks;

namespace UserService.Middleware
{
	public class LoggingCorrelationMiddleware
	{
		private readonly RequestDelegate _next;

		public LoggingCorrelationMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			var correlationId = context.Request.Headers["X-Correlation-ID"].ToString();
			MappedDiagnosticsLogicalContext.Set("CorrelationId", correlationId);
            ActivityLogger.Instance.SystemLog(NLog.LogLevel.Info, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), correlationId, "User", "machine name", this.GetType().Name, "User account created", 1);

            await _next(context);
		}
	}
}
