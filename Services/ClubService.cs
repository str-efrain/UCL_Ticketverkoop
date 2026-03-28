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
    public class ClubService : IService<Club>
    {
        private readonly IDAO<Club> _clubDAO;

        public ClubService(IDAO<Club> clubDAO)
        {
            _clubDAO = clubDAO;
        }
        public async Task AddAsync(Club entity)
        {
            await _clubDAO.AddAsync(entity);
        }

        public async Task DeleteAsync(Club entity)
        {
            await _clubDAO.DeleteAsync(entity);
        }

        public async Task<Club?> FindByIdAsync(int Id)
        {
            return await _clubDAO.FindByIdAsync(Id);
        }

        public async Task<IEnumerable<Club>?> GetAllAsync()
        {
            return await _clubDAO.GetAllAsync();
        }

        public async Task UpdateAsync(Club entity)
        {
            await _clubDAO.UpdateAsync(entity);
        }
    }
}
