using Hallo_Doc.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hallo_Doc.Entity.ViewModel
{
    public class TimesheetData
    {
        public string DateRange { get; set; }
        public bool isFinalize { get; set; }
        public DateOnly startDate { get; set; }
        public DateOnly endDate { get; set; }
        public List<TimeSheet> TimeSheetInfo { get; set; }
        public List<string> OnCallHours { get; set; }
        public List<string> TotalHours { get; set; }
        public List<bool> IsWeekend { get; set; }
        public List<string> NoofHousecall { get; set; }
        public List<string> NoofPhoneConsult { get; set; }
        public List<string> Items { get; set; }
        public List<string> Bills { get; set; }
        public List<string> Amount { get; set; }
    }
    public class ShowTimeSheet
    {
        public DateOnly Date {  get; set; }
        public int? OnCallHours { get; set; }
        public int? TotalHours { get; set; }
        public int? NoofHousecall { get; set; }
        public int? NoofPhoneConsult { get;set; }
        public bool? IsWeekend { get; set; }
        public string? Item { get; set; }
        public string? Bill {  get; set; }
        public string? Amount { get; set; }
        public List<ShowTimeSheet> Weeklysheet {  get; set; }
        public List<TimeSheet> TimeSheetDetail { get; set; }
        public bool? IsFinalize { get; set; }
        public bool? IsApproved { get; set; }
        public string PhysicianName { get; set; }
        public DateOnly startDate { get; set; }
        public DateOnly endDate { get; set; }
    }
}
