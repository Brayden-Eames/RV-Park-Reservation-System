using ApplicationCore.Models;

namespace RV_Park_Reservation_System.ViewModels
{
    public class ReservationVM
    {

        public Reservation reservationObj { get; set; }
        public Payment paymentObj { get; set; }
        public Customer customerObj { get; set; }
        public string accountChoice { get; set; }
    }
}
