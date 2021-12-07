using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using PasswordGenerator;
using RV_Park_Reservation_System.ViewModels;
using Stripe;

namespace RV_Park_Reservation_System.Pages.Admin.Reservations
{
    public class MakeCardPaymentModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationCore.Models.Customer> _userManager;
        private readonly IEmailSender _emailSender;

        public MakeCardPaymentModel(IUnitOfWork unitOfWork, UserManager<ApplicationCore.Models.Customer> userManager, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _emailSender = emailSender;

        }


        [BindProperty]
        public Reservation newReservation { get; set; }

        [BindProperty]
        public Payment paymentObj { get; set; }

        [BindProperty]
        public ApplicationCore.Models.Customer customerObj { get; set; }

        [BindProperty]
        public int reservationID { get; set; }

        [BindProperty]
        public int paymentID { get; set; }

        [BindProperty]
        public string vehicleType { get; set; }

        public bool Error { get; set; } = false;

        public string ReturnUrl { get; set; }

        public ReservationVM reservationVM { get; set; }
        public longTermReservationVM longTermReservationVM { get; private set; }
        public decimal totalAmount { get; private set; }

        public IActionResult OnGet(bool? error, string returnUrl = null)
        {

            //Checks if user is logged in and a customer. 
            if (!User.Identity.IsAuthenticated && !User.IsInRole("Customer"))
            {
                return RedirectToPage("/Shared/Prohibited", new { path = "/Client/PaymentSummary" });
            }

            //Checks if the session is null and returns an error if the session is null.    
            if (HttpContext.Session.Get<longTermReservationVM>(SD.LongTermReservationSession) != null)
            {
                longTermReservationVM = HttpContext.Session.Get<longTermReservationVM>(SD.LongTermReservationSession);
                for (int i = 0; i < longTermReservationVM.paymentObj.Count(); i++)
                {
                    totalAmount += longTermReservationVM.paymentObj[i].PayTotalCost;
                }
            }
            if (HttpContext.Session.Get<ReservationVM>(SD.ReservationSession) != null)
            {
                //Sets the error state. 
                if (error != null)
                {
                    Error = true;
                }
                //Retrieves reservation view model from the session. 
                reservationVM = HttpContext.Session.Get<ReservationVM>(SD.ReservationSession);

                //Sets the reservation and payment objects from the session. 
                newReservation = reservationVM.reservationObj;
                paymentObj = reservationVM.paymentObj;

                //Gets the vehicle type for the summary page. 
                vehicleType = _unitOfWork.Vehicle_Type.Get(v => v.TypeID == newReservation.TypeID).TypeName;


            }
            else
            {
                //Session is empty. 
                Error = true;

            }
            return Page();
        }
        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            //Gets reservationVM from session and sets it to reservationVM
            if (HttpContext.Session.Get<ReservationVM>(SD.ReservationSession) != null)
            {
                reservationVM = HttpContext.Session.Get<ReservationVM>(SD.ReservationSession);
            }


            //Creates a payment intent service though stripes api. 
            var service = new PaymentIntentService();
            PaymentIntent paymentIntent;


            //"Pings" the selected payment intent until stripe updates it to succeeded and payment is fully processed or some error occurs. 
            for (int i = 0; i < 5; i++)
            {
                //Waits one second between iterations. 
                System.Threading.Thread.Sleep(1000);

                //Gets the payment intent
                paymentIntent = service.Get(reservationVM.paymentObj.CCReference);

                //checks if payment intent succeed and addes it to the payment table in database. 
                if (paymentIntent.Status.ToLower() == "succeeded")
                {


                    //Finalizes the payment object. 


                    reservationVM.paymentObj.IsPaid = true;
                    _unitOfWork.Payment.Update(reservationVM.paymentObj);
                    _unitOfWork.Commit();

                    //Updates the reservation to a scheduled status. 
                    if (reservationVM.reservationObj.ResStatusID == 10)
                    {
                        reservationVM.reservationObj.ResStatusID = 11;
                        _unitOfWork.Reservation.Update(reservationVM.reservationObj);
                        _unitOfWork.Commit();
                    }
                    else if (reservationVM.reservationObj.ResStatusID == 1)
                    {
                        reservationVM.reservationObj.ResStatusID = 9;
                        _unitOfWork.Reservation.Update(reservationVM.reservationObj);
                        _unitOfWork.Commit();
                    }



                    //Clears the session. 
                    HttpContext.Session.Clear();

                    //Sends an email to the user with confirmation code. 
                    var customer = await _unitOfWork.Customer.GetAsync(c => c.Id == reservationVM.reservationObj.Id);
                    var user = await _userManager.FindByEmailAsync(customer.Email);
                    var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Client/MyReservations",
                        pageHandler: null,
                        values: new { area = "", code },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(
                        user.CustEmail,
                        "FamCamp Reservation Confirmation",
                        $"This is a confirmation that your reservation is confirmed and paid " + reservationVM.paymentObj.PayTotalCost + ". to view this reservation please visit the MyReservation page under your account. " +
                        $"Your reservation confirmation number is " + reservationVM.reservationObj.ResID.ToString() + "."
                        );


                    //Returns to confirm payment. 
                    return RedirectToPage("/Client/PaymentConfirmation");
                }


            }
            //Some error occured and payment was not completed. Return to current page and display error message. 
            return RedirectToPage("/Admin/Reservations/PaymentSummary", new { error = true });
        }
    }
}
