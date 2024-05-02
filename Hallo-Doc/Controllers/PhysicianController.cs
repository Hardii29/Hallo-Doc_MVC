using AspNetCoreHero.ToastNotification.Abstractions;
using Hallo_Doc.Entity.ViewModel;
using Hallo_Doc.Repository.Repository.Implementation;
using Hallo_Doc.Repository.Repository.Interface;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Hallo_Doc.Controllers
{
    public class PhysicianController : Controller
    {
        private readonly IPhysician _physician;
        private readonly IAdminDashboard _adminDashboard;
        private readonly IProvider _provider;
        private readonly IAdminNavbar _adminNav;
        private readonly ILogger<PhysicianController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly INotyfService _notyf;
        public PhysicianController(ILogger<PhysicianController> logger, IPhysician physician, IAdminDashboard adminDashboard, IProvider provider, IAdminNavbar adminNav, IWebHostEnvironment webHostEnvironment, INotyfService notyf)
        {
            _logger = logger;
            _physician = physician;
            _adminDashboard = adminDashboard;
            _provider = provider;
            _adminNav = adminNav;
            _webHostEnvironment = webHostEnvironment;
            _notyf = notyf;
        }
        [CustomAuthorize("Physician", "7")]
        public IActionResult PhysicianDashboard()
        {
            int PhysicianId = 9;
            var count = _physician.CountRequest(PhysicianId);
            return View(count);
        }
        [CustomAuthorize("Physician", "7")]
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
            var files = _adminDashboard.GetFiles(RequestId);
            return View(files);
        }
        [HttpPost]
        public IActionResult ConcludeCareUpload(int RequestId, ViewDocument viewDocument)
        {
            _adminDashboard.UploadFiles(RequestId, viewDocument);
            return RedirectToAction("ConcludeCare", new { RequestId = RequestId });
        }
        public IActionResult ConcludeCareCase(int RequestId, string Notes)
        {
            if (_physician.ConcludeCare(RequestId, Notes))
            {
                _notyf.Success("Case concluded...");
            }
            else
            {
                _notyf.Error("Please finalize the encounter form first...");
            }
            return Redirect("~/Physician/PhysicianDashBoard");
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
        [CustomAuthorize("Physician", "14")]
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
        [CustomAuthorize("Physician", "10")]
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
        public IActionResult EncounterFinalize(int RequestId)
        {
            bool result = _physician.Finalizeform(RequestId);
            if (result == true)
            {
                _notyf.Success("Encounter Form Finalized successfully..");
            }
            return RedirectToAction("PhysicianDashboard", "Physician");
        }
        public IActionResult IsEncounterFinalized(int requestId)
        {
            
            var res = _physician.IsEncounterFinalized(requestId);
            return Json(res);
        }
        [HttpPost]
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
        [CustomAuthorize("Physician", "9")]
        public IActionResult Schedule()
        {
            ViewBag.Region = _adminNav.GetRegions();
            return View();
        }
        public IActionResult EventShift()
        {
            int PhysicianId = 9;
            var shift = _physician.ShiftList(PhysicianId);

            return Json(shift);
        }
        public IActionResult ViewShift(int ShiftId)
        {
            var view = _adminNav.GetShiftDetails(ShiftId);
            return Json(view);
        }
        [HttpPost]
        public IActionResult CreateShift(Schedule schedule)
        {
            schedule.PhysicianId = 9;
            _adminNav.CreateShift(schedule);
            return RedirectToAction("Schedule");
        }
        [HttpPost]
        public IActionResult EditShift(int ShiftId, int RegionId, DateOnly ShiftDate, TimeOnly StartTime, TimeOnly EndTime)
        {
            int PhysicianId = 9;
            _adminNav.EditShift(ShiftId, RegionId, PhysicianId, ShiftDate, StartTime, EndTime);
            return RedirectToAction("Schedule");
        }
        [HttpPost]
        public IActionResult DeleteShift(int ShiftId)
        {
            _adminNav.DeleteShift(ShiftId);
            return RedirectToAction("Schedule");
        }
        [HttpPost]
        public IActionResult RequestAdmin(string Message)
        {
            int PhysicianId = 9;
            bool res = _physician.RequestAdmin(PhysicianId, Message);
            if (res == true)
            {
                _notyf.Success("Request Sent To Admin");
            }
            else
            {
                _notyf.Warning("Mail is not sent");
            }
            return RedirectToAction("PhysicianProfile");
        }
        [HttpPost]
        public IActionResult EditNote(string? PhysicianNotes, int RequestId)
        {
            bool model = _adminDashboard.ViewNotes(PhysicianNotes, RequestId);
            if (model == false)
            {
                return NotFound();
            }
            return RedirectToAction("ViewNotes", new { RequestId = RequestId });
        }
        public IActionResult Invoicing()
        {
            return View();
        }
        public IActionResult BiWeeklySheet()
        { 
            return View();
        }
    }
}
