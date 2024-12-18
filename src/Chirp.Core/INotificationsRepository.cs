namespace Chirp.Core;

public interface INotificationRepository
{
    public Task<List<NotificationDTO>> GetNotifications(string username);
    public Task DeleteNotification(int notificationId);
    public Task ForgetMentions(string authorName);
    public Task DeleteNotificationsForUser(int userId);
}