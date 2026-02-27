using Azure.Core;
using Common;
using Common.Implementation;
using Common.Interface;
using Common.Model;
using DataAccessLayer.Models;
using Extension.Security.Interface;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Extension.Security.Implementation
{
    public class UserManager : IUserManager
    {
        private readonly string _connectionString;
        private INotificationManager _notificationManager;
       // private IAuditLogHelper _auditLog;
        public UserManager(IConfiguration configuration, INotificationManager notificationManager)
        {
            var connStr = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connStr))
                throw new InvalidOperationException("Connection string 'DefaultConnection' is missing or null.");
            _connectionString = connStr;
            _notificationManager = notificationManager;
            //_auditLog = auditLog;
        }
        public bool TestConnection(string header, string tranMessage)
        {
            //    using (TrackerContext context = new("Data Source=AS-BSD-RAZAMER\\RAZAMEER;Initial Catalog=Tracker;Persist Security Info=True;User ID=sa;Password=avanza@123;"))
            //    {
            //        var users = context.Users.ToList();
            //        foreach (var user in users)
            //        {
            //            Console.WriteLine($"{user.Id} - {user.Username}");
            //        }
            //    }

            return true;
        }
        
        public ResponseModel<LoginResponse> ValidateLogin(Common.Model.LoginRequest request, ResponseModel<LoginResponse> response)
        {
            IAuditLogHelper _auditLog = new AuditLogHelper(_connectionString);
            try
            {
                ActivityLogger.Instance.SystemLog(NLog.LogLevel.Info, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), request.correlationId, request.Username, "machine name", this.GetType().Name, "Login", 1);
                //string token = request.Token;//IsJwtToken(request.Token) == true ? request.Token : null;

                using (TrackerContext context = new(_connectionString))
                {
                    //var users = context.UserInfos.ToList();
                    var user = context.Users
                    .FirstOrDefault(u =>
                        u.Username == request.Username ||
                        u.Email == request.Username);

                    if (user == null || !user.Isactive)
                    {
                        response.Message = "Invalid login credentials or user inactive.";
                        response.Status = false;
                    }
                    else
                    {
                        var userDevice = context.UserTrustedDevices
                    .FirstOrDefault(ud =>
                        ud.PkTrustedDeviceId == request.DeviceId &&
                        ud.Username == user.Username);

                        response.Status = true;
                        response.Message = "Success";
                        response.Data = new LoginResponse();
                        bool passwordValid = ComputeHash(user.Username + request.Password) == user.Passwordhash;
                        if (!passwordValid)
                        {
                            response.Message = "Invalid Password.";
                            response.Status = false;
                        }
                        //else if (string.IsNullOrWhiteSpace(token))
                        //{
                            response.Data.Token = GenerateJwtToken(user.Username);//RefreshToken(request);
                            _auditLog.AddAuditLog(user.Username, GlobalConfiguration.ActionTokenIssuer, GlobalConfiguration.UserServiceAPI, response.Data);
                        //}

                       //_auditLog.AddAuditLog(user.Username, GlobalConfiguration.ActionLoginSuccess, GlobalConfiguration.UserServiceAPI, newDevice);
                        if (userDevice == null)
                        {
                            // New device, add to trusted devices
                            UserTrustedDevice newDevice = new()
                            {
                                PkTrustedDeviceId = request.DeviceId,
                                Username = user.Username,
                                Blacklist = 0,
                                Bmv = "",
                                DeviceName = request.DeviceId,
                                DeviceCountryCode = request.CountryCode,
                                DeviceFirstSignIn = DateTime.Now,
                                Devicetype = "Android",
                                DeviceIp = "1.1.1.1",
                                IsAllowed = 1,
                                CreatedOn = DateTime.Now,
                                CreatedBy = "system",
                                UpdatedOn = DateTime.Now,
                                UpdatedBy = "system",
                            };

                            context.Add(newDevice);
                            context.SaveChanges();

                            _auditLog.AddAuditLog(user.Username, GlobalConfiguration.ActionAdd, GlobalConfiguration.UserServiceAPI, newDevice);
                        }

                        
                        response.Data.Username = user.Username;
                        response.Data.IsVerified = user.Isverified;

                        _auditLog.AddAuditLog(user.Username, GlobalConfiguration.ActionLoginSuccess, GlobalConfiguration.UserServiceAPI,response);
                        // Send Login Email
                        _notificationManager.NotifyUser(user.Username, user.Email);
                    }
                }

            }
            catch (Exception ex)
            {
                ActivityLogger.Instance.SystemLog(NLog.LogLevel.Error, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), request.correlationId, request.Username, "machine name", this.GetType().Name, ex.Message, 0, ex);
                response.Message = "Failed to validate login.";
                response.Status = false;
            }

            return response;
        }

		public ResponseModel<LoginResponse> ValidateOTP(Common.Model.LoginRequest request, ResponseModel<LoginResponse> response)
		{
			try
			{
				ActivityLogger.Instance.SystemLog(NLog.LogLevel.Info, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), request.correlationId, request.Username, "machine name", this.GetType().Name, "Login", 1);
				string token = IsJwtToken(request.Token) == true ? request.Token : null;

                _notificationManager.ValidateOTP(request.Username, request.EmailOTP, request.SMSOTP);

			}
			catch (Exception ex)
			{
				ActivityLogger.Instance.SystemLog(NLog.LogLevel.Error, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), request.correlationId, request.Username, "machine name", this.GetType().Name, ex.Message, 0, ex);
				response.Message += ex.Message;
				response.Status = false;
			}

			return response;
		}

		public ResponseModel<LoginResponse> AddUser(Common.Model.LoginRequest request, ResponseModel<LoginResponse> response)
        {
            try
            {
                ActivityLogger.Instance.SystemLog(NLog.LogLevel.Info, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), request.correlationId, request.Username, "machine name", this.GetType().Name, "Login", 1);

                List<string> errors = ValidateRegistration(request);

                if (errors.Any())
                {
                    response.Message = string.Join("; ", errors);
                    response.Status = false;
                    return response;
                }

                string token = request.Token;
                response.Data = new LoginResponse();
                using (TrackerContext context = new(_connectionString))
                {
                    var user = context.Users
                    .FirstOrDefault(u =>
                        u.Username == request.Username ||
                        u.Email == request.Email ||
                        u.Phonenumber == request.PhoneNumber);

                    if (user == null)
                    {
                        User newUser = new();
                        newUser.Username = request.Username;
                        newUser.Email = request.Email;
                        newUser.Phonenumber = request.PhoneNumber;
                        newUser.Fullname = request.FullName;
                        newUser.Passwordhash = ComputeHash(request.Username + request.Password);
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
                            response.Data.Token = GenerateJwtToken(request.Username);
                        }
                        response.Status = true;
                        response.Message = "User successfully created";
                        response.Data.Username = request.Username;
                        response.Data.IsVerified = newUser.Isverified;
                    }
                    else
                    {
                        response.Message = "Username, email or Phone number already exists, please use a unique value";
                        response.Data = null;
                    }

                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Instance.SystemLog(NLog.LogLevel.Error, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), request.correlationId, request.Username, "machine name", this.GetType().Name, ex.Message, 0, ex);
                response.Message = "Failed to add user.";
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
                List<string> errors = new List<string>();

                // 1. Username validation
                if (string.IsNullOrWhiteSpace(request.Username))
                    errors.Add("Username is required.");
                else if (!Regex.IsMatch(request.Username, @"^[a-z0-9_.]{3,50}$"))
                    errors.Add("Username must be 3-50 characters long and only contain lowercase letters, numbers, underscores, or dots.");

                if (errors.Any())
                {
                    response.Message = string.Join("; ", errors);
                    response.Status = false;
                    return response;
                }

                using (TrackerContext context = new(_connectionString))
                {
                    var user = context.Users
                    .FirstOrDefault(u =>
                        u.Username == request.Username);

                    if (user == null)
                    {
                        response.Message = "Username is available";
                        response.Status = true;
                    }
                    else
                    {
                        response.Message = "Username is not available";
                    }
                        
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Instance.SystemLog(NLog.LogLevel.Error, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), request.CorrelationId, request.Username, "machine name", this.GetType().Name, ex.Message, 0, ex);
                response.Message = "Failed to validate username.";
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
                using (TrackerContext context = new(_connectionString))
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
                response.Message = "Failed to update user.";
            }
            ActivityLogger.Instance.SystemLog(NLog.LogLevel.Info, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), request.CorrelationId, request.Username, "machine name", this.GetType().Name, response.Message, 1);
            return response;
        }

        public ResponseModel<List<Feed>> GetFeed(CommonUser request, ResponseModel<List<Feed>> response)
        {
            IAuditLogHelper _auditLog = new AuditLogHelper(_connectionString);
            //var response = new ResponseModel<LoginResponse>();
            try
            {
                ActivityLogger.Instance.SystemLog(NLog.LogLevel.Info, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), request.CorrelationId, request.Username, "machine name", this.GetType().Name, "Login", 1);
                response.Status = false;
                using (TrackerContext context = new(_connectionString))
                {

                    var result = context.Feeds
               .Where(f => f.IsActive == true)               // Only active feeds
               .OrderByDescending(f => f.CreatedAt)  // Latest first
               .ToList();

                    //var result = (from feed in context.Feeds select feed).ToList();
                   
                    
                    response.Status = true;
                    response.Message = "Success";
                    //response.Data = new LoginResponse();
                    response.Data = result;
                    _auditLog.AddAuditLog(request.Username, GlobalConfiguration.ActionSearch, GlobalConfiguration.UserServiceAPI, response.Data);
                }
                   


            }
            catch (Exception ex)
            {
                ActivityLogger.Instance.SystemLog(NLog.LogLevel.Error, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), request.CorrelationId, request.Username, "machine name", this.GetType().Name, ex.Message, 0, ex);
                response.Message = "Failed to retrieve feed.";
                response.Status = false;
            }
                
            return response;
        }

        public string RefreshToken(Common.Model.LoginRequest request)
        {
            if (ValidateToken(request))
            {
                return GenerateJwtToken(request.Username);
            }
            else
            {
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

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Fix for CS0026: Remove 'this' from static method context and use GlobalConfiguration.TokenIssuer instead
        public static bool ValidateToken(Common.Model.LoginRequest request)
        {
            ActivityLogger.Instance.SystemLog(NLog.LogLevel.Info, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), request.correlationId, request.Username, "machine name", "", "Add User", 1);

            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.UTF8.GetBytes(GlobalConfiguration.TokenSecretKey);

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
                var principal = tokenHandler.ValidateToken(request.Token, validationParameters, out SecurityToken validatedToken);

                var username = principal.Claims.First().Value;
                var domain = principal.Claims.First().Issuer;

                if (username == request.Username && domain == GlobalConfiguration.TokenIssuer)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Instance.SystemLog(NLog.LogLevel.Error, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), request.correlationId, request.Username, "machine name", "", "Token validation failed: " + ex.Message, 0, ex);
            }
            return false;
        }

        public static List<string> ValidateRegistration(Common.Model.LoginRequest request)
        {
            var errors = new List<string>();

            // 1. Username validation
            if (string.IsNullOrWhiteSpace(request.Username))
                errors.Add("Username is required.");
            else if (!Regex.IsMatch(request.Username, @"^[a-z0-9_.]{3,50}$"))
                errors.Add("Username must be 3-50 characters long and only contain lowercase letters, numbers, underscores, or dots.");

            // 2. Password validation
            if (string.IsNullOrWhiteSpace(request.Password))
                errors.Add("Password is required.");
            else if (request.Password.Length < 8 ||
                     !Regex.IsMatch(request.Password, @"[A-Z]") ||
                     !Regex.IsMatch(request.Password, @"[a-z]") ||
                     !Regex.IsMatch(request.Password, @"[0-9]") ||
                     !Regex.IsMatch(request.Password, @"[\W_]"))
                errors.Add("Password must be at least 8 characters and include uppercase, lowercase, number, and special character.");

            // 3. Phone validation
            if (!string.IsNullOrWhiteSpace(request.PhoneNumber) &&
                !Regex.IsMatch(request.PhoneNumber, @"^\+?[0-9]{10,15}$"))
                errors.Add("Phone number must be valid and include country code.");

            // 4. Email validation
            if (!string.IsNullOrWhiteSpace(request.Email) &&
                !Regex.IsMatch(request.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                errors.Add("Email format is invalid.");

            // 5. Date of Birth validation (TikTok style 13+ age)
            var age = DateTime.Today.Year - request.DateOfBirth.Year;
            if (request.DateOfBirth > DateTime.Today.AddYears(-age)) age--;
            if (age < 13)
                errors.Add("User must be at least 13 years old.");

            // 6. Gender validation
            var validGenders = new[] { "Male", "Female", "Other" };
            if (!string.IsNullOrWhiteSpace(request.Gender) &&
                Array.IndexOf(validGenders, request.Gender) == -1)
                errors.Add("Gender must be Male, Female, or Other.");

            // 7. DeviceId validation
            if (string.IsNullOrWhiteSpace(request.DeviceId))
                errors.Add("DeviceId is required.");

            return errors;
        }

        public static List<string> ValidateUpdate(Common.Model.LoginRequest request)
        {
            var errors = new List<string>();

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                if (request.Password.Length < 8 ||
                    !Regex.IsMatch(request.Password, @"[A-Z]") ||
                    !Regex.IsMatch(request.Password, @"[a-z]") ||
                    !Regex.IsMatch(request.Password, @"[0-9]") ||
                    !Regex.IsMatch(request.Password, @"[\W_]"))
                {
                    errors.Add("Password must be at least 8 characters and include uppercase, lowercase, number, and special character.");
                }
            }

            if (!string.IsNullOrWhiteSpace(request.PhoneNumber) &&
                !Regex.IsMatch(request.PhoneNumber, @"^\+?[0-9]{10,15}$"))
                errors.Add("Phone number must be 10–15 digits and may start with +.");

            if (!string.IsNullOrWhiteSpace(request.Email) &&
                !Regex.IsMatch(request.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                errors.Add("Invalid email address format.");

            if (!string.IsNullOrWhiteSpace(request.Gender) &&
                !new[] { "Male", "Female", "Other" }.Contains(request.Gender))
                errors.Add("Gender must be Male, Female, or Other.");

            if (request.DateOfBirth != null)
            {
                var dob = request.DateOfBirth;
                var age = DateTime.Today.Year - dob.Year;
                if (dob > DateTime.Today.AddYears(-age)) age--;

                if (age < 13)
                    errors.Add("User must be at least 13 years old.");
            }
            return errors;
        }

        private bool IsJwtToken(string token)
        {
            var regex = new Regex(@"^[A-Za-z0-9-_]+\.[A-Za-z0-9-_]+\.[A-Za-z0-9-_]+$");
            return regex.IsMatch(token);
        }

    }
}
