using AutoMapper;
using Domains.Entities;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System.Threading.Tasks;
using UCL_Ticketverkoop.ViewModels;

namespace UCL_Ticketverkoop.Controllers
{
    public class SubscriptionController : Controller
    {
        private readonly IMapper _mapper;
        private IService<Club> _clubService;

        public SubscriptionController(IMapper mapper, IService<Club> clubService)
        {
            _mapper = mapper;
            _clubService = clubService;
        }
        public async Task<IActionResult> Index(int vakId, int clubId)
        {
            SubSelectVM viewModel = new SubSelectVM
            {
                VakId = vakId,
                Club = _mapper.Map<ClubVM>(await _clubService.FindByIdAsync(clubId))
            };
            return View(viewModel);
        }

        public async Task<IActionResult> AddSubscriptions(int ClubId, int VakId, List<string> selectedSeats)
        {
            return View();
        }
    }
}
