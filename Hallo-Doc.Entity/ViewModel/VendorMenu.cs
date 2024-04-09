using System;
using System.Collections.Generic;
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
        public int? ProfessionId { get; set; }
        public string Profession { get; set; }
        public string Business { get; set; }
        public string Email { get; set; }
        public string FaxNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string BusinessNumber { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Street { get; set; }
    }
}
