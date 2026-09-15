using EvidenciaPoistencov.Controllers;
using EvidenciaPoistencov.Data;
using EvidenciaPoistencov.Models;
using EvidenciaPoistencov.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EvidenciaPoistencov.Tests
{
    public class PoistenecCrudTests
    {
        [Fact]
        public async Task CreatePoistenec_WhenDataIsValid_CreatesPoistenecAndUserAccount()
        {
            using var factory = new CustomWebApplicationFactory();
            using var scope = factory.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (!await roleManager.RoleExistsAsync("Poistenec"))
            {
                await roleManager.CreateAsync(new IdentityRole("Poistenec"));
            }

            var controller = new PoistenciController(context, userManager);

            var model = new PoistenecCreateViewModel
            {
                Meno = "Michal",
                Priezvisko = "Novotný",
                Email = "michal.novotny@test.sk",
                Telefon = "0901234567",
                Ulica = "Záborského 1",
                Mesto = "Prešov",
                PSC = "94987",
                Heslo = "Test123!"
            };

            var result = await controller.Create(model);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            var poistenec = await context.Poistenci.SingleAsync(p => p.Email == model.Email, TestContext.Current.CancellationToken);
            var user = await userManager.FindByEmailAsync(model.Email);
            Assert.NotNull(user);
            Assert.Equal(poistenec.Id, user.PoistenecId);
            Assert.True(await userManager.IsInRoleAsync(user, "Poistenec"));
        }

        [Fact]
        public async Task CreatePoistenec_WhenEmailAlreadyExists_DoesNotCreatePoistenec()
        {
            using var factory = new CustomWebApplicationFactory();
            using var scope = factory.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            const string email = "existing@test.sk";
            var existingUser = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var createUserResult = await userManager.CreateAsync(existingUser, "Test123!");
            Assert.True(createUserResult.Succeeded);
            var controller = new PoistenciController(context, userManager);
            var model = new PoistenecCreateViewModel
            {
                Meno = "Michal",
                Priezvisko = "Novotný",
                Email = email,
                Heslo = "Test123!"
            };

            var result = await controller.Create(model);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Same(model, viewResult.Model);
            Assert.False(controller.ModelState.IsValid);
            Assert.True(controller.ModelState.ContainsKey("Email"));
            var poistenecExists = await context.Poistenci.AnyAsync(p => p.Email == email, TestContext.Current.CancellationToken);
            Assert.False(poistenecExists);
        }

        [Fact]
        public async Task DeletePoistenec_WhenPoistenecExists_DeletesUserAndRelatedInsurance()
        {
            using var factory = new CustomWebApplicationFactory();
            using var scope = factory.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var poistenec = new Poistenec
            {
                Meno = "Michal",
                Priezvisko = "Novotný",
                Email = "delete@test.sk"
            };

            context.Poistenci.Add(poistenec);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var user = new ApplicationUser
            {
                UserName = poistenec.Email,
                Email = poistenec.Email,
                EmailConfirmed = true,
                PoistenecId = poistenec.Id
            };

            var createUserResult = await userManager.CreateAsync(user, "Test123!");
            Assert.True(createUserResult.Succeeded);

            var insurance = new Poistenie
            {
                Nazov = "Test insurance",
                Suma = 15000,
                PlatnostOd = new DateTime(2026, 1, 1),
                PlatnostDo = new DateTime(2027, 1, 1),
                PoistenecId = poistenec.Id
            };

            context.Poistenia.Add(insurance);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);
            var controller = new PoistenciController(context, userManager);

            var result = await controller.DeleteConfirmed(poistenec.Id);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            var deletedPoistenec = await context.Poistenci.FindAsync([poistenec.Id], TestContext.Current.CancellationToken);
            Assert.Null(deletedPoistenec);
            var deletedUser = await userManager.FindByEmailAsync(poistenec.Email);
            Assert.Null(deletedUser);
            var insuranceExists = await context.Poistenia.AnyAsync(p => p.Id == insurance.Id, TestContext.Current.CancellationToken);
            Assert.False(insuranceExists);
        }

        [Fact]
        public async Task EditPoistenec_WhenEmailChanges_UpdatesUserEmailAndUserName()
        {
            using var factory = new CustomWebApplicationFactory();
            using var scope = factory.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            const string oldEmail = "old@test.sk";
            const string newEmail = "new@test.sk";

            var poistenec = new Poistenec
            {
                Meno = "Michal",
                Priezvisko = "Novotný",
                Email = oldEmail
            };

            context.Poistenci.Add(poistenec);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var user = new ApplicationUser
            {
                UserName = oldEmail,
                Email = oldEmail,
                EmailConfirmed = true,
                PoistenecId = poistenec.Id
            };

            var createUserResult = await userManager.CreateAsync(user, "Test123!");
            Assert.True(createUserResult.Succeeded);

            var controller = new PoistenciController(context, userManager);

            var editedPoistenec = new Poistenec
            {
                Id = poistenec.Id,
                Meno = "Michal",
                Priezvisko = "Novotný",
                Email = newEmail
            };

            var result = await controller.Edit(poistenec.Id, editedPoistenec);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirectResult.ActionName);
            var updatedPoistenec = await context.Poistenci.FindAsync([poistenec.Id], TestContext.Current.CancellationToken);
            Assert.NotNull(updatedPoistenec);
            Assert.Equal(newEmail, updatedPoistenec.Email);
            var updatedUser = await userManager.FindByEmailAsync(newEmail);
            Assert.NotNull(updatedUser);
            Assert.Equal(newEmail, updatedUser.Email);
            Assert.Equal(newEmail, updatedUser.UserName);
            Assert.Equal(poistenec.Id, updatedUser.PoistenecId);
            var userWithOldEmail = await userManager.FindByEmailAsync(oldEmail);
            Assert.Null(userWithOldEmail);
        }

        [Fact]
        public async Task EditPoistenec_WhenEmailAlreadyExists_DoesNotUpdatePoistenec()
        {
            using var factory = new CustomWebApplicationFactory();
            using var scope = factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            const string originalEmail = "original@test.sk";
            const string existingEmail = "existing@test.sk";

            var poistenec = new Poistenec
            {
                Meno = "Ján",
                Priezvisko = "Novák",
                Email = originalEmail
            };

            context.Poistenci.Add(poistenec);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var user = new ApplicationUser
            {
                UserName = originalEmail,
                Email = originalEmail,
                EmailConfirmed = true,
                PoistenecId = poistenec.Id
            };

            var createUserResult = await userManager.CreateAsync(user, "Test123!");
            Assert.True(createUserResult.Succeeded);

            var existingUser = new ApplicationUser
            {
                UserName = existingEmail,
                Email = existingEmail,
                EmailConfirmed = true
            };

            var createExistingUserResult = await userManager.CreateAsync(existingUser, "Test123!");

            Assert.True(createExistingUserResult.Succeeded);
            var controller = new PoistenciController(context, userManager);

            var editedPoistenec = new Poistenec
            {
                Id = poistenec.Id,
                Meno = "Ján",
                Priezvisko = "Novák",
                Email = existingEmail
            };

            var result = await controller.Edit(poistenec.Id, editedPoistenec);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Same(editedPoistenec, viewResult.Model);
            Assert.False(controller.ModelState.IsValid);
            Assert.True(controller.ModelState.ContainsKey(nameof(Poistenec.Email)));
            var unchangedPoistenec = await context.Poistenci.FindAsync([poistenec.Id], TestContext.Current.CancellationToken);
            Assert.NotNull(unchangedPoistenec);
            Assert.Equal(originalEmail, unchangedPoistenec.Email);
            var unchangedUser = await userManager.FindByEmailAsync(originalEmail);
            Assert.NotNull(unchangedUser);
            Assert.Equal(originalEmail, unchangedUser.Email);
            Assert.Equal(originalEmail, unchangedUser.UserName);
        }

        [Fact]
        public async Task CreatePoistenec_WhenUserCreationFails_RemovesCreatedPoistenec()
        {
            using var factory = new CustomWebApplicationFactory();
            using var scope = factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            const string email = "failed@test.sk";
            var controller = new PoistenciController(context, userManager);

            var model = new PoistenecCreateViewModel
            {
                Meno = "Ján",
                Priezvisko = "Novák",
                Email = email,
                Heslo = "abc"
            };

            var result = await controller.Create(model);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Same(model, viewResult.Model);
            Assert.False(controller.ModelState.IsValid);
            var poistenecExists = await context.Poistenci.AnyAsync(p => p.Email == email, TestContext.Current.CancellationToken);
            Assert.False(poistenecExists);
            var user = await userManager.FindByEmailAsync(email);
            Assert.Null(user);
        }
    }
}