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
        public IActionResult StartSpeaking(string ip, int rPort, int sPort)
        {
            _communicationService.StartSpeaking(ip, rPort, sPort);
            return Ok();
        }
        [HttpPost]
        public IActionResult StopSpeaking(string ip)
        {
            _communicationService.StopSpeaking(ip);
            return Ok();
        }
    }
}
