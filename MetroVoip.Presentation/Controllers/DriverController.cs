using MetroVoip.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MetroVoip.Presentation.Controllers
{
    public class DriverController : Controller
    {
        private readonly IIntercomService _intercomService;
        private readonly ICommunicationService _communicationService;

        public DriverController(IIntercomService intercomService, ICommunicationService communicationService)
        {
            _intercomService = intercomService;
            _communicationService = communicationService;
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

        [HttpPost]
        public IActionResult StartSpeaking(int kabinId)
        {
            _communicationService.StartSpeaking(kabinId);
            return Ok();
        }

        [HttpPost]
        public IActionResult StopSpeaking(int kabinId)
        {
            _communicationService.StopSpeaking(kabinId);
            return Ok();
        }
    }
}
