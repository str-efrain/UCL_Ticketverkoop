using AutoMapper;
using BeerschopNET9_Identity.Extensions;
using Domains.Entities;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System.Globalization;
using System.Threading.Tasks;
using UCL_Ticketverkoop.ViewModels;

namespace UCL_Ticketverkoop.Controllers
{
    public class SeatSelectController : Controller
    {
        private const string CartSession = "shoppingCart";
        private readonly IService<Match> _matchService;
        private readonly IService<Vak> _vakService;
        private readonly IMapper _mapper;

        public SeatSelectController(IService<Match> matchService, IMapper mapper, IService<Vak> vakService)
        {
            _matchService = matchService;
            _mapper = mapper;
            _vakService = vakService;
        }
        public async Task<IActionResult> Index(int matchId)
        {
            Match match = await _matchService.FindByIdAsync(matchId);
            if (match == null) 
                return NotFound();
           
            MatchVM viewModel = _mapper.Map<MatchVM>(match);  

            return View(viewModel);
        }

        public async Task<IActionResult> SelectSeat(int? MatchId, int? VakId, int? Seat)
        {
            if (!MatchId.HasValue || !VakId.HasValue || !Seat.HasValue)
                return NotFound();

            Match match = await _matchService.FindByIdAsync(MatchId.Value);
            Vak vak = await _vakService.FindByIdAsync(VakId.Value);
            if (match == null || vak == null)
                return NotFound();

            if (Seat < 0 ||int.Parse(Seat.ToString()[0].ToString()) > vak.Rows || int.Parse(Seat.ToString()[1].ToString()) > vak.SeatsPerRow) //controleert of het zitje wel kan bestaan
                return NotFound();

            List<ShoppingCartVM> cart = HttpContext.Session.GetObject<List<ShoppingCartVM>>(CartSession) ?? new List<ShoppingCartVM>(); ;
            cart.Add(new ShoppingCartVM
            {
                Match = _mapper.Map<MatchVM>(match),
                Vak = _mapper.Map<VakVM>(vak),
                Seat = Seat.Value
            });

            HttpContext.Session.SetObject(CartSession, cart);
            return RedirectToAction("Index", "ShoppingCart");

        }





    }
}
