using Microsoft.AspNetCore.Mvc.RazorPages;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Identity;

namespace RV_Park_Reservation_System.Pages.Admin.Users
{
    public class DeleteModel : PageModel
    {
        public string UserID { get; set; }

        private readonly IUnitOfWork _unitOfWork;

        private readonly UserManager<Customer> _userManager;

        public DeleteModel(IUnitOfWork unitOfWork, UserManager<Customer> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;

        }


        public void OnGet(string ID)
        {
            if (ID != null)
            {
                UserID = ID;
                var customer = _unitOfWork.Customer.Get(u => u.Id == UserID);
                _unitOfWork.Customer.Delete(customer);
                Page();
            }
            else
            {
                RedirectToPage("/Admin/Users/Index");
            }

        }
    }
}
