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
    public int page;
    public string authornextpage;
    

    public UserTimelineModel(ICheepRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<ActionResult> OnGet(string author)
    {
        authornextpage = author;
        int pageNumber;
        StringValues pageQuery = Request.Query["page"];
        if(!Int32.TryParse(pageQuery, out pageNumber)) 
		{
			pageNumber = 1;
		}
        page = pageNumber;
        GetCheeps(pageNumber, author);
        return Page();
    }

    private async void GetCheeps(int page, string author)
    {
        Cheeps = await _repository.GetMessagesFromAuthor(author,page);
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
        _repository.CreateCheep(author, Cheep, DateTimeOffset.UtcNow.ToString());
        return RedirectToPage("UserTimeline"); // it is good practice to redirect the user after a post request
    }
}
