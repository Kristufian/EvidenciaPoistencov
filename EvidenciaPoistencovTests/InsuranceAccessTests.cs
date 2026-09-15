using EvidenciaPoistencov.Data;
using EvidenciaPoistencov.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace EvidenciaPoistencov.Tests
{
    public class InsuranceAccessTests
    {
        [Fact]
        public async Task GetInsuranceDetail_WhenInsuranceBelongsToUser_ReturnsOk()
        {
            var factory = new CustomWebApplicationFactory().WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.AddAuthentication(options =>
                        {
                            options.DefaultAuthenticateScheme = TestAuthHandler.AuthenticationScheme;

                            options.DefaultChallengeScheme = TestAuthHandler.AuthenticationScheme;

                            options.DefaultForbidScheme = TestAuthHandler.AuthenticationScheme;
                        })
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.AuthenticationScheme, options => { });
                    });
                });

            int insuranceId;

            using (var scope = factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var poistenec = new Poistenec
                {
                    Meno = "Test",
                    Priezvisko = "User",
                    Email = "test@user.sk"
                };

                context.Poistenci.Add(poistenec);
                await context.SaveChangesAsync(TestContext.Current.CancellationToken);

                var user = new ApplicationUser
                {
                    Id = "test-user",
                    UserName = "test@user.sk",
                    Email = "test@user.sk",
                    PoistenecId = poistenec.Id
                };

                context.Users.Add(user);

                var insurance = new Poistenie
                {
                    Nazov = "Test insurance",
                    Suma = 10000,
                    PlatnostOd = new DateTime(2026, 1, 1),
                    PlatnostDo = new DateTime(2027, 1, 1),
                    PoistenecId = poistenec.Id
                };

                context.Poistenia.Add(insurance);

                await context.SaveChangesAsync(TestContext.Current.CancellationToken);

                insuranceId = insurance.Id;
            }

            var client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            client.DefaultRequestHeaders.Add("X-Test-Role", "Poistenec");
            client.DefaultRequestHeaders.Add("X-Test-UserId", "test-user");

            var response = await client.GetAsync($"/UdajePoistenca/DetailPoistenia/{insuranceId}", TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task GetInsuranceDetail_WhenInsuranceBelongsToAnotherUser_ReturnsNotFound()
        {
            // Arrange
            var factory = new CustomWebApplicationFactory().WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.AddAuthentication(options =>
                        {
                            options.DefaultAuthenticateScheme = TestAuthHandler.AuthenticationScheme;

                            options.DefaultChallengeScheme = TestAuthHandler.AuthenticationScheme;

                            options.DefaultForbidScheme = TestAuthHandler.AuthenticationScheme;
                        })
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.AuthenticationScheme, options => { });
                    });
                });

            int foreignInsuranceId;

            using (var scope = factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var loggedInPoistenec = new Poistenec
                {
                    Meno = "Logged",
                    Priezvisko = "User",
                    Email = "logged@user.sk"
                };

                var otherPoistenec = new Poistenec
                {
                    Meno = "Other",
                    Priezvisko = "User",
                    Email = "other@user.sk"
                };

                context.Poistenci.AddRange(loggedInPoistenec, otherPoistenec);

                await context.SaveChangesAsync(TestContext.Current.CancellationToken);

                var user = new ApplicationUser
                {
                    Id = "test-user",
                    UserName = "logged@user.sk",
                    Email = "logged@user.sk",
                    PoistenecId = loggedInPoistenec.Id
                };

                context.Users.Add(user);

                var foreignInsurance = new Poistenie
                {
                    Nazov = "Foreign insurance",
                    Suma = 20000,
                    PlatnostOd = new DateTime(2026, 1, 1),
                    PlatnostDo = new DateTime(2027, 1, 1),
                    PoistenecId = otherPoistenec.Id
                };

                context.Poistenia.Add(foreignInsurance);

                await context.SaveChangesAsync(TestContext.Current.CancellationToken);

                foreignInsuranceId = foreignInsurance.Id;
            }

            var client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            client.DefaultRequestHeaders.Add("X-Test-Role", "Poistenec");
            client.DefaultRequestHeaders.Add("X-Test-UserId", "test-user");

            var response = await client.GetAsync($"/UdajePoistenca/DetailPoistenia/{foreignInsuranceId}", TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}