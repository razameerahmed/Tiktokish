using Common.Model;
using DataAccessLayer.Models;

namespace Extension.Security.Interface
{
	public interface IUserManager
	{
		public bool TestConnection(string header, string tranMessage);
		public ResponseModel<LoginResponse> ValidateLogin(LoginRequest request, ResponseModel<LoginResponse> response);
		public ResponseModel<LoginResponse> AddUser(LoginRequest request, ResponseModel<LoginResponse> response);
		public ResponseModel<LoginResponse> UpdateUser(CommonUser request, ResponseModel<LoginResponse> response);
		public ResponseModel<LoginResponse> ValidateUserForAdd(CommonUser request, ResponseModel<LoginResponse> response);
		public ResponseModel<List<Feed>> GetFeed(CommonUser request, ResponseModel<List<Feed>> response);

    }
}
