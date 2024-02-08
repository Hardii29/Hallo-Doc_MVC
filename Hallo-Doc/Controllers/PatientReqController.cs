using Microsoft.AspNetCore.Mvc;
using Hallo_Doc.Data;
using Hallo_Doc.Models;
using Hallo_Doc.Models.ViewModel;
using System.Collections;

namespace Hallo_Doc.Controllers
{
    public class PatientReqController : Controller
    {
        private readonly ApplicationDbContext _context;
        public PatientReqController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(PatientReq PatientReq)
        {
            var Aspnetuser = new AspnetUser();
            var User = new User();
            var Request = new Request();
            //var Requestclient = new RequestClient();

            //if (ModelState.IsValid)
            //{
            Aspnetuser.Id = "gju";
            Aspnetuser.Username = PatientReq.FirstName;
            Aspnetuser.Passwordhash = "hardi";
            Aspnetuser.Email = PatientReq.Email;
            Aspnetuser.Phonenumber = PatientReq.Mobile;
            Aspnetuser.Emailconfirmed = new BitArray(1);
            Aspnetuser.Phonenumberconfirmed = new BitArray(1);
            Aspnetuser.Twofactorenabled = new BitArray(1);
            _context.AspnetUsers.Add(Aspnetuser);
            await _context.SaveChangesAsync();

            User.Aspnetuserid = PatientReq.Id;
            User.Firstname = PatientReq.FirstName;
            User.Lastname = PatientReq.LastName;
            User.Email = PatientReq.Email;
            User.Mobile = PatientReq.Mobile;
            User.Street = PatientReq.Street;
            User.City = PatientReq.City;
            User.State = PatientReq.State;
            User.Zipcode = PatientReq.ZipCode;
            User.Createdby = "123";
            User.Createddate = DateTime.Now;
            _context.Users.Add(User);
            await _context.SaveChangesAsync();

            Request.RequestTypeId = 2;
            Request.UserId = User.Userid;
            Request.FirstName = PatientReq.FirstName;
            Request.LastName = PatientReq.LastName;
            Request.Email = PatientReq.Email;
            Request.PhoneNumber = PatientReq.Mobile;
            Request.Status = 4;
            _context.Requests.Add(Request);
            await _context.SaveChangesAsync();

            //Requestclient.RequestId = Request.RequestId;
            //Requestclient.FirstName = viewPatientReq.FirstName;
            //Requestclient.LastName = viewPatientReq.LastName;
            //Requestclient.Address = viewPatientReq.Street;
            //Requestclient.Email = viewPatientReq.Email;
            //Requestclient.PhoneNumber = viewPatientReq.Mobile;
            //_context.RequestClients.Add(Requestclient);
            //await _context.SaveChangesAsync();

            return View("../Home/Index");
            //return RedirectToAction("FamilyReq", "PatientForm");
            //}
            //return RedirectToAction("Index", "Home");
        }
        public IActionResult Index()
        {
            return View("../Patient/Create_patient_req");
        }

    }
}
