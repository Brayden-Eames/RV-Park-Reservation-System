using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RV_Park_Reservation_System.Pages.Admin
{
    public class SitesModel : PageModel
    {
        public IActionResult OnGet()
        {
            if (!User.Identity.IsAuthenticated || User.IsInRole(SD.CustomerRole))
            {
                 return RedirectToPage("/Shared/Prohibited", new { path = "/Admin/Sites/Index"});
            }

            return Page();
        }
    }
}
