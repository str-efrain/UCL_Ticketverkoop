using System;
using System.Collections.Generic;

namespace Domains.Entities;

public partial class Order
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public DateTime OrderDate { get; set; }

    public double? TotalPrice { get; set; }

    public virtual Abonnement? Abonnement { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
