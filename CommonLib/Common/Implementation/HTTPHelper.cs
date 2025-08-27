
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
	public class HTTPHelper: IHTTPHelper
	{
		private readonly HttpClient _httpClient;
		private readonly string _baseAddress;
		
		
		public HTTPHelper(string baseAddress, IHttpClientFactory httpClientFactory)
		{
			_baseAddress = baseAddress;
			_httpClient = httpClientFactory.CreateClient();
			_httpClient.BaseAddress = new Uri(baseAddress);
			// Add default headers or other configurations here
			_httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
		}

		public async Task<string> GetAsync(string endpoint)
		{
			HttpResponseMessage response = await _httpClient.GetAsync(endpoint);
			response.EnsureSuccessStatusCode(); // Throws an exception if not successful
			return await response.Content.ReadAsStringAsync();
		}

		public async Task<string> PostJsonAsync<T>(string endpoint, T data)
		{
			var jsonContent = new StringContent(
				System.Text.Json.JsonSerializer.Serialize(data),
				Encoding.UTF8,
				"application/json");

			HttpResponseMessage response = await _httpClient.PostAsync(endpoint, jsonContent);
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadAsStringAsync();
		}
	}
}
