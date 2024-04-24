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
        private readonly IEmail_SMS _services;
        public AdminLoginRepo(ApplicationDbContext context, IEmail_SMS services) 
        {
            _context = context;
            _services = services;
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
        public async Task<bool> ForgotPassword(string email, string Action, string controller, string baseUrl)
        {
            var UserExist = await _context.AspnetUsers
                .FirstOrDefaultAsync(m => m.Email == email);
            if (UserExist == null)
            {
                return false;
            }
            else
            {
                var resetLink = $"{baseUrl}/{controller}/{Action}";
                var subject = "Password Reset";
                var body = $"Please reset your password by clicking the following link: \n{resetLink}";
                _services.SendEmail(body, subject, email);
                return true;
            }
        }

        public async Task<bool> Reset_password(ForgotPassword model)
        {
            try
            {

                var user = _context.AspnetUsers.FirstOrDefault(x => x.Email == model.Email);
                if (user == null)
                    return false;

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
                user.Passwordhash = hashedPassword;
                _context.Update(user);
                await _context.SaveChangesAsync();
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating password: {ex.Message}");
                return false;
            }

        }
    }
}
