using DataModel;

public interface ICheepRepository
{
    public void CreateMessage();
    public Task<List<CheepDTO>> ReadMessage();
    public void UpdateMessage();
    public Task<List<CheepDTO>> ReadMessagesFromAuthor(string author);
}