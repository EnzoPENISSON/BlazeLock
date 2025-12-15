namespace BlazeLock.DbLib
{
    public class CreateCoffreDto
    {
        public byte[] Libelle { get; set; } = null!;

        public string HashMasterkey { get; set; } = null!;

        public Guid IdUtilisateur { get; set; }
    }
}
