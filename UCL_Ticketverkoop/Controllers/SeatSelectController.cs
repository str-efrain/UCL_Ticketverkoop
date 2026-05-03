using AutoMapper;
using BeerschopNET9_Identity.Extensions;
using Domains.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System.Globalization;
using System.Security.Claims;
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

        public async Task<IActionResult> SeatSelection(int matchId, int vakId)
        {
            
            Match match = await _matchService.FindByIdAsync(matchId);
            if (match == null) 
                return NotFound();
           
            if (!match.HomeClub.Stadium.Vaks.Any(v => v.Id == vakId))
                return NotFound();

            //if(DateTime.Now < match.MatchDate.AddMonths(-1) || DateTime.Now > match.MatchDate)
            //{ 
            //    ViewBag.ErrorMessage = "Sorry, de kaartverkoop voor deze wedstrijd is nog niet gestart of al afgelopen!";
            //    MatchVM VM = _mapper.Map<MatchVM>(match);
            //    return View("Index", VM);
            //}

            

            MatchVM viewModel = _mapper.Map<MatchVM>(match);  
            SeatSelectVM seatSelectVM = new SeatSelectVM
            {
                match = viewModel,
                VakId = vakId
            };

            return View(seatSelectVM);
        }

        [Authorize]
        public async Task<IActionResult> SelectSeat(int MatchId, int VakId, string Seat)
        {

            //Match en vak ophalen
            Match match = await _matchService.FindByIdAsync(MatchId);
            Vak vak = await _vakService.FindByIdAsync(VakId);

            //Check of match en vak bestaan
            if (match == null || vak == null)
            {
                return NotFound();
            }
            //Seat string splitsen in rij en stoelnummer
            string[] seatRowCol = Seat.Split('-');
            int seatRow = int.Parse(seatRowCol[0]);
            int seatCol = int.Parse(seatRowCol[1]);

            //Controleert of het zitje wel kan bestaan
            if (seatRow < 1 || seatRow >= vak.Rows || seatCol < 1 || seatCol >= vak.SeatsPerRow)
            {
                return NotFound();
            }



            //shoppingCart ophalen
            ShoppingCartVM cart = new ShoppingCartVM();
            cart = HttpContext.Session.GetObject<ShoppingCartVM>(CartSession) ?? new ShoppingCartVM();

            //check of er al 4 tickets van deze match in de winkelmand of bestellingen zitten
            var CurrentUserTickets = await _ticketService.GetTicketsByUserAsync(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            int TicketsForMatchInCart = cart.Tickets.Where(t => t.Match.Id == MatchId).Count();
            int TicketsForMatchInDB = CurrentUserTickets.Where(t => t.MatchId == MatchId).Count();
            if (TicketsForMatchInCart + TicketsForMatchInDB >= 4)
            {
                ViewBag.ErrorMessage = "Je kan maar 4 tickets per match kopen!";
                MatchVM matchVM = _mapper.Map<MatchVM>(match);
                SeatSelectVM viewModel = new SeatSelectVM
                {
                    match = matchVM,
                    VakId = VakId
                };
                return View("SeatSelection", viewModel);
            }

            //Check of het zitje al bezet is
            bool isTaken = await _ticketService.IsSeatTakenAsync(MatchId, VakId, Seat);
            bool isInShoppingCart = cart.Tickets.Any(t => t.Match.Id == MatchId && t.Vak.Id == VakId && t.Seat == Seat);
            if (isTaken)
            {
                ViewBag.ErrorMessage = "Sorry,Deze stoel is al bezet!";
                MatchVM matchVM = _mapper.Map<MatchVM>(match);
                SeatSelectVM viewModel = new SeatSelectVM
                {
                    match = matchVM,
                    VakId = VakId
                };
                return View("SeatSelection", viewModel);
            }

            if(isInShoppingCart)
            {
                ViewBag.ErrorMessage = "Je hebt deze stoel al in je winkelmand!";
                MatchVM matchVM = _mapper.Map<MatchVM>(match);
                SeatSelectVM viewModel = new SeatSelectVM
                {
                    match = matchVM,
                    VakId = VakId
                };
                return View("SeatSelection", viewModel);
            }

            TicketVM ticket = new TicketVM
            { 
                Id = cart.Tickets.Count, //Mag NOOIT naar db enkel voor identificatie in de shopping cart
                Match = _mapper.Map<MatchVM>(match),
                Vak = _mapper.Map<VakVM>(vak),
                Seat = Seat
            };

            cart.Tickets.Add(ticket);
            cart.TotalPrice += vak.Price;

            HttpContext.Session.SetObject(CartSession, cart);
            return RedirectToAction("Index", "ShoppingCart");

        }
    }
}
