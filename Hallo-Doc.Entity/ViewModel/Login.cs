using System.ComponentModel.DataAnnotations;

namespace Hallo_Doc.Entity.ViewModel
{
    public class Login
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
    

}
