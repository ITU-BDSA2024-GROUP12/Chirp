namespace Chirp.Core;

public class CheepMentionDTO
{
    public int Id { get; set; } // Mention ID
    public int CheepId { get; set; } // The Cheep containing the mention
    public required string MentionedUsername { get; set; } // Username of the mentioned user
}