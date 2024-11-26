using System.Security.Claims;
using System.Text.RegularExpressions;
using Chirp.Core;
using Chirp.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepRepository _repository;
    
    [BindProperty]
    public string Cheep { get; set; }
    public List<CheepDTO> Cheeps { get; set; } = new List<CheepDTO>();
    
    private SignInManager<ChirpUser> _signInManager;

    public PublicModel(ICheepRepository repository, SignInManager<ChirpUser> signInManager)
    {
        _repository = repository;
        _signInManager = signInManager;
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
        if (matches.Count > 0)
        {

            // Validate the mentions
            var validUsernames = await _repository.GetValidUsernames(matches);

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
        else
        {
            return content;
        }
    }
    
    public async Task<ActionResult> OnGet(int pageNumber = 1)
    {
        StringValues pageQuery = Request.Query["page"];
        Int32.TryParse(pageQuery, out pageNumber);
        GetCheeps(pageNumber);
        return Page();
    }

    private async void GetCheeps(int pageNumber)
    {
        var cheeps = await _repository.GetMessages(pageNumber);

        foreach (var cheep in cheeps)
        {
            cheep.Text = await HighlightMentionsAsync(cheep.Text);
        }

        Cheeps = cheeps;
    }
    
    public ActionResult OnPost(string Cheep)
    {
        if (Cheep.Length > 160)
        {
            ModelState.AddModelError("Cheep", "Cheep is too long, Max 160 Charecters, Your was " + Cheep.Length);
            GetCheeps(1);
            return Page();
        }
        
        // Do something with the text ...
        AuthorDTO author = new AuthorDTO()
        {
            Name = User.FindFirstValue(ClaimTypes.Name),
            Email = User.Identity.Name
        };

               // Parsing mentions from the Cheep text (@username)
        var mentions = Util.ExtractMentions(Cheep);

        var ValidMentions =  _repository.GetValidUsernames(mentions);
        
        _repository.CreateCheep(author, Cheep, DateTimeOffset.UtcNow.ToString());
        return RedirectToPage("Public"); // it is good practice to redirect the user after a post request
    }
}
