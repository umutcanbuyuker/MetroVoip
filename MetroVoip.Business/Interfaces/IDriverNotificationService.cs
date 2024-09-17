namespace MetroVoip.Business.Interfaces
{
    public interface IDriverNotificationService 
    {
        Task NotifyDriverAsync(int cabinId);
    }
}
