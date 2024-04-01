using Hallo_Doc.Entity.ViewModel;
using Hallo_Doc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Hallo_Doc.Controllers
{
    public class ProviderController : Controller
    {
        private readonly IProvider _provider;
        private readonly ILogger<ProviderController> _logger;
        public ProviderController(ILogger<ProviderController> logger, IProvider provider)
        {
            _logger = logger;
            _provider = provider;
        }
        public IActionResult CreateProvider()
        {
            var region = _provider.GetRegions();
            ViewBag.Roles = _provider.GetRoles();
            var model = _provider.CreateProvider();
            model.Regions = region.Select(r=> new RegionModel { RegionId = r.RegionId, Name = r.Name}).ToList();
            return View("~/Views/Admin/CreateProvider.cshtml", model);
        }
        [HttpPost]
        public IActionResult CreateProvider(Provider model)
        {
           
            _provider.AddProvider(model);
            return RedirectToAction("ProviderMenu", "Admin");
        }
        public IActionResult EditPhysician(int ProviderId)
        {
            var region = _provider.GetRegions();
            ViewBag.Roles = _provider.GetRoles();
            var model = _provider.PhysicianAccount(ProviderId);
            model.Regions = region.Select(r => new RegionModel { RegionId = r.RegionId, Name = r.Name }).ToList();
            return View("~/Views/Admin/EditPhysician.cshtml", model);
        }
        [HttpPost]
        public IActionResult EditPrAccount(Provider model)
        {
            _provider.EditPrAccount(model);
            return RedirectToAction("EditPhysician", "Provider", new { ProviderId = model.ProviderId });
        }
        [HttpPost]
        public IActionResult EditPrInfo(Provider model)
        {
            _provider.EditPrInfo(model);
            return RedirectToAction("EditPhysician", "Provider", new { ProviderId = model.ProviderId });
        }
        [HttpPost]
        public IActionResult EditPrBilling(Provider model)
        {
            _provider.EditPrBilling(model);
            return RedirectToAction("EditPhysician", "Provider", new { ProviderId = model.ProviderId });
        }
        [HttpPost]
        public IActionResult EditPrbusiness(Provider model)
        {
            _provider.EditPrbusiness(model);
            return RedirectToAction("EditPhysician", "Provider", new { ProviderId = model.ProviderId });
        }
        [HttpPost]
        public IActionResult EditOnbording(Provider model)
        {
            _provider.EditOnbording(model);
            return RedirectToAction("EditPhysician", "Provider", new { ProviderId = model.ProviderId });
        }
        [HttpPost]
        public IActionResult DeleteAccount(int ProviderId)
        {
            _provider.DeletePrAccount(ProviderId);
            return RedirectToAction("ProviderMenu", "Admin");
        }
    }
}
