using System.Text;

namespace Common
{
	public class SMSHelper: ISMSHelper
	{
		
		
		public SMSHelper()
		{
	
		}

		public async Task<string> GenerateSMS(string content)
		{
			return "OK";
		}

	}
}
