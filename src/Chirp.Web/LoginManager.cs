using AspNet.Security.OAuth.GitHub;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;



namespace Chirp.Web;

public class LoginManager
{
    private readonly SignInManager<IdentityUser> _signInManager;
    public async Task<bool> PasswordSignInAsync(string email, string password, bool rememberMe, bool logoutonfailure)
    {
        return true;
    }

    public async Task<bool> LogIn()
    {
        /*var result = await _signInManager.PasswordSignInAsync(Input.Email,
                       Input.Password, Input.RememberMe, lockoutOnFailure: true);*/
        return true;
    }
}