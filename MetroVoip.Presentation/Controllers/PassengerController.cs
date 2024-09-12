using MetroVoip.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MetroVoip.Presentation.Controllers
{
    public class PassengerController : Controller
    {
        private readonly INotificationService _notificationService;
        public PassengerController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpPost]
        public IActionResult NotifyDriver(int carriageNumber)
        {
            if (carriageNumber < 1 || carriageNumber > 4)
            {
                return BadRequest("Geçersiz vagon numarası.");
            }

            // Bildirimi işlemek için NotificationService kullanılır
            _notificationService.SendNotification(carriageNumber);

            return Ok(new { message = "Bildirim başarıyla gönderildi." });
        }

        public IActionResult Notify()
        {
            return View(); 
        }

    }
}
