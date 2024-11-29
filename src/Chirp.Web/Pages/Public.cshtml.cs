using System.Security.Claims;
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
    public List<CheepDTO> Cheeps { get; set; } = new List<CheepDTO>();
    
    private SignInManager<ChirpUser> _signInManager;

    public int PageNumber;

    public PublicModel(ICheepRepository repository, SignInManager<ChirpUser> signInManager)
    {
        _repository = repository;
        _signInManager = signInManager;
    }
    
    public async Task<ActionResult> OnGet(int pageNumber = 1)
    {
        StringValues pageQuery = Request.Query["page"];
        if(!Int32.TryParse(pageQuery, out PageNumber)) 
        {
            PageNumber = 1;
        }
        await GetCheeps(PageNumber);
        return Page();
    }

    private async Task GetCheeps(int pageNumber)
    {
        Cheeps = await _repository.GetMessages(pageNumber);
    }
    
    public async Task<ActionResult> OnPost(string Cheep)
    {
        
        if (Cheep.Length > 160)
        {
            ModelState.AddModelError("Cheep", "Cheep is too long, Max 160 Charecters, Your was " + Cheep.Length);
            await GetCheeps(1);
            return Page();
        }
        var name = User.FindFirstValue(ClaimTypes.Name);
        string? email = User.Identity?.Name;
        // Do something with the text ...
        if (name is not null && email is not null)
        {
            AuthorDTO author = new AuthorDTO() //This gets name, twice?
            {
                Name = name,
                Email = email
            };
            _repository.CreateCheep(author, Cheep, DateTimeOffset.UtcNow.ToString());
        }

        
        return RedirectToPage("Public"); // it is good practice to redirect the user after a post request
    }
}
