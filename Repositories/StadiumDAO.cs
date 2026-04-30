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
    public class StadiumDAO : IDAO<Stadium>
    {
        private readonly UCL_TicketingDbContext _context;

        public StadiumDAO(UCL_TicketingDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Stadium entity)
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

        public async Task DeleteAsync(Stadium entity)
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

        public async Task<Stadium?> FindByIdAsync(int Id)
        {
            // Eager-load Clubs (and Vaks) so the navigation collections are populated
            return await _context.Stadia
                .AsNoTracking()
                .Include(s => s.Clubs)
                .Include(s => s.Vaks)
                .SingleOrDefaultAsync(s => s.Id == Id);
        }

        public async Task<IEnumerable<Stadium>?> GetAllAsync()
        {
            try
            {
                return await _context.Stadia
                    .AsNoTracking()
                    .Include(s => s.Clubs)
                    .Include(s => s.Vaks)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in de DAO");
                throw;
            }
        }

        public async Task UpdateAsync(Stadium entity)
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
