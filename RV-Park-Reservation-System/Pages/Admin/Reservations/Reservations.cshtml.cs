using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RV_Park_Reservation_System.Pages.Admin
{
    public class ReservationsModel : PageModel
    {
        private readonly IUnitOfWork _unitofWork;
        public ReservationsModel(IUnitOfWork unitofWork) => _unitofWork = unitofWork;

        public IEnumerable<Reservation> ReservationList { get; set; }

        //We might need to make a ViewModel to allow us to pull from the Reservation, Customer, Service Status Type and DODAffiliation tables. 
        public void OnGet()
        {
        }
    }
}
