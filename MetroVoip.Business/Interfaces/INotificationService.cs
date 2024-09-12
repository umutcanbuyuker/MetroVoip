namespace MetroVoip.Business.Interfaces
{
    public interface INotificationService
    {
         Task SendNotification(int carriageNumber);
    }
}
