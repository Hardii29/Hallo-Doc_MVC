using Microsoft.AspNetCore.Http;
namespace Hallo_Doc.Entity.ViewModel
{
    public class PatientReq
    {
        public bool Success;
        public string? Id { get; set; }
        public string? Aspnetuserid { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateOnly DOB { get; set; }
        public string? Symptoms { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? Mobile { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public int RequestWiseFileId { get; set; }
        public IFormFile? File { get; set; }
        public int RequestId { get; set; }
        public string FileName { get; set; } = null!;
        public string? AdminName { get; set; }
        public int AdminId { get; set; } = 0;
    }
}
