using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hallo_Doc.Entity.ViewModel
{
    public class Logs
    {
        public int AdminId { get; set; }
        public string AdminName { get; set; }
        public string Recipient { get; set; }
        public string Email { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? SentDate { get; set;}
        public int Role {  get; set; }
        public string Action { get; set; }
        public string Confirmation { get; set; }
        public List<Logs> EmailLog { get; set;}
        public AccountType RoleId { get; set; }
        public string IsEmailSent { get; set; }
        public int? SentTries { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 5;
    }
    public class SMSLog
    {
        public int AdminId { get; set;}
        public string AdminName { get; set; }
        public string Recipient { get; set; }
        public string Mobile { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? SentDate { get; set; }
        public int Role { get; set; }
        public LogsAction Action { get; set; }
        public string Confirmation { get; set; }
        public List<SMSLog> list { get; set; }
        public AccountType RoleId { get; set; }
        public string IsSMSSent { get; set; }
        public int? SentTries { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 5;

    }
    public enum LogsAction
    {
        SendOrder = 1,
        Request,
        SendLink,
        SendAgreement,
        ContactProvider,
        NewRegistration,
        Forgot
    }
}
