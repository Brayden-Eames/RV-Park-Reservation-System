using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RV_Park_Reservation_System.Pages.Admin.DODAffiliations
{
    public class UpsertModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public DOD_Affiliation DODAffiliationObj { get; set; }

        public UpsertModel(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public IActionResult OnGet(int? id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Shared/Prohibited", new { path = "/Admin/DODAffiliations/Upsert" });
            }

            DODAffiliationObj = new DOD_Affiliation
            {
                DODAffiliationType = ""
            };

            if (id != 0)
            {
                DODAffiliationObj = _unitOfWork.DOD_Affiliation.Get(d => d.DODAffiliationID == id, true);
                if (DODAffiliationObj == null)
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

            if (DODAffiliationObj.DODAffiliationID == 0) //New DOD Affiliation
            {
                _unitOfWork.DOD_Affiliation.Add(DODAffiliationObj);
            }
            else //Update
            {
                _unitOfWork.DOD_Affiliation.Update(DODAffiliationObj);
            }
            _unitOfWork.Commit();
            return RedirectToPage("../Manage");
        }
    }
}
