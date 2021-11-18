using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Infrastructure.Services;
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

            var reservations = _unitOfWork.Reservation.List().ToList();

            reservations = reservations.Where(s=>(s.ResStartDate <= startDate && s.ResEndDate >= startDate) 
                                                 || (s.ResStartDate <= endDate && s.ResEndDate >= endDate) ||
                                                 ((s.ResStartDate >startDate && s.ResEndDate >startDate) && (s.ResStartDate < endDate && s.ResEndDate < endDate))).ToList();


            //Get all sites
            IEnumerable<Site> site = _unitOfWork.Site.List();
            //Get only sites == Vehicle length
            if (User.IsInRole(SD.AdminRole))
            {
                site = site.Where(s => s.SiteLength >vLength);
            }
            else
            {
                if (vLength == 40)
                {
                    site = site.Where(s => s.SiteLength == 42);
                }
                else if (vLength == 45)
                {
                    site = site.Where(s => s.SiteLength == 45);
                }
                else
                {
                    site = site.Where(s => s.SiteLength == 65);
                }
            }
            

            //Create empty sites variable
            List<Site> badSites = new List<Site>();

            //Take all sites from reservations and remove them from good sites. 
            for (int i = 0; i < reservations.Count(); i++)
            {
                var bad = _unitOfWork.Site.Get(s => s.SiteID == reservations[i].SiteID);
                badSites.Add(bad);
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
