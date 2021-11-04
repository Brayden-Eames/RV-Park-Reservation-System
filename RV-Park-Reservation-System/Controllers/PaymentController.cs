using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RV_Park_Reservation_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public PaymentController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        public ActionResult Get(int payID)
        {
            if (payID != 0)
            {
                int paymentID = (int)payID;
                Payment paymentObj = _unitOfWork.Payment.Get(p => p.PayID == paymentID);
                if (paymentObj.CCReference == null )
                {
                    var options = new PaymentIntentCreateOptions
                    {
                        Amount = Convert.ToInt32(paymentObj.PayTotalCost * 100),
                        Currency = "usd",
                        PaymentMethodTypes = new List<string>
                    {
                      "card",
                    },
                    };

                    var service = new PaymentIntentService();
                    var paymentIntent = service.Create(options);
                    paymentObj.CCReference = paymentIntent.Id;
                    _unitOfWork.Payment.Update(paymentObj);
                    _unitOfWork.Commit();
                    var intent = new Stripe.PaymentIntentService();
                    var payment = intent.Get(paymentIntent.Id);
                    return Json(new { client_secret = payment.ClientSecret });
                }
                else
                {
                    var intent = new Stripe.PaymentIntentService();
                    var payment = intent.Get(paymentObj.CCReference);
                    return Json(new { client_secret = payment.ClientSecret });
                }

                

            }

            else
            {
                return Json(new { client_secret = "failure" });
            }
           
        }
    }
}
