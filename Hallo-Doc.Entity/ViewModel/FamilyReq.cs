using System.ComponentModel.DataAnnotations;

namespace Hallo_Doc.Entity.ViewModel
{
    public class FamilyReq
    {
        [Required(ErrorMessage = "Your First name is required")]
        public string? f_firstname {  get; set; }
        public string? f_lastname { get; set; }
        public string? f_mobile { get; set; }
        [Required(ErrorMessage = "Your Email is required")]
        [EmailAddress(ErrorMessage = "Please enter valid Email Address.")]
        public string? f_email { get; set; }
        public string? f_relation { get; set; }
        public required string Id { get; set; }
        [Required(ErrorMessage = "Symptoms is required")]
        public string Symptoms { get; set; }
        [Required(ErrorMessage = "Patient First name is required")]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required(ErrorMessage = "BirthDate is required")]
        public DateOnly DOB { get; set; }
        [Required(ErrorMessage = "Patient Email is required")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Email should be valid")]
        public required string Email { get; set; }
        public string? Mobile { get; set; }
        public string? Street { get; set; }
        [Required(ErrorMessage = "Select Any of one city")]
        public string? City { get; set; }
        public string? State { get; set; }
        [StringLength(6, MinimumLength = 6, ErrorMessage = "It must be of 6 digits")]
        [RegularExpression(@"([0-9]{6})", ErrorMessage = "It must be of 6 numerics")]
        public string? ZipCode { get; set; }
    }

}
