using ebay_api.Models;
using ebay_api.Repositories;
using System.IdentityModel.Tokens.Jwt;

namespace ebay_api.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderService(IOrderRepository orderRepository, IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        // Extraer el username del token JWT
        private string GetUsernameFromJwt()
        {
            var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString()?.Replace("Bearer ", "");
            if (string.IsNullOrEmpty(token)) return null;

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var usernameClaim = jwtToken?.Claims?.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email); // O usar "sub" si lo prefieres
            return usernameClaim?.Value;
        }

        public async Task<List<Order>> GetOrdersForUserAsync()
        {
            var username = GetUsernameFromJwt(); // Obtener el email desde el JWT

            if (username == null)
                throw new UnauthorizedAccessException("No se puede autenticar al usuario.");

            var user = await _userRepository.GetUserByEmailAsync(username);
            if (user == null)
                throw new ("Usuario no encontrado.");

            return await _orderRepository.GetOrdersForUserAsync(int.Parse(user.Id));
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            return await _orderRepository.GetOrderByIdAsync(id);
        }

        public async Task<Order> CreateOrderAsync(string username, Order model)
        {
            var user = await _userRepository.GetUserByEmailAsync(username);
            var order = new Order
            {
                UserId = user.Id,
                ShippingAddress = model.ShippingAddress,
                TotalAmount = model.TotalAmount
            };

            await _orderRepository.CreateOrderAsync(order);
            return order;
        }

        public async Task UpdateOrderAsync(Order order)
        {
            await _orderRepository.UpdateOrderAsync(order);
        }

        public async Task DeleteOrderAsync(int id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order != null)
                await _orderRepository.DeleteOrderAsync(order);
        }

        string IOrderService.GetUsernameFromJwt()
        {
            throw new NotImplementedException();
        }
    }
}
