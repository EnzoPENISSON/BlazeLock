using System;
using System.Collections.Generic;

namespace BlazeLock.API.Models;

public partial class Coffre
{
    public Guid IdCoffre { get; set; } = default!;

    public string? Libelle { get; set; }

    public string? HashMasterkey { get; set; }

    public string? Salt { get; set; }

    public Guid IdUtilisateur { get; set; } = default!;

    public virtual Utilisateur IdUtilisateurNavigation { get; set; } = null!;

    public virtual HashSet<Log>? Logs { get; set; }

    public virtual HashSet<Partage>? Partages { get; set; }

    public virtual HashSet<Dossier>? IdDossiers { get; set; }
}
