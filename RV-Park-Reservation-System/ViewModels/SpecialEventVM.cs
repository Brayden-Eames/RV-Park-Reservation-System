using ApplicationCore.Models;

namespace RV_Park_Reservation_System.ViewModels
{
    public class SpecialEventVM
    {
        public Special_Event SpecialEvent { get; set; }

        //Commenting out because we will hard code the location ID for the initial project
        //public IEnumerable<SelectListItem>  LocationList { get; set; }
    }
}
