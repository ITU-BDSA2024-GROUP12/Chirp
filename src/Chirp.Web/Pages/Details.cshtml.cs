using Chirp.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;
public class CheepDetailsModel : PageModel
{
    private readonly ICheepRepository _repository;

    public CheepDTO Cheep { get; set; }

    public CheepDetailsModel(ICheepRepository repository)
    {
        _repository = repository;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Cheep = await _repository.GetCheepById(id);
        if (Cheep == null)
        {
            return NotFound();
        }
        return Page();
    }
}
