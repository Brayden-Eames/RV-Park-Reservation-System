using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RV_Park_Reservation_System.Pages.Admin.VehicleTypes
{
    public class UpsertModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public Vehicle_Type VehicleTypeObj { get; set; }

        public UpsertModel(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public IActionResult OnGet(int? id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Shared/Prohibited", new { path = "/Admin/VehicleTypes/Upsert" });
            }

            VehicleTypeObj = new Vehicle_Type
            {
                TypeName = "",
                TypeDescription = ""
            };

            if (id != 0)
            {
                VehicleTypeObj = _unitOfWork.Vehicle_Type.Get(v => v.TypeID == id, true);
                if (VehicleTypeObj == null)
                {
                    return NotFound();
                }
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (VehicleTypeObj.TypeID == 0) //New Vehicle Type
            {
                _unitOfWork.Vehicle_Type.Add(VehicleTypeObj);
            }
            else //Update
            {
                _unitOfWork.Vehicle_Type.Update(VehicleTypeObj);
            }
            _unitOfWork.Commit();
            return RedirectToPage("Index");
        }
    }
}
