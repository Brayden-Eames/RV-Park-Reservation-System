using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using PasswordGenerator;
using RV_Park_Reservation_System.ViewModels;
using Stripe;

namespace RV_Park_Reservation_System.Pages.Admin.Reservations
{
    public class AdminUpdateSummaryModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationCore.Models.Customer> _userManager;
        private readonly IEmailSender _emailSender;

        public AdminUpdateSummaryModel(IUnitOfWork unitOfWork, UserManager<ApplicationCore.Models.Customer> userManager, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _emailSender = emailSender;

        }


        [BindProperty]
        public Reservation newReservation { get; set; }

        [BindProperty]
        public Payment paymentObj { get; set; }

        [BindProperty]
        public ApplicationCore.Models.Customer customerObj { get; set; }

        [BindProperty]
        public int reservationID { get; set; }

        [BindProperty]
        public int paymentID { get; set; }

        [BindProperty]
        public string vehicleType { get; set; }

        public bool Error { get; set; } = false;

        public string ReturnUrl { get; set; }

        public ReservationVM reservationVM { get; set; }

       
        public IActionResult OnGet(bool? error, string returnUrl = null)
        {
           
            if (!User.Identity.IsAuthenticated /*&& !User.IsInRole("Customer")*/) //removed check for customer role, cause the admin won't have that role by default. 
            {
                return RedirectToPage("/Shared/Prohibited", new { path = "/Admin/AdminUpdateSummary" });
            }
            
            if (HttpContext.Session.Get<ReservationVM>(SD.ReservationSession) != null)
            {
                if (error != null)
                {
                    Error = true;
                }
                reservationVM = HttpContext.Session.Get<ReservationVM>(SD.ReservationSession);

                newReservation = new Reservation();
                paymentObj = new Payment();
                //newReservation = reservationVM.reservationObj;
                //Add all attributes manually to the newReservation object, that way we can omit the reservation id (this allows the later code to create a brand new reservation)
                newReservation.Customer = reservationVM.reservationObj.Customer;
                newReservation.Id = reservationVM.reservationObj.Id;
                newReservation.ResAcknowledgeValidPets = reservationVM.reservationObj.ResAcknowledgeValidPets;
                newReservation.ResComment = reservationVM.reservationObj.ResComment;
                newReservation.ResCreatedDate = reservationVM.reservationObj.ResCreatedDate;
                newReservation.ResEndDate = reservationVM.reservationObj.ResEndDate;
                newReservation.Reservation_Status = reservationVM.reservationObj.Reservation_Status;
                newReservation.ResLastModifiedBy = reservationVM.reservationObj.ResLastModifiedBy;
                newReservation.ResLastModifiedDate = reservationVM.reservationObj.ResLastModifiedDate;
                newReservation.ResNumAdults = reservationVM.reservationObj.ResNumAdults;
                newReservation.ResNumChildren = reservationVM.reservationObj.ResNumChildren;
                newReservation.ResNumPets = reservationVM.reservationObj.ResNumPets;
                newReservation.ResStartDate = reservationVM.reservationObj.ResStartDate;
                newReservation.ResStatusID = reservationVM.reservationObj.ResStatusID;
                newReservation.ResVehicleLength = reservationVM.reservationObj.ResVehicleLength;
                newReservation.Site = reservationVM.reservationObj.Site;
                newReservation.SiteID = reservationVM.reservationObj.SiteID;
                newReservation.TypeID = reservationVM.reservationObj.TypeID;
                newReservation.Vehicle_Type = reservationVM.reservationObj.Vehicle_Type;

                //paymentObj = reservationVM.paymentObj;

                paymentObj.CCReference = reservationVM.paymentObj.CCReference;
                paymentObj.IsPaid = reservationVM.paymentObj.IsPaid;
                paymentObj.PayDate = reservationVM.paymentObj.PayDate;
                paymentObj.PayID = reservationVM.paymentObj.PayID;
                paymentObj.PayLastModifiedBy = reservationVM.paymentObj.PayLastModifiedBy;
                paymentObj.PayLastModifiedDate = reservationVM.paymentObj.PayLastModifiedDate;
                paymentObj.Payment_Reason = reservationVM.paymentObj.Payment_Reason;
                paymentObj.Payment_Type = reservationVM.paymentObj.Payment_Type;
                paymentObj.PayReasonID = reservationVM.paymentObj.PayReasonID;
                paymentObj.PayTotalCost = reservationVM.paymentObj.PayTotalCost;
                paymentObj.PayTypeID = reservationVM.paymentObj.PayTypeID;

                customerObj = reservationVM.customerObj;
                vehicleType = _unitOfWork.Vehicle_Type.Get(v => v.TypeID == newReservation.TypeID).TypeName;
                //ReturnUrl = returnUrl;
                
            }
            else
            {

                Error = true;

            }
            return Page();
        }
        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            if (HttpContext.Session.Get<ReservationVM>(SD.ReservationSession) != null) //current problem: 
            {
                reservationVM = HttpContext.Session.Get<ReservationVM>(SD.ReservationSession);
            }
            returnUrl ??= Url.Content("~/"); //null-coalescing assignment operator ??= assigns the value of right-hand operand to its left-hand operand only if the left-hand is nulll
            System.Threading.Thread.Sleep(2000);
            var service = new PaymentIntentService();
            var paymentIntent = service.Get(reservationVM.paymentObj.CCReference);

