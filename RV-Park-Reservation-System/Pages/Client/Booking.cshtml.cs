using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using System.Text;
using System.Threading;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;

namespace RV_Park_Reservation_System.Pages.Client
{
    public class BookingModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<Customer> _userManager;

        public BookingModel(IUnitOfWork unitOfWork, UserManager<Customer> userManager) 
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

        public Reservation newReservation { get; set; }






        public void OnGet(bool? error)
        {
            if (error != null)
            {
                Error = (bool)error;
            }
            vehicleTypes = _unitOfWork.Vehicle_Type.List().Select(f => new SelectListItem { Value = f.TypeID.ToString(), Text = f.TypeName + " " + f.TypeDescription });

            sites = _unitOfWork.Site.List().Select(f => new SelectListItem { Value = f.SiteID.ToString(), Text = "Lot " + f.SiteID.ToString() });



        }


        public async Task<IActionResult> OnPost()
        {

            if (ModelState.IsValid)
            {
                newReservation = new Reservation();
                Customer customer = _unitOfWork.Customer.Get(c => c.CustEmail == User.Identity.Name);
                newReservation.Id = await _userManager.GetUserIdAsync(customer);
                newReservation.ResAcknowledgeValidPets = breedPolicy;
                newReservation.ResStartDate = StartDate;
                newReservation.ResEndDate = EndDate;
                newReservation.ResNumAdults = numberOfAdults;
                newReservation.ResNumChildren = numberOfChildren;
                newReservation.ResNumPets = numberOfPets;
                newReservation.VehicleTypeID = vehicleType;
                newReservation.Vehicle_Type = _unitOfWork.Vehicle_Type.Get(v=>v.TypeID == vehicleType);
                newReservation.ResCreatedDate = DateTime.Now;
                newReservation.SiteID = siteid;
                newReservation.ResStatusID = 1;
                newReservation.ResLastModifiedBy = "customer";
                newReservation.ResVehicleLength = vehicleLength;

                _unitOfWork.Reservation.Add(newReservation);
                _unitOfWork.Commit(); 

            }
            else
            {
                Error = true;
                return RedirectToPage("/Client/Booking", new { error = Error});
            }



            return RedirectToPage("/Index");

        }

       

    }
}
