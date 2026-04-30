using AutoMapper;
using Domains.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using UCL_Ticketverkoop.ViewModels;

namespace UCL_Ticketverkoop.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class StadiumsController : ControllerBase
    {
        private IService<Stadium> _stadiumService;
        private readonly IMapper _mapper;
        public StadiumsController(IService<Stadium> stadiumService, IMapper mapper)
        {
            _stadiumService = stadiumService;
            _mapper = mapper;
        }

        // GET: api/Stadium
        [HttpGet] //Authorize erbij als we JWT gaan gebruiken!
        public async Task<ActionResult<StadiumAPIVM>> Get()
        {
            try
            {
                //ophalen Stadiums
                var stadiums = await _stadiumService.GetAllAsync();
                // mapping
                List<StadiumAPIVM> data = _mapper.Map<List<StadiumAPIVM>>(stadiums);

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

        // GET: api/stadium/5
        [HttpGet("{id}", Name = "GetStadiumById")]
        public async Task<ActionResult<StadiumAPIVM>> Get(int id)
        {
            try
            {
                // ophalen Stadium o.b.v. id
                var stadium = await _stadiumService.FindByIdAsync(id);

                // mapping
                StadiumAPIVM data = _mapper.Map<StadiumAPIVM>(stadium);
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
                    error = "Fout bij het ophalen van de stadiongegevens. " + ex.Message
                });
            }
        }
    }
}
