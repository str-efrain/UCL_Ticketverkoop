using Domains.Entities;
using Repositories.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class SeatService:ISeatService
    {
        private readonly ITicketDAO _ticketDAO;
        private readonly IAbonnementDAO  _AbonnementDAO;

        public SeatService(ITicketDAO ticketDAO, IAbonnementDAO abonnementDAO)
        {
            _ticketDAO = ticketDAO;
            _AbonnementDAO = abonnementDAO;
        }

        //abbonement kan de verkoop van een ticket blokkeren, maar een ticket niet de verkoop van een abonnement
        public async Task<bool> isSeatTakenAsync(int clubId, int vakId, string seat, int seasonId, int? matchId = null)
        {
            if (matchId.HasValue)
            {
                var hasTicket = await _ticketDAO.IsSeatTakenAsync(matchId.Value, vakId, seat);

                if (hasTicket)
                    return true;
            }

            var hasSubscription = await _AbonnementDAO.isSeatTakenAsync(clubId, vakId, seat, seasonId);

            return hasSubscription;
        }
    }
}
