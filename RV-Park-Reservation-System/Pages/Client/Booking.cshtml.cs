using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;

using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using RV_Park_Reservation_System.ViewModels;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using Stripe;

namespace RV_Park_Reservation_System.Pages.Client
{
    public static class SessionExtensions
    {

        //Provides a way to serialize objects to be stored in sessions 
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        //Retrieves the serialized objects from the session and de-serializes the object. 
        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }

    public class BookingModel : PageModel
    {

        #region injectedServices
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationCore.Models.Customer> _userManager;

        public BookingModel(IUnitOfWork unitOfWork, UserManager<ApplicationCore.Models.Customer> userManager) 
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;

        }
        #endregion

        #region properties
        public IEnumerable<SelectListItem> vehicleTypes { get; set; }
        public IEnumerable<SelectListItem> sites { get; set; }
        [BindProperty]
        public int vehicleType { get; set; }
        public List<Special_Event> specialEvents { get; set; }
        public string jsonFeed { get; set; }
        [BindProperty]
        public int numberOfAdults { get; set; }
        [BindProperty]
        [Range(0, 10)]
        public int numberOfChildren { get; set; }
        [BindProperty]
        [Range(0, 2)]
        public int numberOfPets { get; set; }
        [BindProperty]
        public bool breedPolicy { get; set; }
        [BindProperty]
        public int vehicleLength { get; set; }
        [BindProperty]
        public DateTime StartDate { get; set; }
        [BindProperty]
        public DateTime EndDate { get; set; }
        [BindProperty]
        public int siteid { get; set; }
        [BindProperty]
        public bool Error { get; set; } = false;
        [BindProperty]
        public decimal totalCost { get; set; }
        public Reservation newReservation { get; set; }
        public ReservationVM reservationVM { get; set; }
        #endregion

        public IActionResult OnGet(bool? error)
        {
            //Checks if user is logged in and is a customer. 
            if (!User.Identity.IsAuthenticated && !User.IsInRole("Customer"))
            {
                return RedirectToPage("/Shared/Prohibited", new { path = "/Client/Booking" });
            }

            //Checks if prevoius page sent an error. 
            if (error != null)
            {
                Error = (bool)error;
            }

            //Gets vehicle types and sites for the dropdown lists. 
            vehicleTypes = _unitOfWork.Vehicle_Type.List().Select(f => new SelectListItem { Value = f.TypeID.ToString(), Text = f.TypeName });

            sites = _unitOfWork.Site.List().Select(f => new SelectListItem { Value = f.SiteID.ToString(), Text = "Lot " + f.SiteID.ToString() });


            //Calls DeletePendingReservations to delete any reservation older than 15 minutes that has not been paid for. 
            deletePendingReservations();


            return Page();
        }


        public async Task<IActionResult> OnPost()
        {

            //Creates a Reservation view model for the reservation object and the payment object.
            reservationVM = new ReservationVM()
            {
                reservationObj = new Reservation(),
                paymentObj = new Payment()
            };


            //Checks if user input is valid. 
            if (ModelState.IsValid)
            {
                //Appends the reservation start time and end time to the start and end dates. 
                TimeSpan startTime = new TimeSpan(13, 0, 0);
                StartDate = StartDate.Date + startTime;
                TimeSpan EndTime = new TimeSpan(12, 0, 0);
                EndDate = EndDate.Date + EndTime;

                
                //Creates the reservation object based on the user input. 
                reservationVM.reservationObj.ResAcknowledgeValidPets = breedPolicy;
                reservationVM.reservationObj.ResStartDate = StartDate;
                reservationVM.reservationObj.ResEndDate = EndDate;
                reservationVM.reservationObj.ResNumAdults = numberOfAdults;
                reservationVM.reservationObj.ResNumChildren = numberOfChildren;
                reservationVM.reservationObj.ResNumPets = numberOfPets;
                reservationVM.reservationObj.TypeID = vehicleType;
                reservationVM.reservationObj.ResCreatedDate = DateTime.Now;
                reservationVM.reservationObj.SiteID = siteid;
                reservationVM.reservationObj.ResStatusID = 1;
                reservationVM.reservationObj.ResLastModifiedBy = User.Identity.Name;
                reservationVM.reservationObj.ResVehicleLength = vehicleLength;
                ApplicationCore.Models.Customer customer = _unitOfWork.Customer.Get(c => c.CustEmail == User.Identity.Name);
                reservationVM.reservationObj.Id = customer.Id;
                _unitOfWork.Reservation.Add(reservationVM.reservationObj);
                _unitOfWork.Commit();

                
                //Creates the payment object based on the user input. 
                reservationVM.paymentObj.PayDate = DateTime.Now;
                reservationVM.paymentObj.PayLastModifiedBy = User.Identity.Name;
                reservationVM.paymentObj.PayLastModifiedDate = DateTime.Now;
                reservationVM.paymentObj.PayReasonID = 1;
                reservationVM.paymentObj.PayTypeID = 1;
                reservationVM.paymentObj.IsPaid = false;

                //Calculates the total cost based on the type of vehicle and amount of days. 
                if (reservationVM.reservationObj.TypeID == 7)//Type 7 is the tent space. 
                {
                    reservationVM.paymentObj.PayTotalCost = (decimal)(Math.Round((reservationVM.reservationObj.ResEndDate - reservationVM.reservationObj.ResStartDate).TotalDays) * 17);
                }
                else
                {
                    reservationVM.paymentObj.PayTotalCost = (decimal)(Math.Round((reservationVM.reservationObj.ResEndDate - reservationVM.reservationObj.ResStartDate).TotalDays) * 25);

                }

                //Checks if a payment reference already exist or not. 
                if (reservationVM.paymentObj.CCReference == null)
                {
                    //Creates a payment intent through stripes api service. 
                    var options = new PaymentIntentCreateOptions
                    {
                        Amount = Convert.ToInt32(reservationVM.paymentObj.PayTotalCost * 100),
                        Currency = "usd",

                        PaymentMethodTypes = new List<string>
                        {
                          "card",
                        },
                    };

                    var service = new PaymentIntentService();
                    var paymentIntent = service.Create(options);
                    reservationVM.paymentObj.CCReference = paymentIntent.Id;
                }

                //Sets the Reservation view model in a session. 
                HttpContext.Session.Set(SD.ReservationSession, reservationVM);

                
                //Resirects to the payment page for user to make the payment and review the information. 
                return RedirectToPage("/Client/PaymentSummary");
            }
            else
            {
                //Shows that some error occurred and redirect to the current page but sets error to true. 
                Error = true;
                return RedirectToPage("/Client/Booking", new { error = Error});
            }
        }       


        public void deletePendingReservations()
        {
            //Gets all reservations and deletes the reservations that are pending and greater than 15 minutes old. 
            var reservations = _unitOfWork.Reservation.List();
            reservations = reservations.Where(r => r.ResStatusID == 1 && (DateTime.Now - r.ResCreatedDate).TotalMinutes > 15);
            _unitOfWork.Reservation.Delete(reservations);
        }

    }
}
