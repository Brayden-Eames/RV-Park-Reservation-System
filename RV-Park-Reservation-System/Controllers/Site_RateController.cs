using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace RV_Park_Reservation_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Site_RateController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public Site_RateController(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        [HttpGet]
        public IActionResult Get()
        {
            return Json(new { data = _unitOfWork.Site_Rate.List() });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.Site_Rate.Get(c => c.RateID == id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting." });
            }
            _unitOfWork.Site_Rate.Delete(objFromDb);
            _unitOfWork.Commit();
            return Json(new { success = true, message = "Delete successful!" });
        }
    }
}
