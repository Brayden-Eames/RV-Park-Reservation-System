using ApplicationCore.Interfaces;
using ApplicationCore.Models;
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
    public class SiteCategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public SiteCategoryController(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        [HttpGet]
        public async Task<IActionResult> OnGet()
        {
            return Json(new { data = await _unitOfWork.Site_Category.ListAsync( a => a.SiteCategory != null) });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> OnDelete(int id)
        {
            var objFromDb = await _unitOfWork.Site_Category.GetAsync(c => c.SiteCategory == id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting." });
            }
            _unitOfWork.Site_Category.Delete(objFromDb);
            _unitOfWork.Commit();
            return Json(new { success = true, message = "Delete successful!" });
        }
    }
}
