using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            UrlPath = path;
            return Page();
        }
    }
}
