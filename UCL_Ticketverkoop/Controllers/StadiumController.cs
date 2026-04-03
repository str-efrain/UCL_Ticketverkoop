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
        private readonly IService<Stadium> _stadiumService;
        private readonly IMapper _mapper;

        public StadiumController(IService<Stadium> stadiumService, IMapper mapper) 
        {
            _stadiumService = stadiumService;
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
                var model = await _stadiumService.FindByIdAsync(id);
                StadiumVM viewModel = _mapper.Map<StadiumVM>(model);
                return View(viewModel);
            }
            catch (Exception ex) 
            {
                return NotFound();
            }
        }
    }
}


