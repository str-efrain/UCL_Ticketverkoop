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
using System.Net.Sockets;
using System.Runtime.ConstrainedExecution;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using UCL_Ticketverkoop.ViewModels;

namespace UCL_Ticketverkoop.Controllers
{
    // TODO buisness rules:
    //• Per ticket wordt een voucher gegenereerd en via e-mail verstuurd.
    //• Abonnementen kunnen enkel aangekocht worden vóór de start van de competitie.
    //Een abonnement-plaats kan niet meer als los ticket verkocht worden.
    //• Overboeking is niet toegestaan; capaciteitslimieten mogen nooit overschreden
    //worden.
    //• Gratis annulatie is mogelijk tot 1 week vóór de wedstrijd.
    //• Gebruikers registreren zich éénmalig en kunnen nadien inloggen.
    //• Wachtwoord vergeten verloopt via e-mail.
    //• Supporters kunnen een hotelovernachting boeken via externe API’s.Deze moeten
    //niet bijgehouden worden
    //Gebruik bestaande externe API’s (bv.hotel-search/booking API via een publieke
    //provider of API-platform).
    //Je hoeft geen echte betaling bij hotel te doen
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
            //Tickets huidige gebruiker ophalen
            var CurrentUserTickets = await _ticketService.GetTicketsByUserAsync(User.FindFirst(ClaimTypes.NameIdentifier)?.Value) ?? new List<Ticket>(); ;

            //shoppingCart ophalen
            ShoppingCartVM cart = new ShoppingCartVM();
            cart = HttpContext.Session.GetObject<ShoppingCartVM>(CartSession) ?? new ShoppingCartVM();

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

            //check of er al 4 tickets van deze match in de winkelmand of bestellingen zitten
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

            //controle of de deze persoon al tickets heeft voor een andere match op dezelfde dag
            bool OtherMatchSameDayDB = CurrentUserTickets.Where(t => t.Match.MatchDate.Date == match.MatchDate.Date && t.Match.Id != MatchId).Any();
            bool OtherMatchSameDayCart = cart.Tickets.Where(t => t.Match.MatchDate.Date == match.MatchDate.Date && t.Match.Id != MatchId).Any();
            if (OtherMatchSameDayDB||OtherMatchSameDayCart) 
            {
                ViewBag.ErrorMessage = "Je hebt al ticket(s) voor een wedstrijd op" +match.MatchDate.ToShortDateString();
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

            if(DateTime.Now < match.MatchDate.AddMonths(-1) || DateTime.Now > match.MatchDate)
            { 
                ViewBag.ErrorMessage = "Sorry, de kaartverkoop voor deze wedstrijd is nog niet gestart of al afgelopen!";
                MatchVM VM = _mapper.Map<MatchVM>(match);
                return View("SeatSelection", VM);
            }

            if (isInShoppingCart)
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

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddSeats(int MatchId, int VakId, List<string> selectedSeats)
        {
            var currentUserTickets = await _ticketService.GetTicketsByUserAsync(User.FindFirst(ClaimTypes.NameIdentifier)?.Value) ?? new List<Ticket>();

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
                SeatSelectVM emptySelection = new SeatSelectVM
                {
                    match = _mapper.Map<MatchVM>(match),
                    VakId = VakId
                };
                return View("SeatSelection", emptySelection);
            }

            ShoppingCartVM cart = HttpContext.Session.GetObject<ShoppingCartVM>(CartSession) ?? new ShoppingCartVM();

            int ticketsForMatchInCart = cart.Tickets.Count(t => t.Match.Id == MatchId);
            int ticketsForMatchInDb = currentUserTickets.Count(t => t.MatchId == MatchId);
            if (ticketsForMatchInCart + ticketsForMatchInDb + seatsToAdd.Count > 4)
            {
                ViewBag.ErrorMessage = "Je kan maar 4 tickets per match kopen!";
                SeatSelectVM limitViewModel = new SeatSelectVM
                {
                    match = _mapper.Map<MatchVM>(match),
                    VakId = VakId
                };
                return View("SeatSelection", limitViewModel);
            }

            bool otherMatchSameDayDb = currentUserTickets.Any(t => t.Match.MatchDate.Date == match.MatchDate.Date && t.Match.Id != MatchId);
            bool otherMatchSameDayCart = cart.Tickets.Any(t => t.Match.MatchDate.Date == match.MatchDate.Date && t.Match.Id != MatchId);
            if (otherMatchSameDayDb || otherMatchSameDayCart)
            {
                ViewBag.ErrorMessage = "Je hebt al ticket(s) voor een wedstrijd op" + match.MatchDate.ToShortDateString();
                SeatSelectVM sameDayViewModel = new SeatSelectVM
                {
                    match = _mapper.Map<MatchVM>(match),
                    VakId = VakId
                };
                return View("SeatSelection", sameDayViewModel);
            }

            int nextId = cart.Tickets.Count;
            foreach (string seat in seatsToAdd)
            {
                string[] seatRowCol = seat.Split('-');
                if (seatRowCol.Length != 2 || !int.TryParse(seatRowCol[0], out int seatRow) || !int.TryParse(seatRowCol[1], out int seatCol))
                {
                    ViewBag.ErrorMessage = "Ongeldige stoel geselecteerd.";
                    SeatSelectVM invalidSeatViewModel = new SeatSelectVM
                    {
                        match = _mapper.Map<MatchVM>(match),
                        VakId = VakId
                    };
                    return View("SeatSelection", invalidSeatViewModel);
                }

                if (seatRow < 1 || seatRow >= vak.Rows || seatCol < 1 || seatCol >= vak.SeatsPerRow)
                {
                    ViewBag.ErrorMessage = "Ongeldige stoel geselecteerd.";
                    SeatSelectVM invalidSeatViewModel = new SeatSelectVM
                    {
                        match = _mapper.Map<MatchVM>(match),
                        VakId = VakId
                    };
                    return View("SeatSelection", invalidSeatViewModel);
                }

                bool isTaken = await _ticketService.IsSeatTakenAsync(MatchId, VakId, seat);
                bool isInShoppingCart = cart.Tickets.Any(t => t.Match.Id == MatchId && t.Vak.Id == VakId && t.Seat == seat);
                if (isTaken || isInShoppingCart)
                {
                    ViewBag.ErrorMessage = "Een of meer geselecteerde stoelen zijn niet beschikbaar.";
                    SeatSelectVM unavailableViewModel = new SeatSelectVM
                    {
                        match = _mapper.Map<MatchVM>(match),
                        VakId = VakId
                    };
                    return View("SeatSelection", unavailableViewModel);
                }

                TicketVM ticket = new TicketVM
                {
                    Id = nextId++,
                    Match = _mapper.Map<MatchVM>(match),
                    Vak = _mapper.Map<VakVM>(vak),
                    Seat = seat
                };

                cart.Tickets.Add(ticket);
                cart.TotalPrice += vak.Price;
            }

            HttpContext.Session.SetObject(CartSession, cart);
            return RedirectToAction("Index", "ShoppingCart");
        }
    }
}
