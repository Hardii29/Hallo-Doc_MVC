using AspNetCoreHero.ToastNotification.Abstractions;
using Hallo_Doc.Entity.ViewModel;
using Hallo_Doc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Hallo_Doc.Controllers
{
    public class PhysicianController : Controller
    {
        private readonly IPhysician _physician;
        private readonly IAdminDashboard _adminDashboard;
        private readonly ILogger<PhysicianController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly INotyfService _notyf;
        public PhysicianController(ILogger<PhysicianController> logger, IPhysician physician, IAdminDashboard adminDashboard, IWebHostEnvironment webHostEnvironment, INotyfService notyf)
        {
            _logger = logger;
            _physician = physician;
            _adminDashboard = adminDashboard;
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
    }
}
