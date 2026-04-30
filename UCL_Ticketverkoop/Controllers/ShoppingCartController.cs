using BeerschopNET9_Identity.Extensions;
using Microsoft.AspNetCore.Mvc;
using UCL_Ticketverkoop.ViewModels;

namespace UCL_Ticketverkoop.Controllers
{
    public class ShoppingCartController : Controller
    {
        private const string CartSession = "shoppingCart";
        public IActionResult Index()
        {
            var cart = GetShoppingCart();

            return View(cart);
        }

        public List<ShoppingCartVM>? GetShoppingCart()
        {
            var shoppingCartVM = HttpContext.Session.GetObject<List<ShoppingCartVM>>(CartSession);
            return shoppingCartVM;
        }
        public void SetShoppingCart(ShoppingCartVM cart)
        {
            HttpContext.Session.SetObject(CartSession, cart);
        }
    }
}
