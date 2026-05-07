using Domains.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ITicketService: IService<Ticket>
    {
        Task AddAsync(Ticket entity, string userId);
        Task<bool> IsSeatTakenAsync(int matchId, int vakId, string seat);
        Task<List<string>> GetTakenSeatsAsync(int matchId, int vakId);
        Task<IEnumerable<Ticket>> GetTicketsByUserAsync(String userId);
        Task CancelTicketAsync(int ticketId, string userId);
    }
}
