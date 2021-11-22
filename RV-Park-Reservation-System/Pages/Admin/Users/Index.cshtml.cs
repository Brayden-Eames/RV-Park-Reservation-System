using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Models;
using ApplicationCore.Interfaces;
using Infrastructure.Services;

namespace RV_Park_Reservation_System.Pages.Admin.User
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<Customer> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        public IndexModel(UserManager<Customer> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }
        public IEnumerable<Customer> ApplicationUsers { get; set; }
        public Dictionary<string, List<string>> UserRoles { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }

        public async Task<IActionResult> OnGetAsync(bool success = false, string message = null)
        {
            if (!User.Identity.IsAuthenticated || User.IsInRole(SD.CustomerRole) || User.IsInRole(SD.EmployeeRole))
            {
                return RedirectToPage("/Shared/Prohibited", new { path = "/Admin/Users/Index" });
            }

            Success = success;
            Message = message;
            UserRoles = new Dictionary<string, List<string>>();
            ApplicationUsers = _unitOfWork.Customer.List();
            foreach (var user in ApplicationUsers)
            {
                var userRole = await _userManager.GetRolesAsync(user);
                UserRoles.Add(user.Id, userRole.ToList());
            }

            return Page();
        }
        public async Task<IActionResult> OnPostLockUnlock(string id)
        {
            var user = _unitOfWork.Customer.Get(u => u.Id == id);
            if (user.LockoutEnd == null)
            {
                user.LockoutEnd = DateTime.Now.AddYears(100);
            }
            else if (user.LockoutEnd > DateTime.Now)
            {
                user.LockoutEnd = DateTime.Now;
            }
            else
            {
                user.LockoutEnd = DateTime.Now.AddYears(100);
            }
            _unitOfWork.Customer.Update(user);
            await _unitOfWork.CommitAsync();
            return RedirectToPage();
        }
    }
}


