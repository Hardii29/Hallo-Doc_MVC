using Hallo_Doc.Entity.Data;
using Hallo_Doc.Entity.ViewModel;
using Hallo_Doc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Web;
using System.Net.Http;
using Hallo_Doc.Entity.Models;

namespace Hallo_Doc.Repository.Repository.Implementation
{
    public class PatientUserRepo : IPatientUser
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public PatientUserRepo(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        
        public DashboardList PatientDashboard(HttpContext httpContext)
        {
            int userId = httpContext.Session.GetInt32("UserId") ?? 0;
            string username = httpContext.Session.GetString("UserName");
            var userRequests = _context.Requests.Where(r => r.UserId == userId).ToList();
            var requestWithFiles = new List<RequestWithFile>();
            foreach (var request in userRequests)
            {
                bool HasFiles = _context.RequestWiseFiles.Any(rwf => rwf.RequestId == request.RequestId);
                var fileId = 0;
                if (HasFiles)
                {
                    fileId = _context.RequestWiseFiles.FirstOrDefault(rwf => rwf.RequestId == request.RequestId)?.RequestWiseFileId ?? 0;
                }
                requestWithFiles.Add(new RequestWithFile
                {
                    Request = new Entity.ViewModel.Request
                    {
                        RequestId = request.RequestId,
                        CreatedDate = (DateTime)request.CreatedDate,
                        Status = (status)request.Status,
                        fileId = fileId,
                        AspAdminId = _context.Admins.FirstOrDefault(a => a.AdminId == 1).AspNetUserId,
                        AspPhysicianId = _context.Physicians.FirstOrDefault(p => p.PhysicianId == 9).AspNetUserId,
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
                    Status = (status)r.Status,
                    
                }).ToList(),
                UserName = username,
                UserId = userId,
            };

            return model;
        }
        public ViewDocument ViewDocument(int RequestId, HttpContext httpContext)
        {
            int userId = httpContext.Session.GetInt32("UserId") ?? 0;
            string username = httpContext.Session.GetString("UserName");
            var files = from f in _context.RequestWiseFiles
                        where f.RequestId == RequestId
                        select new ViewDocument
                        {
                            RequestWiseFileID = f.RequestWiseFileId,
                            CreatedDate = f.CreatedDate,
                            FileName = f.FileName
                        };
            var model = new ViewDocument()
            {
                RequestId = RequestId,
                documents = files.ToList(),
                UserName = username,
            };

            return model;
        }
        public IActionResult DownloadFile(int fileID)
        {
            var file = _context.RequestWiseFiles.FirstOrDefault(f => f.RequestWiseFileId == fileID);
            if (file == null)
            {
                return null;
            }
            var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Documents", file.FileName);
            if (!File.Exists(filePath))
            {
                return null;
            }
            var memory = new MemoryStream();
            using(var stream = new FileStream(filePath, FileMode.Open))
            {
                stream.CopyTo(memory);
            }
            memory.Position=0;
            return new FileStreamResult(memory, "application/octet-stream")
            {
                FileDownloadName = file.FileName
            };
        }
        public PatientProfile? Edit(int userId, HttpContext httpContext)
        {
            int sessionId = httpContext.Session.GetInt32("UserId") ?? 0;
            string Username = httpContext.Session.GetString("UserName");
            if (sessionId != userId)
            {
                return null;
            }
            var model = (from u in _context.Users
                        where u.Userid == userId
                        select new PatientProfile
                         {
                            FirstName = u.Firstname, 
                            LastName = u.Lastname,
                            DOB = u != null && u.Intyear != null && u.Strmonth != null && u.Intdate != null ?
                            new DateOnly((int)u.Intyear, int.Parse(u.Strmonth), (int)u.Intdate) : DateOnly.MinValue,
                            Mobile = u.Mobile,
                            Email = u.Email,
                            State = u.State,
                            Street = u.Street,
                            City = u.City,
                            ZipCode = u.Zipcode,
                            UserName = Username,
                            UserId = userId
                         }).FirstOrDefault();
           
            return model;
        }
        public void EditProfile(int userId, PatientProfile patientProfile)
        {
            patientProfile.UserId = userId;
            var user = _context.Users.FirstOrDefault(u => u.Userid == userId);
            
            user.Email = patientProfile.Email;
            user.Firstname = patientProfile.FirstName;
            user.Lastname = patientProfile.LastName;
            user.Mobile = patientProfile.Mobile;
            user.Street = patientProfile.Street;
            user.City = patientProfile.City;
            user.State = patientProfile.State;
            user.Zipcode = patientProfile.ZipCode;
            user.Intyear = patientProfile.DOB.Value.Year;
            user.Strmonth = patientProfile.DOB.Value.Month.ToString();
            user.Intdate = patientProfile.DOB.Value.Day;

            _context.Users.Update(user);
            _context.SaveChanges();

            
        }
    }
}
