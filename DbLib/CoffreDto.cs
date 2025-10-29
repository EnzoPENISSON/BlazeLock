namespace BlazeLock.DbLib
{
    public class CoffreDto
    {
        public Guid IdCoffre { get; set; }

        public string? Libelle { get; set; }

        public string? HashMasterkey { get; set; }

        public string? Salt { get; set; }

        public Guid IdUtilisateur { get; set; } = default!;
    }
}
