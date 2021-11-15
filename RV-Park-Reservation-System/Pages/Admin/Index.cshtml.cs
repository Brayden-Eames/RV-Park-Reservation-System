using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RV_Park_Reservation_System.Pages.Admin
{
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public IndexModel(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        [BindProperty]
        public List<Site> vacancies { get; set; }
        [BindProperty]
        public List<Site> occupancies { get; set; }

        public IActionResult OnGet()
        {
            if (!User.Identity.IsAuthenticated || User.IsInRole(SD.CustomerRole))
            {
                return RedirectToPage("/Shared/Prohibited", new { path = "/Admin/Index" });
            }
            vacancies = new List<Site>();
            occupancies = new List<Site>();

            DateTime today = DateTime.Now.Date;

            IEnumerable<Reservation> reservations = _unitOfWork.Reservation.List();

            reservations = reservations.Where(s => (DateTime.Compare(s.ResStartDate, today) >=0 && DateTime.Compare(s.ResEndDate, today) >= 0) /*||
                                                 ((s.ResStartDate > today && s.ResEndDate > today) && (s.ResStartDate < today && s.ResEndDate < today))*/);

            IEnumerable<Site> sites = _unitOfWork.Site.List();

            foreach(Site s in sites)
            {
                if (reservations.Where(r => r.SiteID == s.SiteID).Count() == 0)
                {
                    vacancies.Add(s);
                }
                else
                {
                    occupancies.Add(s);
                }
            }

            return Page();
        }
    }
}
