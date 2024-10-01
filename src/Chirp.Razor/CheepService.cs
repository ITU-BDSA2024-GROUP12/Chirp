using Chirp.SQLite;
using CheepViewModel;

public record CheepViewModel(string Author, string Message, string Timestamp);

public interface ICheepService
{
    public List<CheepViewModel.CheepViewModel> GetCheeps();
    public List<CheepViewModel.CheepViewModel> GetCheepsFromAuthor(string author);
}

public class CheepService : ICheepService
{
    private readonly IDatabaseRepository<CheepViewModel.CheepViewModel> _DatabaseRepository =
        DBFacade<CheepViewModel.CheepViewModel>.getInstance();
    
    private int NO_OF_CHEEPS_ON_PAGE = 32;
    
    
    // These would normally be loaded from a database for example
    private static readonly List<CheepViewModel.CheepViewModel> _cheeps = new()
        {
            new CheepViewModel.CheepViewModel("Helge", "Hello, BDSA students!", UnixTimeStampToDateTimeString(1690892208)),
            new CheepViewModel.CheepViewModel("Adrian", "Hej, velkommen til kurset.", UnixTimeStampToDateTimeString(1690895308)),
        };

    public List<CheepViewModel.CheepViewModel> GetCheeps()
    {
        
        int indexStart = 0;
        int indexEnd = page * NO_OF_CHEEPS_ON_PAGE;

        if (page > 1)
        {
            indexStart = indexEnd - NO_OF_CHEEPS_ON_PAGE;
        }
        
        if (indexEnd > _cheeps.Count)
        {
            indexEnd = _cheeps.Count;
        }
        return _DatabaseRepository.Read().ToList();
        //DatabaseRepository.Read();
        return _cheeps.GetRange(indexStart,indexEnd);
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author)
    {
        // filter by the provided author name
        return _DatabaseRepository.ReadFromAuthor(author).ToList();
        return _cheeps.Where(x => x.Author == author).ToList();
    }

    private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }
}

