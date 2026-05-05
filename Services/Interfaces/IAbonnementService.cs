using Domains.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IAbonnementService:IService<Abonnement>
    {
        Task<bool> isSeatTakenAsync(int clubId, int vakId, string seat, int seasonId);
    }
}
