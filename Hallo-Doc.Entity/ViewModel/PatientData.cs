using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hallo_Doc.Entity.ViewModel
{
    public class PatientData
    {
        public List<PatientData> Record { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? RequestedDate { get; set; }
        public DateTime? ConcludedDate { get; set; }
        public status Status { get; set; }
        public int RequestId { get; set; }
        public int RequestTypeId { get; set; }
        public int Fcount { get; set; }
        public string PatientName { get; set; }
        public string? Confirmation { get; set; }
        public string Physician { get; set; }
        public string Email { get; set; }
        public string? Mobile { get; set; }
        public string? Notes { get; set; }
        public bool? IsActive { get; set; }
        public int AdminId { get; set; }
        public string AdminName { get; set; }

    }
    public class BlockHistory
    {
        public int AdminId { get; set; }
        public string AdminName { get; set; }
        public string PatientName { get; set; }
        public string Email { get; set; }
        public string? Mobile { get; set; }
        public DateTime? CreatedDate { get; set; }
        public List<PatientData> list { get; set; }
    }
    public class UserData
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public int AdminId { get; set; }
        public string AdminName { get; set; }
        public List<UserData> data { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 5;

    }
    public enum status
    {
        Unassigned = 1,
        Accepted = 2,
        Cancelled = 3,
        MDEnRoute = 4,
        MDOnSite = 5,
        Conclude = 6,
        CancelledByPatient = 7,
        Closed = 8,
        Unpaid = 9,
        Clear = 10,
        Block = 11
    }
    public enum RequestTypes
    {
        Business = 1,
        Patient = 2,
        Family = 3,
        Concierge = 4
    }
    public class SearchRecords
    {
        public string PatientName { get; set; }
        public string Requestor { get; set; }
        public int RequestTypeID { get; set; }
        public int RequestID { get; set; }
        public DateTime? DateOfService { get; set; }
        public DateTime? CloseCaseDate { get; set; }
        public string Email { get; set; }
        public string? Mobile { get; set; }
        public string? Address { get; set; }
        public string? Zip { get; set; }
        public status Status { get; set; }
        public string? Physician { get; set; }
        public string? PhyNotes { get; set; }
        public string? CancelByPhyNotes { get; set; }
        public string? AdminNotes { get; set; }
        public string? PatientNotes { get; set; }
        public DateTime? Modifieddate { get; set; }
    }
    public class SearchRecordList
    {
        public int ReqStatus { get; set; }
        public string PatientName { get; set; }
        public int RequestTypeID { get; set; }
        public DateTime? StartDOS { get; set; }
        public DateTime? EndDOS { get; set; }
        public string PhysicianName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public List<SearchRecords> list { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 5;
        public int AdminId { get; set; }
        public string AdminName { get; set; }
    }
}
