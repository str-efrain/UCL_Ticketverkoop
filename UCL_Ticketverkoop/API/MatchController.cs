using AutoMapper;
using Domains.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Interfaces;
using UCL_Ticketverkoop.ViewModels;

namespace UCL_Ticketverkoop.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private readonly IMatchService _matchService;
        private readonly IMapper _mapper;

        public MatchController(IMatchService matchService, IMapper mapper)
        {
            _matchService = matchService;
            _mapper = mapper;
        }

        // GET: api/matches?homeClubId=...&awayClubId=...
        [HttpGet]
        public async Task<ActionResult<List<MatchAPIVM>>> Get([FromQuery] int? homeClubId, [FromQuery] int? awayClubId)
        {
            try
            {
                var matches = await _matchService.GetAllAsync();

                if (matches == null)
                {
                    return NotFound();
                }

                if (homeClubId.HasValue)
                {
                    matches = matches.Where(match => match.HomeClubId == homeClubId.Value);
                }

                if (awayClubId.HasValue)
                {
                    matches = matches.Where(match => match.AwayClubId == awayClubId.Value);
                }

                List<MatchAPIVM> data = _mapper.Map<List<MatchAPIVM>>(matches);

                if (data == null)
                {
                    return NotFound();
                }

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // GET: api/match/5
        [HttpGet("{id}", Name = "GetMatchById")]
        public async Task<ActionResult<MatchAPIVM>> Get(int id)
        {
            try
            {
                // ophalen Match o.b.v. id
                var match = await _matchService.FindByIdAsync(id);

                // mapping
                MatchAPIVM data = _mapper.Map<MatchAPIVM>(match);
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
                    error = "Fout bij het ophalen van de matchgegevens." + ex.Message
                });
            }
        }
    }
}
