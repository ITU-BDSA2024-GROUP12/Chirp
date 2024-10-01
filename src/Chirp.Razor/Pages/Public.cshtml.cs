using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CheepViewModel;


namespace Chirp.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _service;
    public List<CheepViewModel.CheepViewModel> Cheeps { get; set; }

    public PublicModel(ICheepService service)
    {
        _service = service;
    }
    
    public ActionResult OnGet(int pageNumber = 1)
    {
        Cheeps = _service.GetCheeps();
        return Page();
    }
}
