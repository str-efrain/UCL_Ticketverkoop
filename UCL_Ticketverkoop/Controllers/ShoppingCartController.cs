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
            return View();
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
    }
}
