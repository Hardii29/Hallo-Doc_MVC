using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Hallo_Doc.Entity.Models;

[Table("request")]
public partial class Request
{
    [Key]
    public int RequestId { get; set; }

    public int RequestTypeId { get; set; }

    public int? UserId { get; set; }

    [StringLength(100)]
    public string? FirstName { get; set; }

    [StringLength(100)]
    public string? LastName { get; set; }

    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    [StringLength(50)]
    public string? Email { get; set; }

    public short Status { get; set; }

    public int? PhysicianId { get; set; }

    [StringLength(20)]
    public string? ConfirmationNumber { get; set; }

    [Column("IP")]
    [StringLength(20)]
    public string? Ip { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? CreatedDate { get; set; }

    [Column(TypeName = "bit(1)")]
    public BitArray? IsDeleted { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? ModifiedDate { get; set; }

    [StringLength(250)]
    public string? DeclinedBy { get; set; }

    [Column(TypeName = "bit(1)")]
    public BitArray? IsUrgentEmailSent { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? LastWellnessDate { get; set; }

    [Column(TypeName = "bit(1)")]
    public BitArray? IsMobile { get; set; }

    public short? CallType { get; set; }

    [Column(TypeName = "bit(1)")]
    public BitArray? CompletedByPhysician { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? LastReservationDate { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? AcceptedDate { get; set; }

    [StringLength(100)]
    public string? RelationName { get; set; }

    [StringLength(50)]
    public string? CaseNumber { get; set; }

    [StringLength(50)]
    public string? CaseTag { get; set; }

    [StringLength(50)]
    public string? CaseTagPhysician { get; set; }

    [StringLength(128)]
    public string? PatientAccountId { get; set; }

    public int? CreatedUserId { get; set; }

    [InverseProperty("Request")]
    public virtual ICollection<RequestBusiness> RequestBusinesses { get; set; } = new List<RequestBusiness>();

    [InverseProperty("Request")]
    public virtual ICollection<RequestConcierge> RequestConcierges { get; set; } = new List<RequestConcierge>();

    [InverseProperty("Request")]
    public virtual ICollection<RequestWiseFile> RequestWiseFiles { get; set; } = new List<RequestWiseFile>();

    [InverseProperty("Request")]
    public virtual ICollection<Requestclient> Requestclients { get; set; } = new List<Requestclient>();

    [ForeignKey("UserId")]
    [InverseProperty("Requests")]
    public virtual User? User { get; set; }
}
