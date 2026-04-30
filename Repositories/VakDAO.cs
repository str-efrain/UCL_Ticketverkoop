using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domains.Data;
using Domains.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace Repositories
{
    public class VakDAO : IDAO<Vak>
    {
        private readonly UCL_TicketingDbContext _context;

        public VakDAO(UCL_TicketingDbContext context)
        {
            _context = context;
        }

            public async Task AddAsync(Vak entity)
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

        public async Task DeleteAsync(Vak entity)
        {
            _context.Entry(entity).State = EntityState.Deleted;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task<Vak?> FindByIdAsync(int Id)
        {
            return await _context.Vaks
                .Include(v => v.Stadium)
                .Include(v => v.Tickets)
                .Include(v => v.Abonnements)
                .AsNoTracking()
                .SingleOrDefaultAsync(v => v.Id == Id);
        }

        public async Task<IEnumerable<Vak>?> GetAllAsync()
        {
            try
            {
                return await _context.Vaks
                    .Include(v => v.Stadium)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in VakDAO.GetAllAsync: " + ex);
                throw;
            }
        }

        public async Task UpdateAsync(Vak entity)
        {
            _context.Update(entity).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
