using Domains.Data;
using Domains.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class AbonnementDAO : IAbonnementDAO
    {
        private readonly UCL_TicketingDbContext _context;

        public AbonnementDAO(UCL_TicketingDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Abonnement entity)
        {
            _context.Entry(entity).State = EntityState.Added;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task DeleteAsync(Abonnement entity)
        {
            _context.Entry(entity).State = EntityState.Deleted;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public async Task<Abonnement?> FindByIdAsync(int Id)
        {
            return await _context.Abonnements
                .AsNoTracking()
                .SingleOrDefaultAsync(o => o.Id == Id);
        }

        public async Task<IEnumerable<Abonnement>?> GetAllAsync()
        {
            try
            {
                return await _context.Abonnements.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in de DAO");
                throw;
            }
        }

        public async Task UpdateAsync(Abonnement     entity)
        {
            _context.Update(entity).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public async Task<bool> isSeatTakenAsync(int clubId, int vakId, string seat, int seasonId)
        {
            return await _context.Abonnements
                .AnyAsync(a => a.ClubId == clubId &&
                               a.VakId == vakId &&
                               a.Seat == seat &&
                               a.SeizoenId == seasonId);
        }
    }
}
