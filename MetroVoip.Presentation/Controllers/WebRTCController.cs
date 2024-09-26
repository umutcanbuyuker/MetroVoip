using Microsoft.AspNetCore.Mvc;

namespace MetroVoip.Presentation.Controllers
{
    public class WebRTCController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
