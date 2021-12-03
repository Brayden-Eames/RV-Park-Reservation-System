using ApplicationCore.Interfaces;
using ApplicationCore.Models;
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
        public ReportsController(IUnitOfWork unitofWork)
        {
            _unitofWork = unitofWork;           
        }

        [HttpGet]
        public async Task<IActionResult> OnGet(DateTime startDate, DateTime endDate)
        {          
            var reservationList = await _unitofWork.Reservation.ListAsync(a => a.Id != null);
            var customerList = await _unitofWork.Customer.ListAsync(a => a.Id != null);
            var siteList = await _unitofWork.Site.ListAsync(a => a.SiteID != null);
            var siteCategoryList = await _unitofWork.Site_Category.ListAsync(a => a.LocationID != null);
            var statusList = await _unitofWork.Reservation_Status.ListAsync(a => a.ResStatusID != null);     
            var siteRateList = await _unitofWork.Site_Rate.ListAsync(a => a.RateID != null);
            var vehicleTypeList = await _unitofWork.Vehicle_Type.ListAsync(a => a.TypeID != null);

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
                    row.nights = nightSpan.Days + 1;
                    row.status = v.status;

                    reservationActivityList.Add(row);
                }

            }
                                          
            return Json(new { data = reservationActivityList});                     
        }

        [HttpPost]
        public async Task<IActionResult> UpdateFinishedReservationStatuses()
        {
            try
            {
                List<Reservation> allOnGoingReservations = new();

                allOnGoingReservations = (List<Reservation>)_unitofWork.Reservation.List(r => r.ResStatusID == 4).ToList();

                if(allOnGoingReservations == null)
                {
                    return Json(new { success = false, message = "Error: No Reservations with Status 'OnGoing' Found" });
                }

                var today = DateTime.Now;

                foreach(Reservation res in allOnGoingReservations)
                {
                    if(res.ResEndDate.Date == today.Date)
                    {                        
                        res.ResStatusID = 3;
                        _unitofWork.Reservation.Update(res);
                    }
                }

                _unitofWork.Commit();

               return Json(new { success = true, message = "Reservations Statuses Changed to Completed" });
               
            }
            catch(Exception e)
            {
               return Json(new { success = false, message = "Error: "+ e.Message });
            }
        }                

    }
}
