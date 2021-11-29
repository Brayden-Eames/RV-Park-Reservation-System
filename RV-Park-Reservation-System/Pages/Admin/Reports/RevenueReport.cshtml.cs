using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RV_Park_Reservation_System.ViewModels;

namespace RV_Park_Reservation_System.Pages.Admin.Reports
{
    public class RevenueReportModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public RevenueReportModel(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        [BindProperty]
        public List<LineItemVM> IncomeList { get; set; }

        [BindProperty]
        public List<LineItemVM> RefundList { get; set; }

        [BindProperty]
        public List<LineItemVM> ChargeList { get; set; }

        [BindProperty]
        public List<LineItemVM> SummaryList { get; set; }

        [BindProperty]
        public DateTime startDate { get; set; }

        [BindProperty]
        public DateTime endDate { get; set; }

        public IActionResult OnGet()
        {
            IncomeList = new List<LineItemVM>();
            RefundList = new List<LineItemVM>();
            ChargeList = new List<LineItemVM>();
            SummaryList = new List<LineItemVM>();
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var paymentList = await _unitOfWork.Payment.ListAsync(p => p.PayDate >= startDate && p.PayDate <= endDate);
            var payTypeList = await _unitOfWork.Payment_Type.ListAsync(a => a.PayTypeID != null);
            var payReasonList = await _unitOfWork.Payment_Reason.ListAsync(a => a.PayReasonID != null);
            List<Reservation> reservationList = new List<Reservation>();

            foreach (Payment p in paymentList)
            {
                Reservation r = await _unitOfWork.Reservation.GetAsync(r => r.ResID == p.ResID);
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
            var income = transactionQuery.Where(t => t.paymentReason.ToLower().Contains("payment"));
            var refunds = transactionQuery.Where(t => t.paymentReason.ToLower().Contains("refund"));
            var charges = transactionQuery.Except(income).Except(refunds);

            foreach (Payment_Type p in payTypeList)
            {
                LineItemVM incomeItem = new LineItemVM();
                incomeItem.Name = p.PayType;
                incomeItem.Amount = income.Where(i => i.paymentType == p.PayType).Sum(i => i.total);
                IncomeList.Add(incomeItem);
                LineItemVM refundItem = new LineItemVM();
                refundItem.Name = p.PayType;
                refundItem.Amount = refunds.Where(i => i.paymentType == p.PayType).Sum(i => i.total);
                RefundList.Add(refundItem);
            }

            foreach (Payment_Reason p in payReasonList)
            {
                if (!p.PayReasonName.ToLower().Contains("payment") && !p.PayReasonName.ToLower().Contains("refund"))
                {
                    LineItemVM item = new LineItemVM();
                    item.Name = p.PayReasonName;
                    item.Amount = charges.Where(i => i.paymentReason == p.PayReasonName).Sum(i => i.total);
                    ChargeList.Add(item);
                }
            }

            decimal total = 0;
            foreach(LineItemVM i in IncomeList)
            {
                total += i.Amount;
            }
            LineItemVM incomeTotal = new LineItemVM();
            incomeTotal.Name = "Total";
            incomeTotal.Amount = total;
            IncomeList.Add(incomeTotal);
            total = 0;
            foreach (LineItemVM i in RefundList)
            {
                total += i.Amount;
            }
            LineItemVM refundTotal = new LineItemVM();
            refundTotal.Name = "Total";
            refundTotal.Amount = total;
            RefundList.Add(refundTotal);
            total = 0;
            foreach (LineItemVM i in ChargeList)
            {
                total += i.Amount;
            }
            LineItemVM chargeTotal = new LineItemVM();
            chargeTotal.Name = "Total";
            chargeTotal.Amount = total;
            ChargeList.Add(chargeTotal);

            LineItemVM grossIncome = new LineItemVM();
            grossIncome.Name = "Gross Income";
            grossIncome.Amount += IncomeList.Find(i => i.Name == "Total").Amount;
            if (ChargeList.Find(i => i.Name == "Total") != null)
            {
                grossIncome.Amount += ChargeList.Find(i => i.Name == "Total").Amount;
            }
            SummaryList.Add(grossIncome);
            LineItemVM refundSummary = new LineItemVM();
            refundSummary.Name = "Refunds";
            refundSummary.Amount = refundTotal.Amount;
            SummaryList.Add(refundSummary);
            LineItemVM netIncome = new LineItemVM();
            netIncome.Name = "Net Income";
            netIncome.Amount = incomeTotal.Amount + refundTotal.Amount;
            SummaryList.Add(netIncome);

            return Page();
        }
    }
}
