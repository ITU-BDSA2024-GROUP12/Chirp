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
    
    [BindProperty]
    public string Cheep { get; set; }
    public List<CheepDTO> Cheeps { get; set; } = new List<CheepDTO>();
    
    private SignInManager<ChirpUser> _signInManager;

    public int page;

    public PublicModel(ICheepRepository repository, SignInManager<ChirpUser> signInManager)
    {
        _repository = repository;
        _signInManager = signInManager;
    }
    
    public async Task<ActionResult> OnGet(int pageNumber = 1)
    {
        StringValues pageQuery = Request.Query["page"];
        if(!Int32.TryParse(pageQuery, out pageNumber)) 
        {
            pageNumber = 1;
        }
        page = pageNumber;
        GetCheeps(pageNumber);
        return Page();
    }

    private async void GetCheeps(int pageNumber)
    {
        Cheeps = await _repository.GetMessages(pageNumber);
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
            Email = User.FindFirstValue(ClaimTypes.Email),
        };
        _repository.CreateCheep(author, Cheep, DateTimeOffset.UtcNow.ToString());
        return RedirectToPage("Public"); // it is good practice to redirect the user after a post request
    }
}
