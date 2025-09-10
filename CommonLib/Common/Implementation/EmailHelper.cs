using Azure.Core;
using Common.Implementation;
using Common.Interface;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Common
{
	public class EmailHelper: IEmailHelper
	{
		IConfiguration _configuration;
		
		public EmailHelper(IConfiguration configuration)
		{
			_configuration = configuration;	
		}

		public async Task<string> GenerateEmail(string subject, string content,string sendTo)
		{
			return EmailbySMTP(subject, content, sendTo);
		}

		public string EmailbySMTP(string subject, string body,string sendTo)
		{
            ActivityLogger.Instance.SystemLog(NLog.LogLevel.Info, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), sendTo, sendTo, "machine name", this.GetType().Name, "Login", 1);

            // Gmail SMTP server configuration
            string smptServer = _configuration["Email:smptServer"] == "" ? _configuration["Email:smptServer"] : "smtp.gmail.com";
			int port = Convert.ToInt32(_configuration["Email:port"] == "" ? _configuration["Email:port"] : "587");
			bool ssl = true;
            string fromEmail = _configuration["Email:fromEmail"];// "syedkhalidhalim@gmail.com";  // Your Gmail address
			string password = _configuration["Email:password"] != "" ? _configuration["Email:fromEmail"] : "boeb rsxm cndt dkwt";// "boeb rsxm cndt dkwt";
			string toEmail = sendTo;//"syedkhalidhalim@hotmail.com";  // Recipient's email address
            subject = subject ?? "Test Subject";
			body = body ?? "This is a test email sent from a application.";

			// Set up the SMTP client
			SmtpClient smtpClient = new SmtpClient(smptServer)
			{
				Port = port, // TLS/STARTTLS
				EnableSsl = ssl, // Use SSL/TLS
				Credentials = new NetworkCredential(fromEmail, password),
			};

			// Create the email message
			MailMessage message = new MailMessage(fromEmail, toEmail, subject, body);

			try
			{
				// Send the email
				smtpClient.Send(message);
				return "Sent";
			}
			catch (Exception ex)
			{
                ActivityLogger.Instance.SystemLog(NLog.LogLevel.Error, string.Format("Executing Method {0}", System.Reflection.MethodBase.GetCurrentMethod().Name), ActionType.View.ToString(), sendTo, sendTo, "machine name", this.GetType().Name, ex.Message, 0, ex);

                return ex.Message;
			}

		}

	}
}
