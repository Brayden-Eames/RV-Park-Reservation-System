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
        #region injectedServices

        private readonly IUnitOfWork _unitOfWork;

        public BookingReservationsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        //Gets the sites based on the users input. 
        [HttpGet]
        public IEnumerable<Site> OnGetSites(string date1, string date2, string vehicleLength)
        {
            //Parses the information to a date and int data type. 
            var startDate = DateTime.Parse(date1);
            var endDate = DateTime.Parse(date2);
            var vLength = Convert.ToInt32(vehicleLength);

            //Gets all reservations that fall in between the start date and end date, or
            //where the end date falls after the start date and befor the end date of the selected dates,
            //or where the start date falls after the start date and befor the end date of the selected dates.
            var reservations = _unitOfWork.Reservation.List()
                                                 .Where(s => (s.ResStartDate <= startDate && s.ResEndDate >= startDate)
                                                 || (s.ResStartDate <= endDate && s.ResEndDate >= endDate) ||
                                                 ((s.ResStartDate > startDate && s.ResEndDate > startDate) && (s.ResStartDate < endDate && s.ResEndDate < endDate))).ToList(); 



            //Get all sites
            IEnumerable<Site> site = _unitOfWork.Site.List();

            //Get only sites == Vehicle length unless the user is an admin and then is gets sites > vehicle length.
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

            //Checks if tent was selected and only displays the tent site if available. 
            if (reservations.Select(s => s.SiteID).Contains(47) && vLength == 20)
            {
                site = Enumerable.Empty<Site>();

            }
            if (vLength == 20 && !reservations.Select(s => s.SiteID).Contains(47))
            {
                site = Enumerable.Empty<Site>();
                site = site.Append(_unitOfWork.Site.Get(s => s.SiteID == 47));
            }

            //Returns the sites. 
            return site;

        }
    }
}
