
using System.ComponentModel.DataAnnotations;

namespace BlazeLock.FRONT.Components.Forms
{
    public class FolderFormModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Le titre est requis")]
        public string Libelle { get; set; } = "";

    }
}
