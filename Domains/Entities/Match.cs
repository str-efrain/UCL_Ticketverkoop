using System;
using System.Collections.Generic;

namespace Domains.Entities;

public partial class Match
{
    public int Id { get; set; }

    public int HomeClubId { get; set; }

    public int AwayClubId { get; set; }

    public DateTime MatchDate { get; set; }

    public virtual Club AwayClub { get; set; } = null!;

    public virtual Club HomeClub { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
