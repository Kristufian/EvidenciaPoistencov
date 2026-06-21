using System.ComponentModel.DataAnnotations;

namespace EvidenciaPoistencov.ViewModels
{
    public class PoistenecCreateViewModel
    {
        [Required(ErrorMessage = "Meno je povinné.")]
        public string Meno { get; set; } = string.Empty;

        [Required(ErrorMessage = "Priezvisko je povinné.")]
        public string Priezvisko { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email je povinný.")]
        [EmailAddress(ErrorMessage = "Zadaj platný email.")]
        public string Email { get; set; } = string.Empty;

        public string? Telefon { get; set; }

        public string? Ulica { get; set; }

        public string? Mesto { get; set; }

        public string? PSC { get; set; }

        [Required(ErrorMessage = "Heslo je povinné.")]
        [DataType(DataType.Password)]
        public string Heslo { get; set; } = string.Empty;
    }
}