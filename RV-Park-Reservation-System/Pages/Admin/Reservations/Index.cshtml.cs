using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using RV_Park_Reservation_System.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Infrastructure.Services;

namespace RV_Park_Reservation_System.Pages.Admin
{
    public class ReservationsModel : PageModel
    {
        private readonly IUnitOfWork _unitofWork;
        public ReservationsModel(IUnitOfWork unitofWork) => _unitofWork = unitofWork;

        [BindProperty]
        public IEnumerable<Reservation> ReservationList { get; set; }

        public AdminReservationVM AdminReservationObject { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
       
        public IActionResult OnGet(bool success = false, string message = null)
        {
            if (!User.Identity.IsAuthenticated || User.IsInRole(SD.CustomerRole))
            {
                return RedirectToPage("/Shared/Prohibited", new { path = "/Admin/Reservations/Index" });
            }

            return Page();
        }

        public async Task<IActionResult> OnPost(int? id)
        {           
            return RedirectToPage("./Upsert", new { reservationID = id });
        }
    }
}
