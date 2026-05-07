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
        private readonly ITicketService _ticketService;
        private readonly IMapper _mapper;

        public HistoryController(IOrderService orderService, ITicketService ticketService, IMapper mapper)
        {
            _orderService = orderService;
            _ticketService = ticketService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            List<OrderVM> orders = new List<OrderVM>();

            var data = await _orderService.GetOrdersByUserIdAsync(userId);

            foreach (var order in data)
            {
                var orderViewModel = _mapper.Map<OrderVM>(order);
                SetCancellationState(orderViewModel);
                orders.Add(orderViewModel);
            }

            return View(orders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelTicket(int ticketId)
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Challenge();
            }

            try
            {
                await _ticketService.CancelTicketAsync(ticketId, userId);
                TempData["SuccessMessage"] = "Ticket canceled successfully.";
            }
            catch (UnauthorizedAccessException)
            {
                TempData["ErrorMessage"] = "This ticket could not be found for your account.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private static void SetCancellationState(OrderVM order)
        {
            foreach (var ticket in order.Tickets)
            {
                ticket.CancelDeadline = ticket.Match.MatchDate.AddDays(-7);
                ticket.CanCancel = !string.Equals(ticket.Status, "Canceled", StringComparison.OrdinalIgnoreCase) &&
                                   DateTime.Now <= ticket.CancelDeadline;
            }
        }
    }
}
