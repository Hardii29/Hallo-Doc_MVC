using Hallo_Doc.Data;
using Hallo_Doc.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Hallo_Doc.Controllers
{
    public class CheckEmailExistController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CheckEmailExistController(ApplicationDbContext context)
        {
            _context = context;
        }
        //public IActionResult Index()
        //{
        //    return View(new PatientReq());
        //}
        [HttpPost]
        public IActionResult checkEmail(string email)
        {
            var exists = _context.AspnetUsers.Any(u => u.Email == email);
            return Ok(exists);
            
        }
    }
}
