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
    public class MatchDAO : IMatchDAO
    {
        private readonly UCL_TicketingDbContext _context;

        public MatchDAO(UCL_TicketingDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Match entity)
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

        public async Task DeleteAsync(Match entity)
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

        public async Task<Match?> FindByIdAsync(int Id)
        {
            return await _context.Matches
                .Include(o => o.Tickets)
                .AsNoTracking()
                .SingleOrDefaultAsync(o => o.Id == Id);
        }

        public async Task<IEnumerable<Match>?> GetAllAsync()
        {
            try
            {
                return await _context.Matches
                    .Include(h=>h.HomeClub)
                        .ThenInclude(c => c.Stadium)
                    .Include(a=>a.AwayClub)
                    .AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in de DAO");
                throw;
            }
        }
        public async Task<IEnumerable<Match>?> GetByClubAsync(int clubId)
        {
            try 
            {
                return await _context.Matches
                    .Include(h => h.HomeClub)
                        .ThenInclude(c => c.Stadium)
                    .Include(a => a.AwayClub)
                    .Where(m => m.HomeClubId == clubId || m.AwayClubId == clubId).ToListAsync();
            }
            catch 
            {
                return null;
            }
        }

        public async Task UpdateAsync(Match entity)
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
