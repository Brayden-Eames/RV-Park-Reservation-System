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
            
        private readonly IUnitOfWork _unitOfWork;

        public ReservationBufferController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public int OnGetSites(string date)
        {
           
            var startDate = DateTime.Parse(date);
           

            var reservations = _unitOfWork.Reservation.List().ToList();

            var user = _unitOfWork.Customer.Get(c => c.CustEmail == User.Identity.Name);

            reservations = reservations.Where(r=>(startDate - r.ResEndDate).TotalDays <= 14 && (startDate - r.ResEndDate).TotalDays > 0  && r.Id == user.Id && r.ResStatusID == 9).ToList();

            var count = reservations.Count();
           

            return count;

        }
    }
}
