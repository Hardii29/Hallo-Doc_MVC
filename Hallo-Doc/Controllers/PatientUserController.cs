using Hallo_Doc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hallo_Doc.Entity.ViewModel;
using Hallo_Doc.Repository.Repository.Implementation;

namespace Hallo_Doc.Controllers
{
    [CustomAuthorize("Patient")]
    public class PatientUserController : Controller
    {
        private readonly IPatientUser _user;
        private readonly ILogger<PatientUserController> _logger;
        private readonly IAdminDashboard _adminDashboard;
        public PatientUserController(ILogger<PatientUserController> logger, IPatientUser user, IAdminDashboard adminDashboard)
        {
            _logger = logger;
            _user = user;
            _adminDashboard = adminDashboard;
        }


        public IActionResult Patient_dashboard()
        {
            var model = _user.PatientDashboard(HttpContext);
            return View(model);
        }
        public IActionResult View_document(int RequestId)
        {
            var model = _user.ViewDocument(RequestId, HttpContext);
            return View(model);
        }
        [HttpPost]
        public IActionResult View_document(int RequestId, ViewDocument viewDocument)
        {
            _adminDashboard.UploadFiles(RequestId, viewDocument);
            return RedirectToAction("View_document", new { RequestId = RequestId });
        }
        [HttpGet]
        public IActionResult Download_file(int fileID)
        {
            return _user.DownloadFile(fileID);
        }
        public IActionResult Patient_profile()
        {
            var userId = (int)HttpContext.Session.GetInt32("UserId");
            var model = _user.Edit(userId, HttpContext);
            return View(model);
        }
        [HttpPost]
        public IActionResult Patient_profile(int userId, [Bind("FirstName", "LastName", "DOB", "Mobile", "Email", "Street", "City", "State", "Zipcode")]PatientProfile patientProfile)
        {
            _user.EditProfile(userId, patientProfile);
            return RedirectToAction("Patient_profile", new { userId = userId });
        }
        public IActionResult Submit_req_Me()
        {
            return View();
        }
        public IActionResult Submit_req_Someone()
        {
            return View();
        }
    }
}
