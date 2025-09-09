using Common.Model;

namespace Extension.Security.Interface
{
	public interface INotificationManager
	{
		public bool GenerateOTP(string header, string tranMessage);
		
	}
}
