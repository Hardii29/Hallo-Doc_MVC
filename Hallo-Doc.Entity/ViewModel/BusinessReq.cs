using System.ComponentModel.DataAnnotations;

namespace Hallo_Doc.Entity.ViewModel
{
    public class BusinessReq
    {
        [Required(ErrorMessage = "Your First name is required")]
        public string? b_firstname { get; set; }
        public string? b_lastname { get; set; }
        public string? b_mobile { get; set; }
        [Required(ErrorMessage = "Your Email is required")]
        [EmailAddress(ErrorMessage = "Please enter valid Email Address.")]
        public string? b_email { get; set; }
        [Required(ErrorMessage = "Symptoms is required")]
        public string Symptoms { get; set; }
        [Required(ErrorMessage = "Patient First name is required")]
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateOnly DOB { get; set; }
        [Required(ErrorMessage = "Patient Email is required")]
        [EmailAddress(ErrorMessage = "Please enter valid Email Address.")]
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        [StringLength(6, MinimumLength = 6, ErrorMessage = "It must be of 6 digits")]
        [RegularExpression(@"([0-9]{6})", ErrorMessage = "It must be of 6 numerics")]
        public string? ZipCode { get; set; }
    }
}
