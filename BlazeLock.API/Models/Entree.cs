using System;
using System.Collections.Generic;

namespace BlazeLock.API.Models;

public partial class Entree
{
    public Guid IdEntree { get; set; } = default!;

    public DateTime DateCreation { get; set; }

    public virtual HashSet<HistoriqueEntree>? HistoriqueEntrees { get; set; }

    public virtual HashSet<Dossier>? IdDossiers { get; set; }
}
