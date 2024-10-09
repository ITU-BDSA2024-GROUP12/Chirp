namespace Chirp.Infrastructure;
public class CheepDTO
{
    public string Author { get; set; }
    public string Text { get; set; }
    public long TimeStamp { get; set; }
    public string DateTime()
    {
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        return dateTime.AddSeconds(TimeStamp).ToString("yyyy-MM-dd HH:mm:ss");
    }
}