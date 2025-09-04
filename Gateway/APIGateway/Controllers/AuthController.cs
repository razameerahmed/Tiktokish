using Common;
using Common.Implementation;
using Common.Interface;
using Common.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{

	private readonly IHttpClientFactory _httpClientFactory;
	private readonly IDistributedCache _cache;
	private readonly TimeSpan _cacheExpiry = TimeSpan.FromMinutes(5);
	private readonly IHTTPHelper _httpHelper;
	//private readonly IHTTPHelper _httpAuthHelper;
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly HttpClient _httpClient;
	private readonly IConfiguration _configuration;

	public AuthController(IHttpClientFactory httpClientFactory, IDistributedCache cache, Func<string, IHTTPHelper> httpHelperFactory, IHttpContextAccessor httpContextAccessor, HttpClient httpClient, IConfiguration configuration)
	{
		_httpClientFactory = httpClientFactory;
		_cache = cache;
		_httpHelper = httpHelperFactory("https://localhost:44323");
		//_httpAuthHelper = httpHelperFactory("https://localhost:44333");
		_httpContextAccessor = httpContextAccessor;
		_httpClient = httpClient;
		_configuration = configuration;
	}

	[HttpPost("logincache")]
	public async Task<IActionResult> LoginFromCache([FromBody] UserLogin user)
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
				return Ok(cachedItem);
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

		return Ok(item);
	}

	private Task<UserLogin> FetchItemFromDatabase(string userName)
	{
		// Simulating a databasUserLogine call
		return Task.FromResult(new UserLogin { Username = "Khalid", Password = "Abc123" });
	}

	[HttpPost("login")]
	public async Task<IActionResult> Login([FromBody] UserLogin user)
	{
		//var userServiceUrl =_configuration["Services:UserService"]; // e.g. "https://localhost:5002"

		var correlationId = HttpContext.Request.Headers["X-Correlation-ID"].ToString();


		var request = new LoginRequest
		{
			Identifier = user.Username,
			Password = user.Password,
			correlationId = correlationId,
			Token = ""
		};
		try
		{
			var response = await _httpHelper.PostJsonAsync("/userservice/validatelogin", request);
			ActivityLogger.Instance.SystemLog(NLog.LogLevel.Info, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), correlationId, request.Identifier, "machine name", this.GetType().Name, "User logged in", 1);

			//if (!response)
			//{
			//	var error = await response..Content.ReadAsStringAsync();
			//	return StatusCode((int)response.StatusCode, error);
			//}
			//var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
			//return Ok(loginResponse);


			return Ok(
				 response);

		}
		catch (Exception ex)
		{
			ActivityLogger.Instance.SystemLog(NLog.LogLevel.Error, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), correlationId, request.Identifier, "machine name", this.GetType().Name, ex.Message, 0, ex);
			return BadRequest("Message" + ex);
		}



		//var correlationId = _httpContextAccessor.HttpContext.Request.Headers["X-Correlation-ID"].ToString();

		//_httpClient.DefaultRequestHeaders.Add("X-Correlation-ID", correlationId);

		//var client = _httpClientFactory.CreateClient("UserService");

		//var response = await client.GetAsync("UserService/Login");

		//if (!response.IsSuccessStatusCode)
		//{
		//	var errorText = await response.Content.ReadAsStringAsync();
		//	return StatusCode((int)response.StatusCode, new
		//	{
		//		ok = false,
		//		from = "Gateway",
		//		downstreamStatus = response.StatusCode,
		//		downstreamBody = errorText
		//	});
		//}

		//var json = await response.Content.ReadAsStringAsync();
		//return Content(json, "application/json");

		//if (user.Username == "admin" && user.Password == "password123456")
		//{
		//	var token = GenerateJwtToken(user.Username);
		//	return Ok(new { token });
		//}
		//return Unauthorized();
	}

	[HttpPost("createuseraccount")]
	public async Task<IActionResult> CreateUserAccount([FromBody] UserLogin user)
	{
		var correlationId = HttpContext.Request.Headers["X-Correlation-ID"].ToString();
		var request = new LoginRequest
		{
			Identifier = user.Username,
			Password = user.Password,
			correlationId = correlationId,
			Token = ""
		};
		try
		{
			var response = await _httpHelper.PostJsonAsync("/userservice/createuseraccount", request);
			ActivityLogger.Instance.SystemLog(NLog.LogLevel.Info, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), correlationId, request.Identifier, "machine name", this.GetType().Name, "User account created", 1);
			return Ok(
				 response);
		}
		catch (Exception ex)
		{
			ActivityLogger.Instance.SystemLog(NLog.LogLevel.Error, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), correlationId, request.Identifier, "machine name", this.GetType().Name, ex.Message, 0, ex);
			return BadRequest("Message" + ex);
		}
	}

	[HttpPost("edituser")]
	public async Task<IActionResult> EditUser(CommonUser user)
	{
		var correlationId = HttpContext.Request.Headers["X-Correlation-ID"].ToString();
		var request = new CommonUser
		{
			Username = user.Username,
			Password = user.Password,
			CorrelationId = correlationId,
			Token = ""
		};
		try
		{
			var response = await _httpHelper.PostJsonAsync("/userservice/edituser", request);
			ActivityLogger.Instance.SystemLog(NLog.LogLevel.Info, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), correlationId, request.Username, "machine name", this.GetType().Name, "User Updated", 1);
			return Ok(
				 response);
		}
		catch (Exception ex)
		{
			ActivityLogger.Instance.SystemLog(NLog.LogLevel.Error, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), correlationId, request.Username, "machine name", this.GetType().Name, ex.Message, 0, ex);
			return BadRequest("Message" + ex);
		}
	}

	[HttpPost("validateusername")]
	public async Task<IActionResult> ValidateUsername([FromBody] CommonUser user)
	{
		var correlationId = HttpContext.Request.Headers["X-Correlation-ID"].ToString();
		var request = new CommonUser
		{
			Username = user.Username,
			Password = user.Password,
			CorrelationId = correlationId,
			Token = ""
		};
		try
		{
			var response = await _httpHelper.PostJsonAsync("/userservice/validateusername", request);
			ActivityLogger.Instance.SystemLog(NLog.LogLevel.Info, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), correlationId, request.Username, "machine name", this.GetType().Name, "User Updated", 1);
			return Ok(
				 response);
		}
		catch (Exception ex)
		{
			ActivityLogger.Instance.SystemLog(NLog.LogLevel.Error, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), correlationId, request.Username, "machine name", this.GetType().Name, ex.Message, 0, ex);
			return BadRequest("Message" + ex);
		}
	}

	//private string GenerateJwtToken(string username)
	//{
	//	var claims = new[]
	//	{
	//		new Claim(JwtRegisteredClaimNames.Sub, username),
	//		new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
	//	};

	//	var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_super_secret_key_1111111111"));
	//	var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

	//	var token = new JwtSecurityToken(
	//		issuer: "yourdomain.com",
	//		audience: "yourdomain.com",
	//		claims: claims,
	//		expires: DateTime.Now.AddMinutes(30),
	//		signingCredentials: creds);

	//	return new JwtSecurityTokenHandler().WriteToken(token);
	//}
}