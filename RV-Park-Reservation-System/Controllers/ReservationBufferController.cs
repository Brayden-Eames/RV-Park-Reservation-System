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
    public class ReservationBufferController : Controller
    {
        #region injectedServices
        private readonly IUnitOfWork _unitOfWork;

        public ReservationBufferController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion


        //Checks the reservation to make sure there are 14 days between user reservations. 
        [HttpGet]
        public int OnGetSites(string date)
        {
           //Parses the date from the string date object. 
            var startDate = DateTime.Parse(date);

            //Gets the reservations and current user.
            var reservations = _unitOfWork.Reservation.List().ToList();
            var user = _unitOfWork.Customer.Get(c => c.CustEmail == User.Identity.Name);

            //Checks the reservations that are within 14 days of the selected date and scheduled. 
            reservations = reservations.Where(r=>(startDate - r.ResEndDate).TotalDays <= 14 && (startDate - r.ResEndDate).TotalDays > 0  && r.Id == user.Id && r.ResStatusID == 9).ToList();

            var count = reservations.Count();
           
            //returns the number of reservations within 14 days. 
            return count;

        }
    }
}
