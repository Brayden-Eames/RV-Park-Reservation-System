using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RV_Park_Reservation_System.Pages.Admin.Reports
{
    public class TransactionReportModel : PageModel
    {
        public IActionResult OnGet()
        {
            if (!User.Identity.IsAuthenticated || User.IsInRole(SD.CustomerRole))
            {
                return RedirectToPage("/Shared/Prohibited", new { path = "/Admin/Reports/TransactionReport" });
            }

            return Page();
        }
    }
}
