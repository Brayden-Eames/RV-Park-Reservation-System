using System;
using System.Collections.Generic;
using System.Linq;
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

        //We might need to make a ViewModel to allow us to pull from the Reservation, Customer, Service Status Type and DODAffiliation tables. 
        public IActionResult OnGet(bool success = false, string message = null)
        {
            if (!User.Identity.IsAuthenticated || User.IsInRole(SD.CustomerRole))
            {
                return RedirectToPage("/Shared/Prohibited", new { path = "/Admin/Reservations/Index" });
            }
            

            //need to pull data from Reservation, Customer, DODAffiliation and ServiceStatusType tables. Use the ViewModel to do so
            Success = success;
            Message = message;
            ReservationList = _unitofWork.Reservation.List(r => r.ResStatusID == 1 || r.ResStatusID == 4 || r.ResStatusID == 9); //9 for scheduled 4 for On Going
            AdminReservationObject = new AdminReservationVM()
            {
                Reservations = _unitofWork.Reservation.List(),
                ListOfCustomers = _unitofWork.Customer.List(),
                DODAffiliationList = _unitofWork.DOD_Affiliation.List(),
                ServiceStatusTypes = _unitofWork.Service_Status_Type.List()
            };

            
            return Page();
        }

        public async Task<IActionResult> OnPost(int? id)
        {           
            return RedirectToPage("./Upsert", new { reservationID = id });
        }
    }
}
