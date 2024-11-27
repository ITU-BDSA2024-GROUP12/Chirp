using System.Security.Claims;
using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages.Shared;

public class AboutMe : PageModel
{
    
    private readonly ICheepRepository _repository;
    
    public List<CheepDTO> Cheeps { get; set; } = new List<CheepDTO>();

    public AboutMe(ICheepRepository repository)
    {
        _repository = repository;
    }
    
    public async void OnGet()
    {
        Cheeps = await _repository.GetMessagesFromAuthor(User.FindFirstValue(ClaimTypes.Name), 1);
    }
}