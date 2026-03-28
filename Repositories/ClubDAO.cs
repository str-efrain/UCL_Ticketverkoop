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
    public class ClubDAO : IDAO<Club>
    {
        private readonly UCL_TicketingDbContext _context;

        public ClubDAO(UCL_TicketingDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Club entity)
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

        public async Task DeleteAsync(Club entity)
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

        public async Task<Club?> FindByIdAsync(int Id)
        {
            return await _context.Clubs
                .Include(o => o.Abonnements)
                .Include(o => o.MatchHomeClubs)
                .Include(o => o.MatchAwayClubs)
                .AsNoTracking()
                .SingleOrDefaultAsync(o => o.Id == Id);
        }

        public async Task<IEnumerable<Club>?> GetAllAsync()
        {
            try
            {
                return await _context.Clubs.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in de DAO");
                throw;
            }
        }

        public async Task UpdateAsync(Club entity)
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
    }
}
