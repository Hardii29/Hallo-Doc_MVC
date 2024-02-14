using System;
using System.Collections.Generic;

namespace Hallo_Doc.Models;

public partial class AspnetUser
{
    public string Id { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string? Passwordhash { get; set; }

    public string? Email { get; set; }

    public string? Phonenumber { get; set; }

    public string? Ip { get; set; }

    public DateTime Createddate { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
