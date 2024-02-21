using System.ComponentModel.DataAnnotations;

namespace Hallo_Doc.Models.ViewModel
{
    public class Login
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
      
        public string? FirstName { get; set; } 
        public string? LastName { get; set; }
    }
    

}
