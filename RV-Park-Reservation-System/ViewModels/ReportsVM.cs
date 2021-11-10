using ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RV_Park_Reservation_System.ViewModels
{
    public class ReportsVM
    {     
        public IEnumerable<SelectListItem> reservationItem { get; set; }
        public IEnumerable<SelectListItem> siteList { get; set; }
        public IEnumerable<SelectListItem> categoryList { get; set; }
        public IEnumerable<SelectListItem> priceList { get; set; }
        public IEnumerable<SelectListItem> customerList { get; set; }
        public IEnumerable<SelectListItem> reservationStatusList { get; set; }


    }
}
