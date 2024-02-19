using Hallo_Doc.Entity.Data;
using Hallo_Doc.Entity.ViewModel;
using Hallo_Doc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hallo_Doc.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hallo_Doc.Repository.Repository.Implementation
{
    public class LoginRepo : ILogin
    {
        private readonly ApplicationDbContext _context;

        public LoginRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        //public IActionResult Index()
        //{
        //    return View();
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<string> Check(Login login)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(login.Password);

            var User = await _context.AspnetUsers
                .FirstOrDefaultAsync(m => m.Email == login.Email);
            if (User == null)
            {
                return "/Patient/Login";
            }
            bool pass = BCrypt.Net.BCrypt.Verify(login.Password, hashedPassword);
            if (!pass)
            {
                return "/Patient/Login";

            }
            return "/Patient/Patient_dashboard";
            //return RedirectToAction("Patient_dashboard", "Patient");


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

      
        //[HttpPost]
        //public IActionResult ForgotPassword(Reset_password model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    var user = _context.AspnetUsers.FirstOrDefault(u => u.Email == model.Email);
        //    if (user != null)
        //    {
        //        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
        //        user.Passwordhash = hashedPassword;
        //        _context.SaveChanges();

        //        return RedirectToAction("Login", "Patient");
        //    }

        //    ModelState.AddModelError(string.Empty, "Email not found.");
        //    return View(model);
        //}
        // GET: /Login/ForgotPassword
        //        public IActionResult ForgotPassword()
        //        {
        //            return View();
        //        }

        //        [HttpPost]
        //        [ValidateAntiForgeryToken]
        //        public async Task<IActionResult> ForgotPassword(ResetPassword model)
        //        {
        //            if (ModelState.IsValid)
        //            {
        //                var user = await _context.AspnetUsers.FindByEmailAsync(model.Email);
        //                if (user != null)
        //                {
        //                    var token = await _context.GeneratePasswordResetTokenAsync(user);
        //                    var callbackUrl = Url.Action("ResetPassword", "Login", new { userId = user.Id, token = token }, protocol: HttpContext.Request.Scheme);

        //                    SendResetPasswordEmail(model.Email, callbackUrl);

        //                    ViewBag.EmailSent = true; 
        //                    return View();
        //                }

        //            }
        //            return View(model);
        //        }

        //        public IActionResult ResetPassword(string userId, string token)
        //        {
        //            var model = new ResetPassword { AspnetuserId = userId, Token = token };
        //            return View(model);
        //        }

        //        [HttpPost]
        //        [ValidateAntiForgeryToken]
        //        public async Task<IActionResult> ResetPassword(ResetPassword model)
        //        {
        //            if (ModelState.IsValid)
        //            {
        //                var user = await _context.FindByIdAsync(model.UserId);
        //                if (user != null)
        //                {
        //                    var result = await _context.ResetPasswordAsync(user, model.Token, model.Password);
        //                    if (result.Succeeded)
        //                    {
        //                        TempData["PasswordUpdated"] = true; 
        //                        return RedirectToAction("Login", "Account");
        //                    }

        //                }

        //            }
        //            return View(model);
        //        }

        //        private void SendResetPasswordEmail(string email, string callbackUrl)
        //        {

        //            var mailMessage = new MailMessage();
        //            mailMessage.To.Add(email)
        //;
        //            mailMessage.Subject = "Reset Your Password";
        //            mailMessage.Body = $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>.";
        //            mailMessage.IsBodyHtml = true;

        //            using (var smtpClient = new SmtpClient("your.smtp.server"))
        //            {
        //                smtpClient.Send(mailMessage);
        //            }
        //        }
    }
}
