namespace BlazeLock.DbLib
{
    public class EntreeHistoriqueDto
    {
        public Guid IdEntree { get; set; }

        public DateTime DateCreation { get; set; }

        public Guid IdDossier { get; set; }

        public HashSet<HistoriqueDto> Historique { get; set; } = new();

    }
}
