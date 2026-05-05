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
    public class OrderDAO : IOrderDAO
    {
        private readonly UCL_TicketingDbContext _context;

        public OrderDAO(UCL_TicketingDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Order entity)
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

        public async Task DeleteAsync(Order entity)
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

        public async Task<Order?> FindByIdAsync(int Id)
        {
            return await _context.Orders
                .Include(o => o.Tickets)
                    .ThenInclude(t => t.Match)
                    .ThenInclude(m => m.HomeClub)
                .Include(o => o.Tickets)
                    .ThenInclude(t => t.Match)
                    .ThenInclude(m => m.AwayClub)
                .Include(o => o.Tickets)
                    .ThenInclude(t => t.Vak)
                .Include(o => o.Abonnement)
                .Include(o => o.User)
                .AsNoTracking()
                .SingleOrDefaultAsync(o => o.Id == Id);
        }

        public async Task<IEnumerable<Order>?> GetAllAsync()
        {
            try
            {
                return await _context.Orders
                    .AsNoTracking()
                    .Include(o => o.Tickets)
                    .Include(o => o.User)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in de DAO");
                throw;
            }
        }

        public async Task UpdateAsync(Order entity)
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

        public async Task<List<Order>> GetOrdersByUserIdAsync(string userId)
        {
            return await _context.Orders
                .Where(o => o.UserId.Equals(userId))
                .Include(o => o.Tickets)
                    .ThenInclude(t => t.Match)
                    .ThenInclude(m => m.HomeClub)
                .Include(o => o.Tickets)
                    .ThenInclude(t => t.Match)
                    .ThenInclude(m => m.AwayClub)
                .Include(o => o.Tickets)
                    .ThenInclude(t => t.Vak)
                .Include(o => o.Abonnement)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
