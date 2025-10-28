using System;
using System.Collections.Generic;

namespace BlazeLock.API.Models;

public partial class Entree
{
    public Guid IdEntree { get; set; } = default!;

    public DateTime? DateCreation { get; set; }

    public virtual ICollection<HistoriqueEntree> HistoriqueEntrees { get; set; } = new List<HistoriqueEntree>();

    public virtual ICollection<Dossier> IdDossiers { get; set; } = new List<Dossier>();
}
