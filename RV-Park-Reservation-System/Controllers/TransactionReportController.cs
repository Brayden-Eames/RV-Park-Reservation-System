using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RV_Park_Reservation_System.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RV_Park_Reservation_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionReportController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public TransactionReportController(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        [HttpGet]
        public IActionResult Get(string startDate, string endDate)
        {
            DateTime DTstartDate = DateTime.Parse(startDate);
            DateTime DTendDate = DateTime.Parse(endDate);
            var paymentList = _unitOfWork.Payment.List(p => p.PayDate >= DTstartDate && p.PayDate <= DTendDate);
            var payTypeList = _unitOfWork.Payment_Type.List();
            var payReasonList = _unitOfWork.Payment_Reason.List();
            var customerList = _unitOfWork.Customer.List();
            List <Reservation> reservationList = new List<Reservation>();

            foreach(Payment p in paymentList)
            {
                Reservation r = _unitOfWork.Reservation.Get(r => r.ResID == p.ResID);
                reservationList.Add(r);
            }

            var transactionQuery = from payment in paymentList
                                   join payType in payTypeList on payment.PayTypeID equals payType.PayTypeID
                                   join payReason in payReasonList on payment.PayReasonID equals payReason.PayReasonID
                                   join res in reservationList on payment.ResID equals res.ResID
                                   into transaction
                                   from subItem in transaction.DefaultIfEmpty()
                                   select new
                                   {
                                       id = payment.PayID,
                                       date = payment.PayDate,
                                       total = payment.PayTotalCost,
                                       paid = payment.IsPaid,
                                       resID = payment.ResID,
                                       paymentType = payType.PayType,
                                       paymentReason = payReason.PayReasonName
                                   };

            var transactionList = new List<TransactionReportVM>();

            foreach(var t in transactionQuery)
            {
                TransactionReportVM transaction = new TransactionReportVM();
                transaction.id = t.id;
                transaction.date = t.date;
                transaction.total = t.total;
                if (t.paid)
                {
                    transaction.paid = "yes";
                }    
                else
                {
                    transaction.paid = "no";
                }
                transaction.resID = t.resID;
                transaction.paymentType = t.paymentType;
                transaction.paymentReason = t.paymentReason;

                transactionList.Add(transaction);
            }

            return Json(new { data = transactionList });
        }
    }
}
