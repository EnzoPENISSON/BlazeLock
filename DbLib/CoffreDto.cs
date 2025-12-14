namespace BlazeLock.DbLib
{
    public class CreateCoffreDto
    {
        public byte[] Libelle { get; set; } = null!;

        public byte[] HashMasterkey { get; set; } = null!;

        public Guid IdUtilisateur { get; set; }
    }
}
