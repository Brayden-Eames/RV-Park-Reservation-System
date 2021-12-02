using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RV_Park_Reservation_System.ViewModels;

namespace RV_Park_Reservation_System.Pages.Admin.Reports
{
    public class RevenueReportModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _environment;

        public RevenueReportModel(IUnitOfWork unitOfWork, IWebHostEnvironment environment)
        {
            _unitOfWork = unitOfWork;
            _environment = environment;
        }

        [BindProperty]
        public bool reportVisible { get; set; }

        [BindProperty]
        public string file { get; set; }

        [BindProperty]
        public string downloadPath { get; set; }

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
            reportVisible = false;
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
            reportVisible = true;
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
                incomeItem.Amount = decimal.Round(income.Where(i => i.paymentType == p.PayType).Sum(i => i.total), 2);
                IncomeList.Add(incomeItem);
                LineItemVM refundItem = new LineItemVM();
                refundItem.Name = p.PayType;
                refundItem.Amount = decimal.Round(refunds.Where(i => i.paymentType == p.PayType).Sum(i => i.total), 2);
                RefundList.Add(refundItem);
            }

            foreach (Payment_Reason p in payReasonList)
            {
                if (!p.PayReasonName.ToLower().Contains("payment") && !p.PayReasonName.ToLower().Contains("refund"))
                {
                    LineItemVM item = new LineItemVM();
                    item.Name = p.PayReasonName;
                    item.Amount = decimal.Round(charges.Where(i => i.paymentReason == p.PayReasonName).Sum(i => i.total), 2);
                    ChargeList.Add(item);
                }
            }
            string file = "Date Range: " + startDate.ToShortDateString() + " - " + endDate.ToShortDateString() + "\n";

            decimal total = 0;
            file += "INCOME:\n";
            foreach (LineItemVM i in IncomeList)
            {
                total += i.Amount;
                file += i.Name + ": " + i.Amount + "\n";
            }
            LineItemVM incomeTotal = new LineItemVM();
            incomeTotal.Name = "Total";
            incomeTotal.Amount = total;
            IncomeList.Add(incomeTotal);
            file += incomeTotal.Name + ": " + incomeTotal.Amount + "\n";
            total = 0;
            file += "\n\nREFUNDS:\n";
            foreach (LineItemVM i in RefundList)
            {
                total += i.Amount;
                file += i.Name + ": " + i.Amount + "\n";
            }
            LineItemVM refundTotal = new LineItemVM();
            refundTotal.Name = "Total";
            refundTotal.Amount = total;
            RefundList.Add(refundTotal);
            file += refundTotal.Name + ": " + refundTotal.Amount + "\n";
            total = 0;
            file += "\n\nCHARGES:\n";
            foreach (LineItemVM i in ChargeList)
            {
                total += i.Amount;
                file += i.Name + ": " + i.Amount + "\n";
            }
            LineItemVM chargeTotal = new LineItemVM();
            chargeTotal.Name = "Total";
            chargeTotal.Amount = total;
            ChargeList.Add(chargeTotal);
            file += chargeTotal.Name + ": " + chargeTotal.Amount + "\n";
            file += "\n\nSUMMARY:\n";
            LineItemVM grossIncome = new LineItemVM();
            grossIncome.Name = "Gross Income";
            grossIncome.Amount += IncomeList.Find(i => i.Name == "Total").Amount;
            if (ChargeList.Find(i => i.Name == "Total") != null)
            {
                grossIncome.Amount += ChargeList.Find(i => i.Name == "Total").Amount;
            }
            SummaryList.Add(grossIncome);
            file += grossIncome.Name + ": " + grossIncome.Amount + "\n";
            LineItemVM refundSummary = new LineItemVM();
            refundSummary.Name = "Refunds";
            refundSummary.Amount = refundTotal.Amount;
            SummaryList.Add(refundSummary);
            file += refundSummary.Name + ": " + refundSummary.Amount + "\n";
            LineItemVM netIncome = new LineItemVM();
            netIncome.Name = "Net Income";
            netIncome.Amount = incomeTotal.Amount + refundTotal.Amount;
            SummaryList.Add(netIncome);
            file += netIncome.Name + ": " + netIncome.Amount + "\n";

            string wwwPath = this._environment.WebRootPath;
            string path = Path.Combine(this._environment.WebRootPath, "Reports");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            else
            {
                Directory.Delete(path, true);
                Directory.CreateDirectory(path);
            }
            string FullPath = Path.Combine(path, "report.txt");
            using (FileStream stream = new FileStream(FullPath, FileMode.Create))
            {
                byte[] info = new UnicodeEncoding().GetBytes(file);
                // Add some information to the file.
                stream.Write(info, 0, info.Length);
            }
            downloadPath = "https://localhost:44371/" + "Reports/report.txt";
            //downloadPath = FullPath;
            ViewData["File"] = file;
            return Page();
        }
    }
}
