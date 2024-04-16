using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hallo_Doc.Entity.ViewModel
{
    public class ViewNotes
    {
        public int AdminId { get; set; }
        public string AdminName { get; set; }
        public int? Requestnotesid { get; set; }
        public int? Requestid { get; set; }
        public string? Strmonth { get; set; }
        public int? Intyear { get; set; }
        public int? Intdate { get; set; }
        public string? PhysicianNotes { get; set; }
        public string? AdminNotes { get; set; }
        public string? PatientNotes { get; set; }
        public string? Createdby { get; set; } = null!;
        public DateTime? Createddate { get; set; }
        public string? Modifiedby { get; set; }
        public DateTime? Modifieddate { get; set; }
        public short Status { get; set; }
        public string? Ip { get; set; }
        public virtual Request Request { get; set; } = null!;
        public List<TransferNotes> transfernotes { get; set; } = null!;
        public List<TransferNotes> cancel { get; set; } = null!;
        public List<TransferNotes> cancelbyphysician { get; set; } = null!;
    }
    public class TransferNotes
    {
        public int Requeststatuslogid { get; set; }
        public int Requestid { get; set; }
        public int? Physicianid { get; set; }
        public int? Transtophysicianid { get; set; }
        public DateTime Createddate { get; set; }
        public string? Notes { get; set; }
        public short Status { get; set; }
        public BitArray? Transtoadmin { get; set; }
        public string? TransPhysician { get; set; }
        public string? Admin { get; set; }
        public string? Physician { get; set; }
        public string TransferNotesData => $"{Admin} transferred <b> {Physician}  </b> to <b> {TransPhysician} </b> on {Createddate}: <b>{Notes}</b>";

    }
}
