using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace RV_Park_Reservation_System.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<Customer> _signInManager;
        private readonly UserManager<Customer> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitofWork;


        public RegisterModel(
            UserManager<Customer> userManager,
            SignInManager<Customer> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager,
            IUnitOfWork unitofWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _unitofWork = unitofWork;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [BindProperty]
        public IEnumerable<SelectListItem> lstServiceStatus { get; set; }

        [BindProperty]
        public IEnumerable<SelectListItem> lstDODAffiliation { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            public string FirstName { get; set; }
            [Required]
            public string LastName { get; set; }
            [Required]
            public string PhoneNumber { get; set; }
            [Required]
            public int ServiceStatusID { get; set; }
            [Required]
            public int DODAffiliationID { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            lstServiceStatus = _unitofWork.Service_Status_Type.List().Select(s => new SelectListItem { Value = s.ServiceStatusID.ToString(), Text = s.ServiceStatusType });
            lstDODAffiliation = _unitofWork.DOD_Affiliation.List().Select(d => new SelectListItem { Value = d.DODAffiliationID.ToString(), Text = d.DODAffiliationType });
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            //retrieve the role from the form
            string role = Request.Form["rdUserRole"].ToString();
            if (role == "") { role = SD.CustomerRole; } //when first account is made, this starts the process of assigning the first account the role of admin, but not for subsequent users.)
            returnUrl ??= Url.Content("~/"); //null-coalescing assignment operator ??= assigns the value of right-hand operand to its left-hand operand only if the left-hand is nulll
            if (ModelState.IsValid)
            {
                //expand identityuser with Customer properties
                var user = new Customer
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    CustEmail = Input.Email,
                    CustFirstName = Input.FirstName,
                    CustLastName = Input.LastName,
                    CustPhone = Input.PhoneNumber,
                    ServiceStatusID = Input.ServiceStatusID,
                    DODAffiliationID = Input.DODAffiliationID
                };
                var result = await _userManager.CreateAsync(user, Input.Password);                

                //add the roles to the ASPNET Roles table if they do not exist yet
                if (!await _roleManager.RoleExistsAsync(SD.AdminRole))
                {
                    _roleManager.CreateAsync(new IdentityRole(SD.AdminRole)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new IdentityRole(SD.CustomerRole)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new IdentityRole(SD.EmployeeRole)).GetAwaiter().GetResult();
                }
                if (result.Succeeded)
                //assign role to the user (from the form radio options available after the first manager is created)
                {
                    List<Customer> userList = _unitofWork.Customer.List().ToList();
                    if (userList.Count == 1)
                    {
                        await _userManager.AddToRoleAsync(user, SD.AdminRole);
                    }
                    else if(role == SD.CustomerRole)
                    {
                        await _userManager.AddToRoleAsync(user, SD.CustomerRole);
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, SD.EmployeeRole);
                    }

                    _logger.LogInformation("User created a new account with password.");
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code, returnUrl },
                        protocol: Request.Scheme);
                    await _emailSender.SendEmailAsync(Input.Email, "You have been regiserted! Please confirm your email.",
                       $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
