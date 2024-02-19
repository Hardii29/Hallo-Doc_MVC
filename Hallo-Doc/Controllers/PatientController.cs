using Hallo_Doc.Data;
using Hallo_Doc.Models;
using Hallo_Doc.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace Hallo_Doc.Controllers
{
    public class PatientController : Controller
    {
        private readonly ApplicationDbContext _context;
        private static Dictionary<string, string> ResetTokens = new Dictionary<string, string>();
        public PatientController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Login()
        {
            return View();
        }
       
     
        public IActionResult Create_request()
        {
            return View("Create_request");
        }
        public IActionResult Create_patient_req()
        {
            return View("Create_patient_req");
        }
        public IActionResult Create_family_req()
        {
            return View("Create_family_req");
        }
        public IActionResult Create_business_req()
        {
            return View("Create_business_req");
        }
        public IActionResult Create_concierge_req()
        {
            return View("Create_concierge_req");
        }
        public async Task<IActionResult> Patient_dashboard()
        {
            var model = new DashboardList
            {
                Requests = _context.Requests.ToList()
            };

            return View(model);
        }
        public IActionResult View_document()
        {
            var files = new DashboardList
            {
                RequestWiseFiles = _context.RequestWiseFiles.ToList()
        };
            
            return View(files);
        }
        [HttpGet]
        public IActionResult DownloadFile(int fileID)
        {
            var file = _context.RequestWiseFiles.FirstOrDefault(f => f.RequestWiseFileId == fileID);
            if (file == null)
            {
                return NotFound();
            }
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Documents", file.FileName);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }
            return PhysicalFile(filePath, "application/octet-stream" ,file.FileName);
        }

        [AllowAnonymous, HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [AllowAnonymous, HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPassword fp)
        {
            if (!string.IsNullOrWhiteSpace(fp.Email))
            {
                var UserExist = await _context.AspnetUsers
                    .FirstOrDefaultAsync(m => m.Email == fp.Email);
                if (UserExist == null)
                {
                    return NotFound();
                }
                else
                {
                    var token = Guid.NewGuid().ToString();
                    SavePasswordResetToken(fp.Email, token);
                    var resetLink = Url.Action("Reset_password", "Patient", new { email = fp.Email, token = token }, "http");
                    SendEmail(fp.Email, resetLink);
                    return RedirectToAction("ForgotPassword");
                }
            }
            ModelState.AddModelError("", "The email address provided is not registered.");
            return View();
        }
        private void SavePasswordResetToken(string email, string token)
        {
            ResetTokens[email] = token;
        }
        private void SendEmail(string email, string resetLink)
        {
            var smtpServer = "mail.etatvasoft.com";
            var smtpPort = 587;
            var smtpUsername = "hardi.jayani";
            var smtpPassword = "LHV0@}YOA?)M";
            var senderEmail = "hardi.jayani@etatvasoft.com";

            var client = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                EnableSsl = true
            };
            var mail = new MailMessage
            {
                From = new MailAddress(senderEmail),
                Subject = "Password Reset",
                Body = "Please reset your password by clicking the following link: <a href='" + resetLink + "'>" + resetLink + "</a>",
                IsBodyHtml = true
            };
            mail.To.Add(email);
            client.Send(mail);
        }

        [AllowAnonymous, HttpGet]
        public IActionResult Reset_password(string email, string token)
        {
            var isValid = ValidateResetToken(email, token);
            if (!isValid)
            {
                return RedirectToAction(nameof(ForgotPassword));
            }
            var model = new ForgotPassword
            {
                Email = email,
                Token = token
            };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult Reset_password(ForgotPassword model)
        {
            if (ModelState.IsValid)
            {
                var isValid = ValidateResetToken(model.Email, model.Token);
                if (!isValid)
                {
                    return RedirectToAction("ForgotPassword");
                }
                var passwordUpdate = UpdatePassword(model.Email, model.Password);
                if (passwordUpdate)
                {
                    return RedirectToAction("Login", "Patient");
                }
                else
                {
                    return RedirectToAction("ForgotPassword");
                }
            }
            return View(model);
        }

        private bool UpdatePassword(string email, string password)
        {
            try
            { 
            var user = _context.AspnetUsers.FirstOrDefault(x => x.Email == email);
            if (user != null)
            {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
                user.Passwordhash = hashedPassword;
                _context.Update(user);
                _context.SaveChanges();
                return true;
            }
            return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating password: {ex.Message}");
                return false;
            }
        }

        private bool ValidateResetToken(string email, string token)
        {
            var savedToken = ResetTokens[email];
            return token.Equals(savedToken);
        }
        public IActionResult Patient_profile()
        {
            return View("Patient_profile");
        }
        public IActionResult Submit_req_Me()
        {
            return View("Submit_req_Me");
        }
        public IActionResult Submit_req_Someone()
        {
            return View("Submit_req_Someone");
        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
