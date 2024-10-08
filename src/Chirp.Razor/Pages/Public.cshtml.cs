using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DataModel;
using Microsoft.Extensions.Primitives;


namespace Chirp.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepRepository _repository;
    public List<CheepDTO> Cheeps { get; set; }

    public PublicModel(ICheepRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<ActionResult> OnGet(int pageNumber = 1)
    {
        StringValues pageQuery = Request.Query["page"];
        if (!pageQuery.ToString().Equals(""))
        {
            pageNumber = int.Parse(pageQuery);
        }
        Cheeps = await _repository.ReadMessage(pageNumber);
        return Page();
    }
}
