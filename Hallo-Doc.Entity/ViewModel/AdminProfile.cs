﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hallo_Doc.Entity.ViewModel
{
    public class AdminProfile
    {
        public int? UserId { get; set; }
        public string? Password { get; set; }
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? DOB { get; set; }
        public string? Mobile { get; set; }
        public string? Email { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? Region { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? AdminName { get; set; }
        public int AdminId { get; set; } = 0;
        public string AltPhone { get; set; }
        public int RoleId { get; set; }
        public int RegionId { get; set; }
    }
}
