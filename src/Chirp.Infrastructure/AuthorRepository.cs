using Chirp.Core;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure;

public class AuthorRepository : IAuthorRepository
{
    private readonly CheepDbContext _cheepDbContext;
    
    public AuthorRepository(CheepDbContext cheepDbContext)
    {
        _cheepDbContext = cheepDbContext;
        
        Console.WriteLine("Created Cheep Repository: " + this.GetType().Name);
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
    
    public async void AnonymizeUser(string name, string email)
    {
        AuthorDTO a = await GetAuthor(name, name);
        var r = _cheepDbContext.Users.Where(user => user.Email == email).ExecuteDeleteAsync().Result;
        var r2 = _cheepDbContext.Cheeps.Where(cheep => cheep.AuthorId == a.AuthorId)
            .ExecuteUpdateAsync(set => set.SetProperty(c => c.AuthorId, 0)).Result;
        var r3 = _cheepDbContext.Authors.Where(author => author.AuthorId == a.AuthorId).ExecuteDeleteAsync().Result;
        
        
        
        /*
        .ExecuteUpdateAsync(setter => setter.SetProperty(e => e.Email, "TEST@SOMETEST.TEST")
            .SetProperty(e => e.NormalizedEmail, "TEST@SOMETEST.TEST")
            .SetProperty(n => n.UserName, "")
            .SetProperty(n => n.NormalizedUserName, "")).Result;*/
        _cheepDbContext.SaveChanges();
        Console.WriteLine(r);
    }

    /// <summary>
    /// Method checks a list of strings for possible matches in the names of the author table
    /// </summary>
    /// <param name="mentions"> The list of names to be validated</param>
    /// <returns>a list of corresponding authors</returns>
    public async Task<List<AuthorDTO>> GetValidUsernames(List<string> mentions) {
        var query = _cheepDbContext.Authors.Where(Author => mentions.Contains(Author.Name)).Select(Author => new AuthorDTO
        {
            AuthorId = Author.AuthorId,
            Name = Author.Name,
            Email = Author.Email,
        });
        var result = await query.ToListAsync();
        return result;
    }
}