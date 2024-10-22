﻿using Chirp.Core;

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
    
/// <summary>
/// Creates a new cheep with the given text, and from the provided author name.
/// If the author does not exists in the db, it creates the author.
/// </summary>
/// <param name="name">Name of the author</param>
/// <param name="email">E-mail of the author</param>
/// <param name="text">Body of the cheep</param>
/// <param name="time">Timestamp "yyyy-mm-dd hh:mm:ss"</param>
/// <returns>True: if the cheep is created, otherwise false</returns>
    public bool CreateCheep(string name, string email, string text, string time)
    {
        try
        {
            CreateAuthor(name,email);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        Author author = GetAuthor(name, email).Result;
        Console.WriteLine("Creating cheep for: "+author.Name + ":" + author.Email + ":" + author.AuthorId + " With text: \n"+text);

        Cheep newCheep = new Cheep()
        {
            Text = text,
            AuthorId = author.AuthorId,
            TimeStamp = DateTime.Parse(time),
            Author = author
    };
        var query = _cheepDbContext.Cheeps.Add(newCheep);
        Task<int> tsk = _cheepDbContext.SaveChangesAsync();

        return (tsk.Result == 1);
    }
/// <summary>
/// Creates a new author with the given name and email
/// </summary>
/// <param name="name">Name of the author</param>
/// <param name="email">E-mail of the author</param>
/// <returns>True: if the author is created successfully</returns>
/// <exception cref="Exception">Throws if the author already exists</exception>
    public bool CreateAuthor(string name, string email)
    {
        if (DoesAuthorExist(name))
        {
            throw new Exception($"Author {name} already exists");
        }

        Console.WriteLine("Creating Author: " + name);
        Author auth = new Author
        {
            Name = name,
            Email = email
        };
        
        _cheepDbContext.Authors.Add(auth);
        Task<int> tsk = _cheepDbContext.SaveChangesAsync();
        return (tsk.Result == 1);
    }
/// <summary>
/// Check if the author with the name exists in the database
/// </summary>
/// <param name="name">Name of the author</param>
/// <returns>
///     True: if the author exists in the db
///     False: if the author does not exist in the db
/// </returns>
    private Boolean DoesAuthorExist(string name)
    {
        var query = _cheepDbContext.Authors.Where(x => x.Name == name);
        if (query.Any())
        {
            return true;
        }
        return false;
    }
/// <summary>
/// Returns a author with the given name and email from the db.
/// </summary>
/// <param name="name">Name of author</param>
/// <param name="email">E-mail of author</param>
/// <returns>Author</returns>
    private async Task<Author> GetAuthor(string name, string email)
    {
        var query = _cheepDbContext.Authors.Where(x => x.Name == name && x.Email == email).Select(author => new Author
        {
            AuthorId = author.AuthorId,
            Name = author.Name,
            Email = author.Email,
        });
        
        //There should only be one author returned, so return the first one.
        Author author = await query.FirstAsync();

        return author;
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