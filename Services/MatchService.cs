using Domains.Entities;
using Repositories.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class MatchService : IService<Match>
    {
        private readonly IDAO<Match> _matchDAO;

        public MatchService(IDAO<Match> matchDAO)
        {
            _matchDAO = matchDAO;
        }
        public async Task AddAsync(Match entity)
        {
            await _matchDAO.AddAsync(entity);
        }

        public async Task DeleteAsync(Match entity)
        {
            await _matchDAO.DeleteAsync(entity);
        }

        public async Task<Match?> FindByIdAsync(int Id)
        {
            return await _matchDAO.FindByIdAsync(Id);
        }

        public async Task<IEnumerable<Match>?> GetAllAsync()
        {
            return await _matchDAO.GetAllAsync();
        }

        public async Task UpdateAsync(Match entity)
        {
            await _matchDAO.UpdateAsync(entity);
        }
    }
}
