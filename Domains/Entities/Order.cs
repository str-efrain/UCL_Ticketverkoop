using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Domains.Entities;

public partial class Order
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public DateTime OrderDate { get; set; }

    public double? TotalPrice { get; set; }

    public virtual IdentityUser User { get; set; } = null!;

    public virtual ICollection<Abonnement> Abonnements { get; set; } = new List<Abonnement>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
