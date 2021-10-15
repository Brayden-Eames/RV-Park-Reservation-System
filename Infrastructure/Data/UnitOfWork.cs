using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext; //dependency injection

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private IGenericRepository<Customer> _Customer;

        private IGenericRepository<Location> _Location;

        private IGenericRepository<Reservation_Status> _Reservation_Status;

        private IGenericRepository<Payment_Reason> _Payment_Reason;

        private IGenericRepository<Payment_Type> _Payment_Type;

        private IGenericRepository<Vehicle_Type> _Vehicle_Type;

        private IGenericRepository<DOD_Affiliation> _DOD_Affiliation;

        private IGenericRepository<Service_Status_Type> _Service_Status_Type;

        private IGenericRepository<Security_Question> _Security_Question;

        private IGenericRepository<Special_Event> _Special_Event;

        private IGenericRepository<Site_Category> _Site_Category;

        private IGenericRepository<Site_Rate> _Site_Rate;

        private IGenericRepository<Security_Answer> _Security_Answer;

        private IGenericRepository<Site> _Site;

        private IGenericRepository<Customer_Password> _Customer_Password;

        private IGenericRepository<Payment> _Payment;

        private IGenericRepository<Reservation> _Reservation;

        public IGenericRepository<Reservation> Reservation
        {
            get
            {
                if (_Reservation == null) _Reservation = new GenericRepository<Reservation>(_dbContext);
                return _Reservation;
            }
        }

        public IGenericRepository<Payment> Payment
        {
            get
            {
                if (_Payment == null) _Payment = new GenericRepository<Payment>(_dbContext);
                return _Payment;
            }
        }
        public IGenericRepository<Customer_Password> Customer_Password
        {
            get
            {
                if (_Customer_Password == null) _Customer_Password = new GenericRepository<Customer_Password>(_dbContext);
                return _Customer_Password;
            }
        }

        public IGenericRepository<Security_Answer> Security_Answer
        {
            get
            {
                if (_Security_Answer == null) _Security_Answer = new GenericRepository<Security_Answer>(_dbContext);
                return _Security_Answer;
            }
        }

        public IGenericRepository<Site> Site
        {
            get
            {
                if (_Site == null) _Site = new GenericRepository<Site>(_dbContext);
                return _Site;
            }
        }

        public IGenericRepository<Site_Rate> Site_Rate
        {
            get
            {
                if (_Site_Rate == null) _Site_Rate = new GenericRepository<Site_Rate>(_dbContext);
                return _Site_Rate;
            }
        }
        public IGenericRepository<Site_Category> Site_Category
        {
            get
            {
                if (_Site_Category == null) _Site_Category = new GenericRepository<Site_Category>(_dbContext);
                return _Site_Category;
            }
        }
        public IGenericRepository<Special_Event> Special_Event
        {
            get
            {
                if (_Special_Event == null) _Special_Event = new GenericRepository<Special_Event>(_dbContext);
                return _Special_Event;
            }
        }
        public IGenericRepository<Security_Question> Security_Question
        {
            get
            {
                if (_Security_Question == null) _Security_Question = new GenericRepository<Security_Question>(_dbContext);
                return _Security_Question;
            }
        }
        public IGenericRepository<Service_Status_Type> Service_Status_Type
        {
            get
            {
                if (_Service_Status_Type == null) _Service_Status_Type = new GenericRepository<Service_Status_Type>(_dbContext);
                return _Service_Status_Type;
            }
        }

        public IGenericRepository<DOD_Affiliation> DOD_Affiliation
        {
            get
            {
                if (_DOD_Affiliation == null) _DOD_Affiliation = new GenericRepository<DOD_Affiliation>(_dbContext);
                return _DOD_Affiliation;
            }
        }

        public IGenericRepository<Vehicle_Type> Vehicle_Type
        {
            get
            {
                if (_Vehicle_Type == null) _Vehicle_Type = new GenericRepository<Vehicle_Type>(_dbContext);
                return _Vehicle_Type;
            }
        }

        public IGenericRepository<Payment_Type> Payment_Type
        {
            get
            {
                if (_Payment_Type == null) _Payment_Type = new GenericRepository<Payment_Type>(_dbContext);
                return _Payment_Type;
            }
        }
        public IGenericRepository<Customer> Customer
        {
            get
            {
                if (_Customer == null) _Customer = new GenericRepository<Customer>(_dbContext);
                return _Customer;
            }
        }

        public IGenericRepository<Location> Location
        {
            get
            {
                if (_Location == null) _Location = new GenericRepository<Location>(_dbContext);
                return _Location;
            }
        }

        public IGenericRepository<Reservation_Status> Reservation_Status
        {
            get
            {
                if (_Reservation_Status == null) _Reservation_Status = new GenericRepository<Reservation_Status>(_dbContext);
                return _Reservation_Status;
            }
        }

        public IGenericRepository<Payment_Reason> Payment_Reason
        {
            get
            {
                if (_Payment_Reason == null) _Payment_Reason = new GenericRepository<Payment_Reason>(_dbContext);
                return _Payment_Reason;
            }
        }


        public int Commit()
        {
            return _dbContext.SaveChanges();
        }

        public async Task<int> CommitAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public void Dispose() => _dbContext.Dispose();
    }
}
