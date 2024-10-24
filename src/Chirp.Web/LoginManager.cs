using AspNet.Security.OAuth.GitHub;
using Microsoft.AspNetCore.Authentication;



namespace Chirp.Web;

public class LoginManager
{
    public async Task<bool> PasswordSignInAsync(string claimType, string claimValue)
    {
        return true;
    }
}