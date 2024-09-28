using Chirp.SQLite;
using CheepViewModel;


public interface ICheepService
{
    public List<CheepViewModel.CheepViewModel> GetCheeps();
    public List<CheepViewModel.CheepViewModel> GetCheepsFromAuthor(string author);
}

public class CheepService : ICheepService
{
    private readonly IDatabaseRepository<CheepViewModel.CheepViewModel> _DatabaseRepository;
    
    // These would normally be loaded from a database for example
    private static readonly List<CheepViewModel.CheepViewModel> _cheeps = new()
        {
            new CheepViewModel.CheepViewModel("Helge", "Hello, BDSA students!", UnixTimeStampToDateTimeString(1690892208)),
            new CheepViewModel.CheepViewModel("Adrian", "Hej, velkommen til kurset.", UnixTimeStampToDateTimeString(1690895308)),
        };

    public CheepService(IDatabaseRepository<CheepViewModel.CheepViewModel> databaseRepository)
    {
        this._DatabaseRepository = databaseRepository;
    }

    public List<CheepViewModel.CheepViewModel> GetCheeps()
    {
        return _DatabaseRepository.Read().ToList();
        return _cheeps;
    }

    public List<CheepViewModel.CheepViewModel> GetCheepsFromAuthor(string author)
    {
        // filter by the provided author name
        Console.WriteLine("GetCheepsFromAuthor " + author);
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

