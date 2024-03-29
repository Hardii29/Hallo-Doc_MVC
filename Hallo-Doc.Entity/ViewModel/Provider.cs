using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hallo_Doc.Entity.ViewModel
{
    public class Provider
    {
        public string? Password { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public string? DOB { get; set; }
        public string? Mobile { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
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
        public string? ZipCode { get; set; }
        public string? AdminName { get; set; }
        public int AdminId { get; set; } = 0;
        public string? MedicalLicense { get; set; }
        public string? NPI {  get; set; }
        public string? BusinessName { get; set;}
        public string? BusinessWebsite { get; set; }
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
}
