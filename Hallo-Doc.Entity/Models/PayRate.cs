using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Hallo_Doc.Entity.Models;

[Table("PayRate")]
public partial class PayRate
{
    [Key]
    public int PayRateId { get; set; }

    public int PhysicianId { get; set; }

    [Column("NightShift_Weekend", TypeName = "character varying")]
    public string? NightShiftWeekend { get; set; }

    [Column(TypeName = "character varying")]
    public string? Shift { get; set; }

    [Column("HouseCall_night_weekend", TypeName = "character varying")]
    public string? HouseCallNightWeekend { get; set; }

    [Column(TypeName = "character varying")]
    public string? PhoneConsults { get; set; }

    [Column("PhoneConsult_night_weekend", TypeName = "character varying")]
    public string? PhoneConsultNightWeekend { get; set; }

    [Column(TypeName = "character varying")]
    public string? BatchTesting { get; set; }

    [Column(TypeName = "character varying")]
    public string? Housecall { get; set; }

    [ForeignKey("PhysicianId")]
    [InverseProperty("PayRates")]
    public virtual Physician Physician { get; set; } = null!;
}
