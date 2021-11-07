using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RV_Park_Reservation_System.Pages.Admin.PaymentReasons
{
    public class UpsertModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public Payment_Reason PaymentReasonObj { get; set; }

        public UpsertModel(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public IActionResult OnGet(int? id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Shared/Prohibited", new { path = "/Admin/PaymentReasons/Upsert" });
            }

            PaymentReasonObj = new Payment_Reason
            {
                PayReasonName = ""
            };

            if (id != 0)
            {
                PaymentReasonObj = _unitOfWork.Payment_Reason.Get(p => p.PayReasonID == id, true);
                if (PaymentReasonObj == null)
                {
                    return NotFound();
                }
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (PaymentReasonObj.PayReasonID == 0) //New Payment Reason
            {
                _unitOfWork.Payment_Reason.Add(PaymentReasonObj);
            }
            else //Update
            {
                _unitOfWork.Payment_Reason.Update(PaymentReasonObj);
            }
            _unitOfWork.Commit();
            return RedirectToPage("Index");
        }
    }
}
