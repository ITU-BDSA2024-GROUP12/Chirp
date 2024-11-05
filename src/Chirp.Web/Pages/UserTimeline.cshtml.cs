using Chirp.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepRepository _repository;
    public List<CheepDTO> Cheeps { get; set; } = new List<CheepDTO>(); 

    public UserTimelineModel(ICheepRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<ActionResult> OnGet(string author)
    {
        int pageNumber;
        StringValues pageQuery = Request.Query["page"];
        if(!Int32.TryParse(pageQuery, out pageNumber)) 
		{
			pageNumber = 1;
		}
        Cheeps = await _repository.ReadMessagesFromAuthor(author,pageNumber);
        return Page();
    }
}
