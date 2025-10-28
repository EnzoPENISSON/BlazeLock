using System;
using System.Collections.Generic;

namespace BlazeLock.API.Models;

public partial class Partage
{
    public string IdUtilisateur { get; set; } = null!;

    public string IdCoffre { get; set; } = null!;

    public bool? IsAdmin { get; set; }

    public virtual Coffre IdCoffreNavigation { get; set; } = null!;

    public virtual Utilisateur IdUtilisateurNavigation { get; set; } = null!;
}
