using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        [BindProperty]
        public Payment CustomerPayment { get; set; }

        [BindProperty]
        public int reservationID { get; set; } 


        [BindProperty]
        public string firstName { get; set; }

        [BindProperty]
        public string lastName { get; set; }

        public IEnumerable<SelectListItem> sites { get; set; }

        [BindProperty]
        public IEnumerable<SelectListItem> lstServiceStatus { get; set; }

        [BindProperty]
        public IEnumerable<SelectListItem> lstDODAffiliation { get; set; }

        [BindProperty]
        public int siteid { get; set; }

        [BindProperty]
        public decimal totalCost { get; set; }

        [BindProperty]
        public IEnumerable<SelectListItem> lstReservationStatus { get; set; }

        [BindProperty]
        public int reservationStatusID { get; set; }


        //Add in member variables similar to how the AdminReservation Create page does it.  Revamp the Upsert frontend to use this functionality as well. 


        public IActionResult OnGet(int? id, string? userId)
        {
            if (!User.Identity.IsAuthenticated || User.IsInRole(SD.CustomerRole))
            {
                return RedirectToPage("/Shared/Prohibited", new { path = "/Admin/Reservations/Upsert" });
            }

            CustomerReservation = _unitOfWork.Reservation.Get(c => c.ResID == id);
            CustomerInfo = _unitOfWork.Customer.Get(c => c.Id == userId);
            CustomerPayment = _unitOfWork.Payment.Get(p => p.ResID == id);
           
            sites = _unitOfWork.Site.List().Select(f => new SelectListItem { Value = f.SiteID.ToString(), Text = "Lot " + f.SiteID.ToString() });
            lstServiceStatus = _unitOfWork.Service_Status_Type.List().Select(s => new SelectListItem { Value = s.ServiceStatusID.ToString(), Text = s.ServiceStatusType });
            lstDODAffiliation = _unitOfWork.DOD_Affiliation.List().Select(d => new SelectListItem { Value = d.DODAffiliationID.ToString(), Text = d.DODAffiliationType });

            lstReservationStatus = _unitOfWork.Reservation_Status.List().Select(s => new SelectListItem { Value = s.ResStatusID.ToString(), Text = s.ResStatusName });
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string value, string updateType)
        {
            if (value == "update") //conditional for updating everything except for adding a new site and/or dates.
            {
                var reservation = await _unitOfWork.Reservation.GetAsync(c => c.ResID == CustomerReservation.ResID); 
                var siteObj = await _unitOfWork.Site.GetAsync(s => s.SiteID == CustomerReservation.Site.SiteID); 
                reservation.Site = siteObj;  
                reservation.Site.SiteNumber = siteObj.SiteNumber;
                reservation.ResStartDate = CustomerReservation.ResStartDate;
                reservation.ResEndDate = CustomerReservation.ResEndDate;
                reservation.ResNumAdults = CustomerReservation.ResNumAdults;
                reservation.ResNumChildren = CustomerReservation.ResNumChildren;
                reservation.ResNumPets = CustomerReservation.ResNumPets;
                reservation.ResAcknowledgeValidPets = CustomerReservation.ResAcknowledgeValidPets;
                reservation.ResComment = CustomerReservation.ResComment;
                reservation.ResVehicleLength = CustomerReservation.ResVehicleLength;
                reservation.Vehicle_Type = CustomerReservation.Vehicle_Type;
                reservation.ResStatusID = CustomerReservation.ResStatusID;

                _unitOfWork.Reservation.Update(reservation);
                _unitOfWork.Commit();
                return RedirectToPage("./Index", new { success = true, message = "Update Successful" });
            }
            else if (value == "addDaysUpdate") //conditional for adding new days/new site
            {
                var reservation = await _unitOfWork.Reservation.GetAsync(c => c.ResID == CustomerReservation.ResID);
                var siteObj = await _unitOfWork.Site.GetAsync(s => s.SiteID == siteid);
                reservation.Site = siteObj;
                reservation.Site.SiteNumber = siteObj.SiteNumber;
                reservation.ResStartDate = CustomerReservation.ResStartDate;
                reservation.ResEndDate = CustomerReservation.ResEndDate;
                reservation.ResNumAdults = CustomerReservation.ResNumAdults;
                reservation.ResNumChildren = CustomerReservation.ResNumChildren;
                reservation.ResNumPets = CustomerReservation.ResNumPets;
                reservation.ResAcknowledgeValidPets = CustomerReservation.ResAcknowledgeValidPets;
                reservation.ResComment = CustomerReservation.ResComment;
                reservation.ResVehicleLength = CustomerReservation.ResVehicleLength;
                reservation.Vehicle_Type = CustomerReservation.Vehicle_Type;
                reservation.ResStatusID = reservationStatusID;


                HttpContext.Session.Set(SD.ReservationUpdateSession, reservation); //uses the ReservationUpdateSession string to handle this separate from regular sessions.

                return RedirectToPage("./Admin/Reservations/AdminUpdateSummary"); //redirect to the admin page
            }
            else
            {

                return RedirectToPage("./Index", new { success = true, message = "Cancel Successful" });
            }
        }
    }
}
