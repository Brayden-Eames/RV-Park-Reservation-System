﻿using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RV_Park_Reservation_System.ViewModels
{
    public class ReservationListVM
    {
        public List<ReservationCalendarVM> ReservationList { get; set; }
    }
}
