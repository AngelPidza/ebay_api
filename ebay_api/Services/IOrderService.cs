using ebay_api.Models;

namespace ebay_api.Services
{
    public interface IOrderService
    {

        string GetUsernameFromJwt();
        Task<List<Order>> GetOrdersForUserAsync();
        Task<Order> GetOrderByIdAsync(int id);
        Task<Order> CreateOrderAsync(string username, Order model);
        Task UpdateOrderAsync(Order order);
        Task DeleteOrderAsync(int id);
        //Nuevo
    }
}
