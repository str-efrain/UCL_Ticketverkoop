using System;
using System.Collections.Generic;

namespace Domains.Entities;

public partial class Stadium
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? City { get; set; }

    public virtual ICollection<Club> Clubs { get; set; } = new List<Club>();

    public virtual ICollection<Vak> Vaks { get; set; } = new List<Vak>();
}
