using ApplicationCore.Interfaces;
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
    public class VehicleTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public VehicleTypeController(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        [HttpGet]
        public async Task<IActionResult> OnGet()
        {
            return Json(new { data = await _unitOfWork.Vehicle_Type.ListAsync(a => a.TypeID != null) });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> OnDelete(int id)
        {
            var objFromDb = await _unitOfWork.Vehicle_Type.GetAsync(v => v.TypeID == id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting." });
            }
            _unitOfWork.Vehicle_Type.Delete(objFromDb);
            _unitOfWork.Commit();
            return Json(new { success = true, message = "Delete successful!" });
        }
    }
}
