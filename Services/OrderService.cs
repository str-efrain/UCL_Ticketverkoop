using Domains.Data;
using Domains.Entities;
using Repositories;
using Repositories.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.Mail.Interfaces;
using Util.PDF.Interfaces;

namespace Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderDAO _orderDAO;
        private readonly IEmailSend _emailSend;
        private readonly ICreatePDF _createPdf;
        private readonly UCL_TicketingDbContext _context;
        private readonly ITicketService _ticketService;
        private readonly IAbonnementService _abonnementService;

        public OrderService(IOrderDAO orderDAO, IEmailSend emailSend, ICreatePDF createPDF,UCL_TicketingDbContext context,ITicketService ticketService,IAbonnementService abonnementService)
        {
            _orderDAO = orderDAO;
            _emailSend = emailSend;
            _createPdf = createPDF;
            _context = context;
            _ticketService = ticketService;
            _abonnementService = abonnementService;
        }

        public async Task<int> ProcessOrderAsync(List<Ticket> tickets, List<Abonnement> subscriptions, string userId,double totalPrice)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
          
                var order = new Order
                {
                    UserId = userId,
                    OrderDate = DateTime.Now,
                    TotalPrice = totalPrice
                };

                await _orderDAO.AddAsync(order);

                await _context.SaveChangesAsync();

                foreach (var ticket in tickets)
                {
                    ticket.OrderId = order.Id;
                    await _ticketService.AddAsync(ticket, userId);
                }

                foreach (var sub in subscriptions)
                {
                    sub.OrderId = order.Id;
                    await _abonnementService.AddAsync(sub);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return order.Id;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw; 
            }
        }

        public async Task AddAsync(Order entity)
        {
            await _orderDAO.AddAsync(entity);
        }

        public async Task DeleteAsync(Order entity)
        {
            await _orderDAO.DeleteAsync(entity);
        }

        public async Task<Order?> FindByIdAsync(int Id)
        {
            return await _orderDAO.FindByIdAsync(Id);
        }

        public async Task<IEnumerable<Order>?> GetAllAsync()
        {
            return await _orderDAO.GetAllAsync();
        }

        public async Task UpdateAsync(Order entity)
        {
            await _orderDAO.UpdateAsync(entity);
        }

        public async Task<List<Order>> GetOrdersByUserIdAsync(string userId)
        {  
            return await _orderDAO.GetOrdersByUserIdAsync(userId);   
        }

        public async Task SendOrderConfirmationAsync(Order order, List<Ticket> tickets, List<Abonnement> subscriptions)
        {
            var orderWithDetails = await _orderDAO.FindByIdAsync(order.Id) ?? order;

            // Tickets PDF
            var ticketPdf = tickets.Any() ? _createPdf.CreateTicketPDFDocument(tickets) : null;

            // Season pass PDF (send separately or together)
            var abonnementPdf = subscriptions.Any() ? _createPdf.CreateAbonnementPDFDocument(subscriptions) : null;

            string userEmail = orderWithDetails.User?.Email ?? order.UserId;

            await _emailSend.SendOrderConfirmationEmailAsync(
                userEmail,
                "Orderbevesting",
                orderWithDetails,
                ticketPdf,
                abonnementPdf);
            
        }
    }
}
