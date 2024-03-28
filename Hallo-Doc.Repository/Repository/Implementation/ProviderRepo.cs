using Hallo_Doc.Entity.Data;
using Hallo_Doc.Entity.Models;
using Hallo_Doc.Entity.ViewModel;
using Hallo_Doc.Repository.Repository.Interface;
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
                var pr = new PhysicianRegion();

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
                Physician.RegionId = provider.RegionId;
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

                pr.PhysicianId = Physician.PhysicianId;
                pr.RegionId = provider.RegionId;
                _context.PhysicianRegions.Add(pr);
                _context.SaveChanges();
        }
    }
}