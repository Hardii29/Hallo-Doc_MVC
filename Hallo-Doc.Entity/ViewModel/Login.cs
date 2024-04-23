using System.ComponentModel.DataAnnotations;

namespace Hallo_Doc.Entity.ViewModel
{
    public class Login
    {
        public bool Success;
        public int UserId { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public required string Email { get; set; }
        [Required(ErrorMessage = "Enter the Password")]
        public required string Password { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ErrorMessage { get; set; }
        public string? UserName { get; set; }
        public int AdminId { get; set;}
        public string? AdminName { get; set;}
    }
    public class UserInfo
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public string AspNetUserId { get; set; }
        public int RoleID { get; set; }
    }
    public class ForgotPassword
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format"), Display(Name = "Registered email address")]
        public required string Email { get; set; }
        [Required(ErrorMessage = "Enter the Password")]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password do not match")]
        public string ConfirmPassword { get; set; }
        public string? AspnetuserId { get; set; }
        public required string Token { get; set; }
    }
}
