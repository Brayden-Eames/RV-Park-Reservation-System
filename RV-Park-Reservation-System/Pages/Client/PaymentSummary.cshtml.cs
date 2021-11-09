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
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using RV_Park_Reservation_System.ViewModels;
using Stripe;

namespace RV_Park_Reservation_System.Pages.Client
{   
    public class PaymentSummaryModel : PageModel
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationCore.Models.Customer> _userManager;
        private readonly IEmailSender _emailSender;

        public PaymentSummaryModel(IUnitOfWork unitOfWork, UserManager<ApplicationCore.Models.Customer> userManager, IEmailSender emailSender)
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
        public string  vehicleType { get; set; }

        public bool Error { get; set; } = false;

        public ReservationVM reservationVM { get; set; }

        public IActionResult OnGet(bool? error)
        {

            if (!User.Identity.IsAuthenticated && !User.IsInRole("Customer"))
            {
                return RedirectToPage("/Shared/Prohibited", new { path = "/Client/PaymentSummary" });
            }

            if (HttpContext.Session.Get<ReservationVM>(SD.ReservationSession) != null)
            { if (error != null)
                {
                    Error = true;
                }
                reservationVM = HttpContext.Session.Get<ReservationVM>(SD.ReservationSession);

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
        public async Task<IActionResult> OnPost()
        {
            if (HttpContext.Session.Get<ReservationVM>(SD.ReservationSession) != null)
            {
               reservationVM = HttpContext.Session.Get<ReservationVM>(SD.ReservationSession);
            }

            System.Threading.Thread.Sleep(2000);
            var service = new PaymentIntentService();
            var paymentIntent =  service.Get(reservationVM.paymentObj.CCReference);


            if (paymentIntent.Status.ToLower() == "succeeded")
            {
                ApplicationCore.Models.Customer customer = _unitOfWork.Customer.Get(c => c.CustEmail == User.Identity.Name);
                reservationVM.reservationObj.Id = customer.Id;
                _unitOfWork.Reservation.Add(reservationVM.reservationObj);
                _unitOfWork.Commit();

                var reservations = _unitOfWork.Reservation.List().Where(r=> r.Customer == customer).Last();
                reservationVM.paymentObj.ResID = reservations.ResID;
                reservationVM.paymentObj.IsPaid = true;
                _unitOfWork.Payment.Add(reservationVM.paymentObj);
                _unitOfWork.Commit();
                HttpContext.Session.Clear();

                var user = await _userManager.FindByEmailAsync(User.Identity.Name);
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
                    $"This is a confirmation that your reservation is confirmed and paid in it's entirety. to view this reservation please visit the MyReservation page under your account.  " +
                    $" <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");



                return RedirectToPage("/Client/PaymentConfirmation");
            }
            else
            {
                return RedirectToPage("/Client/PaymentSummary", new { error = true });

            }





        }
    }
}
