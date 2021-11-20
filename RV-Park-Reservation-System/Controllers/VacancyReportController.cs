using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using RV_Park_Reservation_System.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RV_Park_Reservation_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VacancyReportController : Controller
    {
        private readonly IUnitOfWork _unitofWork;        
        public VacancyReportController(IUnitOfWork unitofWork)
        {
            _unitofWork = unitofWork;            
        }

        [HttpGet]
        public IActionResult Get(DateTime startDate, DateTime endDate)
        {


            var reservationList = _unitofWork.Reservation.List();
            var customerList = _unitofWork.Customer.List();
            var siteList = _unitofWork.Site.List();
            var siteCategoryList = _unitofWork.Site_Category.List();
            var statusList = _unitofWork.Reservation_Status.List();
            var siteRateList = _unitofWork.Site_Rate.List();
            var vehicleTypeList = _unitofWork.Vehicle_Type.List();

            //GET SITE LIST
            var siteQuery = from site in siteList
                            join cat in siteCategoryList on site.SiteCategoryID equals cat.SiteCategory
                            join rat in siteRateList on cat.SiteCategory equals rat.SiteCategoryID
                            into sites
                            from subItem in sites.DefaultIfEmpty()
                            select new
                            {
                                id = site.SiteID,
                                site = site.SiteNumber,
                                length = site.SiteLength,
                                description = site.SiteDescription,
                                rate = subItem.RateAmount
                            };

            var sitesList = new List<VacantSiteReportVM>();

            foreach (var item in siteQuery)
            {
                VacantSiteReportVM row = new VacantSiteReportVM();
                row.id = item.id;
                row.site = item.site;
                row.length = item.length;
                row.description = item.description;
                row.rate = item.rate;

                sitesList.Add(row);
            }


            //GET RESERVATION LIST
            var reservationQuery = from rev in reservationList
                                   join stat in statusList on rev.ResStatusID equals stat.ResStatusID
                                   join veh in vehicleTypeList on rev.TypeID equals veh.TypeID
                                   join cust in customerList on rev.Id equals cust.Id
                                   join sit in siteList on rev.SiteID equals sit.SiteID
                                   join cat in siteCategoryList on sit.SiteCategoryID equals cat.SiteCategory
                                   join rat in siteRateList on cat.SiteCategory equals rat.SiteCategoryID
                                   into reservation
                                   from subItem in reservation.DefaultIfEmpty()
                                   select new
                                   {
                                       id = rev.ResID,
                                       name = cust.CustFirstName + " " + cust.CustLastName,
                                       siteNumber = sit.SiteNumber,
                                       checkIn = rev.ResStartDate.Month + "/" + rev.ResStartDate.Day + "/" + rev.ResStartDate.Year,
                                       checkOut = rev.ResEndDate.Month + "/" + rev.ResEndDate.Day + "/" + rev.ResEndDate.Year,
                                       nights = rev.ResEndDate.Ticks - rev.ResStartDate.Ticks, //gets the difference of tick
                                       status = stat.ResStatusName
                                   };

            var reservationActivityList = new List<ActivityReportVM>();


            foreach (var v in reservationQuery)
            {
                if (v.status != "Cancelled" && v.status != "Completed")  //FILTER OUT CANCELLED AND COMPLETED RESERVATIONS.
                {    
                    TimeSpan nightSpan = new TimeSpan(v.nights); //converts ticks to a timespan                    

                    ActivityReportVM row = new ActivityReportVM();
                    row.id = v.id;
                    row.name = v.name;
                    row.siteNumber = v.siteNumber;
                    row.checkIn = v.checkIn;
                    row.checkOut = v.checkOut;
                    row.nights = nightSpan.Days;
                    row.status = v.status;

                    reservationActivityList.Add(row);
                }
            }


            //THIS DOES AN EXCLUSIVE LEFT JOIN BETWEEN TOTAL SITES AND RESERVED SITES TO GET TOTAL VACANT SITES FOR THE GIVEN PERIOD OF STARTDATE - ENDDATE
            var vacantSiteQuery = from site in sitesList
                                  join booking in reservationActivityList on site.site equals booking.siteNumber
                                  into vacantSites
                                  from subItem in vacantSites.DefaultIfEmpty()
                                  where subItem == null 
                                  || DateTime.Parse(subItem.checkIn) < startDate 
                                  && DateTime.Parse(subItem.checkOut) < startDate 
                                  || DateTime.Parse(subItem.checkIn) > endDate
                                  select new
                                  {
                                      site.id,
                                      site.site,
                                      site.description,
                                      site.length,
                                      site.rate,                                    
                                  };

            var vacantSitesList = new List<VacantSiteReportVM>();

            foreach (var item in vacantSiteQuery)
            {
                VacantSiteReportVM row = new VacantSiteReportVM();
                row.id = item.id;
                row.site = item.site;
                row.description = item.description;
                row.length = item.length;
                row.rate = item.rate;
                row.available = startDate.Month + "/" + startDate.Day + "/" + startDate.Year + " - " + endDate.Month + "/" + endDate.Day + "/" + endDate.Year;

                vacantSitesList.Add(row);
            }


            return Json(new { data = vacantSitesList });

        }
    }
}
