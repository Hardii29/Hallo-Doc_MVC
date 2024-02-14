using Hallo_Doc.Data;
using Hallo_Doc.Models.ViewModel;
using Hallo_Doc.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hallo_Doc.Controllers
{
    public class FamilyReqController : Controller
    {
        private readonly ApplicationDbContext _context;
        public FamilyReqController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View("../Patient/Create_family_req");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(FamilyReq familyReq)
        {
          
            var Request = new Request();
            var Requestclient = new Requestclient();
            var RequestType = new RequestType();

            //if (ModelState.IsValid)
            //{
            

            RequestType.Name = "Family";
            _context.RequestTypes.Add(RequestType);
            await _context.SaveChangesAsync();

            Request.RequestTypeId = 3;
            Request.FirstName = familyReq.f_firstname;
            Request.LastName = familyReq.f_lastname;
            Request.Email = familyReq.f_email;
            Request.PhoneNumber = familyReq.f_mobile;
            Request.Status = 1;
            Request.RelationName = familyReq.f_relation;
            Request.CreatedDate = DateTime.Now;
            _context.Requests.Add(Request);
            await _context.SaveChangesAsync();

            Requestclient.RequestId = Request.RequestId;
            Requestclient.FirstName = familyReq.FirstName;
            Requestclient.LastName = familyReq.LastName;
            Requestclient.Address = familyReq.Street;
            Requestclient.Email = familyReq.Email;
            Requestclient.PhoneNumber = familyReq.Mobile;
            _context.Requestclients.Add(Requestclient);
            await _context.SaveChangesAsync();

            //return View("../Home/Index");
            ////return RedirectToAction("FamilyReq", "PatientForm");
            ////}
            return RedirectToAction("Index", "Home");
        }


    }
}

    

