﻿using System.Diagnostics;
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
        public IActionResult Admin_dashboard(string search)
        {
            ViewBag.CaseReason = _adminDashboard.GetReasons();
            ViewBag.Region = _adminDashboard.GetRegions();
            var count = _adminDashboard.CountRequestData();
            return View(count);
        }
        
        public IActionResult GetPartialView(string btnName, int statusId, string searchValue)
        {
            string partialView = "_" + btnName + "Table";
            var request = _adminDashboard.GetRequestData(statusId, searchValue);
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
        public IActionResult PhysicianList(int RegionId) 
        { 
            var physicians = _adminDashboard.GetPhysician(RegionId);
            return Json(physicians);
        }
      
        [HttpPost]
        public IActionResult CancelCase(int requestId, int CaseTagId, string Notes)
        {
            _adminDashboard.CancleCaseInfo(requestId, CaseTagId, Notes);
            return RedirectToAction("Admin_dashboard");
        }
      
        [HttpPost]
        public IActionResult AssignCase(int RequestId, int PhysicianId, string Notes)
        {
            _adminDashboard.AssignCaseReq(RequestId, PhysicianId, Notes);
            return RedirectToAction("Admin_dashboard");
        }
        
        [HttpPost]
        public IActionResult BlockCase(int RequestId, string Notes)
        {
            _adminDashboard.BlockCaseReq(RequestId, Notes);
            return RedirectToAction("Admin_dashboard");
        }
        public IActionResult View_upload(int RequestId)
        {
            var files = _adminDashboard.GetFiles(RequestId);
            return View(files);
        }
        [HttpPost]
        public IActionResult View_upload(int RequestId, ViewDocument viewDocument)
        {
            _adminDashboard.UploadFiles(RequestId, viewDocument);
            return RedirectToAction("View_upload");
        }
        [HttpGet]
        public IActionResult? Download_file(int fileID)
        {
            return _adminDashboard.DownloadFile(fileID);
        }
        [HttpPost]
        public IActionResult Delete_file(int fileID)
        {
            _adminDashboard.DeleteFile(fileID);
            return RedirectToAction("View_upload");
        }
        [HttpPost]
        public IActionResult DeleteAll(int RequestId)
        {
            _adminDashboard.DeleteAllFiles(RequestId);
            return RedirectToAction("View_upload");
        }
       
        [HttpPost]
        public IActionResult TransferCase(int RequestId, int PhysicianId, string Notes)
        {
            _adminDashboard.TransferCaseReq(RequestId, PhysicianId, Notes);
            return RedirectToAction("Admin_dashboard");
        }
  
        [HttpPost]
        public IActionResult ClearCase(int RequestId)
        {
            _adminDashboard.ClearCaseReq(RequestId);
            return RedirectToAction("Admin_dashboard");
        }
        public IActionResult Agreement()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SendAgreement(string email)
        {
            
            _adminDashboard.SendAgreementEmail(email);
            return RedirectToAction("Admin_dashboard");
        }
        public IActionResult Order(int RequestId)
        {
            ViewBag.Profession = _adminDashboard.GetProfession();
            var order = _adminDashboard.GetOrderView(RequestId);
            return View(order);
        }
        public IActionResult BusinessList(int businessId)
        {
            var business = _adminDashboard.GetBusiness(businessId);
            return Json(business);
        }
        public IActionResult BusinessDetails(int VendorId)
        {
            var detail = _adminDashboard.GetBusinessDetails(VendorId);
            return Json(detail);
        }
        [HttpPost]
        public IActionResult Order(Order order)
        {
            _adminDashboard.SendOrder(order);
            return RedirectToAction("Admin_dashboard");
        }
        public IActionResult Close_case(int RequestId)
        {
            var model = _adminDashboard.GetClearCaseView(RequestId);
            return View(model);
        }
    }
}
