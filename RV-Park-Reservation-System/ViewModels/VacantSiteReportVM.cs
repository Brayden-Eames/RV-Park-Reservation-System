namespace RV_Park_Reservation_System.ViewModels
{
    public class VacantSiteReportVM
    {       
        public int id { get; set; }
        public string site { get; set; } 
        public int length { get; set; }
        public string description { get; set; }
        public string available { get; set; }
        public decimal rate { get; set; } 
    }
}
