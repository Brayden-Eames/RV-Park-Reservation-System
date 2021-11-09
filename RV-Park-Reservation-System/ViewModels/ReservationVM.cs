using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RV_Park_Reservation_System.ViewModels
{
    public class ReservationVM
    {

        public Reservation reservationObj { get; set; }
        public Payment paymentObj { get; set; }
    }
}
