using Domains.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IMatchDAO: IDAO<Match>
    {
        Task<IEnumerable<Match>?> GetByClubAsync(int clubId);
        Task<IEnumerable<Match>?> GetBySeasonAsync(int seasonId);
        Task<IEnumerable<Match>?> GetByClubAndSeasonAsync(int clubId, int seasonId);
    }
}
