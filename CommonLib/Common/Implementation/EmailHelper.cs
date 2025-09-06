using System.Net;
using System.Net.Mail;
using System.Text;

namespace Common
{
	public class EmailHelper: IEmailHelper
	{
		
		
		public EmailHelper()
		{
	
		}

		public async Task<string> GenerateEmail(string subject, string content)
		{
			return EmailbySMTP(subject, content);
		}

		public string EmailbySMTP(string subject, string body)
		{
			string fromEmail = "syedkhalidhalim@gmail.com";  // Your Gmail address
			string password = "boeb rsxm cndt dkwt";
			string toEmail = "syedkhalidhalim@hotmail.com";  // Recipient's email address
            subject = subject ?? "Test Subject";
			body = body ?? "This is a test email sent from a application.";

			// Set up the SMTP client
			SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
			{
				Port = 587, // TLS/STARTTLS
				EnableSsl = true, // Use SSL/TLS
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
				return ex.Message;
			}

		}

	}
}
