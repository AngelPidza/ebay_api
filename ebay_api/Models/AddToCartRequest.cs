namespace ebay_api.Models;

public class AddToCartRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; } = 1; // Por defecto, 1
}