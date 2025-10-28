using System;
using System.Collections.Generic;

namespace BlazeLock.API.Models;

public partial class Utilisateur
{
    public Guid IdUtilisateur { get; set; } = default!;

    public virtual ICollection<Coffre> Coffres { get; set; } = new List<Coffre>();

    public virtual ICollection<Partage> Partages { get; set; } = new List<Partage>();
}
