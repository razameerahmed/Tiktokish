using Microsoft.AspNetCore.Http;
using NLog;
using System.Threading.Tasks;
using ILogger = NLog.ILogger;

namespace APIGateway.Middleware
{
	public class LoggingCorrelationMiddleware
	{
		private readonly RequestDelegate _next;
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

		public LoggingCorrelationMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			var correlationId = context.Request.Headers["X-Correlation-ID"].ToString();
			MappedDiagnosticsLogicalContext.Set("CorrelationId", correlationId);

			//Logger.Info($"Handling request {context.Request.Path}");
			await _next(context);
			//Logger.Info($"Finished request {context.Request.Path}");
		}
	}
}
