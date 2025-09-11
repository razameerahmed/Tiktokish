using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Implementation
{
	public static class GlobalConfiguration
	{
		public static string TokenIssuer = "yourdomain.com";
		public static string TokenAudience = "yourdomain.com";
		public static string TokenSecretKey = "your_super_secret_key_1111111111";
		public static string HashSalt = "TIKTOKISH";
		public static int OTPExpiry = 5;
		public static int TokenExpiry = 5;
	}
}
