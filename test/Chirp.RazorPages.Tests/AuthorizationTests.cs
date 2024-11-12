using Chirp.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Chirp.RazorPages.Tests;

public class AuthorizationTests
{
    private SignInManager<ChirpUser> _signInManager;

    public AuthorizationTests(SignInManager<ChirpUser> signInManager) {
        
        {
            _signInManager = signInManager;
            
        }
    }

    /*[Theory]
    [InlineData("adho@itu.dk", "M32Want_Access")]

    public async void EndtoEndLogin(string email, string password)
    {
        var result = await _signInManager.PasswordSignInAsync(email, password, false, false);

        Assert.True(result.Succeeded);
    }*/
}