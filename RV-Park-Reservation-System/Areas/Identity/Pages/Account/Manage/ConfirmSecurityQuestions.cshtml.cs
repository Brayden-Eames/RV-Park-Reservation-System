using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RV_Park_Reservation_System.Areas.Identity.Pages.Account.Manage
{
    public class ConfirmSecurityQuestionsModel : PageModel
    {
        private readonly UserManager<Customer> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IUnitOfWork _unitOfWork;

        public ConfirmSecurityQuestionsModel(UserManager<Customer> userManager, IEmailSender emailSender, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IEnumerable<Security_Answer> Answers { get; set; }

        public IEnumerable<Security_Question> questions { get; set; }

        public bool Error { get; set; } = false;

        public string email { get; set; }

        public class InputModel
        {

            [EmailAddress]
            public string Email { get; set; }

            public IEnumerable<SelectListItem> ddlQuestion { get; set; }


            public string Question1 { get; set; } 

            public string Question2 { get; set; } 



            [Required]
            public string Answer1 { get; set; } 

            [Required]
            public string Answer2 { get; set; } 
        }

        public async Task OnGet()
        {


            Input = new InputModel();
            
            //Finds User
           
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);

            //Gets answers and questions
            Answers = await _unitOfWork.Security_Answer.ListAsync(a => a.Id == user.Id);

            if (Answers.ToList().Count() == 0)
            {
                RedirectToPage("./UpsertQuestions", new { error = Error });
            }
            else
            {
            Input.Question1 = _unitOfWork.Security_Question.GetById(Answers.ToList()[0].QuestionID).QuestionText;
            Input.Question2 = _unitOfWork.Security_Question.GetById(Answers.ToList()[1].QuestionID).QuestionText;
            }





        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(User.Identity.Name);
                Answers = await _unitOfWork.Security_Answer.ListAsync(a => a.Id == user.Id);
                if (Input.Answer1 != Answers.ToList()[0].AnswerText || Input.Answer2 != Answers.ToList()[1].AnswerText)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return Page();
                }

                // For more information on how to enable account confirmation and password reset please 
                // visit https://go.microsoft.com/fwlink/?LinkID=532713


                return RedirectToPage("./UpsertSecurityQuestions");
            }

            return Page();
        }
    }
}

