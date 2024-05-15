using Hallo_Doc.Entity.Data;
using Hallo_Doc.Entity.Models;
using Hallo_Doc.Entity.ViewModel;
using Hallo_Doc.Repository.Repository.Interface;
using iText.Layout.Element;
using Microsoft.AspNetCore.Hosting;
using Microsoft.DotNet.Scaffolding.Shared;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Twilio.Http;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Hallo_Doc.Repository.Repository.Implementation
{
    public class AdminNavbarRepo : IAdminNavbar
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmail_SMS _services;
        public AdminNavbarRepo(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, IEmail_SMS services)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _services = services;
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
        public bool SendMailPhy(string email, string Message, string ProviderName)
        {
            var subject = "Message from Admin";
            var body = $"Dear Doctor {ProviderName},\n\nPlease find Message : \n{Message}";
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
        public bool SendSMS(string Mobile, string Message, string ProviderName)
        {
            var msg = $"Dear Doctor {ProviderName},\n\nPlease find Message : \n{Message}";
            _services.SendSMS(Mobile, msg);
            BitArray bitArray = new BitArray(1);
            bitArray.Set(0, true);
            Smslog sms = new Smslog
            {
                Smstemplate = msg,
                MobileNumber = Mobile,
                CreateDate = DateTime.Now,
                SentDate = DateTime.Now,
                IsSmssent = bitArray,
                AdminId = 1,
                RoleId = 1,
                Action = 5,
                SentTries= 1,
            };
            _context.Smslogs.Add(sms);
            _context.SaveChanges();
            return true;
        }
        public AccountAccess Access()
        {
            var admin = _context.Admins.FirstOrDefault(a => a.AdminId == 1);

            var query = from r in _context.Roles
                        where r.IsDeleted == new BitArray(1)
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
        public List<AspNetRole> GetNetRoles()
        {
            return _context.AspNetRoles.ToList();
        }
        public UserAccess UserAccess(string AccountType)
        {
            var u = _context.Admins.FirstOrDefault(a => a.AdminId == 1);
            BitArray bitArray = new BitArray(1);
            bitArray.Set(0, false);
            var query = (from aspuser in _context.AspnetUsers
                         join admin in _context.Admins
                         on aspuser.Id equals admin.AspNetUserId into AdminGroup
                         from admin in AdminGroup.DefaultIfEmpty()
                         join physician in _context.Physicians
                         on aspuser.Id equals physician.AspNetUserId into PhyGroup
                         from physician in PhyGroup.DefaultIfEmpty()
                         where (admin != null || physician != null) && (admin.IsDeleted == new BitArray(1) || physician.IsDeleted == new BitArray(1))
                         select new UserAccess
                        {
                             Id = admin != null ? admin.AdminId : (physician != null ? physician.PhysicianId : 0),
                             AccountType = admin != null ? "Admin" : (physician != null ? "Physician" : null),
                             UserName = admin != null ? admin.FirstName + " " + admin.LastName : (physician != null ? physician.FirstName + " " + physician.LastName : null),
                             Status = (short)(admin != null ? admin.Status : (physician != null ? physician.Status : null)),
                             Phone = admin != null ? admin.Mobile : (physician != null ? physician.Mobile : null),
                             RequestCount = _context.Requests.Count(r => r.PhysicianId == physician.PhysicianId),
                             
                         }).ToList();
            if (AccountType != null)
            {
                query = query.Where(r => r.AccountType == "All" || r.AccountType == AccountType).ToList();
            }
            var model = new UserAccess()
            {
                data = query,
                AdminId = u.AdminId,
                AdminName = $"{u.FirstName} {u.LastName}"
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
        public AccountAccess ViewEditRole(int RoleId)
        {
            var admin = _context.Admins.FirstOrDefault(a => a.AdminId == 1);
            AccountAccess? v = (from r in _context.Roles

                             where r.RoleId == RoleId
                             select new AccountAccess
                             {
                                 RoleName = r.Name,
                                 SelectedType = (AccountType)r.AccountType,
                             }).FirstOrDefault();
            List<Menu> Menu = _context.Menus
                .Where(req => req.AccountType == (short)v.SelectedType).ToList();
            v.menus = Menu;
            List<RoleMenu> rm = _context.RoleMenus
                                .Where(obj => obj.RoleId == RoleId).ToList();
            v.editRole = rm;
            v.AdminId = admin.AdminId;
            v.AdminName = $"{admin.FirstName} {admin.LastName}";
            return v;
        }
        public bool SaveEditRole(AccountAccess roles)
        {
            List<int> selectedmenus = roles.list.Split(',').Select(int.Parse).ToList();
            List<int> rolemenus = _context.RoleMenus.Where(r => r.RoleId == roles.RoleId).Select(req => req.RoleMenuId).ToList();

            if (rolemenus.Count > 0)
            {
                foreach (var item in rolemenus)
                {
                    RoleMenu ar = _context.RoleMenus.Where(r => r.RoleId == roles.RoleId).First();
                    _context.RoleMenus.Remove(ar);
                    _context.SaveChanges();
                }
            }
            foreach (var item in selectedmenus)
            {
                RoleMenu ar = new()
                {
                    RoleId = roles.RoleId,
                    MenuId = item
                };
                _context.RoleMenus.Update(ar);
                _context.SaveChanges();
            }
            return true;
        }
        public bool DeleteRole(int RoleId)
        {
            Role r = _context.Roles.Where(x => x.RoleId == RoleId).FirstOrDefault();
            r.IsDeleted[0] = true;
            r.ModifiedDate = DateTime.Now;
            _context.Roles.Update(r);
            _context.SaveChanges();
            return true;
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
        public void CreateShift(Schedule schedule)
        {
            BitArray bitArray = new BitArray(1);
            bitArray.Set(0, false);
            Shift s = new Shift();
            ShiftDetail sd = new ShiftDetail();
            ShiftDetailRegion sdr = new ShiftDetailRegion();

            s.PhysicianId = schedule.PhysicianId;
            s.StartDate = schedule.ShiftDate;
            s.IsRepeat = bitArray;
            s.CreatedBy = "Admin";
            s.CreatedDate = DateTime.Now;
            _context.Shifts.Add(s);
            _context.SaveChanges();

            sd.ShiftId = s.ShiftId;
            sd.ShiftDate = new DateTime(schedule.ShiftDate.Year, schedule.ShiftDate.Month, schedule.ShiftDate.Day, schedule.StartTime.Hour, schedule.StartTime.Minute, schedule.StartTime.Second);
            sd.RegionId = schedule.RegionId;
            sd.StartTime = schedule.StartTime;
            sd.EndTime = schedule.EndTime;
            sd.Status = 2;
            sd.IsDeleted = bitArray;
            _context.ShiftDetails.Add(sd);
            _context.SaveChanges();

            sdr.ShiftDetailId = sd.ShiftDetailId;
            sdr.RegionId = schedule.RegionId;
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
            BitArray bitArray = new BitArray(1);
            bitArray.Set(0, false);
            List<Schedule> allData = (from s in _context.Shifts
                                      join shiftDetail in _context.ShiftDetails
                                      on s.ShiftId equals shiftDetail.ShiftId into shiftGroup
                                      from sd in shiftGroup.DefaultIfEmpty()
                                      join p in _context.Physicians
                                      on s.PhysicianId equals p.PhysicianId into phyShift
                                      from ps in phyShift.DefaultIfEmpty()
                                      where sd.IsDeleted == bitArray
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
                            RegionId = (int)sd.RegionId,
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
        public void ChangeStatus(int ShiftId)
        {
            ShiftDetail sd = _context.ShiftDetails.FirstOrDefault(sd => sd.ShiftId == ShiftId);
            if (sd.Status == 2)
            {
                sd.Status = 4;
                sd.ModifiedBy = "Admin";
                sd.ModifiedDate = DateTime.Now;
                _context.ShiftDetails.Update(sd);
                _context.SaveChanges();
            }
            else
            {
                sd.Status = 2;
                sd.ModifiedBy = "Admin";
                sd.ModifiedDate = DateTime.Now;
                _context.ShiftDetails.Update(sd);
                _context.SaveChanges();
            }
        }
        public MDsOnCall MDsOnCall()
        {
            var admin = _context.Admins.FirstOrDefault(a => a.AdminId == 1);
            DateTime currentDateTime = DateTime.Now;
            TimeOnly currentTimeOfDay = TimeOnly.FromDateTime(DateTime.Now);
            List<MDsOnCall> query = (from p in _context.Physicians
                                     select new MDsOnCall
                         {
                             PhysicianId = p.PhysicianId,
                             PhysicianName = p.FirstName + " " + p.LastName,
                              Photo = p.Photo
                         }).ToList();
            foreach (var item in query)
            {
                List<int> shiftIds = (from s in _context.Shifts
                                      where s.PhysicianId == item.PhysicianId
                                      select s.ShiftId).ToList();

                foreach (var shift in shiftIds)
                {
                    var shiftDetail = (from sd in _context.ShiftDetails
                                       where sd.ShiftId == shift &&
                                             sd.ShiftDate.Date == currentDateTime.Date &&
                                             sd.StartTime <= currentTimeOfDay &&
                                             currentTimeOfDay <= sd.EndTime
                                       select sd).FirstOrDefault();

                    if (shiftDetail != null)
                    {
                        item.onCallStatus = 1;
                    }
                }
            }

            var model = new MDsOnCall()
            {
                data = query,
                AdminId = admin.AdminId,
                AdminName = $"{admin.FirstName} {admin.LastName}"
            };
            return model;
        }
        public RequestedShift RequestedShift(int? regionId)
        {
            var admin = _context.Admins.FirstOrDefault(a => a.AdminId == 1);
            BitArray bitArray = new BitArray(1);
            bitArray.Set(0, false);
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
                        where (regionId == null || regionId == -1 || sd.RegionId == regionId) && sd.Status == 2 && sd.IsDeleted == bitArray
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
        public async Task<bool> DeleteReqShift(string s)
        {
            List<int> shidtID = s.Split(',').Select(int.Parse).ToList();
            try
            {
                foreach (int i in shidtID)
                {
                    ShiftDetail sd = _context.ShiftDetails.FirstOrDefault(sd => sd.ShiftDetailId == i);
                    if (sd != null)
                    {
                        sd.IsDeleted[0] = true;
                        sd.ModifiedBy = "Admin";
                        sd.ModifiedDate = DateTime.Now;
                        _context.ShiftDetails.Update(sd);
                        _context.SaveChanges();
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> UpdateStatusShift(string s)
        {
            List<int> shidtID = s.Split(',').Select(int.Parse).ToList();
            try
            {
                foreach (int i in shidtID)
                {
                    ShiftDetail sd = _context.ShiftDetails.FirstOrDefault(sd => sd.ShiftDetailId == i);
                    if (sd != null)
                    {
                        sd.Status = (short)(sd.Status == 2 ? 4 : 2);
                        sd.ModifiedBy = "Admin";
                        sd.ModifiedDate = DateTime.Now;
                        _context.ShiftDetails.Update(sd);
                        _context.SaveChanges();
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
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
            BitArray bitArray = new BitArray(1);
            bitArray.Set(0, false);
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
            data.IsDeleted = bitArray;
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
        public UserData PatientHistory(UserData data)
        {
            var admin = _context.Admins.FirstOrDefault(a => a.AdminId == 1);

            var query = (from u in _context.Users
                        .Where(d => (string.IsNullOrEmpty(data.Firstname) || d.Firstname.Contains(data.Firstname))
                               && (string.IsNullOrEmpty(data.Lastname) || d.Lastname.Contains(data.Lastname))
                               && (string.IsNullOrEmpty(data.Email) || d.Email.Contains(data.Email))
                               && (string.IsNullOrEmpty(data.Mobile) || d.Mobile.Contains(data.Mobile)))
                        select new UserData
                        {
                            Id = u.Userid,
                            Firstname = u.Firstname,
                            Lastname = u.Lastname,
                            Email = u.Email,
                            Mobile = u.Mobile,
                            Address = u.Street + ", " + u.City + ", " + u.State,
                        }).ToList();
            var model = new UserData();
            int totalItemCount = query.Count();
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)data.PageSize);
            List<UserData> list1 = query.Skip((data.CurrentPage - 1) * data.PageSize).Take(data.PageSize).ToList();

            model.data = list1;
            model.CurrentPage = data.CurrentPage;
            model.TotalPages = totalPages;
            model.AdminId = admin.AdminId;
            model.AdminName = $"{admin.FirstName} {admin.LastName}";
           
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
                              ConcludedDate = req.ModifiedDate,
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
       
        public ProviderLocation ProviderLocation()
        {
            var admin = _context.Admins.FirstOrDefault(a => a.AdminId == 1);

            var result = (from p in _context.PhysicianLocations
                          orderby p.PhysicianId ascending
                          select new ProviderLocation
                          {
                              LocationId = p.LocationId,
                              PhysicianId = p.PhysicianId,
                              lng = p.Longitude,
                              lat = p.Latitude,
                              PhyName = p.PhysicianName,
                              Address = p.Address,
                          }).ToList();

            var model = new ProviderLocation()
            {
                Locations = result,
                AdminId = admin.AdminId,
                AdminName = $"{admin.FirstName} {admin.LastName}"
            };
            return model;
        }
        public Logs EmailLog(Logs el)
        {
            var admin = _context.Admins.FirstOrDefault(a => a.AdminId == 1);
            var result = (from em in _context.EmailLogs
                          join req in _context.Requests
                          on em.RequestId equals req.RequestId into Group
                          from rc in Group.DefaultIfEmpty()
                          where (el.Role == 0 || em.RoleId == el.Role) &&
                                                   (!el.CreatedDate.HasValue || em.CreateDate.Value.Date == el.CreatedDate.Value.Date) &&
                                                   (!el.SentDate.HasValue || em.SentDate == el.SentDate.Value.Date) &&
                                                   (el.Recipient.IsNullOrEmpty() || (rc.FirstName).ToLower().Contains(el.Recipient.ToLower())) &&
                                                   (el.Email.IsNullOrEmpty() || em.EmailId.ToLower().Contains(el.Email.ToLower()))
                          orderby em.CreateDate descending
                          select new Logs
                          {
                              Recipient = _context.AspnetUsers.Where(req => req.Email == em.EmailId).Select(req => req.Username).FirstOrDefault(),
                              Confirmation = em.ConfirmationNumber ?? " - ",
                              CreatedDate = em.CreateDate,
                              SentDate = em.SentDate,
                              RoleId = (AccountType)em.RoleId,
                              Email = em.EmailId,
                              IsEmailSent = (em.IsEmailSent[0] == false ? "No" : "Yes"),
                              SentTries = em.SentTries,
                              Action = em.SubjectName,
                          }).ToList();

            var model = new Logs();
            int totalItemCount = result.Count();
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)el.PageSize);
            List<Logs> list1 = result.Skip((el.CurrentPage - 1) * el.PageSize).Take(el.PageSize).ToList();
            model.EmailLog = list1;
            model.CurrentPage = el.CurrentPage;
            model.TotalPages = totalPages;
            model.AdminId = admin.AdminId;
            model.AdminName = $"{admin.FirstName} {admin.LastName}";

            return model;
        }
        public SMSLog SMSLog(SMSLog sl)
        {
            var admin = _context.Admins.FirstOrDefault(a => a.AdminId == 1);
            var result = (from em in _context.Smslogs
                          join req in _context.Requests
                          on em.RequestId equals req.RequestId into Group
                          from rc in Group.DefaultIfEmpty()
                          where (sl.Role == 0 || em.RoleId == sl.Role) &&
                                                   (!sl.CreatedDate.HasValue || em.CreateDate.Date == sl.CreatedDate.Value.Date) &&
                                                   (!sl.SentDate.HasValue || em.SentDate == sl.SentDate.Value.Date) &&
                                                   (sl.Recipient.IsNullOrEmpty() || (rc.FirstName).ToLower().Contains(sl.Recipient.ToLower())) &&
                                                   (sl.Mobile.IsNullOrEmpty() || em.MobileNumber.ToLower().Contains(sl.Mobile.ToLower()))
                          orderby em.CreateDate descending
                          select new SMSLog
                          {
                              Recipient = _context.AspnetUsers.Where(req => req.Phonenumber == em.MobileNumber).Select(req => req.Username).FirstOrDefault(),
                              Confirmation = em.ConfirmationNumber ?? " - ",
                              CreatedDate = em.CreateDate,
                              SentDate = em.SentDate,
                              RoleId = (AccountType)em.RoleId,
                              Mobile = em.MobileNumber,
                              IsSMSSent = (em.IsSmssent[0] == false ? "No" : "Yes"),
                              SentTries = em.SentTries,
                              Action = (LogsAction)em.Action,
                          }).ToList();

            var model = new SMSLog();
            int totalItemCount = result.Count();
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)sl.PageSize);
            List<SMSLog> list1 = result.Skip((sl.CurrentPage - 1) * sl.PageSize).Take(sl.PageSize).ToList();
            model.list = list1;
            model.CurrentPage = sl.CurrentPage;
            model.TotalPages = totalPages;
            model.AdminId = admin.AdminId;
            model.AdminName = $"{admin.FirstName} {admin.LastName}";

            return model;
        }
        public AdminProfile CreateAdmin()
        {
            var admin = _context.Admins.FirstOrDefault(a => a.AdminId == 1);
            return new AdminProfile
            {

                AdminId = admin.AdminId,
                AdminName = $"{admin.FirstName} {admin.LastName}",

            };
        }
        public void AddAdmin(AdminProfile admin)
        {
            BitArray bitArray = new BitArray(1);
            bitArray.Set(0, false);
            var user = _context.Admins.FirstOrDefault(a => a.AdminId == 1);
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(admin.Password);

            var Aspnetuser = new AspnetUser();
            var Admin = new Admin();
            Aspnetuser.Id = Guid.NewGuid().ToString();
            Aspnetuser.Username = admin.FirstName;
            Aspnetuser.Email = admin.Email;
            Aspnetuser.Passwordhash = hashedPassword;
            Aspnetuser.Phonenumber = admin.Mobile;
            Aspnetuser.Createddate = DateTime.Now;
            _context.AspnetUsers.Add(Aspnetuser);
            _context.SaveChanges();

            Admin.AspNetUserId = Aspnetuser.Id;
            Admin.FirstName = admin.FirstName;
            Admin.LastName = admin.LastName;
            Admin.Email = admin.Email;
            Admin.Mobile = admin.Mobile;
            Admin.Address1 = admin.Address1;
            Admin.Address2 = admin.Address2;
            Admin.City = admin.City;
            Admin.Zip = admin.ZipCode;
            Admin.AltPhone = admin.AltPhone;
            Admin.CreatedBy = user.FirstName;
            Admin.CreatedDate = DateTime.Now;
            Admin.IsDeleted = bitArray;
            Admin.Status = 2;
            Admin.RoleId = admin.RoleId;
            _context.Admins.Add(Admin);
            _context.SaveChanges();
            foreach (var regionId in admin.SelectRegion)
            {
                var pr = new AdminRegion
                {
                    AdminId = Admin.AdminId,
                    RegionId = regionId,
                };
                _context.AdminRegions.Add(pr);

            }
            _context.SaveChanges();
        }
        public ShowTimeSheet GetBiWeeklySheet(DateOnly startDate, DateOnly endDate, int PhysicianId)
        {
            var exist = _context.Invoices.FirstOrDefault(i => i.StartDate == startDate && i.EndDate == endDate && i.PhysicianId == PhysicianId);
            var provider = _context.Physicians.FirstOrDefault(p => p.PhysicianId == PhysicianId);
            if (exist != null)
            {
                var data = _context.TimeSheets.Where(t=> t.InvoiceId == exist.InvoiceId).ToList();
             

                var model = new ShowTimeSheet()
                {
                    TimeSheetDetail = data,
                    IsFinalize = exist.IsFinalize,
                    IsApproved = exist.IsApproved,
                    PhysicianName = $"{provider.FirstName} {provider.LastName}",
                    startDate = startDate,
                    endDate = endDate,
                };
                return model;
            }

            return null;
        }
        public TimesheetData TimeSheet(DateOnly startDate, DateOnly endDate, int PhysicianId)
        {
            var invoice = _context.Invoices.FirstOrDefault(i => i.PhysicianId == PhysicianId && i.StartDate == startDate && i.EndDate == endDate);
            var result = _context.TimeSheets.Where(r => r.Date >= startDate && r.Date <= endDate && r.InvoiceId == invoice.InvoiceId).ToList();
            var payRate = _context.PayRates.FirstOrDefault(p => p.PhysicianId == PhysicianId);
            var payAmount = _context.PayRates.Where(p => p.PhysicianId == PhysicianId).ToList();
            TimesheetData t = new();
            t.PayRateInfo = payAmount;
            t.TimeSheetInfo = result;
            t.endDate = endDate;
            t.startDate = startDate;
            t.PhysicianId = PhysicianId;
            if (payRate != null && result != null)
            {
                int hourlyRate = int.Parse(payRate.Shift);
                int phoneConsultRate = int.Parse(payRate.PhoneConsults);
                int houseCallRate = int.Parse(payRate.Housecall);
                int weekendRate = int.Parse(payRate.NightShiftWeekend);
                int totalHours = 0;
                int phoneConsults = 0;
                int houseCalls = 0;
                int weekends = 0;
                foreach (var entry in result)
                {
                    totalHours += (int)entry.TotalHours;
                    phoneConsults += (int)entry.NoOfPhoneCall;
                    houseCalls += (int)entry.NoOfHouseCall;
                    if (entry.Holiday == true) // Check if the entry is for a weekend
                    {
                        weekends++;
                    }
                }
                t.ShiftTotal = totalHours * hourlyRate;
                t.HouseCallTotal = houseCalls * houseCallRate;
                t.PhoneCallTotal = phoneConsults * phoneConsultRate;
                t.WeekendTotal = weekends * weekendRate;
                t.InvoiceTotal = t.HouseCallTotal + t.PhoneCallTotal + t.ShiftTotal + t.WeekendTotal;
            }
                return t;
        }
        public bool TimeSheetSave(TimesheetData model)
        {
            var count = 0;

            try
            {
                var invoiceId = 0;
                var invoice = _context.Invoices
                .FirstOrDefault(r => r.StartDate == model.startDate && r.EndDate == model.endDate && r.PhysicianId == model.PhysicianId);
           
                    invoiceId = invoice.InvoiceId;
                

                for (var i = model.startDate; i <= model.endDate; i = i.AddDays(1))
                {
                    var detail = _context.TimeSheets.FirstOrDefault(x => x.Date == i && x.InvoiceId == invoiceId);
                    if (detail != null)
                    {
                        detail.Date = default;
                        if (model.TotalHours[count] != null)
                        {
                            detail.Date = i;
                            detail.TotalHours = Convert.ToInt32(model.TotalHours[count]);
                        }
                        if (model.IsWeekend[count] != false)
                        {
                            detail.Date = i;
                            detail.Holiday = model.IsWeekend[count];
                        }
                        if (model.NoofPhoneConsult[count] != null)
                        {
                            detail.Date = i;
                            detail.NoOfPhoneCall = Convert.ToInt32(model.NoofPhoneConsult[count]);
                        }
                        if (model.NoofHousecall[count] != null)
                        {
                            detail.Date = i;
                            detail.NoOfHouseCall = Convert.ToInt32(model.NoofHousecall[count]);
                        }
                        
                        if (detail.Date != default)
                        {
                            detail.InvoiceId = invoiceId;
                            detail.ModifiedDate = DateTime.Now;
                            _context.TimeSheets.Update(detail);
                            _context.SaveChanges();
                        }

                    }
                    

                    count++;
                }
                return true;
            }
            catch (Exception)
            {
                return false;

            }
        }
        public bool ApproveSheet(DateOnly StartDate, DateOnly EndDate, int PhysicianId, string Bonus, string Discription)
        {
            var invoice = _context.Invoices.FirstOrDefault(i => i.StartDate ==  StartDate && i.EndDate == EndDate && i.PhysicianId == PhysicianId);
            if (invoice != null)
            {
                invoice.IsApproved = true;
                invoice.Bonus = Bonus;
                invoice.AdminNotes = Discription;
                _context.Invoices.Update(invoice);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
