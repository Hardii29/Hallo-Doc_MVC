using Hallo_Doc.Data;
using Hallo_Doc.Models;
using Hallo_Doc.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Hallo_Doc.Controllers
{
    public class ConciergeReqController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ConciergeReqController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View("../Patient/Create_concierge_req");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(ConciergeReq ConciergeReq)
        {

            var Request = new Request();
            var Requestclient = new Requestclient();
            var RequestType = new RequestType();
            var Concierge = new Concierge();
            var RequestConcierge = new RequestConcierge();

            //if (ModelState.IsValid)
            //{


            RequestType.Name = "Concierge";
            _context.RequestTypes.Add(RequestType);
            await _context.SaveChangesAsync();

            Request.RequestTypeId = 4;
            Request.FirstName = ConciergeReq.c_firstname;
            Request.LastName = ConciergeReq.c_lastname;
            Request.Email = ConciergeReq.c_email;
            Request.PhoneNumber = ConciergeReq.c_mobile;
            Request.Status = 1;
            Request.CreatedDate = DateTime.Now;
            _context.Requests.Add(Request);
            await _context.SaveChangesAsync();

            Concierge.ConciergeName = ConciergeReq.c_firstname;
            Concierge.Address = ConciergeReq.c_address;
            Concierge.Street = ConciergeReq.c_street;
            Concierge.City = ConciergeReq.c_city;
            Concierge.State = ConciergeReq.c_state;
            Concierge.ZipCode = ConciergeReq.c_zip;
            Concierge.CreatedDate = DateTime.Now;
            _context.Concierges.Add(Concierge);
            await _context.SaveChangesAsync();

            RequestConcierge.RequestId = Request.RequestId;
            RequestConcierge.ConciergeId = Concierge.ConciergeId;
            _context.RequestConcierges.Add(RequestConcierge);
            await _context.SaveChangesAsync();

            Requestclient.RequestId = Request.RequestId;
            Requestclient.FirstName = ConciergeReq.FirstName;
            Requestclient.LastName = ConciergeReq.LastName;
            Requestclient.Address = ConciergeReq.Street;
            Requestclient.Email = ConciergeReq.Email;
            Requestclient.PhoneNumber = ConciergeReq.Mobile;
            _context.Requestclients.Add(Requestclient);
            await _context.SaveChangesAsync();

            //return View("../Home/Index");
            ////return RedirectToAction("FamilyReq", "PatientForm");
            ////}
            return RedirectToAction("Index", "Home");
        }


    }
}