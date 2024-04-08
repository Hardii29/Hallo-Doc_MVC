using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hallo_Doc.Entity.ViewModel
{
    public class RequestedShift
    {
        public List<RequestedShift>? data;
        public int AdminId { get; set; }
        public string AdminName { get; set; }
        public int PhysicianId { get; set; }
        public string PhysicianName { get;set; }
        public int? RegionId { get; set; }
        public string RegionName { get; set; }
        public string ShiftDate { get; set; }
        public string ShiftTime { get; set; }
        public int ShiftId { get; set; }
    }
}
