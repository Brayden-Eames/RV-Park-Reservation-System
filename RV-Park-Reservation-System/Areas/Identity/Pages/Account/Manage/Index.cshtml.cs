using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RV_Park_Reservation_System.ViewModels;

namespace RV_Park_Reservation_System.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitofWork;
        public IndexModel(IUnitOfWork unitofWork) => _unitofWork = unitofWork;
        
        [BindProperty]
        public UserAccountVM userAccountVM { get; set; }


        public IActionResult OnGet()
        {

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Shared/Prohibited", new { path = "/Client/Booking" });
            }

            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var userValue = _unitofWork.Customer.Get(u => u.Id == claim.Value, true);
            var listDOD = _unitofWork.DOD_Affiliation.List();
            var listSST = _unitofWork.Service_Status_Type.List();
   
            userAccountVM = new UserAccountVM()
            {
                user = userValue,
                DODAffiliationList = listDOD.Select(d => new SelectListItem { Value = d.DODAffiliationID.ToString(), Text = d.DODAffiliationType}),
                ServiceStatusTypesList = listSST.Select(s => new SelectListItem { Value = s.ServiceStatusID.ToString(), Text = s.ServiceStatusType})

            };

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            userAccountVM.user.CustLastModifiedBy = userAccountVM.user.CustFirstName + " " + userAccountVM.user.CustLastName;
            userAccountVM.user.CustLastModifiedDate = DateTime.Now;

            _unitofWork.Customer.Update(userAccountVM.user);


            return Page();
        }
    }
}
