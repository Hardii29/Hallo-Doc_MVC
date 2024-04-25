using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Hallo_Doc.Entity.ViewModel
{
    public class PatientReq
    {
        public bool Success;
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        [Required(ErrorMessage = "BirthDate is required")]
        public DateOnly DOB { get; set; }
        [Required(ErrorMessage = "Symptoms is required")]
        public string Symptoms { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter valid Email Address.")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        [Compare("Password", ErrorMessage = "Password doesn't match.")]
        public string? ConformPass { get; set; }
        public string? Mobile { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        [Required(ErrorMessage = "ZipCode is required")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "It must be of 6 digits")]
        [RegularExpression(@"([0-9]{6})", ErrorMessage = "It must be of 6 numerics")]
        public string ZipCode { get; set; }
        public int RequestWiseFileId { get; set; }
        public IFormFile? File { get; set; }
        public int RequestId { get; set; }
        public string FileName { get; set; } = null!;
        public string? AdminName { get; set; }
        public int AdminId { get; set; } = 0;
    }

}
