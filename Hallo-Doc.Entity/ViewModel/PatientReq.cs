﻿using Microsoft.AspNetCore.Http;
namespace Hallo_Doc.Entity.ViewModel
{
    public class PatientReq
    {
        public required string Id { get; set; }
        public required string Aspnetuserid { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? DOB { get; set; }
        public required string Email { get; set; }
        public string? Password { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? Mobile { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public int RequestWiseFileId { get; set; }
        public IFormFile? File { get; set; }
        public int RequestId { get; set; }
        public string FileName { get; set; } = null!;
    }
}