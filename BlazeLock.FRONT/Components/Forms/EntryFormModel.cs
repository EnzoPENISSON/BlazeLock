using System.ComponentModel.DataAnnotations;

namespace BlazeLock.FRONT.Components.Forms
{
    public class EntryFormModel
    {
        [Required(ErrorMessage = "Le titre est requis")]
        public string Libelle { get; set; } = "";

        [Required(ErrorMessage = "Le mot de passe est requis")]
        public string Password { get; set; } = "";

        public string Username { get; set; } = "";
        public string Url { get; set; } = "";
        public string Commentaire { get; set; } = "";
    }
}
