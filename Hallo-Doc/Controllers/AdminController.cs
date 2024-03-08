using System.Diagnostics;
using Hallo_Doc.Entity.ViewModel;
using Hallo_Doc.Entity.Models;
using Hallo_Doc.Entity.Data;
using Hallo_Doc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hallo_Doc.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminDashboard _adminDashboard;
        private readonly ILogger<AdminController> _logger;
        public AdminController(ILogger<AdminController> logger, IAdminDashboard adminDashboard)
        {
            _logger = logger;
            _adminDashboard = adminDashboard;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Admin_dashboard()
        {
            var caseTags = _adminDashboard.GetReasons();
            ViewBag.CaseReason = caseTags;
            
            var count = _adminDashboard.CountRequestData();
            return View(count);
        }
        
        public IActionResult GetPartialView(string btnName, int statusId)
        {
            string partialView = "_" + btnName + "Table";
            var request = _adminDashboard.GetRequestData(statusId);
            return PartialView(partialView, request);
        }
       
        public IActionResult View_case(int requestId)
        {
            var model = _adminDashboard.GetView(requestId);
            return View(model);
        }
        [HttpPost]
        public IActionResult View_case(int requestId, [Bind("FirstName", "LastName", "Mobile", "Email")]ViewCase viewCase)
        {
            _adminDashboard.UpdateViewCase(requestId, viewCase);
            return RedirectToAction("View_case", new {requestId = requestId});
        }
        public IActionResult View_notes()
        {
            return View();
        }
        public IActionResult PhysicianList(int regionId) 
        { 
            var physicians = _adminDashboard.GetPhysician(regionId);
            return Json(physicians);
        }
        public IActionResult CancelCase()
        {
            
            return PartialView("_CancelCase");
        }
        [HttpPost]
        public IActionResult CancelCase(int requestId, string caseTag, string Notes)
        {
            _adminDashboard.CancleCaseInfo(requestId, caseTag, Notes);
            return RedirectToAction("Admin_dashboard");
        }
        public IActionResult AssignCase()
        {
            var regionList = _adminDashboard.GetRegions();
            ViewBag.Regions = regionList;
            return PartialView("_AssignCase");
        }
        public IActionResult BlockCase()
        {
            return PartialView("_BlockCase");
        }
        [HttpPost]
        public IActionResult BlockCase(int RequestId, string Notes)
        {
            _adminDashboard.BlockCaseReq(RequestId, Notes);
            return RedirectToAction("Admin_dashboard");
        }
    }
}
