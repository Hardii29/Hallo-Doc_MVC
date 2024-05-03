
using Hallo_Doc.Entity.Data;
using Hallo_Doc.Entity.Models;
using Hallo_Doc.Entity.ViewModel;
using System.Collections;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;
using Microsoft.CodeAnalysis.Scripting;
using BCrypt.Net;
using System.IO;
using Hallo_Doc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Hallo_Doc.Controllers
{
    public class PatientReqController : Controller
    {
        private readonly IPatientReq _request;
        private readonly ILogger<PatientReqController> _logger;
        public PatientReqController(ILogger<PatientReqController> logger, IPatientReq request)
        {
            _logger = logger;
            _request = request;
        }
        public IActionResult Create_request()
        {
            return View();
        }

        [HttpPost]
        public IActionResult checkEmail(string email)
        {
            var exists = _request.checkEmail(email);
            return Ok(exists);
        }
      
        public IActionResult Create_patient_req()
        {
            ViewBag.Region = _request.GetRegions();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
   
        public IActionResult Create_patient_req(PatientReq PatientReq)
        {
                _request.AddDetails(PatientReq);
                return RedirectToAction("Index", "Home");
        }
        public IActionResult Create_family_req()
        {
            ViewBag.Region = _request.GetRegions();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create_family_req(FamilyReq familyReq)
        {
            _request.FamilyDetails(familyReq);
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Create_concierge_req()
        {
            ViewBag.Region = _request.GetRegions();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create_concierge_req(ConciergeReq conciergeReq)
        {
            _request.ConciergeDetails(conciergeReq);
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Create_business_req()
        {
            ViewBag.Region = _request.GetRegions();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create_business_req(BusinessReq businessReq)
        {
            _request.BusinessDetails(businessReq);
            return RedirectToAction("Index", "Home");
        }
        
    }
}
   
   