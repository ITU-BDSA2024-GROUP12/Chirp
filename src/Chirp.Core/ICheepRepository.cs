namespace Chirp.Core;

public interface ICheepRepository
{
    public Task<List<CheepDTO>> GetMessages(int page);
    public void UpdateMessage();
    public Task<List<CheepDTO>> GetMessagesFromAuthor(string author, int page);
    public bool CreateCheep(AuthorDTO author, string text, List<AuthorDTO>? mentions, string time);
    Task<CheepDTO> GetCheepById(int id);
    public Task<int> CheepCount();
    public Task<int> CheepCountFromAuthor(AuthorDTO author);
}