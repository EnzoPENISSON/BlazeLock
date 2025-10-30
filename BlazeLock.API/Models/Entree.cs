using System;
using System.Collections.Generic;

namespace BlazeLock.API.Models;

public partial class Entree
{
    public Guid IdEntree { get; set; } = default!;

    public DateTime DateCreation { get; set; }

    public Guid IdDossier { get; set; }

    public virtual Dossier Dossier { get; set; } = null!;
    public virtual HashSet<HistoriqueEntree> HistoriqueEntrees { get; set; } = null!;
}
