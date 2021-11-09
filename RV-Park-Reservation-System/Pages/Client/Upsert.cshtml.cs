using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RV_Park_Reservation_System.Pages.Client
{
    public class UpsertModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<Customer> _userManager;

        public UpsertModel(IUnitOfWork unitOfWork, UserManager<Customer> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;

        }

        public IEnumerable<SelectListItem> vehicleTypes { get; set; }

        public IEnumerable<SelectListItem> sites { get; set; }

        [BindProperty]
        public int vehicleType { get; set; }

        public List<Special_Event> specialEvents { get; set; }



        public string jsonFeed { get; set; }

        [BindProperty]
        public int numberOfAdults { get; set; }
        [BindProperty]
        [Range(0, 10)]
        public int numberOfChildren { get; set; }
        [BindProperty]
        [Range(0, 2)]
        public int numberOfPets { get; set; }
        [BindProperty]
        public bool breedPolicy { get; set; }
        [BindProperty]
        public int vehicleLength { get; set; }
        [BindProperty]
        public DateTime StartDate { get; set; }
        [BindProperty]
        public DateTime EndDate { get; set; }
        [BindProperty]
        public int siteid { get; set; }
        [BindProperty]
        public bool Error { get; set; } = false;
        [BindProperty]
        public bool fullHookups { get; set; }

        [BindProperty]
        public decimal totalCost { get; set; }

        public Reservation newReservation { get; set; }

        [BindProperty]
        public int Id { get; set; }





        public void OnGet(bool? error, int id)
        {
            if (error != null)
            {
                Error = (bool)error;
            }
            vehicleTypes = _unitOfWork.Vehicle_Type.List().Select(f => new SelectListItem { Value = f.TypeID.ToString(), Text = f.TypeName + " " + f.TypeDescription });

            sites = _unitOfWork.Site.List().Select(f => new SelectListItem { Value = f.SiteID.ToString(), Text = "Lot " + f.SiteID.ToString() });
            Id = id;



        }


        public async Task<IActionResult> OnPost()
        {

            if (ModelState.IsValid)
            {

                var reservation = _unitOfWork.Reservation.Get(u => u.ResID == Id);
                Customer customer = _unitOfWork.Customer.Get(c => c.CustEmail == User.Identity.Name);
                reservation.ResAcknowledgeValidPets = breedPolicy;
                reservation.ResStartDate = StartDate;
                reservation.ResEndDate = EndDate;
                reservation.ResNumAdults = numberOfAdults;
                reservation.ResNumChildren = numberOfChildren;
                reservation.ResNumPets = numberOfPets;
                reservation.Vehicle_Type = _unitOfWork.Vehicle_Type.Get(v => v.TypeID == vehicleType);
                reservation.ResCreatedDate = DateTime.Now;
                reservation.SiteID = siteid;
                reservation.ResStatusID = 1;
                reservation.ResLastModifiedBy = User.Identity.Name;
                reservation.ResVehicleLength = vehicleLength;





                _unitOfWork.Reservation.Update(reservation);
                _unitOfWork.Commit();



            }
            else
            {
                Error = true;
                return RedirectToPage("/Client/MyReservations ", new { error = Error });
            }



            return RedirectToPage("/Index");

        }

    }
}


