using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RV_Park_Reservation_System.Pages.Admin.Reports
{
    public class TransactionReportModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public TransactionReportModel(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        [BindProperty]
        public List<Payment_Type> paymentTypeList { get; set; }

        [BindProperty]
        public List<Payment_Reason> paymentReasonList { get; set; }

        public IActionResult OnGet()
        {
            if (!User.Identity.IsAuthenticated || User.IsInRole(SD.CustomerRole))
            {
                return RedirectToPage("/Shared/Prohibited", new { path = "/Admin/Reports/TransactionReport" });
            }

            paymentTypeList = _unitOfWork.Payment_Type.List().ToList();
            paymentReasonList = _unitOfWork.Payment_Reason.List().ToList();

            return Page();
        }
    }
}
