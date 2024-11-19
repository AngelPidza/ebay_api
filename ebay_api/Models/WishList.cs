using System;
using System.Collections.Generic;

namespace ebay_api.Models;

public class WishList
{
    public int WishListId { get; set; }
    public string? UserId { get; set; }  // Cambiado a string para coincidir con IdentityUser
    public int? ProductId { get; set; }
    public DateTime AddedDate { get; set; }

    public virtual Product? Product { get; set; }
    public virtual User? User { get; set; }
}