using System;
using System.Collections.Generic;

namespace BlazeLock.API.Models;

public partial class Dossier
{
    public Guid IdDossier { get; set; } = default!;

    public string? Libelle { get; set; }

    public Guid? IdDossier1 { get; set; }

    public virtual Dossier? IdDossier1Navigation { get; set; }

    public virtual HashSet<Dossier>? InverseIdDossier1Navigation { get; set; }

    public virtual HashSet<Coffre>? IdCoffres { get; set; }

    public virtual HashSet<Entree>? IdEntrees { get; set; }
}
