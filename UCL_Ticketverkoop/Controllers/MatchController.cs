using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System.Threading.Tasks;
using UCL_Ticketverkoop.ViewModels;

namespace UCL_Ticketverkoop.Controllers
{
    public class MatchController : Controller
    {
        private readonly IMatchService _matchService;
        private readonly IMapper _mapper;

        public MatchController(IMatchService matchService, IMapper mapper)
        {
            _matchService = matchService;
            _mapper = mapper;   
        }

        public async Task<IActionResult> Index()
        {
            var matches = await _matchService.GetAllAsync();
            var matchVMs = _mapper.Map<IEnumerable<MatchVM>>(matches);
            return View(matchVMs);
        }
    }
}
