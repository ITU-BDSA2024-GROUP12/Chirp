using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CheepViewModel;
using Microsoft.Extensions.Primitives;

namespace Chirp.Razor.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepService _service;
    public List<CheepViewModel.CheepViewModel> Cheeps { get; set; }

    public UserTimelineModel(ICheepService service)
    {
        _service = service;
    }

    public ActionResult OnGet(string author)
    {
        int pageNumber = 1;
        StringValues pageQuery = Request.Query["page"];
        if (!pageQuery.ToString().Equals(""))
        {
            pageNumber = int.Parse(pageQuery);
        }
        Console.WriteLine($"pageQuery: {pageQuery} | pageNumber: {pageNumber}");
        Cheeps = _service.GetCheepsFromAuthor(author, pageNumber);
        return Page();
    }
}
