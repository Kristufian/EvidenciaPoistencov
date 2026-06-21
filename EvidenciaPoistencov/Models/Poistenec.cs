using System.ComponentModel.DataAnnotations;

namespace EvidenciaPoistencov.Models
{
    public class Poistenec
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Meno je povinné.")]
        [StringLength(50)]
        public string Meno { get; set; } = string.Empty;

        [Required(ErrorMessage = "Priezvisko je povinné.")]
        [StringLength(50)]
        public string Priezvisko { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email je povinný.")]
        [EmailAddress(ErrorMessage = "Zadaj platný email.")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [StringLength(30)]
        public string? Telefon { get; set; }

        [StringLength(100)]
        public string? Ulica { get; set; }

        [StringLength(100)]
        public string? Mesto { get; set; }

        [StringLength(20)]
        public string? PSC { get; set; }

        public List<Poistenie> Poistenia { get; set; } = new();
    }
}