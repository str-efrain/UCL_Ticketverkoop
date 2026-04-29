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
        public async Task<ActionResult<List<MatchVM>>> Get([FromQuery] int? homeClubId, [FromQuery] int? awayClubId)
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

                List<MatchVM> data = _mapper.Map<List<MatchVM>>(matches);

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
    }
}
