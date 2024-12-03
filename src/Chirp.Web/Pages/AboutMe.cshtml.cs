using System.Security.Claims;
using System.Text.RegularExpressions;
using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;

namespace Chirp.Web.Pages.Shared;

public class AboutMe : PageModel
{
    private readonly ICheepRepository _repository;
    private readonly IAuthorRepository _authorRepository;
    
    public List<CheepDTO> Cheeps { get; set; } = new List<CheepDTO>();

    public int noOfCheeps;
    public int page;

    public AboutMe(ICheepRepository repository, IAuthorRepository authorRepository)
    {
        _repository = repository;
    }

    private async void GetCheeps(string? name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return;
        }
        Cheeps = await _repository.GetMessagesFromAuthor(name, 1);
    }
    
    public void OnGet()
    {
        
        StringValues pageQuery = Request.Query["page"];
        if(!Int32.TryParse(pageQuery, out page)) 
        {
            page = 1;
        }
        
        string name = User.FindFirstValue(ClaimTypes.Name);
        GetCheeps(page, name);
        noOfCheeps = _repository.CheepCountFromAuthor(name).Result;
        
    }
    
    private async Task GetCheeps(int page, string author)
    {
        var cheeps = await _repository.GetMessagesFromAuthor(author,page);

        foreach (var cheep in cheeps)
        {
            cheep.HighlightedParts = await HighlightMentionsAsync(cheep.Text);
        }

        Cheeps = cheeps;
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
            var validUsernames = await _authorRepository.GetValidUsernames(matches);

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
}