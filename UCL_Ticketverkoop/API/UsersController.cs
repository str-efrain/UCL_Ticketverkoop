using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UCL_Ticketverkoop.ViewModels;

namespace UCL_Ticketverkoop.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IMapper _mapper;

        public UsersController(UserManager<IdentityUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        // GET: api/users
        [HttpGet] //Authorize erbij als we JWT gaan gebruiken!
        public async Task<ActionResult<List<UserVM>>> Get()
        {
            try
            {
                // ophalen Users
                var users = await _userManager.Users.ToListAsync();

                List<UserVM> data = _mapper.Map<List<UserVM>>(users);

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

        // GET: api/users/5
        [HttpGet("{id}", Name = "GetUserById")]
        public async Task<ActionResult<IdentityUser>> Get(string id)
        {
            try
            {
                // ophalen User o.b.v. id
                var user = await _userManager.FindByIdAsync(id);

                //Mapping
                UserVM data = _mapper.Map<UserVM>(user);

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
                    error = "Fout bij het ophalen van de gebruikersgegevens." + ex.Message
                });
            }
        }
    }
}
