namespace Common
{
	public interface IEmailHelper
	{
		Task<string> GenerateEmail(string subject, string content,string sendTo);
	}
}
