using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hallo_Doc.Entity.ViewModel
{
    public class AdminDash
    {
        public int RequestId { get; set; }
        public int RequestTypeId { get; set; }
        public string? PatientName { get; set; }
        public DateOnly? DOB { get; set; }
        public string? Requestor { get; set; }
        public DateTime? RequestedDate { get; set; }
        public string? Email { get; set; }
        public string? Region { get; set; }
        public string? PatientMobile {  get; set; }
        public string? ProviderName { get; set; }
        public string?  Address { get; set; }
        public string? Notes { get; set; }
        public int? RequestClientId { get; set; }
        public int? ProviderId { get; set; }
        public string providerAspid { get; set; }
        public string? RequestorPhoneNumber { get; set; }
       
    }
    public class CountStatusWiseRequest
    {
        public int NewRequest { get; set; }
        public int PendingRequest { get; set; }
        public int ActiveRequest { get; set; }
        public int ConcludeRequest { get; set; }
        public int ToCloseRequest { get; set; }
        public int UnpaidRequest { get; set; }
        public string? AdminName { get; set; }
        public int AdminId { get; set; } = 0;
    }
    public class PaginatedViewModel<T>
    {
        public List<T> AdminDash { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
