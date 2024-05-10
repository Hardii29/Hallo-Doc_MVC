using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Hallo_Doc.Entity.Models;

[Table("TimeSheet")]
public partial class TimeSheet
{
    [Key]
    public int TimesheetId { get; set; }

    public DateOnly Date { get; set; }

    public int? OnCallHours { get; set; }

    public int? TotalHours { get; set; }

    public bool? Holiday { get; set; }

    public int? NoOfHouseCall { get; set; }

    public int? NoOfPhoneCall { get; set; }

    [Column("Item ", TypeName = "character varying")]
    public string? Item { get; set; }

    [Column(TypeName = "character varying")]
    public string? Amount { get; set; }

    [Column(TypeName = "character varying")]
    public string? Bill { get; set; }

    public int InvoiceId { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? CreatedDate { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? ModifiedDate { get; set; }

    [ForeignKey("InvoiceId")]
    [InverseProperty("TimeSheets")]
    public virtual Invoice Invoice { get; set; } = null!;
}
