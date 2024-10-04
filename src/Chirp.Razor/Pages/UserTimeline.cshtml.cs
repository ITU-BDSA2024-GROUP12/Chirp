using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DataModel;
using Microsoft.Extensions.Primitives;

namespace Chirp.Razor.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepRepository _repository;
    public List<CheepDTO> Cheeps { get; set; }

    public UserTimelineModel(ICheepRepository repository)
    {
        _repository = repository;
    }
    
    public ActionResult OnGet(int pageNumber = 1)
    {
        StringValues pageQuery = Request.Query["page"];
        if (!pageQuery.ToString().Equals(""))
        {
            pageNumber = int.Parse(pageQuery);
        }
        _repository.ReadMessage();
        return Page();
    }
}
