using System.Security.Claims;
using Chirp.Core;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class ForgetMe : PageModel
{

    private ICheepRepository _repository;
    
    public ForgetMe(ICheepRepository repository)
    {
        _repository = repository;
    }
    
    public void OnGet()
    {
        Console.WriteLine(User.FindFirstValue(ClaimTypes.Name));
        Console.WriteLine(User.FindFirstValue(ClaimTypes.Email));
        _repository.AnonymizeUser(User.FindFirstValue(ClaimTypes.Name),User.FindFirstValue(ClaimTypes.Email));
    }
}