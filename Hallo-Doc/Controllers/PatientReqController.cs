using Microsoft.AspNetCore.Mvc;
using Hallo_Doc.Data;
using Hallo_Doc.Models;
using Hallo_Doc.Models.ViewModel;
using System.Collections;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;
using Microsoft.CodeAnalysis.Scripting;
using BCrypt.Net;
using System.IO;

namespace Hallo_Doc.Controllers
{
    public class PatientReqController : Controller
    {
        private readonly ApplicationDbContext _context;
        public PatientReqController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View("../Patient/Create_patient_req");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
   
        public async Task<IActionResult> Create(Models.ViewModel.PatientReq PatientReq)
        {
            var existUser = await _context.AspnetUsers.FirstOrDefaultAsync(u => u.Email == PatientReq.Email);
            var UserExist = await _context.Users.FirstOrDefaultAsync(m => m.Email == PatientReq.Email);
            if (existUser == null) { 
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(PatientReq.Password);

                var Aspnetuser = new Models.AspnetUser();
            var User = new Models.User();
            var Request = new Models.Request();
            var Requestclient = new Models.Requestclient();
            var RequestType = new Models.RequestType();

                Aspnetuser.Id = Guid.NewGuid().ToString();
            Aspnetuser.Username = PatientReq.FirstName;
            Aspnetuser.Email = PatientReq.Email;
                Aspnetuser.Passwordhash = hashedPassword;
                Aspnetuser.Phonenumber = PatientReq.Mobile;
            Aspnetuser.Createddate = DateTime.Now;
            _context.AspnetUsers.Add(Aspnetuser);
            await _context.SaveChangesAsync();

            User.Aspnetuserid = Aspnetuser.Id;
            User.Firstname = PatientReq.FirstName;
            User.Lastname = PatientReq.LastName;
            User.Email = PatientReq.Email;
            User.Mobile = PatientReq.Mobile;
            User.Street = PatientReq.Street;
            User.City = PatientReq.City;
            User.State = PatientReq.State;
            User.Zipcode = PatientReq.ZipCode;
            User.Createdby = "123";
            User.Createddate = DateTime.Now;
            _context.Users.Add(User);
            await _context.SaveChangesAsync();

            RequestType.Name = "Patient";
            _context.RequestTypes.Add(RequestType);
            await _context.SaveChangesAsync();

            Request.RequestTypeId = 2;
            Request.UserId = User.Userid;
            Request.FirstName = PatientReq.FirstName;
            Request.LastName = PatientReq.LastName;
            Request.Email = PatientReq.Email;
            Request.PhoneNumber = PatientReq.Mobile;
            Request.Status = 4;
                Request.CreatedDate = DateTime.Now;
                _context.Requests.Add(Request);
            await _context.SaveChangesAsync();

            Requestclient.RequestId = Request.RequestId;
            Requestclient.FirstName = PatientReq.FirstName;
            Requestclient.LastName = PatientReq.LastName;
            Requestclient.Address = PatientReq.Street;
            Requestclient.Email = PatientReq.Email;
            Requestclient.PhoneNumber = PatientReq.Mobile;
            _context.Requestclients.Add(Requestclient);
            await _context.SaveChangesAsync();

            if (PatientReq.File != null && PatientReq.File.Length > 0)
            {
                var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Documents");

                if (!Directory.Exists(uploadsDirectory))
                {
                    Directory.CreateDirectory(uploadsDirectory);
                }

                var fileName = Path.GetFileName(PatientReq.File.FileName);
                var filePath = Path.Combine(uploadsDirectory, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await PatientReq.File.CopyToAsync(stream)
;
                }

                var requestWiseFile = new Models.RequestWiseFile
                {
                    RequestId = Request.RequestId,
                    FileName = fileName,
                    CreatedDate = DateTime.Now,
                };
                //PatientReq.FileName = filePath;
                _context.RequestWiseFiles.Add(requestWiseFile);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Home");
            }
            else
            {
               
                var Request = new Models.Request();
                var Requestclient = new Models.Requestclient();
                var RequestType = new Models.RequestType();

              

                RequestType.Name = "Patient";
                _context.RequestTypes.Add(RequestType);
                await _context.SaveChangesAsync();

                Request.RequestTypeId = 2;
                Request.UserId = UserExist.Userid;
                Request.FirstName = PatientReq.FirstName;
                Request.LastName = PatientReq.LastName;
                Request.Email = PatientReq.Email;
                Request.PhoneNumber = PatientReq.Mobile;
                Request.Status = 4;
                Request.CreatedDate = DateTime.Now;
                _context.Requests.Add(Request);
                await _context.SaveChangesAsync();

                Requestclient.RequestId = Request.RequestId;
                Requestclient.FirstName = PatientReq.FirstName;
                Requestclient.LastName = PatientReq.LastName;
                Requestclient.Address = PatientReq.Street;
                Requestclient.Email = PatientReq.Email;
                Requestclient.PhoneNumber = PatientReq.Mobile;
                _context.Requestclients.Add(Requestclient);
                await _context.SaveChangesAsync();

                if (PatientReq.File != null && PatientReq.File.Length > 0)
                {
                    var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Documents");

                    if (!Directory.Exists(uploadsDirectory))
                    {
                        Directory.CreateDirectory(uploadsDirectory);
                    }

                    var fileName = Path.GetFileName(PatientReq.File.FileName);
                    var filePath = Path.Combine(uploadsDirectory, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await PatientReq.File.CopyToAsync(stream)
    ;
                    }

                    var requestWiseFile = new Models.RequestWiseFile
                    {
                        RequestId = Request.RequestId,
                        FileName = fileName,
                        CreatedDate = DateTime.Now,
                    };
                    //PatientReq.FileName = filePath;
                    _context.RequestWiseFiles.Add(requestWiseFile);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("Index", "Home");
            }
        }
   
    }
}
