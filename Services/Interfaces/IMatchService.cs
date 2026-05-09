using Domains.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IMatchService : IService<Match>
    {
        Task<IEnumerable<Match>?> GetByClubAsync(int clubId);
        Task<IEnumerable<Match>?> GetBySeasonAsync(int seasonId);
        Task<IEnumerable<Match>?> GetByClubAndSeasonAsync(int clubId, int seasonId);
    }
}
