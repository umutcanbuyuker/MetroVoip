using MetroVoip.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MetroVoip.Presentation.Controllers
{
    public class DriverController : Controller
    {
        private readonly IDriverCommunicationService _communicationService;

        public DriverController(IDriverCommunicationService communicationService)
        {
            _communicationService = communicationService;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> StartSpeakingWithPassenger(int kabinId)
        {
            await _communicationService.StartSpeakingWithPassenger(kabinId);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> StopSpeakingWithPassenger(int kabinId)
        {
            await _communicationService.StopSpeakingWithPassenger(kabinId);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> StartListeningPassenger(int kabinId) 
        {
            await _communicationService.StartListeningPassenger(kabinId);
            return Ok();
        }
    }
}
