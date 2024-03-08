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
                    Request = new Request
                    {
                        RequestId = request.RequestId,
                        CreatedDate = (DateTime)request.CreatedDate,
                        Status = request.Status,
                        fileId = fileId,
                    },
                    HasFiles = HasFiles
                });
            }
            var model = new DashboardList
            {
                RequestWithFiles = requestWithFiles,
                Requests = userRequests.Select(r => new Request
                {
                    RequestId = r.RequestId,
                    CreatedDate = (DateTime)r.CreatedDate,
                    Status = r.Status,
                    
                }).ToList(),
                UserName = username,
                UserId = userId,
            };

            return model;
        }
        public ViewDocument ViewDocument(int fileId, HttpContext httpContext)
        {
            int userId = httpContext.Session.GetInt32("UserId") ?? 0;
            string username = httpContext.Session.GetString("UserName");
            var files = _context.RequestWiseFiles.FirstOrDefault(rwf => rwf.RequestWiseFileId == fileId);
            if (files == null)
            {
                return null;
            }
            var model = new ViewDocument
            {
                RequestWiseFileID = files.RequestWiseFileId,
                CreatedDate = files.CreatedDate,
                FileName = files.FileName,

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
                            DOB = u.Dob,
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
            user.Dob = patientProfile.DOB;

            //_context.Users.Update(user);
            _context.SaveChanges();

            
        }
    }
}
