using System;
using System.Collections;
using System.Collections.Generic;

namespace Hallo_Doc.Models;

public partial class AspnetUser
{
    public string Id { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string? Passwordhash { get; set; }

    public string? Securitystamp { get; set; }

    public string? Email { get; set; }

    public BitArray Emailconfirmed { get; set; } = null!;

    public string? Phonenumber { get; set; }

    public BitArray Phonenumberconfirmed { get; set; } = null!;

    public BitArray Twofactorenabled { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
