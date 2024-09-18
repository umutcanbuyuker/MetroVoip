using MetroVoip.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MetroVoip.Presentation.Controllers
{
    public class PassengerController : Controller
    {
        private readonly IPassengerCommunicationService _communicationService;

        public PassengerController(IPassengerCommunicationService communicationService)
        {
            _communicationService = communicationService;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> StartSpeakingWithDriver()
        {
            await _communicationService.StartSpeakingWithDriver();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> StopSpeakingWithDriver()
        {
            await _communicationService.StopSpeakingWithDriver();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> StartListeningDriver()
        {
            await _communicationService.StartListeningDriver();
            return Ok();
        }
    }
}
