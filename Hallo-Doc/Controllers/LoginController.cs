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


namespace Hallo_Doc.Controllers
{
    //[CustomAuthorize("3")]
    public class LoginController : Controller
    {
        private readonly ILogin _login;
        private readonly ILogger<LoginController> _logger;
        private readonly IJWTService _jwtService;
        public LoginController(ILogger<LoginController> logger, ILogin login, IJWTService jwtService)
        {
            _logger = logger;
            _login = login;
            _jwtService = jwtService;
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
                return RedirectToAction("Patient_dashboard", "PatientUser");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View();
            }

        }
        public IActionResult Logout()
        {
            if (Response.Cookies != null)
            {
                Response.Cookies.Delete("jwt");
                return RedirectToAction("Login", "Login");
            }
            return View();
        }
        [AllowAnonymous, HttpGet]
        public IActionResult ForgotPassword()
        {
            return View("~/Views/PatientUser/ForgotPassword.cshtml");
        }
        [AllowAnonymous, HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPassword fp)
        {

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            var Action = "Reset_password";
            var controller = "Login";
            var result = await _login.ForgotPassword(fp.Email, Action, controller, baseUrl);
            if (result)
            {
                return RedirectToAction("Login");
            }


            return View(fp);
        }
        [AllowAnonymous, HttpGet]
        public IActionResult Reset_password(string email, string token)
        {
            if (!_login.ValidateResetToken(email, token))
            {
                return RedirectToAction(nameof(ForgotPassword));
            }
            var model = new ForgotPassword { Email = email, Token = token };
            return View(model);
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reset_password(ForgotPassword model)
        {
            if (ModelState.IsValid)
            {
                
                if (!_login.ValidateResetToken(model.Email, model.Token))
                {
                    return RedirectToAction("ForgotPassword");
                }
                var passwordUpdate = await _login.Reset_password(model.Email, model.Token, model.Password);
                if (passwordUpdate)
                {
                    return RedirectToAction("Login", "Login");
                }
                else
                {
                    return RedirectToAction("ForgotPassword");
                }
            }
            return View(model);
        }
    }
}
    

