using Domains.Data;
using Domains.Entities;
using Domains.Exceptions;
using Repositories;
using Repositories.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketDAO _ticketDAO;
        private readonly ISeatService _seatService;
        private readonly IMatchDAO _matchDAO;
        private readonly IDAO<Vak> _vakDAO;
        public TicketService(ITicketDAO ticketDAO,ISeatService seatService, IMatchDAO matchDAO, IDAO<Vak> vakDAO)
        {
            _ticketDAO = ticketDAO;
            _seatService = seatService;
            _matchDAO = matchDAO;
            _vakDAO = vakDAO;
        }
        public async Task AddAsync(Ticket entity, string userId)
        {
            var userTickets = await _ticketDAO.GetTicketsByUserAsync(userId);
            const int maxTicketsPerUserPerMatch = 4;
            const int maxMatchesPerDayPerUser = 1;
            entity.Match = await _matchDAO.FindByIdAsync(entity.MatchId);
            entity.Vak = await _vakDAO.FindByIdAsync(entity.VakId);

            if (entity.Match == null)
                throw new Exception("Match bestaat niet.");

            //Controle max 4 tickets per user per match 
            int countForMatch = userTickets.Count(t => t.MatchId == entity.MatchId);
            if (countForMatch >= maxTicketsPerUserPerMatch)
            {
                throw new TicketLimitExceededException(maxTicketsPerUserPerMatch);
            }

            //controle of seat al is genomen
            if (await _seatService.isSeatTakenAsync(entity.Match.HomeClubId, entity.VakId, entity.Seat, entity.Match.SeizoenId, entity.Match.Id))
            {
                throw new SeatIsTakenException(entity.Seat, entity.Vak.Name, entity.Match.HomeClub.Stadium.Name);
            }

            //Controle max 1 match per dag per user
            if (userTickets.Count(t => t.MatchId != entity.MatchId && t.Match.MatchDate.Date == entity.Match.MatchDate.Date) >= maxMatchesPerDayPerUser)
            {
                throw new MaxTicketsPerMatchDayExeededException(entity.Match.MatchDate, maxMatchesPerDayPerUser);
            }

            //controle of de ticket verkoop al gestart is
            //if(DateTime.Now < entity.Match.MatchDate.AddMonths(-1) || DateTime.Now > entity.Match.MatchDate)
            //    {
            //        throw new TicketSalesNotActiveException(entity.Match.MatchDate.AddMonths(-1),entity.Match.MatchDate);
            //}

            await _ticketDAO.AddAsync(entity);
        }

        public Task AddAsync(Ticket entity)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(Ticket entity)
        {
           await _ticketDAO.DeleteAsync(entity);
        }

        public async Task<Ticket?> FindByIdAsync(int Id)
        {
            return await _ticketDAO.FindByIdAsync(Id);
        }

        public async Task<IEnumerable<Ticket>?> GetAllAsync()
        {
            return await _ticketDAO.GetAllAsync();
        }

        public async Task<List<string>> GetTakenSeatsAsync(int matchId, int vakId)
        {
            return await _ticketDAO.GetTakenSeatsAsync(matchId, vakId);
        }

        public async Task<IEnumerable<Ticket>> GetTicketsByUserAsync(string userId)
        {
            return await _ticketDAO.GetTicketsByUserAsync(userId);
        }

        public async Task<bool> IsSeatTakenAsync(int matchId, int vakId, string seat)
        {
            return await _ticketDAO.IsSeatTakenAsync(matchId, vakId, seat);   
        }

        public async Task UpdateAsync(Ticket entity)
        {
            await _ticketDAO.UpdateAsync(entity);
        }
    }

}
