using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RV_Park_Reservation_System.ViewModels
{
    public class ReservationCalendarVM
    {
        public Reservation ReservationObj { get; set; }
        public Customer CustomerObj { get; set; }
    }
}
