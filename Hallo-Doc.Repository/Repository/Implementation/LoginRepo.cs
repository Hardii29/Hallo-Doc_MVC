using Hallo_Doc.Entity.Data;
using Hallo_Doc.Entity.ViewModel;
using Hallo_Doc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hallo_Doc.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Net.Mail;
using System.Net;
using System.Security.Policy;
using System.Web;
using Twilio.TwiML.Messaging;

namespace Hallo_Doc.Repository.Repository.Implementation
{
    public class LoginRepo : ILogin
    {
        private readonly ApplicationDbContext _context;
        private readonly IDictionary<string, string> _reset;
        private readonly IEmail_SMS _services;
        public LoginRepo(ApplicationDbContext context, IEmail_SMS services)
        {
            _context = context;
            _reset = new Dictionary<string, string>();
            _services = services;
        }

        public async Task<AspnetUser?> Check(Login login, HttpContext httpContext)
        {
            var User = await _context.AspnetUsers.Include(x => x.AspNetUserRoles)
                .FirstOrDefaultAsync(m => m.Email == login.Email);
            bool pass = BCrypt.Net.BCrypt.Verify(login.Password, User.Passwordhash);
            if (User != null && pass == true )
            {
                var userDetails = await _context.Users.FirstOrDefaultAsync(m => m.Aspnetuserid == User.Id);
                httpContext.Session.SetInt32("UserId", userDetails.Userid);
                httpContext.Session.SetString("UserName", $"{userDetails.Firstname} {userDetails.Lastname}");
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
