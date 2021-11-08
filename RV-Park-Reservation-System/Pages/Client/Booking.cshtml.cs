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

using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using RV_Park_Reservation_System.ViewModels;
using Infrastructure.Services;
using System.Web;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace RV_Park_Reservation_System.Pages.Client
{
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }

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

        [BindProperty]
        public decimal totalCost { get; set; }

        public Reservation newReservation { get; set; }


        public IActionResult OnGet(bool? error)
        {
            if (!User.Identity.IsAuthenticated && !User.IsInRole("Customer"))
            {
                return RedirectToPage("/Shared/Prohibited", new { path = "/Client/Booking" });
            }

            if (error != null)
            {
                Error = (bool)error;
            }
            vehicleTypes = _unitOfWork.Vehicle_Type.List().Select(f => new SelectListItem { Value = f.TypeID.ToString(), Text = f.TypeName });

            sites = _unitOfWork.Site.List().Select(f => new SelectListItem { Value = f.SiteID.ToString(), Text = "Lot " + f.SiteID.ToString() });


            return Page();
        }


        public async Task<IActionResult> OnPost()
        {

            reservationVM = new ReservationVM()
            {
                reservationObj = new Reservation(),
                paymentObj = new Payment()
            };


            if (ModelState.IsValid)
            {
     

                reservationVM.reservationObj.ResAcknowledgeValidPets = breedPolicy;
                reservationVM.reservationObj.ResStartDate = StartDate;
                reservationVM.reservationObj.ResEndDate = EndDate;
                reservationVM.reservationObj.ResNumAdults = numberOfAdults;
                reservationVM.reservationObj.ResNumChildren = numberOfChildren;
                reservationVM.reservationObj.ResNumPets = numberOfPets;
                reservationVM.reservationObj.TypeID = vehicleType;
                //reservationVM.reservationObj.Vehicle_Type = _unitOfWork.Vehicle_Type.Get(v=>v.TypeID == vehicleType);
                reservationVM.reservationObj.ResCreatedDate = DateTime.Now;
                reservationVM.reservationObj.SiteID = siteid;
                reservationVM.reservationObj.ResStatusID = 1;
                reservationVM.reservationObj.ResLastModifiedBy = User.Identity.Name;
                reservationVM.reservationObj.ResVehicleLength = vehicleLength;

                reservationVM.paymentObj.PayDate = DateTime.Now;
                reservationVM.paymentObj.PayLastModifiedBy = User.Identity.Name;
                reservationVM.paymentObj.PayLastModifiedDate = DateTime.Now;
                reservationVM.paymentObj.PayReasonID = 1;
                reservationVM.paymentObj.PayTypeID = 1;
                //reservationVM.paymentObj.ResID = 0;
                reservationVM.paymentObj.IsPaid = false;
                reservationVM.paymentObj.PayTotalCost = totalCost;

                HttpContext.Session.Set(SD.ReservationSession, reservationVM);

                var reservationVMtest = HttpContext.Session.Get<ReservationVM>(SD.ReservationSession);
                
                return RedirectToPage("/Client/PaymentSummary");
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
