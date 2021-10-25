using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RV_Park_Reservation_System.ViewModels;

namespace RV_Park_Reservation_System.Pages.Admin.ManageSites
{
    public class UpsertModel : PageModel
    {
        private readonly IUnitOfWork _unitofWork;
        private readonly IWebHostEnvironment _hostingEnvironment;

        [BindProperty]
        public SitesVM SiteVmObj { get; set; }
        public UpsertModel(IUnitOfWork unitofWork, IWebHostEnvironment hostEnvironment)
        {
            _unitofWork = unitofWork;
            _hostingEnvironment = hostEnvironment;
        }

        public IActionResult OnGet(int? id)
        {
            var siteCategories = _unitofWork.Site_Category.List();

            SiteVmObj = new SitesVM()
            {
                SiteItem = new Site(),                
                CategoryList = siteCategories.Select(c => new SelectListItem { Value = c.SiteCategory.ToString(), Text = c.SiteCategoryName}),               
            };

            if(id != 0) //edit mode
            {
                SiteVmObj.SiteItem = _unitofWork.Site.Get(s => s.SiteID == id, true);
                if(SiteVmObj == null)
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

            DateTime currentDate = DateTime.Now;
         
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            string firstname = "FirstName"; //TODO: GET FIRST NAME OF USER using claim.value;
        

            SiteVmObj.SiteItem.SiteLastModifiedBy = firstname;
            SiteVmObj.SiteItem.SiteLastModifiedDate = currentDate.Date;

            if (SiteVmObj.SiteItem.SiteID == 0)
            {   
                _unitofWork.Site.Add(SiteVmObj.SiteItem);
            }
            else
            {
                _unitofWork.Site.Update(SiteVmObj.SiteItem);
            }
            _unitofWork.Commit();

            return RedirectToPage("./Sites");
        }


    }
}
