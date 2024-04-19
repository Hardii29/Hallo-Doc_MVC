using Hallo_Doc.Entity.Data;
using Hallo_Doc.Entity.Models;
using Hallo_Doc.Entity.ViewModel;
using Hallo_Doc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hallo_Doc.Repository.Repository.Implementation
{
    public class PhysicianRepo : IPhysician
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmail_SMS _services;
        public PhysicianRepo(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, IEmail_SMS services)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _services = services;
        }
        public StatusCount CountRequest(int PhysicianId)
        {
            return new StatusCount
            {
                NewRequest = _context.Requests.Where(r => r.Status == 1 && r.PhysicianId == PhysicianId).Count(),
                PendingRequest = _context.Requests.Where(r => r.Status == 2 && r.PhysicianId == PhysicianId).Count(),
                ActiveRequest = _context.Requests.Where(r => (r.Status == 4 || r.Status == 5) && r.PhysicianId == PhysicianId).Count(),
                ConcludeRequest = _context.Requests.Where(r => (r.Status == 6 || r.Status == 9) && r.PhysicianId == PhysicianId).Count(),
          
            };
        }
        public PaginationModel<PhysicianDash> GetRequestData(int statusid ,int PhysicianId, int page, int pagesize)
        {
            List<int> id = new List<int>();
            if (statusid == 1) { id.Add(1); }
            if (statusid == 2) { id.Add(2); }
            if (statusid == 3) id.AddRange(new int[] { 4, 5 });
            if (statusid == 4) { id.Add(6); }
            var list = (from req in _context.Requests
                        join reqClient in _context.Requestclients
                        on req.RequestId equals reqClient.RequestId into reqClientGroup
                        from rc in reqClientGroup.DefaultIfEmpty()
                        where id.Contains(req.Status) && (req.PhysicianId == PhysicianId) 
                        //&& (searchValue == null ||
                        //       rc.FirstName.Contains(searchValue) || rc.LastName.Contains(searchValue) ||
                        //       req.FirstName.Contains(searchValue) || req.LastName.Contains(searchValue))
                        //       && (requesttype == -1 || req.RequestTypeId == requesttype)
                        orderby req.CreatedDate descending
                        select new PhysicianDash
                        {
                            RequestId = req.RequestId,
                            RequestTypeId = req.RequestTypeId,
                            Requestor = req != null ? req.FirstName + " " + req.LastName : "",
                            PatientName = rc != null ? rc.FirstName + " " + rc.LastName : "",
                            DOB = rc != null && rc.IntYear != null && rc.StrMonth != null && rc.IntDate != null ?
                                            new DateOnly((int)rc.IntYear, int.Parse(rc.StrMonth), (int)rc.IntDate) : DateOnly.MinValue,
                            Email = rc != null ? rc.Email : "",
                            Status = req.Status,
                            PatientMobile = rc != null ? rc.PhoneNumber : "",
                            Address = rc.Address + "," + rc.Street + "," + rc.City + "," + rc.State + "," + rc.ZipCode,
                            RequestorPhoneNumber = req != null ? req.PhoneNumber : "",
                            RequestClientId = rc != null ? rc.RequestclientId : null
                        }).ToList();
      
            int totalItemCount = list.Count();
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)pagesize);
            List<PhysicianDash> list1 = list.Skip((page - 1) * pagesize).Take(pagesize).ToList();

            PaginationModel<PhysicianDash> viewModel = new PaginationModel<PhysicianDash>()
            {
                PhysicianDash = list1,
                CurrentPage = page,
                TotalPages = totalPages,
            };

            return viewModel;
        }
        public bool AcceptCase(int RequestId, int PhysicianId)
        {
            var requestData = _context.Requests.FirstOrDefault(r => r.RequestId == RequestId);
            if (requestData != null)
            {
                requestData.Status = 2;
                _context.Requests.Update(requestData);
                _context.SaveChanges();
                RequestStatusLog rsl = new RequestStatusLog();
                rsl.RequestId = (int)RequestId;
                rsl.PhysicianId = PhysicianId;
                rsl.Notes = "Accpted by Your Physician";
                rsl.CreatedDate = DateTime.Now;
                rsl.Status = 2;
                _context.RequestStatusLogs.Add(rsl);
                _context.SaveChanges();
                return true;
            }
            else { return false; }
        }
        public bool TransferCase(int RequestId, string Notes)
        {
            BitArray bit1 = new BitArray(1);
            bit1.Set(0, true);
            var requestData = _context.Requests.FirstOrDefault(r => r.RequestId == RequestId && r.Status==2);
            if (requestData != null)
            {
                requestData.Status = 1;
                requestData.PhysicianId = null;
                _context.Requests.Update(requestData);
                _context.SaveChanges();
                var rsl = _context.RequestStatusLogs.FirstOrDefault(r => r.RequestId == RequestId && r.Status == 2);
                rsl.PhysicianId = null;
                rsl.Notes = Notes;
                rsl.CreatedDate = DateTime.Now;
                rsl.Status = 1;
                rsl.TransToAdmin = bit1;
                _context.RequestStatusLogs.Update(rsl);
                _context.SaveChanges();
                var rn = _context.RequestNotes.FirstOrDefault(r => r.RequestId == RequestId);
                rn.PhysicianNotes = Notes;
                rn.ModifiedDate = DateTime.Now;
                rn.ModifiedBy = "Physician";
                _context.RequestNotes.Update(rn);
                _context.SaveChanges();
                return true;
            }
            else { return false; }
        }
        public bool Housecall(int RequestId)
        {
            var request = _context.Requests.FirstOrDefault(req => req.RequestId == RequestId);
            request.Status = 5;
            request.ModifiedDate = DateTime.Now;
            _context.Requests.Update(request);
            _context.SaveChanges();

            RequestStatusLog rsl = new RequestStatusLog();
            rsl.RequestId = RequestId;
            rsl.CreatedDate = DateTime.Now;
            rsl.Status = 5;
            _context.RequestStatusLogs.Add(rsl);
            _context.SaveChanges();
            return true;
        }
        public bool Consult(int RequestId)
        {
            var request = _context.Requests.FirstOrDefault(req => req.RequestId == RequestId);
            request.Status = 6;
            request.ModifiedDate = DateTime.Now;
            _context.Requests.Update(request);
            _context.SaveChanges();

            RequestStatusLog rsl = new RequestStatusLog();
            rsl.RequestId = RequestId;
            rsl.CreatedDate = DateTime.Now;
            rsl.Status = 6;
            _context.RequestStatusLogs.Add(rsl);
            _context.SaveChanges();
            return true;
        }
        public bool ConcludeCare(int RequestId, string Notes)
        {
            var requestData = _context.Requests.FirstOrDefault(e => e.RequestId == RequestId);
            requestData.Status = 8;
            requestData.ModifiedDate = DateTime.Now;
            _context.Requests.Update(requestData);
            _context.SaveChanges();

            RequestStatusLog rsl = new RequestStatusLog
            {
                RequestId = RequestId,
                Notes = Notes,
                Status = 8,
                CreatedDate = DateTime.Now
            };
            _context.RequestStatusLogs.Add(rsl);
            _context.SaveChanges();
            return true;
        }
        public void CreateReq(PatientReq req, int PhysicianId)
        {
            var admin = _context.Physicians.Where(x => x.PhysicianId == PhysicianId).FirstOrDefault();

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
            Request.PhysicianId = PhysicianId;
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
            RequestNotes.PhysicianNotes = req.Symptoms;
            RequestNotes.CreatedDate = DateTime.Now;
            RequestNotes.CreatedBy = admin.FirstName;
            _context.RequestNotes.Add(RequestNotes);
            _context.SaveChanges();

        }
        public bool Finalizeform(int RequestId)
        {
            var E = _context.EncounterForms.FirstOrDefault(e => e.RequestId == RequestId);
            E.IsFinalize = true;
            _context.SaveChanges();
            return true;
        }
        public bool IsEncounterFinalized(int requestId)
        {
            return _context.EncounterForms.Any(e => e.RequestId == requestId && e.IsFinalize == true);
        }
        public List<Schedule> ShiftList(int PhysicianId)
        {
            List<Schedule> allData = (from s in _context.Shifts
                                      join shiftDetail in _context.ShiftDetails
                                      on s.ShiftId equals shiftDetail.ShiftId into shiftGroup
                                      from sd in shiftGroup.DefaultIfEmpty()
                                      where s.PhysicianId == PhysicianId
                                      select new Schedule
                                      {
                                          ShiftId = s.ShiftId,
                                          ShiftDate = s.StartDate,
                                          StartTime = sd.StartTime,
                                          EndTime = sd.EndTime,
                                          Start = new DateTime(s.StartDate.Year, s.StartDate.Month, s.StartDate.Day, sd.StartTime.Hour, sd.StartTime.Minute, sd.StartTime.Second),
                                          End = new DateTime(s.StartDate.Year, s.StartDate.Month, s.StartDate.Day, sd.EndTime.Hour, sd.EndTime.Minute, sd.EndTime.Second),
                                          Status = sd.Status,
                                          Color = sd.Status == 2 ? "pink" : "lightgreen",
                                          Border = sd.Status == 2 ? "deeppink" : "darkgreen",
                                      }).ToList();
            return allData;
        }
        public bool RequestAdmin(int PhysicianId, string Message)
        {
            var res = _context.Physicians.FirstOrDefault(e => e.PhysicianId == PhysicianId);
            string email = "hardi.jayani@etatvasoft.com";
            var subject = "Request To Admin";
            var body = $"Hello Admin ,\n\nThis is Doctor {res.FirstName} : \n{Message}";
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
                Action = 3,
                RoleId = 1,
            };
            _context.EmailLogs.Add(em);
            _context.SaveChanges();
            return true;
        }
        public bool ViewNotes(string? PhysicianNotes, int RequestId)
        {
            try
            {
                RequestNote notes = _context.RequestNotes.FirstOrDefault(E => E.RequestId == RequestId);
                if (notes != null)
                {

                    if (PhysicianNotes != null)
                    {
                        notes.PhysicianNotes = PhysicianNotes;
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
                        PhysicianNotes = PhysicianNotes,
                        CreatedDate = DateTime.Now,
                        CreatedBy = "Physician"

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
    }
}
