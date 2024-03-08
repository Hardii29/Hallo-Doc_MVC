using Hallo_Doc.Entity.Models;
using Hallo_Doc.Entity.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hallo_Doc.Repository.Repository.Interface
{
    public interface ILogin
    {
        Task<AspnetUser?> Check(Login login, HttpContext httpContext);
        Task<bool> ForgotPassword(string email, string Action, string controller, string baseUrl);
        bool ValidateResetToken(string email, string token);
        Task<bool> Reset_password(string email, string token, string newPassword);
    }
}
