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
    
    public void CreateMessage(/*CheepDTO cheep*/)
    {
        //TODO: CREATE CHEEPDTO
        throw new NotImplementedException();
    /*
        Message newMessage = new() { Text = message.Text, ... };
        var queryResult = await _dbContext.Messages.AddAsync(newMessage); // does not write to the database!

        await _dbContext.SaveChangesAsync(); // persist the changes in the database
        return queryResult.Entity.CheepId;
        */
    }

    public async Task<List<CheepDTO>> ReadMessage()
    {
        var query = _cheepDbContext.Cheeps.Select(cheep => new CheepDTO
        {
            Author = cheep.Author.Name,
            Text = cheep.Text,
            Timestamp = cheep.TimeStamp
        });
        var result = await query.ToListAsync();

        return result;
    }

    public async Task<List<CheepDTO>> ReadMessagesFromAuthor(string author)
    {
        var query = _cheepDbContext.Cheeps.Where(cheep => cheep.Author.Name == author).Select(cheep => new CheepDTO
        {
            Author = cheep.Author.Name,
            Text = cheep.Text,
            Timestamp = cheep.TimeStamp
        });
        var result = await query.ToListAsync();

        return result;
    }

    public void UpdateMessage()
    {
        throw new NotImplementedException();
    }
}