using AutoMapper;
using Domains.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.Interfaces;
using UCL_Ticketverkoop.ViewModels;

namespace UCL_Ticketverkoop.Controllers
{
    public class StadiumController : Controller
    {
        private readonly IService<Club> _clubService;
        private readonly ISeizoenService _seasonService;
        private readonly IMapper _mapper;

        public StadiumController(IService<Club> clubService, ISeizoenService seasonService, IMapper mapper)
        {
            _clubService = clubService;
            _seasonService = seasonService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Details(int id, int? seasonId)
        {
            try
            {
                var model = await _clubService.FindByIdAsync(id);
                var seasons = (await _seasonService.GetAllAsync() ?? Enumerable.Empty<Seizoen>())
                    .OrderByDescending(s => s.StartDate)
                    .ToList();
                var selectedSeason = GetSelectedSeason(seasons, seasonId);

                ViewBag.Seasons = new SelectList(seasons, "Id", "Name", selectedSeason?.Id);
                ViewBag.SelectedSeasonId = selectedSeason?.Id;
                ViewBag.SelectedSeasonName = selectedSeason?.Name;

                ClubVM viewModel = _mapper.Map<ClubVM>(model);
                return View(viewModel);
            }
            catch (Exception)
            {
                return NotFound();
            }
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
