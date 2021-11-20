using ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace RV_Park_Reservation_System.ViewModels
{
    public class SitesVM
    {
        public Site SiteItem { get; set; }       

        public IEnumerable<SelectListItem> CategoryList { get; set; }
       
    }
}
