using LABHTTP.TEST.Integration;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace LABHTTP.TEST.Auth
{
    public class AuthIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public AuthIntegrationTests(CustomWebApplicationFactory factory)
        {
            var cookieHandler = new CookieDelegatingHandler();

            _client = factory.CreateDefaultClient(cookieHandler);
            _client.BaseAddress = new Uri("https://localhost");
        }

        [Fact]
        public async Task Protected_Endpoint_Without_Login_Returns_401()
        {
            var response = await _client.GetAsync("/api/user/me");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Login_Allows_Access_To_Protected_Endpoint()
        {
            await RegisterAndLogin();

            var response = await _client.GetAsync("/api/user/me");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Logout_Removes_Access()
        {
            await RegisterAndLogin();

            await _client.PostAsync("/api/user/logout", null);

            var response = await _client.GetAsync("/api/user/me");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        private async Task RegisterAndLogin()
        {
            await _client.PostAsJsonAsync("/api/user/register", new
            {
                email = "test@test.com",
                password = "Password123!"
            });

            await _client.PostAsJsonAsync("/api/user/login", new
            {
                email = "test@test.com",
                password = "Password123!"
            });
        }
    }
}
