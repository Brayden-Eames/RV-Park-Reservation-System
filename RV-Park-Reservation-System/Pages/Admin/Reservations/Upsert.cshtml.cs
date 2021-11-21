using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Infrastructure.Services;
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
            sites = _unitOfWork.Site.List().Select(f => new SelectListItem { Value = f.SiteID.ToString(), Text = "Lot " + f.SiteID.ToString() });
            lstServiceStatus = _unitOfWork.Service_Status_Type.List().Select(s => new SelectListItem { Value = s.ServiceStatusID.ToString(), Text = s.ServiceStatusType });
            lstDODAffiliation = _unitOfWork.DOD_Affiliation.List().Select(d => new SelectListItem { Value = d.DODAffiliationID.ToString(), Text = d.DODAffiliationType });

            lstReservationStatus = _unitOfWork.Reservation_Status.List().Select(s => new SelectListItem { Value = s.ResStatusID.ToString(), Text = s.ResStatusName });

            if(CustomerInfo.DODAffiliationID == 1)
            {
                CustomerInfo.DOD_Affiliation.Equals("Army");
            }
            else if(CustomerInfo.DODAffiliationID == 2)
            {
                CustomerInfo.DOD_Affiliation.Equals("Air Force");
            }
            else if (CustomerInfo.DODAffiliationID == 3)
            {
                CustomerInfo.DOD_Affiliation.Equals("Navy");
            }
            else if (CustomerInfo.DODAffiliationID == 4)
            {
                CustomerInfo.DOD_Affiliation.Equals("Marines");
            }
            else if (CustomerInfo.DODAffiliationID == 5)
            {
                CustomerInfo.DOD_Affiliation.Equals("Coast Guard");
            }
            else if (CustomerInfo.DODAffiliationID == 11)
            {
                CustomerInfo.DOD_Affiliation.Equals("Space Force");
            }

            if(CustomerInfo.ServiceStatusID == 1)
            {
                CustomerInfo.Service_Status_Type.Equals("Active");
            }
            else if(CustomerInfo.ServiceStatusID == 2)
            {
                CustomerInfo.Service_Status_Type.Equals("Retired");
            }
            else if (CustomerInfo.ServiceStatusID == 3)
            {
                CustomerInfo.Service_Status_Type.Equals("Reserves");
            }
            else if (CustomerInfo.ServiceStatusID == 4)
            {
                CustomerInfo.Service_Status_Type.Equals("PCS");
            }
            else if (CustomerInfo.ServiceStatusID == 9)
            {
                CustomerInfo.Service_Status_Type.Equals("Civillian");
            }

            
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
