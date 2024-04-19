using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Hallo_Doc.Entity.Models;

[Table("EncounterForm")]
public partial class EncounterForm
{
    [Key]
    public int EncounterFormId { get; set; }

    public int RequestId { get; set; }

    [StringLength(200)]
    public string? HistoryOfInjury { get; set; }

    [StringLength(500)]
    public string? MedicalHistory { get; set; }

    [StringLength(200)]
    public string? Medicatons { get; set; }

    [StringLength(100)]
    public string? Allergies { get; set; }

    [StringLength(20)]
    public string? Temp { get; set; }

    [Column("HR")]
    [StringLength(20)]
    public string? Hr { get; set; }

    [Column("RR")]
    [StringLength(20)]
    public string? Rr { get; set; }

    [StringLength(20)]
    public string? BloodPressureSystolic { get; set; }

    [StringLength(20)]
    public string? BloodPressureDiastolic { get; set; }

    [StringLength(20)]
    public string? O2 { get; set; }

    [StringLength(100)]
    public string? Pain { get; set; }

    [StringLength(100)]
    public string? Heent { get; set; }

    [Column("CV")]
    [StringLength(100)]
    public string? Cv { get; set; }

    [StringLength(100)]
    public string? Chest { get; set; }

    [Column("ABD")]
    [StringLength(100)]
    public string? Abd { get; set; }

    [StringLength(100)]
    public string? Extremeties { get; set; }

    [StringLength(100)]
    public string? Skin { get; set; }

    [StringLength(100)]
    public string? Neuro { get; set; }

    [StringLength(100)]
    public string? Other { get; set; }

    [StringLength(100)]
    public string? Diagnosis { get; set; }

    [StringLength(500)]
    public string? TreatmentPlan { get; set; }

    [StringLength(500)]
    public string? MedicationDispensed { get; set; }

    [StringLength(500)]
    public string? Procedures { get; set; }

    [StringLength(500)]
    public string? FollowUp { get; set; }

    public int? AdminId { get; set; }

    public int? PhysicianId { get; set; }

    public bool? IsFinalize { get; set; }

    [ForeignKey("AdminId")]
    [InverseProperty("EncounterForms")]
    public virtual Admin? Admin { get; set; }

    [ForeignKey("PhysicianId")]
    [InverseProperty("EncounterForms")]
    public virtual Physician? Physician { get; set; }

    [ForeignKey("RequestId")]
    [InverseProperty("EncounterForms")]
    public virtual Request Request { get; set; } = null!;
}
