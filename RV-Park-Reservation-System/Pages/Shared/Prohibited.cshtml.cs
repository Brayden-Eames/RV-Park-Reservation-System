using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RV_Park_Reservation_System.Pages.Shared
{
    public class ProhibitedModel : PageModel
    {
        [BindProperty]
        public string UrlPath { get; set; }
        public IActionResult OnGet(string path)
        {
            UrlPath = path; //this is for redirecting back after log in.
            return Page();
        }
    }
}
