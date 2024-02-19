using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Hallo_Doc.Entity.Models;

[Table("Requestclient")]
public partial class Requestclient
{
    [Key]
    public int RequestclientId { get; set; }

    public int RequestId { get; set; }

    [StringLength(100)]
    public string FirstName { get; set; } = null!;

    [StringLength(100)]
    public string? LastName { get; set; }

    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    [StringLength(100)]
    public string? Location { get; set; }

    [StringLength(500)]
    public string? Address { get; set; }

    [StringLength(50)]
    public string? Email { get; set; }

    public int? RegionId { get; set; }

    [StringLength(20)]
    public string? NotiMobile { get; set; }

    [StringLength(50)]
    public string? NotiEmail { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    [StringLength(20)]
    public string? StrMonth { get; set; }

    [Column("intYear")]
    public int? IntYear { get; set; }

    [Column("intDate")]
    public int? IntDate { get; set; }

    [Column(TypeName = "bit(1)")]
    public BitArray? IsMobile { get; set; }

    [StringLength(100)]
    public string? Street { get; set; }

    [StringLength(100)]
    public string? City { get; set; }

    [StringLength(100)]
    public string? State { get; set; }

    [StringLength(10)]
    public string? ZipCode { get; set; }

    public short? CommunicationType { get; set; }

    public short? RemindReservationCount { get; set; }

    public short? RemindHouseCallCount { get; set; }

    public short? IsSetFollowupSent { get; set; }

    [Column("IP", TypeName = "character varying")]
    public string? Ip { get; set; }

    public short? IsReservationRemindSent { get; set; }

    [Precision(9, 0)]
    public decimal? Latitude { get; set; }

    [Precision(9, 0)]
    public decimal? Longitude { get; set; }

    [ForeignKey("RequestId")]
    [InverseProperty("Requestclients")]
    public virtual Request Request { get; set; } = null!;
}
