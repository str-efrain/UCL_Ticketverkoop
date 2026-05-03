using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domains.Data;
using Domains.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace Repositories
{
    public class TicketDAO : ITicketDAO
    {
        private readonly UCL_TicketingDbContext _context;
        public TicketDAO(UCL_TicketingDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Ticket entity)
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

        public async Task DeleteAsync(Ticket entity)
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

        public async Task<Ticket?> FindByIdAsync(int Id)
        {
            return await _context.Tickets
                .Include(t => t.Match)
                .Include(t => t.Order)
                .Include (t => t.Vak)
                .AsNoTracking()
                .SingleOrDefaultAsync(t => t.Id == Id);
        }

        public async Task<IEnumerable<Ticket>?> GetAllAsync()
        {
            try
            {
                return await _context.Tickets.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in de DAO");
                throw;
            }
        }

        public async Task UpdateAsync(Ticket entity)
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

        public async Task<bool> IsSeatTakenAsync(int matchId, int vakId, string seat)
        {
            return await _context.Tickets
                .AnyAsync(t => t.MatchId == matchId &&
                               t.VakId == vakId &&
                               t.Seat.Equals(seat));
        }

        public async Task<IEnumerable<Ticket>> GetTicketsByUserAsync(String userId)
        {
            return await _context.Tickets
                .Include(t=> t.Order)
                .Where(t => t.Order.UserId == userId)
                .Include(t => t.Match)
                .ToListAsync();


        }
    }


    }
