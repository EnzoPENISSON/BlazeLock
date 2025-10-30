using System;
using System.Collections.Generic;

namespace BlazeLock.API.Models;

public partial class Partage
{
    public Guid IdUtilisateur { get; set; } = default!;

    public Guid IdCoffre { get; set; } = default!;

    public bool IsAdmin { get; set; }

    public virtual Coffre Coffre { get; set; } = null!;

    public virtual Utilisateur Utilisateur { get; set; } = null!;
}
