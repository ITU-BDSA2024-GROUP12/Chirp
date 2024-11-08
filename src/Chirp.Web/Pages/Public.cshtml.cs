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

    public PublicModel(ICheepRepository repository, SignInManager<ChirpUser> signInManager)
    {
        _repository = repository;
        _signInManager = signInManager;
    }
    
    public async Task<ActionResult> OnGet(int pageNumber = 1)
    {
        StringValues pageQuery = Request.Query["page"];
        Int32.TryParse(pageQuery, out pageNumber);
        Cheeps = await _repository.ReadMessage(pageNumber);
        return Page();
    }
    
    public ActionResult OnPost(string Cheep)
    {
        // Do something with the text ...
        AuthorDTO author = new AuthorDTO()
        {
            Name = User.FindFirstValue("UserName"),
            Email = User.Identity.Name
        };
        _repository.CreateCheep(author, Cheep, DateTimeOffset.UtcNow.ToString());
        return RedirectToPage("Public"); // it is good practice to redirect the user after a post request
    }
}
