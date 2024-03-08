using Hallo_Doc.Entity.Data;
using Hallo_Doc.Entity.Models;
using Hallo_Doc.Entity.ViewModel;
using Hallo_Doc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        
   
        public IActionResult Patient_dashboard()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            string username = HttpContext.Session.GetString("UserName");
            var userRequests = _context.Requests.Where(r => r.UserId == userId).ToList();
            var requestWithFiles = new List<RequestWithFile>();
            foreach (var request in userRequests)
            {
                bool HasFiles = _context.RequestWiseFiles.Any(rwf => rwf.RequestId == request.RequestId);
                var fileId = 0;
                if(HasFiles)
                {
                    fileId = _context.RequestWiseFiles.FirstOrDefault(rwf => rwf.RequestId == request.RequestId)?.RequestWiseFileId ?? 0;
                }
                requestWithFiles.Add(new RequestWithFile
                { 
                    Request = new Entity.ViewModel.Request
                    {
                        RequestId = request.RequestId,
                        CreatedDate= (DateTime)request.CreatedDate,
                        Status = request.Status,
                        fileId = fileId,
                    },
                    HasFiles = HasFiles 
                });
            }
            var model = new DashboardList
            {
                RequestWithFiles = requestWithFiles,
                Requests = userRequests.Select(r => new Entity.ViewModel.Request
                {
                    RequestId = r.RequestId,
                    CreatedDate = (DateTime)r.CreatedDate,
                    Status = r.Status,
                   
                }).ToList(),
                UserName = username,
            };

            return View(model);
        }
        public IActionResult View_document(int fileId)
        {
            var files = _context.RequestWiseFiles.FirstOrDefault(rwf => rwf.RequestWiseFileId == fileId);
            if (files == null)
            {
                return NotFound();
            }
            var model = new ViewDocument 
                { 
                    RequestWiseFileID = files.RequestWiseFileId,
                    CreatedDate = files.CreatedDate,
                    FileName = files.FileName,
                
            };
            
            return View(model);
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

        private static bool ValidateResetToken(string email, string token)
        {
            var savedToken = ResetTokens[email];
            return token.Equals(savedToken);
        }
        public IActionResult Patient_profile()
        {
            return View("Patient_profile");
        }
        [HttpGet]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string email, [Bind("Model")] PatientReq patientReq)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u =>u.Email == email);
                    if (user == null)
                    {
                        return NotFound();
                    }

                    user.Email = patientReq.Email;
                    user.Firstname = patientReq.FirstName;
                    user.Lastname = patientReq.LastName;
                    user.Mobile = patientReq.Mobile;
                    user.Street = patientReq.Street;
                    user.City = patientReq.City;
                    user.State = patientReq.State;
                    user.Zipcode = patientReq.ZipCode;
                    //user.Dob = patientReq.DOB;
                        
                        _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Patient_profile");
                    
                }
                catch (DbUpdateConcurrencyException)
                {
                    
                        throw;
                    
                }
            }
            return View(patientReq);
        }
        private bool UserExists(string email)
        {
            return (_context.Users?.Any(e => e.Email == email)).GetValueOrDefault();
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
