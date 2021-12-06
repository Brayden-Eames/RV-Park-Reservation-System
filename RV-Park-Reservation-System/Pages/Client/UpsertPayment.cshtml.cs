using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RV_Park_Reservation_System.ViewModels;

namespace RV_Park_Reservation_System.Pages.Client
{
    public class UpsertPaymentModel : PageModel
    {
        #region InjectedServices
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<Customer> _userManager;

        public UpsertPaymentModel(IUnitOfWork unitOfWork, UserManager<Customer> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;

        }
        #endregion

        #region Properties

        public Payment payment { get; set; }

        public IEnumerable<Payment> payments { get; set; }
        [BindProperty]
        public int payID { get; set; }
        #endregion

        public void OnGet(int? id)
        {
            payments = _unitOfWork.Payment.List().Where(p => p.ResID == id);

        }

        public async Task<IActionResult> OnPost()
        {

            ReservationVM reservationVM = new ReservationVM()
            {
                reservationObj = new Reservation(),
                paymentObj = new Payment(),
               
            };

            
            reservationVM.paymentObj = _unitOfWork.Payment.Get(p => p.PayID == payID);
            reservationVM.reservationObj = _unitOfWork.Reservation.Get(r => r.ResID == reservationVM.paymentObj.ResID);
            var paymentList = _unitOfWork.Payment.List().Where(p => p.ResID == reservationVM.reservationObj.ResID).ToList().ToArray();
            longTermReservationVM longTermReservationVM = new longTermReservationVM()
            {
                reservationObj = reservationVM.reservationObj,
                paymentObj = paymentList,
                
            };


            HttpContext.Session.Set(SD.ReservationSession, reservationVM);
            HttpContext.Session.Set(SD.LongTermReservationSession, longTermReservationVM);
            return RedirectToPage("/Client/PaymentSummary");
        }
    }
}
