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
            ViewBag.Roles = _provider.GetRoles();
            var model = _provider.CreateProvider();
            return View("~/Views/Admin/CreateProvider.cshtml", model);
        }
    }
}
