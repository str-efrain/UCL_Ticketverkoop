using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domains.Data;
using Domains.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace Repositories
{
    public class TicketDAO : ITicketDAO
    {
        private readonly UCL_TicketingDbContext _context;
        public TicketDAO(UCL_TicketingDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Ticket entity)
        {

        }

        public async Task DeleteAsync(Ticket entity)
        {

        }

        public async Task<Ticket?> FindByIdAsync(int Id)
        {

        }

        public async Task<IEnumerable<Ticket>?> GetAllAsync()
        {

        }

        public async Task UpdateAsync(Ticket entity)
        {

        }

        public async Task<IEnumerable<Ticket>?> getByUserId()
        {
            throw new NotImplementedException();

        }


}
