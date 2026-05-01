using Domains.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{

    public interface ITicketDAO : IDAO<Ticket>
    {
        Task<bool> IsSeatTakenAsync(int matchId, int vakId, int seat);
    }
}
