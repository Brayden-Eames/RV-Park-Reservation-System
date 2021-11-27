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
    public class SpecialEventController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public SpecialEventController(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        [HttpGet]
        public async Task<IActionResult> OnGet()
        {
            return Json(new { data = await _unitOfWork.Special_Event.ListAsync(a => a.EventID != null) });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.Special_Event.Get(c => c.EventID == id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting." });
            }
            _unitOfWork.Special_Event.Delete(objFromDb);
            _unitOfWork.Commit();
            return Json(new { success = true, message = "Delete successful!" });
        }
    }
}
