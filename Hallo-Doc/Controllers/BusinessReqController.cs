using Hallo_Doc.Data;
using Hallo_Doc.Models.ViewModel;
using Hallo_Doc.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hallo_Doc.Controllers
{
    public class BusinessReqController : Controller
    {
        private readonly ApplicationDbContext _context;
        public BusinessReqController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View("../Patient/Create_business_req");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(Models.ViewModel.BusinessReq BusinessReq)
        {

            var Request = new Models.Request();
            var Requestclient = new Models.Requestclient();
            var RequestType = new Models.RequestType();

            //if (ModelState.IsValid)
            //{


            RequestType.Name = "Business";
            _context.RequestTypes.Add(RequestType);
            await _context.SaveChangesAsync();

            Request.RequestTypeId = 1;
            Request.FirstName = BusinessReq.b_firstname;
            Request.LastName = BusinessReq.b_lastname;
            Request.Email = BusinessReq.b_email;
            Request.PhoneNumber = BusinessReq.b_mobile;
            Request.Status = 1;
            Request.CreatedDate = DateTime.Now;
            _context.Requests.Add(Request);
            await _context.SaveChangesAsync();

            Requestclient.RequestId = Request.RequestId;
            Requestclient.FirstName = BusinessReq.FirstName;
            Requestclient.LastName = BusinessReq.LastName;
            Requestclient.Address = BusinessReq.Street;
            Requestclient.Email = BusinessReq.Email;
            Requestclient.PhoneNumber = BusinessReq.Mobile;
            _context.Requestclients.Add(Requestclient);
            await _context.SaveChangesAsync();

            //return View("../Home/Index");
            ////return RedirectToAction("FamilyReq", "PatientForm");
            ////}
            return RedirectToAction("Index", "Home");
        }


    }
}
