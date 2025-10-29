namespace DbLib
{
    public class PartageDto
    {
        public Guid IdUtilisateur { get; set; }
        public Guid IdCoffre { get; set; }

        public bool IsAdmin { get; set; }
    }
}
