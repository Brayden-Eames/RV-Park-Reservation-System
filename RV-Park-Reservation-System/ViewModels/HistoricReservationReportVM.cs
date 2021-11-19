using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RV_Park_Reservation_System.ViewModels
{
    public class HistoricReservationReportVM
    {
        public int id { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string siteNumber { get; set; }
        public string checkIn { get; set; }
        public string checkOut { get; set; }      
        public string status { get; set; }

    }
}
