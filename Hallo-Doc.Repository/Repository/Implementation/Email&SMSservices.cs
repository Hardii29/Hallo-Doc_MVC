using Hallo_Doc.Repository.Repository.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Hallo_Doc.Repository.Repository.Implementation
{
    public class Email_SMSservices :IEmail_SMS
    {
        private readonly IConfiguration _configuration;
        public Email_SMSservices(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void SendEmail(string body, string  subject, string email)
        {
            using (MailMessage message = new MailMessage())
            {
                message.From = new MailAddress("hardi.jayani@etatvasoft.com");
                message.To.Add("hardi.jayani@etatvasoft.com");
                message.Body = body;
                message.Subject = subject;
                using (SmtpClient smtp = new SmtpClient("mail.etatvasoft.com", 587))
                {
                    smtp.Credentials = new System.Net.NetworkCredential("hardi.jayani@etatvasoft.com", "LHV0@}YOA?)M");
                    smtp.EnableSsl = true;
                    smtp.Send(message);
                }
            }
        }
        public bool SendSMS(string receiverPhoneNumber, string message)
        {
            string receiver = "+91"+receiverPhoneNumber;
            string accountSid = _configuration["TwilioSettings:AccountSid"];
            string authToken = _configuration["TwilioSettings:AuthToken"];
            string twilioPhoneNumber = _configuration["TwilioSettings:TwilioPhoneNumber"];

            TwilioClient.Init(accountSid, authToken);

            try
            {
                var smsMessage = MessageResource.Create(
                    body: message,
                    from: new Twilio.Types.PhoneNumber(twilioPhoneNumber),
                    to: new Twilio.Types.PhoneNumber(receiver)
                );
                
                Console.WriteLine("SMS sent successfully. SID: " + smsMessage.Sid);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while sending the SMS: " + ex.Message);
            }
            return false;
        }
    }
    
}
