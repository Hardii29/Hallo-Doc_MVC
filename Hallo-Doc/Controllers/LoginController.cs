using Hallo_Doc.Data;
using Microsoft.AspNetCore.Mvc;
using Hallo_Doc.Models;
using Npgsql;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Hallo_Doc.Models.ViewModel;
using Microsoft.AspNetCore.Identity;

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
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(login.Password);
            //var Aspnetuser = new AspnetUser();
            //Aspnetuser.Email = login.Email;
            //Aspnetuser.Passwordhash = hashedPassword;
            //if (Email == null || Passwordhash == null)
            //{
            //    return NotFound("Email and Password Required.");
            //}
           
            var User = await _context.AspnetUsers
                .FirstOrDefaultAsync(m => m.Email == login.Email);
            if (User == null)
            {
                return NotFound();
            }
            bool pass = BCrypt.Net.BCrypt.Verify(login.Password, hashedPassword);
            if (!pass) {
                return NotFound();
            
            }
            return RedirectToAction("Patient_dashboard", "Patient");
        
           
            //NpgsqlConnection connection = new NpgsqlConnection("Server=localhost;Database=Hallo-Doc;Username=postgres;Password=&^54UYtr;Include Error Detail=True");
            //string Query = "SELECT * FROM \"ASPNetUsers\" where \"email\"=@Email and \"passwordhash \"=@PasswordHash";
            //connection.Open();
            //NpgsqlCommand command = new NpgsqlCommand(Query, connection);
            //command.Parameters.AddWithValue("@Email", Email);
            //command.Parameters.AddWithValue("@PasswordHash", PasswordHash);
            //NpgsqlDataReader reader = command.ExecuteReader();
            //DataTable dataTable = new DataTable();
            //dataTable.Load(reader);
            //int numRows = dataTable.Rows.Count;
            //if (numRows > 0)
            //{
            //    foreach (DataRow row in dataTable.Rows)
            //    {
            //        HttpContext.Session.SetString("UserName", row["username"].ToString());
            //        HttpContext.Session.SetString("UserID", row["Id"].ToString());
            //    }
            //    return RedirectToAction("Patient_dashboard", "Patient");
            //}
            //else
            //{
            //    ViewData["Error"] = " Your Username or password is incorrect. ";
            //    return View("../Patient/Login");
            //}
        }
    }
    }

