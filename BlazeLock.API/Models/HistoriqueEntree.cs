using System;
using System.Collections;
using System.Collections.Generic;

namespace BlazeLock.API.Models;

public partial class HistoriqueEntree
{
    public Guid IdHistorique { get; set; } = default!;

    public byte[] Libelle { get; set; } = null!;

    public byte[] LibelleTag { get; set; } = null!;

    public byte[] LibelleVi { get; set; } = null!;

    public DateTime DateUpdate { get; set; }

    public byte[] Username { get; set; } = null!;

    public byte[] UsernameTag { get; set; } = null!;

    public byte[] UsernameVi { get; set; } = null!;

    public byte[] Url { get; set; } = null!;

    public byte[] UrlTag { get; set; } = null!;

    public byte[] UrlVi { get; set; } = null!;

    public byte[] Password { get; set; } = null!;

    public byte[] PasswordTag { get; set; } = null!;

    public byte[] PasswordVi { get; set; } = null!;

    public byte[] Commentaire { get; set; } = null!;

    public byte[] CommentaireTag { get; set; } = null!;

    public byte[] CommentaireVi { get; set; } = null!;

    public Guid IdEntree { get; set; } = default!;

    public virtual Entree Entree { get; set; } = null!;
}
