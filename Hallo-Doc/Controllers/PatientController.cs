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

            return View(model);
        }
        public IActionResult View_document()
        {
            var files = new DashboardList
            {
                RequestWiseFiles = _context.RequestWiseFiles.ToList()
        };
            
            return View(files);
        }
        [HttpGet]
        public IActionResult DownloadFile(int fileID)
        {
            var file = _context.RequestWiseFiles.FirstOrDefault(f => f.RequestWiseFileId == fileID);
            if (file == null)
            {
                return NotFound();
            }
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Documents", file.FileName);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }
            return PhysicalFile(filePath, "application/octet-stream" ,file.FileName);
        }
        public IActionResult Patient_profile()
        {
            return View("Patient_profile");
        }
        public IActionResult Submit_req_Me()
        {
            return View("Submit_req_Me");
        }
        public IActionResult Submit_req_Someone()
        {
            return View("Submit_req_Someone");
        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
