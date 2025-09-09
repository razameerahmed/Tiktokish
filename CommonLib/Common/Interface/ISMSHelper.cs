namespace Common
{
	public interface ISMSHelper
	{
		Task<string> GenerateSMS(string content);
	}
}
