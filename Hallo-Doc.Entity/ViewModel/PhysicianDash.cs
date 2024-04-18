namespace Hallo_Doc.Entity.ViewModel
{
    public class PhysicianDash
    {
        public int RequestId { get; set; }
        public int RequestTypeId { get; set; }
        public string? PatientName { get; set; }
        public DateOnly? DOB { get; set; }
        public string? Requestor { get; set; }
        public DateTime? RequestedDate { get; set; }
        public string? Email { get; set; }
        public string? Region { get; set; }
        public string? PatientMobile { get; set; }
        public string? ProviderName { get; set; }
        public string? Address { get; set; }
        public string? Notes { get; set; }
        public int? RequestClientId { get; set; }
        public int? ProviderId { get; set; }
        public string? RequestorPhoneNumber { get; set; }
        public int Status { get; set; }
    }
    public class StatusCount
    {
        public int NewRequest { get; set; }
        public int PendingRequest { get; set; }
        public int ActiveRequest { get; set; }
        public int ConcludeRequest { get; set; }
   
        public string? PhyName { get; set; }
        public int PhyId { get; set; } = 0;
    }
    public class PaginationModel<T>
    {
        public List<T> PhysicianDash { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
