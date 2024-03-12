using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hallo_Doc.Entity.Data;
using Hallo_Doc.Entity.Models;
using Hallo_Doc.Entity.ViewModel;
using Hallo_Doc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            return new CountStatusWiseRequest
            {
                NewRequest = _context.Requests.Where(r => r.Status == 1).Count(),
                PendingRequest = _context.Requests.Where(r => r.Status == 2).Count(),
                ActiveRequest = _context.Requests.Where(r => (r.Status == 4 || r.Status == 5)).Count(),
                ConcludeRequest = _context.Requests.Where(r => r.Status == 6).Count(),
                ToCloseRequest = _context.Requests.Where(r => (r.Status == 3 || r.Status == 7 || r.Status == 8)).Count(),
                UnpaidRequest = _context.Requests.Where(r => r.Status == 9).Count()

            };
        }
        public List<AdminDash> GetRequestData(int statusid)
        {
            List<int> id = new List<int>();
            if (statusid == 1) { id.Add(1); }
            if (statusid == 2) { id.Add(2); }
            if (statusid == 3)
                id.AddRange(new int[] { 4, 5 });

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
                        where id.Contains(req.Status)
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
                            ProviderName = p != null ? p.FirstName + " " + p.LastName: "",
                            PatientMobile = rc != null ? rc.PhoneNumber : "",
                            Address = rc.Address + "," + rc.Street + "," + rc.City + "," + rc.State + "," + rc.ZipCode,
                            Notes = rc != null ? rc.Notes: "",
                            // ProviderID = req.Physicianid,
                            RequestorPhoneNumber = req != null ? req.PhoneNumber : ""
                        }).ToList();

            return list;
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
                //client.RequestId = viewCase.RequestId;
                _context.SaveChanges();
            }
        }
        public List<CaseTag> GetReasons()
        {
            return _context.CaseTags.ToList();
        }
        public List<Region> GetRegions()
        {
            return _context.Regions.ToList();
        }
        public List<Physician> GetPhysician(int regionId)
        {
            var physicians = _context.Physicians.Where(p => p.RegionId == regionId).ToList();
            return physicians;
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
    }
}
