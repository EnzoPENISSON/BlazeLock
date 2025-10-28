using System;
using System.Collections.Generic;

namespace BlazeLock.API.Models;

public partial class Dossier
{
    public string IdDossier { get; set; } = null!;

    public string? Libelle { get; set; }

    public string? IdDossier1 { get; set; }

    public virtual Dossier? IdDossier1Navigation { get; set; }

    public virtual ICollection<Dossier> InverseIdDossier1Navigation { get; set; } = new List<Dossier>();

    public virtual ICollection<Coffre> IdCoffres { get; set; } = new List<Coffre>();

    public virtual ICollection<Entree> IdEntrees { get; set; } = new List<Entree>();
}
