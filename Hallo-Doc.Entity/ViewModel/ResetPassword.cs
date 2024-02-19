using System.ComponentModel.DataAnnotations;

namespace Hallo_Doc.Entity.ViewModel
{
    public class ResetPassword
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public required string Password { get; set; }
        [Compare("Password", ErrorMessage = "Password do not match")]
        public required string ConfirmPassword { get; set; }
        public string? AspnetuserId { get; set; }
        public string? Token { get; set; }
    }
}
