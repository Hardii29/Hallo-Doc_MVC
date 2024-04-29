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
using Microsoft.DotNet.Scaffolding.Shared;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Twilio.Http;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Hallo_Doc.Repository.Repository.Implementation
{
    public class AdminDashboardRepo : IAdminDashboard
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmail_SMS _services;
        public AdminDashboardRepo(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, IEmail_SMS services)
        {  
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _services = services;
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
            //switch (sortColumn)
            //{
            //    case "Name":
            //        list = sortOrder == "desc" ? list.OrderByDescending(x => x.PatientName).ToList() : list.OrderBy(x => x.PatientName).ToList();
            //        break;
                    
            //}
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
            var admin = _context.Admins.FirstOrDefault(a => a.AdminId == 1);
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
                            Region = rg != null ? rg.Name : "",
                            AdminId = admin.AdminId,
                            AdminName = $"{admin.FirstName} {admin.LastName}"
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
        public ViewNotes viewNotesData(int RequestId)
        {
            var admin = _context.Admins.FirstOrDefault(a => a.AdminId == 1);
            var req = _context.Requests.FirstOrDefault(W => W.RequestId == RequestId);
            //var symptoms = _context.RequestClients.FirstOrDefault(W => W.RequestId == RequestId);
            var transferlog = (from rs in _context.RequestStatusLogs
                               join py in _context.Physicians on rs.PhysicianId equals py.PhysicianId into pyGroup
                               from py in pyGroup.DefaultIfEmpty()
                               join p in _context.Physicians on rs.TransToPhysicianId equals p.PhysicianId into pGroup
                               from p in pGroup.DefaultIfEmpty()
                                   //join a in _context.Admins on rs.AdminId equals a.AdminId into aGroup
                                   //from a in aGroup.DefaultIfEmpty()
                               where rs.RequestId == RequestId && rs.Status == 2
                               select new TransferNotes
                               {
                                   TransPhysician = p.FirstName,
                                   //Admin = a.FirstName,
                                   Physician = py.FirstName,
                                   Requestid = rs.RequestId,
                                   Notes = rs.Notes,
                                   Status = rs.Status,
                                   Physicianid = rs.PhysicianId,
                                   Createddate = rs.CreatedDate,
                                   Requeststatuslogid = rs.RequestStatusLogId,
                                   Transtoadmin = rs.TransToAdmin,
                                   Transtophysicianid = rs.TransToPhysicianId
                               }).ToList();
            var cancelbyprovider = _context.RequestStatusLogs.Where(E => E.RequestId == RequestId && (E.TransToAdmin != null));
            var cancel = _context.RequestStatusLogs.Where(E => E.RequestId == RequestId && (E.Status == 7 || E.Status == 3));
            var model = _context.RequestNotes.FirstOrDefault(E => E.RequestId == RequestId);
            ViewNotes allData = new ViewNotes();
            allData.Requestid = RequestId;
            //allData.PatientNotes = symptoms.Notes;
            if (model == null)
            {
                allData.PhysicianNotes = "-";
                allData.AdminNotes = "-";
            }
            else
            {
                allData.Status = req.Status;
                allData.Requestnotesid = model.RequestNotesId;
                allData.PhysicianNotes = model.PhysicianNotes ?? "-";
                allData.AdminNotes = model.AdminNotes ?? "-";
            }

            List<TransferNotes> transfer = new List<TransferNotes>();
            foreach (var item in transferlog)
            {
                transfer.Add(new TransferNotes
                {
                    TransPhysician = item.TransPhysician,
                    // Admin = item.Admin,
                    Physician = item.Physician,
                    Requestid = item.Requestid,
                    Notes = item.Notes ?? "-",
                    Status = item.Status,
                    Physicianid = item.Physicianid,
                    Createddate = item.Createddate,
                    Requeststatuslogid = item.Requeststatuslogid,
                    Transtoadmin = item.Transtoadmin,
                    Transtophysicianid = item.Transtophysicianid
                });
            }
            allData.transfernotes = transfer;
            List<TransferNotes> cancelbyphysician = new List<TransferNotes>();
            foreach (var item in cancelbyprovider)
            {
                cancelbyphysician.Add(new TransferNotes
                {
                    Requestid = item.RequestId,
                    Notes = item.Notes ?? "-",
                    Status = item.Status,
                    Physicianid = item.PhysicianId,
                    Createddate = item.CreatedDate,
                    Requeststatuslogid = item.RequestStatusLogId,
                    Transtoadmin = item.TransToAdmin,
                    Transtophysicianid = item.TransToPhysicianId
                });
            }
            allData.cancelbyphysician = cancelbyphysician;

            List<TransferNotes> cancelrq = new List<TransferNotes>();
            foreach (var item in cancel)
            {
                cancelrq.Add(new TransferNotes
                {
                    Requestid = item.RequestId,
                    Notes = item.Notes ?? "-",
                    Status = item.Status,
                    Physicianid = item.PhysicianId,
                    Createddate = item.CreatedDate,
                    Requeststatuslogid = item.RequestStatusLogId,
                    Transtoadmin = item.TransToAdmin,
                    Transtophysicianid = item.TransToPhysicianId
                });
            }
            allData.cancel = cancelrq;
            allData.AdminId = admin.AdminId;
            allData.AdminName = $"{admin.FirstName} {admin.LastName}";
            return allData;
        }
        public bool ViewNotes(string? adminnotes, int RequestId)
        {
            try
            {
                RequestNote notes = _context.RequestNotes.FirstOrDefault(E => E.RequestId == RequestId);
                if (notes != null)
                {
                 
                     if (adminnotes != null)
                    {
                            notes.AdminNotes = adminnotes;
                            notes.ModifiedDate = DateTime.Now;
                            _context.RequestNotes.Update(notes);
                            _context.SaveChangesAsync();
                            return true;
                      
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    RequestNote rn = new RequestNote
                    {
                        RequestId = RequestId,
                        AdminNotes = adminnotes,
                        CreatedDate = DateTime.Now,
                        CreatedBy = "Admin"

                    };
                    _context.RequestNotes.Add(rn);
                    _context.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
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
            BitArray bitArray = new BitArray(1);
            bitArray.Set(0, false);
            if (requestData != null)
            {
                requestData.Status = 11;
                _context.Requests.Update(requestData);
                _context.SaveChanges();
                RequestStatusLog rsl = new RequestStatusLog();
                rsl.RequestId = (int)RequestId;
                rsl.Notes = Notes;
                rsl.CreatedDate = DateTime.Now;
                rsl.Status = 11;
                _context.RequestStatusLogs.Add(rsl);
                _context.SaveChanges();
                BlockRequest block = new BlockRequest
                {
                    RequestId = requestData.RequestId,
                    PhoneNumber = requestData.PhoneNumber,
                    Email = requestData.Email,
                    Reason = Notes,
                    IsActive = bitArray,
                    CreatedDate = DateTime.Now,
                };
                _context.BlockRequests.Add(block);
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
                
                req.PhysicianId = PhysicianId;
                _context.Requests.Update(req);
                _context.SaveChanges();
                RequestStatusLog rsl = new RequestStatusLog
                {
                    RequestId = RequestId,
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
        public ViewDocument GetFiles(int requestId)
        {
            var admin = _context.Admins.FirstOrDefault(a => a.AdminId == 1);
            var files = from f in _context.RequestWiseFiles
                        where f.RequestId == requestId
                        select new ViewDocument
                                {
                                    RequestWiseFileID = f.RequestWiseFileId,
                                    CreatedDate = f.CreatedDate,
                                    FileName = f.FileName
                                };
            var model = new ViewDocument()
            {
                RequestId = requestId,
                documents = files.ToList(),
                AdminId = admin.AdminId,
                AdminName = $"{admin.FirstName} {admin.LastName}"
            };
            return model;
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
                var timestamp = DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss");
                var fileName = $"{Path.GetFileNameWithoutExtension(viewDocument.File.FileName)}_{timestamp}{Path.GetExtension(viewDocument.File.FileName)}";
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
        public bool SendAgreementEmail(string email, int RequestId)
        {
            var baseUrl = "http://localhost:5203";
            var Action = "Agreement";
            var controller = "Home";
            
            string agreementPageLink = $"{baseUrl}/{controller}/{Action}?requestId={RequestId}";
            var subject = "Agreement for your request";
            var body = $"Dear Patient,\n\nPlease review and agree the agreement using the following link: \n{agreementPageLink}";

            _services.SendEmail(body, subject, email);
            BitArray bitArray = new BitArray(1);
            bitArray.Set(0, true);
            EmailLog em = new EmailLog
            {
                RequestId = RequestId,
                EmailTemplate = body,
                SubjectName = subject,
                EmailId = email,
                ConfirmationNumber = _context.Requests.Where(req => req.RequestId == RequestId).Select(req => req.ConfirmationNumber).FirstOrDefault(),
                CreateDate = DateTime.Now,
                SentDate = DateTime.Now,
                IsEmailSent = bitArray,
                AdminId = 1,
                SentTries = 1,
                Action = 4, 
                RoleId = 1, 
            };
            _context.EmailLogs.Add(em);
            _context.SaveChanges();
            return true;
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
                request.PhysicianId = null;
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
            var admin = _context.Admins.FirstOrDefault(a => a.AdminId == 1);
            var order = new Order();
            order.RequestId = requestId;
            order.AdminId = admin.AdminId;
            order.AdminName = $"{admin.FirstName} {admin.LastName}";
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
            var admin = _context.Admins.FirstOrDefault(a => a.AdminId == 1);
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
                            FileName = rf != null ? rf.FileName : "",
                            AdminId = admin.AdminId,
                            AdminName = $"{admin.FirstName} {admin.LastName}"
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
        public bool SendLink(string email, string firstName, string lastName)
        {
            var fullName = $"{firstName} {lastName}";
            var baseUrl = "http://localhost:5203";
            var Action = "Create_request";
            var controller = "PatientReq";
            
            string SubmitPageLink = $"{baseUrl}/{controller}/{Action}";
            var subject = "Create Patient Request";
            var body = $"Dear {fullName},\n\nPlease Submit request by filling Create Request form: \n{SubmitPageLink}";
            _services.SendEmail(body, subject, email);
            BitArray bitArray = new BitArray(1);
            bitArray.Set(0, true);
            EmailLog em = new EmailLog
            {
                EmailTemplate = body,
                SubjectName = subject,
                EmailId = email,
                CreateDate = DateTime.Now,
                SentDate = DateTime.Now,
                IsEmailSent = bitArray,
                AdminId = 1,
                SentTries = 1,
                Action = 2,
                RoleId = 1,
            };
            _context.EmailLogs.Add(em);
            _context.SaveChanges();
            return true;
        }
        public Encounter? EncounterInfo(int RequestId)
        {
            var encounter = (from rc in _context.Requestclients
                             join en in _context.EncounterForms on rc.RequestId equals en.RequestId into renGroup
                             from subEn in renGroup.DefaultIfEmpty()
                             where rc.RequestId == RequestId
                             select new Encounter
                             {
                                 RequestId = rc.RequestId,
                                 FirstName = rc.FirstName,
                                 LastName = rc.LastName,
                                 Address = rc.Address,
                                 DOB = rc != null && rc.IntYear != null && rc.StrMonth != null && rc.IntDate != null ?
                                            new DateOnly((int)rc.IntYear, int.Parse(rc.StrMonth), (int)rc.IntDate) : DateOnly.MinValue,
                                 Mobile = rc.PhoneNumber,
                                 Email = rc != null ? rc.Email : "",
                                 Injury = subEn.HistoryOfInjury,
                                 History = subEn.MedicalHistory,
                                 Medications = subEn.Medicatons,
                                 Allergies = subEn.Allergies,
                                 Temp = subEn.Temp,
                                 HR = subEn.Hr,
                                 RR = subEn.Rr,
                                 Bp = subEn.BloodPressureSystolic,
                                 Bpd = subEn.BloodPressureDiastolic,
                                 O2 = subEn.O2,
                                 Pain = subEn.Pain,
                                 Heent = subEn.Heent,
                                 CV = subEn.Cv,
                                 Chest = subEn.Chest,
                                 ABD = subEn.Abd,
                                 Extr = subEn.Extremeties,
                                 Skin = subEn.Skin,
                                 Neuro = subEn.Neuro,
                                 Other = subEn.Other,
                                 Diagnosis = subEn.Diagnosis,
                                 Treatment = subEn.TreatmentPlan,
                                 MDispensed = subEn.MedicationDispensed,
                                 Procedures = subEn.Procedures,
                                 Followup = subEn.FollowUp
                             }).FirstOrDefault();
            return encounter;
        }
        public void EditEncounterinfo(Encounter ve)
        {
            var RC = _context.Requestclients.FirstOrDefault(rc => rc.RequestId == ve.RequestId);
            RC.FirstName = ve.FirstName;
            RC.LastName = ve.LastName;
            RC.Address = ve.Address;
            RC.StrMonth = ve.DOB.Month.ToString();
            RC.IntDate = ve.DOB.Day;
            RC.IntYear = ve.DOB.Year;
            RC.PhoneNumber = ve.Mobile;
            RC.Email = ve.Email;
            _context.Update(RC);

            var E = _context.EncounterForms.FirstOrDefault(e => e.RequestId == ve.RequestId);
            if (E == null)
            {
                E = new EncounterForm { RequestId = (int)ve.RequestId };
                _context.EncounterForms.Add(E);
            }
            E.MedicalHistory = ve.History;
            E.HistoryOfInjury = ve.Injury;
            E.Medicatons = ve.Medications;
            E.Allergies = ve.Allergies;
            E.Temp = ve.Temp;
            E.Hr = ve.HR;
            E.Rr = ve.RR;
            E.BloodPressureSystolic = ve.Bp;
            E.BloodPressureDiastolic = ve.Bpd;
            E.O2 = ve.O2;
            E.Pain = ve.Pain;
            E.Heent = ve.Heent;
            E.Cv = ve.CV;
            E.Chest = ve.Chest;
            E.Abd = ve.ABD;
            E.Extremeties = ve.Extr;
            E.Skin = ve.Skin;
            E.Neuro = ve.Neuro;
            E.Other = ve.Other;
            E.Diagnosis = ve.Diagnosis;
            E.TreatmentPlan = ve.Treatment;
            E.MedicationDispensed = ve.MDispensed;
            E.Procedures = ve.Procedures;
            E.FollowUp = ve.Followup;
            E.IsFinalize = false;
            _context.SaveChanges();
        }
    }
}
