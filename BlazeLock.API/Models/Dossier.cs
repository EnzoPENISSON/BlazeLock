using System;
using System.Collections.Generic;

namespace BlazeLock.API.Models;

public partial class Dossier
{
    public Guid IdDossier { get; set; } = default!;

    public string Libelle { get; set; } = "default";

    public Guid IdCoffre { get; set; }

    public virtual Coffre Coffre { get; set; } = null!;

    public virtual HashSet<Entree>? Entrees { get; set; }
}
