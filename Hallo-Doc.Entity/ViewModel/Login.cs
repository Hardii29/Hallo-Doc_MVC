using System.ComponentModel.DataAnnotations;

namespace Hallo_Doc.Entity.ViewModel
{
    public class Login
    {
        public bool Success;
        public int UserId { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ErrorMessage { get; set; }
        public string? UserName { get; set; }
    }
}
