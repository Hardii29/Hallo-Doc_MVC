using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Hallo_Doc.Entity.Models;

[Table("ASPNetUsers")]
public partial class AspnetUser
{
    [Key]
    [Column("id")]
    [StringLength(128)]
    public string Id { get; set; } = null!;

    [Column("username")]
    [StringLength(256)]
    public string Username { get; set; } = null!;

    [Column("passwordhash", TypeName = "character varying")]
    public string? Passwordhash { get; set; }

    [Column("email")]
    [StringLength(256)]
    public string? Email { get; set; }

    [Column("phonenumber")]
    [StringLength(20)]
    public string? Phonenumber { get; set; }

    [Column("IP")]
    [StringLength(20)]
    public string? Ip { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime Createddate { get; set; }

    [InverseProperty("AspNetUser")]
    public virtual ICollection<Admin> Admins { get; set; } = new List<Admin>();

    [InverseProperty("User")]
    public virtual ICollection<AspNetUserRole> AspNetUserRoles { get; set; } = new List<AspNetUserRole>();

    [InverseProperty("RecieverNavigation")]
    public virtual ICollection<ChatHistory> ChatHistoryRecieverNavigations { get; set; } = new List<ChatHistory>();

    [InverseProperty("SenderNavigation")]
    public virtual ICollection<ChatHistory> ChatHistorySenderNavigations { get; set; } = new List<ChatHistory>();

    [InverseProperty("AspNetUser")]
    public virtual ICollection<Physician> Physicians { get; set; } = new List<Physician>();

    [InverseProperty("Aspnetuser")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
