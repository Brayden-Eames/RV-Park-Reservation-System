using ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace RV_Park_Reservation_System.ViewModels
{
    public class SiteRateVM
    {
        public Site_Rate SiteRate { get; set; }

        public IEnumerable<SelectListItem> SiteCategoryList { get; set; }
    }
}
