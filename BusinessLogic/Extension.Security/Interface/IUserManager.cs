using Common.Interface;
using Common.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension.Security.Interface
{
	public interface IUserManager
	{
		public bool TestConnection();
		public LoginResponse ValidateLogin(LoginRequest request);
	}
}
