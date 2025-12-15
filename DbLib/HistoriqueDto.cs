namespace BlazeLock.DbLib
{
    public class HistoriqueDto
    {
        public Guid IdEntree { get; set; }
        public string? Libelle { get; set; }
        public DateTime DateUpdate { get; set; }
        public byte[]? Username { get; set; }
        public byte[]? UsernameTag { get; set; }
        public byte[]? UsernameVi { get; set; }
        public byte[]? Url { get; set; }
        public byte[]? UrlTag { get; set; }
        public byte[]? UrlVi { get; set; }
        public byte[] Password { get; set; } = null!;
        public byte[] PasswordTag { get; set; } = null!;
        public byte[] PasswordVi { get; set; } = null!;
        public byte[]? Commentaire { get; set; }
        public byte[]? CommentaireTag { get; set; }
        public byte[]? CommentaireVi { get; set; }

    }
}
