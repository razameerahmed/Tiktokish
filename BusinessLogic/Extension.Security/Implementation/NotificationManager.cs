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
using Common;

namespace Extension.Security.Implementation
{
    public class NotificationManager : INotificationManager
    {
		private readonly string _connectionString;
        private  IEmailHelper _emailHelper;
        private  ISMSHelper _smsHelper;
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
		public async Task<bool> GenerateOTP(string header, string tranMessage)
        {
            await _smsHelper.GenerateSMS("");
            await _emailHelper.GenerateEmail("Login Email","Successfull Login");
            await _pushNotificationHelper.GeneratePushNotification("");


			return true;
        }


		
	}
}
