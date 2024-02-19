using Hallo_Doc.Data;
using Microsoft.AspNetCore.Mvc;
using Hallo_Doc.Models;
using Npgsql;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Hallo_Doc.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using System.Net.Mail;
using NuGet.Protocol.Plugins;
using Microsoft.AspNetCore.Authorization;

namespace Hallo_Doc.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;
        
        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Check(Login login)
        {
            
            
            var User = await _context.AspnetUsers
                .FirstOrDefaultAsync(m => m.Email == login.Email);
            if (User == null)
            {
                return NotFound();
            }
            bool pass = BCrypt.Net.BCrypt.Verify(login.Password, User.Passwordhash);
            if (!pass) {
                return NotFound();
            
            }
            return RedirectToAction("Patient_dashboard", "Patient");
        
        }
       
    }
}
    

