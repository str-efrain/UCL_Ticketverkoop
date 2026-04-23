using System;
using System.Collections.Generic;

namespace Domains.Entities;

public partial class Seizoen
{
    public int Id { get; set; }

    public int Name { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public virtual ICollection<Abonnement> Abonnements { get; set; } = new List<Abonnement>();
}
