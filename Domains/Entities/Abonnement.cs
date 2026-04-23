using System;
using System.Collections.Generic;

namespace Domains.Entities;

public partial class Abonnement
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int ClubId { get; set; }

    public int VakId { get; set; }

    public string Seat { get; set; } = null!;

    public int SeizoenId { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Club Club { get; set; } = null!;

    public virtual Vak Vak { get; set; } = null!;

    public virtual Seizoen Seizoen { get; set; } = null!;
}
