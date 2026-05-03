using BeerschopNET9_Identity.Extensions;
using Domains.Entities;
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
        private readonly IService<Order> _orderService;
        private readonly ITicketService _ticketService;

        public ShoppingCartController(IService<Order> orderService, ITicketService ticketService)
        {
            _orderService = orderService;
            _ticketService = ticketService;
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

        public async Task<IActionResult> Buy()
        {
            ShoppingCartVM cart = GetShoppingCart();

            if(cart == null || !cart.Tickets.Any())
            {
                ViewBag.ErrorMessage = "Je mand is leeg!";
                return View("Index",cart);
            }

            Order order = new Order
            {
               
                UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value, 
                OrderDate = DateTime.Now,
                TotalPrice = cart.TotalPrice
            };

            await _orderService.AddAsync(order);

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

            return RedirectToAction("Index", "Checkout", new { orderId = order.Id });

        }
        public IActionResult Delete(int? id) 
        {
            ShoppingCartVM cart = GetShoppingCart();

            if (!id.HasValue)
                return NotFound();            

            cart.Tickets.RemoveAll(t => t.Id == id.Value);
            cart.TotalPrice = cart.Tickets.Sum(t => t.Vak.Price);

            SetShoppingCart(cart);

            return View("Index", cart);
        }
    }
}
