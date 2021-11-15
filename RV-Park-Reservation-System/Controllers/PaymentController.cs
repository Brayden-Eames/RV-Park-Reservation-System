using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RV_Park_Reservation_System.Pages.Client;
using RV_Park_Reservation_System.ViewModels;
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
        public ActionResult Get()
        {
            if (HttpContext.Session.Get<ReservationVM>(SD.ReservationSession) != null )
            {

                ReservationVM reservationVM = new ReservationVM();
                reservationVM = HttpContext.Session.Get<ReservationVM>(SD.ReservationSession);
                Payment paymentObj = reservationVM.paymentObj;
                ApplicationCore.Models.Customer modelCustomer = _unitOfWork.Customer.Get(c => c.Email == User.Identity.Name);
                if (reservationVM.paymentObj.CCReference == null )
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
                    reservationVM.paymentObj = paymentObj;
                    HttpContext.Session.Set(SD.ReservationSession, reservationVM);

                    var intent = new Stripe.PaymentIntentService();
                    var payment = intent.Get(paymentIntent.Id);
                    return Json(new { client_secret = payment.ClientSecret });
                }
                else
                {
                    var intent = new Stripe.PaymentIntentService();
                    var payment = intent.Get(reservationVM.paymentObj.CCReference);
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
