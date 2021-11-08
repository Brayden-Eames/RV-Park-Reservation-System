using ApplicationCore.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RV_Park_Reservation_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class clientReservationController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOptions<Infrastructure.Services.StripeSettings> _stripe;
        public clientReservationController(IUnitOfWork unitOfWork, IOptions<Infrastructure.Services.StripeSettings> stripe) {
            _unitOfWork = unitOfWork;
            _stripe = stripe;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var customer = _unitOfWork.Customer.Get(c => c.CustEmail == User.Identity.Name);
            return Json(new { data = _unitOfWork.Reservation.List().Where(c=>c.Customer == customer) });
        }



        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
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
            return Json(new { success = true, message = "Delete Successful" });
        }
    }
}
