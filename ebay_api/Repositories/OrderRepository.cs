using Microsoft.EntityFrameworkCore;
using ebay_api.Models;

namespace ebay_api.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly GameWorldContext _context;

        public OrderRepository(GameWorldContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetOrdersForUserAsync(int userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId.ToString())
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToListAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderId == id);
        }

        public async Task CreateOrderAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateOrderAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteOrderAsync(Order order)
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }
    }
}
