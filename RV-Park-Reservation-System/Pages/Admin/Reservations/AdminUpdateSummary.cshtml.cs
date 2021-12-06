using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
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
    public class AdminUpdateSummaryModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationCore.Models.Customer> _userManager;
        private readonly IEmailSender _emailSender;

        public AdminUpdateSummaryModel(IUnitOfWork unitOfWork, UserManager<ApplicationCore.Models.Customer> userManager, IEmailSender emailSender)
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
        public int reservationID { get; set; }

        [BindProperty]
        public int paymentID { get; set; }

        [BindProperty]
        public string vehicleType { get; set; }

        public bool Error { get; set; } = false;

        public string ReturnUrl { get; set; }

        public ReservationVM reservationVM { get; set; }

       
        public IActionResult OnGet(bool? error, string returnUrl = null)
        {
           
            if (!User.Identity.IsAuthenticated /*&& !User.IsInRole("Customer")*/) //removed check for customer role, cause the admin won't have that role by default. 
            {
                return RedirectToPage("/Shared/Prohibited", new { path = "/Admin/AdminUpdateSummary" });
            }
            
            if (HttpContext.Session.Get<ReservationVM>(SD.ReservationSession) != null)
            {
                if (error != null)
                {
                    Error = true;
                }
                reservationVM = HttpContext.Session.Get<ReservationVM>(SD.ReservationSession);

                //Sets the reservation and payment objects from the session. 
                newReservation = reservationVM.reservationObj;              
                paymentObj = reservationVM.paymentObj;
                vehicleType = _unitOfWork.Vehicle_Type.Get(v => v.TypeID == newReservation.TypeID).TypeName;
            }
            else
            {
                Error = true;
            }
            return Page();
        }
        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            if (HttpContext.Session.Get<ReservationVM>(SD.ReservationSession) != null) //current problem: 
            {
                reservationVM = HttpContext.Session.Get<ReservationVM>(SD.ReservationSession);
            }
            //returnUrl ??= Url.Content("~/"); //null-coalescing assignment operator ??= assigns the value of right-hand operand to its left-hand operand only if the left-hand is nulll
            //System.Threading.Thread.Sleep(2000);

            var service = new PaymentIntentService();
            PaymentIntent paymentIntent;
            //newReservation = new Reservation();
            //paymentObj = new Payment();

            for (int i = 0; i < 5; i++)
            {

                System.Threading.Thread.Sleep(1000);

                paymentIntent = service.Get(reservationVM.paymentObj.CCReference);


                if (paymentIntent.Status.ToLower() == "succeeded")
                {

                    ApplicationCore.Models.Customer customer = _unitOfWork.Customer.Get(c => c.CustFirstName == reservationVM.customerObj.CustFirstName && c.CustLastName == reservationVM.customerObj.CustLastName && c.CustEmail == reservationVM.customerObj.CustEmail);

                    newReservation.ResID = 0;
                    //paymentObj.PayTypeID = reservationVM.paymentObj.PayTypeID;

                    _unitOfWork.Reservation.Add(newReservation);
                    await _unitOfWork.CommitAsync();

                    var reservations = _unitOfWork.Reservation.List().Where(r => r.Customer == customer).Last();
                    paymentObj.ResID = reservations.ResID;
                    paymentObj.IsPaid = true;
                    _unitOfWork.Payment.Add(paymentObj);
                    await _unitOfWork.CommitAsync();
                    HttpContext.Session.Clear();

                    // --- RESERVATION CONFIRMATION EMAIL --- //
                    var user = await _userManager.FindByEmailAsync(reservationVM.customerObj.Email);
                    var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Client/MyReservations",
                        pageHandler: null,
                        values: new { area = "", code },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(
                        user.Email,
                        "FamCamp Reservation Confirmation",
                        $"This is a confirmation that your reservation is confirmed and paid in it's entirety. to view this reservation please visit the MyReservation page under your account.  " +
                        $" <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");


                    return RedirectToPage("/Client/PaymentConfirmation");
                }
            }

                return RedirectToPage("/Admin/Reservations/AdminUpdateSummary", new { error = true });

            
        }
    }
}
