using ebay_api.Models;

namespace ebay_api.Repositories
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetOrdersForUserAsync(int userId);
        Task<Order> GetOrderByIdAsync(int id);
        Task CreateOrderAsync(Order order);
        Task UpdateOrderAsync(Order order);
        Task DeleteOrderAsync(Order order);
    }
}
