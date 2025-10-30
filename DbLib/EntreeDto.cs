namespace BlazeLock.DbLib
{
    public class EntreeDto
    {
        public Guid IdEntree { get; set; }
        public DateTime DateCreation { get; set; }
        public string? Libelle { get; set; }
        public string? LibelleTag { get; set; }
        public string? LibelleVi { get; set; }
        public DateTime DateUpdate { get; set; }
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

    }
}
