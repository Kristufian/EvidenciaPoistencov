using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EvidenciaPoistencov.Models
{
    public class Poistenie : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Názov poistenia je povinný.")]
        [StringLength(100)]
        [Display(Name = "Názov poistenia")]
        public string Nazov { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Predmet poistenia")]
        public string? PredmetPoistenia { get; set; }

        [Required(ErrorMessage = "Poistná suma je povinná.")]
        [Range(0.01, 999999, ErrorMessage = "Suma musí byť väčšia ako 0.")]
        [Precision(18, 2)][Display(Name = "Poistná suma")]
        public decimal Suma { get; set; }

        [Required(ErrorMessage = "Dátum začiatku platnosti je povinný.")]
        [DataType(DataType.Date)]
        [Display(Name = "Platnosť od")]
        public DateTime PlatnostOd { get; set; }

        [Required(ErrorMessage = "Dátum konca platnosti je povinný.")]
        [DataType(DataType.Date)]
        [Display(Name = "Platnosť do")]
        public DateTime? PlatnostDo { get; set; }

        public Poistenec? Poistenec { get; set; }

        [Display(Name = "Poistenec")]
        [Range(1, int.MaxValue, ErrorMessage = "Poistenec je povinný.")]
        public int PoistenecId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (PlatnostDo.HasValue && PlatnostDo.Value < PlatnostOd)
            {
                yield return new ValidationResult("Dátum konca platnosti nemôže byť skorší ako dátum začiatku platnosti.", new[] { nameof(PlatnostDo) });
            }
        }


    }
}