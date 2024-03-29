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
                Aspnetuser.Username = provider.FirstName;
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
    }
}