﻿using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace RV_Park_Reservation_System.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            if (User.IsInRole(SD.AdminRole) || User.IsInRole(SD.EmployeeRole))
            {
                return RedirectToPage("./Admin/Index");
            }

            return Page();
        }
    }
}
