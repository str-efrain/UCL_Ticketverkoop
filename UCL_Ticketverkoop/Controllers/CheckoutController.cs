using AutoMapper;
using Domains.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using UCL_Ticketverkoop.ViewModels;

namespace UCL_Ticketverkoop.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        IOrderService _orderService;
        IMapper _mapper;

        public CheckoutController(IOrderService orderService,IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index(int? orderId)
        {
            if(!orderId.HasValue)
                return NotFound();
            
            var order = await _orderService.FindByIdAsync(orderId.Value);

            if(order == null)
                return NotFound();

            OrderVM viewModel = _mapper.Map<OrderVM>(order);


            return View(viewModel);
        }
    }
}
