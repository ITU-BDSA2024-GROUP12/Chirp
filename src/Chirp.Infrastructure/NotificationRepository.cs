using Chirp.Core;
namespace Chirp.Infrastructure;
using Microsoft.EntityFrameworkCore;

public class NotificationRepository : INotificationRepository
{
    private readonly CheepDbContext _cheepDbContext;

    public NotificationRepository(CheepDbContext cheepDbContext, IAuthorRepository authorRepository)
    {
        _cheepDbContext = cheepDbContext;
        
        Console.WriteLine("Created Notification Repository: " + this.GetType().Name);
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

        public async Task ForgetMentions(string authorName)
    {
        const string deletedUserPlaceholder = "DeletedUser";

         // Find mentions of the deleted user and update the username to "DeletedUser"
        var mentionsToUpdate = _cheepDbContext.CheepMentions
        .Where(m => m.MentionedUsername == authorName);

        //Change the database
        await mentionsToUpdate.ForEachAsync(m => m.MentionedUsername = deletedUserPlaceholder);
        await _cheepDbContext.SaveChangesAsync();
    }

    public async Task DeleteNotificationsForUser(int userId)
    {
        var notifications = _cheepDbContext.Notifications.Where(n => n.AuthorId == userId);
        _cheepDbContext.Notifications.RemoveRange(notifications);
        await _cheepDbContext.SaveChangesAsync();
    }
}