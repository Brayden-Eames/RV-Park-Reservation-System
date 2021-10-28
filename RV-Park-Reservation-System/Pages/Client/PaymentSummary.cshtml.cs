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

namespace RV_Park_Reservation_System.Pages.Client
{
    public class PaymentSummaryModel : PageModel
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<Customer> _userManager;

        public PaymentSummaryModel(IUnitOfWork unitOfWork, UserManager<Customer> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;

        }

        
        public Reservation newReservation { get; set; }


        public Payment paymentObj { get; set; }

        [BindProperty]
        public int reservationID { get; set; }

        [BindProperty]
        public int paymentID { get; set; }

        public bool Error { get; set; } = false;


        public void OnGet(int? resID, int? payID)
        {
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



        }

        public IActionResult OnPost()
        {
            Payment thisPayment = _unitOfWork.Payment.Get(p => p.ResID == reservationID);









            return RedirectToPage("/PaymentSummary");

        }
    }
}
