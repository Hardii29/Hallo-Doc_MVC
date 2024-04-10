using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hallo_Doc.Entity.ViewModel
{
    public class PatientData
    {
        public DateTime CreatedDate { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime ConcludedDate { get; set; }
        public short Status { get; set; }
        public int RequestId { get; set; }
        public int RequestTypeId { get; set; }
        public int Fcount { get; set; }
        public string PatientName { get; set; }
        public string Confirmation { get; set; }
        public string Physician { get; set; }
        public string Email { get; set; }
        public string? Mobile { get; set; }
        public string? Notes { get; set; }
        public bool? IsActive { get; set; }
        public int AdminId { get; set; }
        public string AdminName { get; set; }

    }
    public class BlockHistory
    {
        public int AdminId { get; set; }
        public string AdminName { get; set; }
        public string PatientName { get; set; }
        public string Email { get; set; }
        public string? Mobile { get; set; }
        public DateTime? CreatedDate { get; set; }
        public List<PatientData> list { get; set; }
    }
}
