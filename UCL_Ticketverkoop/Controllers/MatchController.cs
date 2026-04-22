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

        public MatchController(IMatchService matchService, IMapper mapper, IService<Club> clubService)
        {
            _matchService = matchService;
            _mapper = mapper;
            _clubService = clubService; 
        }

        public async Task<IActionResult> Index()
        {
            ClubMatchVM viewModel = new ClubMatchVM();

            viewModel.Matches = _mapper.Map<IEnumerable<MatchVM>>(await _matchService.GetAllAsync());
            viewModel.Clubs = new SelectList(await _clubService.GetAllAsync(), "Id", "Name");

            return View(viewModel);

        }

        [HttpPost]
        public async Task<IActionResult> getMatchesByClubs(ClubMatchVM model)
        {
            IEnumerable<Match> matches;
            if (model.ClubID.HasValue)
            {
                matches = await _matchService.GetByClubAsync(model.ClubID.Value);
            }
            else
            {
                matches = await _matchService.GetAllAsync();
            }

            model.Matches = _mapper.Map<IEnumerable<MatchVM>>(matches);

            return PartialView("_MatchData", model.Matches);
        }
    }
}
