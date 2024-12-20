using System.Collections.Specialized;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Chirp.Core;
using Chirp.Infrastructure;
using Chirp.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Extensions.Primitives;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepRepository _cRepository;
    private readonly IAuthorRepository _aRepository;
    
    [BindProperty]
    public string Cheep { get; set; }
    public List<CheepDTO> Cheeps { get; set; } = new List<CheepDTO>();
    
    private SignInManager<ChirpUser> _signInManager;


    public int page;
    
    public int noOfCheeps;

    public decimal pagesOfCheeps;


    public List<int> Follows;

    public PublicModel(ICheepRepository cRepository, IAuthorRepository aRepository, SignInManager<ChirpUser> signInManager)
    {
        _cRepository = cRepository;
        _aRepository = aRepository;
        _signInManager = signInManager;
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
            var regex = new Regex(@"@([a-zA-Z0-9]+(?:-[a-zA-Z0-9]+)?)(?=\s|$)");
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
    
    public async Task<ActionResult> OnGet(int pageNumber = 1)
    {
        if (User.Identity.IsAuthenticated)
        {
            Follows = new List<int>();
            List<int> initFollows = await _aRepository.GetFollowerIds(User.FindFirstValue(ClaimTypes.Name));
            if (initFollows != null)
            {
                foreach (var list in initFollows)
                {
                    Follows.Add(list);
                }
            }
        }
        StringValues pageQuery = Request.Query["page"];
        if(!Int32.TryParse(pageQuery, out page)) 
        {
            page = 1;
        }

        
        noOfCheeps = _cRepository.CheepCount().Result;

        pagesOfCheeps = Math.Ceiling((decimal)noOfCheeps / (decimal)32.0);
        
        await GetCheeps(page);

        return Page();
    }

    private async Task GetCheeps(int pageNumber)
    {
        var cheeps = await _cRepository.GetMessages(pageNumber);

        foreach (var cheep in cheeps)
        {
            cheep.HighlightedParts = await HighlightMentionsAsync(cheep.Text);
        }

        Cheeps = cheeps;
    }

    public async Task<IActionResult> OnPostFollow()
    {
        var following = await _aRepository.GetFollowerIds(User.FindFirstValue(ClaimTypes.Name));
        //List<int> authorname = new List<int>();
        //authorname.Add(Convert.ToInt32(Request.Form["author"]));
        int authorname = Convert.ToInt32(Request.Form["author"]);
        if (following != null)
        {
            if (!following.Contains(authorname))
            {
                _aRepository.FollowUser(authorname, User.FindFirstValue(ClaimTypes.Name));
            }
        }
        else
        {
            _aRepository.FollowUser(authorname, User.FindFirstValue(ClaimTypes.Name));
        }
        return RedirectToPage("Public"); // it is good practice to redirect the user after a post request
    }
    
    public async Task<IActionResult> OnPostUnfollow()
    {
        var following = await _aRepository.GetFollowerIds(User.FindFirstValue(ClaimTypes.Name));
        //List<int> authorname = new List<int>();
        //authorname.Add(Convert.ToInt32(Request.Form["author"]));
        int authorname = Convert.ToInt32(Request.Form["author"]);
        if (following != null)
        { 
            await _aRepository.UnfollowUser(authorname, User.FindFirstValue(ClaimTypes.Name));
        }
        return RedirectToPage("Public"); // it is good practice to redirect the user after a post request
    }
    
    
    public async Task<IActionResult> OnPostCheep(string Cheep)
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
                Email = User.FindFirstValue(ClaimTypes.Email)
            };

            // Parsing mentions from the Cheep text (@username)
            var mentions = Util.ExtractMentions(Cheep);

            //only query if there are mentions
            if (mentions.Count > 0)
            {
                var validMentions = await _aRepository.GetValidUsernames(mentions);
                _cRepository.CreateCheep(author, Cheep, validMentions, DateTimeOffset.UtcNow.ToString());
            }
            else _cRepository.CreateCheep(author, Cheep, null, DateTimeOffset.UtcNow.ToString());

        return RedirectToPage("Public"); // it is good practice to redirect the user after a post request
    }
}