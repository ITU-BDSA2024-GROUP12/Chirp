using Chirp.Core;
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

    public PublicModel(ICheepRepository repository)
    {
        _repository = repository;
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
        return RedirectToPage("Public"); // it is good practice to redirect the user after a post request
    }
}
