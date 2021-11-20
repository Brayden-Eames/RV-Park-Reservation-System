using ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace RV_Park_Reservation_System.ViewModels
{
    public class UserAccountVM
    {
        public Customer user { get; set; }
        public IEnumerable<SelectListItem> DODAffiliationList { get; set; }
        public IEnumerable<SelectListItem> ServiceStatusTypesList { get; set; }
    }
}
