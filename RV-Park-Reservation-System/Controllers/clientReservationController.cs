using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Stripe;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RV_Park_Reservation_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class clientReservationController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOptions<Infrastructure.Services.StripeSettings> _stripe;
        private readonly UserManager<ApplicationCore.Models.Customer> _userManager;
        private readonly IEmailSender _emailSender;



        public clientReservationController(IUnitOfWork unitOfWork, IOptions<Infrastructure.Services.StripeSettings> stripe, UserManager<ApplicationCore.Models.Customer> userManager, IEmailSender emailSender) {
            _unitOfWork = unitOfWork;
            _stripe = stripe;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var customer = _unitOfWork.Customer.Get(c => c.CustEmail == User.Identity.Name);
            return Json(new { data = _unitOfWork.Reservation.List().Where(c=>c.Customer == customer  /*Removed this if we want to show all reservations->*/&& c.ResStatusID == 9) });
        }



        [HttpDelete("{id}")]
        public async  Task<IActionResult> Delete(int id)
        {
            var objFromDb = _unitOfWork.Reservation.Get(c => c.ResID == id);
            var payObj = _unitOfWork.Payment.Get(p => p.ResID == id);
            if (payObj.IsPaid == false)
            {
                return Json(new { success = false, message = "Reservation is not paid." });
            }
            StripeConfiguration.ApiKey = _stripe.Value.SecretKey;

            var intent = new Stripe.PaymentIntentService();
            var payment = intent.Get(payObj.CCReference);
            int refundAmount = 0 ;

            if ((objFromDb.ResStartDate - DateTime.Now).TotalDays >= 4)
            {
                refundAmount = (int)(payObj.PayTotalCost * 100) - 1000;
            }
            else if ((objFromDb.ResStartDate -DateTime.Now  ).TotalDays < 4)
            {
                refundAmount = (int)(payObj.PayTotalCost * 100) - 2500;
            }
            var specialEvents = _unitOfWork.Special_Event.List();
            
            foreach (var events in specialEvents )
            {
                if ((objFromDb.ResStartDate <= events.EventStartDate && objFromDb.ResEndDate >= events.EventStartDate)
                                     || (objFromDb.ResStartDate <= events.EventEndDate && objFromDb.ResEndDate >= events.EventEndDate) ||
                                     ((objFromDb.ResStartDate > events.EventStartDate && objFromDb.ResEndDate > events.EventStartDate) 
                                     && (objFromDb.ResStartDate < events.EventEndDate && objFromDb.ResEndDate < events.EventEndDate)))
                {
                    refundAmount = (int)(payObj.PayTotalCost * 100) - 2500;
                }
            }
          

            var refunds = new RefundService();
            var refundOptions = new RefundCreateOptions
            {
                Charge = payment.Charges.Data[0].Id,
                Amount = refundAmount,
            };
            var refund = refunds.Create(refundOptions);

            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting!" });
            }
            _unitOfWork.Reservation.Delete(objFromDb);
            _unitOfWork.Commit();

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
                "FamCamp Reservation Cancel Confirmation",
                $"This is a confirmation that your reservation is canceled and refunded. ");


            return Json(new { success = true, message = "Delete Successful" });
        }
    }
}
