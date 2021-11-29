using ApplicationCore.Models;
using System;
using System.Collections.Generic;

namespace RV_Park_Reservation_System.ViewModels
{
    public class AdminReservationVM
    {
        public IEnumerable<Reservation> Reservations { get; set; }
        public IEnumerable<Customer> ListOfCustomers { get; set; }
        public IEnumerable<Site> ListOfSites { get; set; }
        public IEnumerable<DOD_Affiliation> DODAffiliationList { get; set; }
        public IEnumerable<Service_Status_Type> ServiceStatusTypes { get; set; }

        public Reservation reservationObj { get; set; }
        public Customer customerObj { get; set; }

        public int reservationID { get; set; }
        public  DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string fullName { get; set; }
        public string siteNumber { get; set; }
        public string customerID { get; set; }

    }
}
