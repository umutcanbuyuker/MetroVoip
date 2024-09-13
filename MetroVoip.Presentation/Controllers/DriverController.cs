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
