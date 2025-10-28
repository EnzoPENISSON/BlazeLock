using System;
using System.Collections.Generic;

namespace BlazeLock.API.Models;

public partial class Log
{
    public string IdLog { get; set; } = null!;

    public string? Texte { get; set; }

    public DateTime? Timestamp { get; set; }

    public string IdCoffre { get; set; } = null!;

    public virtual Coffre IdCoffreNavigation { get; set; } = null!;
}
