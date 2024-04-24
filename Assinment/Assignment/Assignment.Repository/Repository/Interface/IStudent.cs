using Assignment.Entity.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Repository.Repository.Interface
{
    public interface IStudent
    {
        void AddStudent(string FirstName, string LastName, string Email, DateTime BirthDate, string Gender, string Grade, string Course);
        List<StudentInfo> GetStudents(string searchValue);
        void EditStudent(int StudentId, string FirstName, string LastName, string Email, DateTime BirthDate, string Gender, string Grade, string Course);
        void DeleteStudent(int StudentId);
    }
}
