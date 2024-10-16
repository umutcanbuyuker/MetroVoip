using MetroVoip.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MetroVoip.Presentation.Controllers
{
    public class IntercomController : Controller
    {
        private readonly ICommendIntercomService _intercomService;
        public IntercomController(ICommendIntercomService intercomService)
        {
            _intercomService = intercomService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AcceptCall()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AcceptCall(string ipAddress, string username, string password)
        {
            await _intercomService.AcceptIncomingCall(ipAddress, username, password);
            ViewBag.Message = "Arama kabul edildi.";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CancelCall(string ipAddress, string username, string password)
        {
            await _intercomService.CancelCall(ipAddress, username, password);
            ViewBag.Message = "Arama iptal edildi.";
            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeclineCall(string ipAddress, string username, string password)
        {
            await _intercomService.DeclineCall(ipAddress, username, password);
            ViewBag.Message = "Arama reddedildi.";
            return View("Index");
        }
    }
}
