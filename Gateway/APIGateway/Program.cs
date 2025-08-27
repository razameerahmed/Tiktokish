using Extension.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<UserManager, UserManager>();

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

builder.Services.AddAuthorization();

var app = builder.Build();

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