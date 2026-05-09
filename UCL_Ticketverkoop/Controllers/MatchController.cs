using AutoMapper;
using Domains.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services;
using Services.Interfaces;
using System.Threading.Tasks;
using UCL_Ticketverkoop.ViewModels;

namespace UCL_Ticketverkoop.Controllers
{
    public class MatchController : Controller
    {
        private readonly IMatchService _matchService;
        private readonly IMapper _mapper;
        private readonly IService<Club> _clubService;
        private readonly ISeizoenService _seasonService;

        public MatchController(IMatchService matchService, IMapper mapper, IService<Club> clubService, ISeizoenService seasonService)
        {
            _matchService = matchService;
            _mapper = mapper;
            _clubService = clubService;
            _seasonService = seasonService;
        }

        public async Task<IActionResult> Index(int? seasonId)
        {
            var seasons = (await _seasonService.GetAllAsync() ?? Enumerable.Empty<Seizoen>())
                .OrderByDescending(s => s.StartDate)
                .ToList();
            var selectedSeason = GetSelectedSeason(seasons, seasonId);

            ClubMatchVM viewModel = new ClubMatchVM
            {
                SeasonID = selectedSeason?.Id,
                SelectedSeasonName = selectedSeason?.Name,
                Matches = selectedSeason == null
                    ? new List<MatchVM>()
                    : _mapper.Map<IEnumerable<MatchVM>>(await _matchService.GetBySeasonAsync(selectedSeason.Id)),
                Clubs = new SelectList(await _clubService.GetAllAsync(), "Id", "Name"),
                Seasons = new SelectList(seasons, "Id", "Name", selectedSeason?.Id)
            };

            return View(viewModel);

        }

        [HttpPost]
        public async Task<IActionResult> getMatchesByClubs(ClubMatchVM model)
        {
            var seasons = (await _seasonService.GetAllAsync() ?? Enumerable.Empty<Seizoen>())
                .OrderByDescending(s => s.StartDate)
                .ToList();
            var selectedSeason = GetSelectedSeason(seasons, model.SeasonID);

            IEnumerable<Match> matches = Enumerable.Empty<Match>();
            if (selectedSeason != null && model.ClubID.HasValue)
            {
                matches = await _matchService.GetByClubAndSeasonAsync(model.ClubID.Value, selectedSeason.Id)
                    ?? Enumerable.Empty<Match>();
            }
            else if (selectedSeason != null)
            {
                matches = await _matchService.GetBySeasonAsync(selectedSeason.Id)
                    ?? Enumerable.Empty<Match>();
            }

            model.SeasonID = selectedSeason?.Id;
            model.SelectedSeasonName = selectedSeason?.Name;
            model.Matches = _mapper.Map<IEnumerable<MatchVM>>(matches);

            return PartialView("_MatchCalendarResults", model);
        }

        private static Seizoen? GetSelectedSeason(List<Seizoen> seasons, int? seasonId)
        {
            if (seasonId.HasValue)
            {
                var season = seasons.FirstOrDefault(s => s.Id == seasonId.Value);
                if (season != null)
                {
                    return season;
                }
            }

            return seasons.FirstOrDefault(s => s.IsCurrentSeason) ?? seasons.FirstOrDefault();
        }
    }
}
