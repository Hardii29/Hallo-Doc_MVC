using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hallo_Doc.Repository.Repository.Interface
{
    public interface IEmail_SMS
    {
        void SendEmail(string body, string subject, string email);
        bool SendSMS(string receiverPhoneNumber, string message);
    }
}
