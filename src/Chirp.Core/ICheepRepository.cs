namespace Chirp.Core;

public interface ICheepRepository
{
    public Task<List<CheepDTO>> GetMessages(int page);
    public void UpdateMessage();
    public Task<List<CheepDTO>> GetMessagesFromAuthor(string author, int page);
    public bool CreateAuthor(AuthorDTO author);
    public bool CreateCheep(AuthorDTO author, string text, string time);
    public Task<AuthorDTO> GetAuthorByEmail(string email);
    public Task<AuthorDTO> GetAuthorByName(string name);
    public Task<AuthorDTO> GetAuthor(string name, string email);
    public Task<List<AuthorDTO>> GetValidUsernames(List<string> mentions);
    //public Task<NotificationDTO> CreateNotification(int authorId, int cheepId);

}