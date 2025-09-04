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
using Microsoft.Extensions.Configuration;
using System.Reflection.Metadata.Ecma335;

namespace Extension.Security.Implementation
{
    public class UserManager : IUserManager
    {
		private readonly string _connectionString;

		public UserManager(IConfiguration configuration)
		{
			_connectionString = configuration.GetConnectionString("DefaultConnection");
		}
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
				using (TiktokishContext context = new(_connectionString))
				{
					//var users = context.UserInfos.ToList();
					var user = context.Users
					.FirstOrDefault(u =>
						u.Username == request.Identifier ||
						u.Email == request.Identifier );

					if (user == null || !user.IsActive)
					{
						response.Message = "Invalid login credentials.";
						response.Status = false;
					}
					else
					{
						response.Status = true;
						response.Message = "Success";
						response.Data = new LoginResponse();
						bool passwordValid = ComputeHash(user.Username+request.Password) == user.PasswordHash;
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
						response.Data.IsVerified = user.IsVerified;
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
						u.PhoneNumber == request.Identifier);

					if (user == null)
					{
						User newUser = new();
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

				response.Status = true;
				using (TiktokishContext context = new(_connectionString))
				{
					var user = context.Users
					.FirstOrDefault(u =>
						u.Username == request.Username ||
						u.Email == request.Username ||
						u.PhoneNumber == request.Username);

					if (user != null)
					{
						response.Message = "User Name is not available";
						response.Status = false;
					}
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
				using (TiktokishContext context = new(_connectionString))
				{
					//var users = context.UserInfos.ToList();
					var user = context.Users
					.FirstOrDefault(u =>
						u.Username == request.Username ||
						u.Email == request.Username ||
						u.PhoneNumber == request.Username);

					if (user == null || !user.IsActive)
					{
						response.Message = "Invalid login credentials.";
						response.Status = false;
					}
					else
					{
						user.PhoneNumber = request.PhoneNumber == null ? user.PhoneNumber : request.PhoneNumber;
						user.RealName = request.FullName == null ? user.RealName : request.FullName;
						//user.PasswordHash = request.PasswordHash == user.PasswordHash ? user.PasswordHash : request.PasswordHash;
						user.AvatarUrl = request.AvatarUrl == null ? user.AvatarUrl : request.AvatarUrl;
						user.Bio = request.Bio == null ? user.Bio : request.Bio;
						user.IsActive = request.IsActive == user.IsActive ? user.IsActive : request.IsActive;
						user.UpdatedAt = DateTime.Now;
						user.IsVerified = request.IsVerified == user.IsVerified ? user.IsVerified : request.IsVerified;
						user.Role = request.Role == null ? user.Role : request.Role;
						user.Locale = request.Locale == null ? user.Locale : request.Locale;
						user.DevicePreference = request.DevicePreference == null ? user.DevicePreference : request.DevicePreference;

						response.Data = new LoginResponse();
						//bool passwordValid = true;// BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
						if (request.PasswordHash != user.PasswordHash)
						{
							user.PasswordHash = "ComputeHash";//"Invalid login credentials.";
						}

						if (token == null || token == "")
						{
							response.Data.Token = GenerateJwtToken(user.Username);
						}

						response.Status = true;
						response.Message = "Success";
						response.Data.Username = user.Username;
						response.Data.IsVerified = user.IsVerified;
					}
				}
			}
			catch (Exception ex)
			{
				ActivityLogger.Instance.SystemLog(NLog.LogLevel.Error, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), request.CorrelationId, request.Username, "machine name", this.GetType().Name, ex.Message, 0, ex);
				response.Message += ex.Message;
				response.Status = false;
			}
			ActivityLogger.Instance.SystemLog(NLog.LogLevel.Info, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), request.CorrelationId, request.Username, "machine name", this.GetType().Name, response.Message, 1);
			return response;
		}


		public string ComputeHash(string input)
		{
			string hashSalt = "TIKTOKISH";
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
