namespace BlazeLock.DbLib
{
    public class CoffreDto
    {
        public Guid IdCoffre { get; set; }

        public string? Libelle { get; set; }

        public byte[]? HashMasterkey { get; set; }

        public string? ClearPassword { get; set; }

        public byte[]? Salt { get; set; }

        public Guid IdUtilisateur { get; set; } = default!;
    }
}
