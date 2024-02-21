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
using Microsoft.AspNetCore.Http;

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
            var userDetails = await _context.Users.FirstOrDefaultAsync(m => m.Aspnetuserid == User.Id);
            if (userDetails == null)
            {
                return NotFound(nameof(Login));
            }
            login.FirstName = userDetails.Firstname;
            login.LastName = userDetails.Lastname;
            string userName = $"{login.FirstName} {login.LastName}";
            HttpContext.Session.SetInt32("UserId", userDetails.Userid);
            HttpContext.Session.SetString("UserName", User.Username);
            return RedirectToAction("Patient_dashboard", "Patient");
        
        }
       
    }
}
    

