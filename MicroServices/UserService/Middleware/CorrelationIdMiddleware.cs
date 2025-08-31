using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace UserService.Middleware
{
	public class CorrelationIdMiddleware
	{
		private const string CorrelationHeader = "X-Correlation-ID";
		private readonly RequestDelegate _next;

		public CorrelationIdMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			if (!context.Request.Headers.ContainsKey(CorrelationHeader))
			{
				context.Request.Headers[CorrelationHeader] = Guid.NewGuid().ToString();
			}

			context.Response.OnStarting(() =>
			{
				context.Response.Headers[CorrelationHeader] = context.Request.Headers[CorrelationHeader];
				return Task.CompletedTask;
			});

			await _next(context);
		}
	}
}
