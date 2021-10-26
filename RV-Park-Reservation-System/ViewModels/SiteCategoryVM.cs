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

        public IEnumerable<SelectListItem>  LocationList { get; set; }
    }
}
