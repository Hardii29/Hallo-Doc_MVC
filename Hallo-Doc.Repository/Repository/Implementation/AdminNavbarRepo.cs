using Hallo_Doc.Entity.Data;
using Hallo_Doc.Entity.Models;
using Hallo_Doc.Entity.ViewModel;
using Hallo_Doc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Hallo_Doc.Repository.Repository.Implementation
{
    public class AdminNavbarRepo : IAdminNavbar
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AdminNavbarRepo(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public List<Entity.Models.Region> GetRegions()
        {
            return _context.Regions.ToList();
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
        public List<Menu> GetMenuList(AccountType accountType)
        {
            switch (accountType)
            {
                case AccountType.Admin:
                    return _context.Menus.Where(m => m.AccountType == 1).ToList();
                case AccountType.Physician:
                    return _context.Menus.Where(m => m.AccountType == 2).ToList();
                case AccountType.Patient:
                    return _context.Menus.Where(m => m.AccountType == 3).ToList();
                case AccountType.All:
                    return _context.Menus.ToList();
                default:
                    return new List<Menu>();
            }
        }
        public void CreateRole(AccountAccess access)
        {
            var role = _context.Roles.FirstOrDefault(r => r.Name == access.RoleName);
            foreach (var menuId in access.SelectMenu)
            {
                var rm = new RoleMenu
                {
                    RoleId = role.RoleId,
                    MenuId = menuId,
                };
                _context.RoleMenus.Add(rm);

            }
            _context.SaveChanges();
        }
        public Schedule Schedule()
        {
            var admin = _context.Admins.FirstOrDefault(a => a.AdminId == 1);
            return new Schedule
            {

                AdminId = admin.AdminId,
                AdminName = $"{admin.FirstName} {admin.LastName}",

            };
        }
        public void CreateShift(int RegionId, int PhysicianId, DateOnly ShiftDate, TimeOnly StartTime, TimeOnly EndTime)
        {
            BitArray bitArray = new BitArray(1);
            bitArray.Set(0, false);
            Shift s = new Shift();
            ShiftDetail sd = new ShiftDetail();
            ShiftDetailRegion sdr = new ShiftDetailRegion();

            s.PhysicianId = PhysicianId;
            s.StartDate = ShiftDate;
            s.IsRepeat = bitArray;
            s.CreatedBy = "Admin";
            s.CreatedDate = DateTime.Now;
            _context.Shifts.Add(s);
            _context.SaveChanges();

            sd.ShiftId = s.ShiftId;
            sd.ShiftDate = new DateTime(ShiftDate.Year, ShiftDate.Month, ShiftDate.Day, StartTime.Hour, StartTime.Minute, StartTime.Second);
            sd.RegionId = RegionId;
            sd.StartTime = StartTime;
            sd.EndTime = EndTime;
            sd.Status = 2;
            sd.IsDeleted = bitArray;
            _context.ShiftDetails.Add(sd);
            _context.SaveChanges();

            sdr.ShiftDetailId = sd.ShiftDetailId;
            sdr.RegionId = RegionId;
            sdr.IsDeleted = bitArray;
            _context.ShiftDetailRegions.Add(sdr);
            _context.SaveChanges();

        }
        public List<Physician> PhysicianCalender(int? regionId)
        {
            if (regionId == null)
            {
                var allPhysicians = _context.Physicians.ToList();
                return allPhysicians;
            }
            else
            {
                var physicians = _context.Physicians.Where(p => p.RegionId == regionId).ToList();
                return physicians;
            }
        }
        public List<Physician> AllPhysician()
        {
            var allPhysicians = _context.Physicians.ToList();
            return allPhysicians;
        }
        public List<Schedule> ShiftList()
        {
            List<Schedule> allData = (from s in _context.Shifts
                                      join shiftDetail in _context.ShiftDetails
                                      on s.ShiftId equals shiftDetail.ShiftId into shiftGroup
                                      from sd in shiftGroup.DefaultIfEmpty()
                                      join p in _context.Physicians
                                      on s.PhysicianId equals p.PhysicianId into phyShift
                                      from ps in phyShift.DefaultIfEmpty()
                                      select new Schedule
                                      {
                                          PhysicianId = s.PhysicianId,
                                          PhysicianName = ps.FirstName + " " + ps.LastName,
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
        public Schedule GetShiftDetails(int ShiftId)
        {
            var data = (from s in _context.Shifts
                        join shiftDetail in _context.ShiftDetails
                        on s.ShiftId equals shiftDetail.ShiftId into shiftGroup
                        from sd in shiftGroup.DefaultIfEmpty()
                        join p in _context.Physicians
                        on s.PhysicianId equals p.PhysicianId into phyShift
                        from ps in phyShift.DefaultIfEmpty()
                        join r in _context.Regions
                        on sd.RegionId equals r.RegionId into shiftRegion
                        from rs in shiftRegion.DefaultIfEmpty()
                        where s.ShiftId == ShiftId
                        select new Schedule
                        {
                            RegionId = sd.RegionId,
                            RegionName = rs.Name,
                            PhysicianId = s.PhysicianId,
                            PhysicianName = ps.FirstName + " " + ps.LastName,
                            ShiftId = s.ShiftId,
                            ShiftDate = s.StartDate,
                            StartTime = sd.StartTime,
                            EndTime = sd.EndTime,
                            Start = new DateTime(s.StartDate.Year, s.StartDate.Month, s.StartDate.Day, sd.StartTime.Hour, sd.StartTime.Minute, sd.StartTime.Second),
                            End = new DateTime(s.StartDate.Year, s.StartDate.Month, s.StartDate.Day, sd.EndTime.Hour, sd.EndTime.Minute, sd.EndTime.Second),
                        }).FirstOrDefault();
            return data;
        }
        public void EditShift(int ShiftId, int RegionId, int PhysicianId, DateOnly ShiftDate, TimeOnly StartTime, TimeOnly EndTime)
        {
            var s = _context.Shifts.FirstOrDefault(s => s.ShiftId == ShiftId);
            var sd = _context.ShiftDetails.FirstOrDefault(sd => sd.ShiftId == ShiftId);
            var sdr = _context.ShiftDetailRegions.FirstOrDefault(sdr => sdr.ShiftDetail.ShiftId == ShiftId);
            if (s != null)
            {
                s.PhysicianId = PhysicianId;
                s.StartDate = ShiftDate;
                _context.Shifts.Update(s);
                _context.SaveChanges();
            }

            if (sd != null)
            {
                sd.ShiftDate = new DateTime(ShiftDate.Year, ShiftDate.Month, ShiftDate.Day, StartTime.Hour, StartTime.Minute, StartTime.Second);
                sd.RegionId = RegionId;
                sd.StartTime = StartTime;
                sd.EndTime = EndTime;
                sd.ModifiedBy = "Admin";
                sd.ModifiedDate = DateTime.Now;
                _context.ShiftDetails.Update(sd);
                _context.SaveChanges();
            }

            if (sdr != null)
            {
                sdr.RegionId = RegionId;
                _context.ShiftDetailRegions.Update(sdr);
                _context.SaveChanges();
            }

        }
        public void DeleteShift(int ShiftId)
        {
            var sdr = _context.ShiftDetailRegions.Where(sdr => sdr.ShiftDetail.ShiftId == ShiftId);
            _context.ShiftDetailRegions.RemoveRange(sdr);
            _context.SaveChanges();

            var sd = _context.ShiftDetails.Where(sd => sd.ShiftId == ShiftId);
            _context.ShiftDetails.RemoveRange(sd);
            _context.SaveChanges();

            var shift = _context.Shifts.Where(s => s.ShiftId == ShiftId);
            _context.Shifts.RemoveRange(shift);
            _context.SaveChanges();
        }
        public MDsOnCall MDsOnCall()
        {
            var admin = _context.Admins.FirstOrDefault(a => a.AdminId == 1);

            var query = (from p in _context.Physicians
                         join s in _context.Shifts
                         on p.PhysicianId equals s.PhysicianId into physicianShifts
                         from ps in physicianShifts.DefaultIfEmpty()
                         join sd in _context.ShiftDetails
                         on ps.ShiftId equals sd.ShiftId into shiftDetails
                         from sd in shiftDetails.DefaultIfEmpty()
                         join r in _context.Regions
                         on sd.RegionId equals r.RegionId into shiftRegions
                         from sr in shiftRegions.DefaultIfEmpty()
                         select new MDsOnCall
                         {
                             RegionId = sd != null ? sd.RegionId : null,
                             RegionName = sr != null ? sr.Name : null,
                             PhysicianId = p.PhysicianId,
                             PhysicianName = p.FirstName + " " + p.LastName,
                             ShiftId = ps != null ? ps.ShiftId : 0,
                             ShiftDate = ps != null ? ps.StartDate : DateOnly.MinValue,
                             StartTime = sd != null ? sd.StartTime : TimeOnly.MinValue,
                             EndTime = sd != null ? sd.EndTime : TimeOnly.MinValue,
                             Start = ps != null && sd != null ? new DateTime(ps.StartDate.Year, ps.StartDate.Month, ps.StartDate.Day, sd.StartTime.Hour, sd.StartTime.Minute, sd.StartTime.Second) : DateTime.MinValue,
                             End = ps != null && sd != null ? new DateTime(ps.StartDate.Year, ps.StartDate.Month, ps.StartDate.Day, sd.EndTime.Hour, sd.EndTime.Minute, sd.EndTime.Second) : DateTime.MinValue,
                             Photo = p.Photo
                         }).GroupBy(md => md.PhysicianId).Select(g => g.First());


            var model = new MDsOnCall()
            {
                data = query.ToList(),
                AdminId = admin.AdminId,
                AdminName = $"{admin.FirstName} {admin.LastName}"
            };
            return model;
        }
        public RequestedShift RequestedShift()
        {
            var admin = _context.Admins.FirstOrDefault(a => a.AdminId == 1);

            var query = from s in _context.Shifts
                        join shiftDetail in _context.ShiftDetails
                        on s.ShiftId equals shiftDetail.ShiftId into shiftGroup
                        from sd in shiftGroup.DefaultIfEmpty()
                        join p in _context.Physicians
                        on s.PhysicianId equals p.PhysicianId into phyShift
                        from ps in phyShift.DefaultIfEmpty()
                        join r in _context.Regions
                        on sd.RegionId equals r.RegionId into shiftRegion
                        from rs in shiftRegion.DefaultIfEmpty()
                        orderby s.StartDate descending
                        select new RequestedShift
                        {
                            RegionId = sd.RegionId,
                            RegionName = rs.Name,
                            PhysicianId = s.PhysicianId,
                            PhysicianName = ps.FirstName + " " + ps.LastName,
                            ShiftId = s.ShiftId,
                            ShiftDate = s.StartDate.ToString("MMM dd, yyyy"),
                            ShiftTime = sd.StartTime.ToString("h:mm tt") + " - " + sd.EndTime.ToString("h:mm tt")
                        };

            var model = new RequestedShift()
            {
                data = query.ToList(),
                AdminId = admin.AdminId,
                AdminName = $"{admin.FirstName} {admin.LastName}"
            };
            return model;
        }
        public VendorMenu VendorMenu(string searchValue, int Profession)
        {
            var admin = _context.Admins.FirstOrDefault(a => a.AdminId == 1);

            var result = (from Hp in _context.HealthProffessionals
                          join Hpt in _context.HealthProfessionalTypes
                          on Hp.Profession equals Hpt.HealthProfessionalTypeId into AdminGroup
                          from asp in AdminGroup.DefaultIfEmpty()
                          where (searchValue == null || Hp.VendorName.Contains(searchValue))
                             && (Profession == 0 || Hp.Profession == Profession)
                             && (Hp.IsDeleted == new BitArray(1))
                          select new VendorMenu
                          {
                              VendorId = Hp.VendorId,
                              Profession = asp.ProfessionName,
                              Business = Hp.VendorName,
                              Email = Hp.Email,
                              FaxNumber = Hp.FaxNumber,
                              PhoneNumber = Hp.PhoneNumber,
                              BusinessNumber = Hp.BusinessContact
                          }).ToList();

            var model = new VendorMenu()
            {
                data = result,
                AdminId = admin.AdminId,
                AdminName = $"{admin.FirstName} {admin.LastName}"
            };
            return model;
        }
        
        public VendorMenu AddBusiness()
        {
            var admin = _context.Admins.FirstOrDefault(a => a.AdminId == 1);
            return new VendorMenu
            {

                AdminId = admin.AdminId,
                AdminName = $"{admin.FirstName} {admin.LastName}",

            };
        }
        public void AddVendor(VendorMenu model)
        {
            var data = new HealthProffessional();
            var lastPhy = _context.HealthProffessionals.OrderByDescending(p => p.VendorId).FirstOrDefault();
            var lastid = lastPhy != null ? lastPhy.VendorId : 1000;
            int newId = lastid + 1;
            data.VendorId = newId;
            data.Profession = model.ProfessionId;
            data.VendorName = model.Business;
            data.Email = model.Email;
            data.FaxNumber = model.FaxNumber;
            data.PhoneNumber = model.PhoneNumber;
            data.BusinessContact = model.BusinessNumber;
            data.Address = model.Street;
            data.City = model.City;
            data.Zip = model.Zip;
            data.State = model.State;
            data.CreatedDate = DateTime.Now;
            _context.HealthProffessionals.Add(data);
            _context.SaveChanges();
        }
        public VendorMenu EditVendor(int VendorId)
        {
            var admin = _context.Admins.FirstOrDefault(a => a.AdminId == 1);
            var model = (from p in _context.HealthProffessionals
                         where p.VendorId == VendorId
                         select new VendorMenu
                         {
                             VendorId = p.VendorId,
                             Business = p.VendorName,
                             ProfessionId = p.Profession,
                             Email = p.Email,
                             FaxNumber = p.FaxNumber,
                             PhoneNumber = p.PhoneNumber,
                             BusinessNumber = p.BusinessContact,
                             Street = p.Address,
                             City = p.City,
                             Zip = p.Zip,
                             State = p.State,
                             AdminId = admin.AdminId,
                             AdminName = $"{admin.FirstName} {admin.LastName}",

                         }).FirstOrDefault();

            return model;
        }
        public void EditVendorInfo(int VendorId, VendorMenu model)
        {
            var data = _context.HealthProffessionals.FirstOrDefault(v => v.VendorId == VendorId);
            if (data != null)
            {
                data.Profession = model.ProfessionId;
                data.VendorName = model.Business;
                data.Email = model.Email;
                data.FaxNumber = model.FaxNumber;
                data.PhoneNumber = model.PhoneNumber;
                data.BusinessContact = model.BusinessNumber;
                data.Address = model.Street;
                data.City = model.City;
                data.Zip = model.Zip;
                data.State = model.State;
                data.ModifiedDate = DateTime.Now;
                _context.HealthProffessionals.Update(data);
                _context.SaveChanges();
            }
        }
        public bool DeleteBusiness(int VendorId)
        {
            HealthProffessional r = _context.HealthProffessionals.Where(x => x.VendorId == VendorId).FirstOrDefault();
            r.IsDeleted[0] = true;
            r.ModifiedDate = DateTime.Now;
            _context.HealthProffessionals.Update(r);
            _context.SaveChanges();
            return true;
        }
        public BlockHistory BlockedHistory(BlockHistory history)
        {
            var admin = _context.Admins.FirstOrDefault(a => a.AdminId == 1);
            var data = (from req in _context.BlockRequests
                        join r in _context.Requests on req.RequestId equals r.RequestId
                        where (string.IsNullOrEmpty(history.PatientName) || r.FirstName.Contains(history.PatientName))
                           && (history.CreatedDate == null || req.CreatedDate.Value.Date == history.CreatedDate)
                           && (string.IsNullOrEmpty(history.Email) || req.Email.Contains(history.Email))
                           && (string.IsNullOrEmpty(history.Mobile) || req.PhoneNumber.Contains(history.Mobile))
                        select new PatientData
                        {
                            PatientName = r.FirstName + " " + r.LastName,
                            Email = req.Email,
                            CreatedDate = (DateTime)req.CreatedDate,
                            IsActive = req.IsActive[0],
                            RequestId = req.RequestId,
                            Mobile = req.PhoneNumber,
                            Notes = req.Reason
                        }).ToList();
            BlockHistory bh = new BlockHistory();
            bh.list = data;
            bh.AdminId = admin.AdminId;
            bh.AdminName = $"{admin.FirstName} {admin.LastName}";
            return bh;
           
        }
        public bool UnBlock(int reqId)
        {
            BlockRequest r = _context.BlockRequests.Where(x => x.RequestId == reqId).FirstOrDefault();
            r.IsActive[0] = true;
            r.ModifiedDate = DateTime.Now;
            _context.BlockRequests.Update(r);
            _context.SaveChanges();

            Entity.Models.Request req = _context.Requests.Where(x => x.RequestId == reqId).FirstOrDefault();
            req.Status = 1;
            req.ModifiedDate = DateTime.Now;
            _context.Requests.Update(req);
            _context.SaveChanges();

            return true;
        }
        public UserData PatientHistory(string fname, string lname, string email, string phone)
        {
            var admin = _context.Admins.FirstOrDefault(a => a.AdminId == 1);

            var query = from u in _context.Users
                        .Where(pp => (string.IsNullOrEmpty(fname) || pp.Firstname.Contains(fname))
                               && (string.IsNullOrEmpty(lname) || pp.Lastname.Contains(lname))
                               && (string.IsNullOrEmpty(email) || pp.Email.Contains(email))
                               && (string.IsNullOrEmpty(phone) || pp.Mobile.Contains(phone)))
                        select new UserData
                        {
                            Id = u.Userid,
                            Firstname = u.Firstname,
                            Lastname = u.Lastname,
                            Email = u.Email,
                            Mobile = u.Mobile,
                            Address = u.Street + ", " + u.City + ", " + u.State,
                        };

            var model = new UserData()
            {
                data = query.ToList(),
                AdminId = admin.AdminId,
                AdminName = $"{admin.FirstName} {admin.LastName}"
            };
            return model;
        }
        public PatientData PatientRecord(int UserId)
        {
            var admin = _context.Admins.FirstOrDefault(a => a.AdminId == 1);

            var result = (from req in _context.Requests
                          join reqClient in _context.Requestclients
                          on req.RequestId equals reqClient.RequestId into reqClientGroup
                          from rc in reqClientGroup.DefaultIfEmpty()
                          join phys in _context.Physicians
                          on req.PhysicianId equals phys.PhysicianId into physGroup
                          from p in physGroup.DefaultIfEmpty()
                          where req.UserId == UserId
                          select new PatientData
                          {
                              PatientName = rc.FirstName + " " + rc.LastName,
                              RequestedDate = req.CreatedDate,
                              Confirmation = req.ConfirmationNumber,
                              Physician = p.FirstName + " " + p.LastName,
                              ConcludedDate = req.CreatedDate,
                              Status = (status)req.Status,
                              RequestTypeId = req.RequestTypeId,
                              RequestId = req.RequestId
                          }).ToList();

            var model = new PatientData()
            {
                Record = result,
                AdminId = admin.AdminId,
                AdminName = $"{admin.FirstName} {admin.LastName}"
            };
            return model;
        }
        public SearchRecordList SearchRecord(SearchRecordList sl)
        {
            var admin = _context.Admins.FirstOrDefault(a => a.AdminId == 1);

            var result = (from req in _context.Requests
                          join reqClient in _context.Requestclients
                          on req.RequestId equals reqClient.RequestId into reqClientGroup
                          from rc in reqClientGroup.DefaultIfEmpty()
                          join phys in _context.Physicians
                          on req.PhysicianId equals phys.PhysicianId into physGroup
                          from p in physGroup.DefaultIfEmpty()
                          join nts in _context.RequestNotes
                          on req.RequestId equals nts.RequestId into ntsgrp
                          from nt in ntsgrp.DefaultIfEmpty()
                          where req.IsDeleted == new BitArray(1) && (sl.ReqStatus == 0 || req.Status == sl.ReqStatus) &&
                                                    (sl.RequestTypeID == 0 || req.RequestTypeId == sl.RequestTypeID) &&
                                                    (!sl.StartDOS.HasValue || req.CreatedDate.Value.Date >= sl.StartDOS.Value.Date) &&
                                                    (!sl.EndDOS.HasValue || req.CreatedDate.Value.Date <= sl.EndDOS.Value.Date) &&
                                                    (sl.PatientName.IsNullOrEmpty() || (req.FirstName + " " + req.LastName).ToLower().Contains(sl.PatientName.ToLower())) &&
                                                    (sl.PhysicianName.IsNullOrEmpty() || (p.FirstName + " " + p.LastName).ToLower().Contains(sl.PhysicianName.ToLower())) &&
                                                    (sl.Email.IsNullOrEmpty() || rc.Email.ToLower().Contains(sl.Email.ToLower())) &&
                                                    (sl.Mobile.IsNullOrEmpty() || rc.PhoneNumber.ToLower().Contains(sl.Mobile.ToLower()))
                          orderby req.CreatedDate descending
                          select new SearchRecords
                          {
                              PatientName = rc.FirstName + " " + rc.LastName,
                              RequestTypeID = req.RequestTypeId,
                              DateOfService = req.CreatedDate,
                              Email = rc.Email ?? "-",
                              Mobile = rc.PhoneNumber ?? "-",
                              Address = rc.Address + "," + rc.City,
                              Zip = rc.ZipCode,
                              Status = (status)req.Status,
                              Physician = p.FirstName + " " + p.LastName ?? "-",
                              PhyNotes = nt != null ? nt.PhysicianNotes ?? "-" : "-",
                              AdminNotes = nt != null ? nt.AdminNotes ?? "-" : "-",
                              PatientNotes = rc.Notes ?? "-",
                              RequestID = req.RequestId,
                              Modifieddate = req.ModifiedDate
                          }).ToList();

            var model = new SearchRecordList();
            int totalItemCount = result.Count();
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)sl.PageSize);
            List<SearchRecords> list1 = result.Skip((sl.CurrentPage - 1) * sl.PageSize).Take(sl.PageSize).ToList();

            model.list = list1;
            model.CurrentPage = sl.CurrentPage;
            model.TotalPages = totalPages;
            model.AdminId = admin.AdminId;
            model.AdminName = $"{admin.FirstName} {admin.LastName}";
            
            return model;
        }
        public bool RecordsDelete(int RequestId)
        {
            Entity.Models.Request hp = _context.Requests.Where(x => x.RequestId == RequestId).FirstOrDefault();
            hp.IsDeleted[0] = true;
            _context.Requests.Update(hp);
            _context.SaveChanges();
            return true;
        }
    }
}
