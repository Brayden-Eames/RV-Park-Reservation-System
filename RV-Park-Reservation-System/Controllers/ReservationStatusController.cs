using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace RV_Park_Reservation_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationStatusController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ReservationStatusController(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        [HttpGet]
        public IActionResult Get()
        {
            return Json(new { data = _unitOfWork.Reservation_Status.List() });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.Reservation_Status.Get(r => r.ResStatusID == id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting." });
            }
            _unitOfWork.Reservation_Status.Delete(objFromDb);
            _unitOfWork.Commit();
            return Json(new { success = true, message = "Delete successful!" });
        }
    }
}
