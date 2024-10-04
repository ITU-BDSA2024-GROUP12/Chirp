using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CheepViewModel;
using Microsoft.Extensions.Primitives;


namespace Chirp.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _service;
    private readonly ICheepRepository _repository;
    public List<CheepViewModel.CheepViewModel> Cheeps { get; set; }

    public PublicModel(ICheepService service, ICheepRepository repository)
    {
        _service = service;
        _repository = repository;
    }
    
    public ActionResult OnGet(int pageNumber = 1)
    {
        StringValues pageQuery = Request.Query["page"];
        if (!pageQuery.ToString().Equals(""))
        {
            pageNumber = int.Parse(pageQuery);
        }
        Cheeps = _service.GetCheeps(pageNumber);
        _repository.ReadMessage();
        return Page();
    }
}
