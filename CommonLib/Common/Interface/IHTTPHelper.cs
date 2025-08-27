using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
	public interface IHTTPHelper
	{
		Task<string> GetAsync(string endpoint);
		Task<string> PostJsonAsync<T>(string endpoint, T data);
	}
}
