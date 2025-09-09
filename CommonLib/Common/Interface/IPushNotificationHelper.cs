namespace Common
{
	public interface IPushNotificationHelper
	{
		Task<string> GeneratePushNotification(string content);
	}
}
