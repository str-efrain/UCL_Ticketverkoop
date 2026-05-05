using Domains.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IAbonnementDAO : IDAO<Abonnement>
    {
        Task<bool> isSeatTakenAsync(int clubId, int vakId, string seat, int seasonId);
    }
}
