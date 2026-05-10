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
        private readonly IService<Vak> _vakService;
        private readonly IService<Club> _clubService;
        public AbonnementService(IAbonnementDAO abonnementDAO, ISeatService seatService, ISeizoenService seizoenService,IService<Vak> vakService,IService<Club> clubService)
        {
            _abonnementDAO = abonnementDAO;
            _seatService = seatService;
            _seizoenService = seizoenService;
            _vakService = vakService;
            _clubService = clubService;
        }

        public async Task AddAsync(Abonnement entity)
        {
            entity.Seizoen = await _seizoenService.FindByIdAsync(entity.SeizoenId);
            entity.Vak = await _vakService.FindByIdAsync(entity.VakId);
            entity.Club =  await _clubService.FindByIdAsync(entity.ClubId);


            if (entity.Seat == null || entity.VakId == 0 || entity.ClubId == 0 || entity.SeizoenId == 0)
            {
                throw new Exception("ongeldige gegevens");
            }

            if(entity.Seizoen == null)
            {
                throw new Exception("ongeldig seizoen");
            }

            string[] seatRowCol = entity.Seat.Split('-');
            if (seatRowCol.Length != 2 || !int.TryParse(seatRowCol[0], out int seatRow) || !int.TryParse(seatRowCol[1], out int seatCol))
                throw new Exception("ongeldige stoel");

            if (seatRow < 1 || seatRow > entity.Vak.Rows || seatCol < 1 || seatCol > entity.Vak.SeatsPerRow)
                throw new Exception("ongeldige stoel");

            //controle of seat al is genomen
            if (await _seatService.isSeatTakenAsync(entity.ClubId, entity.VakId, entity.Seat, entity.SeizoenId, null))
            {
                throw new SeatIsTakenException(entity.Seat, entity.Vak.Name, entity.Club.Stadium.Name);
            }

            await _abonnementDAO.AddAsync(entity);

            DateTime startDateAsDateTime = entity.Seizoen.StartDate.ToDateTime(TimeOnly.MinValue);
            DateOnly stopAbonnementen = DateOnly.FromDateTime(startDateAsDateTime.AddMonths(-1));

            DateOnly vandaag = DateOnly.FromDateTime(DateTime.Now);

            //controle of de abonnementenverkoop al gestart is en nog niet gestopt is
            if (vandaag >= stopAbonnementen)
            {
                DateTime end = stopAbonnementen.ToDateTime(TimeOnly.MinValue);

                throw new SubscriptionSalesNotActiveException(end);
            }
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

        public async Task<List<string>> GetTakenSeatsAsync(int clubId, int vakId, int seasonId)
        {
            return await _abonnementDAO.GetTakenSeatsAsync(clubId, vakId, seasonId);
        }

        public async Task UpdateAsync(Abonnement entity)
        {
            await _abonnementDAO.UpdateAsync(entity);
        }
    }
}
