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
            
            StripeConfiguration.ApiKey = _stripe.Value.SecretKey;

            var refunds = new RefundService();
            var refundOptions = new RefundCreateOptions
            {
                Charge = payObj.CCReference,
                /*Amount = (payObj.PayTotalCost * 100)*/
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
