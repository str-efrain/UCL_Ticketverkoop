using Domains.Entities;
using Domains.Exceptions;
using Repositories.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class AbonnementService:IAbonnementService
    {
        private readonly IAbonnementDAO _abonnementDAO;
        private readonly ISeatService _seatService;
        private readonly ISeizoenService _seizoenService;
        public AbonnementService(IAbonnementDAO abonnementDAO, ISeatService seatService, ISeizoenService seizoenService)
        {
            _abonnementDAO = abonnementDAO;
            _seatService = seatService;
            _seizoenService = seizoenService;
        }

        public async Task AddAsync(Abonnement entity)
        {
            entity.Seizoen = await _seizoenService.FindByIdAsync(entity.SeizoenId);

            DateTime startDateAsDateTime = entity.Seizoen.StartDate.ToDateTime(TimeOnly.MinValue);
            DateOnly startAbonnementen = DateOnly.FromDateTime(startDateAsDateTime.AddMonths(-2));
            DateOnly stopAbonnementen = DateOnly.FromDateTime(startDateAsDateTime.AddMonths(-1));

            DateOnly vandaag = DateOnly.FromDateTime(DateTime.Now);

            //controle of de abonnementenverkoop al gestart is en nog niet gestopt is
            //if (vandaag < startAbonnementen || vandaag >= stopAbonnementen)
            //{
            //    DateTime start= startAbonnementen.ToDateTime(TimeOnly.MinValue);
            //    DateTime end = stopAbonnementen.ToDateTime(TimeOnly.MinValue);

            //    throw new SubscriptionSalesNotActiveException(start, end);
            //}

            //controle of seat al is genomen
            if (await _seatService.isSeatTakenAsync(entity.ClubId, entity.VakId, entity.Seat, entity.SeizoenId,null))
            {
                throw new SeatIsTakenException(entity.Seat, entity.Vak.Name, entity.Club.Stadium.Name);
            }


            await _abonnementDAO.AddAsync(entity);
        }

        public async Task DeleteAsync(Abonnement entity)
        {
            await _abonnementDAO.DeleteAsync(entity);
        }

        public async Task<Abonnement?> FindByIdAsync(int Id)
        {
            return await _abonnementDAO.FindByIdAsync(Id);
        }

        public async Task<IEnumerable<Abonnement>?> GetAllAsync()
        {
            return await _abonnementDAO.GetAllAsync();
        }

        public async Task<bool> isSeatTakenAsync(int clubId, int vakId, string seat, int seasonId)
        {
            return await _abonnementDAO.isSeatTakenAsync(clubId,vakId,seat,seasonId);
        }

        public async Task UpdateAsync(Abonnement entity)
        {
            await _abonnementDAO.UpdateAsync(entity);
        }
    }
}
