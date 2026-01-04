namespace BlazeLock.DbLib
{
    public class EntreeHistoriqueDto
    {
        public Guid IdEntree { get; set; }

        public DateTime dateCreation { get; set; }

        public Guid IdDossier { get; set; }

        public HashSet<EntreeDto> historique { get; set; } = new();

    }
}
