using AutoMapper;
using BeerschopNET9_Identity.Extensions;
using Domains.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System.Threading.Tasks;
using UCL_Ticketverkoop.ViewModels;

namespace UCL_Ticketverkoop.Controllers
{
    public class SubscriptionController : Controller
    {
        private const string CartSession = "shoppingCart";
        private readonly IMapper _mapper;
        private readonly IService<Club> _clubService;
        private readonly ISeizoenService _seizoenService;
        private readonly IService<Vak> _vakService;
        private readonly IAbonnementService _abonnementService;

        public SubscriptionController(IMapper mapper, IService<Club> clubService, ISeizoenService seizoenService, IService<Vak> vakService, IAbonnementService abonnementService)
        {
            _mapper = mapper;
            _clubService = clubService;
            _seizoenService = seizoenService;
            _vakService = vakService;
            _abonnementService = abonnementService;
        }
        public async Task<IActionResult> Index(int vakId, int clubId, int? seasonId)
        {
            var selectedSeason = seasonId.HasValue
                ? await _seizoenService.FindByIdAsync(seasonId.Value)
                : await _seizoenService.GetCurrentSeasonAsync();

            if (selectedSeason == null)
            {
                return NotFound();
            }

            var takenSeats = await _abonnementService.GetTakenSeatsAsync(clubId, vakId, selectedSeason.Id);

            SubSelectVM viewModel = new SubSelectVM
            {
                Vak = _mapper.Map<VakVM>( await _vakService.FindByIdAsync(vakId)),
                Club = _mapper.Map<ClubVM>(await _clubService.FindByIdAsync(clubId)),
                SeasonId = selectedSeason.Id,
                SeasonName = selectedSeason.Name,
                TakenSeats = new HashSet<string>(takenSeats ?? Enumerable.Empty<string>(), StringComparer.OrdinalIgnoreCase)
            };
            return View(viewModel);
        }
        public ShoppingCartVM? GetShoppingCart()
        {
            var shoppingCartVM = HttpContext.Session.GetObject<ShoppingCartVM>(CartSession);
            return shoppingCartVM;
        }
        public void SetShoppingCart(ShoppingCartVM cart)
        {
            HttpContext.Session.SetObject(CartSession, cart);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddSubscriptions(int ClubId, int VakId, int SeasonId, List<string> selectedSeats)
        {
            ShoppingCartVM shoppingCart = GetShoppingCart() ?? new ShoppingCartVM();
            var selectedSeason = await _seizoenService.FindByIdAsync(SeasonId);

            if (selectedSeason == null)
            {
                return NotFound();
            }

            if (shoppingCart.Subscriptions == null)
            {
                shoppingCart.Subscriptions = new List<SubscriptionVM>();
            }

            foreach (var seat in selectedSeats)
            {
                shoppingCart.Subscriptions.Add(new SubscriptionVM
                {
                    Id = shoppingCart.Subscriptions.Count + 1, //mag niet naar db om in shopping cart te kunnen onderscheiden
                    Club = _mapper.Map<ClubVM>(await _clubService.FindByIdAsync(ClubId)),
                    Vak = _mapper.Map<VakVM>(await _vakService.FindByIdAsync(VakId)),
                    Seat = seat,
                    SeizoenId = selectedSeason.Id,
                    SeizoenName = selectedSeason.Name
                });

                shoppingCart.TotalPrice += shoppingCart.Subscriptions.Last().Vak.SubscriptionPrice;
            }
     

            SetShoppingCart(shoppingCart);
            return RedirectToAction("Index", "ShoppingCart");
        }
    }
}
