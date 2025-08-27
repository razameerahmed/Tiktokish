using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{

	private readonly IHttpClientFactory _httpClientFactory;

	public AuthController(IHttpClientFactory httpClientFactory)
	{
		_httpClientFactory = httpClientFactory;
	}


	[HttpPost("login")]
	public async Task<IActionResult> Login([FromBody] APIGateway.UserLogin user)
	{
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