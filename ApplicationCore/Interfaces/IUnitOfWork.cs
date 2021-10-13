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

        int Commit();

        Task<int> CommitAsync();
    }
}
