using AspNetCoreHero.ToastNotification.Abstractions;
using Hallo_Doc.Entity.Data;
using Hallo_Doc.Entity.ViewModel;
using Hallo_Doc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hallo_Doc.Controllers
{
    public class AdminNavbarController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAdminNavbar _adminNav;
        private readonly INotyfService _notyf;
        private readonly ILogger<AdminNavbarController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AdminNavbarController(ILogger<AdminNavbarController> logger, IAdminNavbar adminNav, INotyfService notyf, IWebHostEnvironment webHostEnvironment, ApplicationDbContext context)
        {
            _logger = logger;
            _adminNav = adminNav;
            _notyf = notyf;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }
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
                _adminNav.SendMailPhy(Email, Message, ProviderName);
            }
            return RedirectToAction("ProviderMenu");
        }
        public IActionResult AccountAccess()
        {
            var model = _adminNav.Access();
            return View(model);
        }
        public IActionResult UserAccess()
        {
            var model = _adminNav.UserAccess();
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
        public IActionResult CreateShift(int RegionId, int PhysicianId, DateOnly ShiftDate, TimeOnly StartTime, TimeOnly EndTime)
        {
            _adminNav.CreateShift(RegionId, PhysicianId, ShiftDate, StartTime, EndTime);
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
        public IActionResult RequestedShift()
        {
            var model = _adminNav.RequestedShift();
            return View(model);
        }
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
        public IActionResult BlockHistory(BlockHistory history)
        {
            var model = _adminNav.BlockedHistory(history);
            return View(model);
        }
        public IActionResult ProviderLocation()
        {
            return View();
        }
    }
}
