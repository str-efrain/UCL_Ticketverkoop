using System;
using System.Collections.Generic;

namespace Domains.Entities;

public partial class Abonnement
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public int ClubId { get; set; }

    public int VakId { get; set; }

    public string Seat { get; set; } = null!;

    public string Seizoen { get; set; } = null!;

    public virtual Club Club { get; set; } = null!;

    public virtual Vak Vak { get; set; } = null!;
}
