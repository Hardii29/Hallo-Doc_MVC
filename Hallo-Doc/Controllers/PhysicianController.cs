using Microsoft.AspNetCore.Mvc;

namespace Hallo_Doc.Controllers
{
    public class PhysicianController : Controller
    {
        public IActionResult PhysicianDashboard()
        {
            return View();
        }
    }
}
