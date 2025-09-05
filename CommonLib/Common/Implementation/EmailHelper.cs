using System.Text;

namespace Common
{
	public class EmailHelper: IEmailHelper
	{
		
		
		public EmailHelper()
		{
	
		}

		public async Task<string> GenerateEmail(string content)
		{
			return "OK";
		}

	}
}
