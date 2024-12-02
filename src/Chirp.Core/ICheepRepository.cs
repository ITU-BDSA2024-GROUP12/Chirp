namespace Chirp.Core;

public interface ICheepRepository
{
    public Task<List<CheepDTO>> GetMessages(int page);
    public void UpdateMessage();
    public Task<List<CheepDTO>> GetMessagesFromAuthor(string author, int page);
    public bool CreateCheep(AuthorDTO author, string text, List<AuthorDTO>? mentions, string time);
    public Task<List<NotificationDTO>> GetNotifications(string username);
    public Task DeleteNotification(int notificationId);
    public Task ForgetMentions(string authorName);
    public Task DeleteNotificationsForUser(int userId);
    Task<CheepDTO> GetCheepById(int id);
}