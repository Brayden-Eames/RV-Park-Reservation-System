using ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RV_Park_Reservation_System.ViewModels
{
    public class SpecialEventVM
    {
        public Special_Event SpecialEvent { get; set; }

        public IEnumerable<SelectListItem>  LocationList { get; set; }
    }
}
