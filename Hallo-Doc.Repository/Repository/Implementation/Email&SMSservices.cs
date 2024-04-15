using Hallo_Doc.Repository.Repository.Interface;
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
        public void SendEmail(string body, string  subject, string email)
        {
            using (MailMessage message = new MailMessage())
            {
                message.From = new MailAddress("hardi.jayani@etatvasoft.com");
                message.To.Add(email);
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
            string accountSid = "AC4bf3b374a445381a40e8d37605b8b53d";
            string authToken = "fc1d430d4c010a13f0368824e885abb4";
            string twilioPhoneNumber = "+12515125413";

            TwilioClient.Init(accountSid, authToken);

            try
            {
                var smsMessage = MessageResource.Create(
                    body: message,
                    from: new Twilio.Types.PhoneNumber(twilioPhoneNumber),
                    to: new Twilio.Types.PhoneNumber(receiverPhoneNumber)
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
