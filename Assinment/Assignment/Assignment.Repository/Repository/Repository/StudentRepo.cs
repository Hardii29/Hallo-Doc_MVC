using Assignment.Entity.Data;
using Assignment.Entity.Models;
using Assignment.Entity.ViewModel;
using Assignment.Repository.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Repository.Repository.Repository
{
    public class StudentRepo : IStudent
    {
        private readonly ApplicationDbContext _context;
        public StudentRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public void AddStudent(string FirstName, string LastName, string Email, DateTime BirthDate, string Gender, string Grade, string Course)
        {
            int age = 0;
            age = DateTime.Now.Year - BirthDate.Year;
            if (DateTime.Now.DayOfYear < BirthDate.DayOfYear)
                age = age - 1;
            var CourseExist = _context.Courses.FirstOrDefault(c => c.Name == Course);
            if (CourseExist == null)
            {
                var c = new Course();
                var lastPhy = _context.Courses.OrderByDescending(p => p.CourseId).FirstOrDefault();
                var lastid = lastPhy != null ? lastPhy.CourseId : 1000;
                int newId = lastid + 1;
                c.CourseId = newId;
                c.Name = Course;
                _context.Courses.Add(c);
                _context.SaveChanges();
            }
            var Student = new StudentDatum();
            Student.FirstName = FirstName;
            Student.LastName = LastName;
            Student.Email = Email;
            Student.Age = age;
            Student.Gender = Gender;
            Student.Grade = Grade;
            Student.Course = Course;
            Student.CourseId = _context.Courses.FirstOrDefault(c => c.Name == Course).CourseId;
            Student.IsDeleted = false;
            _context.StudentData.Add(Student);
            _context.SaveChanges();
        }
        public List<StudentInfo> GetStudents(string searchValue)
        {
            List<StudentInfo> list = (from s in _context.StudentData
                                      where s.IsDeleted == false && (searchValue == null ||
                               s.FirstName.Contains(searchValue) || s.LastName.Contains(searchValue) ||
                               s.Email.Contains(searchValue) || s.Course.Contains(searchValue) ||
                               s.Gender.Contains(searchValue) || s.Grade.Contains(searchValue))
                                      orderby s.StudentId
                                     select new StudentInfo
                                     {
                                         StudentId = s.StudentId,
                                         FirstName = s.FirstName,
                                         LastName = s.LastName,
                                         Email = s.Email,
                                         CourseName = s.Course,
                                         Grade = s.Grade,
                                         Gender = s.Gender,
                                         Age = (int)s.Age,

                                     }).ToList();
            return list;
        }
        public void EditStudent(int StudentId, string FirstName, string LastName, string Email, DateTime BirthDate, string Gender, string Grade, string Course)
        {
            int age = 0;
            age = DateTime.Now.Year - BirthDate.Year;
            if (DateTime.Now.DayOfYear < BirthDate.DayOfYear)
                age = age - 1;

            var Student = _context.StudentData.FirstOrDefault(s => s.StudentId == StudentId);
            Student.FirstName = FirstName;
            Student.LastName = LastName;
            Student.Email = Email;
            Student.Age = age;
            Student.Gender = Gender;
            Student.Grade = Grade;
            Student.Course = Course;
            Student.CourseId = _context.Courses.FirstOrDefault(c => c.Name == Course).CourseId;
            _context.StudentData.Update(Student);
            _context.SaveChanges();
        }
        public void DeleteStudent(int StudentId)
        {
            var data = _context.StudentData.FirstOrDefault(s => s.StudentId ==StudentId);
            _context.StudentData.RemoveRange(data);
            _context.SaveChanges();
        }
    }
}
