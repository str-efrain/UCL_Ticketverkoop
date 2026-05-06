using BeerschopNET9_Identity.Extensions;
using Domains.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Versioning;
using Services.Interfaces;
using System.Security.Claims;
using UCL_Ticketverkoop.ViewModels;

namespace UCL_Ticketverkoop.Controllers
{
    public class ShoppingCartController : Controller
    {
        private const string CartSession = "shoppingCart";
        private readonly IOrderService _orderService;
        private readonly ITicketService _ticketService;
        private readonly ISeatService _seatService;
        private readonly IAbonnementService _abonnementService;

        public ShoppingCartController(IOrderService orderService, ITicketService ticketService, ISeatService seatService, IAbonnementService abonnementService)
        {
            _orderService = orderService;
            _ticketService = ticketService;
            _seatService = seatService;
            _abonnementService = abonnementService;

        }


        public IActionResult Index()
        {
            var cart = GetShoppingCart();

            return View(cart);
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

        [Authorize]
        public async Task<IActionResult> Buy()
        {
            ShoppingCartVM cart = GetShoppingCart();

            if (cart == null || (!cart.Tickets.Any() && !cart.Subscriptions.Any()))
            {
                ViewBag.ErrorMessage = "Je mand is leeg!";
                return View("Index", cart);
            }

            if (cart.Tickets != null && cart.Tickets.Any())
            {
                foreach (var item in cart.Tickets)
                {
                    bool isTaken = await _seatService.isSeatTakenAsync(item.Match.HomeTeamId, item.Vak.Id, item.Seat, item.Match.SeizoenId, item.Match.Id);
                    if (isTaken)
                    {
                        ViewBag.ErrorMessage = $"Sorry,De stoel {item.Seat} in vak {item.Vak.Name} voor de match {item.Match.AwayTeamName} is al bezet!";
                        return View("Index", cart);
                    }
                }
            }

            if (cart.Subscriptions != null && cart.Subscriptions.Any())
            {
                foreach (var item in cart.Subscriptions)
                {
                    bool isTaken = await _seatService.isSeatTakenAsync(item.Club.Id, item.Vak.Id, item.Seat, item.SeizoenId, null);
                    if (isTaken)
                    {
                        ViewBag.ErrorMessage = $"Sorry,De stoel {item.Seat} in vak {item.Vak.Name} voor de dit seizoen is al bezet!";
                        return View("Index", cart);
                    }
                }
            }



            Order order = new Order
            {

                UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                OrderDate = DateTime.Now,
                TotalPrice = cart.TotalPrice
            };

            await _orderService.AddAsync(order);

            if (cart.Tickets != null && cart.Tickets.Any())
            {
                foreach (var item in cart.Tickets)
                {
                    await _ticketService.AddAsync(new Ticket
                    {
                        OrderId = order.Id,
                        MatchId = item.Match.Id,
                        VakId = item.Vak.Id,
                        Seat = item.Seat,
                    });

                }
            }

            if (cart.Subscriptions != null && cart.Subscriptions.Any())
            {
                foreach (var item in cart.Subscriptions)
                {
                    await _abonnementService.AddAsync(new Abonnement
                    {
                        OrderId = order.Id,
                        ClubId = item.Club.Id,
                        VakId = item.Vak.Id,
                        Seat = item.Seat,
                        SeizoenId = item.SeizoenId
                    });
                }



            }

            //Mailing
            await _orderService.SendOrderConfirmationAsync(order);

            return RedirectToAction("Index", "Checkout", new { orderId = order.Id });

        }
        public IActionResult Delete(int? id, string type)
        {
            ShoppingCartVM cart = GetShoppingCart();
            if (type == "Ticket")
            {

                if (!id.HasValue)
                    return NotFound();

                cart.Tickets.RemoveAll(t => t.Id == id.Value);
                cart.TotalPrice = cart.Tickets.Sum(t => t.Vak.Price);

                SetShoppingCart(cart);

                return View("Index", cart);
            }
            else if (type == "Subscription")
            {
                if (!id.HasValue)
                    return NotFound();
                cart.Subscriptions.RemoveAll(s => s.Id == id.Value);
                cart.TotalPrice = cart.Subscriptions.Sum(s => s.Vak.Price);
                SetShoppingCart(cart);
                return View("Index", cart);

            }
            else
            {
                return NotFound();
            }
        }
    }
}
