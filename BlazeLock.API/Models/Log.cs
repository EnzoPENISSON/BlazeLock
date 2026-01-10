using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlazeLock.API.Models;

public partial class Log
{
    public Guid IdLog { get; set; } = default!;

    [MaxLength(500)]
    public string? Texte { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.Now;

    // To keep Log entries even if the Coffre is deleted
    public Guid? IdCoffre { get; set; }

    public virtual Coffre Coffre { get; set; } = null!;
    public Guid IdUtilisateur { get; set; }
    public virtual Utilisateur Utilisateur { get; set; } = null!;
}
