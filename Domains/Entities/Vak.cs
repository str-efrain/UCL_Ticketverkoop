using System;
using System.Collections.Generic;

namespace Domains.Entities;

public partial class Vak
{
    public int Id { get; set; }

    public int StadiumId { get; set; }

    public string Name { get; set; } = null!;

    public int Capacity { get; set; }

    public string? Ring { get; set; }

    public double Price { get; set; }

    public virtual ICollection<Abonnement> Abonnements { get; set; } = new List<Abonnement>();

    public virtual Stadium Stadium { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
