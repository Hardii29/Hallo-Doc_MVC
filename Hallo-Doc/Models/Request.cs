using System;
using System.Collections.Generic;

namespace Hallo_Doc.Models;

public partial class Request
{
    public int RequestId { get; set; }

    public int? RequestTypeId { get; set; }

    public int? UserId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public short? Status { get; set; }

    public int? PhysicianId { get; set; }

    public string? ConfirmationNumber { get; set; }

    public string? Ip { get; set; }

    public virtual User? User { get; set; }
}
