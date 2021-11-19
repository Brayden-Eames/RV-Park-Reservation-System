using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace RV_Park_Reservation_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpecialEventController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public SpecialEventController(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        [HttpGet]
        public IActionResult Get()
        {
            return Json(new { data = _unitOfWork.Special_Event.List() });
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
