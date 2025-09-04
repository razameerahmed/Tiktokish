namespace Common
{
	public interface IHTTPHelper
	{
		Task<string> GetAsync(string endpoint);
		Task<string> PostJsonAsync<T>(string endpoint, T data);
	}
}
