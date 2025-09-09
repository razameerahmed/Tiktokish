using System.Text;
using DataAccessLayer.Models;
using Extension.Security.Interface;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Common.Interface;
using Common.Model;
using Microsoft.EntityFrameworkCore;
using Common.Implementation;
using Microsoft.Extensions.Configuration;

namespace Extension.Security.Implementation
{
    public class UserManager : IUserManager
    {
		private readonly string _connectionString;
		private INotificationManager _notificationManager;
        public UserManager(IConfiguration configuration, INotificationManager notificationManager)
        {
            var connStr = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connStr))
                throw new InvalidOperationException("Connection string 'DefaultConnection' is missing or null.");
            _connectionString = connStr;
			_notificationManager = notificationManager;

        }
		public bool TestConnection(string header, string tranMessage)
        {
            using (TiktokishContext context = new("Data Source=AS-BSD-RAZAMER\\RAZAMEER;Initial Catalog=Tiktokish;Persist Security Info=True;User ID=sa;Password=avanza@123;"))
            {
                var users = context.Users.ToList();
                foreach (var user in users)
                {
                    Console.WriteLine($"{user.Id} - {user.Username}");
                }
            }

            return true;
        }
		public ResponseModel<LoginResponse> ValidateLogin(LoginRequest request, ResponseModel<LoginResponse> response)
		{			
			try
			{
				ActivityLogger.Instance.SystemLog(NLog.LogLevel.Info, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), request.correlationId, request.Identifier, "machine name", this.GetType().Name, "Login", 1);
				string token = request.Token;
				using (TiktokishContext context = new(_connectionString))
				{
					//var users = context.UserInfos.ToList();
					var user = context.Users
					.FirstOrDefault(u =>
						u.Username == request.Identifier ||
						u.Email == request.Identifier );

					if (user == null || !user.Isactive)
					{
						response.Message = "Invalid login credentials.";
						response.Status = false;
					}
					else
					{
						response.Status = true;
						response.Message = "Success";
						response.Data = new LoginResponse();
						bool passwordValid = ComputeHash(user.Username+request.Password) == user.Passwordhash;
						if (!passwordValid)
						{
							response.Message = "Invalid Password.";
							response.Status = false;
						}
						else if (token == null || token == "")
						{
							response.Data.Token = GenerateJwtToken(user.Username);
						}
						
						response.Data.Username = user.Username;
						response.Data.IsVerified = user.Isverified;
					}
				}
			}
			catch (Exception ex)
			{
				ActivityLogger.Instance.SystemLog(NLog.LogLevel.Error, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), request.correlationId, request.Identifier, "machine name", this.GetType().Name, ex.Message, 0, ex);
				response.Message += ex.Message;
				response.Status = false;
			}

			return response;
		}

		public ResponseModel<LoginResponse> AddUser(LoginRequest request, ResponseModel<LoginResponse> response)
		{
			try
			{
				ActivityLogger.Instance.SystemLog(NLog.LogLevel.Info, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), request.correlationId, request.Identifier, "machine name", this.GetType().Name, "Login", 1);

				string token = request.Token;
				response.Data = new LoginResponse();
				using (TiktokishContext context = new(_connectionString))
				{
					var user = context.Users
					.FirstOrDefault(u =>
						u.Username == request.Identifier ||
						u.Email == request.Identifier ||
						u.Phonenumber == request.Identifier);

					if (user == null)
					{
						User newUser = new();
						newUser.Username = request.Identifier;
						newUser.Email = request.Identifier;
						newUser.Phonenumber = request.Identifier;
						newUser.Fullname = request.Identifier;
						newUser.Passwordhash = ComputeHash(request.Identifier+request.Password);
						newUser.Avatarurl = "";
						newUser.Biometric = "";
						newUser.Isactive = true;
						newUser.Createdat = DateTime.Now;
						newUser.Updatedat = DateTime.Now;
						newUser.Isverified = false;
						newUser.Role = "User";
						newUser.Lastloginat = DateTime.Now;
						newUser.Locale = "";
						newUser.Devicetype = "";

						context.Add(newUser);
						context.SaveChanges();

						if (token == null || token == "")
						{
							response.Data.Token = GenerateJwtToken(request.Identifier);
						}
						response.Status = true;
						response.Message = "User successfully created";
						response.Data.Username = request.Identifier;
						response.Data.IsVerified = newUser.Isverified;
					}
					else
					{
						response.Message = "User already exists";
						response.Data = null;
					}
					
				}
			}
			catch (Exception ex)
			{
				ActivityLogger.Instance.SystemLog(NLog.LogLevel.Error, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), request.correlationId, request.Identifier, "machine name", this.GetType().Name, ex.Message, 0, ex);
				response.Message += ex.Message;
				response.Status = false;
			}
			return response;
		}
		public ResponseModel<LoginResponse> ValidateUserForAdd(CommonUser request, ResponseModel<LoginResponse> response)
		{
			try
			{
				ActivityLogger.Instance.SystemLog(NLog.LogLevel.Info, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), request.CorrelationId, request.Username, "machine name", this.GetType().Name, "Add User", 1);

				response.Status = false;
				using (TiktokishContext context = new(_connectionString))
				{
					var user = context.Users
					.FirstOrDefault(u =>
						u.Username == request.Username ||
						u.Email == request.Username ||
						u.Phonenumber == request.Username);

					if (user != null)
					{
						response.Message = "Username is available";
						response.Status = true;
					}
                    response.Message = "Username is not available";
                }
			}
			catch (Exception ex) {
				ActivityLogger.Instance.SystemLog(NLog.LogLevel.Error, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), request.CorrelationId, request.Username, "machine name", this.GetType().Name, ex.Message, 0, ex);
				response.Message += ex.Message;
				response.Status = false;
			}		
		
			return response;
		}
		public ResponseModel<LoginResponse> UpdateUser(CommonUser request, ResponseModel<LoginResponse> response)
		{
			try
			{
				ActivityLogger.Instance.SystemLog(NLog.LogLevel.Info, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), request.CorrelationId, request.Username, "machine name", this.GetType().Name, "Login", 1);

				string token = request.Token;
                response.Data = new LoginResponse();
                response.Status = false;
                using (TiktokishContext context = new(_connectionString))
				{
					var user = context.Users
					.FirstOrDefault(u =>
						u.Username == request.Username ||
						u.Email == request.Username ||
						u.Phonenumber == request.Username);

					if (user == null || !user.Isactive)
					{
						response.Message = "Invalid login credentials.";
					}
					else
					{
						user.Phonenumber = request.Phonenumber == null ? user.Phonenumber : request.Phonenumber;
						user.Fullname = request.Fullname == null ? user.Fullname : request.Fullname;
						user.Avatarurl = request.Avatarurl == null ? user.Avatarurl : request.Avatarurl;
						user.Biometric = request.Biometric == null ? user.Biometric : request.Biometric;
						user.Isactive = request.Isactive == user.Isactive ? user.Isactive : request.Isactive;
						user.Updatedat = DateTime.Now;
						user.Isverified = request.Isverified == user.Isverified ? user.Isverified : request.Isverified;
						user.Role = request.Role == null ? user.Role : request.Role;
						user.Locale = request.Locale == null ? user.Locale : request.Locale;
						user.Devicetype = request.Devicetype == null ? user.Devicetype : request.Devicetype;

						string hashedPassword = ComputeHash(user.Username + request.Password);

                        if (hashedPassword != user.Passwordhash)
						{
							user.Passwordhash = hashedPassword;
						}

						context.Update(user);
                        context.SaveChanges();

                        if (token == null || token == "")
						{
							response.Data.Token = GenerateJwtToken(user.Username);
						}

						response.Status = true;
						response.Message = "Success";
						response.Data.Username = user.Username;
						response.Data.IsVerified = user.Isverified;
					}
				}
			}
			catch (Exception ex)
			{
				ActivityLogger.Instance.SystemLog(NLog.LogLevel.Error, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), request.CorrelationId, request.Username, "machine name", this.GetType().Name, ex.Message, 0, ex);
				response.Message += ex.Message;
			}
			ActivityLogger.Instance.SystemLog(NLog.LogLevel.Info, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), request.CorrelationId, request.Username, "machine name", this.GetType().Name, response.Message, 1);
			return response;
		}

		public string RefreshToken(string userName, string token,string requestDomain) {
			if (ValidateToken(token, userName, requestDomain))
			{
				return GenerateJwtToken(userName);
			}
			else {
				return "Username or domain mismatch.";
			}
		}

		public string ComputeHash(string input)
		{
			string hashSalt = GlobalConfiguration.HashSalt;
			input = string.Concat(input, hashSalt);
			byte[] byteData = [];//System.Security.Cryptography.Encoding.GetBytes(input);
			string result;
			string hashedResult = string.Empty;
			SHA512 hashCalculator = new SHA512Managed();
			result = Convert.ToBase64String(hashCalculator.ComputeHash(byteData));

			//hashedResult = Encoding.GetString(result);
			//if (hashedResult.Contains("'"))
			//    hashedResult = hashedResult.Replace("'", "");
			//if (hashedResult.Contains("\0"))
			//    hashedResult = hashedResult.Replace("\0", "");
			//hashedResult = Regex.Escape(hashedResult);
			//return hashedResult;
			return result;
		}

		public string GenerateJwtToken(string username)
		{
			var claims = new[]
			{
			new Claim(JwtRegisteredClaimNames.Sub, username),
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
		};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GlobalConfiguration.TokenSecretKey));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: GlobalConfiguration.TokenIssuer,
				audience: GlobalConfiguration.TokenAudience,
				claims: claims,
				expires: DateTime.Now.AddMinutes(30),
				signingCredentials: creds);

			var tokena= new JwtSecurityTokenHandler().WriteToken(token);

			return tokena;
		}

		public static bool ValidateToken(string token, string requestUsername, string requestDomain)
		{
			var tokenHandler = new JwtSecurityTokenHandler();

			var key = Encoding.UTF8.GetBytes(GlobalConfiguration.TokenSecretKey); // or Encoding.UTF8.GetBytes for raw keys
			
			var validationParameters = new TokenValidationParameters
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(key),

				ValidateIssuer = true,
				ValidIssuer = GlobalConfiguration.TokenIssuer,

				ValidateAudience = true,
				ValidAudience = GlobalConfiguration.TokenAudience,

				ValidateLifetime = true,
				ClockSkew = TimeSpan.FromMinutes(5)
			};

			try
			{
				var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

				var username = principal.Claims.First().Value;
				var domain = principal.Claims.First().Issuer;

				if (username == requestUsername && domain == requestDomain)
				{
					//Console.WriteLine("✅ Token is valid with correct username and domain.");
					return true;
				}
				else
				{
					return false;
					//Console.WriteLine("Username or domain mismatch.");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Token validation failed: {ex.Message}");
			}
			return false; 
		}
	

}
}
