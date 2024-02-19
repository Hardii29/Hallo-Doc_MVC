//using Hallo_Doc.Entity.Data;
//using Hallo_Doc.Entity.ViewModel;
//using Hallo_Doc.Repository.Repository.Interface;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Hallo_Doc.Repository.Repository.Implementation
//{
//    public class PatientRepo : IPatient
//    {
//        private readonly ApplicationDbContext _context;

//        public PatientRepo(ApplicationDbContext context)
//        {
//            _context = context;
//        }
       
//        public async Task<string> Reset_password()
//        {
//            return "/Patient/Reset_password";
//        }
//        public async Task<string> Forgot_password()
//        {
//            return "/Patient/Forgot_password";
//        }
//        public async Task<string> Create_request()
//        {
//            return "/Patient/Create_request";
//        }
//        public async Task<string> Create_patient_req()
//        {
//            return "/Patient/Create_patient_req";
//        }
//        public async Task<string> Create_family_req()
//        {
//            return "/Patient/Create_family_req";
//        }
//        public async Task<string> Create_business_req()
//        {
//            return "/Patient/Create_business_req";
//        }
//        public async Task<string> Create_concierge_req()
//        {
//            return "/Patient/Create_concierge_req";
//        }
//        public async Task<IActionResult> Patient_dashboard()
//        {
//            var model = new DashboardList
//            {
//                Requests = _context.Requests.ToList()
//            };

//            return View(model);
//        }
//        public IActionResult View_document()
//        {
//            var files = new DashboardList
//            {
//                RequestWiseFiles = _context.RequestWiseFiles.ToList()
//            };

//            return View(files);
//        }
//        [HttpGet]
//        public IActionResult DownloadFile(int fileID)
//        {
//            var file = _context.RequestWiseFiles.FirstOrDefault(f => f.RequestWiseFileId == fileID);
//            if (file == null)
//            {
//                return NotFound();
//            }
//            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Documents", file.FileName);
//            if (!System.IO.File.Exists(filePath))
//            {
//                return NotFound();
//            }
//            return PhysicalFile(filePath, "application/octet-stream", file.FileName);
//        }
//        public IActionResult Patient_profile()
//        {
//            return View("Patient_profile");
//        }
//        public IActionResult Submit_req_Me()
//        {
//            return View("Submit_req_Me");
//        }
//        public IActionResult Submit_req_Someone()
//        {
//            return View("Submit_req_Someone");
//        }
//        public IActionResult Error()
//        {
//            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
//        }
//    }
//}
