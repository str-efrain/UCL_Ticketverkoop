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
        private const string CanceledStatus = "Canceled";
        private readonly ITicketDAO _ticketDAO;
        private readonly ISeatService _seatService;
        private readonly IMatchDAO _matchDAO;
        private readonly IDAO<Vak> _vakDAO;
        private readonly UCL_TicketingDbContext _context;
        public TicketService(ITicketDAO ticketDAO,ISeatService seatService, IMatchDAO matchDAO, IDAO<Vak> vakDAO, UCL_TicketingDbContext context)
        {
            _ticketDAO = ticketDAO;
            _seatService = seatService;
            _matchDAO = matchDAO;
            _vakDAO = vakDAO;
            _context = context;
        }
        public async Task AddAsync(Ticket entity, string userId)
        {
            var userTickets = await _ticketDAO.GetTicketsByUserAsync(userId);
            const int maxTicketsPerUserPerMatch = 4;
            const int maxMatchesPerDayPerUser = 1;
            var match = await _matchDAO.FindByIdAsync(entity.MatchId);
            var vak = await _vakDAO.FindByIdAsync(entity.VakId);
            var seat = entity.Seat;

            //controle of seat al is genomen
            //moet voor de addAsync want anders vindt ie zich zelf alsan were,isSeatTaken controleerd ook in local
            if (await _seatService.isSeatTakenAsync(match.HomeClubId, vak.Id, entity.Seat, match.SeizoenId, match.Id))
            {
                throw new SeatIsTakenException(entity.Seat, vak.Name, match.HomeClub.Stadium.Name);
            }


            //Ze worden hier eerst toegevoegd omdat je anders reace conditions hebt bij max 4 tickets per match
            //dit is veilig omdat er een transactie is in de orderservice
            await _ticketDAO.AddAsync(entity);

            if (match == null)
                throw new Exception("Invalid Match");

            if (vak == null)
                throw new Exception("Invalid Vak");

            string[] seatRowCol = seat.Split('-');
            if (seatRowCol.Length != 2 || !int.TryParse(seatRowCol[0], out int seatRow) || !int.TryParse(seatRowCol[1], out int seatCol))
                throw new Exception("Invalid seat");

            if (seatRow < 1 || seatRow > vak.Rows || seatCol < 1 || seatCol > vak.SeatsPerRow)
                throw new Exception("Invalid seat");

           

            //Controle max 4 tickets per user per match 

            //dit haalt alles op wat recent is toegevoegd voor de savechanges
            var localTickets = _context.Tickets.Local
                .Where(t => t.Order.UserId == userId && t.MatchId == entity.MatchId && t.Status != CanceledStatus)
                .ToList();

            int totaalAantal = userTickets.Count(t => t.MatchId == entity.MatchId && t.Status != CanceledStatus) + localTickets.Count - 1; //-1 omdat de ticket die we net hebben toegevoegd ook in local zit, maar nog niet in de database, dus die moeten we eraf halen
            if (totaalAantal >= maxTicketsPerUserPerMatch)
            {
                throw new TicketLimitExceededException(maxTicketsPerUserPerMatch);
            }

            //Controle max 1 match per dag per user
            if (userTickets.Count(t => t.Status != CanceledStatus && t.MatchId != entity.MatchId && t.Match.MatchDate.Date == match.MatchDate.Date) >= maxMatchesPerDayPerUser)
            {
                throw new MaxTicketsPerMatchDayExeededException(maxMatchesPerDayPerUser);
            }

            //controle of de ticket verkoop al gestart is
            //if(DateTime.Now < entity.Match.MatchDate.AddMonths(-1) || DateTime.Now > entity.Match.MatchDate)
            //    {
            //        throw new TicketSalesNotActiveException(entity.Match.MatchDate.AddMonths(-1),entity.Match.MatchDate);
            //}

        }

        public Task AddAsync(Ticket entity)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(Ticket entity)
        {
           await _ticketDAO.DeleteAsync(entity);
        }

        public async Task CancelTicketAsync(int ticketId, string userId)
        {
            var ticket = await _ticketDAO.FindByIdAsync(ticketId);

            if (ticket == null || ticket.Order.UserId != userId)
            {
                throw new UnauthorizedAccessException("Ticket not found for this user.");
            }

            if (ticket.Status == CanceledStatus)
            {
                return;
            }

            if (DateTime.Now > ticket.Match.MatchDate.AddDays(-7))
            {
                throw new InvalidOperationException("Tickets can only be canceled until one week before the match.");
            }

            await _ticketDAO.UpdateStatusAsync(ticket.Id, CanceledStatus);
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
