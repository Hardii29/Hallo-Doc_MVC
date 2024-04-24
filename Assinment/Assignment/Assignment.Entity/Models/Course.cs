using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Assignment.Entity.Models;

[Table("Course")]
public partial class Course
{
    [Key]
    public int CourseId { get; set; }

    [StringLength(20)]
    public string? Name { get; set; }

    [InverseProperty("CourseNavigation")]
    public virtual ICollection<StudentDatum> StudentData { get; set; } = new List<StudentDatum>();
}
