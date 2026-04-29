using AutoMapper;
using Domains.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using UCL_Ticketverkoop.ViewModels;

namespace UCL_Ticketverkoop.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClubsController : ControllerBase
    {
        private IService<Club> _clubService;
        private readonly IMapper _mapper;
        public ClubsController(IService<Club> clubService, IMapper mapper)
        {
            _clubService = clubService;
            _mapper = mapper;
        }

        // GET: api/Club
        [HttpGet] //Authorize erbij als we JWT gaan gebruiken!
        public async Task<ActionResult<ClubVM>> Get()
        {
            try
            {
                //ophalen Clubs
                var clubs = await _clubService.GetAllAsync();
                // mapping
                List<ClubVM> data = _mapper.Map<List<ClubVM>>(clubs);

                if (data == null)
                {// Als de gegevens niet worden gevonden, retourneer een 404 Not Found-status
                    return NotFound();
                }
                // Retourneer de gegevens als alles goed is verlopen
                // HTTP-statuscode 200
                return Ok(data);
            }
            catch (Exception ex)
            {
                // Als er een fout optreedt, retourneer een 500 Internal Server Error-status
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // GET: api/club/5
        [HttpGet("{id}", Name = "GetClubById")]
        public async Task<ActionResult<ClubVM>> Get(int id)
        {
            try
            {
                // ophalen Club o.b.v. id
                var club = await _clubService.FindByIdAsync(id);

                // mapping
                ClubVM data = _mapper.Map<ClubVM>(club);
                if (data == null)
                {
                    return NotFound();
                }
                // Retourneer data
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Fout bij het ophalen van de clubgegevens." + ex.Message
                });
            }
        }
    }
}
