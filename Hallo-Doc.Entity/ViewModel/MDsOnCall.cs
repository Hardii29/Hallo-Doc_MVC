using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hallo_Doc.Entity.ViewModel
{
    public class MDsOnCall
    {
        public List<MDsOnCall>? data;
        public int AdminId { get; set; }
        public string AdminName { get; set; }
        public int PhysicianId { get; set; }
        public string PhysicianName { get;set; }
        public string? Photo {  get; set; }
    }
}
