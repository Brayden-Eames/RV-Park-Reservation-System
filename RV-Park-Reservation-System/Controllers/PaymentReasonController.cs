using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace RV_Park_Reservation_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentReasonController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public PaymentReasonController(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        [HttpGet]
        public IActionResult Get()
        {
            return Json(new { data = _unitOfWork.Payment_Reason.List() });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.Payment_Reason.Get(p => p.PayReasonID == id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting." });
            }
            _unitOfWork.Payment_Reason.Delete(objFromDb);
            _unitOfWork.Commit();
            return Json(new { success = true, message = "Delete successful!" });
        }
    }
}
