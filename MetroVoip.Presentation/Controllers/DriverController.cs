using MetroVoip.Business.Interfaces;
using MetroVoip.Business.Services;
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
        public async Task<IActionResult> ConferenceCallUser()
        {
            return View();
        }
        public async Task<IActionResult> ConferenceCallManager()
        {
            return View();
        }
        public async Task<IActionResult> WebRtc()
        {
            return View();
        }
        public async Task<IActionResult> ConferenceCall()
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
