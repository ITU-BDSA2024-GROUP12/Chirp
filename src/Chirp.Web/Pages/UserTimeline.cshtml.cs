using System.Security.Claims;
using Chirp.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepRepository _repository;
    public List<CheepDTO> Cheeps { get; set; } = new List<CheepDTO>();
    public int PageNumber;

    public UserTimelineModel(ICheepRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<ActionResult> OnGet(string author)
    {
        StringValues pageQuery = Request.Query["page"];
        if(!Int32.TryParse(pageQuery, out PageNumber)) 
		{
			PageNumber = 1;
		}
        
        await GetCheeps(PageNumber, author);
        return Page();
    }

    private async Task GetCheeps(int page, string author)
    {
        Cheeps = await _repository.GetMessagesFromAuthor(author,page);
    }
    
    public async Task<ActionResult> OnPost(string Cheep)
    {
        // Do something with the text ...
        var name = User.FindFirstValue(ClaimTypes.Name);
        var email = User.FindFirstValue(ClaimTypes.Email);
        if (Cheep.Length > 160)
        {
            ModelState.AddModelError("Cheep", "Cheep is too long, Max 160 Charecters, Your was " + Cheep.Length);
            if (name is not null)
            {
                await GetCheeps(1, name);
            }
            return Page(); 
            
        }
        if (name is not null && email is not null)
        {
            AuthorDTO author = new AuthorDTO()
            {
                Name = name,
                Email = email
            };
            _repository.CreateCheep(author, Cheep, DateTimeOffset.UtcNow.ToString());
        }
        return RedirectToPage("UserTimeline"); // it is good practice to redirect the user after a post request
        
        
        
        
    }
}
