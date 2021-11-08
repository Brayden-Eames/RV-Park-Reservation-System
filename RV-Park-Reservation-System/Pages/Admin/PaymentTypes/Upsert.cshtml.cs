using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RV_Park_Reservation_System.Pages.Admin.PaymentTypes
{
    public class UpsertModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public Payment_Type PaymentTypeObj { get; set; }

        public UpsertModel(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public IActionResult OnGet(int? id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Shared/Prohibited", new { path = "/Admin/PaymentTypes/Upsert" });
            }

            PaymentTypeObj = new Payment_Type
            {
                PayType = ""
            };

            if (id != 0)
            {
                PaymentTypeObj = _unitOfWork.Payment_Type.Get(p => p.PayTypeID == id, true);
                if (PaymentTypeObj == null)
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

            if (PaymentTypeObj.PayTypeID == 0) //New Payment Type
            {
                _unitOfWork.Payment_Type.Add(PaymentTypeObj);
            }
            else //Update
            {
                _unitOfWork.Payment_Type.Update(PaymentTypeObj);
            }
            _unitOfWork.Commit();
            return RedirectToPage("Index");
        }
    }
}
