using EvidenciaPoistencov.Models;
using System.ComponentModel.DataAnnotations;

namespace EvidenciaPoistencov.Tests
{
    public class PoistenieTests
    {
        [Fact]
        public void Validate_EndDateIsBeforeStartDate_ReturnsValidationError()
        {
            var poistenie = new Poistenie
            {
                PlatnostOd = new DateTime(2026, 9, 10),
                PlatnostDo = new DateTime(2026, 9, 5)
            };

            var validationContext = new ValidationContext(poistenie);
            var results = poistenie.Validate(validationContext).ToList();
            Assert.Contains(results, result => result.MemberNames.Contains(nameof(Poistenie.PlatnostDo)));
        }

        [Fact]
        public void Validate_EndDateIsAfterStartDate_ReturnsNoValidationError()
        {
            var poistenie = new Poistenie
            {
                PlatnostOd = new DateTime(2026, 9, 5),
                PlatnostDo = new DateTime(2026, 9, 10)
            };

            var validationContext = new ValidationContext(poistenie);
            var results = poistenie.Validate(validationContext).ToList();
            Assert.Empty(results);
        }
        [Fact]
        public void Validate_EndDateEqualsStartDate_ReturnsNoValidationError()
        {
            var poistenie = new Poistenie
            {
                PlatnostOd = new DateTime(2026, 9, 10),
                PlatnostDo = new DateTime(2026, 9, 10)
            };

            var validationContext = new ValidationContext(poistenie);
            var results = poistenie.Validate(validationContext).ToList();
            Assert.Empty(results);
        }
        [Fact]
        public void Validate_InsuranceAmountIsZero_ReturnsValidationError()
        {
            var poistenie = new Poistenie
            {
                Nazov = "Test insurance",
                Suma = 0,
                PlatnostOd = new DateTime(2026, 9, 10),
                PlatnostDo = new DateTime(2026, 9, 20)
            };

            var validationContext = new ValidationContext(poistenie);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(poistenie, validationContext, results, validateAllProperties: true);
            Assert.Contains(results, result => result.MemberNames.Contains(nameof(Poistenie.Suma)));
        }
    }
}
