using ebay_api.Models;
using ebay_api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ebay_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            try
            {
                // Obtener el username del usuario autenticado
                var username = User?.Identity?.Name;
                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized("Usuario no autenticado");
                }

                var orders = await _orderService.GetOrdersForUserAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();
            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(Order model)
        {
            var order = await _orderService.CreateOrderAsync(User.Identity.Name, model);
            return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, order);
        }
    }
}
