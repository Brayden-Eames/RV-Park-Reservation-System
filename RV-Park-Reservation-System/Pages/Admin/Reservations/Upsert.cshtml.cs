using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RV_Park_Reservation_System.Pages.Admin.Reservations
{
    public class ReservationsUpdateModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        //private readonly UserManager<Customer> _userManager;

        public ReservationsUpdateModel(IUnitOfWork unitOfWork/*, UserManager<Customer> userManager*/)
        {
            _unitOfWork = unitOfWork;
            //_userManager = userManager;
        }

        //passing the 'ResID' in through the reservations.cshtml, we can then use that to go through unitofWork and retrieve that reservation, and by extension the customer's info

        [BindProperty]
        public Reservation CustomerReservation { get; set; }

        [BindProperty]
        public Customer CustomerInfo { get; set; }

        public int reservationID { get; set; }

        public IActionResult OnGet(int? id, string? userId)
        {
            if (!User.Identity.IsAuthenticated || User.IsInRole(SD.CustomerRole))
            {
                return RedirectToPage("/Shared/Prohibited", new { path = "/Admin/Reservations/Upsert" });
            }

            CustomerReservation = _unitOfWork.Reservation.Get(c => c.ResID == id);
            CustomerInfo = _unitOfWork.Customer.Get(c => c.Id == userId);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string value)
        { //current issue: value is not being sent to the page. Fix: Switch if checks to check for update instead of delete
            if(value == "update") // if the delete button is clicked
            {
                var reservation = _unitOfWork.Reservation.Get(c => c.ResID == CustomerReservation.ResID);
                reservation.SiteID = CustomerReservation.SiteID;
                reservation.ResStartDate = CustomerReservation.ResStartDate;
                reservation.ResEndDate = CustomerReservation.ResEndDate;
                reservation.ResNumAdults = CustomerReservation.ResNumAdults;
                reservation.ResNumChildren = CustomerReservation.ResNumChildren;
                reservation.ResNumPets = CustomerReservation.ResNumPets;
                reservation.ResAcknowledgeValidPets = CustomerReservation.ResAcknowledgeValidPets;
                reservation.ResComment = CustomerReservation.ResComment;
                reservation.ResVehicleLength = CustomerReservation.ResVehicleLength;
                reservation.Vehicle_Type = CustomerReservation.Vehicle_Type;
                reservation.Reservation_Status = CustomerReservation.Reservation_Status;
                _unitOfWork.Reservation.Update(reservation);
                _unitOfWork.Commit();
                return RedirectToPage("./Index", new { success = true, message = "Update Successful" });
            }
            else
            {

                return RedirectToPage("./Index", new { success = true, message = "Cancel Successful" });
            }
        }
    }
}
