using Hallo_Doc.Entity.Data;
using Hallo_Doc.Entity.Models;
using Hallo_Doc.Entity.ViewModel;
using Hallo_Doc.Repository.Repository.Interface;
using Microsoft.DotNet.Scaffolding.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hallo_Doc.Repository.Repository.Implementation
{
    public class ProviderRepo : IProvider
    {
        private readonly ApplicationDbContext _context;
        public ProviderRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public Provider CreateProvider()
        {
            var admin = _context.Admins.FirstOrDefault(a => a.AdminId == 1);
            return new Provider
            {

                AdminId = admin.AdminId,
                AdminName = $"{admin.FirstName} {admin.LastName}",

            };
        }
        public List<Region> GetRegions()
        {
            return _context.Regions.ToList();
        }
        public List<Role> GetRoles()
        {
            return _context.Roles.ToList();
        }
        public void AddProvider(Provider provider)
        {
            BitArray bit1 = new BitArray(1);
            bit1.Set(0, (bool)provider.IsAgreement);
            BitArray bit2 = new BitArray(1);
            bit2.Set(0, (bool)provider.IsBackground);
            BitArray bit3 = new BitArray(1);
            bit3.Set(0, (bool)provider.IsHIPAA);
            BitArray bit4 = new BitArray(1);
            bit4.Set(0, (bool)provider.IsNonDisclosure);
            
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(provider.Password);

                var Aspnetuser = new AspnetUser();
                var Physician = new Physician();

                Aspnetuser.Id = Guid.NewGuid().ToString();
                Aspnetuser.Username = $"MD.{provider.LastName}.{provider.FirstName.Substring(0, 1)}";
            Aspnetuser.Email = provider.Email;
                Aspnetuser.Passwordhash = hashedPassword;
                Aspnetuser.Phonenumber = provider.Mobile;
                Aspnetuser.Createddate = DateTime.Now;
                _context.AspnetUsers.Add(Aspnetuser);
                _context.SaveChanges();
            
                var lastPhy = _context.Physicians.OrderByDescending(p => p.PhysicianId).FirstOrDefault();
            var lastid = lastPhy != null ? lastPhy.PhysicianId : 1000;
            int newId = lastid + 1;
            Physician.PhysicianId = newId;
                Physician.AspNetUserId = Aspnetuser.Id;
                Physician.FirstName = !string.IsNullOrEmpty(provider.FirstName) ? provider.FirstName : "Unknown";
                Physician.LastName = provider.LastName;
                Physician.Email = !string.IsNullOrEmpty(provider.Email) ? provider.Email : "Unknown";
                Physician.Mobile = provider.Mobile;
                Physician.MedicalLicense = provider.MedicalLicense;
                Physician.AdminNotes = provider.AdminNotes;
                Physician.Address1 = provider.Address1;
                Physician.Address2 = provider.Address2;
                Physician.City = provider.City;
            //Physician.RegionId = (int)provider.StateId;
            Physician.Zip = provider.ZipCode;
                Physician.AltPhone = provider.AltPhone;
                Physician.CreatedBy = "Admin: "+provider.AdminName;
                Physician.CreatedDate = DateTime.Now;
                Physician.Status = 2;
                Physician.BusinessName = !string.IsNullOrEmpty(provider.BusinessName) ? provider.BusinessName : "Unknown";
                Physician.BusinessWebsite = !string.IsNullOrEmpty(provider.BusinessWebsite) ? provider.BusinessWebsite : "Unknown";
                Physician.RoleId = provider.RoleId;
                Physician.Npinumber = provider.NPI;
                Physician.IsAgreementDoc = bit1;
                Physician.IsBackgroundDoc = bit2;
                Physician.IsTrainingDoc = bit3;
                Physician.IsNonDisclosureDoc = bit4;
            if (provider.File != null && provider.File.Length > 0)
            {
                var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Documents");

                if (!Directory.Exists(uploadsDirectory))
                {
                    Directory.CreateDirectory(uploadsDirectory);
                }

                var fileName = Path.GetFileName(provider.File.FileName);
                var filePath = Path.Combine(uploadsDirectory, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    provider.File.CopyTo(stream);
                }

                Physician.Photo = fileName;
            }
                _context.Physicians.Add(Physician);
                _context.SaveChanges();

            foreach (var regionId in provider.SelectRegion)
            {
                var pr = new PhysicianRegion
                {
                    PhysicianId = Physician.PhysicianId,
                    RegionId = regionId,
                };
                _context.PhysicianRegions.Add(pr);
                
            }
            _context.SaveChanges();
        }
        public Provider? PhysicianAccount(int ProviderId)
        {
            BitArray bit1 = new BitArray(1);
            bit1.Set(0, true);
            BitArray bit2 = new BitArray(1);
            bit2.Set(0, true);
            BitArray bit3 = new BitArray(1);
            bit3.Set(0, true);
            BitArray bit4 = new BitArray(1);
            bit4.Set(0, true);
            BitArray bit5 = new BitArray(1);
            bit5.Set(0, true);
            var admin = _context.Admins.FirstOrDefault(a => a.AdminId == 1);
            var model = (from p in _context.Physicians
                         where p.PhysicianId == ProviderId
                         select new Provider
                         {
                             ProviderId = ProviderId,
                             UserName = $"MD.{p.LastName}.{p.FirstName.Substring(0, 1)}",
                             Status = (int)p.Status,
                             RoleId = (int)p.RoleId,
                             FirstName = p.FirstName,
                             LastName = p.LastName,
                             Mobile = p.Mobile,
                             Email = p.Email,
                             MedicalLicense = p.MedicalLicense,
                             NPI = p.Npinumber,
                             SyncEmail = p.Email,
                             Address1 = p.Address1,
                             Address2 = p.Address2,
                             City = p.City,
                             StateId = (int)p.RegionId,
                             ZipCode = p.Zip,
                             AltPhone = p.AltPhone,
                             BusinessName = p.BusinessName,
                             BusinessWebsite = p.BusinessWebsite,
                             AdminNotes = p.AdminNotes,
                             Photo = p.Photo,
                             Sign = p.Signature,
                             IsAgreement = p != null ? (p.IsAgreementDoc == bit1): false,
                             IsBackground = p != null ? (p.IsBackgroundDoc == bit2) : false,
                             IsHIPAA = p != null ? (p.IsTrainingDoc == bit3) : false,
                             IsNonDisclosure = p != null ? (p.IsNonDisclosureDoc == bit4) : false,
                             IsLicense = p != null ? (p.IsLicenceDoc == bit5) : false,
                             AdminId = admin.AdminId,
                             AdminName = $"{admin.FirstName} {admin.LastName}",
                             
                         }).FirstOrDefault();

            return model;
        }
        public void EditPrAccount(Provider pr)
        {
            var user = _context.Physicians.FirstOrDefault(p => p.PhysicianId == pr.ProviderId);
            if (user != null)
            {
                user.RoleId = pr.RoleId;
                user.Status = (short?)pr.Status;
                user.ModifiedBy = $"Admin: {pr.AdminName}";
                user.ModifiedDate = DateTime.Now;
                _context.Physicians.Update(user);
                _context.SaveChanges();
            }
        }
        public void EditPrInfo(Provider pr)
        {
            var user = _context.Physicians.FirstOrDefault(p => p.PhysicianId == pr.ProviderId);
            if (user != null)
            {
                user.FirstName = pr.FirstName;
                user.LastName = pr.LastName;
                user.Email = pr.Email;
                user.Mobile = pr.Mobile;
                user.MedicalLicense = pr.MedicalLicense;
                user.Npinumber = pr.NPI;
                user.SyncEmailAddress = pr.SyncEmail;
                user.ModifiedBy = $"Admin: {pr.AdminName}";
                user.ModifiedDate = DateTime.Now;
                _context.Physicians.Update(user);
                _context.SaveChanges();
            }
        }
        public void EditPrBilling(Provider pr)
        {
            var user = _context.Physicians.FirstOrDefault(p => p.PhysicianId == pr.ProviderId);
            if (user != null)
            {
                user.Address1 = pr.Address1;
                user.Address2 = pr.Address2;
                user.City = pr.City;
                user.Zip = pr.ZipCode;
                user.AltPhone = pr.AltPhone;
                user.ModifiedBy = $"Admin: {pr.AdminName}";
                user.ModifiedDate = DateTime.Now;
                _context.Physicians.Update(user);
                _context.SaveChanges();
            }
        }
        public void EditPrbusiness(Provider pr)
        {
            var user = _context.Physicians.FirstOrDefault(p => p.PhysicianId == pr.ProviderId);
            if (user != null)
            {
                user.BusinessName = pr.BusinessName;
                user.BusinessWebsite = pr.BusinessWebsite;
                if (pr.File != null && pr.File.Length > 0)
                {
                    var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Documents");

                    if (!Directory.Exists(uploadsDirectory))
                    {
                        Directory.CreateDirectory(uploadsDirectory);
                    }

                    var fileName = Path.GetFileName(pr.File.FileName);
                    var filePath = Path.Combine(uploadsDirectory, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        pr.File.CopyTo(stream);
                    }

                    user.Photo = fileName;
                }
                if (pr.FileSgn != null && pr.FileSgn.Length > 0)
                {
                    var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Documents");

                    if (!Directory.Exists(uploadsDirectory))
                    {
                        Directory.CreateDirectory(uploadsDirectory);
                    }

                    var fileName = Path.GetFileName(pr.FileSgn.FileName);
                    var filePath = Path.Combine(uploadsDirectory, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        pr.FileSgn.CopyTo(stream);
                    }

                    user.Signature = fileName;
                }
                user.AdminNotes = pr.AdminNotes;
                user.ModifiedBy = $"Admin: {pr.AdminName}";
                user.ModifiedDate = DateTime.Now;
                _context.Physicians.Update(user);
                _context.SaveChanges();
            }
        }
        public void EditOnbording(Provider pr)
        {
            BitArray bit1 = new BitArray(1);
            bit1.Set(0, (bool)pr.IsAgreement);
            BitArray bit2 = new BitArray(1);
            bit2.Set(0, (bool)pr.IsBackground);
            BitArray bit3 = new BitArray(1);
            bit3.Set(0, (bool)pr.IsHIPAA);
            BitArray bit4 = new BitArray(1);
            bit4.Set(0, (bool)pr.IsNonDisclosure);
            BitArray bit5 = new BitArray(1);
            bit5.Set(0, (bool)pr.IsLicense);
            var user = _context.Physicians.FirstOrDefault(p => p.PhysicianId == pr.ProviderId);
            if (user != null)
            {
                user.IsAgreementDoc = bit1;
                user.IsBackgroundDoc = bit2;
                user.IsTrainingDoc = bit3;
                user.IsNonDisclosureDoc = bit4;
                user.IsLicenceDoc = bit5;
                user.ModifiedBy = $"Admin: {pr.AdminName}";
                user.ModifiedDate = DateTime.Now;
                _context.Physicians.Update(user);
                _context.SaveChanges();
            }
        }
        public void DeletePrAccount(int ProviderId) 
        {
            var user = _context.Physicians.FirstOrDefault(p => p.PhysicianId == ProviderId);
            if (user != null)
            {
                var id = _context.AspnetUsers.FirstOrDefault(a => a.Id == user.AspNetUserId);
                var notifications = _context.PhysicianNotifications.Where(n => n.PhysicianId == ProviderId);
                _context.PhysicianNotifications.RemoveRange(notifications);
                _context.SaveChanges();

                var locations = _context.PhysicianLocations.Where(l => l.PhysicianId == ProviderId);
                _context.PhysicianLocations.RemoveRange(locations);
                _context.SaveChanges();

                var region = _context.PhysicianRegions.Where(r => r.PhysicianId == ProviderId);
                _context.PhysicianRegions.RemoveRange(region);
                _context.SaveChanges();

                var rsl = _context.RequestStatusLogs.Where(rsl => rsl.PhysicianId == ProviderId);
                _context.RequestStatusLogs.RemoveRange(rsl);
                _context.SaveChanges();

                //var shift = _context.Shifts.Where(s => s.PhysicianId == ProviderId);
                //_context.Shifts.RemoveRange(shift);
                //_context.SaveChanges();

                var requests = _context.Requests.Where(r => r.PhysicianId == ProviderId);
                foreach (var request in requests)
                {
                    request.PhysicianId = null;
                }
                _context.SaveChanges();

                _context.Physicians.Remove(user);
                _context.SaveChanges();
                //_context.AspnetUsers.RemoveRange(id);
                //_context.SaveChanges();
            }
        }
    }
}