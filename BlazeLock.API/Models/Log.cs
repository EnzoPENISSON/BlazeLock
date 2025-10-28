using System;
using System.Collections.Generic;

namespace BlazeLock.API.Models;

public partial class Log
{
    public Guid IdLog { get; set; } = default!;

    public string? Texte { get; set; }

    public DateTime? Timestamp { get; set; }

    public Guid IdCoffre { get; set; } = default!;

    public virtual Coffre IdCoffreNavigation { get; set; } = null!;
}
