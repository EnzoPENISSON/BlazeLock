using System;
using System.Collections.Generic;

namespace BlazeLock.API.Models;

public partial class Utilisateur
{
    public Guid IdUtilisateur { get; set; } = default!;

    public virtual HashSet<Coffre>? Coffres { get; set; }

    public virtual HashSet<Partage>? Partages { get; set; }

    public virtual HashSet<Log>? Logs { get; set; }
}
