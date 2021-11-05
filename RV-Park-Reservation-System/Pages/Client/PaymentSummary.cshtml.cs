using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Stripe;

namespace RV_Park_Reservation_System.Pages.Client
{
    public class PaymentSummaryModel : PageModel
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationCore.Models.Customer> _userManager;

        public PaymentSummaryModel(IUnitOfWork unitOfWork, UserManager<ApplicationCore.Models.Customer> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;

        }

        [BindProperty]
        public Reservation newReservation { get; set; }

        [BindProperty]
        public Payment paymentObj { get; set; }

        [BindProperty]
        public int reservationID { get; set; }

        [BindProperty]
        public int paymentID { get; set; }

        public bool Error { get; set; } = false;


        public IActionResult OnGet(int? resID, int? payID)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Shared/Prohibited", new { path = "/Client/PaymentSummary" });
            }

            if (resID != null)
            {
                reservationID = (int)resID;
                newReservation = _unitOfWork.Reservation.Get(r => r.ResID == resID);
            }
            if (payID!= null)
            {
                paymentID = (int)payID;
                paymentObj = _unitOfWork.Payment.Get(p => p.PayID == paymentID);
            }
            else
            {

                Error = true;
            }


            return Page();
        }

        public IActionResult OnPost(string stripeToken)
        {
            if (paymentID != 0)
            {
                paymentObj = _unitOfWork.Payment.Get(p => p.PayID == paymentID);
            }
   
            if (stripeToken != null)
            {
                var options = new ChargeCreateOptions
                {
                    Amount = Convert.ToInt32(paymentObj.PayTotalCost*100),
                    Currency = "usd",
                    Description = "Order Id " + paymentObj.PayID,
                    Source = stripeToken,

                };
                var service = new ChargeService();
                Charge charge = service.Create(options);
                paymentObj.CCReference = charge.Id;
                if (charge.Status.ToLower() == "succeeded")
                {
                    paymentObj.IsPaid = true;
                    _unitOfWork.Payment.Update(paymentObj);
                    _unitOfWork.Commit();
                }
                else
                {
                    paymentObj.IsPaid = false;
                }

            }








            return RedirectToPage("/Client/PaymentConfirmation");

        }
    }
}
