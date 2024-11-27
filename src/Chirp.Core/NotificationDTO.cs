namespace Chirp.Core;

public class NotificationDTO
{
    public int Id { get; set; } // Notification ID
    public int CheepId { get; set; } // The Cheep causing the notification
    public required string CheepContent { get; set; } // Optional: Include content of the Cheep
    public required string AuthorName { get; set; } // Name of the user who created the notification
    public DateTime Timestamp { get; set; } // Time of the notification
}
