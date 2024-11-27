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
        _repository.AnonymizeUser("poopballs","rasmus06111@gmail.com1");
    }
}