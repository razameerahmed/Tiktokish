namespace Common
{
	public interface IEmailHelper
	{
		Task<string> GenerateEmail(string content);
	}
}
