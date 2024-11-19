using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace ebay_api.Models;

public partial class User : IdentityUser
{
    public DateTime RegistrationDate { get; set; }

    // Relaciones de navegación
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual ICollection<WishList> WishLists { get; set; } = new List<WishList>();
}
