using Chirp.Core;

namespace Chirp.Infrastructure;

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

    public void CreateAuthor(string name, string email)
    {
        if (doesAuthorExist(name))
        {
            Console.WriteLine("Author already exists");
            throw new Exception($"Author {name} already exists");
        }
        else
        {
            Console.WriteLine("Creating Author: " + name);
            Author auth = new Author();
            auth.Name = name;
            auth.Email = email;
            var query = _cheepDbContext.Authors.Add(auth);
            Task<int> tsk = _cheepDbContext.SaveChangesAsync();
            Console.WriteLine(tsk.Result);
        }
    }

    private Boolean doesAuthorExist(string name)
    {
        var query = _cheepDbContext.Authors.Where(x => x.Name == name);
        Console.WriteLine(query.Any());
        return query.Any();

    }

    public async Task<List<CheepDTO>> ReadMessage(int page)
    {
        var query = _cheepDbContext.Cheeps.Select(cheep => new CheepDTO
        {
            Author = cheep.Author.Name,
            Text = cheep.Text,
            TimeStamp = ((DateTimeOffset) cheep.TimeStamp).ToUnixTimeSeconds()
        }).Skip((page - 1) * 32).Take(32);
        var result = await query.ToListAsync();

        return result;
    }

    public async Task<List<CheepDTO>> ReadMessagesFromAuthor(string author, int page)
    {
        var query = _cheepDbContext.Cheeps.Where(cheep => cheep.Author.Name == author).Select(cheep => new CheepDTO
        {
            Author = cheep.Author.Name,
            Text = cheep.Text,
            TimeStamp = ((DateTimeOffset) cheep.TimeStamp).ToUnixTimeSeconds()
        }).Skip((page - 1) * 32).Take(32);
        var result = await query.ToListAsync();

        return result;
    }

    public void UpdateMessage()
    {
        throw new NotImplementedException();
    }
    
    private static DateTime ToDateTime(long unixTime)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        return dateTime.AddSeconds(unixTime);
    }
}