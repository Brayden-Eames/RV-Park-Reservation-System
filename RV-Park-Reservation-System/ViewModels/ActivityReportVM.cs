namespace RV_Park_Reservation_System.ViewModels
{
    public class ActivityReportVM
    {                                                
        public int id { get; set; }
        public string name { get; set; }
        public string siteNumber { get; set; }
        public string checkIn { get; set; }
        public string checkOut { get; set; }
        public int nights { get; set; }
        public string status { get; set; }

    }
}
