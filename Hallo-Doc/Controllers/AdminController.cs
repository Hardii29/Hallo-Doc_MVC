using Hallo_Doc.Data;
using Microsoft.AspNetCore.Mvc;

namespace Hallo_Doc.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Admin_dashboard()
        {
            return View("Admin_dashboard");
        }
    }
}
