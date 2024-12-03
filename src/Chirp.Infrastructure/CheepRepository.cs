using Chirp.Core;
namespace Chirp.Infrastructure;
using Microsoft.EntityFrameworkCore;

public class CheepRepository : ICheepRepository
{
    private readonly CheepDbContext _cheepDbContext;
    private readonly IAuthorRepository _authorRepository;

    public CheepRepository(CheepDbContext cheepDbContext, IAuthorRepository authorRepository)
    {
        _cheepDbContext = cheepDbContext;
        _authorRepository = authorRepository;
        
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
    public bool CreateCheep(AuthorDTO authorDto, string text, List<AuthorDTO>? mentions, string time)
    {
        if (text.Length > 160)
        {
            throw new InvalidDataException("Cheep too long!");
        }
        try
        {
            _authorRepository.CreateAuthor(authorDto);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        
        

        AuthorDTO author = _authorRepository.GetAuthor(authorDto.Name,authorDto.Email).Result;
        Console.WriteLine("Creating cheep for: "+author.Name + ":" + author.Email + ":" + author.AuthorId + " With text: \n"+text);

        Cheep newCheep = new Cheep()
        {
            Text = text,
            AuthorId = author.AuthorId,
            TimeStamp = DateTime.Parse(time),
        };
        var query = _cheepDbContext.Cheeps.Add(newCheep);
        //add mentions and notifications if present
        if (mentions != null)
        {
            foreach (var mentioned in mentions)
            {
                CheepMention mention = new()
                {
                    MentionedUsername = mentioned.Name,
                    CheepId = newCheep.CheepId,
                    Cheep = newCheep,
                };
                 _cheepDbContext.CheepMentions.Add(mention);
                Notification notification = new()
                {
                    AuthorId = mentioned.AuthorId,
                    Cheep = newCheep,
                    CheepId = newCheep.CheepId,
                    Content = $"{author.Name} mentioned you in a Cheep!",
                    TimeStamp = DateTime.Parse(time),
                };
                _cheepDbContext.Notifications.Add(notification);
            }   
        }
        //save changes to database
        Task<int> tsk = _cheepDbContext.SaveChangesAsync();

        return tsk.Result == 1;
    }

    public bool FollowUser(AuthorDTO authorName, AuthorDTO userName)
    {
        AuthorDTO author = _authorRepository.GetAuthorByName(authorName.Name).Result;
        AuthorDTO user = _authorRepository.GetAuthor(userName.Name, userName.Email).Result;
        
        
        Task<int> tsk = _cheepDbContext.SaveChangesAsync();
        return tsk.Result == 1;
    }
    
    
    public async Task<List<CheepDTO>> GetMessages(int page)
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

    public async Task<List<CheepDTO>> GetMessagesFromAuthor(string author, int page)
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

    /// <summary>
    /// Takes in the known author name and finds the author id to get notifications.
    /// </summary>
    /// <param name="username">The author who's notifications to get</param>
    /// <returns>A list of notifications if present</returns>
    public async Task<List<NotificationDTO>> GetNotifications(string username)
    {
        // Fetch the AuthorId based on the provided username
        var author = await _cheepDbContext.Authors
        .FirstOrDefaultAsync(a => a.Name == username);

        if (author == null)
        {
            throw new ArgumentException("Invalid username: no author found.", nameof(username));
        }

        // Use the AuthorId to filter notifications
        var notifications = await _cheepDbContext.Notifications
            .Where(n => n.AuthorId == author.AuthorId) 
            .Select(n => new NotificationDTO
            {
                Id = n.Id,
                AuthorId = n.AuthorId,
                CheepId = n.CheepId,
                Content = n.Content,
                Timestamp = n.TimeStamp
            })
            .ToListAsync();

        return notifications ?? new List<NotificationDTO>();
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

    public async  Task<CheepDTO> GetCheepById(int id)
    {
        var cheep = await _cheepDbContext.Cheeps
        .Where(c => c.CheepId == id)
        .Select(c => new CheepDTO
        {
            Text = c.Text,
            Author = c.Author.Name,
            TimeStamp = ((DateTimeOffset)c.TimeStamp).ToUnixTimeSeconds()
        })
        .FirstOrDefaultAsync();

        if(cheep == null){
             throw new ArgumentException("Invalid cheepId: cheep " + id);
        }

        return cheep;
    }

    public async Task DeleteNotification(int notificationId)
    {
        var notification = await _cheepDbContext.Notifications
        .FirstOrDefaultAsync(n => n.Id == notificationId);
        if (notification != null)
        {
            _cheepDbContext.Notifications.Remove(notification);
            await _cheepDbContext.SaveChangesAsync();
        } 
    }
}