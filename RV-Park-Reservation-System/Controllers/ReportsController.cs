using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RV_Park_Reservation_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : Controller
    {
        private readonly IUnitOfWork _unitofWork;
        private readonly IWebHostEnvironment _hostingEnv;
        public ReportsController(IUnitOfWork unitofWork, IWebHostEnvironment hostingEnv)
        {
            _unitofWork = unitofWork;
            _hostingEnv = hostingEnv;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Json(new { data = _unitofWork.Reservation.List(null, null, "Site,Site_Category,Site_Rate,Customer,Reservation_Status")});
        }
   
    }
}