            if (paymentIntent.Status.ToLower() == "succeeded")
            {
                ApplicationCore.Models.Customer customer = _unitOfWork.Customer.Get(c => c.CustFirstName == reservationVM.customerObj.CustFirstName && c.CustLastName == reservationVM.customerObj.CustLastName && c.CustEmail == reservationVM.customerObj.CustEmail);
                newReservation.Id = customer.Id;
                newReservation.ResLastModifiedBy = reservationVM.reservationObj.ResLastModifiedBy;
                newReservation.ResAcknowledgeValidPets = reservationVM.reservationObj.ResAcknowledgeValidPets;
                newReservation.ResComment = reservationVM.reservationObj.ResComment;
                newReservation.ResCreatedDate = reservationVM.reservationObj.ResCreatedDate;
                newReservation.ResEndDate = reservationVM.reservationObj.ResEndDate;
                newReservation.Reservation_Status = reservationVM.reservationObj.Reservation_Status;
                newReservation.ResLastModifiedBy = reservationVM.reservationObj.ResLastModifiedBy;
                newReservation.ResLastModifiedDate = reservationVM.reservationObj.ResLastModifiedDate;
                newReservation.ResNumAdults = reservationVM.reservationObj.ResNumAdults;
                newReservation.ResNumChildren = reservationVM.reservationObj.ResNumChildren;
                newReservation.ResNumPets = reservationVM.reservationObj.ResNumPets;
                newReservation.ResStartDate = reservationVM.reservationObj.ResStartDate;
                newReservation.ResStatusID = reservationVM.reservationObj.ResStatusID;
                newReservation.ResVehicleLength = reservationVM.reservationObj.ResVehicleLength;
                newReservation.SiteID = reservationVM.reservationObj.SiteID;
                newReservation.TypeID = reservationVM.reservationObj.TypeID;
                newReservation.Vehicle_Type = reservationVM.reservationObj.Vehicle_Type;

                paymentObj.CCReference = reservationVM.paymentObj.CCReference;
                paymentObj.IsPaid = reservationVM.paymentObj.IsPaid;
                paymentObj.PayDate = reservationVM.paymentObj.PayDate;
                //paymentObj.PayID = reservationVM.paymentObj.PayID;
                paymentObj.PayLastModifiedBy = reservationVM.paymentObj.PayLastModifiedBy;
                paymentObj.PayLastModifiedDate = reservationVM.paymentObj.PayLastModifiedDate;
                paymentObj.Payment_Reason = reservationVM.paymentObj.Payment_Reason;
                paymentObj.Payment_Type = reservationVM.paymentObj.Payment_Type;
                paymentObj.PayReasonID = reservationVM.paymentObj.PayReasonID;
                paymentObj.PayTotalCost = reservationVM.paymentObj.PayTotalCost;
                paymentObj.PayTypeID = reservationVM.paymentObj.PayTypeID;

                _unitOfWork.Reservation.Add(newReservation); //Current issue: becauase we are updating the old reservation, we aren't creating a brand new one, just altering the original.
                await _unitOfWork.CommitAsync();

                var reservations = _unitOfWork.Reservation.List().Where(r => r.Customer == customer).Last();
                paymentObj.ResID = reservations.ResID;
                paymentObj.IsPaid = true;
                _unitOfWork.Payment.Add(paymentObj);
                await _unitOfWork.CommitAsync();
                HttpContext.Session.Clear();

               
                
                // --- RESERVATION CANCELLATION EMAIL --- //


                // --- RESERVATION CONFIRMATION EMAIL --- //
                var user = await _userManager.FindByEmailAsync(reservationVM.customerObj.Email);
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Client/MyReservations",
                    pageHandler: null,
                    values: new { area = "", code },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(
                    user.Email,
                    "FamCamp Reservation Confirmation",
                    $"This is a confirmation that your reservation is confirmed and paid in it's entirety. to view this reservation please visit the MyReservation page under your account.  " +
                    $" <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");


                return RedirectToPage("/Client/PaymentConfirmation");
            }
            else
            {
                return RedirectToPage("/Admin/Reservations/AdminPaymentSummary", new { error = true });

            }
        }
    }
}
