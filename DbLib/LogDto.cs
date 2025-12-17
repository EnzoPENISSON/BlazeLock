namespace BlazeLock.DbLib
{
    public class LogDto
    {
        public string? Texte { get; set; }
        public Guid IdCoffre { get; set; }
        public Guid IdUtilisateur { get; set; }
        public DateTime Timestamp { get; set; }

    }
}
