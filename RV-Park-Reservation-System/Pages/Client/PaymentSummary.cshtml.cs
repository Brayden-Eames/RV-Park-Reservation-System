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
using RV_Park_Reservation_System.ViewModels;
using Stripe;

namespace RV_Park_Reservation_System.Pages.Client
{
    public class PaymentSummaryModel : PageModel
    {
        #region injectedServices
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationCore.Models.Customer> _userManager;
        private readonly IEmailSender _emailSender;

        public PaymentSummaryModel(IUnitOfWork unitOfWork, UserManager<ApplicationCore.Models.Customer> userManager, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _emailSender = emailSender;

        }
        #endregion

        #region properties
        [BindProperty]
        public Reservation newReservation { get; set; }

        [BindProperty]
        public Payment paymentObj { get; set; }

        [BindProperty]
        public int reservationID { get; set; }

        [BindProperty]
        public int paymentID { get; set; }

        [BindProperty]
        public string  vehicleType { get; set; }

        public bool Error { get; set; } = false;

        public ReservationVM reservationVM { get; set; }
        #endregion


        public IActionResult OnGet(bool? error)
        {
            //Checks if user is logged in and a customer. 
            if (!User.Identity.IsAuthenticated && !User.IsInRole("Customer"))
            {
                return RedirectToPage("/Shared/Prohibited", new { path = "/Client/PaymentSummary" });
            }

            //Checks if the session is null and returns an error if the session is null.    
            if (HttpContext.Session.Get<ReservationVM>(SD.ReservationSession) != null)
            { 
                //Sets the error state. 
                if (error != null)
                {
                    Error = true;
                }
                //Retrieves reservation view model from the session. 
                reservationVM = HttpContext.Session.Get<ReservationVM>(SD.ReservationSession);

                //Sets the reservation and payment objects from the session. 
                newReservation = reservationVM.reservationObj;
                paymentObj = reservationVM.paymentObj;

                //Gets the vehicle type for the summary page. 
                vehicleType = _unitOfWork.Vehicle_Type.Get(v => v.TypeID == newReservation.TypeID).TypeName;

            }
            else
            {
                //Session is empty. 
                Error = true;

            }
            return Page();
        }



        public async Task<IActionResult> OnPost()
        {
            //Gets reservationVM from session and sets it to reservationVM
            if (HttpContext.Session.Get<ReservationVM>(SD.ReservationSession) != null)
            {
               reservationVM = HttpContext.Session.Get<ReservationVM>(SD.ReservationSession);
            }


            //Creates a payment intent service though stripes api. 
            var service = new PaymentIntentService();
            PaymentIntent paymentIntent; 


            //"Pings" the selected payment intent until stripe updates it to succeeded and payment is fully processed or some error occurs. 
            for (int i = 0; i < 5; i++)
            {
                //Waits one second between iterations. 
                System.Threading.Thread.Sleep(1000);

                //Gets the payment intent
                paymentIntent = service.Get(reservationVM.paymentObj.CCReference);

                //checks if payment intent succeed and addes it to the payment table in database. 
                if (paymentIntent.Status.ToLower() == "succeeded")
                {
                    //get the current logged in customer. 
                    ApplicationCore.Models.Customer customer = _unitOfWork.Customer.Get(c => c.CustEmail == User.Identity.Name);

                    //Finalizes the payment object. 
                    var reservations = _unitOfWork.Reservation.List().Where(r => r.Customer == customer).Last();
                    reservationVM.paymentObj.ResID = reservations.ResID;
                    reservationVM.paymentObj.IsPaid = true;
                    _unitOfWork.Payment.Add(reservationVM.paymentObj);
                    _unitOfWork.Commit();

                    //Updates the reservation to a scheduled status. 
                    reservations.ResStatusID = 9;
                    _unitOfWork.Reservation.Update(reservations);
                    _unitOfWork.Commit();

                    //Clears the session. 
                    HttpContext.Session.Clear();

                    //Sends an email to the user with confirmation code. 
                    var user = await _userManager.FindByEmailAsync(User.Identity.Name);
                    var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Client/MyReservations",
                        pageHandler: null,
                        values: new { area = "", code },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(
                        user.CustEmail,
                        "FamCamp Reservation Confirmation",
                        $"This is a confirmation that your reservation is confirmed and paid in it's entirety. to view this reservation please visit the MyReservation page under your account. " +
                        $"Your reservation confirmation number is " + reservations.ResID.ToString() + "."
                        );


                    //Returns to confirm payment. 
                    return RedirectToPage("/Client/PaymentConfirmation");
                }


            }
            //Some error occured and payment was not completed. Return to current page and display error message. 
            return RedirectToPage("/Client/PaymentSummary", new { error = true });

        }
    }
}
