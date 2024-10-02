﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CheepViewModel;
using Microsoft.Extensions.Primitives;


namespace Chirp.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _service;
    public List<CheepViewModel.CheepViewModel> Cheeps { get; set; }

    public PublicModel(ICheepService service)
    {
        _service = service;
    }
    
    public ActionResult OnGet(int pageNumber = 1)
    {
        StringValues pageQuery = Request.Query["page"];
        if (!pageQuery.ToString().Equals(""))
        {
            pageNumber = int.Parse(pageQuery);
        }
        Cheeps = _service.GetCheeps(pageNumber);
        return Page();
    }
}
