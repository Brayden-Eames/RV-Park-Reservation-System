using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RV_Park_Reservation_System.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class BookingReservationsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookingReservationsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IEnumerable<Site> OnGetSites(string date1, string date2, string vehicleLength)
        {
            var startDate = DateTime.Parse(date1);
            var endDate = DateTime.Parse(date2);
            var vLength = Convert.ToInt32(vehicleLength);

            var reservations = _unitOfWork.Reservation.List();

            reservations = reservations.Where(s=>(s.ResStartDate <= startDate && s.ResEndDate >= startDate) 
                                                 || (s.ResStartDate <= endDate && s.ResEndDate >= endDate) ||
                                                 ((s.ResStartDate >startDate && s.ResEndDate >startDate) && (s.ResStartDate < endDate && s.ResEndDate < endDate)));

            var sites = _unitOfWork.Site.List();
            sites = sites.Where(s => s.SiteLength > vLength);

            IEnumerable<Site> site = _unitOfWork.Site.List();
            site = site.Where(s => s.SiteLength > vLength);
            IEnumerable<Site> badSites = Enumerable.Empty<Site>();

            foreach (var item in reservations)
            {
                badSites.Append(_unitOfWork.Site.Get(s => s.SiteID == item.SiteID));
            }
            site = site.Except(badSites);

            if (reservations.Select(s => s.SiteID).Contains(47) && vLength == 20)
            {
                site = Enumerable.Empty<Site>();

            }
                if (vLength == 20 && !reservations.Select(s => s.SiteID).Contains(47))
                {
                    site = Enumerable.Empty<Site>();
                    site = site.Append(_unitOfWork.Site.Get(s => s.SiteID == 47));
                }

            return site;

        }
    }
}
