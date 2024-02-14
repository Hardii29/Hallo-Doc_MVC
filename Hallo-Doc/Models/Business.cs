using System;
using System.Collections;
using System.Collections.Generic;

namespace Hallo_Doc.Models;

public partial class Business
{
    public int BusinessId { get; set; }

    public string Name { get; set; } = null!;

    public string? Address1 { get; set; }

    public string? Address2 { get; set; }

    public string? City { get; set; }

    public int? RegionId { get; set; }

    public string? ZipCode { get; set; }

    public string? PhoneNumber { get; set; }

    public string? FaxNumber { get; set; }

    public BitArray? IsRegistered { get; set; }

    public DateTime CreatedDate { get; set; }

    public short? Status { get; set; }

    public BitArray? IsDeleted { get; set; }

    public string? Ip { get; set; }

    public virtual ICollection<RequestBusiness> RequestBusinesses { get; set; } = new List<RequestBusiness>();
}
