using BeerschopNET9_Identity.Extensions;
using Domains.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Versioning;
using Services.Interfaces;
using System.Security.Claims;
using UCL_Ticketverkoop.ViewModels;
using Domains.Exceptions;

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
        public void ClearShoppingCart()
        {
            HttpContext.Session.Remove(CartSession);
        }
        public void SetShoppingCart(ShoppingCartVM cart)
        {
            HttpContext.Session.SetObject(CartSession, cart);
        }

        [Authorize]
        public async Task<IActionResult> Buy()
        {
            try
            {
                ShoppingCartVM cart = GetShoppingCart();

                if (cart == null || (!cart.Tickets.Any() && !cart.Subscriptions.Any()))
                {
                    ViewBag.ErrorMessage = "Je mand is leeg!";
                    return View("Index", cart);
                }

                List<Ticket> tickets = new List<Ticket>();
                List<Abonnement> subscriptions = new List<Abonnement>();
                Order order = new Order();

                order = new Order
                {

                    UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                    OrderDate = DateTime.Now,
                    TotalPrice = cart.TotalPrice
                };


                if (cart.Tickets != null && cart.Tickets.Any())
                {
                    foreach (var item in cart.Tickets)
                    {
                        tickets.Add(new Ticket
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
                        subscriptions.Add(new Abonnement
                        {
                            OrderId = order.Id,
                            ClubId = item.Club.Id,
                            VakId = item.Vak.Id,
                            Seat = item.Seat,
                            SeizoenId = item.SeizoenId
                        });
                    }



                }

                int orderId = await _orderService.ProcessOrderAsync(tickets, subscriptions, order.UserId, cart.TotalPrice);

                //Mailing
                //await _orderService.SendOrderConfirmationAsync(order);

                //Cart leegmaken
                ClearShoppingCart();

                return RedirectToAction("Index", "Checkout", new { orderId = orderId });
            }
            catch (BusinessRuleException e)
            {
                ViewBag.ErrorMessage = e.Message;
                return View("Index");
            }
            catch(Exception)
            {
                ViewBag.ErrorMessage = "Er is een fout opgetreden bij het verwerken van je bestelling. Probeer het later opnieuw.";
                return View("Index");
            }
        }
        public IActionResult Delete(int? id, string type)
        {
            ShoppingCartVM cart = GetShoppingCart();
            if (type == "Ticket")
            {

                if (!id.HasValue)
                    return NotFound();

                cart.Tickets.RemoveAll(t => t.Id == id.Value);
                cart.TotalPrice = cart.Subscriptions.Sum(s => s.Vak.Price) + cart.Tickets.Sum(t => t.Vak.Price);

                SetShoppingCart(cart);

                return View("Index", cart);
            }
            else if (type == "Subscription")
            {
                if (!id.HasValue)
                    return NotFound();
                cart.Subscriptions.RemoveAll(s => s.Id == id.Value);
                cart.TotalPrice = cart.Subscriptions.Sum(s => s.Vak.Price) + cart.Tickets.Sum(t => t.Vak.Price);
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
