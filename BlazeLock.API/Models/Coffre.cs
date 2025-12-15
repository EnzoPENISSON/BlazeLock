using System;
using System.Collections;
using System.Collections.Generic;

namespace BlazeLock.API.Models;

public partial class Coffre
{
    public Guid IdCoffre { get; set; } = default!;

    public string Libelle { get; set; } = null!;

    public byte[] HashMasterkey { get; set; } = null!;

    public byte[] Salt { get; set; } = null!;

    public Guid IdUtilisateur { get; set; } = default!;

    public virtual Utilisateur Utilisateur { get; set; } = null!;

    public virtual HashSet<Log>? Logs { get; set; }

    public virtual HashSet<Partage>? Partages { get; set; }

    public virtual HashSet<Dossier> Dossiers { get; set; } = null!;
}
