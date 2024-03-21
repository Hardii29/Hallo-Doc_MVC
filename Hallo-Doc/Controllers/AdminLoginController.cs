using Hallo_Doc.Entity.ViewModel;
using Hallo_Doc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Hallo_Doc.Controllers
{
    public class AdminLoginController : Controller
    {
        private readonly IAdminLogin _login;
        private readonly ILogger<AdminLoginController> _logger;
        private readonly IJWTService _jwtService;
        public AdminLoginController(ILogger<AdminLoginController> logger, IAdminLogin login, IJWTService jwtService)
        {
            _logger = logger;
            _login = login;
            _jwtService = jwtService;
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
            if (result != null)
            {
                /* SessionUtils.SetLoggedInUser(HttpContext.Session, user);*/
                var jwtToken = _jwtService.GenerateToken(result);
                Response.Cookies.Append("jwt", jwtToken);
                return RedirectToAction("Admin_dashboard", "Admin");
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
                return RedirectToAction("AdminLogin", "Login");
            }
            return View();
        }
      
    }
}
