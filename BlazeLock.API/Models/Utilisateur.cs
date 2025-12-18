using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlazeLock.API.Models;

public partial class Utilisateur
{
    public Guid IdUtilisateur { get; set; } = default!;

    [MaxLength(100)]
    public string? email { get; set; }

    public virtual HashSet<Coffre>? Coffres { get; set; }

    public virtual HashSet<Partage>? Partages { get; set; }

    public virtual HashSet<Log>? Logs { get; set; }
}
