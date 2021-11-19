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
    public class HistoricReservationReportController: Controller
    {
        private readonly IUnitOfWork _unitofWork;
        public HistoricReservationReportController(IUnitOfWork unitofWork)
        {
            _unitofWork = unitofWork;
        }

        [HttpGet]
        public IActionResult Get()
        {
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
                                   select new
                                   {
                                       id = rev.ResID,
                                       name = cust.CustFirstName + " " + cust.CustLastName,
                                       phone = cust.CustPhone,
                                       email = cust.CustEmail,
                                       siteNumber = sit.SiteNumber,
                                       checkIn = rev.ResStartDate.Month + "/" + rev.ResStartDate.Day + "/" + rev.ResStartDate.Year,
                                       checkOut = rev.ResEndDate.Month + "/" + rev.ResEndDate.Day + "/" + rev.ResEndDate.Year,                                       
                                       status = stat.ResStatusName
                                   };

            var historicReservationList = new List<HistoricReservationReportVM>();


            foreach (var v in reservationQuery)
            {               
                    HistoricReservationReportVM row = new HistoricReservationReportVM();
                    row.id = v.id;
                    row.name = v.name;
                    row.phone = v.phone;
                    row.email = v.email;
                    row.siteNumber = v.siteNumber;
                    row.checkIn = v.checkIn;
                    row.checkOut = v.checkOut;                   
                    row.status = v.status;

                historicReservationList.Add(row);                

            }

            return Json(new { data = historicReservationList });
        }
    }
}
