using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Hallo_Doc.Entity.Models;

[Table("ChatHistory")]
public partial class ChatHistory
{
    [Key]
    public int ChatId { get; set; }

    [Column(TypeName = "character varying")]
    public string Reciever { get; set; } = null!;

    [Column(TypeName = "character varying")]
    public string Sender { get; set; } = null!;

    [Column(TypeName = "character varying")]
    public string? Message { get; set; }

    public bool? IsRead { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? CreatedDate { get; set; }

    [ForeignKey("Reciever")]
    [InverseProperty("ChatHistoryRecieverNavigations")]
    public virtual AspnetUser RecieverNavigation { get; set; } = null!;

    [ForeignKey("Sender")]
    [InverseProperty("ChatHistorySenderNavigations")]
    public virtual AspnetUser SenderNavigation { get; set; } = null!;
}
