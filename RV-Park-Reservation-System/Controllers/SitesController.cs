using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace RV_Park_Reservation_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SitesController : Controller
    {
        private readonly IUnitOfWork _unitofWork;
        private readonly IWebHostEnvironment _hostingEnv;
        public SitesController(IUnitOfWork unitofWork, IWebHostEnvironment hostingEnv)
        {
            _unitofWork = unitofWork;
            _hostingEnv = hostingEnv;
        }

        [HttpGet]
        public async Task<IActionResult> OnGet()
        {
            return Json(new { data = await _unitofWork.Site.ListAsync(null, null, "Site_Category") });
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitofWork.Site.Get(s => s.SiteID  == id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
          
            _unitofWork.Site.Delete(objFromDb);
            _unitofWork.Commit();
            return Json(new { success = true, message = "Delete Successful" });
        }
    }
}

