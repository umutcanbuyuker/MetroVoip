using MetroVoip.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MetroVoip.Presentation.Controllers
{
    public class DriverController : Controller
    {
        private readonly IIntercomService _intercomService;
        public DriverController(IIntercomService intercomService)
        {
            _intercomService = intercomService;
        }
        public async Task<IActionResult> Index()
        {
            var intercoms = await _intercomService.GetAllIntercomsAsync();
            return View(intercoms);
        }
        public async Task<IActionResult> SelectIntercom(int carriageNumber)
        {
            await _intercomService.StartCommunicationAsync(carriageNumber);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> StopIntercom(int carriageNumber)
        {
            await _intercomService.StopCommunicationAsync(carriageNumber);
            return RedirectToAction("Index");
        }
    }
}
