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
using Stripe;

namespace RV_Park_Reservation_System.Pages.Admin.Reservations
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
    public class AdminBookingModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationCore.Models.Customer> _userManager;

        public AdminBookingModel(IUnitOfWork unitOfWork, UserManager<ApplicationCore.Models.Customer> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;

        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IEnumerable<SelectListItem> vehicleTypes { get; set; }

        public IEnumerable<SelectListItem> sites { get; set; }

        [BindProperty]
        public IEnumerable<SelectListItem> lstServiceStatus { get; set; }

        [BindProperty]
        public IEnumerable<SelectListItem> lstDODAffiliation { get; set; }
    
        [BindProperty]
        public int vehicleType { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email Address")]
            public string emailAddress { get; set; }

            //[Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [Required]
            public string firstName { get; set; }
            [Required]
            public string lastName { get; set; }
            [Required]
            public string phoneNumber { get; set; }
            [Required]
            public int serviceStatusType { get; set; }
            [Required]
            public int DODAffiliationID { get; set; }
            [Required]
            public string accountOption { get; set; } //this is the choice of new or generic account
        }


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

        public ReservationVM reservationVM { get; set; }

        public IActionResult OnGet(bool? error) //may need to change, as it'll kick out an admin if they have only the admin role and not customer
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
            lstServiceStatus = _unitOfWork.Service_Status_Type.List().Select(s => new SelectListItem { Value = s.ServiceStatusID.ToString(), Text = s.ServiceStatusType });
            lstDODAffiliation = _unitOfWork.DOD_Affiliation.List().Select(d => new SelectListItem { Value = d.DODAffiliationID.ToString(), Text = d.DODAffiliationType });

            return Page();
        }


        public async Task<IActionResult> OnPost()
        {

            
            reservationVM = new ReservationVM()
            {
                reservationObj = new Reservation(),
                paymentObj = new Payment(),
                customerObj = new ApplicationCore.Models.Customer()
            };

         
            if (ModelState.IsValid) 
            {
                TimeSpan startTime = new TimeSpan(13, 0, 0);
                StartDate = StartDate.Date + startTime;
                TimeSpan EndTime = new TimeSpan(12, 0, 0);
                EndDate = EndDate.Date + EndTime;

                if(Input.accountOption == "newUser")
                {
                    reservationVM.customerObj.CustFirstName = Input.firstName;
                    reservationVM.customerObj.CustLastName = Input.lastName;
                    reservationVM.customerObj.CustEmail = Input.emailAddress;
                    reservationVM.customerObj.UserName = Input.emailAddress;
                    reservationVM.customerObj.Email = Input.emailAddress;
                    reservationVM.customerObj.CustPhone = Input.phoneNumber;
                    reservationVM.customerObj.DODAffiliationID = Input.DODAffiliationID;
                    reservationVM.customerObj.ServiceStatusID = Input.serviceStatusType;
                    reservationVM.accountChoice = Input.accountOption;

                    reservationVM.reservationObj.ResAcknowledgeValidPets = breedPolicy;
                    reservationVM.reservationObj.ResStartDate = StartDate;
                    reservationVM.reservationObj.ResEndDate = EndDate;
                    reservationVM.reservationObj.ResNumAdults = numberOfAdults;
                    reservationVM.reservationObj.ResNumChildren = numberOfChildren;
                    reservationVM.reservationObj.ResNumPets = numberOfPets;
                    reservationVM.reservationObj.TypeID = vehicleType;
                    reservationVM.reservationObj.ResCreatedDate = DateTime.Now;
                    reservationVM.reservationObj.SiteID = siteid;
                    reservationVM.reservationObj.ResStatusID = 1;
                    reservationVM.reservationObj.ResLastModifiedBy = User.Identity.Name;
                    reservationVM.reservationObj.ResVehicleLength = vehicleLength;
                    //reservationVM.reservationObj.Vehicle_Type = vehicle;


                    reservationVM.paymentObj.PayDate = DateTime.Now;
                    reservationVM.paymentObj.PayLastModifiedBy = User.Identity.Name;
                    reservationVM.paymentObj.PayLastModifiedDate = DateTime.Now;
                    reservationVM.paymentObj.PayReasonID = 1;
                    reservationVM.paymentObj.PayTypeID = 1;
                    reservationVM.paymentObj.IsPaid = false;
                    reservationVM.paymentObj.PayTotalCost = totalCost;
                }
                else
                {
                    reservationVM.customerObj.CustFirstName = Input.firstName;
                    reservationVM.customerObj.CustLastName = Input.lastName;
                    reservationVM.customerObj.CustEmail = Input.emailAddress;
                    reservationVM.customerObj.UserName = Input.emailAddress;
                    reservationVM.customerObj.Email = Input.emailAddress;
                    reservationVM.accountChoice = Input.accountOption;

                    reservationVM.reservationObj.ResAcknowledgeValidPets = breedPolicy;
                    reservationVM.reservationObj.ResStartDate = StartDate;
                    reservationVM.reservationObj.ResEndDate = EndDate;
                    reservationVM.reservationObj.ResNumAdults = numberOfAdults;
                    reservationVM.reservationObj.ResNumChildren = numberOfChildren;
                    reservationVM.reservationObj.ResNumPets = numberOfPets;
                    reservationVM.reservationObj.TypeID = vehicleType;
                    reservationVM.reservationObj.ResCreatedDate = DateTime.Now;
                    reservationVM.reservationObj.SiteID = siteid;
                    reservationVM.reservationObj.ResStatusID = 1;
                    reservationVM.reservationObj.ResLastModifiedBy = User.Identity.Name;
                    reservationVM.reservationObj.ResVehicleLength = vehicleLength;

                    reservationVM.paymentObj.PayDate = DateTime.Now;
                    reservationVM.paymentObj.PayLastModifiedBy = User.Identity.Name;
                    reservationVM.paymentObj.PayLastModifiedDate = DateTime.Now;
                    reservationVM.paymentObj.PayReasonID = 1;
                    reservationVM.paymentObj.PayTypeID = 2;
                    reservationVM.paymentObj.IsPaid = false;
                    reservationVM.paymentObj.PayTotalCost = totalCost;
                }

                if(Input.accountOption == "newUser")
                {
                    if (reservationVM.paymentObj.CCReference == null)
                    {
                        var options = new PaymentIntentCreateOptions
                        {
                            Amount = Convert.ToInt32(reservationVM.paymentObj.PayTotalCost * 100),
                            Currency = "usd",

                            PaymentMethodTypes = new List<string>
                            {
                                "card",
                            },
                        };

                        var service = new PaymentIntentService();
                        var paymentIntent = service.Create(options);
                        reservationVM.paymentObj.CCReference = paymentIntent.Id;
                    }
                    HttpContext.Session.Set(SD.ReservationSession, reservationVM);

                    return RedirectToPage("/Admin/Reservations/AdminPaymentSummary");
                }
                else //if the 'generic user' option is selected
                {
                    HttpContext.Session.Set(SD.ReservationSession, reservationVM);
                    return RedirectToPage("/Admin/Reservations/AdminCashSummary");
                }            
            }
            else
            {
                Error = true;
                return RedirectToPage("/Admin/Reservations/AdminBooking", new { error = Error });
            }
        }
    }
}




