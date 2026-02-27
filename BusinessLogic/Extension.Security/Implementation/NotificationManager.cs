using Azure;
using Azure.Core;
using Common;
using Common.Implementation;
using Common.Interface;
using Common.Model;
using DataAccessLayer.Models;
using Extension.Security.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.CodeDom.Compiler;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Extension.Security.Implementation
{
	public class NotificationManager : INotificationManager
	{
		private readonly string _connectionString;
		private IEmailHelper _emailHelper;
		private ISMSHelper _smsHelper;
		private IPushNotificationHelper _pushNotificationHelper;

		public NotificationManager(IConfiguration configuration, IEmailHelper emailHelper, ISMSHelper smsHelper,
			IPushNotificationHelper pushNotificationHelper)
		{
			var connStr = configuration.GetConnectionString("DefaultConnection");
			if (string.IsNullOrEmpty(connStr))
				throw new InvalidOperationException("Connection string 'DefaultConnection' is missing or null.");
			_connectionString = connStr;
			_emailHelper = emailHelper;
			_smsHelper = smsHelper;
			_pushNotificationHelper = pushNotificationHelper;

		}
		public async Task<bool> GenerateOTP(string userName, string sendTo)
		{
			if (string.IsNullOrEmpty(userName))
				throw new InvalidOperationException("User infomation is not valid");

			// Register OTP
			var otpData = RegisterOTP(userName);
			if (otpData is not null)
			{
				await _smsHelper.GenerateSMS($"Your OTP is {otpData.SmsOtp}");
				await _emailHelper.GenerateEmail("OTP Generated", $"Your Email OTP is {otpData.EmailOtp} \n Your SMS OTP is {otpData.SmsOtp}", sendTo);
			}

			return true;
		}

		public async Task<bool> NotifyUser(string userName, string sendTo)
		{
			await _emailHelper.GenerateEmail("Login Alert", $"Hello {userName}, \r\n We noticed a login to your account:\r\n\r\nDate: {DateTime.Now}    \r\nIP Address: [IP Address]\r\n\r\nIf this was you, no action is needed. If not, please reset your password immediately. ", sendTo);
			return true;
		}

		public bool ValidateOTP(string userName, string emailOTP, string smsOTP)
		{
			try
			{
				using (TiktokishContext context = new(_connectionString))
				{
					var userOTP = context.UserOtps.OrderByDescending( x => x.CreatedOn)
					.FirstOrDefault(u =>
						u.Username == userName &&
						u.ExpiryDate >= DateTime.Now &&
						u.Status == 0);
					
					if (userOTP != null && userOTP.EmailOtp == emailOTP && userOTP.SmsOtp == smsOTP)
					{
						userOTP.Status = 1;
						context.Update(userOTP);
						context.SaveChanges();
						return true;
					}
					return false;
				}
			}
			catch (Exception ex)
			{
				ActivityLogger.Instance.SystemLog(NLog.LogLevel.Error, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), "", "", "machine name", this.GetType().Name, ex.Message, 0, ex);
				return false;
			}
		}

		public UserOtp? RegisterOTP(string userName)
		{
			UserOtp? data = null;
			try
			{
				using (TiktokishContext context = new(_connectionString))
				{
					data = new UserOtp
					{
						CreatedBy = "system",
						CreatedOn = DateTime.Now,
						UpdatedBy = "system",
						UpdatedOn = DateTime.Now,

						EmailOtp = GenerateRandomNumber(),
						SmsOtp = GenerateRandomNumber(),
						Username = userName,
						ExpiryDate = DateTime.Now.AddMinutes(GlobalConfiguration.OTPExpiry),

						InvalidRetry = "3",
						RetryCount = "3",
						Status = 0,

						Issplit = null,
						Otpexpiry = null,
						Otptype = null

					};
					context.Add(data);
					context.SaveChanges();
					return data;
				}
			}
			catch (Exception ex)
			{
				ActivityLogger.Instance.SystemLog(NLog.LogLevel.Error, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), "", "", "machine name", this.GetType().Name, ex.Message, 0, ex);
				return data;
			}
		}

		public string GenerateRandomNumber()
		{
			Random random = new Random();
			int number = random.Next(0, 10000); // allows 0000 to 9999
			string padded = number.ToString("D4"); // always 4 digits
			return padded;
		}
	}
}
