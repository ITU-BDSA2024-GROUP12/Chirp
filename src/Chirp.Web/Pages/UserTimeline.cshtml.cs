using System.Security.Claims;
using System.Text.RegularExpressions;
using Chirp.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepRepository _cRepository;
    private readonly IAuthorRepository _aRepository;
    public List<CheepDTO> Cheeps { get; set; } = new List<CheepDTO>();
    public List<NotificationDTO> Notifications { get; set; }

    public int page;

    public UserTimelineModel(ICheepRepository cRepository, IAuthorRepository aRepository)
    {
        _cRepository = cRepository;
        _aRepository = aRepository;
    }

    /// <summary>
    /// Replaces valid @usernames with a highlight
    /// It is seperated into parts to allow insertion of links while keeping the safety of Razor pages and avoiding raw html
    /// </summary>
    /// <param name="content">The Text of a cheep</param>
    /// <returns>a list of parts that is either a highlight or plain text</returns>
    private async Task<List<(string Text, bool IsMention)>> HighlightMentionsAsync(string content)
    {
        // Extract mentioned usernames using Regex

        var matches = Util.ExtractMentions(content);
        var result = new List<(string Text, bool IsMention)>();

        if (matches.Count > 0)
        {
            // Validate the mentions
            var validUsernames = await _aRepository.GetValidUsernames(matches);

            // Break content into parts
            var regex = new Regex(@"@([A-Za-z0-9_\. -]+)");
            int lastIndex = 0;
            foreach (Match match in regex.Matches(content))
            {
                if (match.Index > lastIndex)
                {
                    // Add plain text before the mention
                    result.Add((content.Substring(lastIndex, match.Index - lastIndex), false));
                }

                var username = match.Groups[1].Value;
                if (validUsernames.Any(u => u.Name == username))
                {
                    // Valid mention
                    result.Add((match.Value, true));
                }
                else
                {
                    // Invalid mention, treat as plain text
                    result.Add((match.Value, false));
                }

                lastIndex = match.Index + match.Length;
            }

            // Add any remaining plain text
            if (lastIndex < content.Length)
            {
                result.Add((content.Substring(lastIndex), false));
            }
        }
        else
        {
            // No mentions, return the entire content as plain text
            result.Add((content, false));
        }

        return result;
    }
    
    

    public async Task<ActionResult> OnGet(string author)
    {
        if (author == User.Identity.Name)
        {
            int pageNumber;
            StringValues pageQuery = Request.Query["page"];
            if (!Int32.TryParse(pageQuery, out pageNumber))
            {
                pageNumber = 1;
            }

            page = pageNumber;
            GetFollowedCheeps(pageNumber, User.Identity.Name);
            var userName = User.Identity.Name;
            if (User.Identity.IsAuthenticated && author == userName)
            {
                GetNotifications(author);
            }
            else
            {
                Notifications = new List<NotificationDTO>(); //Empty instead of null
            }
        }
        else
        {
            int pageNumber;
            StringValues pageQuery = Request.Query["page"];
            if (!Int32.TryParse(pageQuery, out pageNumber))
            {
                pageNumber = 1;
            }

            page = pageNumber;
            GetCheeps(pageNumber, author);
            var userName = User.Identity.Name;
            if (User.Identity.IsAuthenticated && author == userName)
            {
                GetNotifications(author);
            }
            else
            {
                Notifications = new List<NotificationDTO>(); //Empty instead of null
            }
        }

        return Page();
    }

    private async void GetCheeps(int page, string author)
    {
        var cheeps = await _cRepository.GetMessagesFromAuthor(author,page);

        foreach (var cheep in cheeps)
        {
            cheep.HighlightedParts = await HighlightMentionsAsync(cheep.Text);
        }

        Cheeps = cheeps;
    }
    
    private async void GetFollowedCheeps(int page, string user)
    {
        var following = await _cRepository.GetFollowerIds(user);
        var allCheeps = new List<CheepDTO>();
        if (following != null)
        {
            foreach (int i in following)
            {
                AuthorDTO author = await _aRepository.GetAuthorById(i);
                var cheeps = await _cRepository.GetMessagesFromAuthor(author.Name,page);
                allCheeps.AddRange(cheeps);
            }

            foreach (var cheep in allCheeps)
            {
                cheep.HighlightedParts = await HighlightMentionsAsync(cheep.Text);
            }
        }
        Cheeps = allCheeps;
    }

    private async void GetNotifications(string author){
        var notifications = await _cRepository.GetNotifications(author);

        Notifications = notifications;
    }
    
    public async Task<IActionResult> OnPostFollow()
    {
        var following = await _cRepository.GetFollowerIds(User.FindFirstValue(ClaimTypes.Name));
        int authorname = Convert.ToInt32(Request.Form["author"]);
        if (following != null)
        {
            if (following.Contains(authorname))
            {
                _cRepository.UnfollowUser(authorname, User.FindFirstValue(ClaimTypes.Name));
            }
            else
            {
                _cRepository.FollowUser(authorname, User.FindFirstValue(ClaimTypes.Name));
            }
        }
        else
        {
            _cRepository.FollowUser(authorname, User.FindFirstValue(ClaimTypes.Name));
        }
        return RedirectToPage("UserTimeline"); // it is good practice to redirect the user after a post request
    }
    
    /// <summary>
    /// Method to to post, ensures proper lenght and checks for mentions, before passing it to the create cheep methods
    /// </summary>
    /// <param name="Cheep">The text to be cheeped</param>
    /// <returns>A redirect to the same page, to update with the new cheep</returns>
    public async Task<IActionResult> OnPostCheep(string Cheep)
    {
        // Do something with the text ...
        if (Cheep.Length > 160)
        {
            ModelState.AddModelError("Cheep", "Cheep is too long, Max 160 Charecters, Your was " + Cheep.Length);
            GetCheeps(1, User.FindFirstValue(ClaimTypes.Name));
            return Page();
        }
        AuthorDTO author = new AuthorDTO()
        {
            Name = User.FindFirstValue(ClaimTypes.Name),
            Email = User.FindFirstValue(ClaimTypes.Email)
        };

        // Parsing mentions from the Cheep text (@username)
        var mentions = Util.ExtractMentions(Cheep);
        
        //only query if there's any mentions
        if(mentions.Count > 0) {
             var validMentions =  await _aRepository.GetValidUsernames(mentions);
             _cRepository.CreateCheep(author, Cheep, validMentions, DateTimeOffset.UtcNow.ToString());
        }
        else _cRepository.CreateCheep(author, Cheep, null, DateTimeOffset.UtcNow.ToString());
        
        return RedirectToPage("UserTimeline"); // it is good practice to redirect the user after a post request
    }
}
