using Hallo_Doc.Data;
using Hallo_Doc.Models;
using Hallo_Doc.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;

namespace Hallo_Doc.Controllers
{
    public class PatientController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PatientController(ApplicationDbContext context)
        {
            _context = context;
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
        public async Task<IActionResult> Patient_dashboard()
        {
            var model = new DashboardList
            {
                Requests = _context.Requests.ToList()
            };
            //var requests = await _context.Requests
            //    .Include(r => r.RequestWiseFiles)
            //    .ToListAsync();

            //var requestViewModels = requests.Select(r => new RequestViewModel
            //{
            //    RequestId = r.RequestId,
            //    CreatedDate = (DateTime)r.CreatedDate,
            //    Status = r.Status,
            //    HasFile = r.RequestWiseFiles != null && r.RequestWiseFiles.Any()
            //}).ToList();

            //var model = new DashboardList
            //{
            //    Requests = requestViewModels
            //};

            return View(model);
        }
        public IActionResult View_document()
        {
            return View("View_document");
        }
        public IActionResult Patient_profile()
        {
            return View("Patient_profile");
        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
