using System;
using System.Collections.Generic;

namespace Domains.Entities;

public partial class Club
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? LogoUrl { get; set; }

    public int StadiumId { get; set; }

    public virtual ICollection<Abonnement> Abonnements { get; set; } = new List<Abonnement>();

    public virtual ICollection<Match> MatchAwayClubs { get; set; } = new List<Match>();

    public virtual ICollection<Match> MatchHomeClubs { get; set; } = new List<Match>();

    public virtual Stadium Stadium { get; set; } = null!;
}
