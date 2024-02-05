using Hallo_Doc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Hallo_Doc.Controllers
{
    public class PatientController : Controller
    {
        private readonly ILogger<PatientController> _logger;

        public PatientController(ILogger<PatientController> logger)
        {
            _logger = logger;
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Reset_password()
        {
            return View("Reset_password");
        }
        public IActionResult Create_request()
        {
            return View("Create_request");
        }
        public IActionResult Create_patient_req()
        {
            return View("Create_patient_req");
        }
        public IActionResult Create_family_req()
        {
            return View("Create_family_req");
        }
        public IActionResult Create_business_req()
        {
            return View("Create_business_req");
        }
        public IActionResult Create_concierge_req()
        {
            return View("Create_concierge_req");
        }
        public IActionResult Patient_dashboard()
        {
            return View("Patient_dashboard");
        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
