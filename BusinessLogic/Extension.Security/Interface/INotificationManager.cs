using Common.Model;

namespace Extension.Security.Interface
{
	public interface INotificationManager
	{
		public Task<bool> GenerateOTP(string header, string tranMessage,string sendTo);
		
	}
}
