namespace Chirp.Core;

public class NotificationDTO
{
    public int Id { get; set; } // Notification ID
    public int CheepId { get; set; } // The Cheep causing the notification
    public string? Content { get; set; } //message displayed in the notification
    public required int AuthorId { get; set; } //user recieving the notification
    public DateTime Timestamp { get; set; } // Time of the notification
}
