using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hallo_Doc.Entity.ViewModel
{
    public class AdminProfile
    {
        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
        public string? UserName { get; set; }
        [Required(ErrorMessage = "First name is required")]
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? DOB { get; set; }
        public string? Mobile { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter valid Email Address.")]
        public string? Email { get; set; }
        [Compare("Email", ErrorMessage = "Email doesn't match.")]
        public string ConfirmEmail { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? Region { get; set; }
        public string? State { get; set; }
        [Required(ErrorMessage = "ZipCode is required")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "It must be of 6 digits")]
        [RegularExpression(@"([0-9]{6})", ErrorMessage = "It must be of 6 numerics")]
        public string? ZipCode { get; set; }
        public string? AdminName { get; set; }
        public int AdminId { get; set; } = 0;
        public string AltPhone { get; set; }
        public int RoleId { get; set; }
        public int RegionId { get; set; }       
        public List<RegionList> Regions { get; set; }
        public List<int> SelectRegion { get; set; }
    }
    public class RegionList
    {
        public int RegionId { get; set; }
        public string? Name { get; set; }
    }
}
