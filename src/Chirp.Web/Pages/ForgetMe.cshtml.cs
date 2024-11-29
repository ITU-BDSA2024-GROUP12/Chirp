using System.Security.Claims;
using Chirp.Core;
using Chirp.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class ForgetMe : PageModel
{
    private IAuthorRepository _repository;
    private SignInManager<ChirpUser> _signInManager;
    
    public ForgetMe(IAuthorRepository repository, SignInManager<ChirpUser> signInManager)
    {
        _repository = repository;
        _signInManager = signInManager;
    }
    
    public ActionResult OnGet()
    {
        var name = User.FindFirstValue(ClaimTypes.Name);
        var email = User.FindFirstValue(ClaimTypes.Email);
        if (name is null || email is null)
        {
            return RedirectToPage("/about-me");
        }
        _repository.AnonymizeUser(name,email);
        _signInManager.SignOutAsync();
        return Redirect("/");
    }
}