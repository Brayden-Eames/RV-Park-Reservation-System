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

                ApplicationCore.Models.Customer customer = _unitOfWork.Customer.Get(c => c.CustEmail == User.Identity.Name); //Gets User From Customer Email
                


                //If User Selects more than 30 days
                if ((EndDate - StartDate).TotalDays >= 30)
                {


                    DateTime endOfMonthStartDate = new DateTime(StartDate.Year,
                                                       StartDate.Month,
                                                       DateTime.DaysInMonth(StartDate.Year,
                                                                            StartDate.Month));
                    DateTime nextMonthStartDate = new DateTime();
                    if (StartDate.Month == 12)
                    {
                        nextMonthStartDate = new DateTime(StartDate.Year +1, 1, 1);
                    }
                    else
                    {
                        nextMonthStartDate = new DateTime(StartDate.Year,StartDate.Month + 1,1); 
                                                                             
                    }

                    DateTime startOfMonthEndDate = new DateTime(EndDate.Year,
                                                                EndDate.Month, 1);
                    var totalDayDiff = Math.Ceiling((endOfMonthStartDate - StartDate).TotalDays) + 1 + Math.Floor((EndDate - startOfMonthEndDate).TotalDays);
                    var monthDiff = getMonthDifference(nextMonthStartDate, startOfMonthEndDate);

                    longTermReservationVM longTermReservationVM = new longTermReservationVM()
                    {
                        reservationObj = new Reservation(),
                        paymentObj = new Payment[monthDiff + 2],

                    };

                    int resid = await createReservation(StartDate, EndDate);
                    reservationVM.reservationObj = _unitOfWork.Reservation.Get(r => r.ResID == resid);
                    reservationVM.reservationObj.ResStatusID = 10;
                    _unitOfWork.Reservation.Update(reservationVM.reservationObj);
                    await _unitOfWork.CommitAsync();
                    if (vehicleType != 7)
                    {
                        decimal startCost = (decimal)(Math.Ceiling((endOfMonthStartDate - StartDate).TotalDays) + 1) * 25;
                        
                        int startpayment = await createPayment(startCost, resid);

                        reservationVM.paymentObj = _unitOfWork.Payment.Get(p => p.PayID == startpayment);


                        //Sets the Reservation view model in a session. 
                        HttpContext.Session.Set(SD.ReservationSession, reservationVM);
                        longTermReservationVM.reservationObj = reservationVM.reservationObj;
                        longTermReservationVM.paymentObj[0] = reservationVM.paymentObj;


                        if ((EndDate - startOfMonthEndDate).TotalDays * 25 > 700)
                        {
                            monthDiff++;
                            startOfMonthEndDate = EndDate;
                        }
                        for (int i = 0; i < monthDiff; i++)
                        {
                            int payID = await createPayment(700, resid);
                            longTermReservationVM.paymentObj[i + 1] = _unitOfWork.Payment.Get(p => p.PayID == payID);
                        }
                       /* if (monthDiff >=1)
                        {

                            for (int i = 0; i < monthDiff; i++)
                            {
                                DateTime startOfMonth = nextMonthStartDate;
                                DateTime endOfMonth = new DateTime();
                                if (startOfMonth.Month == 12)
                                {
                                    nextMonthStartDate = new DateTime(startOfMonth.Year + 1, 1, 1);
                                    endOfMonth = new DateTime(startOfMonth.Year +1, startOfMonth.Month , DateTime.DaysInMonth(startOfMonth.Year, startOfMonth.Month ));
                                }
                                else
                                {
                                    nextMonthStartDate = new DateTime(startOfMonth.Year, startOfMonth.Month + 1, 1);
                                    endOfMonth = new DateTime(startOfMonth.Year, startOfMonth.Month, DateTime.DaysInMonth(startOfMonth.Year, startOfMonth.Month));
                                }
                                int monthResId = await createReservation(startOfMonth, endOfMonth, 700);
                                longTermReservationVM.reservationObj[1 + i] = _unitOfWork.Reservation.Get(r=>r.ResID == monthResId);
                                longTermReservationVM.paymentObj[1 + i] = _unitOfWork.Payment.Get(p=> p.ResID == monthResId);


                            }
                        }*/
                        if ((EndDate - startOfMonthEndDate).TotalDays != 0)
                        {
                            decimal endCost = (decimal)(Math.Floor((EndDate - startOfMonthEndDate).TotalDays)) * 25;
                            int payid = await createPayment(endCost, resid);
                            
                            longTermReservationVM.paymentObj[longTermReservationVM.paymentObj.Count()-1] = _unitOfWork.Payment.Get(p => p.PayID == payid);

                        }

                        
                        
                        //Sets the Reservation view model in a session. 
                        HttpContext.Session.Set(SD.LongTermReservationSession, longTermReservationVM);
                    }
                    else
                    {
                        decimal cost = (decimal)(Math.Round((EndDate - StartDate).TotalDays) * 17);
                        int payid = await createPayment(cost, resid);
                        reservationVM.reservationObj = _unitOfWork.Reservation.Get(r => r.ResID == resid);
                        reservationVM.paymentObj = _unitOfWork.Payment.Get(p => p.PayID == payid);


                        //Sets the Reservation view model in a session. 
                        HttpContext.Session.Set(SD.ReservationSession, reservationVM);


                    }


                    //Resirects to the payment page for user to make the payment and review the information. 
                    return RedirectToPage("/Client/PaymentSummary");









                }
                else
                {
                    //Creates the reservation object based on the user input. 
                    int resid = await createReservation(StartDate, EndDate);
                    reservationVM.reservationObj = _unitOfWork.Reservation.Get(r=> r.ResID == resid);

                    //Calculates the total cost based on the type of vehicle and amount of days. 
                    if (reservationVM.reservationObj.TypeID == 7)//Type 7 is the tent space. 
                    {
                        reservationVM.paymentObj.PayTotalCost = (decimal)(Math.Round((reservationVM.reservationObj.ResEndDate - reservationVM.reservationObj.ResStartDate).TotalDays) * 17);
                    }
                    else
                    {
                        reservationVM.paymentObj.PayTotalCost = (decimal)(Math.Round((reservationVM.reservationObj.ResEndDate - reservationVM.reservationObj.ResStartDate).TotalDays) * 25);

                    }
                    int payid = await createPayment(reservationVM.paymentObj.PayTotalCost, resid);

                    reservationVM.paymentObj = _unitOfWork.Payment.Get(p => p.PayID == payid);
                    
                    //Sets the Reservation view model in a session. 
                    HttpContext.Session.Set(SD.ReservationSession, reservationVM);


                    //Resirects to the payment page for user to make the payment and review the information. 
                    return RedirectToPage("/Client/PaymentSummary");
                }


                
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

        public static int getMonthDifference(DateTime startDate, DateTime endDate)
        {
            int monthsApart = 12 * (startDate.Year - endDate.Year) + startDate.Month - endDate.Month;
            return Math.Abs(monthsApart);
        }


        public async Task<int> createReservation(DateTime startDate, DateTime endDate)
        {
            Reservation Res = new Reservation();
            ApplicationCore.Models.Customer customer = _unitOfWork.Customer.Get(c => c.CustEmail == User.Identity.Name);


            Res.ResAcknowledgeValidPets = breedPolicy;
            Res.ResStartDate = startDate;
            Res.ResEndDate = endDate;
            Res.ResNumAdults = numberOfAdults;
            Res.ResNumChildren = numberOfChildren;
            Res.ResNumPets = numberOfPets;
            Res.TypeID = vehicleType;
            Res.ResCreatedDate = DateTime.Now;
            Res.SiteID = siteid;
            Res.ResStatusID = 1;
            Res.ResLastModifiedBy = User.Identity.Name;
            Res.ResVehicleLength = vehicleLength;
            Res.Id = customer.Id;
            _unitOfWork.Reservation.Add(Res);
            _unitOfWork.Commit();

            //Creates the payment object based on the user input. 
            return Res.ResID;
        }
        public async Task<int> createPayment(decimal cost, int resID)
        {
            Payment ResPayment = new Payment();

            ResPayment.PayDate = DateTime.Now;
            ResPayment.PayLastModifiedBy = User.Identity.Name;
            ResPayment.PayLastModifiedDate = DateTime.Now;
            ResPayment.PayReasonID = 1;
            ResPayment.PayTypeID = 1;
            ResPayment.IsPaid = false;
            ResPayment.PayTotalCost = cost;
            ResPayment.ResID = resID;
            if (ResPayment.CCReference == null)
            {
                //Creates a payment intent through stripes api service. 
                var options = new PaymentIntentCreateOptions
                {
                    Amount = Convert.ToInt32(ResPayment.PayTotalCost * 100),
                    Currency = "usd",

                    PaymentMethodTypes = new List<string>
                            {
                              "card",
                            },
                };

                var service = new PaymentIntentService();
                var paymentIntent = service.Create(options);
                ResPayment.CCReference = paymentIntent.Id;
            }
            _unitOfWork.Payment.Add(ResPayment);
            await _unitOfWork.CommitAsync();
            return ResPayment.PayID;
        }
    }
}
