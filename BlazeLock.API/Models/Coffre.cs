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

    public virtual ICollection<Log> Logs { get; set; } = new List<Log>();

    public virtual ICollection<Partage> Partages { get; set; } = new List<Partage>();

    public virtual ICollection<Dossier> IdDossiers { get; set; } = new List<Dossier>();
}
