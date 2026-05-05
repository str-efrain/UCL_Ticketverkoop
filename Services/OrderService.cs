using Domains.Entities;
using Repositories;
using Repositories.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderDAO _orderDAO;

        public OrderService(IOrderDAO orderDAO)
        {
            _orderDAO = orderDAO;
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
    }
}
