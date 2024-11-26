using System.Security.Claims;
using System.Text.RegularExpressions;
using Chirp.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepRepository _repository;
    public List<CheepDTO> Cheeps { get; set; } = new List<CheepDTO>();
    

    public UserTimelineModel(ICheepRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Replaces valid @usernames with a highlight
    /// </summary>
    /// <param name="content">The Text of a cheep</param>
    /// <returns></returns>
    private async Task<string> HighlightMentionsAsync(string content)
    {
        // Extract mentioned usernames using Regex

        var matches = Util.ExtractMentions(content);

        // Validate the mentions
        var validUsernames =  await _repository.GetValidUsernames(matches);

        // Replace mentions with links for valid usernames only
        return new Regex(@"@(\w+)").Replace(content, match =>
        {
            var username = match.Groups[1].Value;
            if (validUsernames.Any(u => u.Name == username))
            {
                return $"<a href='/profile/{username}'>@{username}</a>";
            }
            return $"@{username}"; // Leave as plain text if not valid
        });
    }
    
    public async Task<ActionResult> OnGet(string author)
    {
        int pageNumber;
        StringValues pageQuery = Request.Query["page"];
        if(!Int32.TryParse(pageQuery, out pageNumber)) 
		{
			pageNumber = 1;
		}
        GetCheeps(pageNumber, author);
        return Page();
    }

    private async void GetCheeps(int page, string author)
    {
        var cheeps = await _repository.GetMessagesFromAuthor(author,page);

        foreach (var cheep in cheeps)
        {
            cheep.Text = await HighlightMentionsAsync(cheep.Text);
        }

        Cheeps = cheeps;
    }
    
    public ActionResult OnPost(string Cheep)
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
            Email = User.Identity.Name
        };

        // Parsing mentions from the Cheep text (@username)
        var mentions = Util.ExtractMentions(Cheep);

        var ValidMentions =  _repository.GetValidUsernames(mentions);

        _repository.CreateCheep(author, Cheep, DateTimeOffset.UtcNow.ToString());
        return RedirectToPage("UserTimeline"); // it is good practice to redirect the user after a post request
    }
}
