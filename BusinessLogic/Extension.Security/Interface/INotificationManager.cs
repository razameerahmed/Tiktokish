using Common.Model;

namespace Extension.Security.Interface
{
	public interface INotificationManager
	{
		public Task<bool> GenerateOTP(string userName, string sendTo);
		public Task<bool> GenerateEmail(string userName, string sendTo);
		public bool ValidateOTP(string userName, string emailOTP, string smsOTP);


	}
}
