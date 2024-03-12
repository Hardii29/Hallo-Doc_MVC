﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Hallo_Doc.Entity.Models;

[Table("Region")]
public partial class Region
{
    [Key]
    public int RegionId { get; set; }

    [StringLength(50)]
    public string Name { get; set; } = null!;

    [StringLength(50)]
    public string? Abbreviation { get; set; }

    [InverseProperty("Region")]
    public virtual ICollection<Physician> Physicians { get; set; } = new List<Physician>();
}