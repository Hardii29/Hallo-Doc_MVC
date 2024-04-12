using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hallo_Doc.Entity.ViewModel
{
    public class VendorMenu
    {
        public List<VendorMenu>? data;
        public int AdminId { get; set; }
        public string AdminName { get; set; }
        public int VendorId { get; set; }
        [Required(ErrorMessage = "Select Any profession")]
        public int? ProfessionId { get; set; }
        public string Profession { get; set; }
        [Required(ErrorMessage = "Business Name is required")]
        public string Business { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter valid Email Address.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Fax Number is must Required")]
        public string FaxNumber { get; set; }
        [Required(ErrorMessage = "Phone Number is Required")]
        public string PhoneNumber { get; set; }
        public string BusinessNumber { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        [StringLength(6, MinimumLength = 6, ErrorMessage = "It must be of 6 digits")]
        [RegularExpression(@"([0-9]{6})", ErrorMessage = "It must be of 6 numerics")]
        public string Zip { get; set; }
        public string Street { get; set; }
    }
    public class Order
    {
        public int? RequestId { get; set; }
        public string? Prescription { get; set; }
        public int? NoOfRefill { get; set; }
        public int VendorId { get; set; }
        public string FaxNumber { get; set; } = null!;
        public string? Email { get; set; }
        public string? BusinessContact { get; set; }
        [Required(ErrorMessage = "To place order profession is Required")]
        public int HealthProfessionalTypeId { get; set; }
    }
}
