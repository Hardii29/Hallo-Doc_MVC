using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Hallo_Doc.Entity.Models;

[Table("Invoice")]
public partial class Invoice
{
    [Key]
    public int InvoiceId { get; set; }

    public int PhysicianId { get; set; }

    public int? AdminId { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public bool? IsFinalize { get; set; }

    public bool? IsApproved { get; set; }

    [Column(TypeName = "character varying")]
    public string? AdminNotes { get; set; }

    [Column(TypeName = "character varying")]
    public string? Bonus { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? CreatedDate { get; set; }

    [ForeignKey("PhysicianId")]
    [InverseProperty("Invoices")]
    public virtual Physician Physician { get; set; } = null!;

    [InverseProperty("Invoice")]
    public virtual ICollection<TimeSheet> TimeSheets { get; set; } = new List<TimeSheet>();
}
