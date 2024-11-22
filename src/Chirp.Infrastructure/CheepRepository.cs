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
    
/// <summary>
/// Creates a new cheep with the given text, and from the provided author name.
/// If the author does not exists in the db, it creates the author.
/// </summary>
/// <param name="authorDto">DTO of the Cheep Author</param>
/// <param name="text">Body of the cheep</param>
/// <param name="time">Timestamp "yyyy-mm-dd hh:mm:ss"</param>
/// <returns>True: if the cheep is created, otherwise false</returns>
    public bool CreateCheep(AuthorDTO authorDto, string text, string time)
    {
        if (text.Length > 160)
        {
            throw new InvalidDataException("Cheep too long!");
        }
        try
        {
            CreateAuthor(authorDto);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        
        

        AuthorDTO author = GetAuthor(authorDto.Name,authorDto.Email).Result;
        Console.WriteLine("Creating cheep for: "+author.Name + ":" + author.Email + ":" + author.AuthorId + " With text: \n"+text);

        Cheep newCheep = new Cheep()
        {
            Text = text,
            AuthorId = author.AuthorId,
            TimeStamp = DateTime.Parse(time),
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
    public bool CreateAuthor(AuthorDTO author)
    {
        if (DoesAuthorExist(author.Name))
        {
            throw new Exception($"Author {author.Name} already exists");
        }

        Console.WriteLine("Creating Author: " + author.Name);
        Author auth = new Author
        {
            Name = author.Name,
            Email = author.Email
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
        return query.Any();
    }

    /// <summary>
    /// Returns a author with the given email from the db.
    /// </summary>
    /// <param name="email">E-Mail of author</param>
    /// <returns>Author</returns>
    public async Task<AuthorDTO> GetAuthorByEmail(string email)
    {
        var query = _cheepDbContext.Authors.Where(x => x.Email == email).Select(author => new AuthorDTO
        {
            AuthorId = author.AuthorId,
            Name = author.Name,
            Email = author.Email,
        });
        
        //There should only be one author returned, so return the first one.
        AuthorDTO author = await query.FirstAsync();
        
        if (author == null)
        {
            throw new UserNotFoundException();
        }

        return author;
    }    
    /// <summary>
    /// Returns a author with the given name from the db.
    /// </summary>
    /// <param name="name">Name of author</param>
    /// <returns>Author</returns>
    public async Task<AuthorDTO> GetAuthorByName(string name)
    {
        var query = _cheepDbContext.Authors.Where(x => x.Name == name).Select(author => new AuthorDTO
        {
            AuthorId = author.AuthorId,
            Name = author.Name,
            Email = author.Email,
        });
        
        //There should only be one author returned, so return the first one.
        AuthorDTO author = await query.FirstAsync();
        
        if (author == null)
        {
            throw new UserNotFoundException();
        }

        return author;
    }


/// <summary>
/// Returns a author with the given name and email from the db.
/// </summary>
/// <param name="name">Name of author</param>
/// <param name="email">E-mail of author</param>
/// <returns>Author</returns>
    public async Task<AuthorDTO> GetAuthor(string name, string email)
    {
        if (!DoesAuthorExist(name))
        {
            throw new UserNotFoundException();
        }
        
        var query = _cheepDbContext.Authors.Where(x => x.Name == name && x.Email == email).Select(author => new AuthorDTO
        {
            AuthorId = author.AuthorId,
            Name = author.Name,
            Email = author.Email,
        });
        
        //There should only be one author returned, so return the first one.
        AuthorDTO author = await query.FirstAsync();
        
        return author;
    }

    public async Task<List<CheepDTO>> ReadMessage(int page)
    {
        var query = _cheepDbContext.Cheeps.Select(cheep => new CheepDTO
        {
            Author = cheep.Author.Name,
            Text = cheep.Text,
            TimeStamp = ((DateTimeOffset) cheep.TimeStamp).ToUnixTimeSeconds()
        }).AsEnumerable().OrderByDescending(x => x.TimeStamp).Skip((page - 1) * 32).Take(32);
        var result = query.ToList();
       
        return result;
    }

    public async Task<List<CheepDTO>> ReadMessagesFromAuthor(string author, int page)
    {
        var query = _cheepDbContext.Cheeps.Where(cheep => cheep.Author.Name == author).Select(cheep => new CheepDTO
        {
            Author = cheep.Author.Name,
            Text = cheep.Text,
            TimeStamp = ((DateTimeOffset) cheep.TimeStamp).ToUnixTimeSeconds()
        }).AsEnumerable().OrderByDescending(x => x.TimeStamp).Skip((page - 1) * 32).Take(32);
        var result = query.ToList();
       
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