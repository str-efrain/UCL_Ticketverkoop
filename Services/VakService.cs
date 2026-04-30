    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domains.Entities;
using Repositories;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services
{
    public class VakService : IService<Vak>
    {
        private readonly IDAO<Vak> _vakDAO;

        public VakService(IDAO<Vak> vakDAO)
        {
            _vakDAO = vakDAO;
        }

        public async Task AddAsync(Vak entity)
        {
            await _vakDAO.AddAsync(entity);
        }

        public async Task DeleteAsync(Vak entity)
        {
            await _vakDAO.DeleteAsync(entity);
        }

        public async Task<Vak?> FindByIdAsync(int Id)
        {
            return await _vakDAO.FindByIdAsync(Id);
        }

        public async Task<IEnumerable<Vak>?> GetAllAsync()
        {
            return await _vakDAO.GetAllAsync();
        }

        public async Task UpdateAsync(Vak entity)
        {
            await _vakDAO.UpdateAsync(entity);
        }
    }
}
