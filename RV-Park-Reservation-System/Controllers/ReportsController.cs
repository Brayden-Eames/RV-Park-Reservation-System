using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RV_Park_Reservation_System.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RV_Park_Reservation_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : Controller
    {
        private readonly IUnitOfWork _unitofWork;
        private readonly IWebHostEnvironment _hostingEnv;
        public ReportsController(IUnitOfWork unitofWork, IWebHostEnvironment hostingEnv)
        {
            _unitofWork = unitofWork;
            _hostingEnv = hostingEnv;
        }

        [HttpGet]
        public IActionResult Get(DateTime startDate, DateTime endDate)
        {
            //NEED TO DO SOME TABLE JOINING TO GET THIS DATA CORRECT.
            //FIRST MAKE A QUERY TO GET ALL THE NEEDED DATA
            //GET RESULTS AND TURN IT INTO A LIST
            //CONVERT LIST TO JSON DATA.

            /*
           VACANCY REPORT:                        
           */
            var reservationList = _unitofWork.Reservation.List();
            var customerList = _unitofWork.Customer.List();
            var siteList = _unitofWork.Site.List();
            var siteCategoryList = _unitofWork.Site_Category.List();
            var statusList = _unitofWork.Reservation_Status.List();     
            var siteRateList = _unitofWork.Site_Rate.List();
            var vehicleTypeList = _unitofWork.Vehicle_Type.List();

            var reservationQuery = from rev in reservationList
                                   join stat in statusList on rev.ResStatusID equals stat.ResStatusID
                                   join veh in vehicleTypeList on rev.TypeID equals veh.TypeID 
                                   join cust in customerList on rev.Id equals cust.Id
                                   join sit in siteList on rev.SiteID equals sit.SiteID
                                   join cat in siteCategoryList on sit.SiteCategoryID equals cat.SiteCategory
                                   join rat in siteRateList on cat.SiteCategory equals rat.SiteCategoryID
                                   into reservation
                                   from subItem in reservation.DefaultIfEmpty()
                                   select new {
                                       id = rev.ResID,
                                       name = cust.CustFirstName + " " + cust.CustLastName,                                                                             
                                       siteNumber = sit.SiteNumber, 
                                       checkIn = rev.ResStartDate.Month + "/" + rev.ResStartDate.Day + "/" + rev.ResStartDate.Year,
                                       checkOut = rev.ResEndDate.Month + "/" + rev.ResEndDate.Day + "/" + rev.ResEndDate.Year,
                                       nights = rev.ResEndDate.Ticks - rev.ResStartDate.Ticks, //gets the difference of ticks
                                       status = stat.ResStatusName                                   
                                   };

            var reservationActivityList = new List<ActivityReportVM>();


            foreach (var v in reservationQuery)
            {
                if (v.status != "Cancelled" && v.status != "Completed")
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
                                          
            return Json(new { data = reservationActivityList});                     
        }

    }
}
