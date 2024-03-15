using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hallo_Doc.Entity.ViewModel
{
    public class ViewCase
    {
        public int RequestId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateOnly? DOB { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public string? Region { get; set; }
        public int RequestWiseFileID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? FileName { get; set; }
        public IFormFile? File { get; set; }
    }
}
