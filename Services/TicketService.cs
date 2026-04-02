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
        private readonly IDAO<Ticket> _ticketDAO;
        public TicketService(IDAO<Ticket> ticketDAO)
        {
            _ticketDAO = ticketDAO;
        }
        public async Task AddAsync(Ticket entity)
        {
           await _ticketDAO.AddAsync(entity);
        }

        public async Task DeleteAsync(Ticket entity)
        {
            await _ticketDAO.DeleteAsync(entity);
        }

        public async Task<Ticket?> FindByIdAsync(int Id)
        {
           return await _ticketDAO.FindByIdAsync(Id);
        }

        public async Task<IEnumerable<Ticket>?> GetAllAsync()
        {
            return await _ticketDAO.GetAllAsync();
        }

        public async Task UpdateAsync(Ticket entity)
        {
          await _ticketDAO.UpdateAsync(entity);
        }


    }

}
