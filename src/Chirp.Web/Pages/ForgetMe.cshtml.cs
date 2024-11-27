using System.Security.Claims;
using Chirp.Core;
using Chirp.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class ForgetMe : PageModel
{

    private ICheepRepository _repository;
    private SignInManager<ChirpUser> _signInManager;
    
    public ForgetMe(ICheepRepository repository, SignInManager<ChirpUser> signInManager)
    {
        _repository = repository;
        _signInManager = signInManager;
    }
    
    public ActionResult OnGet()
    {
        _repository.AnonymizeUser(User.FindFirstValue(ClaimTypes.Name),User.FindFirstValue(ClaimTypes.Email));
        _signInManager.SignOutAsync();
        return Redirect("/");
    }
}