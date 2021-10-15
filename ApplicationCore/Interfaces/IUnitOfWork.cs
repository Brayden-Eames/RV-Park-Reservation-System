using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IUnitOfWork
    {
   
        public IGenericRepository<Customer> Customer { get; }
        public IGenericRepository<Location> Location { get; }
        public IGenericRepository<Reservation_Status> Reservation_Status { get; }
        public IGenericRepository<Payment_Reason> Payment_Reason { get; }
        public IGenericRepository<Payment_Type> Payment_Type { get; }
        public IGenericRepository<Vehicle_Type> Vehicle_Type { get; }
        public IGenericRepository<DOD_Affiliation> DOD_Affiliation { get; }
        public IGenericRepository<Service_Status_Type> Service_Status_Type { get; }
        public IGenericRepository<Security_Question> Security_Question { get; }
        public IGenericRepository<Special_Event> Special_Event { get; }
        public IGenericRepository<Site_Category> Site_Category { get; }
        public IGenericRepository<Site_Rate> Site_Rate { get; }
        public IGenericRepository<Security_Answer> Security_Answer { get; }
        public IGenericRepository<Site> Site { get; }
        public IGenericRepository<Customer_Password> Customer_Password { get; }
        public IGenericRepository<Payment> Payment { get; }
        public IGenericRepository<Reservation> Reservation { get; }




        int Commit();

        Task<int> CommitAsync();
    }
}
