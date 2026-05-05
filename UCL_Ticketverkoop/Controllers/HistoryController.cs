using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System.Security.Claims;
using UCL_Ticketverkoop.ViewModels;

namespace UCL_Ticketverkoop.Controllers
{
    [Authorize]
    public class HistoryController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public HistoryController(IOrderService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            List<OrderVM> orders = new List<OrderVM>();

            var data = await _orderService.GetOrdersByUserIdAsync(userId);

            foreach (var order in data)
            {
                orders.Add(_mapper.Map<OrderVM>(order));
            }

            return View(orders);
        }
    }
}
