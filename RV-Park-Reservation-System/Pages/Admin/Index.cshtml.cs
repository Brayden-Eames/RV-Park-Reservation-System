using System;
using System.Collections.Generic;
using System.Linq;
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

        [BindProperty]
        public int checkins { get; set; }

        [BindProperty]
        public int checkouts { get; set; }

        [BindProperty]
        public int ongoing { get; set; }

        public IActionResult OnGet()
        {
            if (!User.Identity.IsAuthenticated || User.IsInRole(SD.CustomerRole))
            {
                return RedirectToPage("/Shared/Prohibited", new { path = "/Admin/Index" });
            }
            vacancies = new List<Site>();
            occupancies = new List<Site>();

            //DateTime today = DateTime.Now.Date;
            DateTime today = new DateTime(2021, 11, 15).Date;

            IEnumerable<Reservation> reservations = _unitOfWork.Reservation.List();

            reservations = reservations.Where(s => (DateTime.Compare(s.ResStartDate.Date, today.Date) <= 0 && DateTime.Compare(s.ResEndDate.Date, today.Date) >= 0));

            checkins = reservations.Where(c => (DateTime.Compare(c.ResStartDate.Date, today.Date) == 0)).Count();
            checkouts = reservations.Where(c => (DateTime.Compare(c.ResEndDate.Date, today.Date) == 0)).Count();
            ongoing = reservations.Where(c => (DateTime.Compare(c.ResStartDate.Date, today.Date) < 0) && DateTime.Compare(c.ResEndDate.Date, today.Date) > 0).Count();

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
