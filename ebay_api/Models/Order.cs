using System;
using System.Collections.Generic;

namespace ebay_api.Models;

public class Order
{
    public int OrderId { get; set; }
    public string? UserId { get; set; }  // Cambiado a string para coincidir con IdentityUser
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string? ShippingAddress { get; set; }

    public virtual User? User { get; set; }
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}