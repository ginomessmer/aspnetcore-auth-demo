using System;
using AuthDemo.Web;
using AuthDemo.Web.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace AuthDemo.Tests
{
    public class AuthTests : IDisposable
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        private readonly PostLoginDto _loginDetails = new PostLoginDto
        {
            Username = "test",
            Password = "test"
        };

        public AuthTests()
        {
            _server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>()
                .ConfigureAppConfiguration(builder => builder.AddJsonFile("appsettings.json")));

            _client = _server.CreateClient();
        }

        [Fact]
        public async Task PostAuthenticationLogin_Authenticate_ShouldSucceed()
        {
            // Act
            var client = await GetAuthenticatedHttpClientAsync(_client);

            // Assert
            Assert.NotEmpty(client.DefaultRequestHeaders.Authorization.Parameter);
        }

        [Fact]
        public async Task GetWeatherForecast_Authorize_ShouldSucceed()
        {
            // Arrange
            var client = await GetAuthenticatedHttpClientAsync(_client);

            // Act
            var response = await client.GetAsync("/weatherforecast");

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetWeatherForecast_AsAnonymous_ShouldFail()
        {
            // Act
            var response = await _client.GetAsync("/weatherforecast");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }



        // Helpers
        private async Task<HttpClient> GetAuthenticatedHttpClientAsync(HttpClient client)
        {
            // Post
            var response = await _client.PostAsJsonAsync("/auth/login", _loginDetails);

            // Get JWT
            response.EnsureSuccessStatusCode();
            var jwt = await response.Content.ReadAsStringAsync();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
            return client;
        }

        public void Dispose()
        {
            _server?.Dispose();
            _client?.Dispose();
        }
    }
}
