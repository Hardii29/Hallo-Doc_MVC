using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hallo_Doc.Entity.ViewModel
{
    public class Schedule
    {
        public string? AdminName { get; set; }
        public int AdminId { get; set; }
        public int? RegionId { get; set; }
        public string? RegionName { get; set;}
        public int PhysicianId { get; set; }
        public string? PhysicianName { get;set; }
        public DateOnly ShiftDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public short Status { get; set; }
        public BitArray? IsRepeat { get; set; }
        public DateTime Start {  get; set; }
        public DateTime End { get; set; }
        public string? Color { get; set; }
        public string? Border { get; set; }    
        public int ShiftId { get; set; }
    }
}
