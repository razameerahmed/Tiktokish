using APIGateway;
using Common;
using Common.Implementation;
using Common.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{

	private readonly IHttpClientFactory _httpClientFactory;
    private readonly IDistributedCache _cache;
	private readonly TimeSpan _cacheExpiry = TimeSpan.FromMinutes(5);
	private readonly IHTTPHelper _httpHelper;
	private readonly IHTTPHelper _httpAuthHelper;
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly HttpClient _httpClient;

	public AuthController(IHttpClientFactory httpClientFactory, IDistributedCache cache, Func<string, IHTTPHelper> httpHelperFactory, IHttpContextAccessor httpContextAccessor, HttpClient httpClient)
	{
		_httpClientFactory = httpClientFactory;
        _cache = cache;
		_httpHelper = httpHelperFactory("https://localhost:44323");
		_httpAuthHelper = httpHelperFactory("https://localhost:44333");
		_httpContextAccessor = httpContextAccessor;
		_httpClient = httpClient;

	}

	[HttpPost("logincache")]
    public async Task<UserLogin> LoginFromCache([FromBody] APIGateway.UserLogin user)
    {
        string cacheKey = "";
        string correlationId = new Guid().ToString();

        try
        {
            ActivityLogger.Instance.SystemLog(NLog.LogLevel.Info, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), correlationId, "User", "machine name", this.GetType().Name, "User account created", 1);

            _ = _httpHelper.GetAsync("userservice");

            cacheKey = $"Item_{user.Username}";

            // Step 1: Try to get the item from Redis cache
            var cachedItem = await _cache.GetStringAsync(cacheKey);
            if (cachedItem != null)
            {
                Console.WriteLine("✅ Item retrieved from cache!");
                return JsonConvert.DeserializeObject<UserLogin>(cachedItem);
            }
        }
        catch (Exception ex)
        {
            ActivityLogger.Instance.SystemLog(NLog.LogLevel.Info, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), correlationId, "User", "machine name", this.GetType().Name, ex.Message, 0, ex);
            throw; // Fixed CA2200: Use 'throw;' to preserve stack trace
        }

        // Step 2: Simulate database fetch (or real DB call in a production app)
        var item = await FetchItemFromDatabase(user.Username);

        // Step 3: Cache the item in Redis
        await _cache.SetStringAsync(
            cacheKey,
            JsonConvert.SerializeObject(item),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = _cacheExpiry }
        );

        return item;
    }

	private Task<UserLogin> FetchItemFromDatabase(string userName)
	{
		// Simulating a databasUserLogine call
		return Task.FromResult(new UserLogin { Username = "Khalid", Password = "Abc123" });
	}

	[HttpPost("login")]
	public async Task<IActionResult> Login([FromBody] APIGateway.UserLogin user)
	{
		var correlationId = _httpContextAccessor.HttpContext.Request.Headers["X-Correlation-ID"].ToString();

		_httpClient.DefaultRequestHeaders.Add("X-Correlation-ID", correlationId);

		var client = _httpClientFactory.CreateClient("UserService");

		var response = await client.GetAsync("WeatherForecast");

		if (!response.IsSuccessStatusCode)
		{
			var errorText = await response.Content.ReadAsStringAsync();
			return StatusCode((int)response.StatusCode, new
			{
				ok = false,
				from = "Gateway",
				downstreamStatus = response.StatusCode,
				downstreamBody = errorText
			});
		}

		var json = await response.Content.ReadAsStringAsync();
		return Content(json, "application/json");

		//if (user.Username == "admin" && user.Password == "password123456")
		//{
		//	var token = GenerateJwtToken(user.Username);
		//	return Ok(new { token });
		//}
		//return Unauthorized();
	}

	private string GenerateJwtToken(string username)
	{
		var claims = new[]
		{
			new Claim(JwtRegisteredClaimNames.Sub, username),
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
		};

		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_super_secret_key_1111111111"));
		var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		var token = new JwtSecurityToken(
			issuer: "yourdomain.com",
			audience: "yourdomain.com",
			claims: claims,
			expires: DateTime.Now.AddMinutes(30),
			signingCredentials: creds);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}
}