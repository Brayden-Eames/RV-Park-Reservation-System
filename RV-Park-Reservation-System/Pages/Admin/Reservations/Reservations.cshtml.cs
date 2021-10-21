using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using RV_Park_Reservation_System.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RV_Park_Reservation_System.Pages.Admin
{
    public class ReservationsModel : PageModel
    {
        private readonly IUnitOfWork _unitofWork;
        public ReservationsModel(IUnitOfWork unitofWork) => _unitofWork = unitofWork;

        public IEnumerable<Reservation> ReservationList { get; set; }

        public AdminReservationVM AdminReservationObject { get; set; }

        //We might need to make a ViewModel to allow us to pull from the Reservation, Customer, Service Status Type and DODAffiliation tables. 
        public void OnGet()
        {
            //need to pull data from Reservation, Customer, DODAffiliation and ServiceStatusType tables. Use the ViewModel to do so
            ReservationList = _unitofWork.Reservation.List();
            AdminReservationObject = new AdminReservationVM()
            {
                Reservations = _unitofWork.Reservation.List(),
                ListOfCustomers = _unitofWork.Customer.List(),
                DODAffiliationList = _unitofWork.DOD_Affiliation.List(),
                ServiceStatusTypes = _unitofWork.Service_Status_Type.List()
            };

            //IMPORTANT: NEED TO MAKE LINQ QUERY (Copy admin methodology from JohariWindow) TO DISPLAY PROPER DATA
            //Possible Idea: use admin vm page object data to populate lists for the LINQ query
        }
    }
}
