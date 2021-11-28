using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using RV_Park_Reservation_System.Pages.Admin.Reservations;
using RV_Park_Reservation_System.ViewModels;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RV_Park_Reservation_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class adminReservationUpdateController : Controller
    {
        #region injectedServices
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOptions<Infrastructure.Services.StripeSettings> _stripe;
        private readonly UserManager<ApplicationCore.Models.Customer> _userManager;
        private readonly IEmailSender _emailSender;

        public adminReservationUpdateController(IUnitOfWork unitOfWork, IOptions<Infrastructure.Services.StripeSettings> stripe, UserManager<ApplicationCore.Models.Customer> userManager, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _stripe = stripe;
            _userManager = userManager;
            _emailSender = emailSender;
        }
        #endregion

        //[HttpGet]
        //commenting out for testing purposes, as this relates to showing the customer only their index page of just their scheduled reservations. 
        //public IActionResult Index()
        //{
        //    //var customer = _unitOfWork.Customer.Get(c => c.CustEmail == User.Identity.Name); 
        //    //return Json(new { data = _unitOfWork.Reservation.List().Where(c => c.Customer == customer && c.ResStatusID == 9) });
        //}


        //Process the refund amount and updates the selected reservation to canceled. 
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            string checkOption;
            if (HttpContext == null) 
            {
                checkOption = "cancel";
            }
            else
            {
                checkOption = "update";
            }

            //Gets the payment and reservation object from the Database.
            var objFromDb = await _unitOfWork.Reservation.GetAsync(c => c.ResID == id);
            var payObj = await _unitOfWork.Payment.GetAsync(p => p.ResID == id);
            var custObj = await _unitOfWork.Customer.GetAsync(d => d.Id == objFromDb.Id);

            //Checks if the user paid or not. 
            if (payObj.IsPaid == false)
            {
                return Json(new { success = false, message = "Reservation is not paid." });
            }

            //Gets the stripe key and payment intent for the selected user. 
            StripeConfiguration.ApiKey = _stripe.Value.SecretKey;
            var intent = new Stripe.PaymentIntentService();
            var payment = intent.Get(payObj.CCReference);

            //Calculates the refund amount based on the policy. 
            int refundAmount = 0;
            if ((objFromDb.ResStartDate - DateTime.Now).TotalDays >= 4)
            {
                refundAmount = (int)(payObj.PayTotalCost * 100) - 1000;
            }
            else if ((objFromDb.ResStartDate - DateTime.Now).TotalDays < 4)
            {
                refundAmount = (int)(payObj.PayTotalCost * 100) - 2500;
            }

            //Checks if user is cancelling mid reservation and calculates refund based on remaining days. 
            if ((objFromDb.ResStartDate - DateTime.Now).TotalDays < 0)
            {
                int daysLeft = (int)Math.Round((DateTime.Now - objFromDb.ResStartDate).TotalDays);

                int totalRefund = daysLeft * 2500;
                refundAmount = refundAmount - totalRefund;

            }

            //Checks if user is cancelling on a special event. 
            var specialEvents = _unitOfWork.Special_Event.List();
            foreach (var events in specialEvents)
            {
                if ((objFromDb.ResStartDate <= events.EventStartDate && objFromDb.ResEndDate >= events.EventStartDate)
                                     || (objFromDb.ResStartDate <= events.EventEndDate && objFromDb.ResEndDate >= events.EventEndDate) ||
                                     ((objFromDb.ResStartDate > events.EventStartDate && objFromDb.ResEndDate > events.EventStartDate)
                                     && (objFromDb.ResStartDate < events.EventEndDate && objFromDb.ResEndDate < events.EventEndDate)))
                {
                    refundAmount = (int)(payObj.PayTotalCost * 100) - 2500;
                }
            }

            //Creates the refund object using stripe and adds the refund amount. 
            if (refundAmount != 0)
            {
                var refunds = new RefundService();
                var refundOptions = new RefundCreateOptions
                {
                    Charge = payment.Charges.Data[0].Id, //within data at this position we have an attribute called 'AmountRefunded', which is 5000, meaning 2500 is left. 
                    Amount = refundAmount, 
                };
                //payment.Charges.Data[0].AmountRefunded = 0; //testing purposes
                var refund = refunds.Create(refundOptions);
            }


            //Updates the payment and reservation objects based on the refund. 
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting!" });
            }
            objFromDb.ResStatusID = 2;
            _unitOfWork.Reservation.Update(objFromDb);
            _unitOfWork.Commit();
            Payment refunded = new Payment();
            refunded = payObj;
            refunded.PayID = 0;
            refunded.PayTotalCost = 0 - (refundAmount / 100);
            refunded.PayReasonID = 4;
            _unitOfWork.Payment.Add(refunded);
            _unitOfWork.Commit();


            //Sends email to user to confirm cancellation. 
            var user = await _userManager.FindByEmailAsync(custObj.CustEmail); //modify to handle finding the user based on the res id. ***STILL NEEDS TO BE FIXED***
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Client/MyReservations",
                pageHandler: null,
                values: new { area = "", code },
                protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(
                user.CustEmail,
                "FamCamp Reservation Cancel Confirmation",
                $"This is a confirmation that your reservation is canceled and refunded. ");

            if(checkOption == "cancel")
            {
               
                return Json(new { success = true, message = "Cancel Successful" }); //need to change this to redirectToPage(/index) for returning to the grid.
            }
            return Json(new { }); //need to change this to redirectToPage(/index) for returning to the grid.
            
            
        }
    }
}

