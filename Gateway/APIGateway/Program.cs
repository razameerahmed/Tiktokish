using APIGateway.Middleware;
using Common;
using Common.Implementation;
using Common.Interface;
using Extension.Security.Implementation;
using Extension.Security.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NLog;
using NLog.Web;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{
    //logger.Debug("Starting Gateway API...");
    ActivityLogger.Instance.SystemLog(NLog.LogLevel.Info, "Starting Gateway API...", ActionType.View.ToString(), "", "User", "machine name","", "User account created", 1);



    var builder = WebApplication.CreateBuilder(args);

	// Add services to the container.
	builder.Services.AddScoped<IUserManager, UserManager>();

	builder.Logging.ClearProviders();
	builder.Host.UseNLog();


	builder.Services.AddControllers();
	builder.Services.AddEndpointsApiExplorer();
	builder.Services.AddSwaggerGen();

	builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
		.AddJwtBearer(options =>
		{

			////JWT TOKEN interrupt
			//options.Events = new JwtBearerEvents
			//{
			//	OnTokenValidated = async context =>
			//	{
			//		//var blacklistService = context.HttpContext.RequestServices.GetRequiredService<IJwtBlacklistService>();
			//		var token = context.SecurityToken;

			//		if (token != null )//&& blacklistService.IsBlacklisted(token.RawData))
			//		{
			//			context.Fail("Token is blacklisted.");
			//		}
			//		await Task.CompletedTask;
			//	}
			//};

			options.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				ValidIssuer = "yourdomain.com",
				ValidAudience = "yourdomain.com",
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_super_secret_key_1111111111"))
			};
		});

	builder.Services.AddHttpClient("UserService", (sp, client) =>
	{
		var config = sp.GetRequiredService<IConfiguration>();
		var baseUrl = config["Services:UserServiceBaseUrl"];
		client.BaseAddress = new Uri(baseUrl!);
		client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
	});


	builder.Services.AddStackExchangeRedisCache(options =>
	{
		options.Configuration = builder.Configuration.GetConnectionString("Redis");
		options.InstanceName = "MyRedisInstance";
	});

	builder.Services.AddStackExchangeRedisOutputCache(options =>
	{
		options.Configuration = builder.Configuration.GetConnectionString("Redis");
		options.InstanceName = "MyOutputCacheInstance";
	});

	builder.Services.AddOutputCache(options =>
	{
		options.AddBasePolicy(builder => builder.Expire(TimeSpan.FromSeconds(60))); // Example policy
	});

	// Services Initiated
	//builder.Services.AddScoped<IHTTPHelper, HTTPHelper>();
	builder.Services.AddScoped<Func<string, IHTTPHelper>>(sp => baseAddress =>
	{
		var factory = sp.GetRequiredService<IHttpClientFactory>();
		return new HTTPHelper(baseAddress, factory);
	});

	builder.Services.AddAuthorization();

	var app = builder.Build();


	app.UseMiddleware<CorrelationIdMiddleware>();
	app.UseMiddleware<LoggingCorrelationMiddleware>();

	// Configure the HTTP request pipeline.
	if (app.Environment.IsDevelopment())
	{
		app.UseSwagger();
		app.UseSwaggerUI();
	}
	app.UseHttpsRedirection();
	app.UseAuthentication();
	app.UseAuthorization();

	app.MapControllers();

	app.Run();
}
catch (Exception ex)
{
    //logger.Error(ex, "Stopped program because of exception");

    ActivityLogger.Instance.SystemLog(NLog.LogLevel.Info, "Stopped program because of exception", ActionType.View.ToString(), "", "User", "machine name", "", ex.Message, 0, ex);

    throw;
}
finally
{
	LogManager.Shutdown();
}