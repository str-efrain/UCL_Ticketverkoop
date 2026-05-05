using Microsoft.AspNetCore.Mvc;

namespace UCL_Ticketverkoop.Controllers
{
    public class SubscriptionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
