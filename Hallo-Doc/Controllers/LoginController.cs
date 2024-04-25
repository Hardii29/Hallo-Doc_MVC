using Hallo_Doc.Entity.Data;
using Hallo_Doc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Hallo_Doc.Entity.Models;
using Npgsql;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Hallo_Doc.Entity.ViewModel;
using Microsoft.AspNetCore.Identity;
using System.Net.Mail;
using NuGet.Protocol.Plugins;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Hallo_Doc.Repository.Repository.Implementation;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace Hallo_Doc.Controllers
{
    //[CustomAuthorize("3")]
    public class LoginController : Controller
    {
        private readonly ILogin _login;
        private readonly ILogger<LoginController> _logger;
        private readonly IJWTService _jwtService;
        private readonly INotyfService _notyf;
        public LoginController(ILogger<LoginController> logger, ILogin login, IJWTService jwtService, INotyfService notyf)
        {
            _logger = logger;
            _login = login;
            _jwtService = jwtService;
            _notyf = notyf;
        }
        public IActionResult Login()
        {
            return View("~/Views/PatientUser/Login.cshtml");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Login(Login login)
        {
            var result = await _login.Check(login, HttpContext);
            if (result != null) {
                /* SessionUtils.SetLoggedInUser(HttpContext.Session, user);*/
                var jwtToken = _jwtService.GenerateToken(result);
                Response.Cookies.Append("jwt", jwtToken);
                _notyf.Success("Login Successfully..");
                return RedirectToAction("Patient_dashboard", "PatientUser");
            }
            else
            {
                _notyf.Error("Invalid Username and Password");
                return RedirectToAction("Login", "Login");
            }

        }
        public IActionResult Logout()
        {
            if (Response.Cookies != null)
            {
                Response.Cookies.Delete("jwt");
                
            }
            return RedirectToAction("Login", "Login");
        }
        public IActionResult ForgotPassword()
        {
            return View("~/Views/PatientUser/ForgotPassword.cshtml");
        }
        [AllowAnonymous, HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPassword fp)
        {
            if (fp.Email != null)
            {
                var baseUrl = "http://localhost:5203";
                var Action = "Reset_password";
                var controller = "Login";
                var result = await _login.ForgotPassword(fp.Email, Action, controller, baseUrl);
                if (result == true)
                {
                    _notyf.Success("Please Check your Mail..");
                    return RedirectToAction("ForgotPassword", "Login");
                }
            }
            _notyf.Error("Please enter Email");
            return RedirectToAction("ForgotPassword", "Login");
        }
        public IActionResult Reset_password()
        {
            return View("~/Views/PatientUser/Reset_password.cshtml");
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reset_password(ForgotPassword model)
        {
            if (ModelState.IsValid)
            {
                var passwordUpdate = await _login.Reset_password(model);
                if (passwordUpdate == true)
                {
                    _notyf.Success("Password Updated Successfully..");
                    return RedirectToAction("Login", "Login");
                }
            }
            _notyf.Error("Enter Valid Values");
            return RedirectToAction("ForgotPassword", "Login");
        }
    }
}
    

