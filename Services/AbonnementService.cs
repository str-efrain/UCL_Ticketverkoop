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
    public class AbonnementService:IAbonnementService
    {
        private readonly IAbonnementDAO _abonnementDAO;
        public AbonnementService(IAbonnementDAO abonnementDAO)
        {
            _abonnementDAO = abonnementDAO;
        }

        public async Task AddAsync(Abonnement entity)
        {
            await _abonnementDAO.AddAsync(entity);
        }

        public async Task DeleteAsync(Abonnement entity)
        {
            await _abonnementDAO.DeleteAsync(entity);
        }

        public async Task<Abonnement?> FindByIdAsync(int Id)
        {
            return await _abonnementDAO.FindByIdAsync(Id);
        }

        public async Task<IEnumerable<Abonnement>?> GetAllAsync()
        {
            return await _abonnementDAO.GetAllAsync();
        }

        public async Task<bool> isSeatTakenAsync(int clubId, int vakId, string seat, int seasonId)
        {
            return await _abonnementDAO.isSeatTakenAsync(clubId,vakId,seat,seasonId);
        }

        public async Task UpdateAsync(Abonnement entity)
        {
            await _abonnementDAO.UpdateAsync(entity);
        }
    }
}
