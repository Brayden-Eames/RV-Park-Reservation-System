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
using RV_Park_Reservation_System.ViewModels;

namespace RV_Park_Reservation_System.Pages.Admin.Reservations
{
    public class MakeCashPaymentModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationCore.Models.Customer> _userManager;
        private readonly IEmailSender _emailSender;

        public MakeCashPaymentModel(IUnitOfWork unitOfWork, UserManager<ApplicationCore.Models.Customer> userManager, IEmailSender emailSender)
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

        public IActionResult OnGet(bool? error, string returnUrl = null)
        {

            if (!User.Identity.IsAuthenticated /*&& !User.IsInRole("Customer")*/) //removed check for customer role, cause the admin won't have that role by default. 
            {
                return RedirectToPage("/Shared/Prohibited", new { path = "/Admin/AdminPaymentSummary" });
            }

            if (HttpContext.Session.Get<ReservationVM>(SD.ReservationSession) != null)
            {
                if (error != null)
                {
                    Error = true;
                }
                reservationVM = HttpContext.Session.Get<ReservationVM>(SD.ReservationSession);

                newReservation = reservationVM.reservationObj;
                paymentObj = reservationVM.paymentObj;
                customerObj = reservationVM.customerObj;
                vehicleType = _unitOfWork.Vehicle_Type.Get(v => v.TypeID == newReservation.TypeID).TypeName;
                ReturnUrl = returnUrl;

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
            
            paymentObj = reservationVM.paymentObj;
            paymentObj.IsPaid = true;
            paymentObj.PayTypeID = 2;
            _unitOfWork.Payment.Update(paymentObj);
            await _unitOfWork.CommitAsync();

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
                $"This is a confirmation that your reservation is confirmed and paid "+ reservationVM.paymentObj.PayTotalCost + ". to view this reservation please visit the MyReservation page under your account.  " +
                $" <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            return RedirectToPage("/Client/PaymentConfirmation");
        }

    }
}
