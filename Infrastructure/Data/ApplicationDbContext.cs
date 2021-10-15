using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customer { get; set; }
        public DbSet<Location> Location { get; set; }
        public DbSet<Reservation_Status> Reservation_Status { get; set; }
        public DbSet<Payment_Reason> Payment_Reason { get; set; }
        public DbSet<Payment_Type> Payment_Type { get; set; }
        public DbSet<Vehicle_Type> Vehicle_Type { get; set; }
        public DbSet<DOD_Affiliation> DOD_Affiliation { get; set; }
        public DbSet<Service_Status_Type> Service_Status_Type { get; set; }
        public DbSet<Security_Question> Security_Question { get; set; }
        public DbSet<Special_Event> Special_Event { get; set; }
        public DbSet<Site_Category> Site_Category { get; set; }
        public DbSet<Site_Rate> Site_Rate { get; set; }
        public DbSet<Security_Answer> Security_Answer { get; set; }
        public DbSet<Site> Site { get; set; }
        public DbSet<Customer_Password> Customer_Password { get; set; }
        public DbSet<Payment> Payment { get; set; }



    }
}
