using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Models;
using Extension.Security.Interface;
using System.Security.Cryptography;
using System.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Common.Interface;
using Common.Model;
using Microsoft.EntityFrameworkCore;
using Common.Implementation;

namespace Extension.Security.Implementation
{
    public class UserManager : IUserManager
    {
		public bool TestConnection(string header, string tranMessage)
        {
            using (TiktokishContext context = new("Data Source=AS-BSD-RAZAMER\\RAZAMEER;Initial Catalog=Tiktokish;Persist Security Info=True;User ID=sa;Password=avanza@123;"))
            {
                var users = context.UserInfos.ToList();
                foreach (var user in users)
                {
                    Console.WriteLine($"{user.Id} - {user.UserName}");
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
				using (TiktokishContext context = new("Data Source=AS-BSD-RAZAMER\\RAZAMEER;Initial Catalog=Tiktokish;Persist Security Info=True;User ID=vision;Password=avanza@123;"))
				{
					//var users = context.UserInfos.ToList();
					var user = context.Users
					.FirstOrDefault(u =>
						u.Username == request.Identifier ||
						u.Email == request.Identifier ||
						u.PhoneNumber == request.Identifier);

					if (user == null || !user.IsActive)
					{
						response.Message = "Invalid login credentials.";
					}
					else
					{
						response.Data = new LoginResponse();
						bool passwordValid = true;// BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
						if (!passwordValid)
						{
							response.Message = "Invalid login credentials.";
						}

						if (token == null || token == "")
						{
							response.Data.Token = "asda";
							//response.Data.Token = GenerateJwtToken(user.Username);
						}

						response.Status = true;
						response.Message = "Success";
						response.Data.Username = user.Username;
						response.Data.IsVerified = user.IsVerified;
					}
					//response.Data. = DateTime.UtcNow;
					//context.SaveChanges();
				}
			}
			catch (Exception ex)
			{
				ActivityLogger.Instance.SystemLog(NLog.LogLevel.Error, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), request.correlationId, request.Identifier, "machine name", this.GetType().Name, "Login", 0, ex);
				response.Message += ex.Message;
				response.Status = false;
			}

			return response;

			//var res = new LoginResponse
			//{ Token = request.Token, Username = "Response from Extension" };
			//return res;

		}

		public ResponseModel<LoginResponse> AddUser(LoginRequest request, ResponseModel<LoginResponse> response)
		{
			try
			{
				ActivityLogger.Instance.SystemLog(NLog.LogLevel.Info, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), request.correlationId, request.Identifier, "machine name", this.GetType().Name, "Login", 1);

				string token = request.Token;
				response.Data = new LoginResponse();
				using (TiktokishContext context = new("Data Source=AS-BSD-RAZAMER\\RAZAMEER;Initial Catalog=Tiktokish;Persist Security Info=True;User ID=vision;Password=avanza@123;"))
				{
					var user = context.Users
					.FirstOrDefault(u =>
						u.Username == request.Identifier ||
						u.Email == request.Identifier ||
						u.PhoneNumber == request.Identifier);

					if (user == null)
					{
						User newUser = new User();
						newUser.Username = request.Identifier;
						newUser.Email = request.Identifier;
						newUser.PhoneNumber = request.Identifier;
						newUser.RealName = request.Identifier;
						newUser.PasswordHash = "";
						newUser.AvatarUrl = "";
						newUser.Bio = "";
						newUser.IsActive = true;
						newUser.CreatedAt = DateTime.Now;
						newUser.UpdatedAt = DateTime.Now;
						newUser.IsVerified = false;
						newUser.Role = "User";
						newUser.LastLoginAt = DateTime.Now;
						newUser.Locale = "";
						newUser.DevicePreference = "";
						newUser.EcomStatus = "User";

						context.Add(newUser);
						context.SaveChanges();

						if (token == null || token == "")
						{
							response.Data.Token = "asda";
							//response.Data.Token = GenerateJwtToken(user.Username);
						}
						response.Status = true;
						response.Message = "User successfully created";
						response.Data.Username = request.Identifier;
						response.Data.IsVerified = newUser.IsVerified;

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
				ActivityLogger.Instance.SystemLog(NLog.LogLevel.Error, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), request.correlationId, request.Identifier, "machine name", this.GetType().Name, "Login", 0, ex);
				response.Message += ex.Message;
				response.Status = false;
			}
			return response;
		}

		public byte[] ComputeHash(string input)
		{
			string hashSalt = "SYMMETRY";
			input = string.Concat(input, hashSalt);
			byte[] byteData = [];//System.Security.Cryptography.Encoding.GetBytes(input);
			byte[] result;
			string hashedResult = string.Empty;
			SHA512 hashCalculator = new SHA512Managed();
			result = hashCalculator.ComputeHash(byteData);

			//hashedResult = Encoding.GetString(result);
			//if (hashedResult.Contains("'"))
			//    hashedResult = hashedResult.Replace("'", "");
			//if (hashedResult.Contains("\0"))
			//    hashedResult = hashedResult.Replace("\0", "");
			//hashedResult = Regex.Escape(hashedResult);
			//return hashedResult;
			return result;
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

			var tokena= new JwtSecurityTokenHandler().WriteToken(token);

			return tokena;
		}
	}
}
