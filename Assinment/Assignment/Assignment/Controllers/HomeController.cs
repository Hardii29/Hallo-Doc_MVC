using Assignment.Models;
using Assignment.Repository.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Assignment.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IStudent _student;
        public HomeController(ILogger<HomeController> logger, IStudent student)
        {
            _logger = logger;
            _student = student;
        }

        public IActionResult Index(string searchValue)
        {
            var model = _student.GetStudents(searchValue);
            return View(model);
        }
        [HttpPost]
        public IActionResult AddStudent(string FirstName, string LastName, string Email, DateTime BirthDate, string Gender, string Grade, string Course)
        {
            _student.AddStudent(FirstName, LastName, Email, BirthDate, Gender, Grade, Course);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult EditStudent(int StudentId, string FirstName, string LastName, string Email, DateTime BirthDate, string Gender, string Grade, string Course)
        {
            _student.EditStudent(StudentId, FirstName, LastName, Email, BirthDate, Gender, Grade, Course);
            return RedirectToAction("Index");
        }
        public IActionResult DeleteRecord(int StudentId)
        {
            _student.DeleteStudent(StudentId);
            return RedirectToAction("Index");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}