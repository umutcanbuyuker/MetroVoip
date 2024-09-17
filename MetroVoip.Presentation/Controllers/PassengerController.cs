using MetroVoip.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MetroVoip.Presentation.Controllers
{
    public class PassengerController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}
