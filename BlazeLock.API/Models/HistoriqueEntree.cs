using System;
using System.Collections.Generic;

namespace BlazeLock.API.Models;

public partial class HistoriqueEntree
{
    public string IdHistorique { get; set; } = null!;

    public string? Libelle { get; set; }

    public string? LibelleTag { get; set; }

    public string? LibelleVi { get; set; }

    public string DateUpdate { get; set; } = null!;

    public string? Username { get; set; }

    public string? UsernameTag { get; set; }

    public string? UsernameVi { get; set; }

    public string? Url { get; set; }

    public string? UrlTag { get; set; }

    public string? UrlVi { get; set; }

    public string Password { get; set; } = null!;

    public string PasswordTag { get; set; } = null!;

    public string PasswordVi { get; set; } = null!;

    public string? Commentaire { get; set; }

    public string? CommentaireTag { get; set; }

    public string? CommentaireVi { get; set; }

    public string IdEntree { get; set; } = null!;

    public virtual Entree IdEntreeNavigation { get; set; } = null!;
}
