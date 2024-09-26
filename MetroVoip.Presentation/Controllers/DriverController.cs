using MetroVoip.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MetroVoip.Presentation.Controllers
{
    public class DriverController : Controller
    {
        private readonly IDriverCommunicationService _driverCommunicationService;

        public DriverController(IDriverCommunicationService communicationService)
        {
            _driverCommunicationService = communicationService;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult StartSpeaking(string ip, int rPort, int sPort)
        {
            _driverCommunicationService.StartSpeakingWithPassenger(ip, rPort, sPort);
            return Ok();
        }
        [HttpPost]
        public IActionResult StopSpeaking(string ip)
        {
            _driverCommunicationService.StopSpeakingWithPassenger(ip);
            return Ok();
        }

        [HttpPost]
        public IActionResult StartListening(string ip, int rPort, int sPort)
        {
            _driverCommunicationService.StartListeningWithPassenger(ip, rPort, sPort);
            return Ok();
        }
        [HttpPost]
        public IActionResult StopListening(string ip)
        {
            _driverCommunicationService.StopListeningWithPassenger(ip);
            return Ok();
        }
    }
}
