﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hallo_Doc.Entity.ViewModel
{
    public class AdminDash
    {
        public int RequestId { get; set; }
        public int RequestTypeId { get; set; }
        public string? PatientName { get; set; }
        public DateOnly? DOB { get; set; }
        public string? Requestor { get; set; }
        public DateTime? RequestedDate { get; set; }
        public string? Email { get; set; }
        public string? Region { get; set; }
        public string? PatientMobile {  get; set; }
        public string? ProviderName { get; set; }
        public string?  Address { get; set; }
        public string? Notes { get; set; }
        public int? RegionId { get; set; }
        public int? PhysicianId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? RequestorPhoneNumber { get; set; }
        public string? Name { get; set; }
        public int? CaseTagId { get; set; }
        public short? Status { get; set; }
        
    }
}
