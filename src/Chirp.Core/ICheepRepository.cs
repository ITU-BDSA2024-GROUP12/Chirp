namespace Chirp.Core;

public interface ICheepRepository
{
    public Task<List<CheepDTO>> GetMessages(int page);
    public void UpdateMessage();
    public Task<List<CheepDTO>> GetMessagesFromAuthor(string author, int page);
    public bool CreateCheep(AuthorDTO author, string text, List<AuthorDTO>? mentions, string time);
    public bool FollowUser(AuthorDTO authorName, AuthorDTO userName);
    public Task<List<NotificationDTO>> GetNotifications(string username);
    public Task DeleteNotification(int notificationId);

    Task<CheepDTO> GetCheepById(int id);
}