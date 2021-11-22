using ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RV_Park_Reservation_System.ViewModels
{
    public class SiteCategoryVM
    {
        public Site_Category SiteCategory { get; set; }

        //Commenting out because we will hard code the location ID for the initial project
        //public IEnumerable<SelectListItem>  LocationList { get; set; }
    }
}
