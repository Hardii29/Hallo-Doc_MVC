using Hallo_Doc.Entity.Data;
using Hallo_Doc.Entity.Models;
using Hallo_Doc.Entity.ViewModel;
using Hallo_Doc.Repository.Repository.Interface;

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hallo_Doc.Repository.Repository.Implementation
{
    public class PatientReqRepo : IPatientReq
    {
        private readonly ApplicationDbContext _context;

        public PatientReqRepo(ApplicationDbContext context)
        {
            _context = context;

        }
        public bool checkEmail(string email)
        {
            var exists = _context.AspnetUsers.Any(u => u.Email == email);
            if (exists)
            {
                return true;
            }
            return false;

        }
        public List<Entity.Models.Region> GetRegions()
        {
            return _context.Regions.ToList();
        }

        public void AddDetails(PatientReq patientReq)
        {
            
            var existUser =  _context.AspnetUsers.FirstOrDefault(u => u.Email == patientReq.Email);
            var UserExist =  _context.Users.FirstOrDefault(m => m.Email == patientReq.Email);
            if (existUser == null)
            {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(patientReq.Password);

                var Aspnetuser = new AspnetUser();
                var User = new User();
                var Admin = new Admin();
                var Request = new Entity.Models.Request();
                var Requestclient = new Requestclient();
                var RequestType = new RequestType();

                Aspnetuser.Id = Guid.NewGuid().ToString();
                Aspnetuser.Username = patientReq.FirstName;
                Aspnetuser.Email = patientReq.Email;
                Aspnetuser.Passwordhash = hashedPassword;
                Aspnetuser.Phonenumber = patientReq.Mobile;
                Aspnetuser.Createddate = DateTime.Now;
                _context.AspnetUsers.Add(Aspnetuser);
                 _context.SaveChanges();

                //Admin.AspNetUserId = Aspnetuser.Id;
                //Admin.FirstName = patientReq.FirstName;
                //Admin.LastName = patientReq.LastName;
                //Admin.Email = patientReq.Email;
                //Admin.Mobile = patientReq.Mobile;
                //Admin.Address1 = patientReq.Street;
                //Admin.City = patientReq.City;
                //Admin.Zip = patientReq.ZipCode;
                //Admin.CreatedBy = patientReq.FirstName;
                //Admin.CreatedDate = DateTime.Now;
                //_context.Admins.Add(Admin);
                //_context.SaveChanges();
                User.Aspnetuserid = Aspnetuser.Id;
                User.Firstname = patientReq.FirstName;
                User.Lastname = patientReq.LastName;
                User.Email = patientReq.Email;
                User.Mobile = patientReq.Mobile;
                User.Street = patientReq.Street;
                User.City = patientReq.City;
                User.State = patientReq.State;
                User.Zipcode = patientReq.ZipCode;
                User.Createdby = "Patient";
                User.Intyear = patientReq.DOB.Year;
                User.Strmonth = patientReq.DOB.Month.ToString();
                User.Intdate = patientReq.DOB.Day;
                User.Createddate = DateTime.Now;
                _context.Users.Add(User);
                _context.SaveChanges();

                //RequestType.Name = "Patient";
                //_context.RequestTypes.Add(RequestType);
                //_context.SaveChanges();

                Request.RequestTypeId = 2;
                Request.UserId = User.Userid;
                Request.FirstName = patientReq.FirstName;
                Request.LastName = patientReq.LastName;
                Request.Email = patientReq.Email;
                Request.PhoneNumber = patientReq.Mobile;
                Request.Status = 1;
                Request.CreatedDate = DateTime.Now;
                _context.Requests.Add(Request);
                _context.SaveChanges();


                Requestclient.RequestId = Request.RequestId;
                Requestclient.FirstName = patientReq.FirstName;
                Requestclient.LastName = patientReq.LastName;
                Requestclient.Address = patientReq.Street;
                Requestclient.City = patientReq.City;
                Requestclient.Street = patientReq.Street;
                Requestclient.State = patientReq.State;
                Requestclient.ZipCode = patientReq.ZipCode;
                Requestclient.Email = patientReq.Email;
                Requestclient.IntYear = patientReq.DOB.Year;
                Requestclient.StrMonth = patientReq.DOB.Month.ToString();
                Requestclient.IntDate = patientReq.DOB.Day;
                Requestclient.PhoneNumber = patientReq.Mobile;
                Requestclient.RegionId = _context.Regions.FirstOrDefault(r => r.Name == patientReq.City).RegionId;
                Requestclient.Notes = patientReq.Symptoms;
                _context.Requestclients.Add(Requestclient);
                _context.SaveChanges();

                if (patientReq.File != null && patientReq.File.Length > 0)
                {
                    var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Documents");

                    if (!Directory.Exists(uploadsDirectory))
                    {
                        Directory.CreateDirectory(uploadsDirectory);
                    }

                    var fileName = Path.GetFileName(patientReq.File.FileName);
                    var filePath = Path.Combine(uploadsDirectory, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                         patientReq.File.CopyTo(stream);
                    }

                    var requestWiseFile = new Entity.Models.RequestWiseFile
                    {
                        RequestId = Request.RequestId,
                        FileName = fileName,
                        CreatedDate = DateTime.Now,
                    };
                    //PatientReq.FileName = filePath;
                    _context.RequestWiseFiles.Add(requestWiseFile);
                     _context.SaveChanges();
                }
            }
            
            else
            {
                var Request = new Entity.Models.Request();
                var Requestclient = new Requestclient();
                var RequestType = new RequestType();

                RequestType.Name = "Patient";
                _context.RequestTypes.Add(RequestType);
                 _context.SaveChanges();

                Request.RequestTypeId = 2;
                Request.UserId = UserExist.Userid;
                Request.FirstName = patientReq.FirstName;
                Request.LastName = patientReq.LastName;
                Request.Email = patientReq.Email;
                Request.PhoneNumber = patientReq.Mobile;
                Request.Status = 1;
                Request.CreatedDate = DateTime.Now;
                _context.Requests.Add(Request);
                 _context.SaveChanges();

                Requestclient.RequestId = Request.RequestId;
                Requestclient.FirstName = patientReq.FirstName;
                Requestclient.LastName = patientReq.LastName;
                Requestclient.Address = patientReq.Street;
                Requestclient.City = patientReq.City;
                Requestclient.Street = patientReq.Street;
                Requestclient.State = patientReq.State;
                Requestclient.ZipCode = patientReq.ZipCode;
                Requestclient.Email = patientReq.Email;
                Requestclient.IntYear = patientReq.DOB.Year;
                Requestclient.StrMonth = patientReq.DOB.Month.ToString();
                Requestclient.IntDate = patientReq.DOB.Day;
                Requestclient.PhoneNumber = patientReq.Mobile;
                Requestclient.RegionId = _context.Regions.FirstOrDefault(r => r.Name == patientReq.City).RegionId;
                Requestclient.Notes = patientReq.Symptoms;
                _context.Requestclients.Add(Requestclient);
                 _context.SaveChanges();

                if (patientReq.File != null && patientReq.File.Length > 0)
                {
                    var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Documents");

                    if (!Directory.Exists(uploadsDirectory))
                    {
                        Directory.CreateDirectory(uploadsDirectory);
                    }

                    var fileName = Path.GetFileName(patientReq.File.FileName);
                    var filePath = Path.Combine(uploadsDirectory, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                         patientReq.File.CopyToAsync(stream)
    ;
                    }

                    var requestWiseFile = new Entity.Models.RequestWiseFile
                    {
                        RequestId = Request.RequestId,
                        FileName = fileName,
                        CreatedDate = DateTime.Now,
                    };
                    //PatientReq.FileName = filePath;
                    _context.RequestWiseFiles.Add(requestWiseFile);
                     _context.SaveChanges();
                }
                
              
            }
        }
        public void FamilyDetails(FamilyReq familyReq)
        {
            var Request = new Entity.Models.Request();
            var Requestclient = new Requestclient();
            //var RequestType = new RequestType();

          
            //RequestType.Name = "Family";
            //_context.RequestTypes.Add(RequestType);
            // _context.SaveChanges();

            Request.RequestTypeId = 3;
            Request.FirstName = familyReq.f_firstname;
            Request.LastName = familyReq.f_lastname;
            Request.Email = familyReq.f_email;
            Request.PhoneNumber = familyReq.f_mobile;
            Request.Status = 1;
            Request.RelationName = familyReq.f_relation;
            Request.CreatedDate = DateTime.Now;
            _context.Requests.Add(Request);
            _context.SaveChanges();

            Requestclient.RequestId = Request.RequestId;
            Requestclient.FirstName = familyReq.FirstName;
            Requestclient.LastName = familyReq.LastName;
            Requestclient.Address = familyReq.Street;
            Requestclient.City = familyReq.City;
            Requestclient.Street = familyReq.Street;
            Requestclient.State = familyReq.State;
            Requestclient.ZipCode = familyReq.ZipCode;
            Requestclient.Email = familyReq.Email;
            Requestclient.IntYear = familyReq.DOB.Year;
            Requestclient.StrMonth = familyReq.DOB.Month.ToString();
            Requestclient.IntDate = familyReq.DOB.Day;
            Requestclient.PhoneNumber = familyReq.Mobile;
            Requestclient.Notes = familyReq.Symptoms;
            Requestclient.RegionId = _context.Regions.FirstOrDefault(r => r.Name == familyReq.City).RegionId;
            _context.Requestclients.Add(Requestclient);
            _context.SaveChanges();

        }
        public void ConciergeDetails(ConciergeReq conciergeReq)
        {
            var Request = new Entity.Models.Request();
            var Requestclient = new Requestclient();
            var RequestType = new RequestType();
            var Concierge = new Concierge();
            var RequestConcierge = new RequestConcierge();

            //RequestType.Name = "Concierge";
            //_context.RequestTypes.Add(RequestType);
            //_context.SaveChanges();

            Request.RequestTypeId = 4;
            Request.FirstName = conciergeReq.c_firstname;
            Request.LastName = conciergeReq.c_lastname;
            Request.Email = conciergeReq.c_email;
            Request.PhoneNumber = conciergeReq.c_mobile;
            Request.Status = 1;
            Request.CreatedDate = DateTime.Now;
            _context.Requests.Add(Request);
            _context.SaveChanges();

            Concierge.ConciergeName = conciergeReq.c_firstname;
            Concierge.Address = conciergeReq.c_address;
            Concierge.Street = conciergeReq.c_street;
            Concierge.City = conciergeReq.c_city;
            Concierge.State = conciergeReq.c_state;
            Concierge.ZipCode = conciergeReq.c_zip;
            Concierge.CreatedDate = DateTime.Now;
            _context.Concierges.Add(Concierge);
            _context.SaveChanges();

            RequestConcierge.RequestId = Request.RequestId;
            RequestConcierge.ConciergeId = Concierge.ConciergeId;
            _context.RequestConcierges.Add(RequestConcierge);
            _context.SaveChanges();

            Requestclient.RequestId = Request.RequestId;
            Requestclient.FirstName = conciergeReq.FirstName;
            Requestclient.LastName = conciergeReq.LastName;
            Requestclient.Address = conciergeReq.Street;
            Requestclient.City = conciergeReq.c_city;
            Requestclient.Street = conciergeReq.Street;
            Requestclient.State = conciergeReq.c_state;
            Requestclient.ZipCode = conciergeReq.c_zip;
            Requestclient.Email = conciergeReq.Email;
            Requestclient.IntYear = conciergeReq.DOB.Year;
            Requestclient.StrMonth = conciergeReq.DOB.Month.ToString();
            Requestclient.IntDate = conciergeReq.DOB.Day;
            Requestclient.PhoneNumber = conciergeReq.Mobile;
            Requestclient.RegionId = _context.Regions.FirstOrDefault(r => r.Name == conciergeReq.c_city).RegionId;
            Requestclient.Notes = conciergeReq.Symptoms;
            _context.Requestclients.Add(Requestclient);
            _context.SaveChanges();

        }
        public void BusinessDetails(BusinessReq businessReq)
        {
            var Request = new Entity.Models.Request();
            var Requestclient = new Requestclient();
            var RequestType = new RequestType();

            //RequestType.Name = "Business";
            //_context.RequestTypes.Add(RequestType);
            //_context.SaveChanges();

            Request.RequestTypeId = 1;
            Request.FirstName = businessReq.b_firstname;
            Request.LastName = businessReq.b_lastname;
            Request.Email = businessReq.b_email;
            Request.PhoneNumber = businessReq.b_mobile;
            Request.Status = 1;
            Request.CreatedDate = DateTime.Now;
            _context.Requests.Add(Request);
            _context.SaveChanges();

            Requestclient.RequestId = Request.RequestId;
            Requestclient.FirstName = businessReq.FirstName;
            Requestclient.LastName = businessReq.LastName;
            Requestclient.Address = businessReq.Street;
            Requestclient.Email = businessReq.Email;
            Requestclient.City = businessReq.City;
            Requestclient.Street = businessReq.Street;
            Requestclient.State = businessReq.State;
            Requestclient.ZipCode = businessReq.ZipCode;
            Requestclient.IntYear = businessReq.DOB.Year;
            Requestclient.StrMonth = businessReq.DOB.Month.ToString();
            Requestclient.IntDate = businessReq.DOB.Day;
            Requestclient.PhoneNumber = businessReq.Mobile;
            Requestclient.RegionId = _context.Regions.FirstOrDefault(r => r.Name == businessReq.City).RegionId;
            Requestclient.Notes = businessReq.Symptoms;
            _context.Requestclients.Add(Requestclient);
            _context.SaveChanges();

        }
    }
}
