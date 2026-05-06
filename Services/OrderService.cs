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

        public OrderService(IOrderDAO orderDAO, IEmailSend emailSend, ICreatePDF createPDF)
        {
            _orderDAO = orderDAO;
            _emailSend = emailSend;
            _createPdf = createPDF;
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

        public async Task SendOrderConfirmationAsync(Order order)
        {
            var orderWithDetails = await _orderDAO.FindByIdAsync(order.Id) ?? order;
            var pdfStream = _createPdf.CreateTicketPDFDocument(orderWithDetails.Tickets.ToList());
            if (!string.IsNullOrWhiteSpace(orderWithDetails.User.Email))
            {
                await _emailSend.SendOrderConfirmationEmailAsync(
                    orderWithDetails.User.Email,
                    "Orderbevesting",
                    "Bedankt voor jouw bestelling." +
                    "<br/>Bijgevoegd vind je jouw tickets.",
                    pdfStream);
            }
        }
    }
}
