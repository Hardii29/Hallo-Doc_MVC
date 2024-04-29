using AspNetCoreHero.ToastNotification.Abstractions;
using Hallo_Doc.Models;
using Hallo_Doc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Hallo_Doc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAdminDashboard _adminDashboard;
        private readonly INotyfService _notyf;
        public HomeController(ILogger<HomeController> logger, IAdminDashboard adminDashboard, INotyfService notyf)
        {
            _logger = logger;
            _adminDashboard = adminDashboard;
            _notyf = notyf;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Agreement(int RequestId)
        {
            TempData["RequestID"] = " " + RequestId;
            TempData["PatientName"] = "Hardi";
            return View();
        }
        public IActionResult Accept(int RequestID)
        {
            bool result = _adminDashboard.SendAgreement_accept(RequestID);
            if (result == true)
            {
                _notyf.Success("Agreement Accepted");
            }
            return RedirectToAction("Login", "Login");
        }

        public IActionResult Reject(int RequestID, string Notes)
        {
            bool result = _adminDashboard.SendAgreement_Reject(RequestID, Notes);
            if (result == true)
            {
                _notyf.Warning("Agreement Rejected");
            }
            return RedirectToAction("Login", "Login");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}