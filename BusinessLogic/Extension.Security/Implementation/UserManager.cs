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

namespace Extension.Security.Implementation
{
    public class UserManager : IUserManager
    {
		public bool TestConnection(string header, string tranMessage)
        {
            using (TiktokishContext context = new("Data Source=AS-BSD-RAZAMER\\\\RAZAMEER;Initial Catalog=Tiktokish;Persist Security Info=True;User ID=sa;Password=avanza@123;"))
            {

                var users = context.UserInfos.ToList();
                foreach (var user in users)
                {
                    Console.WriteLine($"{user.Id} - {user.Username}");
                }
            }

            return true;
        }

		public bool TestConnection()
		{
			throw new NotImplementedException();
		}

		public LoginResponse ValidateLogin(LoginRequest request)
		{



			var res = new LoginResponse
			{ Token = request.Token, Username= "Response from Extension" };
			return res;


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

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
