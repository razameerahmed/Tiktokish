using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Implementation
{
	public static class GlobalConfiguration
	{
        public const string ActionLoginSuccess = "Login Success";
        public const string ActionLoginFailed = "Login Failed";
        public const string ActionAdd = "Add";
        public const string ActionUpdate = "Update";
        public const string ActionDelete = "Delete";
        public const string ActionSearch = "Search";
        public const string ActionExport = "Export";
        public const string ActionImport = "Import";
        public const string ActionTokenIssuer = "Token Issued";
        public const string UserServiceAPI = "User Service";
        public const string GatewayAPI = "Gateway Service";
        public const string TokenIssuer = "yourdomain.com";
		public const string TokenAudience = "yourdomain.com";
		public const string TokenSecretKey = "your_super_secret_key_1111111111";
		public const string HashSalt = "Tracker";
		public const int OTPExpiry = 5;
		public const int TokenExpiry = 5;
	}
}
