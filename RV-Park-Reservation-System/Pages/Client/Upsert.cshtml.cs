using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RV_Park_Reservation_System.ViewModels;
using Stripe;

namespace RV_Park_Reservation_System.Pages.Client
{
    public class UpsertModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        //private readonly UserManager<ApplicationCore.Models.Customer> _userManager;

        public UpsertModel(IUnitOfWork unitOfWork /*UserManager<ApplicationCore.Models.Customer> userManager*/)
        {
            _unitOfWork = unitOfWork;
            //_userManager = userManager;
        }

        //passing the 'ResID' in through the reservations.cshtml, we can then use that to go through unitofWork and retrieve that reservation, and by extension the customer's info

        [BindProperty]
        public Reservation CustomerReservation { get; set; }

        [BindProperty]
        public ApplicationCore.Models.Customer CustomerInfo { get; set; }

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

        public ReservationVM reservationVM { get; set; }

        public Vehicle_Type custVehicleType { get; set; }


        public async Task<IActionResult> OnGet(int? id)
        {

            CustomerReservation = await _unitOfWork.Reservation.GetAsync(c => c.ResID == id);  //Getting reservation with reservation id
            CustomerInfo = await _unitOfWork.Customer.GetAsync(c => c.Id == CustomerReservation.Id); // Getting customer details with reservation details
            CustomerPayment = await _unitOfWork.Payment.GetAsync(p => p.ResID == id); //Gettting Payment associated with reservation id

            sites = _unitOfWork.Site.List().Select(f => new SelectListItem { Value = f.SiteID.ToString(), Text = "Lot " + f.SiteID.ToString() });
            lstServiceStatus = _unitOfWork.Service_Status_Type.List().Select(s => new SelectListItem { Value = s.ServiceStatusID.ToString(), Text = s.ServiceStatusType });  //Getting Servives Status of customer 
            lstDODAffiliation = _unitOfWork.DOD_Affiliation.List().Select(d => new SelectListItem { Value = d.DODAffiliationID.ToString(), Text = d.DODAffiliationType });  //Getting DODAffiliationstomer 

            lstReservationStatus = _unitOfWork.Reservation_Status.List().Select(s => new SelectListItem { Value = s.ResStatusID.ToString(), Text = s.ResStatusName });

            custVehicleType = await _unitOfWork.Vehicle_Type.GetAsync(v => v.TypeID == CustomerReservation.TypeID);


            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string value, string updateType)
        {
            if (value == "update") //conditional for updating everything except for adding a new site and/or dates.
            {
                var reservation = await _unitOfWork.Reservation.GetAsync(c => c.ResID == CustomerReservation.ResID);
                var siteObj = await _unitOfWork.Site.GetAsync(s => s.SiteID == CustomerReservation.Site.SiteID);
               
                reservation.ResNumAdults = CustomerReservation.ResNumAdults;
                reservation.ResNumChildren = CustomerReservation.ResNumChildren;
                reservation.ResNumPets = CustomerReservation.ResNumPets;


                _unitOfWork.Reservation.Update(reservation);
                _unitOfWork.Commit();
                return RedirectToPage("./MyReservations", new { success = true, message = "Update Successful" });
            }
            else if (value == "addDaysUpdate") //conditional for adding new days/new site
            {
                var reservation = await _unitOfWork.Reservation.GetAsync(c => c.ResID == CustomerReservation.ResID);
                var siteObj = await _unitOfWork.Site.GetAsync(s => s.SiteID == siteid);
                var paymentObject = await _unitOfWork.Payment.GetAsync(p => p.ResID == CustomerReservation.ResID);
                var customerInfoObj = await _unitOfWork.Customer.GetAsync(c => c.CustFirstName == CustomerInfo.CustFirstName && c.CustLastName == CustomerInfo.CustLastName);

                reservationVM = new ReservationVM()
                {
                    reservationObj = reservation,
                    paymentObj = paymentObject,
                    customerObj = customerInfoObj
                };

                //Functionality is being displayed due to problem in implementing it.
                //reservationVM.reservationObj.Site = siteObj;
                //reservationVM.reservationObj.Site.SiteNumber = siteObj.SiteNumber;
                //reservationVM.reservationObj.ResStartDate = CustomerReservation.ResStartDate;
                //reservationVM.reservationObj.ResEndDate = CustomerReservation.ResEndDate;
                //reservationVM.reservationObj.ResNumAdults = CustomerReservation.ResNumAdults;
                //reservationVM.reservationObj.ResNumChildren = CustomerReservation.ResNumChildren;
                //reservationVM.reservationObj.ResNumPets = CustomerReservation.ResNumPets;
                //reservationVM.reservationObj.ResAcknowledgeValidPets = CustomerReservation.ResAcknowledgeValidPets;
                //reservationVM.reservationObj.ResComment = CustomerReservation.ResComment;
                //reservationVM.reservationObj.ResVehicleLength = CustomerReservation.ResVehicleLength;
                //reservationVM.reservationObj.Vehicle_Type = CustomerReservation.Vehicle_Type;
                //reservationVM.reservationObj.ResStatusID = reservationStatusID;



                //reservationVM.paymentObj.PayDate = DateTime.Now;
                //reservationVM.paymentObj.PayLastModifiedBy = User.Identity.Name;
                //reservationVM.paymentObj.PayLastModifiedDate = DateTime.Now;
                //reservationVM.paymentObj.PayReasonID = 1;
                //reservationVM.paymentObj.PayTypeID = 1;
                //reservationVM.paymentObj.IsPaid = false;
                //reservationVM.paymentObj.CCReference = null;

                //if (reservationVM.reservationObj.TypeID == 7)//Type 7 is the tent space. 
                //{
                //    reservationVM.paymentObj.PayTotalCost = (decimal)(Math.Round((reservationVM.reservationObj.ResEndDate - reservationVM.reservationObj.ResStartDate).TotalDays) * 17);
                //}
                //else
                //{
                //    reservationVM.paymentObj.PayTotalCost = (decimal)(Math.Round((reservationVM.reservationObj.ResEndDate - reservationVM.reservationObj.ResStartDate).TotalDays) * 25);

                //}


                //if (reservationVM.paymentObj.CCReference == null)
                //{
                //    //Creates a payment intent through stripes api service. 
                //    var options = new PaymentIntentCreateOptions
                //    {
                //        Amount = Convert.ToInt32(reservationVM.paymentObj.PayTotalCost * 100),
                //        Currency = "usd",

                //        PaymentMethodTypes = new List<string>
                //        {
                //          "card",
                //        },
                //    };

                //    var service = new PaymentIntentService();
                //    var paymentIntent = service.Create(options);
                //    reservationVM.paymentObj.CCReference = paymentIntent.Id;
                // }

                HttpContext.Session.Clear();

                HttpContext.Session.Set(SD.ReservationSession, reservationVM); //uses the ReservationSession string to handle this separate from regular sessions.

                return RedirectToPage("./UpsertPaymentSummary"); //redirect to the admin page
            }
            else
            {

                return RedirectToPage("./MyReservations", new { success = true, message = "Cancel Successful" });
            }
        }

    }
}




