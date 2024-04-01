using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Hallo_Doc.Entity.Data;
using Hallo_Doc.Entity.Models;
using Hallo_Doc.Entity.ViewModel;
using Hallo_Doc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Hallo_Doc.Repository.Repository.Implementation
{
    public class AdminDashboardRepo : IAdminDashboard
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AdminDashboardRepo(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {  
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public CountStatusWiseRequest CountRequestData()
        {
            var admin = _context.Admins.FirstOrDefault(a => a.AdminId == 1);
            return new CountStatusWiseRequest
            {
                NewRequest = _context.Requests.Where(r => r.Status == 1).Count(),
                PendingRequest = _context.Requests.Where(r => r.Status == 2).Count(),
                ActiveRequest = _context.Requests.Where(r => (r.Status == 4 || r.Status == 5)).Count(),
                ConcludeRequest = _context.Requests.Where(r => r.Status == 6).Count(),
                ToCloseRequest = _context.Requests.Where(r => (r.Status == 3 || r.Status == 7 || r.Status == 8)).Count(),
                UnpaidRequest = _context.Requests.Where(r => r.Status == 9).Count(),
                AdminId = admin.AdminId,
                AdminName = $"{admin.FirstName} {admin.LastName}",

            };
        }
        public PaginatedViewModel<AdminDash> GetRequestData(int statusid, string searchValue, int page, int pagesize, int? Region, string sortColumn, string sortOrder, int? requesttype)
        {
            List<int> id = new List<int>();
            if (statusid == 1) { id.Add(1); }
            if (statusid == 2) { id.Add(2); }
            if (statusid == 3) id.AddRange(new int[] { 4, 5 });
            if (statusid == 4) { id.Add(6); }
            if (statusid == 5) id.AddRange(new int[] { 3, 7, 8 });
            if (statusid == 6) { id.Add(9); }
            var list = (from req in _context.Requests
                        join reqClient in _context.Requestclients
                        on req.RequestId equals reqClient.RequestId into reqClientGroup
                        from rc in reqClientGroup.DefaultIfEmpty()
                        join phys in _context.Physicians
                        on req.PhysicianId equals phys.PhysicianId into physGroup
                        from p in physGroup.DefaultIfEmpty()
                        join reg in _context.Regions
                        on rc.RegionId equals reg.RegionId into RegGroup
                        from rg in RegGroup.DefaultIfEmpty()
                        where id.Contains(req.Status) && (searchValue == null ||
                               rc.FirstName.Contains(searchValue) || rc.LastName.Contains(searchValue) ||
                               req.FirstName.Contains(searchValue) || req.LastName.Contains(searchValue) ||
                               rc.Email.Contains(searchValue) || rc.PhoneNumber.Contains(searchValue) ||
                               rc.Address.Contains(searchValue) || rc.Notes.Contains(searchValue) ||
                               p.FirstName.Contains(searchValue) || p.LastName.Contains(searchValue) ||
                               rg.Name.Contains(searchValue))
                               && (Region == -1 || rc.RegionId == Region)
                               && (requesttype == -1 || req.RequestTypeId == requesttype)
                        orderby req.CreatedDate descending
                        select new AdminDash
                        {
                            RequestId = req.RequestId,
                            RequestTypeId = req.RequestTypeId,
                            Requestor = req != null ? req.FirstName + " " + req.LastName : "",
                            PatientName = rc != null ? rc.FirstName + " " + rc.LastName : "",
                            DOB = rc != null && rc.IntYear != null && rc.StrMonth != null && rc.IntDate != null ?
                                            new DateOnly((int)rc.IntYear, int.Parse(rc.StrMonth), (int)rc.IntDate) : DateOnly.MinValue,
                            RequestedDate = req.CreatedDate,
                            Email = rc != null ? rc.Email : "",
                            Region = rg != null ? rg.Name : "",
                            ProviderName = p != null ? p.FirstName + " " + p.LastName : "",
                            PatientMobile = rc != null ? rc.PhoneNumber : "",
                            Address = rc.Address + "," + rc.Street + "," + rc.City + "," + rc.State + "," + rc.ZipCode,
                            Notes = rc != null ? rc.Notes : "",
                            ProviderId = req.PhysicianId,
                            RequestorPhoneNumber = req != null ? req.PhoneNumber : "",
                            RequestClientId = rc != null ? rc.RequestclientId : null
                        }).ToList();
            switch (sortColumn)
            {
                case "Name":
                    list = sortOrder == "desc" ? list.OrderByDescending(x => x.PatientName).ToList() : list.OrderBy(x => x.PatientName).ToList();
                    break;
                    
            }
            int totalItemCount = list.Count();
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)pagesize);
            List<AdminDash> list1 = list.Skip((page - 1) * pagesize).Take(pagesize).ToList();

            PaginatedViewModel<AdminDash> viewModel = new PaginatedViewModel<AdminDash>()
            {
                AdminDash = list1,
                CurrentPage = page,
                TotalPages = totalPages,
            };

            return viewModel;
        }
        public ViewCase GetView(int requestId)
        {
            var data = (from req in _context.Requests
                        join reqClient in _context.Requestclients
                        on req.RequestId equals reqClient.RequestId into reqClientGroup
                        from rc in reqClientGroup.DefaultIfEmpty()
                        join reg in _context.Regions
                        on rc.RegionId equals reg.RegionId into RegGroup
                        from rg in RegGroup.DefaultIfEmpty()
                        where req.RequestId == requestId
                        select new ViewCase
                        {
                            RequestId = req.RequestId,
                            FirstName = rc != null ? rc.FirstName : "",
                            LastName = rc != null ? rc.LastName : "",
                            DOB = rc != null && rc.IntYear != null && rc.StrMonth != null && rc.IntDate != null ?
                            new DateOnly((int)rc.IntYear, int.Parse(rc.StrMonth), (int)rc.IntDate) : DateOnly.MinValue,
                            Address = rc.Address + "," + rc.Street + "," + rc.City + "," + rc.State + "," + rc.ZipCode,
                            Mobile = rc != null ? rc.PhoneNumber : "",
                            Email = rc != null ? rc.Email : "",
                            Region = rg != null ? rg.Name : ""
                        }).FirstOrDefault();
            return data;
        }
        public void UpdateViewCase(int requestId, ViewCase viewCase)
        {
            viewCase.RequestId = requestId;
            var client = _context.Requestclients.FirstOrDefault(rc => rc.RequestId == requestId);
            if (client != null)
            {
                client.FirstName = viewCase.FirstName; 
                client.LastName = viewCase.LastName;
                client.Email = viewCase.Email;
                client.PhoneNumber = viewCase.Mobile;
                client.Address = viewCase.Address;
                //client.RequestId = viewCase.RequestId;
                _context.SaveChanges();
            }
        }
        public bool CancelViewCase(int RequestId)
        {
            var requestData = _context.Requests.FirstOrDefault(r => r.RequestId == RequestId);
            if (requestData != null)
            {
                requestData.Status = 3;
                _context.Requests.Update(requestData);
                _context.SaveChanges();
                RequestStatusLog rsl = new RequestStatusLog();
                rsl.RequestId = (int)RequestId;
                rsl.CreatedDate = DateTime.Now;
                rsl.Status = 3;
                _context.RequestStatusLogs.Add(rsl);
                _context.SaveChanges();
                return true;
            }
            else { return false; }
        }
        public List<CaseTag> GetReasons()
        {
            return _context.CaseTags.ToList();
        }
        public List<Entity.Models.Region> GetRegions()
        {
            return _context.Regions.ToList();
        }
        public List<Physician> GetPhysician(int regionId)
        {
            var physicians = _context.Physicians.Where(p => p.RegionId == regionId).ToList();
            return physicians;
        }
        public List<HealthProfessionalType> GetProfession() 
        {
            return _context.HealthProfessionalTypes.ToList();
        }
        public List<HealthProffessional> GetBusiness(int businessId)
        {
            var business = _context.HealthProffessionals.Where(b => b.Profession ==  businessId).ToList();
            return business;
        }
        public Order GetBusinessDetails(int VendorId)
        {
            var data = (from v in _context.HealthProffessionals where v.VendorId == VendorId select new Order
            {
                Email = v.Email,
                FaxNumber = v.FaxNumber,
                BusinessContact = v.BusinessContact
            })
            .FirstOrDefault();
            
            return data;
        }
        public bool CancleCaseInfo(int? requestId, int CaseTagId, string Notes)
        {
            try
            {
                var requestData = _context.Requests.FirstOrDefault(e => e.RequestId == requestId);
                var casetag = _context.CaseTags.FirstOrDefault(c =>  c.CaseTagId == CaseTagId);
                if (requestData != null)
                {
                    requestData.CaseTag = casetag?.Name;
                    requestData.Status = 3;
                    _context.Requests.Update(requestData);
                    _context.SaveChanges();
                    RequestStatusLog rsl = new RequestStatusLog();
                    rsl.RequestId = (int)requestId;
                    rsl.Notes = Notes;
                    rsl.CreatedDate = DateTime.Now;
                    rsl.Status = 3;
                    _context.RequestStatusLogs.Add(rsl);
                    _context.SaveChanges();
                    return true;
                }
                else { return false; }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool BlockCaseReq(int RequestId, string Notes)
        {
            var requestData = _context.Requests.FirstOrDefault(r => r.RequestId == RequestId);
            if (requestData != null)
            {
                requestData.Status = 3;
                _context.Requests.Update(requestData);
                _context.SaveChanges();
                RequestStatusLog rsl = new RequestStatusLog();
                rsl.RequestId = (int)RequestId;
                rsl.Notes = Notes;
                rsl.CreatedDate = DateTime.Now;
                rsl.Status = 3;
                _context.RequestStatusLogs.Add(rsl);
                _context.SaveChanges();
                return true;
            }
            else { return false; }
        }
        public bool AssignCaseReq(int RequestId, int PhysicianId, string Notes)
        {
            var req = _context.Requests.FirstOrDefault(r =>r.RequestId == RequestId);
            if (req != null)
            {
                req.Status = 2;
                req.PhysicianId = PhysicianId;
                _context.Requests.Update(req);
                _context.SaveChanges();
                RequestStatusLog rsl = new RequestStatusLog
                {
                    RequestId = RequestId,
                    Status = 2,
                    PhysicianId = req.PhysicianId,
                    //AdminId = 1,
                    Notes = Notes,
                    CreatedDate = DateTime.Now
                };
                _context.RequestStatusLogs.Add(rsl);
                _context.SaveChanges();
                RequestNote note = new RequestNote
                {
                    RequestId = RequestId,
                    AdminNotes = Notes,
                    CreatedDate = DateTime.Now,
                    CreatedBy = "Admin"
                };
                _context.RequestNotes.Add(note);
                _context.SaveChanges();
                return true;
                
            }
            else { return false; }
        }
        public List<ViewDocument> GetFiles(int requestId)
        {
            var files = _context.RequestWiseFiles
                                .Where(f => f.RequestId == requestId)
                                .Select(f => new ViewDocument
                                {
                                    RequestId = f.RequestId,
                                    RequestWiseFileID = f.RequestWiseFileId,
                                    CreatedDate = f.CreatedDate,
                                    FileName = f.FileName
                                })
                                .ToList();

            return files;
        }
        public void UploadFiles(int requestId, ViewDocument viewDocument)
        {
            var request = _context.Requests.FirstOrDefault(f => f.RequestId == requestId);
            if (request != null && viewDocument.File != null && viewDocument.File.Length > 0)
            {
                var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Documents");

                if (!Directory.Exists(uploadsDirectory))
                {
                    Directory.CreateDirectory(uploadsDirectory);
                }

                var fileName = Path.GetFileName(viewDocument.File.FileName);
                var filePath = Path.Combine(uploadsDirectory, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    viewDocument.File.CopyTo(stream);
                }

                var requestWiseFile = new Entity.Models.RequestWiseFile
                {
                    RequestId = requestId,
                    FileName = fileName,
                    CreatedDate = DateTime.Now,
                };
                _context.RequestWiseFiles.Add(requestWiseFile);
                _context.SaveChanges();
            }
        }
        public IActionResult? DownloadFile(int fileID)
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
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                stream.CopyTo(memory);
            }
            memory.Position = 0;
            return new FileStreamResult(memory, "application/octet-stream")
            {
                FileDownloadName = file.FileName
            };
        }
        public void DeleteFile(int fileID)
        {
            var file = _context.RequestWiseFiles.Find(fileID);
            if (file != null)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(),"Documents" , file.FileName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                _context.RequestWiseFiles.Remove(file);
                _context.SaveChanges();
            }
        }
        public void DeleteAllFiles(int RequestId)
        {
            var files = _context.RequestWiseFiles.Where(f => f.RequestId == RequestId).ToList();
            foreach (var file in files)
            {
                _context.RequestWiseFiles.Remove(file);
            }
            _context.SaveChanges();
        }
        public bool TransferCaseReq(int RequestId, int PhysicianId, string Notes)
        {
            var req = _context.Requests.FirstOrDefault(r => r.RequestId == RequestId);
            if (req != null)
            {
                //req.Status = 2;
                req.PhysicianId = PhysicianId;
                _context.Requests.Update(req);
                var rsl = _context.RequestStatusLogs.FirstOrDefault(r => r.RequestId == RequestId && r.Status == 2 && r.PhysicianId != PhysicianId);
                if (rsl != null)
                {
                    rsl.TransToPhysicianId = PhysicianId;
                    rsl.Notes = Notes;
                    rsl.CreatedDate = DateTime.Now;
                    _context.RequestStatusLogs.Update(rsl);
                }
                
                RequestNote note = new RequestNote
                {
                    RequestId = RequestId,
                    AdminNotes = Notes,
                    CreatedDate = DateTime.Now,
                    CreatedBy = "Admin"
                };
                _context.RequestNotes.Add(note);
                _context.SaveChanges();
                return true;
            }
            else 
            { 
                return false; 
            }
        }
        public bool ClearCaseReq(int RequestId)
        {
            var requestData = _context.Requests.FirstOrDefault(r => r.RequestId == RequestId);
            if (requestData != null)
            {
                requestData.Status = 10;
                _context.Requests.Update(requestData);
                _context.SaveChanges();
                RequestStatusLog rsl = new RequestStatusLog();
                rsl.RequestId = (int)RequestId;
                rsl.Notes = "Cleared by Admin";
                rsl.CreatedDate = DateTime.Now;
                rsl.Status = 10;
                _context.RequestStatusLogs.Add(rsl);
                _context.SaveChanges();
                return true;
            }
            else { return false; }
        }
        public void SendAgreementEmail(string email, int RequestId)
        {
            var baseUrl = "http://localhost:5203";
            var Action = "Agreement";
            var controller = "Admin";
            
            string agreementPageLink = $"{baseUrl}/{controller}/{Action}?requestId={RequestId}";

            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress("hardi.jayani@etatvasoft.com");
                mail.To.Add(email);
                mail.Subject = "Agreement for your request";
                mail.Body = $"Dear user,\n\nPlease review and agree the agreement using the following link: \n{agreementPageLink}";

                using (SmtpClient smtp = new SmtpClient("mail.etatvasoft.com", 587))
                {
                    smtp.Credentials = new System.Net.NetworkCredential("hardi.jayani@etatvasoft.com", "LHV0@}YOA?)M");
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }
        }
        public bool SendAgreement_accept(int RequestID)
        {
            var request = _context.Requests.Find(RequestID);
            if (request != null)
            {
                request.Status = 4;
                _context.Requests.Update(request);
                _context.SaveChanges();
                RequestStatusLog rsl = new RequestStatusLog();
                rsl.RequestId = RequestID;
                rsl.Status = 4;
                rsl.CreatedDate = DateTime.Now;
                _context.RequestStatusLogs.Add(rsl);
                _context.SaveChanges();
            }
            return true;
        }
        public bool SendAgreement_Reject(int RequestID, string Notes)
        {
            var request = _context.Requests.Find(RequestID);
            if (request != null)
            {
                request.Status = 7;
                _context.Requests.Update(request);
                _context.SaveChanges();
                RequestStatusLog rsl = new RequestStatusLog();
                rsl.RequestId = RequestID;
                rsl.Status = 7;
                rsl.Notes = Notes;
                rsl.CreatedDate = DateTime.Now;
                _context.RequestStatusLogs.Add(rsl);
                _context.SaveChanges();
            }
            return true;
        }
        public Order GetOrderView(int requestId)
        {
            var order = new Order();
            order.RequestId = requestId;
            return order;
        }
        public void SendOrder(Order order)
        {
            var details = new OrderDetail();
            details.RequestId = order.RequestId;
            details.VendorId = order.VendorId;
            details.BusinessContact = order.BusinessContact;
            details.FaxNumber = order.FaxNumber;
            details.Email = order.Email;
            details.CreatedDate = DateTime.Now;
            details.Prescription = order.Prescription;
            details.NoOfRefill = order.NoOfRefill;
            details.CreatedBy = "Admin";
            _context.OrderDetails.Add(details);
            _context.SaveChanges();
        }
        public ViewCase GetClearCaseView(int requestId)
        {
            var data = (from req in _context.Requests
                        join reqClient in _context.Requestclients
                        on req.RequestId equals reqClient.RequestId into reqClientGroup
                        from rc in reqClientGroup.DefaultIfEmpty()
                        join rwf in _context.RequestWiseFiles
                        on req.RequestId equals rwf.RequestId into RwfGroup
                        from rf in RwfGroup.DefaultIfEmpty()
                        where req.RequestId == requestId
                        select new ViewCase
                        {
                            RequestId = req != null ? req.RequestId : 0,
                            FirstName = rc != null ? rc.FirstName : "",
                            LastName = rc != null ? rc.LastName : "",
                            DOB = rc != null && rc.IntYear != null && rc.StrMonth != null && rc.IntDate != null ?
                            new DateOnly((int)rc.IntYear, int.Parse(rc.StrMonth), (int)rc.IntDate) : DateOnly.MinValue,
                            Mobile = rc != null ? rc.PhoneNumber : "",
                            Email = rc != null ? rc.Email : "",
                            RequestWiseFileID = rf != null ? rf.RequestWiseFileId : 0,
                            CreatedDate = rf != null ? rf.CreatedDate : DateTime.MinValue,
                            FileName = rf != null ? rf.FileName : ""
                        }).FirstOrDefault();
            return data;
        }
        public void UpdateCloseCase(int requestId, ViewCase viewCase)
        {
            viewCase.RequestId = requestId;
            var client = _context.Requestclients.FirstOrDefault(rc => rc.RequestId == requestId);
            if (client != null)
            {
                client.Email = viewCase.Email;
                client.PhoneNumber = viewCase.Mobile;
                _context.SaveChanges();
            }
        }
        public bool CloseCaseReq(int RequestId)
        {
            var requestData = _context.Requests.FirstOrDefault(r => r.RequestId == RequestId);
            if (requestData != null)
            {
                requestData.Status = 9;
                _context.Requests.Update(requestData);
                _context.SaveChanges();
                RequestStatusLog rsl = new RequestStatusLog();
                rsl.RequestId = (int)RequestId;
                rsl.Notes = "Admin has closed the request";
                rsl.CreatedDate = DateTime.Now;
                rsl.Status = 9;
                _context.RequestStatusLogs.Add(rsl);
                _context.SaveChanges();
                return true;
            }
            else { return false; }
        }
        public AdminProfile? Profile(int adminId)
        {
            var model = (from a in _context.Admins
                         where a.AdminId == adminId
                         select new AdminProfile
                         {
                             FirstName = a.FirstName,
                             LastName = a.LastName,
                             Mobile = a.Mobile,
                             Email = a.Email,
                             Address1 = a.Address1,
                             City = a.City,
                             ZipCode = a.Zip,
                             AdminName = $"{a.FirstName} {a.LastName}",
                             AdminId = a.AdminId,
                             UserName = $"{a.FirstName} {a.LastName}",
                             State = "Gujrat"
                         }).FirstOrDefault();

            return model;
        }
        public void EditProfile(int adminId, AdminProfile profile)
        {
            profile.AdminId = adminId;
            profile.AdminName = $"{profile.FirstName} {profile.LastName}";
            var admin = _context.Admins.FirstOrDefault(a => a.AdminId == adminId);
            if (admin != null) {
                if(profile.FirstName != null) {
                    admin.FirstName = profile.FirstName;
                }
                if (profile.LastName != null)
                {
                    admin.LastName = profile.LastName;
                }
                if (profile.Mobile != null)
                {
                    admin.Mobile = profile.Mobile;
                }
                if (profile.Email != null)
                {
                    admin.Email = profile.Email;
                }
                if (profile.Address1 != null)
                {
                    admin.Address1 = profile.Address1;
                }
                if (profile.City != null) { 
                admin.City = profile.City;
                }
                if (profile.ZipCode != null) { 
                admin.Zip = profile.ZipCode;
                }
                if (profile.Mobile !=null) { 
                admin.AltPhone = profile.Mobile;
                }
                _context.Admins.Update(admin);
                _context.SaveChanges();
        }
        }
        public PatientReq? Admin()
        {
            var admin = _context.Admins.FirstOrDefault(a => a.AdminId == 1);
            return new PatientReq
            {
                
                AdminId = admin.AdminId,
                AdminName = $"{admin.FirstName} {admin.LastName}",

            };
        }
        public void CreateReq(PatientReq req)
        {
            var admin = _context.Admins.Where(x => x.AdminId == 1).FirstOrDefault();

            var Request = new Entity.Models.Request();
            var Requestclient = new Requestclient();
            var RequestNotes = new RequestNote();
            Request.RequestTypeId = 1;
            Request.Status = 1;
            //Request.UserId = Int32.Parse(UserId);
            Request.FirstName = admin.FirstName;
            Request.LastName = admin.LastName;
            Request.Email = admin.Email;
            Request.PhoneNumber = admin.Mobile;
            Request.CreatedDate = DateTime.Now;
            Request.IsUrgentEmailSent = new BitArray(1);
            Request.ConfirmationNumber = req.City.Substring(0, 2) + DateTime.Now.ToString("yyyyMM") + req.LastName.Substring(0, 2) + req.FirstName.Substring(0, 2) + "002";
            _context.Requests.Add(Request);
            _context.SaveChanges();
            Requestclient.RequestId = Request.RequestId;
            Requestclient.FirstName = req.FirstName;
            Requestclient.LastName = req.LastName;
            Requestclient.Address = req.Street + "," + req.City + "," + req.State + "," + req.ZipCode;
            Requestclient.Email = req.Email;
            Requestclient.PhoneNumber = req.Mobile;
            Requestclient.Notes = req.Symptoms;
            Requestclient.IntDate = req.DOB.Day;
            Requestclient.IntYear = req.DOB.Year;
            Requestclient.StrMonth = (req.DOB.Month).ToString();
            _context.Requestclients.Add(Requestclient);
            _context.SaveChanges();
            RequestNotes.RequestId = Request.RequestId;
            RequestNotes.AdminNotes = req.Symptoms;
            RequestNotes.CreatedDate = DateTime.Now;
            RequestNotes.CreatedBy = "Admin";
            _context.RequestNotes.Add(RequestNotes);
            _context.SaveChanges();
           
        }
        public List<AdminDash> Export(string status, int? Region, int? requesttype)
        {
            List<int> statusdata = status.Split(',').Select(int.Parse).ToList();
            List<AdminDash> allData = (from req in _context.Requests
                                       join reqClient in _context.Requestclients
                                       on req.RequestId equals reqClient.RequestId into reqClientGroup
                                       from rc in reqClientGroup.DefaultIfEmpty()
                                       join phys in _context.Physicians
                                       on req.PhysicianId equals phys.PhysicianId into physGroup
                                       from p in physGroup.DefaultIfEmpty()
                                       join reg in _context.Regions
                                       on rc.RegionId equals reg.RegionId into RegGroup
                                       from rg in RegGroup.DefaultIfEmpty()
                                       where statusdata.Contains((int)req.Status)
                                       && (Region == -1 || rc.RegionId == Region)
                                       && (requesttype == -1 || req.RequestTypeId == requesttype)
                                       orderby req.CreatedDate descending
                                       select new AdminDash
                                       {
                                           RequestId = req.RequestId,
                                           RequestTypeId = req.RequestTypeId,
                                           Requestor = req != null ? req.FirstName + " " + req.LastName : "",
                                           PatientName = rc != null ? rc.FirstName + " " + rc.LastName : "",
                                           DOB = rc != null && rc.IntYear != null && rc.StrMonth != null && rc.IntDate != null ?
                                            new DateOnly((int)rc.IntYear, int.Parse(rc.StrMonth), (int)rc.IntDate) : DateOnly.MinValue,
                                           RequestedDate = req.CreatedDate,
                                           Email = rc != null ? rc.Email : "",
                                           Region = rg != null ? rg.Name : "",
                                           ProviderName = p != null ? p.FirstName + " " + p.LastName : "",
                                           PatientMobile = rc != null ? rc.PhoneNumber : "",
                                           Address = rc.Address + "," + rc.Street + "," + rc.City + "," + rc.State + "," + rc.ZipCode,
                                           Notes = rc != null ? rc.Notes : "",
                                           ProviderId = req.PhysicianId,
                                           RequestorPhoneNumber = req != null ? req.PhoneNumber : "",
                                           RequestClientId = rc != null ? rc.RequestclientId : null
                                       }).ToList();
            return allData;
        }
        public void SendLink(string email, string firstName, string lastName)
        {
            var fullName = $"{firstName} {lastName}";
            var baseUrl = "http://localhost:5203";
            var Action = "Create_request";
            var controller = "PatientReq";
            
            string SubmitPageLink = $"{baseUrl}/{controller}/{Action}";

            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress("hardi.jayani@etatvasoft.com");
                mail.To.Add(email);
                mail.Subject = "Agreement for your request";
                mail.Body = $"Dear {fullName},\n\nPlease Submit request by filling Create Request form: \n{SubmitPageLink}";

                using (SmtpClient smtp = new SmtpClient("mail.etatvasoft.com", 587))
                {
                    smtp.Credentials = new System.Net.NetworkCredential("hardi.jayani@etatvasoft.com", "LHV0@}YOA?)M");
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }
        }
        public ProviderMenu? ProviderMenu(int? regionId = null)
        {
            BitArray bitArray = new BitArray(1);
            bitArray.Set(0, true);
            var admin = _context.Admins.FirstOrDefault(a => a.AdminId == 1);

            var query = from p in _context.Physicians
                        join PhysicianRole in _context.Roles
                        on p.RoleId equals PhysicianRole.RoleId into pRoleGroup
                        from pr in pRoleGroup.DefaultIfEmpty()
                        join PhysicianNoti in _context.PhysicianNotifications
                        on p.PhysicianId equals PhysicianNoti.PhysicianId into pNotifications
                        from pn in pNotifications.DefaultIfEmpty()
                        join region in _context.Regions
                        on p.RegionId equals region.RegionId into regionGroup
                        from prg in regionGroup.DefaultIfEmpty()
                        orderby p.PhysicianId ascending
                        select new ProviderMenu
                        {
                            ProviderId = p.PhysicianId,
                            ProviderName = p.FirstName + " " + p.LastName,
                            RoleId = pr.RoleId,
                            RoleName = pr.Name,
                            RegionId = prg.RegionId,
                            Status = p.Status,
                            IsNotificationStopped = pn != null ? (pn.IsNotificationStopped == bitArray) : false,
                            Mobile = p.Mobile,
                            Email = p.Email,
                        };

            if (regionId != null)
            {
                query = query.Where(p => regionId == -1 || p.RegionId == regionId);
            }
            var model = new ProviderMenu()
            {
                data = query.ToList(),
                AdminId = admin.AdminId,
                AdminName = $"{admin.FirstName} {admin.LastName}"
            };
            return model;
        }

        public void StopNotfy(int ProviderId)
        {
            var model = new PhysicianNotification();
            model.PhysicianId = ProviderId;
            BitArray bitArray = new BitArray(1);
            bitArray.Set(0, true);
            model.IsNotificationStopped = bitArray;
            _context.PhysicianNotifications.Add(model);
            _context.SaveChanges();
        }
        public void SendMailPhy(string email, string Message, string ProviderName)
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress("hardi.jayani@etatvasoft.com");
                mail.To.Add(email);
                mail.Subject = "Message from Admin";
                mail.Body = $"Dear Doctor {ProviderName},\n\nPlease find Message : \n{Message}";

                using (SmtpClient smtp = new SmtpClient("mail.etatvasoft.com", 587))
                {
                    smtp.Credentials = new System.Net.NetworkCredential("hardi.jayani@etatvasoft.com", "LHV0@}YOA?)M");
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }
        }
        public AccountAccess Access()
        {
            var admin = _context.Admins.FirstOrDefault(a => a.AdminId == 1);

            var query = from r in _context.Roles
                        select new AccountAccess
                        {
                            RoleId = r.RoleId,
                            RoleName = r.Name,
                            Accounttype = r.AccountType
                        };

            var model = new AccountAccess()
            {
                data = query.ToList(),
                AdminId = admin.AdminId,
                AdminName = $"{admin.FirstName} {admin.LastName}"
            };
            return model;
        }
        public UserAccess UserAccess()
        {
            var admin = _context.Admins.FirstOrDefault(a => a.AdminId == 1);

            var query = from r in _context.Roles
                        select new UserAccess
                        {
                            RoleId = r.RoleId,
                            RoleName = r.Name,
                            AccountType = r.AccountType,
                            UserName = r.AccountType == 1 ? admin.FirstName + " " + admin.LastName :
                                       r.AccountType == 2 ? $"{_context.Physicians.FirstOrDefault(p => p.RoleId == r.RoleId).FirstName} {_context.Physicians.FirstOrDefault(p => p.RoleId == r.RoleId).LastName}" : null,
                            Phone = r.AccountType == 1 ? admin.Mobile :
                                    r.AccountType == 2 ? _context.Physicians.FirstOrDefault(p => p.RoleId == r.RoleId).Mobile : null,
                            Status = (short)(r.AccountType == 1 ? admin.Status :
                                     r.AccountType == 2 ? _context.Physicians.FirstOrDefault(p => p.RoleId == r.RoleId).Status : null),
                            RequestCount = r.AccountType == 1 ? _context.Requests.Count() : 0,
                        };

            var model = new UserAccess()
            {
                data = query.ToList(),
                AdminId = admin.AdminId,
                AdminName = $"{admin.FirstName} {admin.LastName}"
            };
            return model;
        }
        public AccountAccess CreateAccess()
        {
            var admin = _context.Admins.FirstOrDefault(a => a.AdminId == 1);
            return new AccountAccess
            {

                AdminId = admin.AdminId,
                AdminName = $"{admin.FirstName} {admin.LastName}",

            };
        }
    }
}
