using AspNetCoreHero.ToastNotification.Abstractions;
using Hallo_Doc.Entity.Data;
using Hallo_Doc.Entity.ViewModel;
using Hallo_Doc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.DotNet.Scaffolding.Shared;
using Microsoft.EntityFrameworkCore;
using Hallo_Doc.Repository.Repository.Implementation;
using Hallo_Doc.Entity.Models;

namespace Hallo_Doc.Controllers
{
    public class AdminNavbarController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAdminNavbar _adminNav;
        private readonly INotyfService _notyf;
        private readonly IAdminDashboard _adminDashboard;
        private readonly IPhysician _physician;
        private readonly ILogger<AdminNavbarController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AdminNavbarController(ILogger<AdminNavbarController> logger, IAdminNavbar adminNav, INotyfService notyf, IAdminDashboard adminDashboard, IPhysician physician, IWebHostEnvironment webHostEnvironment, ApplicationDbContext context)
        {
            _logger = logger;
            _adminNav = adminNav;
            _notyf = notyf;
            _adminDashboard = adminDashboard;
            _physician = physician;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }
        [CustomAuthorize("Admin", "12")]
        public IActionResult ProviderMenu(int? RegionId = null)
        {
            ViewBag.Region = _adminNav.GetRegions();
            var modal = _adminNav.ProviderMenu(RegionId);
            return View(modal);
        }
        public IActionResult StopNotification(int ProviderId)
        {
            _adminNav.StopNotfy(ProviderId);
            return RedirectToAction("ProviderMenu");
        }
        [HttpPost]
        public IActionResult ContactProvider(string radiobtn, string Email, string Message, string ProviderName, string Mobile)
        {

            if (radiobtn == "email" || radiobtn == "both")
            {
                bool sent = _adminNav.SendMailPhy(Email, Message, ProviderName);
                if(sent == true)
                {
                    _notyf.Success("Email sent Successfully..");
                }
            }
            if (radiobtn == "sms" || radiobtn == "both")
            {
                bool sent = _adminNav.SendSMS(Mobile, Message, ProviderName);
                if(sent == true)
                {
                    _notyf.Success("SMS sent Successfully..");
                }
            }
            return RedirectToAction("ProviderMenu");
        }
        [CustomAuthorize("Admin", "4")]
        public IActionResult AccountAccess()
        {
            var model = _adminNav.Access();
            return View(model);
        }
        public IActionResult UserAccess(string AccountType)
        {
            ViewBag.AspNetRole = _adminNav.GetNetRoles();
            var model = _adminNav.UserAccess(AccountType);
            return View(model);
        }
        public IActionResult CreateAccess()
        {
            var model = _adminNav.CreateAccess();
            return View(model);
        }
        [HttpGet]
        public JsonResult GetMenuList(int accountType)
        {
            var menuList = _adminNav.GetMenuList((AccountType)accountType);
            return Json(menuList);
        }
        [HttpPost]
        public IActionResult CreateAccess(AccountAccess access)
        {
            _adminNav.CreateRole(access);
            return RedirectToAction("AccountAccess");
        }
        public IActionResult EditRole(int RoleId)
        {
            var model = _adminNav.ViewEditRole(RoleId);
            return View(model);
        }
        [HttpPost]
        public IActionResult EditRole(AccountAccess access)
        {
            bool succ = _adminNav.SaveEditRole(access);
            if(succ == true)
            {
                _notyf.Success("Role menu updated..");
            }
            return RedirectToAction("AccountAccess");
        }
        public IActionResult DeleteRole(int RoleId)
        {
            bool res = _adminNav.DeleteRole(RoleId);
            if (res == true)
            {
                _notyf.Success("Role Successfully Deleted..");
            }
            return RedirectToAction("AccountAccess");
        }
        [CustomAuthorize("Admin", "2")]
        public IActionResult Scheduling()
        {
            ViewBag.Physician = _adminNav.AllPhysician();
            ViewBag.Region = _adminNav.GetRegions();
            var model = _adminNav.Schedule();
            return View(model);
        }
        public IActionResult PhysicianCalender(int? RegionId)
        {
            var physicians = _adminNav.PhysicianCalender(RegionId);
            return Json(physicians);
        }
        [HttpPost]
        public IActionResult CreateShift(Schedule schedule)
        {
            var check = Request.Form["repeatDay"].ToList();
            _adminNav.CreateShift(schedule);
            return RedirectToAction("Scheduling");
        }
        public IActionResult EventShift()
        {
            var shift = _adminNav.ShiftList();

            return Json(shift);
        }
        public IActionResult ViewShift(int ShiftId)
        {
            var view = _adminNav.GetShiftDetails(ShiftId);
            return Json(view);
        }
        [HttpPost]
        public IActionResult EditShift(int ShiftId, int RegionId, int PhysicianId, DateOnly ShiftDate, TimeOnly StartTime, TimeOnly EndTime)
        {
            _adminNav.EditShift(ShiftId, RegionId, PhysicianId, ShiftDate, StartTime, EndTime);
            return RedirectToAction("Scheduling");
        }
        [HttpPost]
        public IActionResult DeleteShift(int ShiftId)
        {
            _adminNav.DeleteShift(ShiftId);
            return RedirectToAction("Scheduling");
        }
        [HttpPost]
        public IActionResult ChangeStatus(int ShiftId)
        {
            _adminNav.ChangeStatus(ShiftId);
            return RedirectToAction("Scheduling");
        }
        [HttpPost]
        public async Task<IActionResult> CheckShift(int PhysicianId, DateTime ShiftDate, TimeOnly StartTime)
        {
            bool exist = await _context.ShiftDetails.AnyAsync(s => s.Shift.PhysicianId == PhysicianId && s.ShiftDate.Date == ShiftDate.Date && s.StartTime == StartTime);
            if (!exist)
            {
                return Json(new { isAvailable = true });
            }
            return Json(new {isAvailable = false});
        }
        public IActionResult MDsOnCall()
        {
            var model = _adminNav.MDsOnCall();
            return View(model);
        }
        [HttpGet("images/{fileName}")]
        public IActionResult GetImage(string fileName)
        {
            var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Documents", fileName);
            if (System.IO.File.Exists(filePath))
            {
                var fileStream = System.IO.File.OpenRead(filePath);
                return File(fileStream, "image/jpeg");
            }
            else
            {
                return NotFound();
            }
        }
        public IActionResult RequestedShift(int? regionId)
        {
            ViewBag.Region = _adminNav.GetRegions();
            var model = _adminNav.RequestedShift(regionId);
            return View(model);
        }
        public async Task<IActionResult> _ApprovedShifts(string shiftids)
        {
            if (await _adminNav.UpdateStatusShift(shiftids))
            {
                _notyf.Success("Shifts Approved Successfully..");
            }
            return RedirectToAction("RequestedShift");
        }

