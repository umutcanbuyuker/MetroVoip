using Microsoft.AspNetCore.Mvc;

namespace MetroVoip.Presentation.Controllers
{
    public class PassengerController : Controller
    {

        public IActionResult Index()
        {
            var ipAddress = HttpContext.Connection.LocalIpAddress.ToString();

            ListenerInfo listenerInfo = new() { IpAddress = ipAddress, rPort = 5000, sPort = 5000 };
            return View(listenerInfo);
        }


        [HttpPost]
        public IActionResult NotifyDriver(int carriageNumber)
        {
            return Ok();
        }
    }
}
