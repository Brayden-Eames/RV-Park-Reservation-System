using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RV_Park_Reservation_System.ViewModels;

namespace RV_Park_Reservation_System.Pages.Admin.SiteRate
{
    public class UpsertModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public SiteRateVM SiteRateObj { get; set; }

        public UpsertModel(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public IActionResult OnGet(int? id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Shared/Prohibited", new { path = "/Admin/SiteRates/Upsert" });
            }

            var categories = _unitOfWork.Site_Category.List();

            SiteRateObj = new SiteRateVM
            {
                SiteRate = new Site_Rate(),
                SiteCategoryList = categories.Select(c => new SelectListItem { Value = c.SiteCategory.ToString(), Text = c.SiteCategoryName })
            };

            if (id != 0)
            {
                SiteRateObj.SiteRate = _unitOfWork.Site_Rate.Get(r => r.RateID == id, true);
                if (SiteRateObj == null)
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

            if (SiteRateObj.SiteRate.RateID == 0) //New Site Category
            {
                SiteRateObj.SiteRate.RateLastModifiedBy = User.Identity.Name;
                SiteRateObj.SiteRate.RateModifiedDate = DateTime.Now;
                _unitOfWork.Site_Rate.Add(SiteRateObj.SiteRate);
            }
            else //Update
            {
                SiteRateObj.SiteRate.RateLastModifiedBy = User.Identity.Name;
                SiteRateObj.SiteRate.RateModifiedDate = DateTime.Now;
                _unitOfWork.Site_Rate.Update(SiteRateObj.SiteRate);
            }
            _unitOfWork.Commit();
            return RedirectToPage("../Manage");
        }
    }
}
