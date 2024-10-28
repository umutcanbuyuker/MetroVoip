using MetroVoip.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MetroVoip.Presentation.Controllers
{
    public class DriverController : Controller
    {
        public IDriverCommunicationService _service;
        public DriverController(IDriverCommunicationService service)
        {
            _service = service;
        }
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> StartCall()
        {
            await _service.StartSipCall();
            return Json(new { success = true, message = "Görüşme başlatıldı." });
        }

        [HttpPost]
        public IActionResult EndCall()
        {
            _service.EndSipCall();
            return Json(new { success = true, message = "Görüşme sonlandırıldı." });
        }
    }
}
