using AutoMapper;
using BeerschopNET9_Identity.Extensions;
using Domains.Entities;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using UCL_Ticketverkoop.ViewModels;

namespace UCL_Ticketverkoop.Controllers
{
    public class ClubController : Controller
    {
        private readonly IService<Club> _clubService;
        private readonly IMapper _mapper;



        public ClubController(IService<Club> clubService, IMapper mapper, IService<Vak> vakService) 
        {
            _clubService = clubService;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index()
        {
            var clubs = await _clubService.GetAllAsync();
           
            if (clubs == null)
            {
                return View(new List<ClubVM>());
            }

            var viewModels = _mapper.Map<IEnumerable<ClubVM>>(clubs);

            return View(viewModels);
        }

       
    }
}
