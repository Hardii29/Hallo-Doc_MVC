using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Assignment.Entity.Models;

public partial class StudentDatum
{
    [Key]
    public int StudentId { get; set; }

    [StringLength(20)]
    public string FirstName { get; set; } = null!;

    [StringLength(20)]
    public string? LastName { get; set; }

    public int CourseId { get; set; }

    public int? Age { get; set; }

    [StringLength(100)]
    public string? Email { get; set; }

    [StringLength(20)]
    public string? Gender { get; set; }

    [StringLength(20)]
    public string? Course { get; set; }

    [StringLength(50)]
    public string? Grade { get; set; }

    public bool? IsDeleted { get; set; }

    [ForeignKey("CourseId")]
    [InverseProperty("StudentData")]
    public virtual Course CourseNavigation { get; set; } = null!;
}
