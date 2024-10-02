using Chirp.SQLite;
using CheepViewModel;

//public record CheepViewModel(string Author, string Message, string Timestamp);

public interface ICheepService
{
    public List<CheepViewModel.CheepViewModel> GetCheeps(int pageNumber);
    public List<CheepViewModel.CheepViewModel> GetCheepsFromAuthor(string author, int pageNumber);
}

public class CheepService : ICheepService
{
    private readonly IDatabaseRepository<CheepViewModel.CheepViewModel> _DatabaseRepository =
        DBFacade<CheepViewModel.CheepViewModel>.getInstance();
    
    public List<CheepViewModel.CheepViewModel> GetCheeps(int page)
    {
        List<CheepViewModel.CheepViewModel> cheeps = _DatabaseRepository.Read(page).ToList();

        return cheeps;
    }

    public List<CheepViewModel.CheepViewModel> GetCheepsFromAuthor(string author, int page)
    {
        // filter by the provided author name
        List<CheepViewModel.CheepViewModel> cheeps = _DatabaseRepository.ReadFromAuthor(author,page).ToList();

        return cheeps;
    }

    private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }
}

