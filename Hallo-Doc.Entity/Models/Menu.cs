using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Hallo_Doc.Entity.Models;

[Table("Menu")]
public partial class Menu
{
    [Key]
    public int MenuId { get; set; }

    [StringLength(50)]
    public string Name { get; set; } = null!;

    public short AccountType { get; set; }

    public int? SortOrder { get; set; }
}
