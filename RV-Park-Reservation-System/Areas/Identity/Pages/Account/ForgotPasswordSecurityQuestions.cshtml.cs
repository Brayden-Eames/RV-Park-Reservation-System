using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace RV_Park_Reservation_System.Areas.Identity.Pages.Account
{
    public class ForgotPasswordSecurityQuestionsModel : PageModel
    {
        private readonly UserManager<Customer> _userManager;
        private readonly IEmailSender _emailSender;

        public ForgotPasswordSecurityQuestionsModel(UserManager<Customer> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {

            [EmailAddress]
            public string Email { get; set; }

            public string Question1 { get; set; } = "Question 1 Placeholder";

            public string Question2 { get; set; } = "Question 2 Placeholder";

            [Required]
            public string Answer1 { get; set; } = "correct answer placeholder";

            [Required]
            public string Answer2 { get; set; } = "correct answer placeholder";
        }

        public void OnGet(string? email)
        {
            Input.Email = email;

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (Input.Answer1 == "correct answer placeholder" && Input.Answer2 == "correct answer placeholder")
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please 
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(
                    Input.Email,
                    "Reset Password",
                    $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                return RedirectToPage("/ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}
