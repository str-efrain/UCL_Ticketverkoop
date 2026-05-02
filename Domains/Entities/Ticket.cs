using System;
using System.Collections.Generic;

namespace Domains.Entities;

public partial class Ticket
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int MatchId { get; set; }

    public int VakId { get; set; }

    public string Seat { get; set; }

    public string? Status { get; set; }

    public virtual Match Match { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;

    public virtual Vak Vak { get; set; } = null!;
}
