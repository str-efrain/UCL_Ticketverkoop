using AutoMapper;
using Domains.Entities;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System.Threading.Tasks;
using UCL_Ticketverkoop.ViewModels;

namespace UCL_Ticketverkoop.Controllers
{
    public class StadiumController : Controller
    {
        private readonly IService<Club> _clubService;
        private readonly IMapper _mapper;

        public StadiumController(IService<Club> clubService, IMapper mapper) 
        {
            _clubService = clubService;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Details(int id)
        {
            try 
            {
                var model = await _clubService.FindByIdAsync(id);
                ClubVM viewModel = _mapper.Map<ClubVM>(model);
                return View(viewModel);
            }
            catch (Exception ex) 
            {
                return NotFound();
            }
        }
    }
}


