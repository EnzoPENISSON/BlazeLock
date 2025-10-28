using System;
using System.Collections.Generic;

namespace BlazeLock.API.Models;

public partial class Utilisateur
{
    public string IdUtilisateur { get; set; } = null!;

    public virtual ICollection<Coffre> Coffres { get; set; } = new List<Coffre>();

    public virtual ICollection<Partage> Partages { get; set; } = new List<Partage>();
}
