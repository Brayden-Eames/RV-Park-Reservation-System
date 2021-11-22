using ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RV_Park_Reservation_System.ViewModels
{
    public class SiteRateVM
    {
        public Site_Rate SiteRate { get; set; }

        public IEnumerable<SelectListItem> SiteCategoryList { get; set; }
    }
}
