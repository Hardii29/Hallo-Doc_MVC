using AspNetCoreHero.ToastNotification.Abstractions;
using Hallo_Doc.Entity.ViewModel;
using Hallo_Doc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hallo_Doc.Controllers
{
    public class AdminLoginController : Controller
    {
        private readonly IAdminLogin _login;
        private readonly ILogger<AdminLoginController> _logger;
        private readonly IJWTService _jwtService;
        private readonly INotyfService _notyf;
        public AdminLoginController(ILogger<AdminLoginController> logger, IAdminLogin login, IJWTService jwtService, INotyfService notyf)
        {
            _logger = logger;
            _login = login;
            _jwtService = jwtService;
            _notyf = notyf;
        }
        public IActionResult AdminLogin()
        {
            return View("~/Views/Admin/AdminLogin.cshtml");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Login(Login login)
        {
            var result = await _login.Login(login);
            if (result != null && result.AspNetUserRoles.FirstOrDefault().RoleId == "Admin")
            {
                /* SessionUtils.SetLoggedInUser(HttpContext.Session, user);*/
                var jwtToken = _jwtService.GenerateToken(result);
                Response.Cookies.Append("jwt", jwtToken);
                return RedirectToAction("Admin_dashboard", "Admin");
            }
            else if (result != null && result.AspNetUserRoles.FirstOrDefault().RoleId == "Physician")
            {
                var jwtToken = _jwtService.GenerateToken(result);
                Response.Cookies.Append("jwt", jwtToken);
                return RedirectToAction("PhysicianDashboard", "Physician");
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
                
            }
            return RedirectToAction("AdminLogin", "AdminLogin");
        }
        public IActionResult AdminForgotPassword()
        {
            return View("~/Views/Admin/AdminForgotPassword.cshtml");
        }
        [AllowAnonymous, HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPassword fp)
        {
            if(fp.Email != null)
            {
                var baseUrl = "http://localhost:5203";
                var Action = "AdminResetPassword";
                var controller = "AdminLogin";
                var result = await _login.ForgotPassword(fp.Email, Action, controller, baseUrl);
                if (result == true)
                {
                    _notyf.Success("Please Check your Mail..");
                    return RedirectToAction("AdminForgotPassword", "AdminLogin");
                }
            }
            _notyf.Error("Please enter Email");
            return RedirectToAction("AdminForgotPassword", "AdminLogin");
        }
        public IActionResult AdminResetPassword()
        {
            return View("~/Views/Admin/AdminResetPassword.cshtml");
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ForgotPassword model)
        {
            if (ModelState.IsValid)
            {
                var passwordUpdate = await _login.Reset_password(model);
                if (passwordUpdate == true)
                {
                    _notyf.Success("Password Updated Successfully..");
                    return RedirectToAction("AdminLogin", "AdminLogin");
                }
            }
            _notyf.Error("Enter Valid Values");
            return RedirectToAction("AdminForgotPassword", "AdminLogin");
        }
    }
}
