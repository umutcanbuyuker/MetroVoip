using MetroVoip.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MetroVoip.Presentation.Controllers
{
    public class IntercomController : Controller
    {
        private readonly ICommendIntercomService _intercomService;
        public IntercomController(ICommendIntercomService intercomService)
        {
            _intercomService = intercomService;
        }
        [HttpPost]
        public async Task<IActionResult> StopCall()
        {
            try
            {
                await _intercomService.StopCallAsync();
                return Json(new { success = true, message = "Görüşme sonlandırıldı." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Hata: {ex.Message}" });
            }
        }
    }
}
