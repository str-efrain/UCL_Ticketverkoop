using Domains.Data;
using Domains.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories
{
    public class SeizoenDAO : ISeizoenDAO
    {
        private readonly UCL_TicketingDbContext _context;

        public SeizoenDAO(UCL_TicketingDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Seizoen entity)
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

        public async Task DeleteAsync(Seizoen entity)
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

        public async Task<Seizoen?> FindByIdAsync(int id)
        {
            return await _context.Seizoenen
                .AsNoTracking()
                .SingleOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Seizoen>?> GetAllAsync()
        {
            try
            {
                return await _context.Seizoenen
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in SeizoenDAO");
                throw;
            }
        }

        public async Task<Seizoen> GetCurrentSeasonAsync()
        {
            return await _context.Seizoenen
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.IsCurrentSeason);
        }

        public async Task UpdateAsync(Seizoen entity)
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