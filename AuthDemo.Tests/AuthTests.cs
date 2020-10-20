using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AuthDemo.Web;
using AuthDemo.Web.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace AuthDemo.Tests
{
    public class AuthTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

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
            // Arrange
            var loginDetails = new PostLoginDto()
            {
                Username = "test",
                Password = "test"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/auth/login", loginDetails);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetWeatherForecast_Authorize_ShouldSucceed()
        {
            // Act
            var response = await _client.GetAsync("/weatherforecast");

            // Assert
            response.EnsureSuccessStatusCode();
        }
    }
}
