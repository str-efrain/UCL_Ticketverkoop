using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domains.Entities;
using Repositories;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services
{
    internal class TicketService : IService<Ticket>
    {
        public Task AddAsync(Ticket entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Ticket entity)
        {
            throw new NotImplementedException();
        }

        public Task<Ticket?> FindByIdAsync(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Ticket>?> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Ticket entity)
        {
            throw new NotImplementedException();
        }


    }
}
