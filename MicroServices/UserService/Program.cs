using Common.Implementation;
using Common.Interface;
using Extension.Security.Implementation;
using Extension.Security.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using System;
using UserService.Middleware;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
try
{
	var builder = WebApplication.CreateBuilder(args);

    ActivityLogger.Instance.SystemLog(NLog.LogLevel.Info, "User Service Starting", ActionType.View.ToString(), "", "User", "machine name", "", "User account created", 1);

    // Add services (like DI, controllers, DB, etc.)
    builder.Services.AddControllers();

	// NLog: setup NLog for Dependency injection
	builder.Logging.ClearProviders();
	builder.Host.UseNLog();

	// Add services to the container.
	builder.Services.AddScoped<IUserManager, UserManager>();

	var app = builder.Build();


	app.UseMiddleware<CorrelationIdMiddleware>();
	app.UseMiddleware<LoggingCorrelationMiddleware>();

	// Middleware pipeline
	if (app.Environment.IsDevelopment())
	{
		app.UseDeveloperExceptionPage();
	}

	app.UseHttpsRedirection();

	app.UseAuthorization();

	app.MapControllers();
	app.UseHttpsRedirection();
	app.Run();
}
catch (Exception ex)
{
    ActivityLogger.Instance.SystemLog(NLog.LogLevel.Info, "Stopped program because of exception", ActionType.View.ToString(), "", "User", "machine name", "", ex.Message, 0, ex);
    throw;
}
finally
{
	NLog.LogManager.Shutdown();
}
