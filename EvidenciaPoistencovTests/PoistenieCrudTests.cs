using System;
using System.Collections.Generic;
using System.Text;
using EvidenciaPoistencov.Controllers;
using EvidenciaPoistencov.Data;
using EvidenciaPoistencov.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EvidenciaPoistencov.Tests
{
    public class PoistenieCrudTests
    {
        [Fact]
        public async Task CreateInsurance_WhenStartDateIsInPast_DoesNotCreateInsurance()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            using var scope = factory.Services.CreateScope();

            var context = scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>();

            var poistenec = new Poistenec
            {
                Meno = "Michal",
                Priezvisko = "Novotný",
                Email = "michal@test.sk"
            };

            context.Poistenci.Add(poistenec);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var controller = new PoisteniaController(context);

            var poistenie = new Poistenie
            {
                Nazov = "Test insurance",
                Suma = 10000,
                PlatnostOd = DateTime.Today.AddDays(-1),
                PlatnostDo = DateTime.Today.AddDays(10),
                PoistenecId = poistenec.Id
            };

            var result = await controller.Create(poistenie);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Same(poistenie, viewResult.Model);
            Assert.True(controller.ModelState.ContainsKey(nameof(Poistenie.PlatnostOd)));
            var insuranceExists = await context.Poistenia.AnyAsync(TestContext.Current.CancellationToken);

            Assert.False(insuranceExists);
        }

        [Fact]
        public async Task CreateInsurance_WhenEndDateIsEarlierThanTomorrow_DoesNotCreateInsurance()
        {
            using var factory = new CustomWebApplicationFactory();
            using var scope = factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var poistenec = new Poistenec
            {
                Meno = "Michal",
                Priezvisko = "Novotný",
                Email = "michal2@test.sk"
            };

            context.Poistenci.Add(poistenec);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var controller = new PoisteniaController(context);

            var poistenie = new Poistenie
            {
                Nazov = "Test insurance",
                Suma = 10000,
                PlatnostOd = DateTime.Today,
                PlatnostDo = DateTime.Today,
                PoistenecId = poistenec.Id
            };

            var result = await controller.Create(poistenie);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Same(poistenie, viewResult.Model);
            Assert.True(controller.ModelState.ContainsKey(nameof(Poistenie.PlatnostDo)));
            var insuranceExists = await context.Poistenia.AnyAsync(TestContext.Current.CancellationToken);

            Assert.False(insuranceExists);
        }
    }
}
