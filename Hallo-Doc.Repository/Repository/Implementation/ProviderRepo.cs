using Hallo_Doc.Entity.Data;
using Hallo_Doc.Entity.Models;
using Hallo_Doc.Entity.ViewModel;
using Hallo_Doc.Repository.Repository.Interface;
using System;
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
    }
}
