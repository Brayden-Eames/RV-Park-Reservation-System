using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RV_Park_Reservation_System.Pages.Admin.ServiceStatuses
{
    public class UpsertModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public Service_Status_Type ServiceStatusTypeObj { get; set; }

        public UpsertModel(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public IActionResult OnGet(int? id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Shared/Prohibited", new { path = "/Admin/ServiceStatuses/Upsert" });
            }

            ServiceStatusTypeObj = new Service_Status_Type
            {
                ServiceStatusType = ""
            };

            if (id != 0)
            {
                ServiceStatusTypeObj = _unitOfWork.Service_Status_Type.Get(s => s.ServiceStatusID == id, true);
                if (ServiceStatusTypeObj == null)
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

            if (ServiceStatusTypeObj.ServiceStatusID == 0) //New Service Status Type
            {
                _unitOfWork.Service_Status_Type.Add(ServiceStatusTypeObj);
            }
            else //Update
            {
                _unitOfWork.Service_Status_Type.Update(ServiceStatusTypeObj);
            }
            _unitOfWork.Commit();
            return RedirectToPage("../Manage");
        }
    }
}
