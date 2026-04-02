using Microsoft.AspNetCore.Mvc;

namespace UCL_Ticketverkoop.Controllers
{
    public class StadiumController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
