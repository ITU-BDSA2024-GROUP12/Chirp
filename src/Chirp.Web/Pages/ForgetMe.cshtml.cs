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
    private INotificationRepository _nRepository;
    private SignInManager<ChirpUser> _signInManager;
    
    public ForgetMe(IAuthorRepository repository, INotificationRepository notificationRepository, SignInManager<ChirpUser> signInManager)
    {
        _repository = repository;
        _nRepository = notificationRepository;
        _signInManager = signInManager;
    }
    
    public ActionResult OnGet()
    {
        var username = User.FindFirstValue(ClaimTypes.Name);
        var email = User.FindFirstValue(ClaimTypes.Email);
        if(username != null && email != null) {
            _nRepository.ForgetMentions(username);
            _nRepository.DeleteNotificationsForUser(_repository.GetAuthor(username, email).Id);
            _repository.DeleteUser(username,email);
            _signInManager.SignOutAsync();
        }
        return Redirect("/");
    }
}