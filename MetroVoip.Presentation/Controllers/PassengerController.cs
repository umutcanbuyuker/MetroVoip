using MetroVoip.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MetroVoip.Presentation.Controllers
{
    public class PassengerController : Controller
    {
        private readonly IPassengerCommunicationService _passengerCommunicationService;

        public PassengerController(IPassengerCommunicationService passengerCommunicationService)
        {
            _passengerCommunicationService = passengerCommunicationService;
        }

        public IActionResult Index()
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            ListenerInfo listenerInfo = new() { IpAddress = ipAddress, ListenPort = 5000, SpeakPort = 5000 ,RequestIpAddress = HttpContext.Connection.LocalIpAddress.ToString() };
            return View(listenerInfo);
        }


        [HttpPost]
        public IActionResult NotifyDriver(int carriageNumber)
        {
            return Ok();
        }

        [HttpPost]
        public IActionResult StartSpeaking(string ip, int speakPort, int listenPort)
        {
            _passengerCommunicationService.StartSpeakingWithDriver(ip, speakPort, listenPort);
            return Ok();
        }
        [HttpPost]
        public IActionResult StopSpeaking(string ip)
        {
            _passengerCommunicationService.StopSpeakingWithDriver(ip);
            return Ok();
        }

        [HttpPost]
        public IActionResult StartListening(string ip, int speakPort, int listenPort)
        {
            _passengerCommunicationService.StartListeningWithDriver(ip, speakPort, listenPort);
            return Ok();
        }
        [HttpPost]
        public IActionResult StopListening(string ip)
        {
            _passengerCommunicationService.StopListeningWithDriver(ip);
            return Ok();
        }
    }
}
