using System.Diagnostics;
using Hallo_Doc.Entity.ViewModel;
using Hallo_Doc.Entity.Models;
using Hallo_Doc.Entity.Data;
using Hallo_Doc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Hallo_Doc.Repository.Repository.Implementation;
using NuGet.Protocol.Plugins;
using OfficeOpenXml;
using AspNetCoreHero.ToastNotification.Abstractions;
using Twilio.Http;

namespace Hallo_Doc.Controllers
{
    //[CustomAuthorize("Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminDashboard _adminDashboard;
        private readonly ILogger<AdminController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly INotyfService _notyf;
        public AdminController(ILogger<AdminController> logger, IAdminDashboard adminDashboard, IWebHostEnvironment webHostEnvironment, INotyfService notyf)
        {
            _logger = logger;
            _adminDashboard = adminDashboard;
            _webHostEnvironment = webHostEnvironment;
            _notyf = notyf;
        }
        [CustomAuthorize("Admin", "6")]
        public IActionResult Admin_dashboard()
        {
            ViewBag.CaseReason = _adminDashboard.GetReasons();
            ViewBag.Region = _adminDashboard.GetRegions();
            var count = _adminDashboard.CountRequestData();
            return View(count);
        }
        [CustomAuthorize("Admin", "6")]
        public IActionResult GetPartialView(string btnName, int statusId, string searchValue, string sortColumn, string sortOrder, int pagesize = 5, int requesttype = -1, int Region = -1, int page = 1)
        {
            string partialView = "_" + btnName + "Table";
            var request = _adminDashboard.GetRequestData(statusId, searchValue, page, pagesize, Region, sortColumn, sortOrder, requesttype);
            return PartialView(partialView, request);
        }
        
        public IActionResult View_case(int requestId)
        {
            var model = _adminDashboard.GetView(requestId);
            return View(model);
        }
        [HttpPost]
        public IActionResult View_case(int requestId, [Bind("FirstName", "LastName", "Mobile", "Email", "Address")]ViewCase viewCase)
        {
            _adminDashboard.UpdateViewCase(requestId, viewCase);
            return RedirectToAction("View_case", new {requestId = requestId});
        }
        public IActionResult CancelViewCase(int RequestId)
        {
            _adminDashboard.CancelViewCase(RequestId);
            return RedirectToAction("Admin_dashboard");
        }
        
        public IActionResult View_notes(int RequestId)
        {
            var model = _adminDashboard.viewNotesData(RequestId);
            return View(model);
        }
        [HttpPost]
        public IActionResult EditNote(string? AdminNotes, int RequestId)
        {
            bool model = _adminDashboard.ViewNotes(AdminNotes, RequestId);
            if(model==false)
            {
                return NotFound();
            }
            return RedirectToAction("View_notes", new {RequestId = RequestId});
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
            return RedirectToAction("View_upload", new { RequestId = RequestId });
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
        
        [HttpPost]
        public IActionResult SendAgreement(string email, int RequestId)
        {
            
            bool result = _adminDashboard.SendAgreementEmail(email, RequestId);
            if (result == true)
            {
                _notyf.Success("Agreement is sent to the patient successfully..");
            }
            return RedirectToAction("Admin_dashboard");
        }
        [CustomAuthorize("Admin", "17")]
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
        [HttpPost]
        public IActionResult Close_case(int RequestId, [Bind("Mobile", "Email")] ViewCase viewCase)
        {
            _adminDashboard.UpdateCloseCase(RequestId, viewCase);
            return RedirectToAction("Close_case", new { requestId = RequestId });
        }
        public IActionResult CloseCaseInfo(int RequestId)
        {
            _adminDashboard.CloseCaseReq(RequestId);
            return RedirectToAction("Admin_dashboard");
        }
        public IActionResult Encounter(int requestId)
        {
            var model = _adminDashboard.EncounterInfo(requestId);
            return View(model);
        }
        [HttpPost]
        public IActionResult Encounter(Encounter encounter)
        {
            _adminDashboard.EditEncounterinfo(encounter);
            return RedirectToAction("Encounter", new { requestId = encounter.RequestId });
        }
        [CustomAuthorize("Admin", "5")]
        public IActionResult AdminProfile(int adminId)
        {
            var data = _adminDashboard.Profile(adminId);
            return View(data);
        }
        [HttpPost]
        public IActionResult AdminProfile(int adminId, [Bind("FirstName", "LastName", "Mobile", "Email", "Address1", "ZipCode", "City")] AdminProfile profile)
        {
            _adminDashboard.EditProfile(adminId, profile);
            return RedirectToAction("AdminProfile", new { adminId = adminId });
        }
       public IActionResult CreateRequest()
        {
            var data = _adminDashboard.Admin();
            return View(data);
        }
        [HttpPost]
        public IActionResult CreateRequest(PatientReq req)
        {
            _adminDashboard.CreateReq(req);
            return RedirectToAction("Admin_dashboard");

        }
        public IActionResult Export(string status, int Region = -1, int requesttype = -1)
        {
            var requestData = _adminDashboard.Export(status, Region, requesttype);

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("RequestData");

                worksheet.Cells[1, 1].Value = "Name";
                worksheet.Cells[1, 2].Value = "Requestor";
                worksheet.Cells[1, 3].Value = "Request Date";
                worksheet.Cells[1, 4].Value = "Phone";
                worksheet.Cells[1, 5].Value = "Address";
                worksheet.Cells[1, 6].Value = "Notes";
                worksheet.Cells[1, 7].Value = "Physician";
                worksheet.Cells[1, 8].Value = "Birth Date";
                worksheet.Cells[1, 9].Value = "RequestTypeId";
                worksheet.Cells[1, 10].Value = "Email";
                worksheet.Cells[1, 11].Value = "RequestId";

                for (int i = 0; i < requestData.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = requestData[i].PatientName;
                    worksheet.Cells[i + 2, 2].Value = requestData[i].Requestor;
                    worksheet.Cells[i + 2, 3].Value = requestData[i].RequestedDate;
                    worksheet.Cells[i + 2, 4].Value = requestData[i].PatientMobile;
                    worksheet.Cells[i + 2, 5].Value = requestData[i].Address;
                    worksheet.Cells[i + 2, 6].Value = requestData[i].Notes;
                    worksheet.Cells[i + 2, 7].Value = requestData[i].ProviderName;
                    worksheet.Cells[i + 2, 8].Value = requestData[i].DOB;
                    worksheet.Cells[i + 2, 9].Value = requestData[i].RequestTypeId;
                    worksheet.Cells[i + 2, 10].Value = requestData[i].Email;
                    worksheet.Cells[i + 2, 11].Value = requestData[i].RequestId;
                }

                byte[] excelBytes = package.GetAsByteArray();

                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
        }
        [HttpPost]
        public IActionResult SendLink(string email, string FirstName, string LastName)
        {
            bool sendEmail = _adminDashboard.SendLink(email, FirstName, LastName);
            if (sendEmail == true)
            {
                _notyf.Success("Request Page is sent to the patient successfully..");
            }
            return RedirectToAction("Admin_dashboard");
        }
        public IActionResult GetChatView(string Id)
        {
            var model = _adminDashboard.ChatInfo(Id);
            return PartialView("_ChatPage", model);
        }
        public IActionResult GetChatHistory(string Sender, string Reciever)
        {
            var chat = _adminDashboard.ChatHistory(Sender, Reciever);
            return Json(chat);
        }
    }
}
