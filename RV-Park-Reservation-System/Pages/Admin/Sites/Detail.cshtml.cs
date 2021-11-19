using System.Collections.Generic;
using System.Linq;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RV_Park_Reservation_System.ViewModels;

namespace RV_Park_Reservation_System.Pages.Admin.Sites
{
    public class DetailModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public DetailModel(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        [BindProperty]
        public ReservationListVM ReservationList { get; set; }

        [BindProperty]
        public Site SiteObj { get; set; }

        public IActionResult OnGet(int id)
        {
            if(id == 0)
            {
                return RedirectToPage("Index");
            }

            SiteObj = _unitOfWork.Site.Get(s => s.SiteID == id);
            ReservationList = new ReservationListVM();
            ReservationList.ReservationList = new List<ReservationCalendarVM>();

            List<Reservation> res = _unitOfWork.Reservation.List().Where(r => r.SiteID == SiteObj.SiteID).ToList();

            foreach(Reservation r in res)
            {
                Customer c = _unitOfWork.Customer.Get(c => c.Id == r.Id);
                r.ResStartDate = r.ResStartDate.Date;
                r.ResEndDate = r.ResEndDate.Date;
                ReservationCalendarVM calObj = new ReservationCalendarVM{
                    ReservationObj = r,
                    CustomerObj = c
                };
                ReservationList.ReservationList.Add(calObj);
            }

            return Page();
        }
    }
}
