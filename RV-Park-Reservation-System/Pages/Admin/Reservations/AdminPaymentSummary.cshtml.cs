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
    public class AdminPaymentSummaryModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationCore.Models.Customer> _userManager;
        private readonly IEmailSender _emailSender;

        public AdminPaymentSummaryModel(IUnitOfWork unitOfWork, UserManager<ApplicationCore.Models.Customer> userManager, IEmailSender emailSender)
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
                return RedirectToPage("/Shared/Prohibited", new { path = "/Admin/AdminPaymentSummary" });
            }

            if (HttpContext.Session.Get<ReservationVM>(SD.ReservationSession) != null)
            {
                if (error != null)
                {
                    Error = true;
                }
                reservationVM = HttpContext.Session.Get<ReservationVM>(SD.ReservationSession);  //Getting reservation ViewModel from Session

                newReservation = reservationVM.reservationObj; //Storing reservation viewmodel's reservation object to a new Reservation
                paymentObj = reservationVM.paymentObj;  //Storing reservation viewmodel's payment object to a new payment Object
                customerObj = reservationVM.customerObj;   //Storing reservation viewmodel's customer object to a new Customer Obj
                vehicleType = _unitOfWork.Vehicle_Type.Get(v => v.TypeID == newReservation.TypeID).TypeName;  //Getting Vehicle type name from vehicle type id in Reservation
                ReturnUrl = returnUrl;

            }
            else
            {

                Error = true;

            }
            return Page();
        }
        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            if (HttpContext.Session.Get<ReservationVM>(SD.ReservationSession) != null)
            {
                reservationVM = HttpContext.Session.Get<ReservationVM>(SD.ReservationSession);
            }
            returnUrl ??= Url.Content("~/"); //null-coalescing assignment operator ??= assigns the value of right-hand operand to its left-hand operand only if the left-hand is nulll
            System.Threading.Thread.Sleep(2000);
            var service = new PaymentIntentService();
            var paymentIntent = service.Get(reservationVM.paymentObj.CCReference);


            if (paymentIntent.Status.ToLower() == "succeeded")
            {
                var password = new Password().IncludeNumeric().IncludeLowercase().IncludeUppercase().IncludeSpecial().LengthRequired(12);
                var pwResult = password.Next(); //generating and creating temp password

                var userResult = await _userManager.CreateAsync(reservationVM.customerObj, pwResult); //creating new account with password. and awaitng creation
                await _userManager.AddToRoleAsync(reservationVM.customerObj, SD.CustomerRole); //adds customer role to new account 

                ApplicationCore.Models.Customer customer = _unitOfWork.Customer.Get(c => c.CustFirstName == reservationVM.customerObj.CustFirstName && c.CustLastName == reservationVM.customerObj.CustLastName && c.CustEmail == reservationVM.customerObj.CustEmail);
                reservationVM.reservationObj.Id = customer.Id;
                _unitOfWork.Reservation.Add(reservationVM.reservationObj);
                _unitOfWork.Commit();

                var reservations = _unitOfWork.Reservation.List().Where(r => r.Customer == customer).Last();
                reservationVM.paymentObj.ResID = reservations.ResID;
                reservationVM.paymentObj.IsPaid = true;
                _unitOfWork.Payment.Add(reservationVM.paymentObj);
                _unitOfWork.Commit();
                HttpContext.Session.Clear();

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


                // --- ACCOUNT CREATION CONFIRMATION EMAIL --- // 
                var codeConfirm = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                codeConfirm = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(codeConfirm));
                var callbackUrlConfirm = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = user.Id, code, returnUrl },
                    protocol: Request.Scheme);
                await _emailSender.SendEmailAsync(reservationVM.customerObj.Email, "You have been regiserted! Please confirm your email.",
                   $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                if (_userManager.Options.SignIn.RequireConfirmedAccount)
                {
                    return RedirectToPage("RegisterConfirmation", new { email = reservationVM.customerObj.CustEmail, returnUrl });
                }
                else
                {
                    //await _signInManager.SignInAsync(user, isPersistent: false); //Find out what this does in register
                    //return LocalRedirect(returnUrl);
                }

                // --- TEMP PASSWORD EMAIL --- //
                await _emailSender.SendEmailAsync(
                    reservationVM.customerObj.Email,
                    "FamCamp Account Temporary Password",
                    $"Your FamCamp account has been successfully created! Here is your temporary password:  " +
                    $"<strong>{pwResult}</strong>  " +
                    $" Log in using this password, then we strongly recommend changing your password. ");




                return RedirectToPage("/Client/PaymentConfirmation");
            }
            else
            {
                return RedirectToPage("/Admin/Reservations/AdminPaymentSummary", new { error = true });

            }
        }
    }
}

