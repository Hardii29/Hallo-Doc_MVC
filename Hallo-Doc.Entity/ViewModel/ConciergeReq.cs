using System.ComponentModel.DataAnnotations;

namespace Hallo_Doc.Entity.ViewModel
{
    public class ConciergeReq
    {
        [Required(ErrorMessage = "Your First name is required")]
        public string c_firstname { get; set; }
        public string c_lastname { get; set; }
        [Required(ErrorMessage = "Your Email is required")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Email should be valid")]
        public string c_email { get; set; }
        public string c_mobile { get; set; }
        public string c_address { get; set; }
        public string c_street { get; set; }
        public string c_city { get; set; }
        public string c_state { get; set; }
        [StringLength(6, MinimumLength = 6, ErrorMessage = "It must be of 6 digits")]
        [RegularExpression(@"([0-9]{6})", ErrorMessage = "It must be of 6 numerics")]
        public string c_zip { get; set; }
        [Required(ErrorMessage = "Symptoms is required")]
        public string Symptoms { get; set; }
        [Required(ErrorMessage = "Patient First name is required")]
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateOnly DOB { get; set; }
        [Required(ErrorMessage = "Patient Email is required")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Email should be valid")]
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public string? Street { get; set; }
       
    }
}
