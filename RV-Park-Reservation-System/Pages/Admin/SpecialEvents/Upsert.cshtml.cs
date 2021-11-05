using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RV_Park_Reservation_System.ViewModels;

namespace RV_Park_Reservation_System.Pages.Admin.SpecialEvent
{
    public class UpsertModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public SpecialEventVM SpecialEventObj { get; set; }

        public UpsertModel(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public IActionResult OnGet(int? id)
        {
            if (!User.Identity.IsAuthenticated || User.IsInRole(SD.CustomerRole))
            {
                return RedirectToPage("/Shared/Prohibited", new { path = "/Admin/SpecialEvents/Upsert" });
            }

            //Commenting out because we will hard code the location ID for the initial project
            //var locations = _unitOfWork.Location.List();

            SpecialEventObj = new SpecialEventVM
            {
                SpecialEvent = new Special_Event()
                //Commenting out because we will hard code the location ID for the initial project
                //LocationList = locations.Select(l => new SelectListItem { Value = l.LocationID.ToString(), Text = l.LocationName })
            };

            if (id != 0)
            {
                SpecialEventObj.SpecialEvent = _unitOfWork.Special_Event.Get(e => e.EventID == id, true);
                if (SpecialEventObj == null)
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

            //Hard coding the location ID for the initial project
            SpecialEventObj.SpecialEvent.LocationID = 1;

            if (SpecialEventObj.SpecialEvent.EventID == 0) //New Special Event
            {
                _unitOfWork.Special_Event.Add(SpecialEventObj.SpecialEvent);
            }
            else //Update
            {
                _unitOfWork.Special_Event.Update(SpecialEventObj.SpecialEvent);
            }
            _unitOfWork.Commit();
            return RedirectToPage("../Manage");
        }
    }
}
