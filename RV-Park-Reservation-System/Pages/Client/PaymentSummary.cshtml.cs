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

        [BindProperty]
        public string  vehicleType { get; set; }

        public bool Error { get; set; } = false;


        public void OnGet(int? resID, int? payID)
        {
            if (resID != null)
            {
                reservationID = (int)resID;
                newReservation = _unitOfWork.Reservation.Get(r => r.ResID == resID);
                vehicleType = _unitOfWork.Vehicle_Type.Get(v => v.TypeID == newReservation.VehicleTypeID).TypeName;

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



        }

        public IActionResult OnPost()
        {
            if (paymentID != 0)
            {
                paymentObj = _unitOfWork.Payment.Get(p => p.PayID == paymentID);
            }

            var service = new PaymentIntentService();
            var paymentIntent =  service.Get(paymentObj.CCReference);


            if (paymentIntent.Status.ToLower() == "succeeded")
            {
                paymentObj.IsPaid = true;
                _unitOfWork.Payment.Update(paymentObj);
                _unitOfWork.Commit();
                return RedirectToPage("/Client/PaymentConfirmation");
            }
            else
            {
                paymentObj.IsPaid = false;

            }
            return RedirectToPage("/Client/PaymentConfirmation");




        }
    }
}
