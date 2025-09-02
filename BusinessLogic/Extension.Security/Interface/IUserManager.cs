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
		public bool TestConnection(string header, string tranMessage);
		public ResponseModel<LoginResponse> ValidateLogin(LoginRequest request, ResponseModel<LoginResponse> response);
		public ResponseModel<LoginResponse> AddUser(LoginRequest request, ResponseModel<LoginResponse> response);
	}
}
