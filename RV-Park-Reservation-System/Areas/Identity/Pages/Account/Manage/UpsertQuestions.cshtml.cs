using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;

namespace RV_Park_Reservation_System.Areas.Identity.Pages.Account.Manage
{
    public class UpsertQuestionsModel : PageModel
    {
        private readonly UserManager<Customer> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public UpsertQuestionsModel(UserManager<Customer> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public IEnumerable<Security_Answer> Answers { get; set; }

        public IEnumerable<Security_Question> questions { get; set; }
        [BindProperty]
        public bool Error { get; set; } = true;

        public class InputModel
        {

            [EmailAddress]
            public string Email { get; set; }

            public IEnumerable<SelectListItem> ddlQuestion { get; set; }

            [BindProperty]
            public string Question1 { get; set; }

            [BindProperty]
            public string Question2 { get; set; }

            [BindProperty]
            public Security_Answer Answer1 { get; set; }

            [BindProperty]
            public Security_Answer Answer2 { get; set; }

            [Required]
            public string Answer1Text { get; set; }
            [Required]
            public string Answer2Text { get; set; }
        }


        public async Task OnGet(bool? error, string? email)
        {
            Input = new InputModel();
            if (error != null)
            {
                Error = (bool)error;
            }
            if (email != null)
            {
                Input.Email = email;
                var user = await _userManager.FindByEmailAsync(Input.Email);
                Answers = await _unitOfWork.Security_Answer.ListAsync(a => a.Id == user.Id);
            }
            else
            {
                var user = await _userManager.FindByEmailAsync(User.Identity.Name);
                Answers = await _unitOfWork.Security_Answer.ListAsync(a => a.Id == user.Id);
            }

            
            
            

            if (Error == true)
            {


                //Gets answers and questions


                if (Answers.ToList().Count() == 0)
                {   
                    Error = false;
                    var questions = _unitOfWork.Security_Question.List();
                    Input.ddlQuestion = questions.Select(q => new SelectListItem { Value = q.QuestionID.ToString(), Text = q.QuestionText });
                }
                else
                {

                    Input.Question1 = _unitOfWork.Security_Question.GetById(Answers.ToList()[0].QuestionID).QuestionText;
                    Input.Question2 = _unitOfWork.Security_Question.GetById(Answers.ToList()[1].QuestionID).QuestionText;
                }
            }
            else
            {
                var questions = _unitOfWork.Security_Question.List();
                Input.ddlQuestion = questions.Select(q => new SelectListItem { Value = q.QuestionID.ToString(), Text = q.QuestionText });
                Input.Question1 = _unitOfWork.Security_Question.GetById(Answers.ToList()[0].QuestionID).QuestionText;
                Input.Question2 = _unitOfWork.Security_Question.GetById(Answers.ToList()[1].QuestionID).QuestionText;
            }
            //Finds User






        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(User.Identity.Name);
                if (Error == true)
                {


                    Answers = await _unitOfWork.Security_Answer.ListAsync(a => a.Id == user.Id);
                    Input.Question1 = _unitOfWork.Security_Question.GetById(Answers.ToList()[0].QuestionID).QuestionText;
                    Input.Question2 = _unitOfWork.Security_Question.GetById(Answers.ToList()[1].QuestionID).QuestionText;
                    if (Input.Answer1Text != Answers.ToList()[0].AnswerText || Input.Answer2Text != Answers.ToList()[1].AnswerText || Input.Question1 == Input.Question2)
                    {

                        // Don't reveal that the user does not exist or is not confirmed
                        return RedirectToPage("./UpsertQuestions", new { error = Error });
                    }

                    // For more information on how to enable account confirmation and password reset please 
                    // visit https://go.microsoft.com/fwlink/?LinkID=532713

                    Error = false;
                    return RedirectToPage("./UpsertQuestions", new { error = Error});
                }
                else
                {
                    if (Input.Question1 == null || Input.Question2 == null || Input.Answer1Text == null || Input.Answer2Text == null || Input.Question1 == Input.Question2)
                    {
                        return RedirectToPage("./UpsertQuestions", new { error = Error });
                    }
                    Input.Answer1 = new Security_Answer();
                    Input.Answer2 = new Security_Answer();

                    Answers = _unitOfWork.Security_Answer.List(a => a.Id == user.Id).ToList();

                    //Deletes existing answers
                    if (Answers.ToList().Count() != 0)
                    {
                        _unitOfWork.Security_Answer.Delete(Answers);
                        _unitOfWork.Commit();
                    }
                    //Answer 2 inputs
                    Input.Answer1.QuestionID = Convert.ToInt32(Input.Question1);
                    Input.Answer1.Id = user.Id;
                    Input.Answer1.AnswerText = Input.Answer1Text;

                    //Answer 1 inputs
                    Input.Answer2.QuestionID = Convert.ToInt32(Input.Question2);
                    Input.Answer2.Id = user.Id;
                    Input.Answer2.AnswerText = Input.Answer2Text;

                    //add answers to table.
                    _unitOfWork.Security_Answer.Add(Input.Answer1);
                    _unitOfWork.Security_Answer.Add(Input.Answer2);
                    _unitOfWork.Commit();




                    return RedirectToPage("/Index");


                }




            }

            return RedirectToPage("./UpsertQuestions", new { error = Error });
        }
    }
}
