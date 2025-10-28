using System;
using System.Collections.Generic;

namespace BlazeLock.API.Models;

public partial class Coffre
{
    public string IdCoffre { get; set; } = null!;

    public string? Libelle { get; set; }

    public string? HashMasterkey { get; set; }

    public string? Salt { get; set; }

    public string IdUtilisateur { get; set; } = null!;

    public virtual Utilisateur IdUtilisateurNavigation { get; set; } = null!;

    public virtual ICollection<Log> Logs { get; set; } = new List<Log>();

    public virtual ICollection<Partage> Partages { get; set; } = new List<Partage>();

    public virtual ICollection<Dossier> IdDossiers { get; set; } = new List<Dossier>();
}
