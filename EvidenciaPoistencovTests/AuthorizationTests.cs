using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace EvidenciaPoistencov.Tests
{
    public class AuthorizationTests
    {
        [Fact]
        public async Task GetPoistenci_WhenUserIsNotAuthenticated_RedirectsToLogin()
        {
            var factory = new CustomWebApplicationFactory();

            var client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
            var response = await client.GetAsync("/Poistenci", TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

            Assert.StartsWith("/Identity/Account/Login", response.Headers.Location?.AbsolutePath);
        }

        [Fact]
        public async Task GetPoistenci_WhenUserHasPoistenecRole_ReturnsForbidden()
        {
            var factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.UseEnvironment("Testing");

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

            var client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
            var response = await client.GetAsync(
                "/Poistenci",
                TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetPoistenci_WhenUserHasAdminRole_ReturnsOk()
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

            var client = factory.CreateClient();

            client.DefaultRequestHeaders.Add("X-Test-Role", "Admin");
            var response = await client.GetAsync("/Poistenci", TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetRegister_WhenPublicRegistrationIsDisabled_RedirectsToLogin()
        {
            // Arrange
            var factory = new CustomWebApplicationFactory();

            var client = factory.CreateClient(new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false
                });

            var response = await client.GetAsync("/Identity/Account/Register", TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

            Assert.Equal("/Identity/Account/Login", response.Headers.Location?.OriginalString);
        }
    }
}
