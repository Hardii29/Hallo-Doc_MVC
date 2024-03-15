using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hallo_Doc.Entity.ViewModel
{
    public class Order
    {
        public int? RequestId { get; set; }
        public string Name { get; set; } = null!;
        public string? Prescription { get; set; }
        public int? NoOfRefill { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public int OrderId { get; set; }
        public int HealthProfessionalTypeId { get; set; }
        public string ProfessionName { get; set; } = null!;
        public int VendorId { get; set; } 
        public string VendorName { get; set;} = null!;
        public string FaxNumber { get; set; } = null!;
        public string? Email { get; set; }
        public string? BusinessContact { get; set; }
    }
}
