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
        public IEnumerable<int> OnGetSites(string date1, string date2, string vehicleLength)
        {
            var startDate = DateTime.Parse(date1);
            var endDate = DateTime.Parse(date2);
            var vLength = Convert.ToInt32(vehicleLength);

            var reservations = _unitOfWork.Reservation.List();

            reservations = reservations.Where(s=>(s.ResStartDate <= startDate && s.ResEndDate > startDate) 
                                                 || (s.ResStartDate <= endDate && s.ResEndDate > endDate));

            var sites = _unitOfWork.Site.List();
            sites = sites.Where(s => s.SiteLength > vLength);

            IEnumerable<int> SiteIDs = sites.Select(r => r.SiteID).ToList();
            IEnumerable<int> badSiteIDs = reservations.Select(r => r.SiteID).ToList().Distinct();
            SiteIDs = SiteIDs.Except(badSiteIDs);

            return SiteIDs;

        }
    }
}
