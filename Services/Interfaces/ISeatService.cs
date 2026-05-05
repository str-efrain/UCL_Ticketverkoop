using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ISeatService
    {
        Task<bool> isSeatTakenAsync(int clubId, int vakId, string seat, int seasonId ,int? matchId = null);
    }
}
