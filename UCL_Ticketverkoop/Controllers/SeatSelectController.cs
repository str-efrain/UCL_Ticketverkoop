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
        private readonly IMatchService _matchService;
        private readonly IService<Vak> _vakService;
        private readonly ITicketService _ticketService;
        private readonly IMapper _mapper;

        public SeatSelectController(IMatchService matchService, IMapper mapper, IService<Vak> vakService,ITicketService ticketService)
        {
            _matchService = matchService;
            _mapper = mapper;
            _vakService = vakService;
            _ticketService = ticketService;
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
            if (!(await _ticketService.IsSeatTakenAsync(MatchId.Value, VakId.Value, Seat.Value)))
            {
                ShoppingCartVM cart = new ShoppingCartVM();
                cart = HttpContext.Session.GetObject<ShoppingCartVM>(CartSession) ?? new ShoppingCartVM();

                TicketVM ticket = new TicketVM
                {
                    Match = _mapper.Map<MatchVM>(match),
                    Vak = _mapper.Map<VakVM>(vak),
                    Seat = Seat.Value
                };


                cart.Tickets.Add(ticket);
                cart.TotalPrice += vak.Price;

                HttpContext.Session.SetObject(CartSession, cart);
                return RedirectToAction("Index", "ShoppingCart");
            }
            else
            {
                ViewBag.ErrorMessage = "Sorry,Deze stoel is al bezet!";
                MatchVM viewModel = _mapper.Map<MatchVM>(match);
                return View("Index", viewModel);
            }

            

        }





    }
}
