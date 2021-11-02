using System;
using ApplicationCore.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RV_Park_Reservation_System.ViewModels
{
    public class AdminReservationVM
    {
        public IEnumerable<Reservation> Reservations { get; set; }
        public IEnumerable<Customer> ListOfCustomers { get; set; }
        public IEnumerable<DOD_Affiliation> DODAffiliationList { get; set; }
        public IEnumerable<Service_Status_Type> ServiceStatusTypes { get; set; }
    }
}
