using MetroVoip.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MetroVoip.Presentation.Controllers
{
    public class DriverController : Controller
    {
        private readonly ICommunicationService _communicationService;

        public DriverController(ICommunicationService communicationService)
        {
            _communicationService = communicationService;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> StartSpeaking(int kabinId)
        {
            await _communicationService.StartSpeaking(kabinId);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> StopSpeaking(int kabinId)
        {
            await _communicationService.StopSpeaking(kabinId);
            return Ok();
        }
    }
}
