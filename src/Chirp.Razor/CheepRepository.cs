using DataModel;
using Microsoft.EntityFrameworkCore;

public class CheepRepository : ICheepRepository
{
    private readonly CheepDbContext _cheepDbContext;

    public CheepRepository(CheepDbContext cheepDbContext)
    {
        _cheepDbContext = cheepDbContext;
        Console.WriteLine("Created Cheep Repository: " + this.GetType().Name);
    }
    
    public void CreateMessage()
    {
        
    }

    public async void ReadMessage()
    {
        var q = _cheepDbContext.Cheeps.Select(message => new { message.Author, message.text });
        var result = await q.ToListAsync();
        
    }

    public void UpdateMessage()
    {
        
    }
}