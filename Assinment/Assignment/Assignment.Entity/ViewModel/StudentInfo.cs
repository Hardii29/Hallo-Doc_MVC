using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Entity.ViewModel
{
    public class StudentInfo
    {
        public int StudentId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter valid Email Address.")]
        public string? Email { get; set; }
        public int CourseId { get; set; }
        public string? CourseName { get;set; }
        public int Age { get; set; }
        public string? Gender { get; set; }
        public string? Grade { get; set; }

    }
}
