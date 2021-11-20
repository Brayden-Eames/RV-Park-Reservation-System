using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RV_Park_Reservation_System.Pages.Admin.ReservationStatuses
{
    public class UpsertModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public Reservation_Status ReservationStatusObj { get; set; }

        public UpsertModel(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public IActionResult OnGet(int? id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Shared/Prohibited", new { path = "/Admin/ReservationStatuses/Upsert" });
            }

            ReservationStatusObj = new Reservation_Status
            {
                ResStatusName = "",
                ResStatusDescription = ""
            };

            if (id != 0)
            {
                ReservationStatusObj = _unitOfWork.Reservation_Status.Get(r => r.ResStatusID == id, true);
                if (ReservationStatusObj == null)
                {
                    return NotFound();
                }
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (ReservationStatusObj.ResStatusID == 0) //New Reservation Status
            {
                _unitOfWork.Reservation_Status.Add(ReservationStatusObj);
            }
            else //Update
            {
                _unitOfWork.Reservation_Status.Update(ReservationStatusObj);
            }
            _unitOfWork.Commit();
            return RedirectToPage("Index");
        }
    }
}
