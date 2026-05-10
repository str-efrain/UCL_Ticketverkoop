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
        // subscriptions
        .Include(o => o.Abonnements)
            .ThenInclude(a => a.Vak)
        .Include(o => o.Abonnements)
            .ThenInclude(a => a.Seizoen)
        .Include(o => o.Abonnements)
            .ThenInclude(a => a.Club)
                .ThenInclude(c => c.Stadium)

        // tickets -> match -> home/away club + seizoen
        .Include(o => o.Tickets)
            .ThenInclude(t => t.Match)
                .ThenInclude(m => m.HomeClub)
                    .ThenInclude(hc => hc.Stadium)
        .Include(o => o.Tickets)
            .ThenInclude(t => t.Match)
                .ThenInclude(m => m.AwayClub)
                    .ThenInclude(ac => ac.Stadium)
        .Include(o => o.Tickets)
            .ThenInclude(t => t.Match)
                .ThenInclude(m => m.Seizoen)

        // tickets -> vak (and vak's stadium used by VakVM.StadiumName)
        .Include(o => o.Tickets)
            .ThenInclude(t => t.Vak)
                .ThenInclude(v => v.Stadium)

        // user
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
                    .ThenInclude(t => t.Match)
                    .ThenInclude(m => m.HomeClub)
                .Include(o => o.Tickets)
                    .ThenInclude(t => t.Match)
                    .ThenInclude(m => m.AwayClub)
                .Include(o => o.Tickets)
                    .ThenInclude(t => t.Vak)
                .Include(o => o.Abonnements)
                    .ThenInclude(a => a.Club)
                    .ThenInclude(c => c.Stadium)
                .Include(o => o.Abonnements)
                    .ThenInclude(a => a.Vak)
                 .Include(o => o.Abonnements)
                    .ThenInclude(a => a.Seat)
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
                .OrderByDescending(o => o.OrderDate)
                .ThenByDescending(o => o.Id)
                .Include(o => o.Tickets)
                    .ThenInclude(t => t.Match)
                    .ThenInclude(m => m.HomeClub)
                .Include(o => o.Tickets)
                    .ThenInclude(t => t.Match)
                    .ThenInclude(m => m.AwayClub)
                .Include(o => o.Tickets)
                    .ThenInclude(t => t.Vak)
                .Include(o => o.Abonnements)
                    .ThenInclude(a => a.Club)
                    .ThenInclude(c => c.Stadium)
                .Include(o => o.Abonnements)
                    .ThenInclude(a => a.Vak)

                .AsNoTracking()
                .ToListAsync();
        }
    }
}
