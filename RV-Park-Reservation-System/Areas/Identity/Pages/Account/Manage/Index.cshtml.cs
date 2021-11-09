using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RV_Park_Reservation_System.ViewModels;

namespace RV_Park_Reservation_System.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitofWork;
        private readonly UserManager<Customer> _userManager;
        public IndexModel(IUnitOfWork unitofWork, UserManager<Customer> userManager)
        {
            _userManager = userManager;
            _unitofWork = unitofWork;
        }
        
        [BindProperty]
        public UserAccountVM userAccountVM { get; set; }
        
       
        public async Task<IActionResult> OnGetAsync()
        {

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Shared/Prohibited", new { path = "/Client/Booking" });
            }

            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var userValue = await _userManager.FindByIdAsync(claim.Value); /* _unitofWork.Customer.Get(u => u.Id == claim.Value, true);*/
            var listDOD = _unitofWork.DOD_Affiliation.List();
            var listSST = _unitofWork.Service_Status_Type.List();
            
            if(userValue == null)
            {
                return NotFound();
            }

            userAccountVM = new UserAccountVM()
            {                   
                user = userValue,
                DODAffiliationList = listDOD.Select(d => new SelectListItem { Value = d.DODAffiliationID.ToString(), Text = d.DODAffiliationType}),
                ServiceStatusTypesList = listSST.Select(s => new SelectListItem { Value = s.ServiceStatusID.ToString(), Text = s.ServiceStatusType})

            };        

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var listDOD = _unitofWork.DOD_Affiliation.List();
            var listSST = _unitofWork.Service_Status_Type.List();

            userAccountVM.DODAffiliationList = listDOD.Select(d => new SelectListItem { Value = d.DODAffiliationID.ToString(), Text = d.DODAffiliationType });
            userAccountVM.ServiceStatusTypesList = listSST.Select(s => new SelectListItem { Value = s.ServiceStatusID.ToString(), Text = s.ServiceStatusType });
                
            if (!ModelState.IsValid)
            {
                return Page();
            }


            var updatedUser = await _userManager.FindByIdAsync(userAccountVM.user.Id);

            updatedUser.CustFirstName = userAccountVM.user.CustFirstName;
            updatedUser.CustLastName = userAccountVM.user.CustLastName;
            updatedUser.CustEmail = userAccountVM.user.CustEmail;
            updatedUser.CustPhone = userAccountVM.user.CustPhone;
            updatedUser.DODAffiliationID = userAccountVM.user.DODAffiliationID;
            updatedUser.DOD_Affiliation = userAccountVM.user.DOD_Affiliation;
            updatedUser.ServiceStatusID = userAccountVM.user.ServiceStatusID;
            updatedUser.Service_Status_Type = userAccountVM.user.Service_Status_Type;
            updatedUser.CustLastModifiedBy = userAccountVM.user.CustFirstName + " " + userAccountVM.user.CustLastName;
            updatedUser.CustLastModifiedDate = DateTime.Now;       

            var result = await _userManager.UpdateAsync(updatedUser);
        

            return RedirectToPage("/Index");
        }
    }
}
