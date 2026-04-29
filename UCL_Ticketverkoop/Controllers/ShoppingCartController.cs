using AutoMapper;
using Domains.Entities;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System.Threading.Tasks;
using UCL_Ticketverkoop.ViewModels;

namespace UCL_Ticketverkoop.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IService<Stadium> _stadiumService;
        private readonly IMapper _mapper;

        public ShoppingCartController(IService<Stadium> stadiumService, IMapper mapper)
        {
            _stadiumService = stadiumService;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index(int stadiumId, int matchId)
        {
            Stadium stadium = await _stadiumService.FindByIdAsync(stadiumId);
            if (stadium == null) 
                return NotFound();

            StadiumVM viewModel = _mapper.Map<StadiumVM>(stadium);  

            return View(viewModel);
        }
    }
}
