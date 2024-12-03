using Chirp.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;
public class CheepDetailsModel : PageModel
{
    private readonly ICheepRepository _cRepository;
    private readonly INotificationRepository _nRepository;
    public CheepDTO Cheep { get; set; }

    public CheepDetailsModel(ICheepRepository cRepository, INotificationRepository nRepository)
    {
        _cRepository = cRepository;
        _nRepository = nRepository;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Cheep = await _cRepository.GetCheepById(id);
        if (Cheep == null)
        {
            return NotFound();
        }
        
        return Page();
    }

    public async Task<IActionResult> OnPostDismissAsync(int notificationId)
    {
        // Delete the notification
        await _nRepository.DeleteNotification(notificationId);

        // Redirect back to the user's timeline
        return Redirect($"/{User.Identity.Name}");
    }
}
