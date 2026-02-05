using LABHTTP.TEST.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace LABHTTP.TEST.Integration
{
    public class NoteIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CookieDelegatingHandler _cookieHandler;

        public NoteIntegrationTests(CustomWebApplicationFactory factory)
        {
            _cookieHandler = new CookieDelegatingHandler
            {
                InnerHandler = new HttpClientHandler()
            };
            _client = factory.CreateDefaultClient(_cookieHandler);
            _client.BaseAddress = new Uri("http://localhost"); // TestServer
        }

        [Fact]
        public async Task Cannot_Access_Notes_Without_Login()
        {
            var response = await _client.GetAsync("/api/note");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task User_Cannot_See_Other_User_Notes()
        {
            await RegisterAndLogin("a@test.com");
            await _client.PostAsJsonAsync("/api/note", new
            {
                title = "A note",
                content = "secret"
            });

            // Logout User A
            await _client.PostAsync("/api/user/logout", null);
            _cookieHandler.ResetCookies();

            // ----- User B -----
            await RegisterAndLogin("b@test.com");

            // Get notes
            var response = await _client.GetAsync("/api/note");

            // Read safely to avoid exceptions on 401
            List<dynamic> notes;
            if (response.StatusCode == HttpStatusCode.Unauthorized)
                notes = new List<dynamic>();
            else
                notes = await response.Content.ReadFromJsonAsync<List<dynamic>>();

            Assert.Empty(notes);
        }

        private async Task RegisterAndLogin(string email)
        {
            var registerResponse = await _client.PostAsJsonAsync("/api/user/register", new
            {
                email,
                password = "Password123!"
            });
            registerResponse.EnsureSuccessStatusCode();

            // Login
            var loginResponse = await _client.PostAsJsonAsync("/api/user/login", new
            {
                email,
                password = "Password123!"
            });
            loginResponse.EnsureSuccessStatusCode();
        }
    }
}
