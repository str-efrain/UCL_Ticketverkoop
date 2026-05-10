using AutoMapper;
using BeerschopNET9_Identity.Extensions;
using Domains.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NuGet.Common;
using Services.Interfaces;
using System;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.ConstrainedExecution;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using UCL_Ticketverkoop.ViewModels;

namespace UCL_Ticketverkoop.Controllers
{
    [Authorize]
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

            if (DateTime.Now < match.MatchDate.AddMonths(-1) || DateTime.Now > match.MatchDate)
            {
                ViewBag.ErrorMessage = "Sorry, de kaartverkoop voor deze wedstrijd is nog niet gestart of al afgelopen!";
                MatchVM VM = _mapper.Map<MatchVM>(match);
                return View("Index", VM);
            }



            SeatSelectVM seatSelectVM = await BuildSeatSelectVm(match, vakId);
            return View(seatSelectVM);
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddSeats(int MatchId, int VakId, List<string> selectedSeats)
        {
            var currentUserTickets = await _ticketService.GetTicketsByUserAsync(User.FindFirst(ClaimTypes.NameIdentifier)?.Value) ?? new List<Ticket>();

            //Match en vak ophalen
            Match match = await _matchService.FindByIdAsync(MatchId);
            Vak vak = await _vakService.FindByIdAsync(VakId);

            if (match == null || vak == null)
            {
                return NotFound();
            }

            var seatsToAdd = selectedSeats?.Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList() ?? new List<string>();

            if (seatsToAdd.Count == 0)
            {
                ViewBag.ErrorMessage = "Selecteer minstens één stoel.";
                SeatSelectVM emptySelection = await BuildSeatSelectVm(match, VakId);
                return View("SeatSelection", emptySelection);
            }

            ShoppingCartVM cart = HttpContext.Session.GetObject<ShoppingCartVM>(CartSession) ?? new ShoppingCartVM();

            int ticketsForMatchInCart = cart.Tickets.Count(t => t.Match.Id == MatchId);
            int ticketsForMatchInDb = currentUserTickets.Count(t => t.MatchId == MatchId);
            if (ticketsForMatchInCart + ticketsForMatchInDb + seatsToAdd.Count > 4)
            {
                ViewBag.ErrorMessage = "Je kan maar 4 tickets per match kopen!";
                SeatSelectVM limitViewModel = await BuildSeatSelectVm(match, VakId);
                return View("SeatSelection", limitViewModel);
            }

            bool otherMatchSameDayDb = currentUserTickets.Any(t => t.Match.MatchDate.Date == match.MatchDate.Date && t.Match.Id != MatchId);
            bool otherMatchSameDayCart = cart.Tickets.Any(t => t.Match.MatchDate.Date == match.MatchDate.Date && t.Match.Id != MatchId);
            if (otherMatchSameDayDb || otherMatchSameDayCart)
            {
                ViewBag.ErrorMessage = "Je hebt al ticket(s) voor een wedstrijd op " + match.MatchDate.ToShortDateString();
                SeatSelectVM sameDayViewModel = await BuildSeatSelectVm(match, VakId);
                return View("SeatSelection", sameDayViewModel);
            }

            int nextId = cart.Tickets.Count;
            foreach (string seat in seatsToAdd)
            {
                string[] seatRowCol = seat.Split('-');
                if (seatRowCol.Length != 2 || !int.TryParse(seatRowCol[0], out int seatRow) || !int.TryParse(seatRowCol[1], out int seatCol))
                {
                    ViewBag.ErrorMessage = "Ongeldige stoel geselecteerd.";
                    SeatSelectVM invalidSeatViewModel = await BuildSeatSelectVm(match, VakId);
                    return View("SeatSelection", invalidSeatViewModel);
                }

                if (seatRow < 1 || seatRow > vak.Rows || seatCol < 1 || seatCol > vak.SeatsPerRow)
                {
                    ViewBag.ErrorMessage = "Ongeldige stoel geselecteerd.";
                    SeatSelectVM invalidSeatViewModel = await BuildSeatSelectVm(match, VakId);
                    return View("SeatSelection", invalidSeatViewModel);
                }

               TicketVM ticket = new TicketVM
                {
                    Id = nextId++,
                    Match = _mapper.Map<MatchVM>(match),
                    Vak = _mapper.Map<VakVM>(vak),
                    Ring = vak.Ring,
                    Seat = seat
                };

                cart.Tickets.Add(ticket);
                cart.TotalPrice += vak.Price;
            }

            HttpContext.Session.SetObject(CartSession, cart);
            return RedirectToAction("Index", "ShoppingCart");
        }
        
        //Omdat we overal SeatSelectVM's maken heb ik hier een mehod voor gemaakt
        private async Task<SeatSelectVM> BuildSeatSelectVm(Match match, int vakId)
        {
            MatchVM viewModel = _mapper.Map<MatchVM>(match);
            var takenSeats = await _ticketService.GetTakenSeatsAsync(match.Id, vakId);
            return new SeatSelectVM
            {
                match = viewModel,
                VakId = vakId,
                TakenSeats = new HashSet<string>(takenSeats ?? Enumerable.Empty<string>(), StringComparer.OrdinalIgnoreCase)
            };
        }
    }
}
