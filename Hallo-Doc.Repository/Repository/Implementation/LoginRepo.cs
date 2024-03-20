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

namespace Hallo_Doc.Repository.Repository.Implementation
{
    public class LoginRepo : ILogin
    {
        private readonly ApplicationDbContext _context;
        private readonly IDictionary<string, string> _reset;
        public LoginRepo(ApplicationDbContext context)
        {
            _context = context;
            _reset = new Dictionary<string, string>();
        }

        public async Task<AspnetUser?> Check(Login login, HttpContext httpContext)
        {
            var User = await _context.AspnetUsers.Include(x => x.AspNetUserRoles)
                .FirstOrDefaultAsync(m => m.Email == login.Email);
            bool pass = BCrypt.Net.BCrypt.Verify(login.Password, User.Passwordhash);
            if (User == null && !pass )
            {
                return null;
            }


            var userDetails = await _context.Users.FirstOrDefaultAsync(m => m.Aspnetuserid == User.Id);
            if (userDetails == null)
            {
                return null;
            }
            //    var aspNetUser = new AspnetUser {
            //        Id = User.Id,
            //    Username = User.Username,
            //    Email = User.Email,
            //    Passwordhash = User.Passwordhash,
            //    //AspNetUserRoles = User.AspNetUserRoles,
            //};
            httpContext.Session.SetInt32("UserId", userDetails.Userid);
            httpContext.Session.SetString("UserName", $"{userDetails.Firstname} {userDetails.Lastname}");
            return User;

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
                var token = Guid.NewGuid().ToString();
                SavePasswordResetToken(email, token);
                var resetLink = $"{baseUrl}/{controller}/{Action}?email={HttpUtility.UrlEncode(email)}&token={HttpUtility.UrlEncode(token)}";
                SendEmail(email, resetLink);
                return true;
            }
        }
        private void SavePasswordResetToken(string email, string token)
        {
            _reset[email] = token;
        }
        private void SendEmail(string email, string resetLink)
        {
            var smtpServer = "mail.etatvasoft.com";
            var smtpPort = 587;
            var smtpUsername = "hardi.jayani";
            var smtpPassword = "LHV0@}YOA?)M";
            var senderEmail = "hardi.jayani@etatvasoft.com";

            using (var client = new SmtpClient(smtpServer, smtpPort))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                client.EnableSsl = true;

                var mail = new MailMessage
                {
                    From = new MailAddress(senderEmail),
                    Subject = "Password Reset",
                    Body = $"Please reset your password by clicking the following link: <a href=\"{resetLink}\">{resetLink}</a>",
                    IsBodyHtml = true
                };
                mail.To.Add(email);
                client.Send(mail);
            }
        }
        public bool ValidateResetToken(string email, string token)
        {
            var savedToken = _reset.TryGetValue(email, out string storedToken) ? storedToken : null;
            
                return savedToken != null && HttpUtility.UrlDecode(token).Equals(savedToken);
          
        }
        public async Task<bool> Reset_password(string email, string token, string newPassword)
        {
            try
            {
                if (!ValidateResetToken(email, token))
                    return false;
                var user = _context.AspnetUsers.FirstOrDefault(x => x.Email == email);
                if (user == null)
                    return false;
                
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
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
