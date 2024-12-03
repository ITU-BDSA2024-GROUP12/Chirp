namespace Chirp.Core;
public class CheepDTO
{
    public string Author { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public long TimeStamp { get; set; }
    
    public int AuthorId { get; set; }
    public string DateTime()
    {
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        return dateTime.AddSeconds(TimeStamp).ToString("yyyy-MM-dd HH:mm:ss");
    }

    public List<(string Text, bool IsMention)> HighlightedParts { get; set; } // Add this
}