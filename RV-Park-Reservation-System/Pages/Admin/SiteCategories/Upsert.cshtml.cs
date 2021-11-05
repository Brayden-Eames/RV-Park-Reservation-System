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

namespace RV_Park_Reservation_System.Pages.Admin.SiteCategory
{
    public class UpsertModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public SiteCategoryVM SiteCategoryObj { get; set; }

        public UpsertModel(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public IActionResult OnGet(int ? id)
        {
            if (!User.Identity.IsAuthenticated || User.IsInRole(SD.CustomerRole))
            {
                return RedirectToPage("/Shared/Prohibited", new { path = "/Admin/SiteCategories/Upsert" });
            }

            //Commenting out because we will hard code the location ID for the initial project
            //var locations = _unitOfWork.Location.List();

            SiteCategoryObj = new SiteCategoryVM
            {
                SiteCategory = new Site_Category()
                //Commenting out because we will hard code the location ID for the initial project
                //LocationList = locations.Select(l => new SelectListItem { Value = l.LocationID.ToString(), Text = l.LocationName })
            };

            if( id != 0)
            {
                SiteCategoryObj.SiteCategory = _unitOfWork.Site_Category.Get(c => c.SiteCategory == id, true);
                if(SiteCategoryObj == null)
                {
                    return NotFound();
                }
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            if(!ModelState.IsValid)
            {
                return Page();
            }

            //Hard coding the location ID for the initial project
            SiteCategoryObj.SiteCategory.LocationID = 1;

            if (SiteCategoryObj.SiteCategory.SiteCategory == 0) //New Site Category
            {
                _unitOfWork.Site_Category.Add(SiteCategoryObj.SiteCategory);
            }
            else //Update
            {
                var objFromDb = _unitOfWork.Site_Category.Get(c => c.SiteCategory == SiteCategoryObj.SiteCategory.SiteCategory, true);
                _unitOfWork.Site_Category.Update(SiteCategoryObj.SiteCategory);
            }
            _unitOfWork.Commit();
            return RedirectToPage("../Manage");
        }
    }
}
