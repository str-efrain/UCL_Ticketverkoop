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
    }
}
