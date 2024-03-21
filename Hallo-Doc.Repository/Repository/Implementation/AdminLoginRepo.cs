using Hallo_Doc.Entity.Data;
using Hallo_Doc.Entity.Models;
using Hallo_Doc.Entity.ViewModel;
using Hallo_Doc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Hallo_Doc.Repository.Repository.Implementation
{
    public class AdminLoginRepo: IAdminLogin
    {
        private readonly ApplicationDbContext _context;
        public AdminLoginRepo(ApplicationDbContext context) 
        {
            _context = context;
        }
        public async Task<AspnetUser?> Login(Login login)
        {
            var User = await _context.AspnetUsers.Include(x => x.AspNetUserRoles)
                .FirstOrDefaultAsync(m => m.Email == login.Email);
            if (User != null)
            {
                
                return User;
            }
       
            return null;
          
        }
      
    }
}