        public async Task<IActionResult> _DeleteShifts(string shiftids)
        {
            if (await _adminNav.DeleteReqShift(shiftids))
            {
                _notyf.Success("Shifts Deleted Successfully..");
            }
            return RedirectToAction("RequestedShift");
        }
        [CustomAuthorize("Admin", "15")]
        public IActionResult VendorMenu(string searchValue, int Profession)
        {
            ViewBag.Professions = _context.HealthProfessionalTypes.ToList();
            var model = _adminNav.VendorMenu(searchValue, Profession);
            return View(model);
        }
        public IActionResult AddBusiness()
        {
            ViewBag.Professions = _context.HealthProfessionalTypes.ToList();
            var model = _adminNav.AddBusiness();
            return View(model);
        }
        [HttpPost]
        public IActionResult AddBusiness(VendorMenu model)
        {
            _adminNav.AddVendor(model);
            return RedirectToAction("VendorMenu");
        }
        public IActionResult EditBusiness(int VendorId)
        {
            ViewBag.Professions = _context.HealthProfessionalTypes.ToList();
            var model = _adminNav.EditVendor(VendorId);
            return View(model);
        }
        [HttpPost]
        public IActionResult EditBusiness(int VendorId,VendorMenu model)
        {
            _adminNav.EditVendorInfo(VendorId, model);
            return RedirectToAction("EditBusiness", new { VendorId = model.VendorId });
        }
        public IActionResult DeleteBusiness(int VendorId)
        {
            bool res = _adminNav.DeleteBusiness(VendorId);
            if (res == true)
            {
                _notyf.Success("Business Successfully Deleted..");
            }
            return RedirectToAction("VendorMenu");
        }
        [CustomAuthorize("Admin", "29")]
        public IActionResult BlockHistory(BlockHistory history)
        {
            var model = _adminNav.BlockedHistory(history);
            return View(model);
        }
        public IActionResult Unblock(int reqId)
        {
            bool res = _adminNav.UnBlock(reqId);
            if (res == true)
            {
                _notyf.Success("Patient is Unblocked");
            }
            return RedirectToAction("BlockHistory");
        }
        [CustomAuthorize("Admin", "23")]
        public IActionResult ProviderLocation()
        {
           
            var model = _adminNav.ProviderLocation();
            return View(model);
        }
        [CustomAuthorize("Admin", "3")]
        public IActionResult PatientHistory(UserData userData)
        {
            var model = _adminNav.PatientHistory(userData);
            return View(model);
        }
        [CustomAuthorize("Admin", "28")]
        public IActionResult PatientRecord(int UserId)
        {
            var model = _adminNav.PatientRecord(UserId);
            return View(model);
        }
        public IActionResult SearchRecords(SearchRecordList search)
        {
            var model = _adminNav.SearchRecord(search);
            return View(model);
        }
        public IActionResult DeleteRecord(int RequestId)
        {
            bool res = _adminNav.RecordsDelete(RequestId);
            if (res == true)
            {
                _notyf.Success("Patient Record Deleted successfully");
            }
            return RedirectToAction("SearchRecords");
        }
        [CustomAuthorize("Admin", "18")]
        public IActionResult EmailLogs(Logs logs)
        {
            var model = _adminNav.EmailLog(logs);
            return View(model);
        }
        [CustomAuthorize("Admin", "32")]
        public IActionResult SMSLogs(SMSLog logs)
        {
            var model = _adminNav.SMSLog(logs);
            return View(model);
        }
        public IActionResult CreateAdmin()
        {
            var region = _adminNav.GetRegions();
            var model = _adminNav.CreateAdmin();
            model.Regions = region.Select(r => new RegionList { RegionId = r.RegionId, Name = r.Name }).ToList();
            return View(model);
        }
        [HttpPost]
        public IActionResult CreateAdmin(AdminProfile admin)
        {
            _adminNav.AddAdmin(admin);
            _notyf.Success("Admin Account Created Successfully..");
            return RedirectToAction("UserAccess");
        }
        public IActionResult DownloadEncounter(int RequestId)
        {
            try
            {
                if (RequestId == 0 || RequestId < 0)
                {
                    throw new Exception("Invalid Request");
                }
                Encounter model = _adminDashboard.EncounterInfo(RequestId);
                if (model == null) throw new Exception("Medical Report Not Exist For this Request");
                using (var ms = new MemoryStream())
                {
                    var writer = new PdfWriter(ms)
    ;
                    var pdf = new PdfDocument(writer);
                    var document = new Document(pdf);

                    var title = new Paragraph("Medical Report")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(20);
                    document.Add(title);
                  
                    var table = new iText.Layout.Element.Table(new float[] { 4, 6 });
                    table.SetWidth(UnitValue.CreatePercentValue(100));

                    table.AddHeaderCell("Property");
                    table.AddHeaderCell("Value");

                    table.AddCell("RequestId");
                    table.AddCell(model.RequestId.ToString());
                    table.AddCell("FirstName");
                    table.AddCell(model.FirstName);
                    table.AddCell("LastName");
                    table.AddCell(model.LastName);
                    table.AddCell("Location");
                    table.AddCell(model.Address ?? "");
                    table.AddCell("DateOfBirth");
                    table.AddCell(model.DOB.ToString());
                    table.AddCell("DateOfService");
                    table.AddCell(model.DOS.ToString());
                    table.AddCell("Mobile");
                    table.AddCell(model.Mobile);
                    table.AddCell("Email");
                    table.AddCell(model.Email);
                    table.AddCell("HistoryOfPresentIllness");
                    table.AddCell(model.Injury ?? "");
                    table.AddCell("MedicalHistory");
                    table.AddCell(model.History ?? "");
                    table.AddCell("Medication");
                    table.AddCell(model.Medications ?? "");
                    table.AddCell("Allergies");
                    table.AddCell(model.Allergies ?? "");
                    table.AddCell("Temprature");
                    table.AddCell(model.Temp ?? "");
                    table.AddCell("HeartRate");
                    table.AddCell(model.HR ?? "");
                    table.AddCell("RespiratoryRate");
                    table.AddCell(model.RR ?? "");
                    table.AddCell("BloodPressureDiastolic");
                    table.AddCell(model.Bpd ?? "");
                    table.AddCell("BloodPressureSystolic");
                    table.AddCell(model.Bp ?? "");
                    table.AddCell("O2Level");
                    table.AddCell(model.O2 ?? "");
                    table.AddCell("Pain");
                    table.AddCell(model.Pain ?? "");
                    table.AddCell("HEENT");
                    table.AddCell(model.Heent ?? "");
                    table.AddCell("CvReading");
                    table.AddCell(model.CV ?? "");
                    table.AddCell("Chest");
                    table.AddCell(model.Chest ?? "");
                    table.AddCell("ABD");
                    table.AddCell(model.ABD ?? "");
                    table.AddCell("Extr");
                    table.AddCell(model.Extr ?? "");
                    table.AddCell("Skin");
                    table.AddCell(model.Skin ?? "");
                    table.AddCell("Neuro");
                    table.AddCell(model.Neuro ?? "");
                    table.AddCell("Other");
                    table.AddCell(model.Other ?? "");
                    table.AddCell("Diagnosis");
                    table.AddCell(model.Diagnosis ?? "");
                    table.AddCell("TreatmentPlan");
                    table.AddCell(model.Treatment ?? "");
                    table.AddCell("MedicationDispensed");
                    table.AddCell(model.MDispensed ?? "");
                    table.AddCell("Procedures");
                    table.AddCell(model.Procedures ?? "");
                    table.AddCell("FollowUp");
                    table.AddCell(model.Followup ?? "");
                    document.Add(table);

                    document.Close();

                    byte[] pdfBytes = ms.ToArray();
                    string filename = "Medical-Report-" + RequestId + DateTime.Now.ToString("_dd-MM-yyyy-hh-mm-ss-fff") + ".pdf";
                    return File(pdfBytes, "application/pdf", filename);
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = ex.Message })
                {
                    StatusCode = 500
                };
            }
        }
        public IActionResult Invoice(string startDate, string endDate, int PhysicianId)
        {
            ViewBag.Physician = _adminNav.AllPhysician();

            ShowTimeSheet model;
            if (startDate != null && endDate != null)
            {
                DateOnly sd = DateOnly.ParseExact(startDate, "dd/MM/yyyy");
                DateOnly ed = DateOnly.ParseExact(endDate, "dd/MM/yyyy");
                model = _adminNav.GetBiWeeklySheet(sd, ed, PhysicianId);

                if (model == null)
                {
                    model = new ShowTimeSheet { Weeklysheet = new List<ShowTimeSheet>() };
                }
            }
            else
            {
                model = new ShowTimeSheet { Weeklysheet = new List<ShowTimeSheet>() };
            }

            return View(model);
        }
        public IActionResult ApproveSheet(string startDate, string endDate, int PhysicianId)
        {
            DateOnly sd = DateOnly.ParseExact(startDate, "dd/MM/yyyy");
            DateOnly ed = DateOnly.ParseExact(endDate, "dd/MM/yyyy");
            var result = _adminNav.TimeSheet(sd, ed, PhysicianId);
            return View(result);
        }
        [HttpPost]
        public IActionResult TimeSheetPost(TimesheetData sendInfo)
        {
            var res = _adminNav.TimeSheetSave(sendInfo);
            return Json(res);
        }
        [HttpPost]
        public IActionResult ApproveTimeSheet(DateOnly StartDate, DateOnly EndDate, int PhysicianId, string Bonus, string Discription)
        {
            var result = _adminNav.ApproveSheet(StartDate, EndDate, PhysicianId, Bonus, Discription);
            return Json(result);
        }
    }
}
