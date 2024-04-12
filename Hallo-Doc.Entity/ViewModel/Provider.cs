using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hallo_Doc.Entity.ViewModel
{
    public class Provider
    {
        public int ProviderId { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public string? DOB { get; set; }
        [Required(ErrorMessage = "Phone Number is required")]
        public string? Mobile { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter valid Email Address.")]
        public string Email { get; set; }
        public string? Role { get; set; }
        [Required(ErrorMessage = "Select Anyone Role")]
        public int RoleId { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? Region { get; set; }
        public int StateId { get; set; }
        public int Status {  get; set; }
        public string? AltPhone { get; set; }
        public string? State { get; set; }
        [StringLength(6, MinimumLength = 6, ErrorMessage = "It must be of 6 digits")]
        [RegularExpression(@"([0-9]{6})", ErrorMessage = "It must be of 6 numerics")]
        public string? ZipCode { get; set; }
        public string? AdminName { get; set; }
        public int AdminId { get; set; } = 0;
        public string? MedicalLicense { get; set; }
        public string? NPI {  get; set; }
        [Required(ErrorMessage = "BusinessName is required")]
        public string BusinessName { get; set;}
        [Required(ErrorMessage = "BusinessWebsite is required")]
        public string BusinessWebsite { get; set; }
        public IFormFile? File { get; set; }
        public IFormFile? FileSgn { get; set; }
        public string? SyncEmail { get; set; }
        public string? AdminNotes { get; set; }
        public bool IsAgreement { get; set; }
        public bool IsBackground { get; set;}
        public bool IsHIPAA { get; set;}
        public bool IsNonDisclosure { get; set;}
        public bool IsLicense { get; set;}
        public List<RegionModel> Regions { get; set; }
        public List<int> SelectRegion {  get; set; }
    }
    public class RegionModel
    {
        public int RegionId { get; set;}
        public string? Name { get; set;}
    }
    public class ProviderLocation
    {
        public int PhysicianId { get; set;}
        public string? PhyName { get; set;}
        public int LocationId { get; set; }
        public string? Address { get; set;}
        public decimal? lat { get; set;}
        public decimal? lng { get; set;}
        public string? AdminName { get; set; }
        public int AdminId { get; set; }
        public List<ProviderLocation> Locations { get; set; }

    }
}
