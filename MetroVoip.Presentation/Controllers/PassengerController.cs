using MetroVoip.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MetroVoip.Presentation.Controllers
{
    public class PassengerController : Controller
    {
        public PassengerController()
        {
        }

        [HttpPost]
        public IActionResult NotifyDriver(int carriageNumber)
        {
            return Ok();
        }
    }
}
