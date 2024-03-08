using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hallo_Doc.Entity.Data;
using Hallo_Doc.Entity.Models;
using Hallo_Doc.Entity.ViewModel;
using Hallo_Doc.Repository.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace Hallo_Doc.Repository.Repository.Implementation
{
    public class AdminDashboardRepo : IAdminDashboard
    {
        private readonly ApplicationDbContext _context;
        public AdminDashboardRepo(ApplicationDbContext context)
        {  
            _context = context; 
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
        public bool CancleCaseInfo(int? requestId, string caseTag, string Notes)
        {
            try
            {
                var requestData = _context.Requests.FirstOrDefault(e => e.RequestId == requestId);
                if (requestData != null)
                {
                    requestData.CaseTag = caseTag;
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
    }
}
