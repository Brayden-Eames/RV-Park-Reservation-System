﻿using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace RV_Park_Reservation_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class calendarEventsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public calendarEventsController(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        [HttpGet]
        public IActionResult Get()
        {
            return Json(new { data = _unitOfWork.Special_Event.List().ToList() });

        }
    }
}
