namespace Chirp.Core;

public interface ICheepRepository
{
    public Task<List<CheepDTO>> ReadMessage(int page);
    public void UpdateMessage();
    public Task<List<CheepDTO>> ReadMessagesFromAuthor(string author, int page);
    public bool CreateAuthor(string name, string email);
    public bool CreateCheep(string author, string email, string text, string time);
}