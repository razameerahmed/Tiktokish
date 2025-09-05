using Common.Model;

namespace Extension.Security.Interface
{
	public interface INotificationManager
	{
		public bool TestConnection(string header, string tranMessage);
		
	}
}
