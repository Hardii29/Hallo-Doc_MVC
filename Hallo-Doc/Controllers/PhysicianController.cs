using AspNetCoreHero.ToastNotification.Abstractions;
using Hallo_Doc.Entity.ViewModel;
using Hallo_Doc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared;

namespace Hallo_Doc.Controllers
{
    public class PhysicianController : Controller
    {
        private readonly IPhysician _physician;
        private readonly IAdminDashboard _adminDashboard;
        private readonly IProvider _provider;
        private readonly ILogger<PhysicianController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly INotyfService _notyf;
        public PhysicianController(ILogger<PhysicianController> logger, IPhysician physician, IAdminDashboard adminDashboard, IProvider provider, IWebHostEnvironment webHostEnvironment, INotyfService notyf)
        {
            _logger = logger;
            _physician = physician;
            _adminDashboard = adminDashboard;
            _provider = provider;
            _webHostEnvironment = webHostEnvironment;
            _notyf = notyf;
        }
        public IActionResult PhysicianDashboard()
        {
            int PhysicianId = 9;
            var count = _physician.CountRequest(PhysicianId);
            return View(count);
        }
        public IActionResult GetDashboard(string btnName, int statusId, int pagesize = 5, int page = 1)
        {
            int PhysicianId = 9;
            string partialView = "_" + btnName;
            var req = _physician.GetRequestData(statusId, PhysicianId, page, pagesize);
            return PartialView(partialView, req);
        }
        [HttpPost]
        public IActionResult AcceptCase(int RequestId)
        {
            int PhysicianId = 9;
            bool result = _physician.AcceptCase(RequestId, PhysicianId);
            if (result == true)
            {
                _notyf.Success("Case Accpted..");
            }
            return RedirectToAction("PhysicianDashboard");
        }
        [HttpPost]
        public IActionResult SendAgreement(string email, int RequestId)
        {

            bool result = _adminDashboard.SendAgreementEmail(email, RequestId);
            if (result == true)
            {
                _notyf.Success("Agreement is sent to the patient successfully..");
            }
            return RedirectToAction("PhysicianDashboard");
        }
        [HttpPost]
        public IActionResult TransferCase(int RequestId, string Notes)
        {

            bool result = _physician.TransferCase(RequestId, Notes);
            if (result == true)
            {
                _notyf.Success("Case Transferred to the Admin Successfully...");
            }
            return RedirectToAction("PhysicianDashboard");
        }
        public IActionResult ConcludeCare(int RequestId)
        {
            return View();
        }
        public IActionResult ViewCase(int requestId)
        {
            var model = _adminDashboard.GetView(requestId);
            return View(model);
        }
        public IActionResult ViewNotes(int RequestId)
        {
            var model = _adminDashboard.viewNotesData(RequestId);
            return View(model);
        }
        public IActionResult ViewUpload(int RequestId)
        {
            var files = _adminDashboard.GetFiles(RequestId);
            return View(files);
        }
        public IActionResult Order(int RequestId)
        {
            ViewBag.Profession = _adminDashboard.GetProfession();
            var order = _adminDashboard.GetOrderView(RequestId);
            return View(order);
        }
        [HttpPost]
        public IActionResult Order(Order order)
        {
            _adminDashboard.SendOrder(order);
            return RedirectToAction("PhysicianDashboard");
        }
        public IActionResult Housecall(int RequestId)
        {
            if (_physician.Housecall(RequestId))
            {
                _notyf.Success("Case Accepted...");
            }
            else
            {
                _notyf.Error("Case Not Accepted...");
            }
            return Redirect("~/Physician/PhysicianDashboard");
        }
        public IActionResult Consult(int RequestId)
        {
            if (_physician.Consult(RequestId))
            {
                _notyf.Success("Case is in conclude state...");
            }
            else
            {
                _notyf.Error("Error...");
            }
            return Redirect("~/Physician/PhysicianDashboard");
        }
        public IActionResult PhysicianProfile()
        {
            int ProviderId = 9;
            var region = _provider.GetRegions();
            ViewBag.Roles = _provider.GetRoles();
            var model = _provider.PhysicianAccount(ProviderId);
            model.Regions = region.Select(r => new RegionModel { RegionId = r.RegionId, Name = r.Name }).ToList();
            return View("PhysicianProfile", model);
        }
        public IActionResult CreateRequest()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateRequest(PatientReq req)
        {
            int PhysicianId = 9;
            _physician.CreateReq(req, PhysicianId);
            return RedirectToAction("PhysicianDashboard");

        }
        [HttpPost]
        public IActionResult SendLink(string email, string FirstName, string LastName)
        {
            bool sendEmail = _adminDashboard.SendLink(email, FirstName, LastName);
            if (sendEmail == true)
            {
                _notyf.Success("Request Page is sent to the patient successfully..");
            }
            return RedirectToAction("PhysicianDashboard");
        }
    }
}
