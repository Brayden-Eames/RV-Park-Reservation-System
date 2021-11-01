using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ApplicationCore.Models;

namespace RV_Park_Reservation_System.Pages.Admin.Reservations
{
    public class ReservationsDeleteModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReservationsDeleteModel(IUnitOfWork unitofWork)
        {
            _unitOfWork = unitofWork;
        }
        public async Task<RedirectToPageResult> OnGet(int? id)
        {
            var reservation = _unitOfWork.Reservation.Get(c => c.ResID == id);
            _unitOfWork.Reservation.Delete(reservation);
            await _unitOfWork.CommitAsync();
            return RedirectToPage("./Reservations", new { success = true, message = "Delete Successful"});
        }
    }
}
