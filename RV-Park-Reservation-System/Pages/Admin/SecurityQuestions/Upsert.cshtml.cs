using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RV_Park_Reservation_System.Pages.Admin.SecurityQuestions
{
    public class UpsertModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public Security_Question SecurityQuestionObj { get; set; }

        public UpsertModel(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public IActionResult OnGet(int? id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Shared/Prohibited", new { path = "/Admin/SecurityQuestions/Upsert" });
            }

            SecurityQuestionObj = new Security_Question
            {
                QuestionText = ""
            };

            if (id != 0)
            {
                SecurityQuestionObj = _unitOfWork.Security_Question.Get(s => s.QuestionID == id, true);
                if (SecurityQuestionObj == null)
                {
                    return NotFound();
                }
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (SecurityQuestionObj.QuestionID == 0) //New Security Question
            {
                _unitOfWork.Security_Question.Add(SecurityQuestionObj);
            }
            else //Update
            {
                _unitOfWork.Security_Question.Update(SecurityQuestionObj);
            }
            _unitOfWork.Commit();
            return RedirectToPage("../Manage");
        }
    }
}
