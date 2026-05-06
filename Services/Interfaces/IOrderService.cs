using Domains.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IOrderService : IService<Order>
    {
        Task<List<Order>> GetOrdersByUserIdAsync(string userId);

        Task SendOrderConfirmationAsync(Order order);
    }
}
