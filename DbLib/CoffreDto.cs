namespace BlazeLock.DbLib
{
    public class CoffreDto
    {
        public Guid IdCoffre { get; set; }

        public string Libelle { get; set; } = null!;

        public string HashMasterkey { get; set; } = null!;

        public string Salt { get; set; } = null!;

        public Guid IdUtilisateur { get; set; } = default!;
    }
}
